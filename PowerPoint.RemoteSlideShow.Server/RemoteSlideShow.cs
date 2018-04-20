using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

using OfficeCore = Microsoft.Office.Core;
using PPT = Microsoft.Office.Interop.PowerPoint;

namespace PowerPoint.RemoteSlideShow.Server
{
    public partial class RemoteSlideShow : Form
    {
        private Func<PPT.Application> PPTAppObject { get; set; }
        private Action RestoreMainFrame { get; set; }
        private PPT.Application PPTApp { get; set; }
        private PPT.Presentation PPTPresentation { get; set; }
        private PPT.Slides PPTSlides { get; set; }
        private List<XProvider.Model.SlideItem> SlideItem { get; set; }
        private Size SlideSize { get; set; }
        private XProvider.Worker.SingleServer WebServer { get; set; }
        private XProvider.TypeValue.SlideShowdModeType SlideShowMode { get; set; }
        private string DocumentName { get; set; }
        private string DocumentPath { get; set; }
        private string WorkID { get; set; }
        private string WorkDirectory { get; set; }

        // --------------------------------------------------------------------------

        private void NotifyErrorAndClose(string message, bool slideShowCommandTime)
        {
            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(
                    new Action(
                        delegate()
                        {
                            this.Activate();
                            this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.Error;
                            this.UIWorkStatusMessage.Text = "오류발생!";

                            if (slideShowCommandTime == false)
                            {
                                MessageBox.Show(
                                    (
                                        "아래의 오류로 인해 더 이상 진행할 수 없습니다." + Environment.NewLine + Environment.NewLine +
                                        message + Environment.NewLine + Environment.NewLine +
                                        "원격 슬라이드 진행을 종료합니다."
                                    ),
                                    this.Text,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );

                                this.Close();
                            }
                        }
                    )
                );
            }
            else
            {
                this.Activate();
                this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.Error;
                this.UIWorkStatusMessage.Text = "오류발생!";

                if (slideShowCommandTime == false)
                {
                    MessageBox.Show(
                        (
                            "아래의 오류로 인해 더 이상 진행할 수 없습니다." + Environment.NewLine + Environment.NewLine +
                            message + Environment.NewLine + Environment.NewLine +
                            "원격 슬라이드 진행을 종료합니다."
                        ),
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    this.Close();
                }
            }
        }

        private string CreateWorkID(string workBaseDirectory)
        {
            string result = String.Empty;
            int tryCount = 0;
            int tryMaxCount = 3;

            while (tryCount < tryMaxCount)
            {
                string workID = Guid.NewGuid().ToString().Replace("-", String.Empty).ToLower();

                if (Directory.Exists((workBaseDirectory + @"\" + workID)) == false)
                {
                    result = workID;
                    tryCount = (tryMaxCount + 1);
                }
                else
                {
                    tryCount++;
                }
            }

            return result;
        }

        private void SelectPowerPointDocument()
        {
            try
            {
                this.PPTApp = this.PPTAppObject();

                // 지정된 파워포인트 파일 선택
                // ** 그냥 ActivePresentation를 써도 되지만.. 여러개 열려서 이것 저것 바뀔까봐~ (이 코드도 뭐 중간에 닫혀지거나 하면 바뀌겠지만 =_=)
                for (int i = 1; i <= this.PPTApp.Presentations.Count; i++)
                {
                    // 열린파일 비교 체크
                    if (this.PPTApp.Presentations[i].FullName.ToUpper() == this.DocumentPath.ToUpper())
                    {
                        // 프레젠테이션 선택
                        this.PPTPresentation = this.PPTApp.Presentations[i];
                        break;
                    }
                }

                // 열린파일 있는지 체크
                if (this.PPTPresentation != null)
                {
                    // 프리젠테이션 슬라이드 선택
                    this.PPTSlides = this.PPTPresentation.Slides;

                    if (this.PPTSlides != null)
                    {
                        this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.PreSetting;
                        this.UIWorkStatusMessage.Text = "슬라이드 셋팅 진행";
                        this.ExecutePreSetting();
                    }
                    else
                    {
                        throw new Exception("파워포인트 슬라이드 정보가 없습니다.");
                    }
                }
                else
                {
                    throw new Exception(("아래 파일의 파워포인트를 찾을 수 없습니다." + Environment.NewLine + this.DocumentPath));
                }
            }
            catch (Exception eEx)
            {
                throw new Exception(eEx.Message);
            }
        }

        private void ExecutePreSetting()
        {
            // 파워포인트가 최대화가 아니면 최대화로 바꿈
            if (this.PPTApp.WindowState != PPT.PpWindowState.ppWindowMaximized)
            {
                this.PPTApp.WindowState = PPT.PpWindowState.ppWindowMaximized;
            }

            // 파워포인트가 활성화 되어 있지 않으면 활성화
            if (this.PPTApp.Active != OfficeCore.MsoTriState.msoTrue)
            {
                this.PPTApp.Activate();
            }

            // 파워포인트가 저장되어 있지 않으면 저장
            if (this.PPTPresentation.Saved != OfficeCore.MsoTriState.msoTrue)
            {
                this.PPTPresentation.Save();
            }

            if (this.PPTSlides.Count > 0)
            {
                // 슬라이드 크기 가져오기
                this.SlideSize = new Size((int)this.PPTPresentation.PageSetup.SlideWidth, (int)this.PPTPresentation.PageSetup.SlideHeight);

                // 내보내기 파일 저장 디렉토리 구성 및 생성
                if (Directory.Exists(this.WorkDirectory) == false)
                {
                    Directory.CreateDirectory(this.WorkDirectory);
                }

                this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.ExportSlide;
                this.UIWorkStatusMessage.Text = "슬라이드 정보 내보내기";

                // 각 슬라이드 스크린샷 내보내기
                for (int i = 1; i <= this.PPTSlides.Count; i++)
                {
                    // 슬라이드 선택!
                    this.PPTSlides[i].Select();

                    // 내보내기 파일 경로
                    string slideExportFilePath = (this.WorkDirectory + @"\Slide_" + i + "." + XProvider.Value.MimeTypeValue.PNGExtension);

                    // 기존파일 존재여부 체크 및 삭제
                    if (File.Exists(slideExportFilePath) == true)
                    {
                        File.Delete(slideExportFilePath);
                    }

                    // 슬라이드 내보내기!
                    this.PPTSlides[i].Export(slideExportFilePath, XProvider.Value.MimeTypeValue.PNGExtension);

                    // 내보내진 슬라이드 저장
                    this.SlideItem.Add(new XProvider.Model.SlideItem(slideExportFilePath, PPTSlides[i].NotesPage.Shapes[2].TextFrame.TextRange.Text));
                }

                this.PPTSlides[1].Select();
                this.Activate();

                this.WebServer.StartServer(this.WorkID.Substring(0, 8));
                
                this.UIConnectURL.Text = String.Join(Environment.NewLine, this.WebServer.ConnectURL);
                this.UIConnectPassword.Text = ("비밀번호 : " + this.WebServer.ConnectPassword);

                this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.Ready;
                this.UIWorkStatusMessage.Text = "원격 슬라이드가 준비됨";
            }
            else
            {
                throw new Exception("구성된 슬라이드 정보가 없습니다.");
            }
        }

        public void DeleteExportSlideAndOrderXML()
        {
            if (this.WebServer != null)
            {
                this.WebServer.StopServer();
            }

            // 슬라이드 파일 삭제
            foreach (XProvider.Model.SlideItem siItem in this.SlideItem)
            {
                if (File.Exists(siItem.ExportFilePath) == true)
                {
                    File.Delete(siItem.ExportFilePath);
                }
            }

            // 빈 디렉토리면 삭제
            if ((this.WorkDirectory != String.Empty) && (Directory.Exists(this.WorkDirectory) == true) && (Directory.GetFiles(this.WorkDirectory, "*.*", SearchOption.AllDirectories).Length <= 0))
            {
                Directory.Delete(this.WorkDirectory);
            }
        }

        public bool SlideShowCommand(string[] comment)
        {
            bool result = false;

            try
            {
                if ((comment.Length == 1) || (comment.Length == 2) && ((comment[0] == "MOVE") && (Regex.IsMatch(comment[1], "^[0-9]+$", RegexOptions.IgnoreCase) == true)))
                {
                    string commandType = comment[0];
                    int moveIndex = ((comment.Length == 2) ? Int32.Parse(comment[1]) : 0);
                    result = true;

                    if (commandType == "RUN")
                    {
                        if (this.InvokeRequired == true)
                        {
                            this.BeginInvoke(
                                new Action(
                                    delegate()
                                    {
                                        try
                                        {
                                            this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.Start;
                                            this.UIWorkStatusMessage.Text = "원격 슬라이드 시작";
                                            this.WindowState = FormWindowState.Minimized;
                                            this.PPTPresentation.SlideShowSettings.Run();
                                        }
                                        catch (Exception eEx)
                                        {
                                            result = false;
                                            this.NotifyErrorAndClose(eEx.Message, true);
                                        }
                                    }
                                )
                            );
                        }
                        else
                        {
                            this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.Start;
                            this.UIWorkStatusMessage.Text = "원격 슬라이드 시작";
                            this.WindowState = FormWindowState.Minimized;
                            this.PPTPresentation.SlideShowSettings.Run();
                        }
                    }
                    else if (commandType == "FIRST")
                    {
                        this.PPTApp.SlideShowWindows[1].View.First();
                    }
                    else if (commandType == "PREVIOUS")
                    {
                        this.PPTApp.SlideShowWindows[1].View.Previous();
                    }
                    else if (commandType == "NEXT")
                    {
                        this.PPTApp.SlideShowWindows[1].View.Next();
                    }
                    else if (commandType == "LAST")
                    {
                        this.PPTApp.SlideShowWindows[1].View.Last();
                    }
                    else if ((commandType == "MOVE") && ((moveIndex >= 1) && (moveIndex <= this.SlideItem.Count)))
                    {
                        this.PPTApp.SlideShowWindows[1].View.GotoSlide(moveIndex);
                    }
                    else if (commandType == "END")
                    {
                        if (this.InvokeRequired == true)
                        {
                            this.BeginInvoke(
                                new Action(
                                    delegate()
                                    {
                                        try
                                        {
                                            this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.End;
                                            this.UIWorkStatusMessage.Text = "원격 슬라이드 종료";
                                            this.UIConnectURL.Text = "-";
                                            this.UIConnectPassword.Text = "-";
                                            this.WindowState = FormWindowState.Normal;
                                            this.Activate();
                                        }
                                        catch (Exception eEx)
                                        {
                                            result = false;
                                            this.NotifyErrorAndClose(eEx.Message, true);
                                        }
                                    }
                                )
                            );
                        }
                        else
                        {
                            this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.End;
                            this.UIWorkStatusMessage.Text = "원격 슬라이드 종료";
                            this.UIConnectURL.Text = "-";
                            this.UIConnectPassword.Text = "-";
                            this.WindowState = FormWindowState.Normal;
                            this.Activate();
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception eEx)
            {
                result = false;
                this.NotifyErrorAndClose(eEx.Message, true);
            }

            return result;
        }

        // --------------------------------------------------------------------------

        public RemoteSlideShow(
            Func<PPT.Application> pptAppObject, 
            string documentName, 
            string documentPath,
            Action restoreMainFrame
        )
        {
            this.PPTAppObject = pptAppObject;
            this.DocumentName = documentName;
            this.DocumentPath = documentPath;
            this.RestoreMainFrame = restoreMainFrame;

            this.PPTApp = null;
            this.PPTPresentation = null;
            this.PPTSlides = null;
            this.SlideItem = new List<XProvider.Model.SlideItem>();
            this.WebServer = new XProvider.Worker.SingleServer(
                XProvider.AppConfig.AppSettings.SingleServerPortNo,
                XProvider.AppConfig.AppSettings.SingleServerRootDirectoryName,
                delegate()
                {
                    return this.WorkID;
                },
                delegate(string sSlideNo)
                {
                    XProvider.Model.SlideItem result;

                    if ((sSlideNo != String.Empty) && (Regex.IsMatch(sSlideNo, "^[0-9]+$", RegexOptions.IgnoreCase) == true))
                    {
                        int slideNo = Int32.Parse(sSlideNo);
                        int slideIndex = (slideNo - 1);

                        result = (
                            ((slideNo >= 1) && (slideNo <= this.SlideItem.Count)) ?
                            (
                                (
                                    (this.SlideItem[slideIndex].ExportFilePath != String.Empty) &&
                                    (File.Exists(this.SlideItem[slideIndex].ExportFilePath) == true)
                                ) ?
                                this.SlideItem[slideIndex] :
                                null
                            ) :
                            null
                        );
                    }
                    else
                    {
                        result = null;
                    }

                    return result;
                },
                delegate()
                {
                    return this.DocumentName;
                },
                delegate()
                {
                    return this.SlideSize;
                },
                delegate()
                {
                    return this.SlideItem.Count;
                },
                this.SlideShowCommand,
                delegate()
                {
                    return (this.SlideShowMode == XProvider.TypeValue.SlideShowdModeType.Error);
                },
                delegate(string errorMessage)
                {
                    this.NotifyErrorAndClose(errorMessage, false);
                }
            );
            this.WorkID = String.Empty;
            this.WorkDirectory = String.Empty;
            this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.Initialize;

            this.InitializeComponent();
            this.Icon = XResource.FormDesign.MainFrame;
            this.Text += (" - " + documentName);
            this.UIWorkStatusMessage.Text = "초기화";
        }

        private void RemoteSlideShow_Load(object sender, EventArgs e)
        {
            try
            {
                this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.CreateWorkID;
                this.UIWorkStatusMessage.Text = "Work ID 생성";
                this.WorkID = this.CreateWorkID(XProvider.AppConfig.AppSettings.SlideExportDirectoryPath);

                if (this.WorkID != String.Empty)
                {
                    this.WorkDirectory = (XProvider.AppConfig.AppSettings.SlideExportDirectoryPath + @"\RemoteSlideShow_" + this.WorkID);
                }
                else
                {
                    throw new Exception("Work ID를 생성하지 못했습니다.");
                }
            }
            catch (Exception eEx)
            {
                this.NotifyErrorAndClose(eEx.Message, false);
            }
        }

        private void RemoteSlideShow_Shown(object sender, EventArgs e)
        {
            try
            {
                this.SlideShowMode = XProvider.TypeValue.SlideShowdModeType.CheckSlide;
                this.UIWorkStatusMessage.Text = "슬라이드 확인";
                this.SelectPowerPointDocument();
            }
            catch (Exception eEx)
            {
                this.NotifyErrorAndClose(eEx.Message, false);
            }
        }

        private void RemoteSlideShow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.SlideShowMode == XProvider.TypeValue.SlideShowdModeType.Start)
            {
                e.Cancel = (
                        MessageBox.Show(
                        (
                            "현재 슬라이드 쑈가 진행중입니다." + Environment.NewLine +
                            "이대로 슬라이드 쑈를 중단하면 원격 제어가 종료됩니다." + Environment.NewLine + Environment.NewLine +
                            "슬라이드 쑈를 중단하시겠습니까?"
                        ),
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.No
                );
            }
            else if (this.SlideShowMode == XProvider.TypeValue.SlideShowdModeType.Ready)
            {
                e.Cancel = (
                        MessageBox.Show(
                        "슬라이드 쑈를 중단하시겠습니까?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    ) == DialogResult.No
                );
            }

            if (e.Cancel == false)
            {
                this.DeleteExportSlideAndOrderXML();
                this.RestoreMainFrame();
            }
        }
    }
}
