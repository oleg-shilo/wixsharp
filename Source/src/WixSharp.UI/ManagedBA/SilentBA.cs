using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

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
    /// </summary>
    /// <example>
    /// <code>
    ///  var bootstrapper =
    ///      new Bundle("My Product",
    ///          new PackageGroupRef("NetFx40Web"),
    ///          new MsiPackage("product.msi"));
    ///
    /// bootstrapper.AboutUrl = "https://wixsharp.codeplex.com/";
    /// bootstrapper.IconFile = "app_icon.ico";
    /// bootstrapper.Version = new Version("1.0.0.0");
    /// bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
    /// bootstrapper.Application = new SilentBootstrapperApplication();
    ///
    /// bootstrapper.Build();
    /// </code>
    /// </example>
    public class SilentBootstrapperApplication : ManagedBootstrapperApplication
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
                string newDef = SilentManagedBA.PrimaryPackageIdVariableName + "=" + PrimaryPackageId;
                if (!StringVariablesDefinition.Contains(newDef))
                    StringVariablesDefinition += ";" + newDef;
            }
            base.AutoGenerateSources(outDir);
        }
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
            get
            {
                try
                {
                    return this.Engine.StringVariables[PrimaryPackageIdVariableName];
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Entry point that is called when the Bootstrapper application is ready to run.
        /// </summary>
        protected override void Run()
        {
            Environment.SetEnvironmentVariable("WIXSHARP_SILENT_BA_PROC_ID", Process.GetCurrentProcess().Id.ToString());

            if (PrimaryPackageId == null)
            {
                MessageBox.Show(PrimaryPackageIdVariableName + " variable is not set", "Wix#");
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
                if (e.State == PackageState.Absent)
                {
                    this.Engine.Plan(LaunchAction.Install);
                }
                else if (e.State == PackageState.Present)
                {
                    this.Engine.Plan(LaunchAction.Uninstall);
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