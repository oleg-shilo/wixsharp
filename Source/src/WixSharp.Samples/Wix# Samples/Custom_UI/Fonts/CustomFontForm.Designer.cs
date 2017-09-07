using WixSharp.Controls;

namespace ConsoleApplication1
{
    partial class CustomFontForm
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
            this.wixLabel2 = new WixSharp.Controls.WixLabel();
            this.wixLabel1 = new WixSharp.Controls.WixLabel();
            this.wixControl1 = new WixSharp.Controls.WixControl();
            this.lineControl1 = new WixSharp.Controls.WixControl();
            this.BackButton = new WixSharp.Controls.WixButton();
            this.NextButton = new WixSharp.Controls.WixButton();
            this.CancelButton = new WixSharp.Controls.WixButton();
            this.wixControl3 = new WixSharp.Controls.WixControl();
            this.wixLabel3 = new WixSharp.Controls.WixLabel();
            this.wixLabel4 = new WixSharp.Controls.WixLabel();
            this.wixLabel5 = new WixSharp.Controls.WixLabel();
            this.SuspendLayout();
            // 
            // wixLabel2
            // 
            this.wixLabel2.BoundProperty = null;
            this.wixLabel2.Hidden = false;
            this.wixLabel2.Id = null;
            this.wixLabel2.Location = new System.Drawing.Point(24, 33);
            this.wixLabel2.Name = "wixLabel2";
            this.wixLabel2.Size = new System.Drawing.Size(335, 20);
            this.wixLabel2.Text = "Please enter the activation code";
            this.wixLabel2.Tooltip = null;
            this.wixLabel2.WixAttributes = "Transparent=yes;\r\n";
            // 
            // wixLabel1
            // 
            this.wixLabel1.BoundProperty = null;
            this.wixLabel1.Hidden = false;
            this.wixLabel1.Id = "Title";
            this.wixLabel1.Location = new System.Drawing.Point(12, 9);
            this.wixLabel1.Name = "wixLabel1";
            this.wixLabel1.Size = new System.Drawing.Size(326, 19);
            this.wixLabel1.Text = "{\\WixUI_Font_Title}Product Activation";
            this.wixLabel1.Tooltip = null;
            this.wixLabel1.WixAttributes = "Transparent=yes;NoPrefix=yes";
            // 
            // wixControl1
            // 
            this.wixControl1.BoundProperty = null;
            this.wixControl1.ControlType = WixSharp.Controls.ControlType.Bitmap;
            this.wixControl1.EmbeddedXML = null;
            this.wixControl1.Hidden = false;
            this.wixControl1.Id = null;
            this.wixControl1.Location = new System.Drawing.Point(0, 1);
            this.wixControl1.Name = "wixControl1";
            this.wixControl1.Size = new System.Drawing.Size(490, 59);
            this.wixControl1.Text = "!(loc.LicenseAgreementDlgBannerBitmap)";
            this.wixControl1.Tooltip = null;
            this.wixControl1.WixAttributes = "TabSkip=no";
            this.wixControl1.WixText = "!(loc.LicenseAgreementDlgBannerBitmap)";
            // 
            // lineControl1
            // 
            this.lineControl1.BoundProperty = null;
            this.lineControl1.ControlType = WixSharp.Controls.ControlType.Line;
            this.lineControl1.EmbeddedXML = null;
            this.lineControl1.Hidden = false;
            this.lineControl1.Id = null;
            this.lineControl1.Location = new System.Drawing.Point(1, 306);
            this.lineControl1.Name = "lineControl1";
            this.lineControl1.Size = new System.Drawing.Size(490, 1);
            this.lineControl1.Tooltip = null;
            this.lineControl1.WixAttributes = "";
            this.lineControl1.WixText = "";
            // 
            // BackButton
            // 
            this.BackButton.BoundProperty = null;
            this.BackButton.Hidden = false;
            this.BackButton.Id = "Back";
            this.BackButton.Location = new System.Drawing.Point(220, 322);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(75, 23);
            this.BackButton.Text = "Back";
            this.BackButton.Tooltip = null;
            this.BackButton.WixAttributes = null;
            this.BackButton.Click += new WixSharp.Controls.ClickHandler(this.BackButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.BoundProperty = null;
            this.NextButton.Hidden = false;
            this.NextButton.Id = "Next";
            this.NextButton.Location = new System.Drawing.Point(307, 322);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 23);
            this.NextButton.Text = "Next";
            this.NextButton.Tooltip = null;
            this.NextButton.WixAttributes = "Default=yes";
            this.NextButton.Click += new WixSharp.Controls.ClickHandler(this.NextButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.BoundProperty = null;
            this.CancelButton.Hidden = false;
            this.CancelButton.Id = "Cancel";
            this.CancelButton.Location = new System.Drawing.Point(403, 322);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.Text = "Cancel";
            this.CancelButton.Tooltip = null;
            this.CancelButton.WixAttributes = null;
            this.CancelButton.Click += new WixSharp.Controls.ClickHandler(this.CancelButton_Click);
            // 
            // wixControl3
            // 
            this.wixControl3.BoundProperty = null;
            this.wixControl3.ControlType = WixSharp.Controls.ControlType.Line;
            this.wixControl3.EmbeddedXML = null;
            this.wixControl3.Hidden = false;
            this.wixControl3.Id = null;
            this.wixControl3.Location = new System.Drawing.Point(0, 61);
            this.wixControl3.Name = "wixControl3";
            this.wixControl3.Size = new System.Drawing.Size(490, 1);
            this.wixControl3.Tooltip = null;
            this.wixControl3.WixAttributes = "";
            this.wixControl3.WixText = "";
            // 
            // wixLabel3
            // 
            this.wixLabel3.BoundProperty = null;
            this.wixLabel3.Hidden = false;
            this.wixLabel3.Id = "Title";
            this.wixLabel3.Location = new System.Drawing.Point(24, 105);
            this.wixLabel3.Name = "wixLabel3";
            this.wixLabel3.Size = new System.Drawing.Size(326, 19);
            this.wixLabel3.Text = "{\\MyRedConsolas}This should be in red and in Consolas";
            this.wixLabel3.Tooltip = null;
            this.wixLabel3.WixAttributes = "Transparent=yes;NoPrefix=yes";
            // 
            // wixLabel4
            // 
            this.wixLabel4.BoundProperty = null;
            this.wixLabel4.Hidden = false;
            this.wixLabel4.Id = "Title";
            this.wixLabel4.Location = new System.Drawing.Point(24, 138);
            this.wixLabel4.Name = "wixLabel4";
            this.wixLabel4.Size = new System.Drawing.Size(326, 19);
            this.wixLabel4.Text = "{\\MyGreenArial}Green Arial text";
            this.wixLabel4.Tooltip = null;
            this.wixLabel4.WixAttributes = "Transparent=yes;NoPrefix=yes";
            // 
            // wixLabel5
            // 
            this.wixLabel5.BoundProperty = null;
            this.wixLabel5.Hidden = false;
            this.wixLabel5.Id = "Title";
            this.wixLabel5.Location = new System.Drawing.Point(24, 172);
            this.wixLabel5.Name = "wixLabel5";
            this.wixLabel5.Size = new System.Drawing.Size(326, 19);
            this.wixLabel5.Tooltip = null;
            this.wixLabel5.WixAttributes = "Transparent=yes;NoPrefix=yes";
            // 
            // ProductActivationForm
            // 
            this.ClientSize = new System.Drawing.Size(490, 358);
            this.Controls.Add(this.wixLabel5);
            this.Controls.Add(this.wixLabel4);
            this.Controls.Add(this.wixLabel3);
            this.Controls.Add(this.wixControl3);
            this.Controls.Add(this.wixLabel2);
            this.Controls.Add(this.wixLabel1);
            this.Controls.Add(this.wixControl1);
            this.Controls.Add(this.lineControl1);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.CancelButton);
            this.Name = "ProductActivationForm";
            this.Text = "[ProductName] Setup";
            this.WixAttributes = "";
            this.ResumeLayout(false);

        }

        #endregion

        new private WixSharp.Controls.WixButton CancelButton;
        private WixSharp.Controls.WixButton BackButton;
        private WixSharp.Controls.WixButton NextButton;
        private WixSharp.Controls.WixControl lineControl1;
        private WixSharp.Controls.WixControl wixControl1;
        private WixSharp.Controls.WixLabel wixLabel1;
        private WixSharp.Controls.WixLabel wixLabel2;
        private WixSharp.Controls.WixControl wixControl3;
        private WixLabel wixLabel3;
        private WixLabel wixLabel4;
        private WixLabel wixLabel5;
    }
}