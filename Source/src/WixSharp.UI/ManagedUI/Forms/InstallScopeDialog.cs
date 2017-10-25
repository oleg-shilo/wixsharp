using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Install Scope dialog
    /// </summary>
    public partial class InstallScopeDialog : ManagedForm, IManagedDialog
    {
        private string installDirProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallScopeDialog"/> class.
        /// </summary>
        public InstallScopeDialog()
        {
            InitializeComponent();
            label1.MakeTransparentOn(banner);
            label2.MakeTransparentOn(banner);
        }

        void InstallDirDialog_Load(object sender, EventArgs e)
        {
            this.machineScopeRadioButton.Checked = MsiRuntime.Session["MSIINSTALLPERUSER"] == "0";
            this.userScopeRadioButton.Checked = MsiRuntime.Session["MSIINSTALLPERUSER"] == "1";

            if (!this.machineScopeRadioButton.Checked && !this.userScopeRadioButton.Checked)
            {
                this.userScopeRadioButton.Checked = true;
            }

            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");

            this.installDirProperty = MsiRuntime.Session.Property("WixSharp_UI_INSTALLDIR");

            string installDirPropertyValue = MsiRuntime.Session.Property(this.installDirProperty);

            if (installDirPropertyValue.IsEmpty())
            {
                //We are executed before any of the MSI actions are invoked so the INSTALLDIR (if set to absolute path)
                //is not resolved yet. So we need to do it manually
                installDir.Text = MsiRuntime.Session.GetDirectoryPath(this.installDirProperty);

                if (installDir.Text == "ABSOLUTEPATH")
                {
                    installDir.Text = MsiRuntime.Session.Property("INSTALLDIR_ABSOLUTEPATH");
                }
            }
            else
            {
                //INSTALLDIR set either from the command line or by one of the early setup events (e.g. UILoaded)
                installDir.Text = Environment.ExpandEnvironmentVariables(installDirPropertyValue);
            }

            ResetLayout();
        }

        void ResetLayout()
        {
            // The form controls are properly anchored and will be correctly resized on parent form
            // resizing. However the initial sizing by WinForm runtime doesn't a do good job with DPI
            // other than 96. Thus manual resizing is the only reliable option apart from going WPF.
            float ratio = (float)banner.Image.Width / (float)banner.Image.Height;
            topPanel.Height = (int)(banner.Width / ratio);
            topBorder.Top = topPanel.Height + 1;

            middlePanel.Top = topBorder.Bottom + 10;

            var upShift = (int)(next.Height * 2.3) - bottomPanel.Height;
            bottomPanel.Top -= upShift;
            bottomPanel.Height += upShift;
        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            if (!installDirProperty.IsEmpty())
            {
                if (userScopeRadioButton.Checked)
                {
                    MsiRuntime.Session["MSIINSTALLPERUSER"] = "1";
                    MsiRuntime.Session[installDirProperty] = Environment.ExpandEnvironmentVariables($@"%LOCALAPPDATA%\Apps\My Company\My Product");
                }

                if (machineScopeRadioButton.Checked)
                {
                    MsiRuntime.Session["MSIINSTALLPERUSER"] = "0";
                    MsiRuntime.Session[installDirProperty] = installDir.Text;
                }
            }

            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        void change_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog { SelectedPath = installDir.Text })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    installDir.Text = dialog.SelectedPath;
                }
            }
        }

        void userScopeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.installDir.Enabled = false;
            this.change.Enabled = false;
        }

        void machineScopeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.installDir.Enabled = true;
            this.change.Enabled = true;
        }
    }
}