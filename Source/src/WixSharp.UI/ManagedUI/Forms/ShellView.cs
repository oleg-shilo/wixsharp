using System;
using System.Drawing;
using System.Windows.Forms;
using WixSharp.UI.Forms;
using WixSharp.UI.ManagedUI;

namespace WixSharp
{
    /// <summary>
    ///
    /// </summary>
    public static class FolderBrowserDialog
    {
        static FolderBrowserDialog()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                if (e.Name.StartsWith("ShellFileDialogs"))
                    return System.Reflection.Assembly.Load(Resources.ShellFileDialogs_dll);
                return null;
            };
        }

        /// <summary>
        /// Shows the folder selection dialog. Returns <see langword="null"/> if the user cancelled the dialog.
        /// Otherwise returns the selected path.
        /// <remarks>
        /// Since .NET does not provide API for this dialog the COM Interop is used instead.
        /// <p>This is a fork of rather excellent https://github.com/daiplusplus/ShellFileDialogs, which
        /// is an extract from Microsoft Windows API Code Pack.</p></remarks>
        ///
        /// </summary>
        /// <param name="parentHWnd">The parent h WND.</param>
        /// <param name="title">The title.</param>
        /// <param name="initialDirectory">The initial directory.</param>
        /// <returns></returns>
        public static string ShowDialog(IntPtr parentHWnd, string title, string initialDirectory) =>
            ShellFileDialogs.FolderBrowserDialog.ShowDialog(parentHWnd, title, initialDirectory);
    }
}

namespace WixSharp.Forms
{
    /// <summary>
    /// Set of ManagedUI dialogs (WinForm) that implement all standard MSI UI dialogs.
    /// </summary>
    public class Dialogs
    {
        /// <summary>
        /// The standard Welcome dialog
        /// </summary>
        static public Type Welcome = typeof(WelcomeDialog);

        /// <summary>
        /// The standard Licence dialog
        /// </summary>
        static public Type Licence = typeof(LicenceDialog);

        /// <summary>
        /// The standard Features dialog
        /// </summary>
        static public Type Features = typeof(FeaturesDialog);

        /// <summary>
        /// The standard InstallDir dialog
        /// </summary>
        static public Type InstallDir = typeof(InstallDirDialog);

        /// <summary>
        /// The standard InstallScope dialog
        /// </summary>
        static public Type InstallScope = typeof(InstallScopeDialog);

        /// <summary>
        /// The standard Installation Progress dialog
        /// </summary>
        static public Type Progress = typeof(ProgressDialog);

        /// <summary>
        /// The standard Setup Type dialog. To be used during the installation of the product.
        /// </summary>
        static public Type SetupType = typeof(SetupTypeDialog);

        /// <summary>
        /// The standard Maintenance Type dialog.To be used during the maintenance of the product.
        /// </summary>
        static public Type MaintenanceType = typeof(MaintenanceTypeDialog);

        /// <summary>
        /// The standard Exit dialog
        /// </summary>
        static public Type Exit = typeof(ExitDialog);
    }

    class ShellView : Form, IShellView
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public ShellView()
        {
            InitializeComponent();
            this.Load += (sender, e) => TopMost = false; //To ensure initial 'on top'
        }

        void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.MinimumSize = new System.Drawing.Size(510, 400);
            this.MaximumSize = new System.Drawing.Size(510, 400);
            this.MaximizeBox = false;
            this.Name = "UIShell";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Icon = ".msi".GetAssiciatedIcon();
            this.ResumeLayout(false);
        }

        public void SetSize(int width, int height)
        {
            this.MaximumSize =
            this.MinimumSize =
            this.Size = new Size(width, height);
        }

        public event Action<IManagedDialog> OnCurrentDialogChanged;

        public void RaiseCurrentDialogChanged(IManagedDialog newDialog)
        {
            if (OnCurrentDialogChanged != null)
                OnCurrentDialogChanged(newDialog);
        }

        /// <summary>
        /// Gets and sets the current dialog of the UI sequence.
        /// </summary>
        /// <value>The current dialog.</value>
        public IManagedDialog CurrentDialog { get; set; }

        public IManagedUIShell Shell { get; set; }
    }
}