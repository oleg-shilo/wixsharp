using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;

namespace WixSharp
{
    /// <summary>
    /// Searches for file and assigns to full path value of parent Property
    /// </summary>
    /// <example>The following sample demonstrates how to use FileSearch in canonical WiX RegisterySearch and FileSearch use-case.
    /// the reg file:
    /// <code>
    /// var project =
    ///     new Project("MyProduct",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         new Property("SQL_BROWSER_LOCATION",
    ///             new RegistrySearch(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\services\SQLBrowser", "ImagePath", RegistrySearchType.file,
    ///                 new FileSearch("sqlbrowser.exe"))),
    ///         ...
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public class FileSearch : GenericNestedEntity, IGenericEntity
    {
        /// <summary>
        /// Unique identifier for the file search and external key into the Signature table.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Xml]
        new public string Id { get => base.Id; set => Id = value; }

        /// <summary>
        /// The languages supported by the file.
        /// </summary>
        [Xml]
        public string Languages;

        /// <summary>
        /// The maximum modification date and time of the file. Formatted as YYYY-MM-DDTHH:mm:ss,
        /// where YYYY is the year, MM is month, DD is day, 'T' is literal, HH is hour, mm is
        /// minute and ss is second.
        /// </summary>
        [Xml]
        public string DateTime;

        /// <summary>
        /// The maximum size of the file.
        /// </summary>
        [Xml]
        public int? MaxSize;

        /// <summary>
        /// The minimum version of the file
        /// </summary>
        [Xml]
        public string MaxVersion;

        /// <summary>
        /// In prior versions of the WiX toolset, this attribute specified the short file name.
        /// This attribute's value may now be either a short or long file name. If a short file name
        /// is specified, the ShortName attribute may not be specified. If a long file name is specified,
        /// the LongName attribute may not be specified. If you wish to manually specify the short file name,
        /// then the ShortName attribute may be specified.
        /// </summary>
        [Xml]
        new public string Name;

        /// <summary>
        /// The short file name of the file in 8.3 format. There is a Windows Installer bug which prevents the
        /// FileSearch functionality from working if both a short and long file name are specified. Since the
        /// Name attribute allows either a short or long name to be specified, it is the only attribute related
        /// to file names which should be specified.
        /// </summary>
        [Xml]
        public string ShortName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSearch"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        public FileSearch(Id id, string name, params IGenericEntity[] children)
        {
            Id = id;
            Name = name;
            Children = children;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSearch"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        public FileSearch(string name, params IGenericEntity[] children)
        {
            Name = name;
            Children = children;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="T:WixSharp.Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            base.Process(context, "FileSearch");
        }
    }
}