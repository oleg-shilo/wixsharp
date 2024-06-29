using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

namespace WixSharp.UI.WPF
{
    public partial class CustomDialogView : WpfDialog, IWpfDialog
    {
        public CustomDialogView()
        {
            InitializeComponent();
        }

        public void Init()
        {
            var topWindow = this.ManagedFormHost.Parent as System.Windows.Forms.Form;
            topWindow.FormClosing += (sender, e) =>
            {
                MessageBox.Show("Closing...");
            };

            ViewModelBinder.Bind(new CustomDialogModel { Host = ManagedFormHost }, this, null);
        }
    }

    public class CustomDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host { get; set; }

        public BitmapImage Banner => Host?.Runtime.Session.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();

        bool canProceed;

        public bool CanProceedIsChecked
        {
            get { return canProceed; }
            set
            {
                canProceed = value;
                NotifyOfPropertyChange(() => CanProceedIsChecked);
                NotifyOfPropertyChange(() => CanGoNext);
            }
        }

        public string User { get; set; } = Environment.UserName;

        public bool CanGoNext
            => CanProceedIsChecked;

        public void GoPrev()
            => Host?.Shell.GoPrev();

        public void GoNext()
            => Host?.Shell.GoNext();

        public void Cancel()
            => Host?.Shell.Cancel();
    }
}