using sys = System.IO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using WixSharp.CommonTasks;
using System.Text;

namespace WixSharp.Bootstrapper
{
    //Useful stuff to have a look at:
    //http://neilsleightholm.blogspot.com.au/2012/05/wix-burn-tipstricks.html
    //https://wixwpf.codeplex.com/

    /// <summary>
    /// Class for defining a WiX standard Burn-based bootstrapper. By default the bootstrapper is using WiX default WiX bootstrapper UI.
    /// </summary>
    /// <example>The following is an example of defining a bootstrapper for two msi files and .NET Web setup.
    /// <code>
    ///  var bootstrapper =
    ///      new Bundle("My Product",
    ///          new PackageGroupRef("NetFx40Web"),
    ///          new MsiPackage("productA.msi"),
    ///          new MsiPackage("productB.msi"));
    ///
    /// bootstrapper.AboutUrl = "https://github.com/oleg-shilo/wixsharp/";
    /// bootstrapper.IconFile = "app_icon.ico";
    /// bootstrapper.Version = new Version("1.0.0.0");
    /// bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
    /// bootstrapper.Application.LogoFile = "logo.png";
    ///
    /// bootstrapper.Build();
    /// </code>
    /// </example>
    public class Bundle : WixProject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        public Bundle()
        {
            if (!Compiler.AutoGeneration.LegacyDefaultIdAlgorithm)
            {
                // in case of Bundle project just do nothing
            }

            this.Include(WixExtension.NetFx);
            this.Include(WixExtension.Bal);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper" /> class.
        /// </summary>
        /// <param name="name">The name of the project. Typically it is the name of the product to be installed.</param>
        /// <param name="items">The project installable items (e.g. directories, files, registry keys, Custom Actions).</param>
        public Bundle(string name, params ChainItem[] items)
        {
            if (!Compiler.AutoGeneration.LegacyDefaultIdAlgorithm)
            {
                // in case of Bundle project just do nothing
            }

            this.Include(WixExtension.NetFx);
            this.Include(WixExtension.Bal);
            Name = name;
            Chain.AddRange(items);
        }

        /// <summary>
        /// The disable rollbackSpecifies whether the bundle will attempt to rollback packages executed in the chain.
        /// If "true" is specified then when a vital package fails to install only that package will rollback and the chain will stop with the error.
        /// The default is "false" which indicates all packages executed during the chain will be rollback to their previous state when a vital package fails.
        /// </summary>
        public bool? DisableRollback;

        /// <summary>
        /// Specifies whether the bundle will attempt to create a system restore point when executing the chain. If "true" is specified then a system restore
        /// point will not be created. The default is "false" which indicates a system restore point will be created when the bundle is installed, uninstalled,
        /// repaired, modified, etc. If the system restore point cannot be created, the bundle will log the issue and continue.
        /// </summary>
        public bool? DisableSystemRestore;

        /// <summary>
        /// Specifies whether the bundle will start installing packages while other packages are still being cached.
        /// If "true", packages will start executing when a rollback boundary is encountered. The default is "false"
        /// which dictates all packages must be cached before any packages will start to be installed.
        /// </summary>
        public bool? ParallelCache;

        /// <summary>
        /// The legal copyright found in the version resources of final bundle executable.
        /// If this attribute is not provided the copyright will be set to "Copyright (c) [Bundle/@Manufacturer]. All rights reserved.".
        /// </summary>
        [Xml]
        public string Copyright;

        /// <summary>
        /// A URL for more information about the bundle to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string AboutUrl;

        /// <summary>
        /// Whether Packages and Payloads not assigned to a container should be added to the default attached container or if they
        /// should be external. The default is yes.
        /// </summary>
        [Xml]
        public bool? Compressed;

        /// <summary>
        /// The condition of the bundle. If the condition is not met, the bundle will refuse to run. Conditions are checked before the
        /// bootstrapper application is loaded (before detect), and thus can only reference built-in variables such as variables which
        /// indicate the version of the OS.
        /// </summary>
        [Xml]
        public string Condition;

        /// <summary>
        /// Parameters of digitally sign
        /// </summary>
        public DigitalSignatureBootstrapper DigitalSignature;

        /// <summary>
        /// Determines whether the bundle can be removed via the Programs and Features (also known as Add/Remove Programs). If the value is
        /// "yes" then the "Uninstall" button will not be displayed. The default is "no" which ensures there is an "Uninstall" button to remove
        /// the bundle. If the "DisableModify" attribute is also "yes" or "button" then the bundle will not be displayed in Programs and
        /// Features and another mechanism (such as registering as a related bundle addon) must be used to ensure the bundle can be removed.
        /// </summary>
        [Xml]
        public bool? DisableRemove;

        /// <summary>
        /// Determines whether the bundle can be modified via the Programs and Features (also known as Add/Remove Programs). If the value is
        /// "button" then Programs and Features will show a single "Uninstall/Change" button. If the value is "yes" then Programs and Features
        /// will only show the "Uninstall" button". If the value is "no", the default, then a "Change" button is shown. See the DisableRemove
        /// attribute for information how to not display the bundle in Programs and Features.
        /// </summary>
        [Xml]
        public string DisableModify;

        /// <summary>
        /// A telephone number for help to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string HelpTelephone;

        /// <summary>
        /// A URL to the help for the bundle to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string HelpUrl;

        /// <summary>
        /// Path to an icon that will replace the default icon in the final Bundle executable. This icon will also be displayed in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml(Name = "IconSourceFile")]
        public string IconFile;

        /// <summary>
        /// The publisher of the bundle to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string Manufacturer;

        /// <summary>
        /// The name of the parent bundle to display in Installed Updates (also known as Add/Remove Programs). This name is used to nest or group bundles that will appear as updates. If the
        /// parent name does not actually exist, a virtual parent is created automatically.
        /// </summary>
        [Xml]
        public string ParentName;

        /// <summary>
        /// Path to a bitmap that will be shown as the bootstrapper application is being loaded. If this attribute is not specified, no splash screen will be displayed.
        /// </summary>
        [Xml(Name = "SplashScreenSourceFile")]
        public string SplashScreenSource;

        /// <summary>
        /// Set this string to uniquely identify this bundle to its own BA, and to related bundles. The value of this string only matters to the BA, and its value has no direct
        /// effect on engine functionality.
        /// </summary>
        [Xml]
        public string Tag;

        /// <summary>
        /// A URL for updates of the bundle to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string UpdateUrl;

        /// <summary>
        /// Unique identifier for a family of bundles. If two bundles have the same UpgradeCode the bundle with the highest version will be installed.
        /// </summary>
        [Xml]
        public Guid UpgradeCode = Guid.NewGuid();

        /// <summary>
        /// The suppress auto insertion of WixMbaPrereq* variables in the bundle definition (WixMbaPrereqPackageId and WixMbaPrereqLicenseUrl).
        /// <para>BA is relying on two internal variables that reflect .NET version (and license) that BA requires at runtime. If user defines
        /// custom Wix# based BA the required variables are inserted automatically, similarly to the standards WiX/Burn BA. However some other
        /// bundle packages (e.g. new PackageGroupRef("NetFx40Web")) may also define these variables so some duplication/collision is possible.
        /// To avoid this you can suppress variables auto-insertion and define them manually as needed.</para>
        /// </summary>
        ///<example>The following is an example of suppressing auto-insertion:
        /// <code>
        /// var bootstrapper = new Bundle("My Product Suite",
        ///                        new PackageGroupRef("NetFx40Web"),
        ///                        new MsiPackage(productMsi)
        ///                        {
        ///                            Id = "MyProductPackageId",
        ///                            DisplayInternalUI = true
        ///                        });
        ///
        /// bootstrapper.SuppressWixMbaPrereqVars = true;
        /// </code>
        /// </example>
        public bool SuppressWixMbaPrereqVars = false;

        /// <summary>
        /// The version of the bundle. Newer versions upgrade earlier versions of the bundles with matching UpgradeCodes. If the bundle is registered in Programs and Features then this attribute will be displayed in the Programs and Features user interface.
        /// </summary>
        [Xml]
        public Version Version;

        /// <summary>
        /// The sequence of the packages to be installed
        /// </summary>
        public List<ChainItem> Chain = new List<ChainItem>();

        /// <summary>
        /// The instance of the Bootstrapper application class application. By default it is a LicenseBootstrapperApplication object.
        /// </summary>
        public WixStandardBootstrapperApplication Application = new LicenseBootstrapperApplication();

        /// <summary>
        /// The generic items to be added to the WiX <c>Bundle</c> element during the build. A generic item must implement
        /// <see cref="IGenericEntity"/> interface;
        ///<example>The following is an example of extending WixSharp Bundle with otherwise not supported Bal:Condition:
        /// <code>
        /// class BalCondition : IXmlBuilder
        /// {
        ///     public string Condition;
        ///     public string Message;
        ///
        ///     public XContainer[] ToXml()
        ///     {
        ///         return new XContainer[] {
        ///             new XElement(WixExtension.Bal.ToXName("Condition"), Condition)
        ///                         .SetAttribute("Message", Message) };
        ///     }
        /// }
        ///
        /// var bootstrapper = new Bundle("My Product Suite", ...
        /// bootstrapper.GenericItems.Add(new BalCondition { Condition = "some condition", Message = "Warning: ....." });
        /// </code>
        /// </example>
        /// </summary>
        public List<IGenericEntity> GenericItems = new List<IGenericEntity>();

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XContainer[] ToXml()
        {
            var result = new List<XContainer>();

            var root = new XElement("Bundle",
                           new XAttribute("Name", Name));

            root.AddAttributes(this.Attributes);
            root.Add(this.MapToXmlAttributes());

            if (Application is ManagedBootstrapperApplication app)
            {
                if (app.PrimaryPackageId == null)
                {
                    var lastPackage = Chain.OfType<WixSharp.Bootstrapper.Package>().LastOrDefault();
                    if (lastPackage != null)
                    {
                        lastPackage.EnsureId();
                        app.PrimaryPackageId = lastPackage.Id;
                    }
                }

                //addresses https://wixsharp.codeplex.com/workitem/149
                if (!SuppressWixMbaPrereqVars)
                {
                    WixVariables["WixMbaPrereqPackageId"] = "Netfx4Full";
                    WixVariables.Add("WixMbaPrereqLicenseUrl", "NetfxLicense.rtf");
                }
            }

            //important to call AutoGenerateSources after PrimaryPackageId is set
            Application.AutoGenerateSources(this.OutDir);

            root.Add(Application.ToXml());

            var all_variabes = new List<Variable>();
            all_variabes.AddRange(this.Variables);
            all_variabes.AddRange(Application.Variables);

            if (Application is IWixSharpManagedBootstrapperApplication wsApp)
                if (wsApp.DowngradeWarningMessage.IsNotEmpty())
                    all_variabes.Add(new Variable("DowngradeWarningMessage", wsApp.DowngradeWarningMessage));

            Compiler.ProcessWixVariables(this, root);

            var context = new ProcessingContext
            {
                Project = this,
                Parent = this,
                XParent = root,
            };

            foreach (IGenericEntity item in all_variabes)
                item.Process(context);

            foreach (IGenericEntity item in GenericItems)
                item.Process(context);

            var xChain = root.AddElement("Chain");
            foreach (var item in this.Chain)
                xChain.Add(item.ToXml());

            xChain.SetAttribute("DisableRollback", DisableRollback);
            xChain.SetAttribute("DisableSystemRestore", DisableSystemRestore);
            xChain.SetAttribute("ParallelCache", ParallelCache);

            result.Add(root);
            return result.ToArray();
        }

        /// <summary>
        /// The Bundle string variables.
        /// </summary>
        /// <para>The variables are defined as a named values map.</para>
        /// <para>If you need to access the variable value from the Package
        /// you will need to add the MsiProperty mapped to this variable.
        /// </para>
        /// <example>
        /// <code>
        /// new ManagedBootstrapperApplication("ManagedBA.dll")
        /// {
        ///     Variables = "FullInstall=Yes; Silent=No".ToStringVariables()
        /// }
        /// ...
        /// new MsiPackage(msiFile) { MsiProperties = "FULL=[FullInstall]" },
        /// </code>
        /// </example>
        public Variable[] Variables = new Variable[0];

        /// <summary>
        /// Builds WiX Bootstrapper application from the specified <see cref="Bundle" /> project instance.
        /// </summary>
        /// <param name="path">The path to the bootstrapper to be build.</param>
        /// <returns></returns>
        public string Build(string path = null)
        {
            if (!Compiler.AutoGeneration.LegacyDefaultIdAlgorithm)
            {
                // in case of Bundle project just do nothing
            }

            var output = new StringBuilder();
            Action<string> collect = line => output.AppendLine(line);

            Compiler.OutputWriteLine += collect;
            try
            {
                if (Compiler.ClientAssembly.IsEmpty())
                    Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

                if (path == null)
                    return Compiler.Build(this);
                else
                    return Compiler.Build(this, path);
            }
            finally
            {
                ValidateCompileOutput(output.ToString());
                Compiler.OutputWriteLine -= collect;
            }
        }

        void ValidateCompileOutput(string output)
        {
            if (!this.SuppressWixMbaPrereqVars && output.Contains("'WixMbaPrereqPackageId' is declared in more than one location."))
            {
                Compiler.OutputWriteLine("======================================================");
                Compiler.OutputWriteLine("");
                Compiler.OutputWriteLine("WARNING: It looks like one of the packages defines " +
                                         "WixMbaPrereqPackageId/WixMbaPrereqLicenseUrl in addition to the definition " +
                                         "auto-inserted by Wix# managed BA. If it is the case set your Bundle project " +
                                         "SuppressWixMbaPrereqVars to 'true' to fix the problem.");
                Compiler.OutputWriteLine("");
                Compiler.OutputWriteLine("======================================================");
            }
            else if (this.SuppressWixMbaPrereqVars && output.Contains("The Windows Installer XML variable !(wix.WixMbaPrereqPackageId) is unknown."))
            {
                Compiler.OutputWriteLine("======================================================");
                Compiler.OutputWriteLine("");
                Compiler.OutputWriteLine("WARNING: It looks like generation of WixMbaPrereqPackageId/WixMbaPrereqLicenseUrl " +
                                         "was suppressed while none of other packages defines it. " +
                                         "If it is the case set your Bundle project SuppressWixMbaPrereqVars to false to fix the problem.");
                Compiler.OutputWriteLine("");
                Compiler.OutputWriteLine("======================================================");
            }
        }

        /// <summary>
        /// Builds the WiX source file and generates batch file capable of building
        /// WiX/MSI bootstrapper with WiX toolset.
        /// </summary>
        /// <param name="path">The path to the batch file to be created.</param>
        /// <returns></returns>
        public string BuildCmd(string path = null)
        {
            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            if (path == null)
                return Compiler.BuildCmd(this);
            else
                return Compiler.BuildCmd(this, path);
        }

        /// <summary>
        /// Validates this Bundle project packages.
        /// </summary>
        public void Validate()
        {
            var msiPackages = this.Chain.Where(x => (x is MsiPackage) && (x as MsiPackage).DisplayInternalUI == true);
            foreach (MsiPackage item in msiPackages)
            {
                try
                {
                    if (Tasks.IsEmbeddedUIPackage(item.SourceFile))
                    {
                        Compiler.OutputWriteLine("");
                        Compiler.OutputWriteLine("WARNING: You have selected enabled DisplayInternalUI for EmbeddedUI-based '"
                            + sys.Path.GetFileName(item.SourceFile) + "'. Currently Burn (WiX) " +
                            "doesn't support integration with EmbeddedUI packages. Read more here: https://github.com/oleg-shilo/wixsharp/wiki/Wix%23-Bootstrapper-(Burn)-integration-notes");

                        Compiler.OutputWriteLine("");
                    }
                }
                catch { }
            }
        }

        internal void ResetAutoIdGeneration(bool supressWarning)
        {
            WixGuid.ConsistentGenerationStartValue = this.UpgradeCode;
            WixEntity.ResetIdGenerator(supressWarning);
        }
    }

    /// <summary>
    /// An interface that needs can be implemented by Silent (no-UI) BA.
    /// </summary>
    public interface IWixSharpManagedBootstrapperApplication
    {
        /// <summary>
        /// Gets or sets the downgrade warning message. The message is displayed when bundle
        /// detects a newer version of primary package is installed and the setup is about to exit.
        /// </summary>
        /// <value>
        /// The downgrade warning message.
        /// </value>
        string DowngradeWarningMessage { get; set; }
    }

    /*
      <Bundle Name="My Product"
            Version="1.0.0.0"
            Manufacturer="OSH"
            AboutUrl="https://github.com/oleg-shilo/wixsharp/"
            IconSourceFile="app_icon.ico"
            UpgradeCode="acaa3540-97e0-44e4-ae7a-28c20d410a60">

        <BootstrapperApplicationRef Id="WixStandardBootstrapperApplication.RtfLicense">
            <bal:WixStandardBootstrapperApplication LicenseFile="readme.txt" LocalizationFile="" LogoFile="app_icon.ico" />
        </BootstrapperApplicationRef>

        <Chain>
            <!-- Install .Net 4 Full -->
            <PackageGroupRef Id="NetFx40Web"/>
            <!--<ExePackage
                Id="Netfx4FullExe"
                Cache="no"
                Compressed="no"
                PerMachine="yes"
                Permanent="yes"
                Vital="yes"
                SourceFile="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bootstrapper\Packages\DotNetFX40\dotNetFx40_Full_x86_x64.exe"
                InstallCommand="/q /norestart /ChainingPackage FullX64Bootstrapper"
                DetectCondition="NETFRAMEWORK35='#1'"
                DownloadUrl="http://go.microsoft.com/fwlink/?LinkId=164193" />-->

            <RollbackBoundary />

            <MsiPackage SourceFile="E:\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Managed Setup\ManagedSetup.msi" Vital="yes" />
        </Chain>
    </Bundle>
     */
}