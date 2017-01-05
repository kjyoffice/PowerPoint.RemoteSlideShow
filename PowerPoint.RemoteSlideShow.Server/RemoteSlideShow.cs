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
        private XProvider.WorkDelegate.dgGetPowerPointObject dgGetPPTAppObj { get; set; }
        private XProvider.WorkDelegate.dgSetRestoreMainFrame dgRestoreMainFrame { get; set; }
        private PPT.Application pptApp { get; set; }
        private PPT.Presentation pptPresen { get; set; }
        private PPT.Slides pptSlides { get; set; }
        private List<XProvider.Model.SlideItem> lSlideItem { get; set; }
        private Size szSlide { get; set; }
        private XProvider.Worker.SingleServer ssServer { get; set; }
        private XProvider.TypeValue.SlideShowdModeType ssmtMode { get; set; }
        private string sDocumentName { get; set; }
        private string sDocumentPath { get; set; }
        private string sWorkID { get; set; }
        private string sWorkDirectory { get; set; }

        // --------------------------------------------------------------------------

        private void NotifyErrorAndClose(string sMessage, bool bSlideShowCommandTime)
        {
            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(
                    new XProvider.WorkDelegate.dgUIThreadInvoke(
                        delegate()
                        {
                            this.Activate();
                            this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.Error;
                            this.lblWorkStatus.Text = "오류발생!";

                            if (bSlideShowCommandTime == false)
                            {
                                MessageBox.Show(
                                    (
                                        "아래의 오류로 인해 더 이상 진행할 수 없습니다." + Environment.NewLine + Environment.NewLine +
                                        sMessage + Environment.NewLine + Environment.NewLine +
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
                this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.Error;
                this.lblWorkStatus.Text = "오류발생!";

                if (bSlideShowCommandTime == false)
                {
                    MessageBox.Show(
                        (
                            "아래의 오류로 인해 더 이상 진행할 수 없습니다." + Environment.NewLine + Environment.NewLine +
                            sMessage + Environment.NewLine + Environment.NewLine +
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

        private string CreateWorkID(string sWorkBaseDirectory)
        {
            string sResult = String.Empty;
            string sWorkID;
            int iTryCount = 0;
            int iTryMaxCount = 3;

            while (iTryCount < iTryMaxCount)
            {
                sWorkID = Guid.NewGuid().ToString().Replace("-", String.Empty).ToLower();

                if (Directory.Exists((sWorkBaseDirectory + @"\" + sWorkID)) == false)
                {
                    sResult = sWorkID;
                    iTryCount = (iTryMaxCount + 1);
                }
                else
                {
                    iTryCount++;
                }
            }

            return sResult;
        }

        private void SelectPowerPointDocument()
        {
            string sErrorMessage = String.Empty;
            string sDocumentPath_Upper = this.sDocumentPath.ToUpper();
            int i;

            try
            {
                this.pptApp = this.dgGetPPTAppObj();

                // 지정된 파워포인트 파일 선택
                // ** 그냥 ActivePresentation를 써도 되지만.. 여러개 열려서 이것 저것 바뀔까봐~ (이 코드도 뭐 중간에 닫혀지거나 하면 바뀌겠지만 =_=)
                for (i = 1; i <= this.pptApp.Presentations.Count; i++)
                {
                    // 열린파일 비교 체크
                    if (this.pptApp.Presentations[i].FullName.ToUpper() == sDocumentPath_Upper)
                    {
                        // 프레젠테이션 선택
                        this.pptPresen = this.pptApp.Presentations[i];
                        break;
                    }
                }

                // 열린파일 있는지 체크
                if (this.pptPresen != null)
                {
                    // 프리젠테이션 슬라이드 선택
                    this.pptSlides = this.pptPresen.Slides;

                    if (this.pptSlides != null)
                    {
                        this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.PreSetting;
                        this.lblWorkStatus.Text = "슬라이드 셋팅 진행";
                        this.ExecutePreSetting();
                    }
                    else
                    {
                        throw new Exception("파워포인트 슬라이드 정보가 없습니다.");
                    }
                }
                else
                {
                    throw new Exception(("아래 파일의 파워포인트를 찾을 수 없습니다." + Environment.NewLine + this.sDocumentPath));
                }
            }
            catch (Exception eEx)
            {
                throw new Exception(eEx.Message);
            }
        }

        private void ExecutePreSetting()
        {
            int i;
            string sSlideExportFilePath;

            // 파워포인트가 최대화가 아니면 최대화로 바꿈
            if (this.pptApp.WindowState != PPT.PpWindowState.ppWindowMaximized)
            {
                this.pptApp.WindowState = PPT.PpWindowState.ppWindowMaximized;
            }

            // 파워포인트가 활성화 되어 있지 않으면 활성화
            if (this.pptApp.Active != OfficeCore.MsoTriState.msoTrue)
            {
                this.pptApp.Activate();
            }

            // 파워포인트가 저장되어 있지 않으면 저장
            if (this.pptPresen.Saved != OfficeCore.MsoTriState.msoTrue)
            {
                this.pptPresen.Save();
            }

            if (this.pptSlides.Count > 0)
            {
                // 슬라이드 크기 가져오기
                this.szSlide = new Size((int)this.pptPresen.PageSetup.SlideWidth, (int)this.pptPresen.PageSetup.SlideHeight);

                // 내보내기 파일 저장 디렉토리 구성 및 생성
                if (Directory.Exists(this.sWorkDirectory) == false)
                {
                    Directory.CreateDirectory(this.sWorkDirectory);
                }

                this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.ExportSlide;
                this.lblWorkStatus.Text = "슬라이드 정보 내보내기";

                // 각 슬라이드 스크린샷 내보내기
                for (i = 1; i <= this.pptSlides.Count; i++)
                {
                    // 슬라이드 선택!
                    this.pptSlides[i].Select();

                    // 내보내기 파일 경로
                    sSlideExportFilePath = (this.sWorkDirectory + @"\Slide_" + i + "." + XProvider.Value.MimeTypeValue.sPNGExtension);

                    // 기존파일 존재여부 체크 및 삭제
                    if (File.Exists(sSlideExportFilePath) == true)
                    {
                        File.Delete(sSlideExportFilePath);
                    }

                    // 슬라이드 내보내기!
                    this.pptSlides[i].Export(sSlideExportFilePath, XProvider.Value.MimeTypeValue.sPNGExtension);

                    // 내보내진 슬라이드 저장
                    this.lSlideItem.Add(new XProvider.Model.SlideItem(sSlideExportFilePath, pptSlides[i].NotesPage.Shapes[2].TextFrame.TextRange.Text));
                }

                this.pptSlides[1].Select();
                this.Activate();

                this.ssServer.StartServer(this.sWorkID.Substring(0, 8));
                
                this.lblConnectURL_LAN.Text = ("주소 : " + this.ssServer.sURL);
                this.lblConnectPassword.Text = ("비밀번호 : " + this.ssServer.sConnectPassword);

                this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.Ready;
                this.lblWorkStatus.Text = "원격 슬라이드가 준비됨";
            }
            else
            {
                throw new Exception("구성된 슬라이드 정보가 없습니다.");
            }
        }

        public void DeleteExportSlideAndOrderXML()
        {
            if (this.ssServer != null)
            {
                this.ssServer.StopServer();
            }

            // 슬라이드 파일 삭제
            foreach (XProvider.Model.SlideItem siItem in this.lSlideItem)
            {
                if (File.Exists(siItem.sExportFilePath) == true)
                {
                    File.Delete(siItem.sExportFilePath);
                }
            }

            // 빈 디렉토리면 삭제
            if ((this.sWorkDirectory != String.Empty) && (Directory.Exists(this.sWorkDirectory) == true) && (Directory.GetFiles(this.sWorkDirectory, "*.*", SearchOption.AllDirectories).Length <= 0))
            {
                Directory.Delete(this.sWorkDirectory);
            }
        }

        public bool SlideShowCommand(string[] sComment)
        {
            bool bResult = false;
            string sCommandType;
            int iMoveIndex;

            try
            {
                if ((sComment.Length == 1) || (sComment.Length == 2) && ((sComment[0] == "MOVE") && (Regex.IsMatch(sComment[1], "^[0-9]+$", RegexOptions.IgnoreCase) == true)))
                {
                    sCommandType = sComment[0];
                    iMoveIndex = ((sComment.Length == 2) ? Int32.Parse(sComment[1]) : 0);
                    bResult = true;

                    if (sCommandType == "RUN")
                    {
                        if (this.InvokeRequired == true)
                        {
                            this.BeginInvoke(
                                new XProvider.WorkDelegate.dgUIThreadInvoke(
                                    delegate()
                                    {
                                        try
                                        {
                                            this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.Start;
                                            this.lblWorkStatus.Text = "원격 슬라이드 시작";
                                            this.WindowState = FormWindowState.Minimized;
                                            this.pptPresen.SlideShowSettings.Run();
                                        }
                                        catch (Exception eEx)
                                        {
                                            bResult = false;
                                            this.NotifyErrorAndClose(eEx.Message, true);
                                        }
                                    }
                                )
                            );
                        }
                        else
                        {
                            this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.Start;
                            this.lblWorkStatus.Text = "원격 슬라이드 시작";
                            this.WindowState = FormWindowState.Minimized;
                            this.pptPresen.SlideShowSettings.Run();
                        }
                    }
                    else if (sCommandType == "FIRST")
                    {
                        this.pptApp.SlideShowWindows[1].View.First();
                    }
                    else if (sCommandType == "PREVIOUS")
                    {
                        this.pptApp.SlideShowWindows[1].View.Previous();
                    }
                    else if (sCommandType == "NEXT")
                    {
                        this.pptApp.SlideShowWindows[1].View.Next();
                    }
                    else if (sCommandType == "LAST")
                    {
                        this.pptApp.SlideShowWindows[1].View.Last();
                    }
                    else if ((sCommandType == "MOVE") && ((iMoveIndex >= 1) && (iMoveIndex <= this.lSlideItem.Count)))
                    {
                        this.pptApp.SlideShowWindows[1].View.GotoSlide(iMoveIndex);
                    }
                    else if (sCommandType == "END")
                    {
                        if (this.InvokeRequired == true)
                        {
                            this.BeginInvoke(
                                new XProvider.WorkDelegate.dgUIThreadInvoke(
                                    delegate()
                                    {
                                        try
                                        {
                                            this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.End;
                                            this.lblWorkStatus.Text = "원격 슬라이드 종료";
                                            this.lblConnectURL_LAN.Text = "-";
                                            this.lblConnectPassword.Text = "-";
                                            this.WindowState = FormWindowState.Normal;
                                            this.Activate();
                                        }
                                        catch (Exception eEx)
                                        {
                                            bResult = false;
                                            this.NotifyErrorAndClose(eEx.Message, true);
                                        }
                                    }
                                )
                            );
                        }
                        else
                        {
                            this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.End;
                            this.lblWorkStatus.Text = "원격 슬라이드 종료";
                            this.lblConnectURL_LAN.Text = "-";
                            this.lblConnectPassword.Text = "-";
                            this.WindowState = FormWindowState.Normal;
                            this.Activate();
                        }
                    }
                    else
                    {
                        bResult = false;
                    }
                }
            }
            catch (Exception eEx)
            {
                bResult = false;
                this.NotifyErrorAndClose(eEx.Message, true);
            }

            return bResult;
        }

        // --------------------------------------------------------------------------

        public RemoteSlideShow(
            XProvider.WorkDelegate.dgGetPowerPointObject dgGetPPTAppObj, 
            string sDocumentName, 
            string sDocumentPath,
            XProvider.WorkDelegate.dgSetRestoreMainFrame dgRestoreMainFrame
        )
        {
            this.dgGetPPTAppObj = dgGetPPTAppObj;
            this.sDocumentName = sDocumentName;
            this.sDocumentPath = sDocumentPath;
            this.dgRestoreMainFrame = dgRestoreMainFrame;

            this.pptApp = null;
            this.pptPresen = null;
            this.pptSlides = null;
            this.lSlideItem = new List<XProvider.Model.SlideItem>();
            this.ssServer = new XProvider.Worker.SingleServer(
                XProvider.AppConfig.AppSettings.iSingleServerPortNo,
                XProvider.AppConfig.AppSettings.sSingleServerRootDirectoryName,
                delegate()
                {
                    return this.sWorkID;
                },
                delegate(string sSlideNo)
                {
                    XProvider.Model.SlideItem siResult;
                    int iSlideNo;
                    int iSlideIndex;

                    if ((sSlideNo != String.Empty) && (Regex.IsMatch(sSlideNo, "^[0-9]+$", RegexOptions.IgnoreCase) == true))
                    {
                        iSlideNo = Int32.Parse(sSlideNo);
                        iSlideIndex = (iSlideNo - 1);

                        siResult = (
                            ((iSlideNo >= 1) && (iSlideNo <= this.lSlideItem.Count)) ?
                            (
                                (
                                    (this.lSlideItem[iSlideIndex].sExportFilePath != String.Empty) &&
                                    (File.Exists(this.lSlideItem[iSlideIndex].sExportFilePath) == true)
                                ) ?
                                this.lSlideItem[iSlideIndex] :
                                null
                            ) :
                            null
                        );
                    }
                    else
                    {
                        siResult = null;
                    }

                    return siResult;
                },
                delegate()
                {
                    return this.sDocumentName;
                },
                delegate()
                {
                    return this.szSlide;
                },
                delegate()
                {
                    return this.lSlideItem.Count;
                },
                new XProvider.WorkDelegate.dgSetSlideShowCommand(this.SlideShowCommand),
                delegate()
                {
                    return (this.ssmtMode == XProvider.TypeValue.SlideShowdModeType.Error);
                },
                delegate(string sErrorMessage)
                {
                    this.NotifyErrorAndClose(sErrorMessage, false);
                }
            );
            this.sWorkID = String.Empty;
            this.sWorkDirectory = String.Empty;
            this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.Initialize;

            this.InitializeComponent();
            this.Icon = XResource.FormDesign.MainFrame;
            this.lblDocumentName.Text = sDocumentName;
            this.lblWorkStatus.Text = "초기화";
        }

        private void RemoteSlideShow_Load(object sender, EventArgs e)
        {
            string sWorkBaseDirectory = XProvider.AppConfig.AppSettings.sSlideExportDirectoryPath;

            try
            {
                this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.CreateWorkID;
                this.lblWorkStatus.Text = "Work ID 생성";
                this.sWorkID = this.CreateWorkID(sWorkBaseDirectory);

                if (this.sWorkID != String.Empty)
                {
                    this.sWorkDirectory = (sWorkBaseDirectory + @"\RemoteSlideShow_" + this.sWorkID);
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
                this.ssmtMode = XProvider.TypeValue.SlideShowdModeType.CheckSlide;
                this.lblWorkStatus.Text = "슬라이드 확인";
                this.SelectPowerPointDocument();
            }
            catch (Exception eEx)
            {
                this.NotifyErrorAndClose(eEx.Message, false);
            }
        }

        private void RemoteSlideShow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ssmtMode == XProvider.TypeValue.SlideShowdModeType.Start)
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
            else if (this.ssmtMode == XProvider.TypeValue.SlideShowdModeType.Ready)
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
                this.dgRestoreMainFrame();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
