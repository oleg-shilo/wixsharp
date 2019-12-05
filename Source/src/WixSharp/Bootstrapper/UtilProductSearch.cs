using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Defines a product search based on the Wix ProductSearch element (Util extension)
    /// </summary>
    /// <example>
    /// <code>
    /// new ExePackage()
    /// {
    ///     DisplayName = "Microsoft Visual C++ 2015 Redistributable (x64) - 14.0.24215",
    ///     DownloadUrl = "https://download.microsoft.com/download/6/A/A/6AA4EDFF-645B-48C5-81CC-ED5963AEAD48/vc_redist.x64.exe",
    ///     DetectCondition = "VCPlusPlus2015_64 >= VCPlusPlus2015_64_RequiredVersion",
    ///     InstallCommand = "/install /quiet /norestart /ChainingPackage \"[WixBundleName]\""
    /// };
    /// 
    /// new Variable()
    /// {
    ///     Name: "VCPlusPlus2015_64_RequiredVersion",
    ///     Type: WixSharp.VariableType.version,
    ///     Value: "14.0.24215")
    /// };
    /// 
    /// bootstrapper.AddWixFragment("Wix/Bundle",
    ///                             new UtilProductSearch
    ///                             {
    ///                                 UpgradeCode = "{36F68A90-239C-34DF-B58C-64B30153CE35}",
    ///                                 Variable = "VCPlusPlus2015_64"
    ///                             });
    /// </code>
    /// Produces a util:ProductSearch and corresponding ExePackage and Variable to bypass the install of MSVC++ 2015 x64
    /// when it is already installed on the system.
    /// </example>
    public class UtilProductSearch : WixObject, IXmlAware
    {
        /// <summary>
        /// The search result type
        /// </summary>
        public ProductSearchResultType Result;

        /// <summary>
        /// The ProductCode to use for the search. This attribute must be omitted if UpgradeCode is specified.
        /// </summary>
        [Xml]
        public string ProductCode;

        /// <summary>
        /// The UpgradeCode to use for the search.
        /// This attribute must be omitted if ProductCode is specified.
        /// Note that if multiple products are found, the highest versioned product will be used for the result.
        /// </summary>
        [Xml]
        public string UpgradeCode;

        /// <summary>
        /// Name of the variable in which to place the result of the search.
        /// </summary>
        [Xml]
        public string Variable;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
            => this.ToXElement(WixExtension.Util.ToXName("ProductSearch"))
                    .SetAttribute("Result", this.Result);
    }
}
