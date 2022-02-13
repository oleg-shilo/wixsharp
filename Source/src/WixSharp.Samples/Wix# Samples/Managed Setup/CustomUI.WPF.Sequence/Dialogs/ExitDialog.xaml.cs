using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;

using IO = System.IO;

namespace WixSharp.UI.WPF
{
    public partial class ExitDialog : WpfDialog, IWpfDialog
    {
        public ExitDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ViewModelBinder.Bind(new ExitDialogModel { Host = ManagedFormHost }, this, null);
        }
    }

    public class ExitDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host { get; set; }

        public BitmapImage Banner => Host?.Runtime.Session.GetResourceBitmap("WixUI_Bmp_Dialog").ToImageSource();

        public bool CanGoPrev => false;

        public void GoPrev()
            => Host?.Shell.GoPrev();

        public void GoExit()
            => Host?.Shell.Exit();

        public void Cancel()
            => Host?.Shell.Cancel();

        public void ViewLog()
        {
            if (Host != null)
                try
                {
                    string wixSharpDir = Path.GetTempPath().PathCombine("WixSharp");
                    if (!Directory.Exists(wixSharpDir))
                        Directory.CreateDirectory(wixSharpDir);

                    string logFile = wixSharpDir.PathCombine(Host.Runtime.ProductName + ".log");
                    IO.File.WriteAllText(logFile, Host.Shell.Log);

                    Process.Start(logFile);
                }
                catch
                {
                    // Catch all, we don't want the installer to crash in an
                    // attempt to view the log.
                }
        }
    }
}