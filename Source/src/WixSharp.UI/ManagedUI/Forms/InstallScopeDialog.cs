using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Install Scope dialog
    /// </summary>
    public partial class InstallScopeDialog :
        ManagedForm,
        // Form,
        IManagedDialog // change ManagedForm->Form if you want to show it in designer
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

        void InstallScopeDialog_Load(object sender, EventArgs e)
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa367559(v=vs.85).aspx

            // The value of the ALLUSERS property, at installation time, determines the installation context.
            // * An ALLUSERS property value of 1 specifies the per - machine installation context.
            // * An ALLUSERS property value of an empty string("") specifies the per - user installation context.
            // * If the value of the ALLUSERS property is set to 2, the Windows Installer always resets the value of the ALLUSERS property to 1 and performs a per-machine installation or it resets the value of the ALLUSERS property to an empty string("") and performs a per-user installation.The value ALLUSERS = 2 enables the system to reset the value of ALLUSERS, and the installation context, dependent upon the user's privileges and the version of Windows

            Runtime.Session["ALLUSERS"] = "2";

            this.machineScopeRadioButton.Checked = Runtime.Session["MSIINSTALLPERUSER"] == "0";
            this.userScopeRadioButton.Checked = Runtime.Session["MSIINSTALLPERUSER"] == "1";

            if (!this.machineScopeRadioButton.Checked && !this.userScopeRadioButton.Checked)
            {
                this.userScopeRadioButton.Checked = true;
            }

            banner.Image = Runtime.Session.GetResourceBitmap("WixUI_Bmp_Banner");

            this.installDirProperty = Runtime.Session.Property("WixSharp_UI_INSTALLDIR");

            string installDirPropertyValue = Runtime.Session.Property(this.installDirProperty);

            if (installDirPropertyValue.IsEmpty())
            {
                //We are executed before any of the MSI actions are invoked so the INSTALLDIR (if set to absolute path)
                //is not resolved yet. So we need to do it manually
                installDir.Text = Runtime.Session.GetDirectoryPath(this.installDirProperty);

                if (installDir.Text == "ABSOLUTEPATH")
                {
                    installDir.Text = Runtime.Session.Property("INSTALLDIR_ABSOLUTEPATH");
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

            change.Top = installDir.Bottom + 5;
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
                    Runtime.Session["MSIINSTALLPERUSER"] = "1";
                    Runtime.Session[installDirProperty] = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Apps\" + Runtime.ProductName);
                }

                if (machineScopeRadioButton.Checked)
                {
                    Runtime.Session["MSIINSTALLPERUSER"] = "0";
                    Runtime.Session[installDirProperty] = installDir.Text;
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