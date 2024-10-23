using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Xml.Linq;
using WixSharp.CommonTasks;
using WixSharp.Nsis;
using WixSharp.UI;

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Container class for common members of the Bootstrapper packages
    /// </summary>
    public abstract class Package : ChainItem
    {
        /// <summary>
        /// The identifier of another package that this one should be installed after. By default the After attribute is
        /// set to the previous sibling package in the Chain or PackageGroup element. If this attribute is specified
        /// ensure that a cycle is not created explicitly or implicitly.
        /// </summary>
        [Xml]
        public string After;

        /// <summary>
        /// By default, a Bundle will use the hash of a package to verify its contents. If this attribute is explicitly
        /// set to "no" and the package is signed with an Authenticode signature the Bundle will verify the contents of
        /// the package using the signature instead. Therefore, the default for this attribute could be considered to be
        /// "true". It is unusual for "true" to be the default of an attribute. In this case, the default was changed in
        /// WiX v3.9 after experiencing real-world issues with Windows verifying Authenticode signatures. Since the
        /// Authenticode signatures are no more secure than hashing the packages directly, the default was changed.
        /// </summary>
        [Xml]
        public bool? SuppressSignatureVerification;

        /// <summary>
        /// Specifies the display name to place in the bootstrapper application data manifest for the package.
        /// By default, ExePackages use the ProductName field from the version information, MsiPackages use the ProductName property, and MspPackages use the DisplayName patch metadata property.
        /// Other package types must use this attribute to define a display name in the bootstrapper application data manifest.
        /// </summary>
        [Xml]
        public string DisplayName;

        /// <summary>
        /// Specifies whether the package can be uninstalled. The default is "no".
        /// </summary>
        [Xml]
        public bool? Permanent;

        /// <summary>
        /// Location of the package to add to the bundle. The default value is the Name attribute, if provided. At a minimum, the SourceFile or Name attribute must be specified.
        /// </summary>
        [Xml]
        public string SourceFile;

        /// <summary>
        /// Specifies the description to place in the bootstrapper application data manifest for the package. By default,
        /// ExePackages use the FileName field from the version information, MsiPackages use the ARPCOMMENTS property, and MspPackages
        /// use the Description patch metadata property. Other package types must use this attribute to define a description in the
        /// bootstrapper application data manifest.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// The URL to use to download the package. The following substitutions are supported:
        /// <para>{0} is replaced by the package Id.</para>
        /// <para>{1} is replaced by the payload Id.</para>
        /// <para>{2} is replaced by the payload file name.</para>
        /// </summary>
        [Xml]
        public string DownloadUrl;

        /// <summary>
        /// A condition to evaluate before installing the package. The package will only be installed if the condition evaluates to true.
        /// If the condition evaluates to false and the bundle is being installed, repaired, or modified, the package will be uninstalled.
        /// </summary>
        [Xml]
        public string InstallCondition;

        /// <summary>
        /// Whether the package payload should be embedded in a container or left as an external payload.
        /// </summary>
        [Xml]
        public bool? Compressed;

        /// <summary>
        /// Whether to cache the package. The default is "yes".
        /// </summary>
        [Xml]
        public bool? Cache;

        /// <summary>
        /// Name of a Variable that will hold the path to the log file.
        /// An empty value will cause the variable to not be set.
        /// The default is "WixBundleLog_[PackageId]" except for MSU packages which default to no logging.
        /// </summary>
        [Xml]
        public string LogPathVariable;

        /// <summary>
        /// Name of a Variable that will hold the path to the log file used during rollback.
        /// An empty value will cause the variable to not be set.
        /// The default is "WixBundleRollbackLog_[PackageId]" except for MSU packages which default to no logging.
        /// </summary>
        [Xml]
        public string RollbackLogPathVariable;

        /// <summary>
        /// Collection of Payloads (the package dependencies).
        /// </summary>
        /// <example>
        /// <code>
        ///  var bootstrapper =
        ///      new Bundle("My Product",
        ///          new MsiPackage(productMsi)
        ///          {
        ///              DisplayInternalUI = true,
        ///              Payloads = new[] {
        ///                                   "script.dll".ToPayload()
        ///                                   "utils.dll".ToPayload()
        ///                               }
        ///              ...
        /// </code>
        /// </example>
        public Payload[] Payloads = new Payload[0];

        internal void EnsureId()
        {
            if (!base.IsIdSet())
            {
                if (Name.IsEmpty())
                    Name = System.IO.Path.GetFileName(SourceFile);
            }
        }
    }

    /// <summary>
    /// Container class for common members of the Bootstrapper chained items
    /// </summary>
    public abstract class ChainItem : WixEntity
    {
        /// <summary>
        /// Specifies whether the package/item must succeed for the chain to continue.
        /// The default "yes" (true) indicates that if the package fails then the chain will fail and rollback or stop.
        /// If "no" is specified then the chain will continue even if the package reports failure.
        /// </summary>
        [Xml]
        public bool? Vital;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        abstract public XContainer[] ToXml();
    }

    /// <summary>
    /// Protocol enum for ExePackage. It's an equivalent of WiX `BurnExeProtocolType`
    /// </summary>
    /// <seealso cref="WixSharp.StringEnum{T}" />
    public class Protocol : StringEnum<Protocol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Protocol(string value) : base(value)
        {
        }

        /// <summary>
        /// The executable package does not support a communication protocol.
        /// </summary>
        public static Protocol none = new Protocol("none");

        /// <summary>
        /// The executable package implements the Burn communication protocol.
        /// </summary>
        public static Protocol burn = new Protocol("burn");

        /// <summary>
        /// The executable package implements the .NET Framework v4.0 communication protocol.
        /// </summary>
        public static Protocol netfx4 = new Protocol("netfx4");
    }

    /// <summary>
    /// Specialized class for embedding MSI into a EXE launcher and then adding it as ExePackage to the bundle..
    /// </summary>
    /// <seealso cref="WixSharp.Bootstrapper.ExePackage" />
    public class MsiExePackage : ExePackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsiExePackage"/> class.
        /// </summary>
        /// <param name="msi">The msi.</param>
        public MsiExePackage(string msi)
        {
            var msi_exe = msi + ".exe";

            (int exitCode, string output) = msi.CompileSelfHostedMsi(msi_exe);
            if (exitCode != 0)
            {
                Compiler.OutputWriteLine("Error: " + output);
                return;
            }
            SourceFile = msi_exe;
            InstallArguments = "/i";
            UninstallArguments = "/x";
            RepairArguments = "/fa";
            Compressed = true;
            productCode = new MsiParser(msi).GetProductCode();
        }

        string productCode;

        /// <summary>
        /// Gets the product code.
        /// </summary>
        /// <value>
        /// The product code.
        /// </value>
        public string ProductCode => productCode;

        /// <summary>
        /// Gets the detect condition variable.
        /// </summary>
        /// <value>
        /// The detect condition variable.
        /// </value>
        public string DetectConditionVariable => $"{base.Name}State";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public new string Name
        {
            get => base.Name;
            set
            {
                base.Name = value;
                DetectCondition = $"({DetectConditionVariable} <> \"2\")"; // state
                /*
                    assignment: Saves the assignment type of the product: per-user (0), or per-machine (1).
                    language: Saves the language of a matching product if found; empty otherwise.
                    state: Saves the state of the product: advertised (1), absent (2), or locally installed (5).
                    version: Saves the version of a matching product if found; 0.0.0.0 otherwise. This is the default.
                 */
            }
        }
    }

    /// <summary>
    /// Standard WiX ExePackage.
    /// </summary>
    public class ExePackage : Package
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExePackage"/> class.
        /// </summary>
        public ExePackage()
        {
            ExitCodes = new List<ExitCode>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExePackage"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public ExePackage(string path) : this()
        {
            //Name = System.IO.Path.GetFileName(path).Expand();
            SourceFile = path;
        }

        /// <summary>
        /// The command-line arguments provided to the ExePackage during install. If this attribute
        /// is absent the executable will be launched with no command-line arguments
        /// </summary>
        [Xml]
        public string InstallArguments;

        /// <summary>
        /// Gets or sets the install command.
        /// </summary>
        /// <value>
        /// The install command.
        /// </value>
        [Obsolete("Use `InstallArguments` instead")]
        public string InstallCommand
        {
            set => InstallArguments = value;
            get => InstallArguments;
        }

        /// <summary>
        /// The command-line arguments to specify to indicate a repair. If the executable package can be repaired but does not require any
        /// special command-line arguments to do so then set the attribute's value to blank. To indicate that the package does not support repair,
        /// omit this attribute.
        /// </summary>
        [Xml]
        public string RepairArguments;

        /// <summary>
        /// Gets or sets the repair command.
        /// </summary>
        /// <value>
        /// The repair command.
        /// </value>
        [Obsolete("Use `RepairArguments` instead")]
        public string RepairCommand
        {
            set => RepairArguments = value;
            get => RepairArguments;
        }

        /// <summary>
        /// The command-line arguments provided to the ExePackage during uninstall. If this attribute is absent the executable will be launched
        /// with no command-line arguments. To prevent an ExePackage from being uninstalled set the Permanent attribute to "yes".
        /// </summary>
        [Xml]
        public string UninstallArguments;

        /// <summary>
        /// Gets or sets the uninstall command.
        /// </summary>
        /// <value>
        /// The uninstall command.
        /// </value>
        [Obsolete("Use `UninstallArguments` instead")]
        public string UninstallCommand
        {
            set => UninstallArguments = value;
            get => UninstallArguments;
        }

        /// <summary>
        /// Indicates the package must be executed elevated. The default is "no".
        /// </summary>
        [Xml]
        public bool? PerMachine;

        /// <summary>
        /// Indicates the communication protocol the package supports for extended progress and error reporting. The default is `none`.
        /// </summary>
        [Xml]
        public Protocol Protocol;

        /// <summary>
        /// A condition that determines if the package is present on the target system.
        /// This condition can use built-in variables and variables returned by searches.
        /// This condition is necessary because Windows doesn't provide a method to detect the presence of an ExePackage.
        /// Burn uses this condition to determine how to treat this package during a bundle action; for example, if this condition
        /// is false or omitted and the bundle is being installed, Burn will install this package.
        /// </summary>
        [Xml]
        public string DetectCondition;

        /// <summary>
        /// Describes map of exit code returned from executable package to a bootstrapper behavior.
        ///http://wixtoolset.org/documentation/manual/v3/xsd/wix/exitcode.html
        /// </summary>
        public List<ExitCode> ExitCodes;

        /// <summary>
        /// Collection of RemotePayloads (the package dependencies).
        /// </summary>
        /// <example>
        /// <code>
        ///  var bootstrapper =
        ///      new Bundle("My Product",
        ///          new ExePackage()
        ///          {
        ///              ...,
        ///              RemotePayloads = new[] {
        ///                                         new RemotePayload
        ///                                         {
        ///                                             Size=50352408,
        ///                                             ...
        ///                                         }
        ///                                     }
        ///              ...
        /// </code>
        /// </example>
        [Obsolete("Use `Payloads` instead.")]
        public RemotePayload[] RemotePayloads = new RemotePayload[0];

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            var root = new XElement("ExePackage");

            root.SetAttribute("Name", Name); //will respect null

            if (this.IsIdSet())
                root.SetAttribute("Id", Id);

            root.AddAttributes(this.Attributes)
                .Add(this.MapToXmlAttributes());

            root.AddPayloads(this.Payloads);
#pragma warning disable CS0618 // Type or member is obsolete
            root.AddPayloads(this.RemotePayloads);
#pragma warning restore CS0618 // Type or member is obsolete

            foreach (var exitCode in ExitCodes)
            {
                root.Add(exitCode.ToXElement());
            }
            return new[] { root };
        }
    }

    /// <summary>
    /// Standard WiX MsiPackage.
    /// </summary>
    public class MsiPackage : Package
    {
        /// <summary>
        /// Specifies whether the bundle will allow individual control over the installation state of Features inside the msi package. Managing
        /// feature selection requires special care to ensure the install, modify, update and uninstall behavior of the package is always correct.
        /// The default is "no".
        /// </summary>
        [Xml]
        public bool? EnableFeatureSelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsiPackage"/> class.
        /// </summary>
        public MsiPackage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsiPackage"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public MsiPackage(string path)
        {
            SourceFile = path;
        }

        /// <summary>
        /// Specifies whether the bundle will show the UI authored into the msi package. The default is "no" which means
        /// all information is routed to the bootstrapper application to provide a unified installation experience.
        /// If "yes" is specified the UI authored into the msi package will be displayed on top of any bootstrapper
        /// application UI.
        /// <para>
        /// Note, this field is only applicable for the stock bootstrapper applications.
        /// This is because when standard BA is used, WixSharp compiler can detect DisplayInternalUI and inject a "magic"
        /// attribute (bal:DisplayInternalUICondition="WixBundleAction = 6") into the bundle WXS definition. This condition
        /// is evaluated at runtime by the standard BA and the msi UI is displayed accordingly.</para>
        /// <para>
        /// However when custom BA UI is used, it is a responsibility of custom BA to manage msi UI visibility.
        /// It is normally done by adjusting `PlanMsiPackageEventArgs.UiLevel` value of the specific MsiPackages from the
        /// `BootstrapperApplication.PlanMsiPackage` event of the BA.</para>
        /// <para>
        /// Note, if you want to display the MSI UI that is not a stock but a custom Managed UI (inMSI terminology EmbeddedUI)
        /// then it is problematic. WiX team has not solved this problem since WiX3 despite the intent (https://github.com/wixtoolset/issues/issues/4921)
        /// Thus the only working options for displaying custom Managed UI are:
        /// </para>
        /// <para> - Follow 'WixBootstrapper_MsiEmbeddedUI' sample, which shows a very simple technique of wrapping msi into a self-hosted executable included in the bundle that happily shows the MSI UI regardless of whether it is a native or a managed one.
        /// </para>
        /// <para>- Use NSIS bootstrapper instead. Sadly. WiX Bundle seems to be too fragile and inflexible.</para>
        /// <para>- If the only reason for using a bootstrapper is to do a few simple actions (e.g. assess the environment) before product installation, then you can use an incredibly simple bootstrapper/launcher that is a pure custom CLI application.
        /// See 'Self-executable_Msi' sample.</para>
        /// </summary>
        public bool? DisplayInternalUI;

        /// <summary>
        /// Specifies whether the MSI will be displayed in Programs and Features (also known as Add/Remove Programs). If "yes" is specified the MSI package
        /// information will be displayed in Programs and Features. The default "no" indicates the MSI will not be displayed.
        /// </summary>
        [Xml]
        public bool? Visible;

        /// <summary>
        /// Override the automatic per-machine detection of MSI packages and force the package to be per-machine. The default is "no", which allows
        /// the tools to detect the expected value.
        /// </summary>
        [Xml]
        public bool? ForcePerMachine;

        /// <summary>
        /// MSI properties to be set based on the value of a burn engine expression. This is a KeyValue mapping expression of the following format:
        /// <para>&lt;key&gt;=&lt;value&gt;[;&lt;key&gt;=&lt;value&gt;]</para>
        /// <para><c>Example:</c> "COMMANDARGS=[CommandArgs];GLOBAL=yes""</para>
        /// </summary>
        public string MsiProperties;

        /// <summary>
        /// The default MsiProperties of a package .
        /// <para>This value is merged with user defined <see cref="WixSharp.Bootstrapper.MsiPackage.MsiProperties"/>.</para>
        /// <para>The default value of this property is "WIXBUNDLEORIGINALSOURCE=[WixBundleOriginalSource]"</para>
        /// </summary>
        public string DefaultMsiProperties = "WIXBUNDLEORIGINALSOURCE=[WixBundleOriginalSource]";

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            var root = new XElement("MsiPackage");

            root.SetAttribute("Name", Name); //will respect null

            if (this.IsIdSet())
                root.SetAttribute("Id", Id);

            root.AddAttributes(this.Attributes)
                .Add(this.MapToXmlAttributes());

            if (DisplayInternalUI == true)
            {
                // bal:DisplayInternalUICondition="WixBundleAction = 6"
                XNamespace bal = "http://wixtoolset.org/schemas/v4/wxs/bal";
                root.SetAttribute(bal + "DisplayInternalUICondition", "WixBundleAction = 6");
            }

            root.AddPayloads(this.Payloads);

            string props = MsiProperties + ";" + DefaultMsiProperties;

            props.ToDictionary().ForEach(p =>
                {
                    root.Add(new XElement("MsiProperty").AddAttributes("Name={0};Value={1}".FormatWith(p.Key, p.Value)));
                });

            return new[] { root };
        }
    }

    /// <summary>
    /// Standard WiX MspPackage.
    /// </summary>
    public class MspPackage : Package
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MspPackage"/> class.
        /// </summary>
        public MspPackage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MspPackage"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public MspPackage(string path)
        {
            SourceFile = path;
        }

        /// <summary>
        /// The identifier to use when caching the package.
        /// </summary>
        [Xml]
        public string CacheId;

        /// <summary>
        /// Specifies whether the bundle will show the UI authored into the msp package. The default is "no" which means all information is routed to the
        /// bootstrapper application to provide a unified installation experience. If "yes" is specified the UI authored into the msp package will be
        /// displayed on top of any bootstrapper application UI.
        /// </summary>
        [Xml]
        public bool? DisplayInternalUI;

        /// <summary>
        /// The size this package will take on disk in bytes after it is installed. By default, the binder will calculate the install size by scanning
        /// the package (File table for MSIs, Payloads for EXEs) and use the total for the install size of the package.
        /// </summary>
        [Xml]
        public string InstallSize;

        /// <summary>
        /// Indicates the package must be executed elevated. The default is "no".
        /// </summary>
        [Xml]
        public bool? PerMachine;

        /// <summary>
        /// Specifies whether to automatically slipstream the patch for any target msi packages in the chain. The default is "no". Even when the value is
        /// "no", you can still author the SlipstreamMsp element under MsiPackage elements as desired.
        /// </summary>
        [Xml]
        public bool? Slipstream;

        /// <summary>
        /// When set to "yes", the Prereq BA will plan the package to be installed if its InstallCondition is "true" or empty.
        /// (http://schemas.microsoft.com/wix/BalExtension)
        /// </summary>
        [Xml]
        public bool? PrereqSupportPackage;

        /// <summary>
        /// The remote payloads
        /// </summary>
        [Obsolete("Use `Payloads` instead.")]
        public RemotePayload[] RemotePayloads = new RemotePayload[0];

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            var root = new XElement("MspPackage");

            root.SetAttribute("Name", Name); //will respect null

            if (this.IsIdSet())
                root.SetAttribute("Id", Id);

            root.AddAttributes(this.Attributes)
                .Add(this.MapToXmlAttributes());

            root.AddPayloads(this.Payloads);
#pragma warning disable CS0618 // Type or member is obsolete
            root.AddPayloads(this.RemotePayloads);
#pragma warning restore CS0618 // Type or member is obsolete

            return new[] { root };
        }
    }

    /// <summary>
    /// Standard WiX MsuPackage.
    /// </summary>
    public class MsuPackage : Package
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsuPackage"/> class.
        /// </summary>
        public MsuPackage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsuPackage"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public MsuPackage(string path)
        {
            SourceFile = path;
        }

        /// <summary>
        /// A condition that determines if the package is present on the target system.
        /// This condition can use built-in variables and variables returned by searches.
        /// This condition is necessary because Windows doesn't provide a method to detect the presence of an ExePackage.
        /// Burn uses this condition to determine how to treat this package during a bundle action;
        /// for example, if this condition is false or omitted and the bundle is being installed, Burn will install this package.
        /// </summary>
        [Xml]
        public string DetectCondition;

        /// <summary>
        /// The knowledge base identifier for the MSU.
        /// The KB attribute must be specified to enable the MSU package to be uninstalled.
        /// Even then MSU uninstallation is only supported on Windows 7 and later.
        /// When the KB attribute is specified, the Permanent attribute will the control whether the package is uninstalled.
        /// </summary>
        [Xml]
        public string KB;

        /// <summary>
        /// The remote payloads
        /// </summary>
        [Obsolete("Use `Payloads` instead.")]
        public RemotePayload[] RemotePayloads = new RemotePayload[0];

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            var root = new XElement("MsuPackage");

            root.SetAttribute("Name", Name); //will respect null

            if (this.IsIdSet())
                root.SetAttribute("Id", Id);

            root.AddAttributes(this.Attributes)
                .Add(this.MapToXmlAttributes());

            root.AddPayloads(this.Payloads);
#pragma warning disable CS0618 // Type or member is obsolete
            root.AddPayloads(this.RemotePayloads);
#pragma warning restore CS0618 // Type or member is obsolete

            return new[] { root };
        }
    }

    internal static class PackagesExtensions
    {
        public static void AddPayloads(this XElement parent, Payload[] payloads)
        {
            if (payloads.Any())
                payloads.ForEach(p => parent.Add(p.ToXElement(p.GetType().Name)));

            if (payloads.Any(x => x.GetType().Name == "RemotePayload"))
                throw new Exception(
                    "`RemotePayload` entity is obsolete. Use concrete (RemotePayload derived) entities instead. " +
                    "IE `ExePackagePayload` instead of `RemotePayload`");
        }
    }

    /// <summary>
    /// Standard WiX PackageGroupRef.
    /// </summary>
    public class PackageGroupRef : ChainItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageGroupRef"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public PackageGroupRef(string name)
        {
            this.Id = new Id(name);
        }

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            var root = new XElement("PackageGroupRef");

            if (this.IsIdSet())
                root.SetAttribute("Id", Id);

            root.AddAttributes(this.Attributes)
                .Add(this.MapToXmlAttributes());

            return new[] { root };
        }
    }

    /// <summary>
    /// Standard WiX RollbackBoundary.
    /// </summary>
    public class RollbackBoundary : ChainItem
    {
        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            return new[] { new XElement("RollbackBoundary") };
        }
    }

    /// <summary>
    /// Exit code returned from executable package.
    /// </summary>
    public class ExitCode
    {
        /// <summary>
        /// Exit code returned from executable package.
        /// If no value is provided it means all values not explicitly set default to this behavior.
        /// </summary>
        [Xml]
        public string Value;

        /// <summary>
        /// Choose one of the supported behaviors error codes: success, error, scheduleReboot, forceReboot.
        /// This attribute's value must be one of the following:
        /// success
        /// error
        /// scheduleReboot
        /// forceReboot
        /// </summary>
        [Xml]
        public BehaviorValues Behavior;

        /// <summary>
        /// Serializes the <see cref="WixSharp.Bootstrapper.ExitCode"/> into XML based on the members marked with
        /// <see cref="WixSharp.XmlAttribute"/> and <see cref="WixSharp.WixObject.Attributes"/>.
        /// </summary>
        /// <returns></returns>
        public XElement ToXElement()
        {
            var element = new XElement("ExitCode", new XAttribute("Behavior", Behavior));
            if (Value != null)
            {
                element.Add(new XAttribute("Value", Value));
            }
            return element;
        }
    }

#pragma warning disable 1591

    public enum YesNoAlways
    {
        yes,
        no,
        always
    }

    public enum BehaviorValues
    {
        /// <summary>
        /// return success on specified error code
        /// </summary>
        success,

        /// <summary>
        /// return error on specified error code
        /// </summary>
        error,

        /// <summary>
        /// schedule reboot on specified error code
        /// </summary>
        scheduleReboot,

        /// <summary>
        /// force reboot on specified error code
        /// </summary>
        forceReboot,
    }

    public class Theme : StringEnum<Theme>
    {
        public Theme(string value) : base(value)
        {
        }

        public static Theme hyperlinkLargeLicense = new Theme(nameof(hyperlinkLargeLicense));
        public static Theme hyperlinkLicense = new Theme(nameof(hyperlinkLicense));
        public static Theme hyperlinkSidebarLicense = new Theme(nameof(hyperlinkSidebarLicense));
        public static Theme none = new Theme(nameof(none));
        public static Theme standard = new Theme(nameof(standard));
        public static Theme rtfLargeLicense = new Theme(nameof(rtfLargeLicense));
        public static Theme rtfLicense = new Theme(nameof(rtfLicense));
    }

    public enum SearchResult
    {
        /// <summary>
        /// Saves true if a matching registry entry or file is found; false otherwise.
        /// </summary>
        exists,

        /// <summary>
        /// Saves the value of the registry key in the variable. Can only be used with RegistrySearch. This is the default.
        /// </summary>
        value,

        /// <summary>
        /// Saves the version information for files that have it (.exe, .dll); zero-version (0.0.0.0) otherwise. Can only be used with FileSearch.
        /// </summary>
        version
    }

    public enum SearchFormat
    {
        /// <summary>
        /// Returns the unformatted value directly from the registry. For example, a REG_DWORD value of '1' is returned as '1', not '#1'.
        /// </summary>
        raw,

        /// <summary>
        /// Returns the value formatted as Windows Installer would. For example, a REG_DWORD value of '1' is returned as '#1', not '1'.
        /// </summary>
        compatible
    }

    /// <summary>
    /// The search result type to use for a <see cref="UtilProductSearch"/>
    /// </summary>
    public enum ProductSearchResultType
    {
        /// <summary>
        /// Saves the version of a matching product if found; 0.0.0.0 otherwise. This is the default.
        /// </summary>
        version,

        /// <summary>
        /// Saves the language of a matching product if found; empty otherwise.
        /// </summary>
        language,

        /// <summary>
        /// Saves the state of the product: advertised (1), absent (2), or locally installed (5).
        /// </summary>
        state,

        /// <summary>
        /// Saves the assignment type of the product: per-user (0), or per-machine (1).
        /// </summary>
        assignment
    }

#pragma warning restore 1591
}