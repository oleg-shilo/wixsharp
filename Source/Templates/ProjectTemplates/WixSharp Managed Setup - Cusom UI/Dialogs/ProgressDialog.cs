using System;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using WixSharp.CommonTasks;

using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharpSetup.Dialogs
{
    /// <summary>
    /// The standard Installation Progress dialog
    /// </summary>
    public partial class ProgressDialog : ManagedForm, IManagedDialog, IProgressDialog // change ManagedForm->Form if you want to show it in designer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        public ProgressDialog()
        {
            InitializeComponent();
            dialogText.MakeTransparentOn(banner);
        }

        void ProgressDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            ResetLayout();

            Shell.StartExecute();
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

            var fontSize = waitPrompt.Font.Size;
            float scaling = 1;
            waitPrompt.Font = new Font(waitPrompt.Font.Name, fontSize * scaling, FontStyle.Italic);
        }

        /// <summary>
        /// Called when Shell is changed. It is a good place to initialize the dialog to reflect the MSI session
        /// (e.g. localize the view).
        /// </summary>
        protected override void OnShellChanged()
        {
            if (MsiRuntime.Session.IsUninstalling())
            {
                dialogText.Text =
                Text = "[ProgressDlgTitleRemoving]";
                description.Text = "[ProgressDlgTextRemoving]";
            }
            else if (MsiRuntime.Session.IsRepairing())
            {
                dialogText.Text =
                Text = "[ProgressDlgTextRepairing]";
                description.Text = "[ProgressDlgTitleRepairing]";
            }
            else if (MsiRuntime.Session.IsInstalling())
            {
                dialogText.Text =
                Text = "[ProgressDlgTitleInstalling]";
                description.Text = "[ProgressDlgTextInstalling]";
            }

            this.Localize();
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
        public override MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            switch (messageType)
            {
                case InstallMessage.InstallStart:
                case InstallMessage.InstallEnd:
                    {
                        waitPrompt.Visible = false;
                    }
                    break;

                case InstallMessage.ActionStart:
                    {
                        try
                        {
                            //messageRecord[0] - is reserved for FormatString value

                            string message = null;

                            bool simple = true;
                            if (simple)
                            {
                                for (int i = messageRecord.FieldCount - 1; i > 0; i--)
                                {
                                    message = messageRecord[i].ToString();
                                }
                            }
                            else
                            {
                                message = messageRecord.FormatString;
                                if (message.IsNotEmpty())
                                {
                                    for (int i = 1; i < messageRecord.FieldCount; i++)
                                    {
                                        message = message.Replace("[" + i + "]", messageRecord[i].ToString());
                                    }
                                }
                                else
                                {
                                    message = messageRecord[messageRecord.FieldCount - 1].ToString();
                                }
                            }

                            if (message.IsNotEmpty())
                                currentAction.Text = currentActionLabel.Text + " " + message;
                        }
                        catch { }
                    }
                    break;
            }
            return MessageResult.OK;
        }

        /// <summary>
        /// Called when MSI execution progress is changed.
        /// </summary>
        /// <param name="progressPercentage">The progress percentage.</param>
        public override void OnProgress(int progressPercentage)
        {
            progress.Value = progressPercentage;

            if (progressPercentage > 0)
            {
                waitPrompt.Visible = false;
            }
        }

        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
        public override void OnExecuteComplete()
        {
            currentAction.Text = null;
            Shell.GoNext();
        }

        /// <summary>
        /// Handles the Click event of the cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        void cancel_Click(object sender, EventArgs e)
        {
            if (Shell.IsDemoMode)
                Shell.GoNext();
            else
            {
                //Debug.Assert(false);

                //try
                //{
                //    MsiRuntime.Session["CANCEL_REQUEST"] = "TRUE";
                //}
                //catch { }

                Shell.Cancel();
            }
        }
    }
}