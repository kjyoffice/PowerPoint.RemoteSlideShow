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
            this.UIOpenDocumentList = new System.Windows.Forms.ListView();
            this.UIOpenDocumentIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UIOpenDocumentName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UIOpenDocumentPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UIRefreshOpenDocumentList = new System.Windows.Forms.Button();
            this.UIStartRemoteSlideShow = new System.Windows.Forms.Button();
            this.UIAboutThisApplication = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UIOpenDocumentList
            // 
            this.UIOpenDocumentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UIOpenDocumentList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.UIOpenDocumentIndex,
            this.UIOpenDocumentName,
            this.UIOpenDocumentPath});
            this.UIOpenDocumentList.FullRowSelect = true;
            this.UIOpenDocumentList.GridLines = true;
            this.UIOpenDocumentList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.UIOpenDocumentList.Location = new System.Drawing.Point(12, 12);
            this.UIOpenDocumentList.MultiSelect = false;
            this.UIOpenDocumentList.Name = "UIOpenDocumentList";
            this.UIOpenDocumentList.Size = new System.Drawing.Size(802, 178);
            this.UIOpenDocumentList.TabIndex = 1;
            this.UIOpenDocumentList.UseCompatibleStateImageBehavior = false;
            this.UIOpenDocumentList.View = System.Windows.Forms.View.Details;
            // 
            // UIOpenDocumentIndex
            // 
            this.UIOpenDocumentIndex.Text = "번호";
            this.UIOpenDocumentIndex.Width = 50;
            // 
            // UIOpenDocumentName
            // 
            this.UIOpenDocumentName.Text = "문서 이름";
            this.UIOpenDocumentName.Width = 250;
            // 
            // UIOpenDocumentPath
            // 
            this.UIOpenDocumentPath.Text = "문서 경로";
            this.UIOpenDocumentPath.Width = 350;
            // 
            // UIRefreshOpenDocumentList
            // 
            this.UIRefreshOpenDocumentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UIRefreshOpenDocumentList.Location = new System.Drawing.Point(12, 196);
            this.UIRefreshOpenDocumentList.Name = "UIRefreshOpenDocumentList";
            this.UIRefreshOpenDocumentList.Size = new System.Drawing.Size(802, 35);
            this.UIRefreshOpenDocumentList.TabIndex = 2;
            this.UIRefreshOpenDocumentList.Text = "새로고침";
            this.UIRefreshOpenDocumentList.UseVisualStyleBackColor = true;
            this.UIRefreshOpenDocumentList.Click += new System.EventHandler(this.UIRefreshOpenDocumentList_Click);
            // 
            // UIStartRemoteSlideShow
            // 
            this.UIStartRemoteSlideShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UIStartRemoteSlideShow.Location = new System.Drawing.Point(12, 237);
            this.UIStartRemoteSlideShow.Name = "UIStartRemoteSlideShow";
            this.UIStartRemoteSlideShow.Size = new System.Drawing.Size(802, 60);
            this.UIStartRemoteSlideShow.TabIndex = 3;
            this.UIStartRemoteSlideShow.Text = "원격 슬라이드 쑈 시작";
            this.UIStartRemoteSlideShow.UseVisualStyleBackColor = true;
            this.UIStartRemoteSlideShow.Click += new System.EventHandler(this.UIStartRemoteSlideShow_Click);
            // 
            // UIAboutThisApplication
            // 
            this.UIAboutThisApplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UIAboutThisApplication.Location = new System.Drawing.Point(12, 303);
            this.UIAboutThisApplication.Name = "UIAboutThisApplication";
            this.UIAboutThisApplication.Size = new System.Drawing.Size(802, 35);
            this.UIAboutThisApplication.TabIndex = 4;
            this.UIAboutThisApplication.Text = "이 프로그램은?";
            this.UIAboutThisApplication.UseVisualStyleBackColor = true;
            this.UIAboutThisApplication.Click += new System.EventHandler(this.UIAboutThisApplication_Click);
            // 
            // MainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 350);
            this.Controls.Add(this.UIAboutThisApplication);
            this.Controls.Add(this.UIRefreshOpenDocumentList);
            this.Controls.Add(this.UIOpenDocumentList);
            this.Controls.Add(this.UIStartRemoteSlideShow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainFrame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "파워포인트 원격 슬라이드 쑈";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrame_FormClosing);
            this.Load += new System.EventHandler(this.MainFrame_Load);
            this.Shown += new System.EventHandler(this.MainFrame_Shown);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView UIOpenDocumentList;
        private System.Windows.Forms.ColumnHeader UIOpenDocumentIndex;
        private System.Windows.Forms.ColumnHeader UIOpenDocumentName;
        private System.Windows.Forms.ColumnHeader UIOpenDocumentPath;
        private System.Windows.Forms.Button UIRefreshOpenDocumentList;
        private System.Windows.Forms.Button UIStartRemoteSlideShow;
        private System.Windows.Forms.Button UIAboutThisApplication;
    }
}

