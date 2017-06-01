using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Container class for common members of the Bootstrapper packages
    /// </summary>
    public abstract class Package : ChainItem
    {
        /// <summary>
        /// Specifies whether the package can be uninstalled.
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
        /// Indicates the package must be executed elevated.
        /// </summary>
        [Xml]
        public bool? PerMachine;

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
        /// Collection of paths to the package dependencies.
        /// </summary>
        public string[] Payloads = new string[0];

        internal void EnsureId()
        {
            if (!base.IsIdSet())
            {
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
        public string InstallCommand;

        /// <summary>
        /// The command-line arguments to specify to indicate a repair. If the executable package can be repaired but does not require any
        /// special command-line arguments to do so then set the attribute's value to blank. To indicate that the package does not support repair,
        /// omit this attribute.
        /// </summary>
        [Xml]
        public string RepairCommand;

        /// <summary>
        /// The command-line arguments provided to the ExePackage during uninstall. If this attribute is absent the executable will be launched
        /// with no command-line arguments. To prevent an ExePackage from being uninstalled set the Permanent attribute to "yes".
        /// </summary>
        [Xml]
        public string UninstallCommand;

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

            if (Payloads.Any())
                Payloads.ForEach(p => root.Add(new XElement("Payload", new XAttribute("SourceFile", p))));

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
        /// Specifies whether the bundle will show the UI authored into the msi package. The default is "no" which means all information is routed to
        /// the bootstrapper application to provide a unified installation experience. If "yes" is specified the UI authored into the msi package will be
        /// displayed on top of any bootstrapper application UI.
        /// </summary>
        [Xml]
        public bool? DisplayInternalUI;

        /// <summary>
        /// Specifies whether the MSI will be displayed in Programs and Features (also known as Add/Remove Programs). If "yes" is specified the MSI package 
        /// information will be displayed in Programs and Features. The default "no" indicates the MSI will not be displayed.
        /// </summary>
        [Xml]
        public bool? Visible;

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

            if (Payloads.Any())
                Payloads.ForEach(p => root.Add(new XElement("Payload", new XAttribute("SourceFile", p))));

            string props = MsiProperties + ";" + DefaultMsiProperties;

            props.ToDictionary().ForEach(p =>
                {
                    root.Add(new XElement("MsiProperty").AddAttributes("Name={0};Value={1}".FormatWith(p.Key, p.Value)));
                });

            return new[] { root };
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
    ///
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

#pragma warning restore 1591
}