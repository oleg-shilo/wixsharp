using System;
using System.Xml.Linq;
using WixToolset.Dtf.WindowsInstaller;

namespace WixSharp
{
    /// <summary>
    /// Represents a Wix Extension
    /// </summary>
    public class WixExtension
    {
        /// <summary>
        /// File name of the represented Wix Extension assembly
        /// </summary>
        /// <remarks>The represented value must include the file name and extension. See example</remarks>
        /// <example>WixIIsExtension.dll</example>
        public string Assembly
        {
            get { return Environment.ExpandEnvironmentVariables(assembly); }
        }

        string assembly;

        /// <summary>
        /// Xml namespace declaration prefix for the represented Wix Extension
        /// </summary>
        public readonly string XmlNamespacePrefix;

        /// <summary>
        /// Xml namespace value for the represented Wix Extension
        /// </summary>
        public readonly string XmlNamespace;

        /// <summary>
        /// Creates a WixExtension instance representing the corresponding XML namespace declaration
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="prefix"></param>
        /// <param name="namespace"></param>
        public WixExtension(string assembly, string prefix, string @namespace)
        {
            if (assembly.IsNullOrEmpty()) throw new ArgumentNullException(nameof(assembly), "assembly is a null reference or empty");

            //note some extensions do not have associated XML namespace (e.g. WixUIExtension.dll).

            this.assembly = assembly;
            XmlNamespacePrefix = prefix;
            XmlNamespace = @namespace;
        }

        /// <summary>
        /// Returns XmlNamespacePrefix as an instance of XNamespace
        /// </summary>
        /// <returns></returns>
        public XNamespace ToXNamespace()
        {
            return XmlNamespace;
        }

        /// <summary>
        /// Creates XName based on the XNamespace and specified name.
        /// </summary>
        /// <returns></returns>
        public XName ToXName(string name)
        {
            return ToXNamespace() + name;
        }

        /// <summary>
        /// Creates XElement based on XName (XNamespace and specified name) and specified content.
        /// </summary>
        /// <returns></returns>
        public XElement XElement(string name, object content)
        {
            return new XElement(ToXNamespace() + name, content);
        }

        /// <summary>
        /// Creates XElement based on XName (XNamespace and specified name) and specified attributes.
        /// <param name="name">Name of the element.</param>
        /// <param name="attributesDefinition">The attributes definition. Rules of the composing the
        /// definition are the same as for <see cref="WixObject.AttributesDefinition"/>.</param>
        /// </summary>
        public XElement XElement(string name, string attributesDefinition)
        {
            return new XElement(ToXNamespace() + name).SetAttributes(attributesDefinition);
        }

        /// <summary>
        /// Gets the xml namespace attribute for this WixExtension
        /// </summary>
        /// <returns></returns>
        public string ToNamespaceDeclaration()
        {
            return GetNamespaceDeclaration(XmlNamespacePrefix, XmlNamespace);
        }

        /// <summary>
        /// Gets the xml namespace attribute for the provided <paramref name="prefix"/> and <paramref name="namespace"/>
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static string GetNamespaceDeclaration(string prefix, string @namespace)
        {
            return string.Format("xmlns:{0}=\"{1}\"", prefix, @namespace);
        }

        /// <summary>
        /// Gets a value indicating whether this WixSharp is building for WiX4.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is wix4; otherwise, <c>false</c>.
        /// </value>
        public static bool IsWix4 => typeof(Session).Assembly.Location.EndsWith("WixToolset.Dtf.WindowsInstaller.dll", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Well-known Wix Extension: difx
        /// </summary>
        public static WixExtension Difx = new WixExtension("%WixLocation%\\WixDifxAppExtension.dll", "difx", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/difxapp" : "http://schemas.microsoft.com/wix/DifxAppExtension");

        /// <summary>
        /// Well-known Wix Extension: Fire (Firewall)
        /// </summary>
        public static WixExtension Fire = new WixExtension("%WixLocation%\\WixFirewallExtension.dll", "fire", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/firewall" : "http://schemas.microsoft.com/wix/FirewallExtension");

        /// <summary>
        /// Well-known Wix Extension: Util
        /// </summary>
        public static WixExtension Util = new WixExtension("%WixLocation%\\WixUtilExtension.dll", "util", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/util" : "http://schemas.microsoft.com/wix/UtilExtension");

        /// <summary>
        /// Well-known Wix Extension: Bal
        /// </summary>
        public static WixExtension Bal = new WixExtension("%WixLocation%\\WixBalExtension.dll", "bal", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/bal" : "http://schemas.microsoft.com/wix/BalExtension");

        /// <summary>
        /// Well-known Wix Extension IIs
        /// </summary>
        public static WixExtension IIs = new WixExtension("%WixLocation%\\WixIIsExtension.dll", "iis", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/iis" : "http://schemas.microsoft.com/wix/IIsExtension");

        /// <summary>
        /// Well-known Wix Extension Sql
        /// </summary>
        public static WixExtension Sql = new WixExtension("%WixLocation%\\WixSqlExtension.dll", "sql", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/sql" : "http://schemas.microsoft.com/wix/SqlExtension");

        /// <summary>
        /// Well-known Wix Extension NetFx
        /// </summary>
        public static WixExtension NetFx = new WixExtension("%WixLocation%\\WiXNetFxExtension.dll", "netfx", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/netfx" : "http://schemas.microsoft.com/wix/NetFxExtension");

        /// <summary>
        /// Well-known Wix Extension Http
        /// </summary>
        public static WixExtension Http = new WixExtension("%WixLocation%\\WiXHttpExtension.dll", "http", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/http" : "http://schemas.microsoft.com/wix/HttpExtension");

        /// <summary>
        /// Well-known Wix Extension COM+
        /// </summary>
        public static WixExtension ComPlus = new WixExtension("%WixLocation%\\WixComPlusExtension.dll", "complus", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/complus" : "http://schemas.microsoft.com/wix/ComPlusExtension");

        /// <summary>
        /// Well-known Wix Extension Dependency
        /// </summary>
        public static WixExtension Dependency = new WixExtension("%WixLocation%\\WixDependencyExtension.dll", "dependency", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/dependency" : "http://schemas.microsoft.com/wix/DependencyExtension");

        /// <summary>
        /// Well-known Wix Extension Gaming
        /// </summary>
        public static WixExtension Gaming = new WixExtension("%WixLocation%\\WixGamingExtension.dll", "gaming", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/gaming" : "http://schemas.microsoft.com/wix/GamingExtension");

        /// <summary>
        /// The Well-known Wix Extension MSMQ
        /// </summary>
        public static WixExtension Msmq = new WixExtension("%WixLocation%\\WixMsmqExtension.dll", "msmq", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/msmq" : "http://schemas.microsoft.com/wix/MsmqExtension");

        /// <summary>
        /// The Well-known Wix Extension PowerShell
        /// </summary>
        public static WixExtension Ps = new WixExtension("%WixLocation%\\WixPSExtension.dll", "ps", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/powershell" : "http://schemas.microsoft.com/wix/PSExtension");

        /// <summary>
        /// Well-known Wix Extension Tag
        /// </summary>
        public static WixExtension Tag = new WixExtension("%WixLocation%\\WixTagExtension.dll", "tag", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/tag" : "http://schemas.microsoft.com/wix/TagExtension");

        /// <summary>
        /// Well-known Wix Extension VS
        /// </summary>
        public static WixExtension Vs = new WixExtension("%WixLocation%\\WixVsExtension.dll", "vs", IsWix4 ? "http://wixtoolset.org/schemas/v4/wxs/vs" : "http://schemas.microsoft.com/wix/VSExtension");

        /// <summary>
        /// Well-known Wix Extension LUX
        /// </summary>
        public static WixExtension Lux = new WixExtension("%WixLocation%\\WixLuxExtension.dll", "lux", IsWix4 ? "http://wixtoolset.org/schemas/v4/lux" : "http://schemas.microsoft.com/wix/2009/Lux");

        // public static WixExtension ??? = new WixExtension("%WixLocation%\\Wix.dll", "thmutil", IsWix4? "http://wixtoolset.org/schemas/v4/thmutil" : "http://wixtoolset.org/schemas/thmutil/2010");

        /// <summary>
        /// Well-known Wix Extension UI
        /// </summary>
        public static WixExtension UI = new WixExtension("%WixLocation%\\WixUIExtension.dll", null, null);
    }

    /// <summary>
    /// The interface for the Wix# types that can generate WiX XML.
    /// </summary>
    public interface IXmlAware
    {
        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        XElement ToXml();
    }
}