using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using sys = System.IO;

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
            Payloads = Payloads.Add(AppAssembly)
                               .AddRange(dependencies);
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
                rawAppAssembly = Compiler.ResolveClientAsm(rawAppAssembly, outDir); //NOTE: if a new file is generated then the Compiler takes care for cleaning any temps
                if (Payloads.Contains("%this%"))
                    Payloads = Payloads.Except(new[] { "%this%" }).Concat(new[] { rawAppAssembly }).ToArray();

            }

            string asmName = Path.GetFileNameWithoutExtension(Utils.OriginalAssemblyFile(rawAppAssembly));

            var suppliedConfig = Payloads.FirstOrDefault(x => Path.GetFileName(x).SameAs("BootstrapperCore.config", true));

            bootstrapperCoreConfig = suppliedConfig;
            if (bootstrapperCoreConfig == null)
            {
                bootstrapperCoreConfig = Path.Combine(outDir, "BootstrapperCore.config");

                sys.File.WriteAllText(bootstrapperCoreConfig,
    @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <configSections>
        <sectionGroup name=""wix.bootstrapper"" type=""Microsoft.Tools.WindowsInstallerXml.Bootstrapper.BootstrapperSectionGroup, BootstrapperCore"">
            <section name=""host"" type=""Microsoft.Tools.WindowsInstallerXml.Bootstrapper.HostSection, BootstrapperCore"" />
        </sectionGroup>
    </configSections>
    <startup useLegacyV2RuntimeActivationPolicy=""true"">
        <supportedRuntime version=""v4.0"" />
    </startup>
    <wix.bootstrapper>
        <host assemblyName=""" + asmName + @""">
            <supportedFramework version=""v4\Full"" />
            <supportedFramework version=""v4\Client"" />
        </host>
    </wix.bootstrapper>
</configuration>
");
                Compiler.TempFiles.Add(bootstrapperCoreConfig);
            }
        }

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            string winInstaller = typeof(Session).Assembly.Location;

            var root = new XElement("BootstrapperApplicationRef");
            root.SetAttribute("Id", "ManagedBootstrapperApplicationHost");

            List<string> files = new List<string> { rawAppAssembly, bootstrapperCoreConfig };
            files.AddRange(Payloads.Distinct()); //note %this% it already resolved at this stage into an absolutepath 

            if(!Payloads.Where(x=>Path.GetFileName(x).SameAs(Path.GetFileName(winInstaller))).Any())
                files.Add(winInstaller);

            if (files.Any())
                files.Distinct().ForEach(p => root.Add(new XElement("Payload", new XAttribute("SourceFile", p))));

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
        /// Source file of the logo graphic.
        /// </summary>
        [Xml]
        public string LogoFile;

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

        /// <summary>
        /// Collection of paths to the package dependencies.
        /// </summary>
        public string[] Payloads = new string[0];

        /// <summary>
        /// The Bundle string variables associated with the Bootstrapper application.
        /// <para>The variables are defined as a named values map.</para>
        /// </summary>
        /// <example>
        /// <code>
        /// new ManagedBootstrapperApplication("ManagedBA.dll")
        /// {
        ///     StringVariablesDefinition = "FullInstall=Yes; Silent=No"
        /// }
        /// </code>
        /// </example>
        public string StringVariablesDefinition = "";
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
    /// bootstrapper.AboutUrl = "https://wixsharp.codeplex.com/";
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
            XNamespace bal = "http://schemas.microsoft.com/wix/BalExtension";

            var root = new XElement("BootstrapperApplicationRef");

            var app = this.ToXElement(bal + "WixStandardBootstrapperApplication");
            
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
                        root.AddElement("Payload").AddAttributes("SourceFile=" + LicensePath);
                    }
                }
            }

            root.Add(app);

            return new[] { root };
        }
    }
}