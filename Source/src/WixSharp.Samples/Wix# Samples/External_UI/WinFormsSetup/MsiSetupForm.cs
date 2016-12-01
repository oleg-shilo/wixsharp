using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WixSharp.UI
{
    public partial class MsiSetupForm : Form
    {
        public MsiSetupForm(string msiFile)
        {
            InitializeComponent();

            session = new MyProductSetup(msiFile);
            session.InUiThread = this.InUIThread;

            session.ProgressChanged += session_ProgressChanged;
            session.ActionStarted += session_ActionStarted;
            session.SetupComplete += session_SetupComplete;

            UpdateLayout();
        }

        void session_SetupComplete()
        {
            if(string.IsNullOrEmpty(session.ErrorStatus))
                setupStatusLbl.Text = "Success";
            else
                setupStatusLbl.Text = "Error. See Log for details";

            progressBar.Value = 0;
            showLogBtn.Enabled = true;
        }

        void session_ActionStarted(object sender, EventArgs e)
        {
            setupStatusLbl.Text = session.CurrentActionName;
        }

        void session_ProgressChanged(object sender, EventArgs e)
        {
            progressBar.Maximum = session.ProgressTotal;
            
            //do not trust MSI events. session.ProgressCurrentPosition can be even negative
            if (session.ProgressCurrentPosition > 0 && session.ProgressCurrentPosition <= progressBar.Maximum) 
                progressBar.Value = Math.Min(session.ProgressTotal, session.ProgressCurrentPosition);
        }

        void UpdateLayout(bool sessionEnd = false)
        {
            this.Text = session.ProductName + " - Setup";

            if (!sessionEnd)
            {
                this.installBtn.Enabled = !session.IsCurrentlyInstalled;
                this.repairBtn.Enabled = session.IsCurrentlyInstalled;
                this.uninstallBtn.Enabled = session.IsCurrentlyInstalled;
                this.showLogBtn.Enabled = false;
            }
            else
                this.showLogBtn.Enabled = true;

            this.productStatusLbl.Text = session.ProductName + " status: " + (session.IsCurrentlyInstalled ? "Installed" : "Not Installed");
        }

        void DisableButtons()
        {
            this.installBtn.Enabled =
            this.repairBtn.Enabled =
            this.uninstallBtn.Enabled = false;
        }

        MyProductSetup session;

        void installBtn_Click(object sender, EventArgs e)
        {
            DisableButtons();
            session.StartInstall();
        }

        void repairBtn_Click(object sender, EventArgs e)
        {
            DisableButtons();
            session.StartRepair();
        }

        void uninstallBtn_Click(object sender, EventArgs e)
        {
            DisableButtons();
            session.StartUnInstall();
        }

        void InUIThread(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private void showLogBtn_Click(object sender, EventArgs e)
        {
            Process.Start(session.LogFile);
        }
    }
}