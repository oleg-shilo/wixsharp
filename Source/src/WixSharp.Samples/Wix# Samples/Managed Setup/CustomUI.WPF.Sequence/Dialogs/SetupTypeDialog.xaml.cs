using System.Linq;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;

using IO = System.IO;

namespace WixSharp.UI.WPF.Sequence
{
    public partial class SetupTypeDialog : WpfDialog, IWpfDialog
    {
        public SetupTypeDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ViewModelBinder.Bind(new SetupTypeDialogModel { Host = ManagedFormHost, }, this, null);
        }
    }

    public class SetupTypeDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host;

        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        public BitmapImage Banner => session?.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();

        void JumpToProgressDialog()
        {
            int index = shell.Dialogs.IndexOfDialogImplementing<IProgressDialog>();
            if (index != -1)
                shell.GoTo(index);
            else
                shell.GoNext(); // if user did not supply progress dialog then simply go next
        }

        public void DoTypical()
        {
            if (shell != null)
                JumpToProgressDialog();
        }

        public void DoComplete()
        {
            if (shell != null)
            {
                // mark all features to be installed
                string[] names = session.Features.Select(x => x.Name).ToArray();
                session["ADDLOCAL"] = names.JoinBy(",");

                JumpToProgressDialog();
            }
        }

        public void DoCustom()
            => shell?.GoNext(); // let the dialog flow through

        public void GoPrev()
            => shell?.GoPrev();

        public void GoNext()
            => shell?.GoNext();

        public void Cancel()
            => shell?.Cancel();
    }
}