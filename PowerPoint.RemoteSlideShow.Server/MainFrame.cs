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
            PPT.Application pptApp;

            try
            {
                pptApp = (Marshal.GetActiveObject("PowerPoint.Application") as PPT.Application);

                if (pptApp == null)
                {
                    throw new Exception("열려진 파워포인트가 없습니다.");
                }
                else if ((pptApp != null) && (pptApp.Presentations.Count <= 0))
                {
                    throw new Exception("열려진 파워포인트 문서가 없습니다.");
                }
            }
            catch (Exception eEx)
            {
                pptApp = null;
                throw new Exception(("파워포인트 확인 중 오류가 발생했습니다." + Environment.NewLine + "Detail : " + eEx.Message));
            }

            return pptApp;
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
            this.btnRefreshOpenDocumentList_Click(null, null);
        }

        private void btnRefreshOpenDocumentList_Click(object sender, EventArgs e)
        {
            PPT.Application pptApp;
            int i;
            string sErrorMessage;

            this.lvOpenDocumentList.Items.Clear();

            try
            {
                pptApp = this.GetPowerPointObject();

                for (i = 1; i <= pptApp.Presentations.Count; i++)
                {
                    if ((pptApp.Presentations[i].Saved == OfficeCore.MsoTriState.msoTrue) && (pptApp.Presentations[i].Path != String.Empty) && (File.Exists(pptApp.Presentations[i].FullName) == true))
                    {
                        this.lvOpenDocumentList.Items.Add(new ListViewItem(new string[] { i.ToString(), pptApp.Presentations[i].Name, pptApp.Presentations[i].FullName }));
                    }
                }

                if (pptApp.Presentations.Count > this.lvOpenDocumentList.Items.Count)
                {
                    MessageBox.Show("파일로 저장되지 않은 문서는 제외되었습니다.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception eEx)
            {
                sErrorMessage = eEx.Message;
                //MessageBox.Show(("열려진 파워포인트 문서 리스트를 만들지 못했습니다." + Environment.NewLine + "Detail : " + eEx.Message), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartRemoteSlideShow_Click(object sender, EventArgs e)
        {
            string[] sSelectedItem;

            if (this.lvOpenDocumentList.SelectedItems.Count > 0)
            {
                sSelectedItem = this.lvOpenDocumentList.SelectedItems[0].SubItems.Cast<ListViewItem.ListViewSubItem>().Select(((x) => (x.Text))).ToArray();

                if (File.Exists(sSelectedItem[2]) == true)
                {
                    if (
                        MessageBox.Show(
                            (
                                "이 문서로 원격 슬라이드 쑈를 진행하시겠습니까?" + Environment.NewLine + Environment.NewLine +
                                sSelectedItem[1] + Environment.NewLine + Environment.NewLine +
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
                            new XProvider.WorkDelegate.dgGetPowerPointObject(this.GetPowerPointObject),
                            sSelectedItem[1],
                            sSelectedItem[2],
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

        private void tsmiFile_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (MessageBox.Show("프로그램을 종료하시겠습니까", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No);
        }
    }
}
