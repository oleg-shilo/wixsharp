using System;

using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The standard Welcome dialog
    /// </summary>
    public partial class WelcomeDialog : ManagedForm, IManagedDialog // change ManagedForm->Form if you want to show it in designer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeDialog"/> class.
        /// </summary>
        public WelcomeDialog()
        {
            InitializeComponent();
        }

        void WelcomeDialog_Load(object sender, EventArgs e)
        {
            image.Image = Runtime.Session.GetResourceBitmap("WixUI_Bmp_Dialog");

            ResetLayout();
        }

        void ResetLayout()
        {
            // The form controls are properly anchored and will be correctly resized on parent form
            // resizing. However the initial sizing by WinForm runtime doesn't a do good job with DPI
            // other than 96. Thus manual resizing is the only reliable option apart from going WPF.

            var bHeight = (int)(next.Height * 2.3);

            var upShift = bHeight - bottomPanel.Height;
            bottomPanel.Top -= upShift;
            bottomPanel.Height = bHeight;

            imgPanel.Height = this.ClientRectangle.Height - bottomPanel.Height;
            float ratio = (float)image.Image.Width / (float)image.Image.Height;
            image.Width = (int)(image.Height * ratio);

            textPanel.Left = image.Right + 5;
            textPanel.Width = (bottomPanel.Width - image.Width) - 10;
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        void next_Click(object sender, EventArgs e)
        {
            Shell.GoNext();
        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }
    }
}