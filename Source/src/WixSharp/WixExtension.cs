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
            get { return assembly.ExpandEnvVars(); }
        }

        readonly string assembly;

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
        public static WixExtension Difx = new WixExtension("WixToolset.DifxApp.wixext", "difx", DifxNamespace);

        /// <summary>
        /// The `Difx` extension namespace
        /// </summary>
        public const string DifxNamespace = "http://wixtoolset.org/schemas/v4/wxs/difxapp";

        /// <summary>
        /// Well-known Wix Extension: Fire (Firewall)
        /// </summary>
        public static WixExtension Fire = new WixExtension("WixToolset.Firewall.wixext", "fire", FireNamespace);

        /// <summary>
        /// The `Firewall` extension namespace
        /// </summary>
        public const string FireNamespace = "http://wixtoolset.org/schemas/v4/wxs/firewall";

        /// <summary>
        /// Well-known Wix Extension: Util
        /// </summary>
        public static WixExtension Util = new WixExtension("WixToolset.Util.wixext", "util", UtilNamespace);

        /// <summary>
        /// The `Util` extension namespace
        /// </summary>
        public const string UtilNamespace = "http://wixtoolset.org/schemas/v4/wxs/util";

        /// <summary>
        /// Well-known Wix Extension: Bal
        /// </summary>
        public static WixExtension Bal = new WixExtension("WixToolset.Bal.wixext", "bal", BalNamespace);

        /// <summary>
        /// The `Bal` extension namespace
        /// </summary>
        public const string BalNamespace = "http://wixtoolset.org/schemas/v4/wxs/bal";

        /// <summary>
        /// Well-known Wix Extension IIs
        /// </summary>
        public static WixExtension IIs = new WixExtension("WixToolset.Iis.wixext", "iis", IisNamespace);

        /// <summary>
        /// The `Iis` extension namespace
        /// </summary>
        public const string IisNamespace = "http://wixtoolset.org/schemas/v4/wxs/iis";

        /// <summary>
        /// Well-known Wix Extension Sql
        /// </summary>
        public static WixExtension Sql = new WixExtension("WixToolset.Sql.wixext", "sql", SqlNamespace);

        /// <summary>
        /// The `Sql` extension namespace
        /// </summary>
        public const string SqlNamespace = "http://wixtoolset.org/schemas/v4/wxs/sql";

        /// <summary>
        /// Well-known Wix Extension NetFx
        /// </summary>
        public static WixExtension NetFx = new WixExtension("WixToolset.Netfx.wixext", "netfx", NetFxNamespace);

        /// <summary>
        /// The `NetFx` extension namespace
        /// </summary>
        public const string NetFxNamespace = "http://wixtoolset.org/schemas/v4/wxs/netfx";

        /// <summary>
        /// Well-known Wix Extension Http
        /// </summary>
        public static WixExtension Http = new WixExtension("WixToolset.Http.wixext", "http", HttpNamespace);

        /// <summary>
        /// The `Http` extension namespace
        /// </summary>
        public const string HttpNamespace = "http://wixtoolset.org/schemas/v4/wxs/http";

        /// <summary>
        /// Well-known Wix Extension UI
        /// </summary>
        public static WixExtension UI = new WixExtension("WixToolset.UI.wixext", "ui", UiNamespace);

        /// <summary>
        /// The `UI` extension namespace
        /// </summary>
        public const string UiNamespace = "http://wixtoolset.org/schemas/v4/wxs/ui";

        /// <summary>
        /// Well-known Wix Extension PowerShell
        /// </summary>
        public static WixExtension PowerShell = new WixExtension("WixToolset.PowerShell.wixext", "ps", PowerShellNamespace);

        /// <summary>
        /// The `PowerShell` extension namespace
        /// </summary>
        public const string PowerShellNamespace = "http://wixtoolset.org/schemas/v4/wxs/powershell";

        /// <summary>
        /// Well-known Wix Extension VisualStudio
        /// </summary>
        public static WixExtension VisualStudio = new WixExtension("WixToolset.VisualStudio.wixext", "vs", VisualStudioNamespace);

        /// <summary>
        /// The `VisualStudio` extension namespace
        /// </summary>
        public const string VisualStudioNamespace = "http://wixtoolset.org/schemas/v4/wxs/vs";

        /// <summary>
        /// Well-known Wix Extension MSMQ
        /// </summary>
        public static WixExtension Msmq = new WixExtension("WixToolset.Msmq.wixext", "msmq", MsmqNamespace);

        /// <summary>
        /// The `Msmq` extension namespace
        /// </summary>
        public const string MsmqNamespace = "http://wixtoolset.org/schemas/v4/wxs/msmq";


        /// <summary>
        /// Well-known Wix Extension ComPLus
        /// </summary>
        public static WixExtension ComPlus = new WixExtension("WixToolset.ComPlus.wixext", "complus", MsmqNamespace);
        /// <summary>
        /// The `ComPLus` extension namespace
        /// </summary>
        public const string ComPlusNamespace = "http://wixtoolset.org/schemas/v4/wxs/complus";

        /// <summary>
        /// Well-known Wix Extension Dependency
        /// </summary>
        public static WixExtension Dependency = new WixExtension("WixToolset.Dependency.wixext", "dep", MsmqNamespace);

        /// <summary>
        /// The `Dependency` extension namespace
        /// </summary>
        public const string DependencyNamespace = "http://wixtoolset.org/schemas/v4/wxs/dependency";

        /// <summary>
        /// Well-known Wix Extension DirectX
        /// </summary>
        public static WixExtension DirectX = new WixExtension("WixToolset.DirectX.wixext", "?", MsmqNamespace);

        /// <summary>
        /// The `DirectX` extension namespace
        /// </summary>
        public const string DirectXNamespace = "http://wixtoolset.org/schemas/v4/wxs/directx";
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