using System;
using System.Linq;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// The type of modification to be made.
    /// This attribute's value must be one of the following:
    /// </summary>
    public enum IniFileAction
    {
        /// <summary>
        /// Creates or updates an .ini entry.
        /// </summary>
        addLine,

        /// <summary>
        /// Creates a new entry or appends a new comma-separated value to an existing entry.
        /// </summary>
        addTag,

        /// <summary>
        /// Creates an .ini entry only if the entry does no already exist.
        /// </summary>
        createLine,

        /// <summary>
        /// Removes an .ini entry.
        /// </summary>
        removeLine,

        /// <summary>
        /// Removes a tag from an .ini entry.
        /// </summary>
        removeTag,
    }

    /// <summary>
    /// Adds or removes .ini file entries.
    /// </summary>
    public class IniFile : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Identifier for ini file.
        /// </summary>
        [Xml]
        public new string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// The type of modification to be made.
        /// </summary>
        [Xml]
        public IniFileAction? Action;

        /// <summary>
        /// Name of a property, the value of which is the full path of the folder containing the .ini file.
        /// <remarks>
        /// Can be name of a directory in the Directory table, a property set by the AppSearch table, or any other property representing a full path.
        /// </remarks>
        /// </summary>
        [Xml]
        public string Directory;

        /// <summary>
        /// The localizable .ini file key within the section.
        /// </summary>
        [Xml]
        public string Key;

        /// <summary>
        /// In prior versions of the WiX toolset, this attribute specified the short name.
        /// This attribute's value may now be either a short or long name.
        /// If a short name is specified, the ShortName attribute may not be specified.
        /// If a long name is specified, the LongName attribute may not be specified.
        /// Also, if this value is a long name, the ShortName attribute may be omitted to allow WiX to attempt to generate a unique short name.
        /// However, if this name collides with another file or you wish to manually specify the short name, then the ShortName attribute may be specified.
        /// </summary>
        [Xml]
        public new string Name { get => base.Name; set => base.Name = value; }

        /// <summary>
        /// This attribute has been deprecated; please use the Name attribute instead.
        /// </summary>
        [Obsolete(message: "This method name is obsolete use `Name` instead")]
        [Xml]
        public string LongName;

        /// <summary>
        /// The short name of the in 8.3 format.
        /// <remarks>
        /// This attribute should only be set if there is a conflict between generated short names or the user wants to manually specify the short name.
        /// </remarks>
        /// </summary>
        [Xml]
        public string ShortName;

        /// <summary>
        /// The localizable .ini file section.
        /// </summary>
        [Xml]
        public string Section;

        /// <summary>
        /// The localizable value to be written or deleted.
        /// <remarks>
        /// This attribute must be set if the Action attribute's value is "addLine", "addTag", or "createLine".
        /// </remarks>
        /// </summary>
        [Xml]
        public string Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public IniFile(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        public IniFile(Feature feature, string name)
        {
            Feature = feature;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        public IniFile(string name, string directory)
        {
            Name = name;
            Directory = directory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        public IniFile(Feature feature, string name, string directory)
        {
            Feature = feature;
            Name = name;
            Directory = directory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        public IniFile(string name, string directory, IniFileAction action)
        {
            Name = name;
            Directory = directory;
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        public IniFile(Feature feature, string name, string directory, IniFileAction action)
        {
            Feature = feature;
            Name = name;
            Directory = directory;
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        public IniFile(string name, string directory, IniFileAction action, string section)
        {
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        public IniFile(Feature feature, string name, string directory, IniFileAction action, string section)
        {
            Feature = feature;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        public IniFile(string name, string directory, IniFileAction action, string section, string key)
        {
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        public IniFile(Feature feature, string name, string directory, IniFileAction action, string section, string key)
        {
            Feature = feature;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public IniFile(string name, string directory, IniFileAction action, string section, string key, string value)
        {
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public IniFile(Feature feature, string name, string directory, IniFileAction action, string section, string key, string value)
        {
            Feature = feature;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public IniFile(Id id)
        {
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The feature.</param>
        public IniFile(Id id, Feature feature)
        {
            Id = id;
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public IniFile(Id id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        public IniFile(Id id, Feature feature, string name)
        {
            Id = id;
            Feature = feature;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        public IniFile(Id id, string name, string directory)
        {
            Id = id;
            Name = name;
            Directory = directory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        public IniFile(Id id, Feature feature, string name, string directory)
        {
            Id = id;
            Feature = feature;
            Name = name;
            Directory = directory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        public IniFile(Id id, string name, string directory, IniFileAction action)
        {
            Id = id;
            Name = name;
            Directory = directory;
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        public IniFile(Id id, Feature feature, string name, string directory, IniFileAction action)
        {
            Id = id;
            Feature = feature;
            Name = name;
            Directory = directory;
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        public IniFile(Id id, string name, string directory, IniFileAction action, string section)
        {
            Id = id;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        public IniFile(Id id, Feature feature, string name, string directory, IniFileAction action, string section)
        {
            Id = id;
            Feature = feature;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        public IniFile(Id id, string name, string directory, IniFileAction action, string section, string key)
        {
            Id = id;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        public IniFile(Id id, Feature feature, string name, string directory, IniFileAction action, string section, string key)
        {
            Id = id;
            Feature = feature;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public IniFile(Id id, string name, string directory, IniFileAction action, string section, string key, string value)
        {
            Id = id;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="action">The action.</param>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public IniFile(Id id, Feature feature, string name, string directory, IniFileAction action, string section, string key, string value)
        {
            Id = id;
            Feature = feature;
            Name = name;
            Directory = directory;
            Action = action;
            Section = section;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            this.CreateAndInsertParentComponent(context)
                .Add(this.ToXElement("IniFile"));
        }
    }
}