using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace WixSharp
{
    /// <summary>
    /// Searches for directory and assigns to value of parent Property.
    /// </summary>
    /// <example>
    /// The following sample demonstrates how to use FileSearch in the canonical WiX DirectorySearch and
    /// FileSearch use-case. The reg file:
    /// <code>
    ///var project =
    ///new Project("MyProduct",
    ///new Dir(@"%ProgramFiles%\My Company\My Product",
    ///new Property("EXISTING_FILE", "NOT_FOUND",
    ///new DirectorySearch(@"%ProgramFiles%\My Company\My Product", 1, new FileSearch("product.exe"))),
    ///...
    ///Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public class DirectorySearch : GenericNestedEntity, IGenericEntity
    {
        /// <summary>
        /// Unique identifier for the directory search.
        /// </summary>
        [Xml]
        new public string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Set the value of the outer Property to the result of this search.
        /// Use the AssignToProperty attribute to search for a file but set the outer property to the directory
        /// containing the file. When this attribute is set to 'true', you may only nest a FileSearch element with a
        /// unique ID or define no child element.
        /// </summary>
        [Xml]
        public bool? AssignToProperty;

        /// <summary>
        /// Depth below the path that the installer searches for the file or directory specified by the search.
        /// When the parent DirectorySearch Depth attribute is greater than 0, the FileSearch ID attribute must be
        /// absent or the same as the parent DirectorySearch ID attribute value, unless the parent
        /// DirectorySearch AssignToProperty attribute value is 'true'.
        /// </summary>
        [Xml]
        public int? Depth;

        /// <summary>
        /// Path on the user's system. Either absolute or relative to containing directories.
        /// </summary>
        [Xml]
        public string Path;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySearch"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="assignToProperty">If the parent property should be assigned the value of the search.</param>
        /// <param name="depth">The search depth.</param>
        /// <param name="children">The children.</param>
        public DirectorySearch(Id id, string name, bool assignToProperty, int depth, params IGenericEntity[] children)
        {
            Id = id;
            AssignToProperty = assignToProperty;
            Name = name;
            Depth = depth;
            Children = children;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySearch"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="assignToProperty">If the parent property should be assigned the value of the search.</param>
        /// <param name="depth">The search depth.</param>
        /// <param name="children">The children.</param>
        public DirectorySearch(string name, bool assignToProperty, int depth, params IGenericEntity[] children)
        {
            AssignToProperty = assignToProperty;
            Name = name;
            Depth = depth;
            Children = children;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see
        /// cref="T:WixSharp.Project"/>. See 'Wix#/samples/Extensions' sample for the details on how
        /// to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            base.Process(context, "DirectorySearch");
        }
    }
}