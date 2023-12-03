namespace WixBootstrapper_UI
{
    partial class MainDialog
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
            this.install = new System.Windows.Forms.Button();
            this.uninstall = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // install
            // 
            this.install.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.install.AutoSize = true;
            this.install.Enabled = false;
            this.install.Location = new System.Drawing.Point(59, 92);
            this.install.MinimumSize = new System.Drawing.Size(75, 0);
            this.install.Name = "install";
            this.install.Size = new System.Drawing.Size(77, 25);
            this.install.TabIndex = 1;
            this.install.Text = "Install";
            this.install.UseVisualStyleBackColor = true;
            this.install.Click += new System.EventHandler(this.install_Click);
            // 
            // uninstall
            // 
            this.uninstall.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.uninstall.AutoSize = true;
            this.uninstall.Enabled = false;
            this.uninstall.Location = new System.Drawing.Point(142, 92);
            this.uninstall.MinimumSize = new System.Drawing.Size(75, 0);
            this.uninstall.Name = "uninstall";
            this.uninstall.Size = new System.Drawing.Size(77, 25);
            this.uninstall.TabIndex = 2;
            this.uninstall.Text = "Uninstall";
            this.uninstall.UseVisualStyleBackColor = true;
            this.uninstall.Click += new System.EventHandler(this.uninstall_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancel.AutoSize = true;
            this.cancel.Location = new System.Drawing.Point(226, 92);
            this.cancel.MinimumSize = new System.Drawing.Size(75, 0);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(86, 25);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(30, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(282, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Placeholder form";
            // 
            // MainDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 125);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.install);
            this.Controls.Add(this.uninstall);
            this.Controls.Add(this.cancel);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainDialog";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button install;
        private System.Windows.Forms.Button uninstall;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label label2;
    }
}