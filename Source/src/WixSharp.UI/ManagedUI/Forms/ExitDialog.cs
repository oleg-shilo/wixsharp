using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Exit dialog
    /// </summary>
    public partial class ExitDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitDialog"/> class.
        /// </summary>
        public ExitDialog()
        {
            InitializeComponent();
        }

        void ExitDialog_Load(object sender, System.EventArgs e)
        {
            image.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Dialog");
            if (Shell.UserInterrupted)
            {
                title.Text = "[UserExitTitle]";
                description.Text = "[UserExitDescription1]";
                this.Localize();
            }
            else if (Shell.ErrorDetected)
            {
                title.Text = "[FatalErrorTitle]";
                description.Text = "[FatalErrorDescription1]";
                this.Localize();
            }

            ResetLayout();

            // show error message if required
            // if (Shell.Errors.Any())
            // {
            //     string lastError = Shell.Errors.LastOrDefault();
            //     MessageBox.Show(lastError);
            // }
        }

        void ResetLayout()
        {
            // The form controls are properly anchored and will be correctly resized on parent form 
            // resizing. However the initial sizing by WinForm runtime doesn't do a good job with DPI 
            // other than 96. Thus manual resizing is the only reliable option apart from going WPF.  

            var bHeight = (int) (next.Height * 2.3);

            var upShift = bHeight - bottomPanel.Height;
            bottomPanel.Top -= upShift;
            bottomPanel.Height = bHeight;

            imgPanel.Height = this.ClientRectangle.Height - bottomPanel.Height;
            float ratio = (float) image.Image.Width / (float) image.Image.Height;
            image.Width = (int) (image.Height * ratio);

            textPanel.Left = image.Right + 5;
            textPanel.Width = (bottomPanel.Width - image.Width) - 10;
        }

        void finish_Click(object sender, System.EventArgs e)
        {
            Shell.Exit();
        }

        void viewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string wixSharpDir = Path.Combine(Path.GetTempPath(), @"WixSharp");
                if (!Directory.Exists(wixSharpDir))
                    Directory.CreateDirectory(wixSharpDir);

                string logFile = Path.Combine(wixSharpDir, MsiRuntime.ProductName + ".log");
                System.IO.File.WriteAllText(logFile, Shell.Log);
                Process.Start(logFile);
            }
            catch { }
        }
    }
}