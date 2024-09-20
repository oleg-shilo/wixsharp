using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using io = System.IO;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Licence dialog
    /// </summary>
    public partial class LicenceDialog : ManagedForm, IManagedDialog // change ManagedForm->Form if you want to show it in designer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicenceDialog"/> class.
        /// </summary>
        public LicenceDialog()
        {
            InitializeComponent();
            titleLbl.MakeTransparentOn(banner);
            label2.MakeTransparentOn(banner);
        }

        void LicenceDialog_Load(object sender, EventArgs e)
        {
            banner.Image = Runtime.Session.GetResourceBitmap("WixUI_Bmp_Banner") ??
                           Runtime.Session.GetResourceBitmap("WixSharpUI_Bmp_Banner");

            agreement.Rtf = Runtime.Session.GetResourceString("WixSharp_LicenceFile");
            accepted.Checked = Runtime.Session["LastLicenceAcceptedChecked"] == "True";

            if (banner.Image != null)
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

            var upShift = (int)(next.Height * 2.3) - bottomPanel.Height;
            bottomPanel.Top -= upShift;
            bottomPanel.Height += upShift;

            middlePanel.Top = topBorder.Bottom + 1;
            middlePanel.Height = (bottomPanel.Top - 1) - middlePanel.Top;
        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        void accepted_CheckedChanged(object sender, EventArgs e)
        {
            next.Enabled = accepted.Checked;
            Runtime.Session["LastLicenceAcceptedChecked"] = accepted.Checked.ToString();
        }

        void print_Click(object sender, EventArgs e)
        {
            try
            {
                var file = Path.Combine(Path.GetTempPath(), Runtime.Session.Property("ProductName") + ".licence.rtf");
                io.File.WriteAllText(file, agreement.Rtf);
                Process.Start(file);
            }
            catch
            {
                //Catch all, we don't want the installer to crash in an
                //attempt to write to a file.
            }
        }

        void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var data = new DataObject();

                if (agreement.SelectedText.Length > 0)
                {
                    data.SetData(DataFormats.UnicodeText, agreement.SelectedText);
                    data.SetData(DataFormats.Rtf, agreement.SelectedRtf);
                }
                else
                {
                    data.SetData(DataFormats.Rtf, agreement.Rtf);
                    data.SetData(DataFormats.Text, agreement.Text);
                }

                Clipboard.SetDataObject(data);
            }
            catch
            {
                //Catch all, we don't want the installer to crash in an
                //attempt at setting data on the clipboard.
            }
        }
    }
}