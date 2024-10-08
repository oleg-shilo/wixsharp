using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace WixSharp
{
    /// <summary>
    /// The value must be 'file' if the child is a FileSearch element, and must be 'directory' if
    /// child is a DirectorySearch element. This attribute's value must be one of the following:
    /// </summary>
    public enum RegistrySearchType
    {
        /// <summary>
        /// The registry value contains the path to a directory.
        /// </summary>
        directory,

        /// <summary>
        ///The registry value contains the path to a file. To return the full file path you must add a FileSearch element as a child of this element; otherwise, the parent directory of the file path is returned.
        /// </summary>
        file,

        /// <summary>
        /// <para>
        /// Sets the raw value from the registry value. Please note that this value will contain a
        /// prefix as follows:
        /// </para>
        /// <para></para>
        /// <para>DWORD: Starts with '#' optionally followed by '+' or '-'.</para>
        /// <para>
        /// REG_BINARY: Starts with '#x' and the installer converts and saves each hexadecimal digit
        ///             (nibble) as an ASCII character prefixed by '#x'.
        /// </para>
        /// <para>REG_EXPAND_SZ: Starts with '#%'.</para>
        /// <para>REG_MULTI_SZ: Starts with '[~]' and ends with '[~]'.</para>
        /// <para>
        /// REG_SZ: No prefix, but if the first character of the registry value is '#', the
        ///         installer escapes the character by prefixing it with another '#'.
        /// </para>
        /// </summary>
        raw
    }

    /// <summary>
    /// Searches for file, directory or registry key and assigns to value of parent Property.
    /// </summary>
    /// <example>
    /// The following sample demonstrates how to use RegisterySearch in canonical WiX
    /// RegisterySearch and FileSearch use-case. the reg file:
    /// <code>
    /// var project =
    ///     new Project("MyProduct",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new Property("SQL_BROWSER_LOCATION",
    ///                 new RegistrySearch(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\services\SQLBrowser", "ImagePath", RegistrySearchType.file,
    ///                 new FileSearch("sqlbrowser.exe"))),
    /// ...
    ///
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public class RegistrySearch : GenericNestedEntity, IGenericEntity
    {
        /// <summary>
        /// Signature to be used for the file, directory or registry key being searched for.
        /// </summary>
        /// <value>The identifier.</value>
        [Xml]
        new public string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Registry value name. If this value is null, then the value from the key's unnamed or
        /// default value, if any, is retrieved.
        /// </summary>
        [Xml]
        new public string Name;

        /// <summary>
        /// Root key for the registry value.This attribute's value must be one of the following:
        /// <para>- ClassesRoot</para>
        /// <para>- CurrentUser</para>
        /// <para>- LocalMachine</para>
        /// <para>- Users</para>
        /// </summary>
        public RegistryHive Root;

        // note string vs. RegistryHive. To support WiX values that are not available in RegistryHive
        [Xml(Name = "Root")]
        internal string root { get => Root; }

        /// <summary>
        /// Key for the registry value.
        /// </summary>
        [Xml]
        public string Key;

        /// <summary>
        /// The value must be 'file' if the child is a FileSearch element, and must be 'directory'
        /// if child is a DirectorySearch element.
        /// </summary>
        [Xml]
        public RegistrySearchType Type;

        /// <summary>
        /// Overrides the default registry to search. The value `true` (always64) will force the search to look in the 64-bit
        /// registry even when building for 32-bit. Simliarly, the value `false` (always32) will force the search to look in the
        /// 32-bit registry even when building for 64-bit. The default value is default where the search will look in
        /// the same registry as the bitness of the package..
        /// </summary>
        public bool? Win64;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySearch"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="root">The root.</param>
        /// <param name="key">The key.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="children">The children.</param>
        public RegistrySearch(Id id, RegistryHive root, string key, string name, RegistrySearchType type, params IGenericEntity[] children)
        {
            Id = id;
            Root = root;
            Key = key;
            Name = name;
            Type = type;
            Children = children;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySearch"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="key">The key.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="children">The children.</param>
        public RegistrySearch(RegistryHive root, string key, string name, RegistrySearchType type, params IGenericEntity[] children)
        {
            Root = root;
            Key = key;
            Name = name;
            Type = type;
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
            // var platform = (context.Project as Project)?.Platform;
            var proj = (context.Project as Project);

            if (this.Win64 == null && proj.Platform != null)
                this.Win64 = proj.Is64Bit;

            base.Process(context, "RegistrySearch")
                .SetAttribute("Bitness", Win64.HasValue ? (Win64.Value ? "always64" : "always32") : null);
        }
    }
}