using WixSharp.Controls;

namespace ConsoleApplication1
{
    partial class ProductActivationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductActivationForm));
            this.wixLabel2 = new WixSharp.Controls.WixLabel();
            this.wixLabel1 = new WixSharp.Controls.WixLabel();
            this.wixControl1 = new WixSharp.Controls.WixControl();
            this.lineControl1 = new WixSharp.Controls.WixControl();
            this.wixTextBox1 = new WixSharp.Controls.WixTextBox();
            this.wixCheckBox1 = new WixSharp.Controls.WixCheckBox();
            this.BackButton = new WixSharp.Controls.WixButton();
            this.NextButton = new WixSharp.Controls.WixButton();
            this.CancelButton = new WixSharp.Controls.WixButton();
            this.wixControl2 = new WixSharp.Controls.WixControl();
            this.wixControl3 = new WixSharp.Controls.WixControl();
            this.wixButton = new WixSharp.Controls.WixButton();
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
            // wixTextBox1
            // 
            this.wixTextBox1.BoundProperty = "SERIALNUMBER";
            this.wixTextBox1.Hidden = false;
            this.wixTextBox1.Id = "";
            this.wixTextBox1.Location = new System.Drawing.Point(12, 227);
            this.wixTextBox1.Name = "wixTextBox1";
            this.wixTextBox1.Size = new System.Drawing.Size(436, 20);
            this.wixTextBox1.Tooltip = null;
            this.wixTextBox1.WixAttributes = null;
            // 
            // wixCheckBox1
            // 
            this.wixCheckBox1.BoundProperty = "USE_ACTIVATION";
            this.wixCheckBox1.CheckBoxValue = "1";
            this.wixCheckBox1.Hidden = false;
            this.wixCheckBox1.Id = null;
            this.wixCheckBox1.Location = new System.Drawing.Point(12, 253);
            this.wixCheckBox1.Name = "wixCheckBox1";
            this.wixCheckBox1.Size = new System.Drawing.Size(429, 24);
            this.wixCheckBox1.Text = "Activate the product";
            this.wixCheckBox1.Tooltip = null;
            this.wixCheckBox1.WixAttributes = null;
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
            // wixControl2
            // 
            this.wixControl2.BoundProperty = "LICENCING_MODEL";
            this.wixControl2.ControlType = WixSharp.Controls.ControlType.RadioButtonGroup;
            this.wixControl2.EmbeddedXML = resources.GetString("wixControl2.EmbeddedXML");
            this.wixControl2.Hidden = false;
            this.wixControl2.Id = null;
            this.wixControl2.Location = new System.Drawing.Point(12, 97);
            this.wixControl2.Name = "wixControl2";
            this.wixControl2.Size = new System.Drawing.Size(247, 108);
            this.wixControl2.Text = "Licensing model";
            this.wixControl2.Tooltip = null;
            this.wixControl2.WixAttributes = null;
            this.wixControl2.WixText = "Licensing model";
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
            // wixButton
            // 
            this.wixButton.BoundProperty = null;
            this.wixButton.Hidden = false;
            this.wixButton.Id = "GetLicence";
            this.wixButton.Location = new System.Drawing.Point(328, 97);
            this.wixButton.Name = "wixButton";
            this.wixButton.Size = new System.Drawing.Size(113, 23);
            this.wixButton.Text = "Get Licence Key";
            this.wixButton.Tooltip = "Go to the [ProductName] Web site and claim the licence key";
            this.wixButton.WixAttributes = null;
            this.wixButton.Click += new WixSharp.Controls.ClickHandler(this.wixButton_Click);
            // 
            // ProductActivationForm
            // 
            this.ClientSize = new System.Drawing.Size(490, 358);
            this.Controls.Add(this.wixControl3);
            this.Controls.Add(this.wixControl2);
            this.Controls.Add(this.wixLabel2);
            this.Controls.Add(this.wixLabel1);
            this.Controls.Add(this.wixControl1);
            this.Controls.Add(this.lineControl1);
            this.Controls.Add(this.wixTextBox1);
            this.Controls.Add(this.wixCheckBox1);
            this.Controls.Add(this.wixButton);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.CancelButton);
            this.Name = "ProductActivationForm";
            this.Text = "[ProductName] Setup";
            this.WixAttributes = "";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        new private WixSharp.Controls.WixButton CancelButton;
        private WixSharp.Controls.WixCheckBox wixCheckBox1;
        private WixSharp.Controls.WixButton BackButton;
        private WixSharp.Controls.WixButton NextButton;
        private WixSharp.Controls.WixTextBox wixTextBox1;
        private WixSharp.Controls.WixControl lineControl1;
        private WixSharp.Controls.WixControl wixControl1;
        private WixSharp.Controls.WixLabel wixLabel1;
        private WixSharp.Controls.WixLabel wixLabel2;
        private WixSharp.Controls.WixControl wixControl2;
        private WixSharp.Controls.WixControl wixControl3;
        private WixSharp.Controls.WixButton wixButton;
    }
}