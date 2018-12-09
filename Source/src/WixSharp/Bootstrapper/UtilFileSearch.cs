using System.Linq;
using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Defines a file search based on WiX FileSearch element (Util Extension).
    /// </summary>
    /// <example>The following is an example of adding a UtilFileSearch fragment into a Bundle definition.
    /// <code>
    /// bootstrapper.AddWixFragment("Wix/Bundle",
    ///                             new UtilFileSearch
    ///                             {
    ///                                 Path = @"[ProgramFilesFolder]Adobe\adobe.exe",
    ///                                 Result = SearchResult.exists,
    ///                                 Variable = "AdobeInstalled"
    ///                             });
    /// </code>
    /// </example>
    public class UtilFileSearch : WixObject, IXmlAware
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
        /// File path to search for.
        /// </summary>
        [Xml]
        public string Path;

        /// <summary>
        /// Name of the variable in which to place the result of the search.
        /// </summary>
        [Xml]
        public string Variable;

        /// <summary>
        /// Rather than saving the matching file path into the variable, a FileSearch can save an attribute of the matching file instead. This attribute's value must be one of the following:
        /// <para>
        /// <c>exists</c> - Saves true if a matching file is found; false otherwise.
        /// </para>
        /// <c>version</c> - Saves the version information for files that have it (.exe, .dll); zero-version (0.0.0.0) otherwise.
        /// </summary>
        [Xml]
        public SearchResult? Result;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
        {
            return this.ToXElement(WixExtension.Util.ToXName("FileSearch"));
        }
    }
}