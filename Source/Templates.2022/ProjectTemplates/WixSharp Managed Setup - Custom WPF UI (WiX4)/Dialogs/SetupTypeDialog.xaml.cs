using Caliburn.Micro;
using System.Linq;
using System.Windows.Media.Imaging;
using WixSharp;
using WixSharp.UI.Forms;

using WixSharp.UI.WPF;

namespace $safeprojectname$
{
    /// <summary>
    /// The standard SetupTypeDialog.
    /// <para>Follows the design of the canonical Caliburn.Micro View (MVVM).</para>
    /// <para>See https://caliburnmicro.com/documentation/cheat-sheet</para>
    /// </summary>
    /// <seealso cref="WixSharp.UI.WPF.WpfDialog" />
    /// <seealso cref="WixSharp.IWpfDialog" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class SetupTypeDialog : WpfDialog, IWpfDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupTypeDialog" /> class.
        /// </summary>
        public SetupTypeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is invoked by WixSHarp runtime when the custom dialog content is internally fully initialized.
        /// This is a convenient place to do further initialization activities (e.g. localization).
        /// </summary>
        public void Init()
        {
            ViewModelBinder.Bind(new SetupTypeDialogModel { Host = ManagedFormHost, }, this, null);
        }
    }

    /// <summary>
    /// ViewModel for standard SetupTypeDialog.
    /// <para>Follows the design of the canonical Caliburn.Micro ViewModel (MVVM).</para>
    /// <para>See https://caliburnmicro.com/documentation/cheat-sheet</para>
    /// </summary>
    /// <seealso cref="Caliburn.Micro.Screen" />
    class SetupTypeDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host;

        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        public BitmapImage Banner => session?.GetResourceBitmap("WixSharpUI_Bmp_Banner").ToImageSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupTypeDialog" /> class.
        /// </summary>
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