using System.Drawing;
using System.Windows.Forms;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The base class for all WinForm based dialogs of ManagedUI.
    /// </summary>
    /// <example>The following is an example of defining installation directory <c>Progam Files/My Company/My Product</c>
    /// containing a single file <c>MyApp.exe</c> and subdirectory <c>Documentation</c> with <c>UserManual.pdf</c> file.
    /// <code>
    /// public partial class CustomDialog : ManagedForm, IManagedDialog
    /// {
    ///     public UserNameDialog()
    ///     {
    ///         //instantiate banner PictureBox and back/next/cancel Buttons
    ///         InitializeComponent();
    ///     }
    ///
    ///     void CustomDialog_Load(object sender, EventArgs e)
    ///     {
    ///         banner.Image = Runtime.Session.GetResourceBitmap("WixSharpUI_Bmp_Banner");
    ///     }
    ///
    ///     void back_Click(object sender, EventArgs e)
    ///     {
    ///         Shell.GoPrev();
    ///     }
    ///
    ///     void next_Click(object sender, EventArgs e)
    ///     {
    ///         Shell.GoNext();
    ///     }
    ///
    ///     void cancel_Click(object sender, EventArgs e)
    ///     {
    ///         Shell.Cancel();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class ManagedForm : Form, IManagedDialog
    {
        internal static int IdealBackgroundImageWidth = 156;
        internal static int IdealBackgroundImageHeight = 312;

        IManagedUIShell shell;

        /// <summary>
        /// Gets or sets the UI shell (main UI window). This property is set the ManagedUI runtime (IManagedUI).
        /// On the other hand it is consumed (accessed) by the UI dialog (IManagedDialog).
        /// </summary>
        /// <value>
        /// The shell.
        /// </value>
        public IManagedUIShell Shell
        {
            get { return shell; }

            set
            {
                shell = value;
                OnShellChanged();
            }
        }

        /// <summary>
        /// Gets the MSI runtime context.
        /// </summary>
        /// <value>
        /// The msi runtime.
        /// </value>
        public MsiRuntime MsiRuntime => (MsiRuntime)Shell.RuntimeContext;

        /// <summary>
        /// Gets the installer runtime context.
        /// </summary>
        /// <value>
        /// The installer runtime.
        /// </value>
        public InstallerRuntime Runtime => (InstallerRuntime)Shell.RuntimeContext;

        /// <summary>
        /// Processes information and progress messages sent to the user interface.
        /// <para> This method directly mapped to the
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI.ProcessMessage" />.</para>
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
        virtual public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            return MessageResult.OK;
        }

        /// <summary>
        /// Called when Shell is changed. It is a good place to initialize the dialog to reflect the MSI session
        /// (e.g. localize the view).
        /// </summary>
        virtual protected void OnShellChanged()
        {
        }

        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
        virtual public void OnExecuteComplete()
        {
        }

        /// <summary>
        /// Called when MSI execute started.
        /// </summary>
        virtual public void OnExecuteStarted()
        {
        }

        /// <summary>
        /// Called when MSI execution progress is changed.
        /// </summary>
        /// <param name="progressPercentage">The progress percentage.</param>
        virtual public void OnProgress(int progressPercentage)
        {
        }

        /// <summary>
        /// Localizes the form and its contained <see cref="T:System.Windows.Forms.Control.Text"/> from the specified localization
        /// delegate 'localize'.
        /// <para>The method substitutes both localization file (*.wxl) entries and MSI properties contained by the input string
        /// with their translated/converted values.</para>
        /// <remarks>
        /// Note that both localization entries and MSI properties must be enclosed in the square brackets
        /// (e.g. "[ProductName] Setup", "[InstallDirDlg_Title]").
        /// </remarks>
        /// </summary>
        public void Localize()
        {
            this.LocalizeWith(Runtime.Localize);
        }

        /// <summary>
        /// Sets the size of the shell.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void SetShellSize(int width, int height)
        {
            this.MaximumSize =
            this.MinimumSize =
            this.Size = new Size(width, height);
        }
    }
}