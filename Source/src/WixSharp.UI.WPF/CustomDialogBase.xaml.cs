using Caliburn.Micro;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

namespace WixSharp.UI.WPF
{
    public partial class CustomDialogBase : WpfDialog, IWpfDialog
    {
        public CustomDialogBase()
        {
            InitializeComponent();
        }

        public StackPanel ButtonsPanel => NavigationPanel;

        public Button[] Buttons => NavigationPanel.Children.OfType<Button>().ToArray();
        public Button GoNextButton => Buttons.FirstOrDefault(b => b.Name == "GoNext");
        public Button GoPrevButton => Buttons.FirstOrDefault(b => b.Name == "GoPrev");
        public Button CancelButton => Buttons.FirstOrDefault(b => b.Name == "Cancel");

        public void Init()
        {
            ViewModelBinder.Bind(new CustomDialogBaseModel { Host = ManagedFormHost }, this, null);
        }

        public void SetUserContent(object userContent)
        {
            DialogContent.Content = userContent;

            if (userContent is IWpfDialogContent standardUserPanel)
            {
                standardUserPanel.Init(this);
            }
        }
    }

    public class CustomDialogBaseModel : Caliburn.Micro.Screen
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