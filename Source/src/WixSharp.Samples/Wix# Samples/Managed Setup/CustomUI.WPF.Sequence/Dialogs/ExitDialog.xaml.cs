using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using WixSharp;
using WixSharp.UI.Forms;
using IO = System.IO;

namespace WixSharp.UI.WPF.Sequence
{
    public partial class ExitDialog : WpfDialog, IWpfDialog
    {
        public ExitDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            UpdateTitles(ManagedFormHost.Runtime.Session);
            ViewModelBinder.Bind(new ExitDialogModel { Host = ManagedFormHost }, this, null);
        }

        /// <summary>
        /// Updates the titles of the dialog depending on the success of the installation action.
        /// </summary>
        /// <param name="session">The session.</param>
        public void UpdateTitles(ISession session)
        {
            if (Shell.UserInterrupted || Shell.Log.Contains("User canceled installation."))
            {
                DialogTitleLabel.Text = "[UserExitTitle]";
                DialogDescription.Text = "[UserExitDescription1]";
            }
            else if (Shell.ErrorDetected)
            {
                DialogTitleLabel.Text = "[FatalErrorTitle]";
                DialogDescription.Text = Shell.CustomErrorDescription ?? "[FatalErrorDescription1]";
            }

            // `Localize` resolves [...] titles and descriptions into the localized strings stored in MSI resources tables
            this.Localize();
        }
    }

    public class ExitDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host { get; set; }
        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        public BitmapImage Banner => session?.GetResourceBitmap("WixSharpUI_Bmp_Dialog").ToImageSource();

        public void GoExit()
            => shell?.Exit();

        public void Cancel()
            => shell?.Exit();

        public void ViewLog()
        {
            if (shell != null)
                try
                {
                    string logFile = session.LogFile;

                    if (logFile.IsEmpty())
                    {
                        string wixSharpDir = Path.GetTempPath().PathCombine("WixSharp");
                        if (!Directory.Exists(wixSharpDir))
                            Directory.CreateDirectory(wixSharpDir);

                        logFile = wixSharpDir.PathCombine(Host.Runtime.ProductName + ".log");
                        IO.File.WriteAllText(logFile, shell.Log);
                    }
                    Process.Start("notepad.exe", logFile);
                }
                catch
                {
                    // Catch all, we don't want the installer to crash in an
                    // attempt to view the log.
                }
        }
    }
}