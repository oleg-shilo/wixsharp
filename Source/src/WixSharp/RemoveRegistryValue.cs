using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Win32;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// Used to remove a registry value during installation.
    /// There is no standard way to remove a single registry value during uninstall (but you can remove an entire key with <see cref="RemoveRegistryKey" />).
    /// </summary>
    public class RemoveRegistryValue : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id
        {
            get { return base.Id; }
            set { Id = value; }
        }

        /// <summary>
        /// The localizable key for the registry value.
        /// If the parent element is a RegistryKey, this value may be omitted to use the path of the parent, or if its specified it will be appended to the path of the parent.
        /// </summary>
        [Xml]
        public string Key;

        /// <summary>
        /// The localizable registry value name.
        /// If this attribute is not provided the default value for the registry key will be set instead.
        /// The Windows Installer allows several special values to be set for this attribute.
        /// You should not use them in WiX. Instead use appropriate values in the Action attribute to get the desired behavior.
        /// </summary>
        [Xml]
        public new string Name;

        /// <summary>
        /// The predefined root key for the registry value.
        /// </summary>
        public RegistryHive Root = RegistryHive.LocalMachine;

        [Xml(Name = "Root")]
        private string root { get { return Root.ToWString(); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Feature feature, string key)
        {
            Key = key;
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="root">The Root.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(RegistryHive root, string key)
        {
            Root = root;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <param name="root">The Root.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Feature feature, RegistryHive root, string key)
        {
            Root = root;
            Key = key;
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="name">The Name.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(string key, string name)
        {
            Key = key;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <param name="name">The Name.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Feature feature, string key, string name)
        {
            Key = key;
            Name = name;
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="root">The Root.</param>
        /// <param name="name">The Name.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(RegistryHive root, string key, string name)
        {
            Root = root;
            Key = key;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <param name="root">The Root.</param>
        /// <param name="name">The Name.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Feature feature, RegistryHive root, string key, string name)
        {
            Root = root;
            Key = key;
            Name = name;
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Id id, string key)
        {
            Id = id;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="feature">The Feature.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Id id, Feature feature, string key)
        {
            Id = id;
            Feature = feature;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="root">The Root.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Id id, RegistryHive root, string key)
        {
            Id = id;
            Root = root;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="feature">The Feature.</param>
        /// <param name="root">The Root.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Id id, Feature feature, RegistryHive root, string key)
        {
            Id = id;
            Feature = feature;
            Root = root;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="name">The Name.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Id id, string key, string name)
        {
            Id = id;
            Key = key;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="feature">The Feature.</param>
        /// <param name="name">The Name.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Id id, Feature feature, string key, string name)
        {
            Id = id;
            Feature = feature;
            Key = key;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="root">The Root.</param>
        /// <param name="name">The Name.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Id id, RegistryHive root, string key, string name)
        {
            Id = id;
            Root = root;
            Key = key;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryValue" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="feature">The Feature.</param>
        /// <param name="root">The Root.</param>
        /// <param name="name">The Name.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryValue(Id id, Feature feature, RegistryHive root, string key, string name)
        {
            Id = id;
            Feature = feature;
            Root = root;
            Key = key;
            Name = name;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            XElement component = this.CreateParentComponent();
            component.Add(this.ToXElement("RemoveRegistryValue"));

            XElement bestParent = context.XParent.FindFirstComponentParent() ??
                                  context.XParent.FistProgramFilesDir();

            bestParent.Add(component);

            MapComponentToFeatures(component.Attr("Id"), ActualFeatures, context);
        }
    }
}