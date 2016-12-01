using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Deployment.Samples.EmbeddedUI;
using Microsoft.Deployment.WindowsInstaller;

namespace EmbeddedUI
{
    public partial class SetupWizard : Form
    {
        ManualResetEvent installStartEvent;
        InstallProgressCounter progressCounter;
        bool canceled;

        public SetupWizard(ManualResetEvent installStartEvent)
        {
            InitializeComponent();
            this.installStartEvent = installStartEvent;
            this.progressCounter = new InstallProgressCounter(0.5);
        }

        public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            try
            {
                this.progressCounter.ProcessMessage(messageType, messageRecord);
                this.progressBar.Value = (int)(this.progressBar.Minimum + this.progressCounter.Progress * (this.progressBar.Maximum - this.progressBar.Minimum));
                this.progressLabel.Text = "" + (int)Math.Round(100 * this.progressCounter.Progress) + "%";

                switch (messageType)
                {
                    case InstallMessage.Error:
                    case InstallMessage.Warning:
                    case InstallMessage.Info:
                        string message = String.Format("{0}: {1}", messageType, messageRecord);
                        this.LogMessage(message);
                        break;
                }

                if (this.canceled)
                {
                    this.canceled = false;
                    return MessageResult.Cancel;
                }
            }
            catch (Exception ex)
            {
                this.LogMessage(ex.ToString());
                this.LogMessage(ex.StackTrace);
            }

            Application.DoEvents();

            return MessageResult.OK;
        }

        void LogMessage(string message)
        {
            messagesTextBox.AppendText(message + Environment.NewLine);
        }

        internal void EnableExit()
        {
            progressBar.Visible =
            progressLabel.Visible =
            cancelButton.Visible = false;
            exitButton.Visible = true;
        }

        void exitButton_Click(object sender, EventArgs e)
        {

            Close();
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            if (installButton.Visible)
            {
                Close();
            }
            else
            {
                canceled = true;
                cancelButton.Enabled = false;
            }
        }

        void installButton_Click(object sender, EventArgs e)
        {
            installButton.Visible = false;
            progressBar.Visible =
            progressLabel.Visible = true;
            installStartEvent.Set();
        }
    }
}
