using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;

using IO = System.IO;

namespace WixSharp.UI.WPF
{
    public partial class WelcomeDialog : WpfDialog, IWpfDialog
    {
        public WelcomeDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ViewModelBinder.Bind(new WelcomeDialogModel { Host = ManagedFormHost }, this, null);
        }
    }

    public class WelcomeDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host;
        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        public BitmapImage Banner => session?.GetResourceBitmap("WixUI_Bmp_Dialog").ToImageSource();

        public bool CanGoPrev => false;

        public void GoPrev()
            => shell?.GoPrev();

        public void GoNext()
            => shell?.GoNext();

        public void Cancel()
            => shell?.Cancel();
    }
}