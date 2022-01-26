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
    public partial class LicenseDialog : WpfDialog, IWpfDialog
    {
        public LicenseDialog()
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

        public string LicenceText => host?.Runtime.Session.GetResourceString("WixSharp_LicenceFile");

        public BitmapImage Banner => Host?.Runtime.Session.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();

        public bool LicenseAcceptedChecked
        {
            get => Host?.Runtime.Session["LastLicenceAcceptedChecked"] == "True";
            set
            {
                if (Host != null)
                    Host.Runtime.Session["LastLicenceAcceptedChecked"] = value.ToString();

                NotifyOfPropertyChange(() => LicenseAcceptedChecked);
                NotifyOfPropertyChange(() => CanGoNext);
            }
        }

        public bool CanGoNext
            => LicenseAcceptedChecked;

        public void GoPrev()
            => Host?.Shell.GoPrev();

        public void GoNext()
            => Host?.Shell.GoNext();

        public void Cancel()
            => Host?.Shell.Cancel();

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
                //Catch all, we don't want the installer to crash in an
                //attempt to write to a file.
            }
        }
    }
}