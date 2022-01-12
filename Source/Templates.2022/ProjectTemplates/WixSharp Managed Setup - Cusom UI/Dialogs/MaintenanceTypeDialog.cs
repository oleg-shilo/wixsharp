using System;
using System.Linq;

using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The standard Maintenance Type dialog
    /// </summary>
    public partial class MaintenanceTypeDialog : ManagedForm, IManagedDialog // change ManagedForm->Form if you want to show it in designer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceTypeDialog"/> class.
        /// </summary>
        public MaintenanceTypeDialog()
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
                    .FirstOrDefault(d => d.GetInterfaces().Contains(typeof(IProgressDialog)));
            }
        }

        void change_Click(object sender, System.EventArgs e)
        {
            Runtime.Session["MODIFY_ACTION"] = "Change";
            Shell.GoNext();
        }

        void repair_Click(object sender, System.EventArgs e)
        {
            Runtime.Session["MODIFY_ACTION"] = "Repair";
            int index = Shell.Dialogs.IndexOf(ProgressDialog);
            if (index != -1)
                Shell.GoTo(index);
            else
                Shell.GoNext();
        }

        void remove_Click(object sender, System.EventArgs e)
        {
            Runtime.Session["REMOVE"] = "ALL";
            Runtime.Session["MODIFY_ACTION"] = "Remove";

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

        void MaintenanceTypeDialog_Load(object sender, System.EventArgs e)
        {
            banner.Image = Runtime.Session.GetResourceBitmap("WixUI_Bmp_Banner");

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