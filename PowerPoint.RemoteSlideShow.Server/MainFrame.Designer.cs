namespace PowerPoint.RemoteSlideShow.Server
{
    partial class MainFrame
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblOpenDocumentList = new System.Windows.Forms.Label();
            this.lvOpenDocumentList = new System.Windows.Forms.ListView();
            this.chOpenDocumentIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chOpenDocumentName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chOpenDocumentPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRefreshOpenDocumentList = new System.Windows.Forms.Button();
            this.btnStartRemoteSlideShow = new System.Windows.Forms.Button();
            this.msMainMenu = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFile_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.msMainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblOpenDocumentList
            // 
            this.lblOpenDocumentList.AutoSize = true;
            this.lblOpenDocumentList.Location = new System.Drawing.Point(12, 24);
            this.lblOpenDocumentList.Name = "lblOpenDocumentList";
            this.lblOpenDocumentList.Size = new System.Drawing.Size(133, 12);
            this.lblOpenDocumentList.TabIndex = 0;
            this.lblOpenDocumentList.Text = "열려진 파워포인트 문서";
            // 
            // lvOpenDocumentList
            // 
            this.lvOpenDocumentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvOpenDocumentList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chOpenDocumentIndex,
            this.chOpenDocumentName,
            this.chOpenDocumentPath});
            this.lvOpenDocumentList.FullRowSelect = true;
            this.lvOpenDocumentList.GridLines = true;
            this.lvOpenDocumentList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvOpenDocumentList.Location = new System.Drawing.Point(12, 39);
            this.lvOpenDocumentList.MultiSelect = false;
            this.lvOpenDocumentList.Name = "lvOpenDocumentList";
            this.lvOpenDocumentList.Size = new System.Drawing.Size(802, 163);
            this.lvOpenDocumentList.TabIndex = 1;
            this.lvOpenDocumentList.UseCompatibleStateImageBehavior = false;
            this.lvOpenDocumentList.View = System.Windows.Forms.View.Details;
            // 
            // chOpenDocumentIndex
            // 
            this.chOpenDocumentIndex.Text = "번호";
            this.chOpenDocumentIndex.Width = 50;
            // 
            // chOpenDocumentName
            // 
            this.chOpenDocumentName.Text = "문서 이름";
            this.chOpenDocumentName.Width = 250;
            // 
            // chOpenDocumentPath
            // 
            this.chOpenDocumentPath.Text = "문서 경로";
            this.chOpenDocumentPath.Width = 350;
            // 
            // btnRefreshOpenDocumentList
            // 
            this.btnRefreshOpenDocumentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshOpenDocumentList.Location = new System.Drawing.Point(12, 208);
            this.btnRefreshOpenDocumentList.Name = "btnRefreshOpenDocumentList";
            this.btnRefreshOpenDocumentList.Size = new System.Drawing.Size(802, 46);
            this.btnRefreshOpenDocumentList.TabIndex = 2;
            this.btnRefreshOpenDocumentList.Text = "새로고침";
            this.btnRefreshOpenDocumentList.UseVisualStyleBackColor = true;
            this.btnRefreshOpenDocumentList.Click += new System.EventHandler(this.btnRefreshOpenDocumentList_Click);
            // 
            // btnStartRemoteSlideShow
            // 
            this.btnStartRemoteSlideShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartRemoteSlideShow.Location = new System.Drawing.Point(12, 260);
            this.btnStartRemoteSlideShow.Name = "btnStartRemoteSlideShow";
            this.btnStartRemoteSlideShow.Size = new System.Drawing.Size(802, 78);
            this.btnStartRemoteSlideShow.TabIndex = 3;
            this.btnStartRemoteSlideShow.Text = "원격 슬라이드 쑈 시작";
            this.btnStartRemoteSlideShow.UseVisualStyleBackColor = true;
            this.btnStartRemoteSlideShow.Click += new System.EventHandler(this.btnStartRemoteSlideShow_Click);
            // 
            // msMainMenu
            // 
            this.msMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile});
            this.msMainMenu.Location = new System.Drawing.Point(0, 0);
            this.msMainMenu.Name = "msMainMenu";
            this.msMainMenu.Size = new System.Drawing.Size(826, 24);
            this.msMainMenu.TabIndex = 4;
            this.msMainMenu.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile_Exit});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(57, 20);
            this.tsmiFile.Text = "파일(&F)";
            // 
            // tsmiFile_Exit
            // 
            this.tsmiFile_Exit.Name = "tsmiFile_Exit";
            this.tsmiFile_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.tsmiFile_Exit.Size = new System.Drawing.Size(168, 22);
            this.tsmiFile_Exit.Text = "끝내기(&X)";
            this.tsmiFile_Exit.Click += new System.EventHandler(this.tsmiFile_Exit_Click);
            // 
            // MainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 350);
            this.Controls.Add(this.btnStartRemoteSlideShow);
            this.Controls.Add(this.btnRefreshOpenDocumentList);
            this.Controls.Add(this.lvOpenDocumentList);
            this.Controls.Add(this.lblOpenDocumentList);
            this.Controls.Add(this.msMainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.msMainMenu;
            this.MaximizeBox = false;
            this.Name = "MainFrame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "파워포인트 원격 슬라이드 쑈";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrame_FormClosing);
            this.Load += new System.EventHandler(this.MainFrame_Load);
            this.Shown += new System.EventHandler(this.MainFrame_Shown);
            this.msMainMenu.ResumeLayout(false);
            this.msMainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOpenDocumentList;
        private System.Windows.Forms.ListView lvOpenDocumentList;
        private System.Windows.Forms.ColumnHeader chOpenDocumentIndex;
        private System.Windows.Forms.ColumnHeader chOpenDocumentName;
        private System.Windows.Forms.ColumnHeader chOpenDocumentPath;
        private System.Windows.Forms.Button btnRefreshOpenDocumentList;
        private System.Windows.Forms.Button btnStartRemoteSlideShow;
        private System.Windows.Forms.MenuStrip msMainMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile_Exit;
    }
}

