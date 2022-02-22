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

namespace WixSharp.UI.WPF.Sequence
{
    public partial class LicenceDialog : WpfDialog, IWpfDialog
    {
        public LicenceDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ViewModelBinder.Bind(
                new LicenseDialogModel
                {
                    ShowRtfContent = x => this.LicenceText.SetRtf(x),
                    Host = ManagedFormHost,
                },
                this,
                null);
        }
    }

    static partial class extension
    {
        public static void SetRtf(this RichTextBox rtb, string document)
        {
            var documentBytes = Encoding.UTF8.GetBytes(document);
            using (var reader = new MemoryStream(documentBytes))
            {
                reader.Position = 0;
                rtb.SelectAll();
                rtb.Selection.Load(reader, DataFormats.Rtf);
            }
        }
    }

    public class LicenseDialogModel : Caliburn.Micro.Screen
    {
        ManagedForm host;
        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        public Action<string> ShowRtfContent;

        public ManagedForm Host
        {
            get => host;
            set
            {
                host = value;

                ShowRtfContent?.Invoke(LicenceText);
                NotifyOfPropertyChange(() => Banner);
                NotifyOfPropertyChange(() => LicenseAcceptedChecked);
                NotifyOfPropertyChange(() => CanGoNext);
            }
        }

        public string LicenceText => session?.GetResourceString("WixSharp_LicenceFile");

        public BitmapImage Banner => session?.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();

        public bool LicenseAcceptedChecked
        {
            get => session?["LastLicenceAcceptedChecked"] == "True";
            set
            {
                if (Host != null)
                    session["LastLicenceAcceptedChecked"] = value.ToString();

                NotifyOfPropertyChange(() => LicenseAcceptedChecked);
                NotifyOfPropertyChange(() => CanGoNext);
            }
        }

        public bool CanGoNext
            => LicenseAcceptedChecked;

        public void GoPrev()
            => shell?.GoPrev();

        public void GoNext()
            => shell?.GoNext();

        public void Cancel()
            => shell?.Cancel();

        public void Print()
        {
            try
            {
                var file = IO.Path.GetTempPath().PathCombine(Host?.Runtime.Session.Property("ProductName") + ".licence.rtf");
                IO.File.WriteAllText(file, LicenceText);
                Process.Start(file);
            }
            catch
            {
                // Catch all, we don't want the installer to crash in an
                // attempt to write to a file.
            }
        }
    }
}