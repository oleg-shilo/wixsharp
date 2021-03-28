using Caliburn.Micro;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

namespace MyProduct
{
    public partial class CustomDialogView : WpfDialog, IWpfDialog
    {
        public CustomDialogView()
        {
            InitializeComponent();
        }

        public void Init()
        {
            var viewModel = new CustomDialogModel { Host = this.ManagedFormHost };
            viewModel.Host.Runtime.Localize(this.Root);           // resolve and translate all elements with translatable content ("[<localization_key>]")
            viewModel.Host.Text = viewModel.DialogTitle;          // setup UI shell title

            ViewModelBinder.Bind(viewModel, this, null);
        }
    }

    public class CustomDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host { get; set; }

        public string DialogTitle => Host?.Runtime.Localize("ProductName") + " Setup";
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