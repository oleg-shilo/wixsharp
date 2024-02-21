using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Adds or removes .xml file entries.
    /// </summary>
    public class XmlFile : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFile" /> class.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="action">The Action.</param>
        /// <param name="elementPath">The ElementPath.</param>
        /// <param name="value">The Value.</param>
        public XmlFile(Id id, XmlFileAction action, string elementPath, string value)
        {
            Id = id;
            Action = action;
            ElementPath = elementPath;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFile" /> class.
        /// </summary>
        /// <param name="action">The Action.</param>
        /// <param name="elementPath">The ElementPath.</param>
        /// <param name="value">The Value.</param>
        public XmlFile(XmlFileAction action, string elementPath, string value)
        {
            Action = action;
            ElementPath = elementPath;
            Value = value;
        }

        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Name of XML node to set/add to the specified element.
        /// Not setting this attribute causes the element's text value to be set.
        /// Otherwise this specified the attribute name that is set.
        /// </summary>
        [Xml]
        public new string Name;

        /// <summary>
        /// The type of modification to be made to the XML file when the component is installed.
        /// </summary>
        [Xml]
        public XmlFileAction Action;

        /// <summary>
        /// The XPath of the element to be modified.
        /// Note that this is a formatted field and therefore, square brackets in the XPath must be escaped.
        /// In addition, XPaths allow backslashes to be used to escape characters, so if you intend to include
        /// literal backslashes, you must escape them as well by doubling them in this attribute.
        /// The string is formatted by MSI first, and the result is consumed as the XPath.
        /// </summary>
        [Xml]
        public string ElementPath;

        /// <summary>
        /// The value to be written. See the Formatted topic for information how to escape square brackets in the value.
        /// </summary>
        [Xml]
        public string Value;

        /// <summary>
        /// Specifies whether or not the modification should be removed on uninstall.
        /// This has no effect on uninstall if the action was deleteValue.
        /// </summary>
        [Xml]
        public bool? Permanent;

        /// <summary>
        /// Specifies whether or not the modification should preserve the modified date.
        /// Preserving the modified date will allow the file to be patched if no other modifications have been made.
        /// </summary>
        [Xml]
        public bool? PreserveModifiedDate;

        /// <summary>
        /// Specifies the order in which the modification is to be attempted on the XML file.
        /// It is important to ensure that new elements are created before you attempt to add an attribute to them.
        /// </summary>
        [Xml]
        public int? Sequence;

        /// <summary>
        /// Specify whether the DOM object should use XPath language or the old XSLPattern language (default) as the query language.
        /// </summary>
        [Xml]
        public XmlFileSelectionLanguage? SelectionLanguage;

        /// <summary>
        /// Path of the .xml file to configure.
        /// </summary>
        public string File;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Util);

            if (File.IsEmpty())
            {
                File = $"[#{context.Parent.Id}]";
            }

            context.XParentComponent.Add(this.ToXElement(WixExtension.Util, "XmlFile").AddAttributes("File=" + File));
            // context.XParent.ParentComponent().Add(this.ToXElement(WixExtension.Util, "XmlFile").AddAttributes("File=" + File));
        }
    }
}