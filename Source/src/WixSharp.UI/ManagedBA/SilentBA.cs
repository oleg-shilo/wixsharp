using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#if WIX4
using WixToolset.Bootstrapper;
#else

using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

#endif

[assembly: BootstrapperApplication(typeof(WixSharp.Bootstrapper.SilentManagedBA))]

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Defines Wix# bootstrapper managed application with no User Interface.
    /// <para>It is a design time 'adapter' for the canonical WiX managed bootstrapper application <see cref="T:WixSharp.Bootstrapper.SilentManagedBA"/>.</para>
    /// <para><see cref="T:WixSharp.Bootstrapper.SilentManagedBA"/> automatically handles <see cref="BootstrapperApplication"/> events and
    /// detects the current package/product state (present vs. absent). The package state detection is based on the <see cref="T:WixSharp.Bootstrapper.SilentBootstrapperApplication.PrimaryPackageId"/>.
    /// If this member is no t then the Id of the lats package in the Bundle will be used instead.</para>
    /// <para>Note, the stock SilentManagedBA may have runtime hosting failure due to the fact that its assembly is compiled for AnyCPU.
    /// While it is a valid hosting option that may or may not work with a specific WiX distro, you may find that you may need to take
    /// this simple class and compile it into a separate x86 assembly. </para>
    /// </summary>
    /// <example>
    /// <code>
    ///  var bootstrapper =
    ///      new Bundle("My Product",
    ///          new PackageGroupRef("NetFx40Web"),
    ///          new MsiPackage("product.msi"));
    ///
    /// bootstrapper.AboutUrl = "https://github.com/oleg-shilo/wixsharp/";
    /// bootstrapper.IconFile = "app_icon.ico";
    /// bootstrapper.Version = new Version("1.0.0.0");
    /// bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
    /// bootstrapper.Application = new SilentBootstrapperApplication();
    ///
    /// bootstrapper.Build();
    /// </code>
    ///
    /// </example>
    public class SilentBootstrapperApplication : ManagedBootstrapperApplication, IWixSharpManagedBootstrapperApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SilentBootstrapperApplication"/> class.
        /// </summary>
        /// <param name="primaryPackageId">The primary package identifier.</param>
        public SilentBootstrapperApplication(string primaryPackageId)
            : base(typeof(SilentManagedBA).Assembly.Location)
        {
            PrimaryPackageId = primaryPackageId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SilentBootstrapperApplication"/> class.
        /// </summary>
        public SilentBootstrapperApplication()
            : base(typeof(SilentManagedBA).Assembly.Location)
        {
        }

        /// <summary>
        /// Automatically generates required sources files for building the Bootstrapper. It is
        /// used to automatically generate the files which, can be generated automatically without
        /// user involvement (e.g. BootstrapperCore.config).
        /// </summary>
        /// <param name="outDir">The output directory.</param>
        public override void AutoGenerateSources(string outDir)
        {
            if (PrimaryPackageId != null)
            {
                Variable newDef = new Variable(SilentManagedBA.PrimaryPackageIdVariableName, PrimaryPackageId);
                if (!Array.Exists(Variables, variable => variable == newDef))
                    Variables = Variables.Combine(newDef);
            }
            base.AutoGenerateSources(outDir);
        }

        /// <summary>
        /// Gets or sets the downgrade warning message. The message is displayed when bundle
        /// detects a newer version of primary package is installed and the setup is about to exit.
        /// <para>The default value is  "A later version of the package (PackageId: {0}) is already
        /// installed. Setup will now exit.".
        /// </para>
        /// </summary>
        /// <value>
        /// The downgrade warning message.
        /// </value>
        public string DowngradeWarningMessage { get; set; }
    }

    /// <summary>
    /// Implements canonical WiX managed bootstrapper application without any UI.
    /// </summary>
    public class SilentManagedBA : BootstrapperApplication
    {
        AutoResetEvent done = new AutoResetEvent(false);

        static internal string PrimaryPackageIdVariableName = "_WixSharp.Bootstrapper.SilentManagedBA.PrimaryPackageId";

        string PrimaryPackageId
        {
            get => this.Engine.StringVariables.Get(PrimaryPackageIdVariableName);
        }

        /// <summary>
        /// Entry point that is called when the Bootstrapper application is ready to run.
        /// </summary>
        protected override void Run()
        {
            Environment.SetEnvironmentVariable("WIXSHARP_SILENT_BA_PROC_ID", Process.GetCurrentProcess().Id.ToString());

            if (PrimaryPackageId == null)
            {
                MessageBox.Show($"SilentBootstrapperApplication.PrimaryPackageId is not set", this.Engine.StringVariables["WixBundleName"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                ApplyComplete += OnApplyComplete;
                DetectComplete += OnDetectComplete;
                DetectPackageComplete += OnDetectPackageComplete;
                PlanComplete += OnPlanComplete;
                Engine.Detect();
                done.WaitOne();
            }
            Engine.Quit(0);
        }

        private void OnDetectComplete(object sender, DetectCompleteEventArgs e)
        {
            if (this.Command.Action == LaunchAction.Uninstall)
            {
                Engine.Log(LogLevel.Verbose, "Invoking automatic plan for uninstall");
                Engine.Plan(LaunchAction.Uninstall);
                //Engine.Quit(0); // Doesn't really required
            }
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
        /// If the planning was successful, it instructs the Bootstrapper Engine to
        /// install the packages.
        /// </summary>
        void OnPlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (e.Status >= 0)
                this.Engine.Apply(System.IntPtr.Zero);
        }

        string DowngradeWarningMessage
        {
            get => this.Engine
                       .StringVariables
                       .Get("DowngradeWarningMessage") ??
                            "A later version of the package (PackageId: {0}) is already installed. Setup will now exit.";
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper DetectPackageComplete event is fired.
        /// Checks the PackageId and sets the installation scenario. The PackageId is the ID
        /// specified in one of the package elements (msipackage, exepackage, msppackage,
        /// msupackage) in the WiX bundle.
        /// </summary>
        void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId == PrimaryPackageId)
            {
                switch (e.State)
                {
                    case PackageState.Obsolete:
                        this.Engine.Log(LogLevel.Error, string.Format(DowngradeWarningMessage, e.PackageId));
                        MessageBox.Show(string.Format(DowngradeWarningMessage, e.PackageId), this.Engine.StringVariables["WixBundleName"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Engine.Quit(0);
                        break;

                    case PackageState.Absent:
                        this.Engine.Plan(LaunchAction.Install);
                        break;

                    case PackageState.Present:
                        this.Engine.Plan(LaunchAction.Uninstall);
                        break;
                }
            }
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper ApplyComplete event is fired.
        /// This is called after a bundle installation has completed. Make sure we updated the view.
        /// </summary>
        void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            done.Set();
        }
    }
}