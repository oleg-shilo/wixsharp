using System.Xml.Linq;
using Microsoft.Win32;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// This is the action that will be taken for this registry value.
    /// This attribute's value must be one of the following:
    /// </summary>
    public enum RemoveRegistryKeyAction
    {
        /// <summary>
        /// Removes a key with all its values and subkeys when the parent component is installed.
        /// </summary>
        removeOnInstall,

        /// <summary>
        /// Removes a key with all its values and subkeys when the parent component is uninstalled.
        /// </summary>
        removeOnUninstall,
    }

    /// <summary>
    /// Used for removing registry keys and all child keys either during install or uninstall.
    /// </summary>
    public class RemoveRegistryKey : WixEntity, IGenericEntity
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
        /// </summary>
        [Xml]
        public string Key;

        /// <summary>
        /// The predefined root key for the registry value.
        /// </summary>
        public RegistryHive Root = RegistryHive.LocalMachine;

        /// <summary>
        /// This is the action that will be taken for this registry value.
        /// </summary>
        [Xml]
        public RemoveRegistryKeyAction Action = RemoveRegistryKeyAction.removeOnUninstall;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryKey" /> class.
        /// </summary>
        /// <param name="key">The Key.</param>
        public RemoveRegistryKey(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryKey" /> class.
        /// </summary>
        /// <param name="root">The Root.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryKey(RegistryHive root, string key)
        {
            Root = root;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryKey" /> class.
        /// </summary>
        /// <param name="root">The Root.</param>
        /// <param name="key">The Key.</param>
        /// <param name="action">The Action.</param>
        public RemoveRegistryKey(RegistryHive root, string key, RemoveRegistryKeyAction action)
        {
            Root = root;
            Key = key;
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryKey" /> class.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryKey(Id id, string key)
        {
            Id = id;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryKey" /> class.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="root">The Root.</param>
        /// <param name="key">The Key.</param>
        public RemoveRegistryKey(Id id, RegistryHive root, string key)
        {
            Id = id;
            Root = root;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRegistryKey" /> class.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="root">The Root.</param>
        /// <param name="key">The Key.</param>
        /// <param name="action">The Action.</param>
        public RemoveRegistryKey(Id id, RegistryHive root, string key, RemoveRegistryKeyAction action)
        {
            Id = id;
            Root = root;
            Key = key;
            Action = action;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.XParent
                .FindFirst("Component")
                .Add(this.ToXElement("RemoveRegistryKey")
                .AddAttributes("Root={0}".FormatWith(Root.ToWString())));
        }
    }
}
