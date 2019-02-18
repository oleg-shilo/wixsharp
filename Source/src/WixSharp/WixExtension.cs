using System;
using System.Xml.Linq;

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
            if (assembly.IsNullOrEmpty()) throw new ArgumentNullException("assembly", "assembly is a null reference or empty");

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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return XmlNamespace;
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
        /// Well-known Wix Extension: difx
        /// </summary>
        public static WixExtension Difx = new WixExtension("%WixLocation%\\WixDifxAppExtension.dll", "difx", DifxNamespace);

        /// <summary>
        /// The `Difx` extension namespace
        /// </summary>
        public const string DifxNamespace = "http://schemas.microsoft.com/wix/DifxAppExtension";

        /// <summary>
        /// Well-known Wix Extension: Fire (Firewall)
        /// </summary>
        public static WixExtension Fire = new WixExtension("%WixLocation%\\WixFirewallExtension.dll", "fire", FireNamespace);

        /// <summary>
        /// The `Firewall` extension namespace
        /// </summary>
        public const string FireNamespace = "http://schemas.microsoft.com/wix/FirewallExtension";

        /// <summary>
        /// Well-known Wix Extension: Util
        /// </summary>
        public static WixExtension Util = new WixExtension("%WixLocation%\\WixUtilExtension.dll", "util", UtilNamespace);

        /// <summary>
        /// The `Util` extension namespace
        /// </summary>
        public const string UtilNamespace = "http://schemas.microsoft.com/wix/UtilExtension";

        /// <summary>
        /// Well-known Wix Extension: Bal
        /// </summary>
        public static WixExtension Bal = new WixExtension("%WixLocation%\\WixBalExtension.dll", "bal", BalNamespace);

        /// <summary>
        /// The `Bal` extension namespace
        /// </summary>
        public const string BalNamespace = "http://schemas.microsoft.com/wix/BalExtension";

        /// <summary>
        /// Well-known Wix Extension IIs
        /// </summary>
        public static WixExtension IIs = new WixExtension("%WixLocation%\\WixIIsExtension.dll", "iis", IisNamespace);

        /// <summary>
        /// The `Iis` extension namespace
        /// </summary>
        public const string IisNamespace = "http://schemas.microsoft.com/wix/IIsExtension";

        /// <summary>
        /// Well-known Wix Extension Sql
        /// </summary>
        public static WixExtension Sql = new WixExtension("%WixLocation%\\WixSqlExtension.dll", "sql", SqlNamespace);

        /// <summary>
        /// The `Sql` extension namespace
        /// </summary>
        public const string SqlNamespace = "http://schemas.microsoft.com/wix/SqlExtension";

        /// <summary>
        /// Well-known Wix Extension NetFx
        /// </summary>
        public static WixExtension NetFx = new WixExtension("%WixLocation%\\WiXNetFxExtension.dll", "netfx", NetFxNamespace);

        /// <summary>
        /// The `NetFx` extension namespace
        /// </summary>
        public const string NetFxNamespace = "http://schemas.microsoft.com/wix/NetFxExtension";

        /// <summary>
        /// Well-known Wix Extension Http
        /// </summary>
        public static WixExtension Http = new WixExtension("%WixLocation%\\WiXHttpExtension.dll", "http", HttpNamespace);

        /// <summary>
        /// The `Http` extension namespace
        /// </summary>
        public const string HttpNamespace = "http://schemas.microsoft.com/wix/HttpExtension";

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