using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;

using IO = System.IO;

namespace WixSharp.UI.WPF.Sequence
{
    public partial class MaintenanceTypeDialog : WpfDialog, IWpfDialog
    {
        public MaintenanceTypeDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ViewModelBinder.Bind(new MaintenanceTypeDialogModel { Host = ManagedFormHost, }, this, null);
        }
    }

    public class MaintenanceTypeDialogModel : Caliburn.Micro.Screen
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

        public void Change()
        {
            if (session != null)
            {
                session["MODIFY_ACTION"] = "Change";
                shell.GoNext();
            }
        }

        public void Repair()
        {
            if (session != null)
            {
                session["MODIFY_ACTION"] = "Repair";
                JumpToProgressDialog();
            }
        }

        public void Remove()
        {
            if (session != null)
            {
                session["REMOVE"] = "ALL";
                session["MODIFY_ACTION"] = "Remove";

                JumpToProgressDialog();
            }
        }

        public void GoPrev()
            => shell?.GoPrev();

        public void Cancel()
            => Host?.Shell.Cancel();
    }
}