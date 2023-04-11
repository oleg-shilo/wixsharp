using Caliburn.Micro;
using System.Windows.Media.Imaging;
using WixSharp;
using WixSharp.UI.Forms;

namespace WixSharp.UI.WPF
{
    /// <summary>
    /// The standard MaintenanceTypeDialog.
    /// <para>Follows the design of the canonical Caliburn.Micro View (MVVM).</para>
    /// <para>See https://caliburnmicro.com/documentation/cheat-sheet</para>
    /// </summary>
    /// <seealso cref="WixSharp.UI.WPF.WpfDialog" />
    /// <seealso cref="WixSharp.IWpfDialog" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MaintenanceTypeDialog : WpfDialog, IWpfDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceTypeDialog" /> class.
        /// </summary>
        public MaintenanceTypeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is invoked by WixSHarp runtime when the custom dialog content is internally fully initialized.
        /// This is a convenient place to do further initialization activities (e.g. localization).
        /// </summary>
        public void Init()
        {
            ViewModelBinder.Bind(new MaintenanceTypeDialogModel { Host = ManagedFormHost, }, this, null);
        }
    }

    /// <summary>
    /// ViewModel for standard MaintenanceTypeDialog.
    /// <para>Follows the design of the canonical Caliburn.Micro ViewModel (MVVM).</para>
    /// <para>See https://caliburnmicro.com/documentation/cheat-sheet</para>
    /// </summary>
    /// <seealso cref="Caliburn.Micro.Screen" />
    class MaintenanceTypeDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host;

        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        public BitmapImage Banner => session?.GetResourceBitmap("WixSharpUI_Bmp_Banner").ToImageSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceTypeDialog" /> class.
        /// </summary>
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