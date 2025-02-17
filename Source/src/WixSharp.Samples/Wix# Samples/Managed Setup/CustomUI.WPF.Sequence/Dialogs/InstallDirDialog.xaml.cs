using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;

using IO = System.IO;

namespace WixSharp.UI.WPF.Sequence
{
    public partial class InstallDirDialog : WpfDialog, IWpfDialog
    {
        public InstallDirDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ViewModelBinder.Bind(new InstallDirDialogModel { Host = ManagedFormHost, }, this, null);
        }
    }

    public class InstallDirDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host;
        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        public BitmapImage Banner => session?.GetResourceBitmap("WixSharpUI_Bmp_Banner").ToImageSource();

        string installDirProperty => session?.Property("WixSharp_UI_INSTALLDIR");

        public string InstallDirPath
        {
            get
            {
                if (Host != null)
                {
                    string installDirPropertyValue = session.Property(installDirProperty);

                    if (installDirPropertyValue.IsEmpty())
                    {
                        // We are executed before any of the MSI actions are invoked so the INSTALLDIR (if set to absolute path)
                        // is not resolved yet. So we need to do it manually
                        var installDir = session.GetDirectoryPath(installDirProperty);

                        if (installDir == "ABSOLUTEPATH")
                            installDir = session.Property("INSTALLDIR_ABSOLUTEPATH");

                        return installDir;
                    }
                    else
                    {
                        //INSTALLDIR set either from the command line or by one of the early setup events (e.g. UILoaded)
                        return installDirPropertyValue;
                    }
                }
                else
                    return null;
            }

            set
            {
                session[installDirProperty] = value;
                base.NotifyOfPropertyChange(() => InstallDirPath);
            }
        }

        public void ChangeInstallDir()
        {
            // `OpenFolderDialog.Select` is still under development so disabling it for now

            if (session.UseModernFolderBrowserDialog())
            {
                //     try
                //     {
                //         var (isSelected, path) = OpenFolderDialog.Select(installDir.Text);
                //         if (isSelected)
                //             installDir.Text = path;
                //         return;
                //     }
                //     catch
                //     {
                //     }
            }

            using (var dialog = new FolderBrowserDialog { SelectedPath = InstallDirPath })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    InstallDirPath = dialog.SelectedPath;
            }
        }

        public void GoPrev()
            => shell?.GoPrev();

        public void GoNext()
            => shell?.GoNext();

        public void Cancel()
            => shell?.Cancel();
    }
}