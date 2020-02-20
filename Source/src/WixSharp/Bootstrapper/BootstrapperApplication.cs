using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using sys = System.IO;
#if WIX4
// using WixToolset.Bootstrapper;
using WixToolset.Dtf.WindowsInstaller;
#else
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Microsoft.Deployment.WindowsInstaller;
#endif

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Class for defining a Wix# application for WiX standard Burn-based bootstrapper.
    /// <para>It is nothing else but a light container for the WiX metadata associated with the
    /// .NET assembly implementing WiX ManagedBootstrapper application.</para>
    /// </summary>
    public class ManagedBootstrapperApplication : WixStandardBootstrapperApplication
    {
        /// <summary>
        /// The assembly implementing Bootstrapper UI application
        /// </summary>
        public string AppAssembly = "";

        string rawAppAssembly = "";
        string bootstrapperCoreConfig = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedBootstrapperApplication"/> class.
        /// </summary>
        /// <param name="appAssembly">The application assembly.</param>
        /// <param name="dependencies">The dependencies.</param>
        public ManagedBootstrapperApplication(string appAssembly, params string[] dependencies)
        {
            AppAssembly = appAssembly;
            Payloads = Payloads.Combine(AppAssembly.ToPayload())
                               .Combine(dependencies.Select(x => x.ToPayload()));
        }

        /// <summary>
        /// Automatically generates required sources files for building the Bootstrapper. It is
        /// used to automatically generate the files which, can be generated automatically without
        /// user involvement (e.g. BootstrapperCore.config).
        /// </summary>
        /// <param name="outDir">The output directory.</param>
        public override void AutoGenerateSources(string outDir)
        {
            //NOTE: while it is tempting, AutoGenerateSources cannot be called during initialization as it is too early.
            //The call must be triggered by Compiler.Build* calls.
            rawAppAssembly = AppAssembly;
            if (rawAppAssembly.EndsWith("%this%"))
            {
                rawAppAssembly = Compiler.ResolveClientAsm(outDir); //NOTE: if a new file is generated then the Compiler takes care for cleaning any temps
                if (Payloads.FirstOrDefault(x => x.SourceFile == "%this%") is Payload payload_this)
                    payload_this.SourceFile = rawAppAssembly;
            }

            string asmName = Path.GetFileNameWithoutExtension(Utils.OriginalAssemblyFile(rawAppAssembly));

            var suppliedConfig = Payloads.Select(x => x.SourceFile).FirstOrDefault(x => Path.GetFileName(x).SameAs("BootstrapperCore.config", true));

            bootstrapperCoreConfig = suppliedConfig;
            if (bootstrapperCoreConfig == null)
            {
                bootstrapperCoreConfig = Path.Combine(outDir, "BootstrapperCore.config");

                sys.File.WriteAllText(bootstrapperCoreConfig,
                                      DefaultBootstrapperCoreConfigContent.Replace("{asmName}", asmName));

                Compiler.TempFiles.Add(bootstrapperCoreConfig);
            }
        }

        /// <summary>
        /// The default content of the BootstrapperCore.config file. It is used in the cases when the custom config file was not specified
        /// in <see cref="ManagedBootstrapperApplication"/> constructor.
        /// <para>BootstrapperCore.config is very important as its content can affect both bootstrapper build outcome and the
        /// runtime behaviour.</para>
        /// <para>See these discussions: </para>
        /// <para>  - https://github.com/oleg-shilo/wixsharp/issues/416 </para>
        /// <para>  - https://github.com/oleg-shilo/wixsharp/issues/389 </para>
        /// </summary>
        public static string DefaultBootstrapperCoreConfigContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <configSections>
        <sectionGroup name=""wix.bootstrapper"" type=""Microsoft.Tools.WindowsInstallerXml.Bootstrapper.BootstrapperSectionGroup, BootstrapperCore"">
            <section name=""host"" type=""Microsoft.Tools.WindowsInstallerXml.Bootstrapper.HostSection, BootstrapperCore"" />
        </sectionGroup>
    </configSections>
    <startup useLegacyV2RuntimeActivationPolicy=""true"">
        <supportedRuntime version=""v2.0.50727"" />
        <supportedRuntime version=""v4.0"" />
    </startup>
    <wix.bootstrapper>
        <host assemblyName=""{asmName}"">
            <supportedFramework version=""v3.5"" runtimeVersion=""v2.0.50727"" />
            <supportedFramework version=""v4\Full"" />
            <supportedFramework version=""v4\Client"" />
        </host>
    </wix.bootstrapper>
</configuration>
";

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            string winInstaller = typeof(Session).Assembly.Location;

            var root = new XElement("BootstrapperApplicationRef");
            root.SetAttribute("Id", "ManagedBootstrapperApplicationHost");

            var files = new List<Payload> { rawAppAssembly.ToPayload(), bootstrapperCoreConfig.ToPayload() };
            files.AddRange(this.Payloads.DistinctBy(x => x.SourceFile)); //note %this% it already resolved at this stage into an absolute path

            if (!Payloads.Any(x => Path.GetFileName(x.SourceFile).SameAs(Path.GetFileName(winInstaller))))
                files.Add(winInstaller.ToPayload());

            if (files.Any())
                files.DistinctBy(x => x.SourceFile).ForEach(p => root.Add(p.ToXElement("Payload")));

            return new[] { root };
        }

        string primaryPackageId;

        /// <summary>
        /// Gets or sets the IDd of the primary package from the bundle.
        /// <para>This ID is used by the application to detect the presence of the package on the target system
        /// and trigger either install or uninstall action.</para>
        /// <para>If it is not set then it is the Id of the last package in th bundle.</para>
        /// </summary>
        /// <value>
        /// The primary package identifier.
        /// </value>
        public string PrimaryPackageId
        {
            get { return primaryPackageId; }
            set { primaryPackageId = value; }
        }

        //public ChainItem DependencyPackage { get; set; }
    }

#pragma warning disable 169

    /// <summary>
    /// Container class for common members of the Bootstrapper standard applications.
    /// </summary>
    public abstract class WixStandardBootstrapperApplication : WixEntity
    {
        /// <summary>
        /// Source file of the RTF license file or URL target of the license link.
        /// </summary>
        public string LicensePath;

        /// <summary>
        /// If set, WixStdBA will supply these arguments when launching the application specified by the LaunchTarget attribute. The string value can be formatted using Burn variables enclosed in brackets, to refer to installation directories and so forth.
        /// </summary>
        [Xml]
        public string LaunchArguments;

        /// <summary>
        /// If set, the success page will show a Launch button the user can use to launch the application being installed. The string value can be formatted using Burn variables enclosed in brackets, to refer to installation directories and so forth.
        /// </summary>
        [Xml]
        public string LaunchTarget;

        /// <summary>
        /// WixStdBA will use this working folder when launching the specified application. The string value can be formatted using Burn variables enclosed in brackets, to refer to installation directories and so forth. This attribute is ignored when the LaunchTargetElevatedId attribute is specified.
        /// </summary>
        [Xml]
        public string LaunchWorkingFolder;

        /// <summary>
        /// If set to "true", WixStdBA will launch the application specified by the LaunchTarget attribute with the SW_HIDE flag. This attribute is ignored when the LaunchTargetElevatedId attribute is specified.
        /// </summary>
        [Xml]
        bool? LaunchHidden;

        /// <summary>
        /// If set to "true", WixStdBA will show a page allowing the user to restart applications when files are in use.
        /// </summary>
        [Xml]
        bool? ShowFilesInUse;

        /// <summary>
        /// If set to "true", the application version will be displayed on the UI.
        /// </summary>
        [Xml]
        bool? ShowVersion;

        /// <summary>
        /// If set to "true", the bundle can be pre-cached using the /cache command line argument.
        /// </summary>
        [Xml]
        bool? SupportCacheOnly;

        /// <summary>
        /// If set to "true", attempting to installer a downgraded version of a bundle will be treated as a successful do-nothing operation. The default behavior (or when explicitly set to "false") is to treat downgrade attempts as failures.
        /// </summary>
        [Xml]
        bool? SuppressDowngradeFailure;

        /// <summary>
        /// If set to "true", the Options button will not be shown and the user will not be able to choose an installation directory.
        /// </summary>
        [Xml]
        bool? SuppressOptionsUI;

        /// <summary>
        /// If set to "true", the Repair button will not be shown in the maintenance-mode UI.
        /// </summary>
        [Xml]
        bool? SuppressRepair;

        /// <summary>
        /// Source file of the theme XML.
        /// </summary>
        [Xml]
        public string ThemeFile;

        /// <summary>
        /// Source file of the logo graphic.
        /// </summary>
        [Xml]
        public string LogoFile;

        /// <summary>
        /// Source file of the side logo graphic.
        /// </summary>
        [Xml]
        public string LogoSideFile;

        /// <summary>
        /// Source file of the theme localization .wxl file.
        /// </summary>
        [Xml]
        public string LocalizationFile;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public abstract XContainer[] ToXml();

        /// <summary>
        /// Automatically generates required sources files for building the Bootstrapper. It is
        /// used to automatically generate the files which, can be generated automatically without
        /// user involvement (e.g. BootstrapperCore.config).
        /// </summary>
        /// <param name="outDir">The output directory.</param>
        public virtual void AutoGenerateSources(string outDir)
        {
        }

        // http://wixtoolset.org/documentation/manual/v3/xsd/wix/payload.html
        /// <summary>
        /// Collection of paths to the package dependencies.
        /// </summary>
        public Payload[] Payloads = new Payload[0];

        /// <summary>
        /// The Bundle string variables associated with the Bootstrapper application.
        /// <para>The variables are defined as a named values map.</para>
        /// </summary>
        /// <example>
        /// <code>
        /// new ManagedBootstrapperApplication("ManagedBA.dll")
        /// {
        ///     Variables = "FullInstall=Yes; Silent=No".ToStringVariables()
        /// }
        /// </code>
        /// </example>
        public Variable[] Variables = new Variable[0];
    }

    /// <summary>
    /// Describes a remote payload to a bootstrapper.
    /// <para>Describes information about a remote file payload that is not
    /// available at the time of building the bundle. The parent must specify DownloadUrl
    /// and must not specify SourceFile when using this element.</para></summary>
    /// <seealso cref="WixSharp.WixEntity" />
    public class RemotePayload : WixEntity
    {
        /// <summary>
        /// Description of the file from version resources.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// Public key of the authenticode certificate used to sign the RemotePayload.Include this attribute if the remote file is signed.
        /// </summary>
        [Xml]
        public string CertificatePublicKey;

        /// <summary>
        /// Thumbprint of the authenticode certificate used to sign the RemotePayload.Include this attribute if the remote file is signed.
        /// </summary>
        [Xml]
        public string CertificateThumbprint;

        /// <summary>
        /// SHA-1 hash of the RemotePayload.Include this attribute if the remote file is unsigned or SuppressSignatureVerification is set to Yes.
        /// </summary>
        [Xml]
        public string Hash;

        /// <summary>
        /// Product name of the file from version resources.
        /// </summary>
        [Xml]
        public string ProductName;

        /// <summary>
        /// Size of the remote file in bytes.
        /// </summary>
        [Xml]
        public int Size;

        /// <summary>
        /// Version of the remote file
        /// </summary>
        [Xml]
        public Version Version;
    }

    /// <summary>
    /// Describes a payload to a bootstrapper.
    /// </summary>
    /// <seealso cref="WixSharp.WixEntity" />
    public class Payload : WixEntity
    {
        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixEntity" />.
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para><para>If the <see cref="Id" /> value is not specified explicitly by the user the Wix# compiler
        /// generates it automatically insuring its uniqueness.</para><remarks>
        /// Note: The ID auto-generation is triggered on the first access (evaluation) and in order to make the id
        /// allocation deterministic the compiler resets ID generator just before the build starts. However if you
        /// accessing any auto-id before the Build*() is called you can it interferes with the ID auto generation and eventually
        /// lead to the WiX ID duplications. To prevent this from happening either:"
        /// <para> - Avoid evaluating the auto-generated IDs values before the call to Build*()</para><para> - Set the IDs (to be evaluated) explicitly</para><para> - Prevent resetting auto-ID generator by setting WixEntity.DoNotResetIdGenerator to true";</para></remarks>
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [Xml]
        public new string Id
        {
            get
            {
                if (base.RawId.IsEmpty() && Compiler.AutoGeneration.SuppressForBundlePayloadUndefinedIds)
                    return null;
                else
                    return base.Id;
            }
            set { base.Id = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payload"/> class.
        /// </summary>
        public Payload() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payload"/> class.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        public Payload(string sourceFile) { SourceFile = sourceFile; }

        /// <summary>
        /// The URL to use to download the package. The following substitutions are supported:
        /// <para>  �{0} is replaced by the package Id. </para>
        /// <para>  �{1} is replaced by the payload Id. </para>
        /// <para>  �{2} is replaced by the payload file name. </para>
        /// </summary>
        [Xml]
        public string DownloadUrl;

        /// <summary>
        /// The destination path and file name for this payload.
        /// The default is the source file name. The use of '..' directories is not allowed
        /// </summary>
        [Xml]
        public new string Name;

        /// <summary>
        /// Location of the source file.
        /// </summary>
        [Xml]
        public string SourceFile;

        /// <summary>
        /// By default, a Bundle will use a package's Authenticode signature to verify the contents.
        /// If the package does not have an Authenticode signature then the Bundle will use a hash
        /// of the package instead. Set this attribute to "yes" to suppress the default behavior and
        /// force the Bundle to always use the hash of the package even when the package is signed.
        /// </summary>
        [Xml]
        public bool? SuppressSignatureVerification;

        /// <summary>
        /// Whether the payload should be embedded in a container or left as an external payload.
        /// </summary>
        [Xml]
        public bool? Compressed;
    }

    /// <summary>
    /// Generic License-based WiX bootstrapper application.
    /// <para>Depending on the value of LicensePath compiler will resolve the application in either <c>WixStandardBootstrapperApplication.RtfLicense</c>
    /// or <c>WixStandardBootstrapperApplication.HyperlinkLicense</c> standard application.</para>
    /// <para>Note: empty LicensePath will suppress displaying the license completely</para>
    /// </summary>
    /// <example>The following is an example of defining a simple bootstrapper displaying the license as an
    /// embedded HTML file.
    /// <code>
    /// var bootstrapper = new Bundle("My Product",
    ///                         new PackageGroupRef("NetFx40Web"),
    ///                         new MsiPackage(productMsi) { DisplayInternalUI = true });
    ///
    /// bootstrapper.AboutUrl = "https://github.com/oleg-shilo/wixsharp/";
    /// bootstrapper.IconFile = "app_icon.ico";
    /// bootstrapper.Version = new Version("1.0.0.0");
    /// bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
    /// bootstrapper.Application.LogoFile = "logo.png";
    /// bootstrapper.Application.LicensePath = "licence.html";
    ///
    /// bootstrapper.Build();
    /// </code>
    /// </example>
    public class LicenseBootstrapperApplication : WixStandardBootstrapperApplication
    {
        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            //http://wixtoolset.org/schemas/v4/wxs/bal
            //XNamespace bal = "http://schemas.microsoft.com/wix/BalExtension";
            XNamespace bal = "http://wixtoolset.org/schemas/v4/wxs/bal";

            var root = new XElement("BootstrapperApplicationRef");

            var app = this.ToXElement(bal + "WixStandardBootstrapperApplication");

            var payloads = this.Payloads.ToList();

            if (LicensePath.IsNotEmpty() && LicensePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
            {
                root.SetAttribute("Id", "WixStandardBootstrapperApplication.RtfLicense");
                app.SetAttribute("LicenseFile", LicensePath);
            }
            else
            {
                root.SetAttribute("Id", "WixStandardBootstrapperApplication.HyperlinkLicense");

                if (LicensePath.IsEmpty())
                {
                    //cannot use SetAttribute as we want to preserve empty attrs
                    app.Add(new XAttribute("LicenseUrl", ""));
                }
                else
                {
                    if (LicensePath.StartsWith("http")) //online HTML file
                    {
                        app.SetAttribute("LicenseUrl", LicensePath);
                    }
                    else
                    {
                        app.SetAttribute("LicenseUrl", System.IO.Path.GetFileName(LicensePath));
                        payloads.Add(new Payload(LicensePath));
                    }
                }

                foreach (Payload item in payloads)
                {
                    var xml = item.ToXElement("Payload");
                    root.AddElement(xml);
                }
            }

            root.Add(app);

            return new[] { root };
        }
    }
}