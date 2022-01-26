using Caliburn.Micro;
using System;
using System.Windows.Media.Imaging;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

namespace WixSharp.UI.WPF
{
    public partial class CustomDialogRawView : WpfDialog, IWpfDialog
    {
        public string User { get; set; } = Environment.UserName;

        public CustomDialogRawView()
        {
            InitializeComponent();
            this.DataContext = this;
            CanProceedIsChecked_Click(null, null);
        }

        public void Init()
        {
            Banner.Source = this.ManagedFormHost?.Runtime.Session.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();
        }

        void GoPrev_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Host?.Shell.GoPrev();
        }

        void GoNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Host?.Shell.GoNext();
        }

        void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Host?.Shell.Cancel();
        }

        void CanProceedIsChecked_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GoNext.IsEnabled = (CanProceedIsChecked.IsChecked == true);
        }
    }
}