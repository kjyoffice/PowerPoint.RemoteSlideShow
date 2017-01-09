namespace PowerPoint.RemoteSlideShow.Server
{
    partial class RemoteSlideShow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UIWorkStatus = new System.Windows.Forms.Label();
            this.UIDocumentName = new System.Windows.Forms.Label();
            this.UIConnectURL = new System.Windows.Forms.Label();
            this.UIConnectPassword = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UIWorkStatus
            // 
            this.UIWorkStatus.AutoSize = true;
            this.UIWorkStatus.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.UIWorkStatus.Location = new System.Drawing.Point(12, 123);
            this.UIWorkStatus.Name = "UIWorkStatus";
            this.UIWorkStatus.Size = new System.Drawing.Size(35, 32);
            this.UIWorkStatus.TabIndex = 0;
            this.UIWorkStatus.Text = "-";
            // 
            // UIDocumentName
            // 
            this.UIDocumentName.AutoSize = true;
            this.UIDocumentName.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.UIDocumentName.Location = new System.Drawing.Point(12, 9);
            this.UIDocumentName.Name = "UIDocumentName";
            this.UIDocumentName.Size = new System.Drawing.Size(36, 32);
            this.UIDocumentName.TabIndex = 0;
            this.UIDocumentName.Text = "-";
            // 
            // UIConnectURL
            // 
            this.UIConnectURL.AutoSize = true;
            this.UIConnectURL.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.UIConnectURL.ForeColor = System.Drawing.Color.Blue;
            this.UIConnectURL.Location = new System.Drawing.Point(12, 41);
            this.UIConnectURL.Name = "UIConnectURL";
            this.UIConnectURL.Size = new System.Drawing.Size(217, 32);
            this.UIConnectURL.TabIndex = 0;
            this.UIConnectURL.Text = "Please wait...";
            // 
            // UIConnectPassword
            // 
            this.UIConnectPassword.AutoSize = true;
            this.UIConnectPassword.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.UIConnectPassword.ForeColor = System.Drawing.Color.Blue;
            this.UIConnectPassword.Location = new System.Drawing.Point(12, 82);
            this.UIConnectPassword.Name = "UIConnectPassword";
            this.UIConnectPassword.Size = new System.Drawing.Size(217, 32);
            this.UIConnectPassword.TabIndex = 0;
            this.UIConnectPassword.Text = "Please wait...";
            // 
            // RemoteSlideShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 164);
            this.Controls.Add(this.UIConnectPassword);
            this.Controls.Add(this.UIConnectURL);
            this.Controls.Add(this.UIDocumentName);
            this.Controls.Add(this.UIWorkStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RemoteSlideShow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "슬라이드 쑈 진행";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RemoteSlideShow_FormClosing);
            this.Load += new System.EventHandler(this.RemoteSlideShow_Load);
            this.Shown += new System.EventHandler(this.RemoteSlideShow_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label UIWorkStatus;
        private System.Windows.Forms.Label UIDocumentName;
        private System.Windows.Forms.Label UIConnectURL;
        private System.Windows.Forms.Label UIConnectPassword;

    }
}