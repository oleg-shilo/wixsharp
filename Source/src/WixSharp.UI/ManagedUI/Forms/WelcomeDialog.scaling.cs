using System;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Welcome dialog
    /// </summary>
    public partial class WelcomeDialog
    {
        //This is an attempt to allow fill background image instead of left banner.
        //It does work but not very reliably as WinForm scaling algorithm is not 100% 
        //compatible with PictureBox stretching. Thus some visual artifacts can happen.
        //
        // Also releasing this solution will trigger the public API change:
        //  old project.BackgroundImage -> project.LeftBannerImage
        //  new project.BackgroundImage 
        //
        // But the biggest problem is that letting image to fill whole client area doesn't 
        // do any good anyway as WinForm containers holding the dialog elements (e.g. textPanel)
        // do not support transparency so the image will be largely hidden/overlapped anyway.
        // The only true solution can be the WPF migration. 

        void Alternative_Load(object sender, EventArgs e)
        {
            image.Image = Runtime.Session.GetEmbeddedBitmap("WixSharpUI_Bmp_Dialog");
            //background additional picturebox occupying the background area 
            //background.Image = Runtime.Session.GetEmbeddedBitmap("WixUI_Bmp_Dialog");

            AlternativeResetLayout();
        }

        void AlternativeResetLayout()
        {
            // The form controls are properly anchored and will be correctly resized on parent form 
            // resizing. However the initial sizing by WinForm runtime doesn't a do good job with DPI 
            // other than 96. Thus manual resizing is the only reliable option apart from going WPF.  

            var bHeight = (int)(next.Height * 2.3);

            var upShift = bHeight - bottomPanel.Height;
            bottomPanel.Top -= upShift;
            bottomPanel.Height = bHeight;

            imgPanel.Height = this.ClientRectangle.Height - bottomPanel.Height;

            if (image.Image == null)
            {
                image.Visible = false;
                float ratio = 156f / 312f; //matching default WiX dialog image
                ratio = 164f / 497f; //found by experiment
                image.Width = (int)(imgPanel.Width * ratio);
            }
            else
            {
                float ratio = (float)image.Image.Width / (float)image.Image.Height;
                image.Width = (int)(image.Height * ratio);
            }

            textPanel.Left = image.Right + 5;
            textPanel.Width = (bottomPanel.Width - image.Width) - 10;
        }
    }
}