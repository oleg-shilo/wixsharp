#region Licence...

/*
The MIT License (MIT)
Copyright (c) 2014 Oleg Shilo
Permission is hereby granted,
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#endregion Licence...

using System;
using System.Linq;
using Microsoft.Win32;

namespace WixSharp
{
    /// <summary>
    /// Values of this type represent possible registry roots.
    /// </summary>
    /// <seealso cref="WixSharp.StringEnum{T}" />
    public class RegistryHive : StringEnum<RegistryHive>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryHive"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public RegistryHive(string value) : base(value) { }

        /// <summary>
        /// The classes root. Equivalent of `HKCR`.
        /// </summary>
        public static RegistryHive ClassesRoot = new RegistryHive("HKCR");

        /// <summary>
        /// The current user.  Equivalent of `HKCU`.
        /// </summary>
        public static RegistryHive CurrentUser = new RegistryHive("HKCU");

        /// <summary>
        /// The local machine.  Equivalent of `HKLM`.
        /// </summary>
        public static RegistryHive LocalMachine = new RegistryHive("HKLM");

        /// <summary>
        /// The users.  Equivalent of `HKU`.
        /// </summary>
        public static RegistryHive Users = new RegistryHive("HKU");

        /// <summary>
        /// Defines ”HKMU” value, which makes it so the registry entry will appear in HKLM
        /// when a per-machine install is run and in HKCU when a per-user install us run.
        /// </summary>
        public static RegistryHive LocalMachineOrUsers = new RegistryHive("HKMU");
    }

    /// <summary>
    /// Defines the registry file (*.reg) containing the entries to be installed.
    /// <para>
    /// Compiler uses the data from this class to call <see cref="T:WixSharp.CommonTasks.ImportRegFile"/>
    /// internally and inject imported <see cref="RegValue"/>s into the <see cref="Project"/>.
    /// </para>
    /// </summary>
    /// <example>The following sample demonstrates how to install Registry entries imported from
    /// the reg file:
    /// <code>
    /// var project =
    ///     new Project("MyProduct",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File(@"readme.txt")),
    ///         new RegFile("MyProduct.reg"),
    ///         ...
    ///
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public class RegFile : WixObject
    {
        /// <summary>
        /// The path to the registry file (*.reg).
        /// </summary>
        public string Path = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegFile"/> class.
        /// </summary>
        public RegFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegFile"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="path">The path.</param>
        public RegFile(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegFile"/> class.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the registry value should be included in.</param>
        /// <param name="path">The path.</param>
        public RegFile(Feature feature, string path)
        {
            Path = path;
            Feature = feature;
        }
    }

    /// <summary>
    /// Defines registry value to be installed.
    /// </summary>
    ///<example>The following is a complete example of the setup for installing a file and two
    ///registry values (string value <c>Message</c> and integer value <c>Count</c> ).
    ///<code>
    ///static public void Main(string[] args)
    ///{
    ///     var project =
    ///         new Project("MyProduct",
    ///             new Dir(@"%ProgramFiles%\My Company\My Product",
    ///                 new File(@"readme.txt")),
    ///
    ///             new RegValue(RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Message", "Hello"),
    ///             new RegValue(RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Count", 777));
    ///
    ///     Compiler.BuildMsi(project);
    /// }
    /// </code>
    /// </example>
    public class RegValue : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegValue"/> class.
        /// </summary>
        public RegValue() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegValue"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="key">The registry key name.</param>
        /// <param name="name">The registry entry name.</param>
        /// <param name="value">The registry entry value.</param>
        public RegValue(string key, string name, object value)
        {
            Name = name;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegValue"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="root">The registry hive name.</param>
        /// <param name="key">The registry key name.</param>
        /// <param name="name">The registry entry name.</param>
        /// <param name="value">The registry entry value.</param>
        public RegValue(RegistryHive root, string key, string name, object value)
        {
            Name = name;
            Root = root;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegValue"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the registry value should be included in.</param>
        /// <param name="root">The registry hive name.</param>
        /// <param name="key">The registry key name.</param>
        /// <param name="name">The registry entry name.</param>
        /// <param name="value">The registry entry value.</param>
        public RegValue(Feature feature, RegistryHive root, string key, string name, object value)
        {
            Name = name;
            Root = root;
            Key = key;
            Value = value;
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegValue"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="RegValue"/> instance.</param>
        /// <param name="key">The registry key name.</param>
        /// <param name="name">The registry entry name.</param>
        /// <param name="value">The registry entry value.</param>
        public RegValue(Id id, string key, string name, object value)
        {
            Id = id.Value;
            Name = name;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegValue"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="RegValue"/> instance.</param>
        /// <param name="root">The registry hive name.</param>
        /// <param name="key">The registry key name.</param>
        /// <param name="name">The registry entry name.</param>
        /// <param name="value">The registry entry value.</param>
        public RegValue(Id id, RegistryHive root, string key, string name, object value)
        {
            Id = id.Value;
            Name = name;
            Root = root;
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegValue"/> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="RegValue"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the registry value should be included in.</param>
        /// <param name="root">The registry hive name.</param>
        /// <param name="key">The registry key name.</param>
        /// <param name="name">The registry entry name.</param>
        /// <param name="value">The registry entry value.</param>
        public RegValue(Id id, Feature feature, RegistryHive root, string key, string name, object value)
        {
            Id = id.Value;
            Name = name;
            Root = root;
            Key = key;
            Value = value;
            Feature = feature;
        }

        /// <summary>
        /// The registry hive name.
        /// <para>Default value is <c>RegistryHive.CurrentUser</c></para>
        /// </summary>
        public RegistryHive Root = RegistryHive.CurrentUser;

        /// <summary>
        /// The registry key name.
        /// <para>Default value is <c>String.Empty</c></para>
        /// </summary>
        public string Key = "";

        /// <summary>
        /// The registry entry value.
        /// </summary>
        public object Value = null;

        /// <summary>
        /// The permissions associated with the registry value
        /// </summary>
        public Permission[] Permissions = null;

        /// <summary>
        /// Defines the installation <see cref="Condition"/>, which is to be checked during the installation to
        /// determine if the registry value should be created on the target system.
        /// </summary>
        public Condition Condition;

        /// <summary>
        /// Facilitates the installation of packages that include both 32-bit and 64-bit components.
        /// Set this attribute to 'yes' to mark the corresponding RegValue as a 64-bit component.
        /// </summary>
        public bool Win64 = false;

        /// <summary>
        /// Set this to create an empty key, if absent, when the parent component is installed.
        /// </summary>
        public RegistryKeyAction RegistryKeyAction = RegistryKeyAction.none;

        /// <summary>
        /// Set this to create an empty key, if absent, when the parent component is installed.
        /// </summary>
        public bool ForceCreateOnInstall;

        /// <summary>
        /// Set this to remove the key with all its values and subkeys when the parent component is uninstalled.
        /// </summary>
        public bool ForceDeleteOnUninstall;

        /// <summary>
        /// Gets or sets the NeverOverwrite attribute of the associated WiX component.
        /// <para>If this attribute is set to 'true', the installer does not install or reinstall the component
        ///  if a registry value for the component already exists. </para>
        /// </summary>
        /// <value>
        /// The never overwrite.
        /// </value>
        public bool? NeverOverwrite
        {
            get => attributesBag.Get("Component:NeverOverwrite") == "yes";

            set => attributesBag.Set("Component:NeverOverwrite", value.ToNullOrYesNo());
        }

        internal string RegValueString
        {
            get
            {
                if (Value is byte[])
                {
                    string hex = BitConverter.ToString(Value as byte[]);
                    return hex.Replace("-", "");
                }
                else
                {
                    return Value.ToString();
                }
            }
        }

        internal string RegTypeString
        {
            get
            {
                if (Value is String)
                {
                    var value = Value as string;
                    if (value.Contains("\n"))
                        return "multiString";
                    else if (value.Contains("%"))
                        return "expandable";
                    else
                        return "string";
                }
                else if (Value is byte[])
                {
                    return "binary";
                }
                else if (Value is Int16 || Value is Int32)
                {
                    return "integer";
                }
                else
                {
                    return "unsupported type";
                }
            }
        }
    }
}