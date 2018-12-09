using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Defines a registry search based on WiX RegistrySearch element (Util Extension).
    /// </summary>
    /// <example>The following is an example of adding a UtilRegistrySearch fragment into a Bundle definition.
    /// <code>
    /// bootstrapper.AddWixFragment("Wix/Bundle",
    ///                             new UtilRegistrySearch
    ///                             {
    ///                                 Root = RegistryHive.LocalMachine,
    ///                                 Key = @"Key=SOFTWARE\Microsoft\Net Framework Setup\NDP\v4\Full",
    ///                                 Value = "Version",
    ///                                 Result = SearchResult.exists,
    ///                                 Variable = "Netfx4FullVersion"
    ///                             });
    /// </code>
    /// </example>
    public class UtilRegistrySearch : WixObject, IXmlAware
    {
        /// <summary>
        /// Id of the search that this one should come after.
        /// </summary>
        [Xml]
        public string After;

        /// <summary>
        /// Condition for evaluating the search. If this evaluates to false, the search is not executed at all.
        /// </summary>
        [Xml]
        public string Condition;

        /// <summary>
        /// Key to search for.
        /// </summary>
        [Xml]
        public string Key;

        /// <summary>
        /// Optional value to search for under the given Key.
        /// </summary>
        [Xml]
        public string Value;

        /// <summary>
        /// Name of the variable in which to place the result of the search.
        /// </summary>
        [Xml]
        public string Variable;

        /// <summary>
        /// Registry root hive to search under.
        /// </summary>
        public RegistryHive Root;

        /// <summary>
        /// Instructs the search to look in the 64-bit registry when the value is 'yes'. When the value is 'no', the search looks in the 32-bit registry. The default value is 'no'.
        /// </summary>
        [Xml]
        public bool? Win64;

        /// <summary>
        /// Rather than saving the matching registry value into the variable, a RegistrySearch can save an attribute of the matching entry instead. This attribute's value must be one of the following:
        /// <para>
        /// <c>exists</c> - Saves true if a matching registry entry is found; false otherwise.
        /// </para>
        /// <c>value</c> - Saves the value of the registry key in the variable. This is the default.
        /// </summary>
        [Xml]
        public SearchResult? Result;

        /// <summary>
        /// Whether to expand any environment variables in REG_SZ, REG_EXPAND_SZ, or REG_MULTI_SZ values.
        /// </summary>
        [Xml]
        public bool? ExpandEnvironmentVariables;

        /// <summary>
        /// What format to return the value in. This attribute's value must be one of the following:
        /// <para>
        /// <c>raw</c> - Returns the unformatted value directly from the registry.For example, a REG_DWORD value of '1' is returned as '1', not '#1'.</para>
        /// <c>compatible</c> - Returns the value formatted as Windows Installer would.For example, a REG_DWORD value of '1' is returned as '#1', not '1'.
        /// </summary>
        [Xml]
        public SearchFormat? Format;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
        {
            return this.ToXElement(WixExtension.Util.ToXName("RegistrySearch"))
                       .SetAttribute("Root", Root);
        }
    }
}