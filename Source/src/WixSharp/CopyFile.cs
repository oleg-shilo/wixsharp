using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// This class implements 'CopyFile' WiX element.
    /// <para>This feature has been contributed as the result of "CopyFile element not implemented #801"</para>
    /// </summary>
    /// <seealso cref="WixSharp.WixEntity" />
    /// <seealso cref="WixSharp.IGenericEntity" />
    public class CopyFile : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyFile"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public CopyFile(Id id)
        {
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyFile"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="sourceName">Name of the source.</param>
        public CopyFile(Id id, Feature feature, string destinationDirectory, string sourceDirectory, string sourceName)
        {
            Id = id;
            Feature = feature;
            DestinationDirectory = destinationDirectory;
            SourceDirectory = sourceDirectory;
            SourceName = sourceName;
        }

        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixEntity" />.
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para><para>If the <see cref="Id" />
        /// value is not specified explicitly by the user the Wix# compiler
        /// generates it automatically insuring its uniqueness.</para><remarks>
        /// Note: The ID auto-generation is triggered on the first access (evaluation) and in order to make the id
        /// allocation deterministic the compiler resets ID generator just before the build starts. However if you
        /// accessing any auto-id before the Build*() is called you can it interferes with the ID auto generation and eventually
        /// lead to the WiX ID duplications. To prevent this from happening either:
        /// <para> - Avoid evaluating the auto-generated IDs values before the call to Build*()</para><para> - Set the IDs
        /// (to be evaluated) explicitly</para>
        /// <para> - Prevent resetting auto-ID generator by setting WixEntity.DoNotResetIdGenerator to true";</para></remarks>
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// This attribute cannot be specified if the element is nested under a File element or the FileId attribute is specified.
        /// In other cases, if the attribute is not specified, the default value is "no" and the file is copied, not moved.
        /// Set the value to "yes" in order to move the file (thus deleting the source file) instead of copying it.
        /// </summary>
        [Xml]
        public bool? Delete;

        /// <summary>
        /// Set this value to the destination directory where an existing file on the target machine should be moved or copied to. This Directory must exist in the installer database at creation time. This attribute cannot be specified in conjunction /// with DestinationProperty.
        /// </summary>
        [Xml]
        public string DestinationDirectory;

        /// <summary>
        /// Set this value to a property that will have a value that resolves to the full path of the destination directory. The property does not have to exist in the installer database at creation time; it could be created at installation time by a custom action, on the command line, etc. This attribute cannot be specified in conjunction with DestinationDirectory.
        /// </summary>
        [Xml]
        public string DestinationProperty;

        /// <summary>
        /// This attribute cannot be specified if the element is nested under a File element. Set this attribute's value to the identifier of a file from a different component to copy it based on the install state of the parent component.
        /// </summary>
        [Xml]
        public string FileId;

        /// <summary>
        /// This attribute cannot be specified if the element is nested under a File element or the FileId attribute is
        /// specified. Set this value to the source directory from which to copy or move an existing file on the target machine.
        /// This Directory must exist in the installer database at creation time. This attribute cannot be specified in conjunction with SourceProperty.
        /// </summary>
        [Xml]
        public string SourceDirectory;

        /// <summary>
        /// This attribute cannot be specified if the element is nested under a File element or the FileId attribute is specified.
        /// Set this value to the localizable name of the file(s) to be copied or moved. All of the files that match the wild card
        /// will be removed from the specified directory. The value is a filename that may also contain the wild card characters "?"
        /// for any single character or "*" for zero or more occurrences of any character. If this attribute is not specified
        /// (and this element is not nested under a File element or specify a FileId attribute) then the SourceProperty attribute
        /// should be set to the name of a property that will resolve to the full path of the source filename.
        /// If the value of this attribute contains a "*" wildcard and the DestinationName attribute is specified, all moved or
        /// copied files retain the file names from their sources.
        /// </summary>
        [Xml]
        public string SourceName;

        /// <summary>
        /// The source propertyThis attribute cannot be specified if the element is nested under a File element or the
        /// FileId attribute is specified. Set this value to a property that will have a value that resolves to the full
        /// path of the source directory (or full path including file name if SourceName is not specified). The property
        /// does not have to exist in the installer database at creation time; it could be created at installation time by
        /// a custom action, on the command line, etc. This attribute cannot be specified in conjunction with SourceDirectory.
        /// </summary>
        [Xml]
        public string SourceProperty;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            XElement component = this.CreateAndInsertParentComponent(context);
            component.Add(this.ToXElement("CopyFile"));
        }
    }
}