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
            this.lblWorkStatus = new System.Windows.Forms.Label();
            this.lblDocumentName = new System.Windows.Forms.Label();
            this.lblConnectURL_LAN = new System.Windows.Forms.Label();
            this.lblConnectPassword = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblWorkStatus
            // 
            this.lblWorkStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWorkStatus.AutoSize = true;
            this.lblWorkStatus.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblWorkStatus.Location = new System.Drawing.Point(12, 165);
            this.lblWorkStatus.Name = "lblWorkStatus";
            this.lblWorkStatus.Size = new System.Drawing.Size(35, 32);
            this.lblWorkStatus.TabIndex = 0;
            this.lblWorkStatus.Text = "-";
            // 
            // lblDocumentName
            // 
            this.lblDocumentName.AutoSize = true;
            this.lblDocumentName.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDocumentName.Location = new System.Drawing.Point(12, 9);
            this.lblDocumentName.Name = "lblDocumentName";
            this.lblDocumentName.Size = new System.Drawing.Size(36, 32);
            this.lblDocumentName.TabIndex = 0;
            this.lblDocumentName.Text = "-";
            // 
            // lblConnectURL_LAN
            // 
            this.lblConnectURL_LAN.AutoSize = true;
            this.lblConnectURL_LAN.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblConnectURL_LAN.ForeColor = System.Drawing.Color.Blue;
            this.lblConnectURL_LAN.Location = new System.Drawing.Point(12, 41);
            this.lblConnectURL_LAN.Name = "lblConnectURL_LAN";
            this.lblConnectURL_LAN.Size = new System.Drawing.Size(217, 32);
            this.lblConnectURL_LAN.TabIndex = 0;
            this.lblConnectURL_LAN.Text = "Please wait...";
            // 
            // lblConnectPassword
            // 
            this.lblConnectPassword.AutoSize = true;
            this.lblConnectPassword.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblConnectPassword.ForeColor = System.Drawing.Color.Blue;
            this.lblConnectPassword.Location = new System.Drawing.Point(12, 82);
            this.lblConnectPassword.Name = "lblConnectPassword";
            this.lblConnectPassword.Size = new System.Drawing.Size(217, 32);
            this.lblConnectPassword.TabIndex = 0;
            this.lblConnectPassword.Text = "Please wait...";
            // 
            // RemoteSlideShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1123, 206);
            this.Controls.Add(this.lblConnectPassword);
            this.Controls.Add(this.lblConnectURL_LAN);
            this.Controls.Add(this.lblDocumentName);
            this.Controls.Add(this.lblWorkStatus);
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

        private System.Windows.Forms.Label lblWorkStatus;
        private System.Windows.Forms.Label lblDocumentName;
        private System.Windows.Forms.Label lblConnectURL_LAN;
        private System.Windows.Forms.Label lblConnectPassword;

    }
}