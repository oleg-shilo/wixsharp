namespace WixSharp.UI
{
    partial class MsiSetupForm
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.installBtn = new System.Windows.Forms.Button();
            this.repairBtn = new System.Windows.Forms.Button();
            this.uninstallBtn = new System.Windows.Forms.Button();
            this.setupStatusLbl = new System.Windows.Forms.Label();
            this.productStatusLbl = new System.Windows.Forms.Label();
            this.showLogBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 135);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(371, 17);
            this.progressBar.TabIndex = 0;
            // 
            // installBtn
            // 
            this.installBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.installBtn.Location = new System.Drawing.Point(308, 7);
            this.installBtn.Name = "installBtn";
            this.installBtn.Size = new System.Drawing.Size(75, 23);
            this.installBtn.TabIndex = 1;
            this.installBtn.Text = "&Install";
            this.installBtn.UseVisualStyleBackColor = true;
            this.installBtn.Click += new System.EventHandler(this.installBtn_Click);
            // 
            // repairBtn
            // 
            this.repairBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.repairBtn.Location = new System.Drawing.Point(308, 36);
            this.repairBtn.Name = "repairBtn";
            this.repairBtn.Size = new System.Drawing.Size(75, 23);
            this.repairBtn.TabIndex = 1;
            this.repairBtn.Text = "&Repair";
            this.repairBtn.UseVisualStyleBackColor = true;
            this.repairBtn.Click += new System.EventHandler(this.repairBtn_Click);
            // 
            // uninstallBtn
            // 
            this.uninstallBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uninstallBtn.Location = new System.Drawing.Point(308, 65);
            this.uninstallBtn.Name = "uninstallBtn";
            this.uninstallBtn.Size = new System.Drawing.Size(75, 23);
            this.uninstallBtn.TabIndex = 1;
            this.uninstallBtn.Text = "&Uninstall";
            this.uninstallBtn.UseVisualStyleBackColor = true;
            this.uninstallBtn.Click += new System.EventHandler(this.uninstallBtn_Click);
            // 
            // setupStatusLbl
            // 
            this.setupStatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.setupStatusLbl.AutoSize = true;
            this.setupStatusLbl.Location = new System.Drawing.Point(9, 110);
            this.setupStatusLbl.Name = "setupStatusLbl";
            this.setupStatusLbl.Size = new System.Drawing.Size(0, 13);
            this.setupStatusLbl.TabIndex = 2;
            // 
            // productStatusLbl
            // 
            this.productStatusLbl.AutoSize = true;
            this.productStatusLbl.Location = new System.Drawing.Point(9, 9);
            this.productStatusLbl.Name = "productStatusLbl";
            this.productStatusLbl.Size = new System.Drawing.Size(43, 13);
            this.productStatusLbl.TabIndex = 3;
            this.productStatusLbl.Text = "Status: ";
            // 
            // showLogBtn
            // 
            this.showLogBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showLogBtn.Location = new System.Drawing.Point(308, 94);
            this.showLogBtn.Name = "showLogBtn";
            this.showLogBtn.Size = new System.Drawing.Size(75, 23);
            this.showLogBtn.TabIndex = 1;
            this.showLogBtn.Text = "Show &Log";
            this.showLogBtn.UseVisualStyleBackColor = true;
            this.showLogBtn.Click += new System.EventHandler(this.showLogBtn_Click);
            // 
            // MsiSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 167);
            this.Controls.Add(this.productStatusLbl);
            this.Controls.Add(this.setupStatusLbl);
            this.Controls.Add(this.showLogBtn);
            this.Controls.Add(this.uninstallBtn);
            this.Controls.Add(this.repairBtn);
            this.Controls.Add(this.installBtn);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MsiSetupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MsiSetupForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button installBtn;
        private System.Windows.Forms.Button repairBtn;
        private System.Windows.Forms.Button uninstallBtn;
        private System.Windows.Forms.Label setupStatusLbl;
        private System.Windows.Forms.Label productStatusLbl;
        private System.Windows.Forms.Button showLogBtn;
    }
}