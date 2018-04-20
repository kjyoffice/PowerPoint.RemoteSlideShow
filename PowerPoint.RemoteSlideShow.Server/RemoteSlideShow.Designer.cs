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
            this.UIConnectURL = new System.Windows.Forms.Label();
            this.UIConnectPassword = new System.Windows.Forms.Label();
            this.UIWorkStatusBar = new System.Windows.Forms.StatusStrip();
            this.UIWorkStatusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.UIWorkStatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // UIConnectURL
            // 
            this.UIConnectURL.AutoSize = true;
            this.UIConnectURL.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.UIConnectURL.ForeColor = System.Drawing.Color.Blue;
            this.UIConnectURL.Location = new System.Drawing.Point(12, 9);
            this.UIConnectURL.Name = "UIConnectURL";
            this.UIConnectURL.Size = new System.Drawing.Size(217, 32);
            this.UIConnectURL.TabIndex = 0;
            this.UIConnectURL.Text = "Please wait...";
            // 
            // UIConnectPassword
            // 
            this.UIConnectPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UIConnectPassword.AutoSize = true;
            this.UIConnectPassword.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.UIConnectPassword.ForeColor = System.Drawing.Color.Blue;
            this.UIConnectPassword.Location = new System.Drawing.Point(12, 198);
            this.UIConnectPassword.Name = "UIConnectPassword";
            this.UIConnectPassword.Size = new System.Drawing.Size(217, 32);
            this.UIConnectPassword.TabIndex = 0;
            this.UIConnectPassword.Text = "Please wait...";
            // 
            // UIWorkStatusBar
            // 
            this.UIWorkStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UIWorkStatusMessage});
            this.UIWorkStatusBar.Location = new System.Drawing.Point(0, 239);
            this.UIWorkStatusBar.Name = "UIWorkStatusBar";
            this.UIWorkStatusBar.Size = new System.Drawing.Size(696, 22);
            this.UIWorkStatusBar.SizingGrip = false;
            this.UIWorkStatusBar.TabIndex = 1;
            // 
            // UIWorkStatusMessage
            // 
            this.UIWorkStatusMessage.Name = "UIWorkStatusMessage";
            this.UIWorkStatusMessage.Size = new System.Drawing.Size(631, 17);
            this.UIWorkStatusMessage.Spring = true;
            this.UIWorkStatusMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RemoteSlideShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 261);
            this.Controls.Add(this.UIWorkStatusBar);
            this.Controls.Add(this.UIConnectPassword);
            this.Controls.Add(this.UIConnectURL);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RemoteSlideShow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "슬라이드 쑈 진행";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RemoteSlideShow_FormClosing);
            this.Load += new System.EventHandler(this.RemoteSlideShow_Load);
            this.Shown += new System.EventHandler(this.RemoteSlideShow_Shown);
            this.UIWorkStatusBar.ResumeLayout(false);
            this.UIWorkStatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label UIConnectURL;
        private System.Windows.Forms.Label UIConnectPassword;
        private System.Windows.Forms.StatusStrip UIWorkStatusBar;
        private System.Windows.Forms.ToolStripStatusLabel UIWorkStatusMessage;
    }
}