using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Generic <see cref="T:WixSharp.WixEntity"/> container for defining WiX <c>Media</c> element attributes.
    /// <para>These attributes describe a disk that makes up the source media for the installation.</para>
    ///<example>The following is an example of defining the <c>Media</c> attributes.
    ///<code>
    /// var project =
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///     ...
    /// project.Media.First().Id=2;
    /// project.Media.First().CompressionLevel=CompressionLevel.high;
    /// //or
    /// project.Media.First().AttributesDefinition = @"CompressionLevel=high;
    ///                                                Id=2";
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    /// </summary>
    public class Media : WixObject
    {
        /// <summary>
        /// Disk identifier for Media table. This number must be equal to or greater than 1.
        /// </summary>
        [Xml]
        public int Id = 1;

        /// <summary>
        /// The name of the cabinet if some or all of the files stored on the media are in
        /// a cabinet file. If no cabinets are used, this attribute must not be set.
        /// <para>The default value is <c> "{projectId}.cab"</c>, which is expanded ar runtime</para>
        /// </summary>
        [Xml]
        public string Cabinet = "{projectId}.cab";

        /// <summary>
        /// Indicates the compression level for the Media's cabinet. This attribute can only
        /// be used in conjunction with the Cabinet attribute. The default is 'mszip'.
        /// </summary>
        [Xml]
        public CompressionLevel? CompressionLevel;

        /// <summary>
        /// The disk name, which is usually the visible text printed on the disk. This localizable
        /// text is used to prompt the user when this disk needs to be inserted. This value will
        /// be used in the "[1]" of the DiskPrompt Property. Using this attribute will require you
        /// to define a DiskPrompt Property.
        /// </summary>
        [Xml]
        public string DiskPrompt;

        /// <summary>
        /// Instructs the binder to embed the cabinet in the product if 'yes'. This attribute can only
        /// be specified in conjunction with the Cabinet attribute.
        /// </summary>
        [Xml]
        public bool? EmbedCab = true;

        /// <summary>
        /// This attribute specifies the root directory for the uncompressed files that are a part of
        /// this Media element. By default, the src will be the output directory for the final image.
        /// The default value ensures the binder generates an installable image. If a relative path is
        /// specified in the src attribute, the value will be appended to the image's output directory.
        /// If an absolute path is provided, that path will be used without modification. The latter two
        /// options are provided to ease the layout of an image onto multiple medias (CDs/DVDs).
        /// </summary>
        [Xml]
        public string Layout;

        /// <summary>
        /// Optional property that identifies the source of the embedded cabinet. If a cabinet is specified
        /// for a patch, this property should be defined and unique to each patch so that the embedded cabinet
        /// containing patched and new files can be located in the patch package. If the cabinet is not embedded
        /// - this is not typical - the cabinet can be found in the directory referenced in this column. If empty,
        /// the external cabinet must be located in the SourceDir directory.
        /// </summary>
        [Xml]
        public string Source;

        /// <summary>
        /// The label attributed to the volume. This is the volume label returned by the GetVolumeInformation function.
        /// If the SourceDir property refers to a removable (floppy or CD-ROM) volume, then this volume label is used
        /// to verify that the proper disk is in the drive before attempting to install files. The entry in this column
        /// must match the volume label of the physical media.
        /// </summary>
        [Xml]
        public string VolumeLabel;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <returns></returns>
        public XContainer ToXml(string projectId="")
        {
            if (Cabinet.IsNotEmpty())
                Cabinet = Cabinet.Replace("{projectId}", projectId);

            return new XElement("Media")
                       .AddAttributes(this.MapToXmlAttributes())
                       .AddAttributes(this.Attributes);
        }
    }
}