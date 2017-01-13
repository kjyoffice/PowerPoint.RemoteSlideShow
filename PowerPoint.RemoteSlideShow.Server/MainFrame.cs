using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

using OfficeCore = Microsoft.Office.Core;
using PPT = Microsoft.Office.Interop.PowerPoint;

namespace PowerPoint.RemoteSlideShow.Server
{
    public partial class MainFrame : Form
    {
        private PPT.Application GetPowerPointObject()
        {
            PPT.Application ppa;

            try
            {
                ppa = (Marshal.GetActiveObject("PowerPoint.Application") as PPT.Application);

                if (ppa == null)
                {
                    throw new Exception("열려진 파워포인트가 없습니다.");
                }
                else if ((ppa != null) && (ppa.Presentations.Count <= 0))
                {
                    throw new Exception("열려진 파워포인트 문서가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                ppa = null;
                throw new Exception(("파워포인트 확인 중 오류가 발생했습니다." + Environment.NewLine + "Detail : " + ex.Message));
            }

            return ppa;
        }

        // --------------------------------------------------------------------------

        public MainFrame()
        {
            this.InitializeComponent();
            this.Icon = XResource.FormDesign.MainFrame;
        }

        private void MainFrame_Load(object sender, EventArgs e)
        {
            //>
        }

        private void MainFrame_Shown(object sender, EventArgs e)
        {
            this.UIRefreshOpenDocumentList_Click(null, null);
        }

        private void UIRefreshOpenDocumentList_Click(object sender, EventArgs e)
        {
            this.UIOpenDocumentList.Items.Clear();

            try
            {
                PPT.Application ppa = this.GetPowerPointObject();

                for (int i = 1; i <= ppa.Presentations.Count; i++)
                {
                    if ((ppa.Presentations[i].Saved == OfficeCore.MsoTriState.msoTrue) && (ppa.Presentations[i].Path != String.Empty) && (File.Exists(ppa.Presentations[i].FullName) == true))
                    {
                        this.UIOpenDocumentList.Items.Add(new ListViewItem(new string[] { i.ToString(), ppa.Presentations[i].Name, ppa.Presentations[i].FullName }));
                    }
                }

                if (ppa.Presentations.Count > this.UIOpenDocumentList.Items.Count)
                {
                    MessageBox.Show("파일로 저장되지 않은 문서는 제외되었습니다.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // ** ppt가 실행되지 않은 경우 예외가 발생함으로 catch문에 따로 처리 없음
                // 사용하지 않은 변수는 오류 처리한 프로젝트 속성에 따라 오류 방지용 ㅋㅋ
                string errorMessage = ex.Message;
                //MessageBox.Show(("열려진 파워포인트 문서 리스트를 만들지 못했습니다." + Environment.NewLine + "Detail : " + eEx.Message), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UIStartRemoteSlideShow_Click(object sender, EventArgs e)
        {
            if (this.UIOpenDocumentList.SelectedItems.Count > 0)
            {
                string[] selectedItem = this.UIOpenDocumentList.SelectedItems[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(((x) => (x.Text))).ToArray();

                if (File.Exists(selectedItem[2]) == true)
                {
                    if (
                        MessageBox.Show(
                            (
                                "이 문서로 원격 슬라이드 쑈를 진행하시겠습니까?" + Environment.NewLine + Environment.NewLine +
                                selectedItem[1] + Environment.NewLine + Environment.NewLine +
                                "(** 진행 전 파일이 저장됩니다.)" + Environment.NewLine
                            ),
                            this.Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        ) == DialogResult.Yes
                    )
                    {
                        this.Visible = false;
                        new RemoteSlideShow(
                            new XProvider.WorkDelegate.GetPowerPointObject(this.GetPowerPointObject),
                            selectedItem[1],
                            selectedItem[2],
                            delegate()
                            {
                                this.Visible = true;
                            }
                        ).Show();
                    }
                }
                else
                {
                    MessageBox.Show("파워포인트 문서 파일이 존재하지 않습니다.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("선택된 파워포인트 문서가 없습니다.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void MainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (MessageBox.Show("프로그램을 종료하시겠습니까", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No);
        }

        private void UIAboutThisApplication_Click(object sender, EventArgs e)
        {
            MessageBox.Show(XProvider.Value.AssemblyValue.AboutView, this.UIAboutThisApplication.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
