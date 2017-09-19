using System;
using System.Linq;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Setup Type dialog
    /// </summary>
    public partial class SetupTypeDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupTypeDialog"/> class.
        /// </summary>
        public SetupTypeDialog()
        {
            InitializeComponent();
            label1.MakeTransparentOn(banner);
            label2.MakeTransparentOn(banner);
        }

        Type ProgressDialog
        {
            get
            {
                return Shell.Dialogs
                            .Where(d => d.GetInterfaces().Contains(typeof(IProgressDialog)))
                            .FirstOrDefault();
            }
        }

        void typical_Click(object sender, System.EventArgs e)
        {
            int index = Shell.Dialogs.IndexOf(ProgressDialog);
            if (index != -1)
                Shell.GoTo(index);
            else
                Shell.GoNext();
        }

        void custom_Click(object sender, System.EventArgs e)
        {
            Shell.GoNext();
        }

        void complete_Click(object sender, System.EventArgs e)
        {
            string[] names = MsiRuntime.Session.Features.Select(x => x.Name).ToArray();
            MsiRuntime.Session["ADDLOCAL"] = names.Join(",");

            int index = Shell.Dialogs.IndexOf(ProgressDialog);
            if (index != -1)
                Shell.GoTo(index);
            else
                Shell.GoNext();
        }

        void back_Click(object sender, System.EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, System.EventArgs e)
        {
            Shell.GoNext();
        }

        void cancel_Click(object sender, System.EventArgs e)
        {
            Shell.Cancel();
        }

        void SetupTypeDialog_Load(object sender, System.EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");

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

            middlePanel.Top = topBorder.Bottom + 5;
            middlePanel.Height = (bottomPanel.Top - 5) - middlePanel.Top;
        }
    }
}