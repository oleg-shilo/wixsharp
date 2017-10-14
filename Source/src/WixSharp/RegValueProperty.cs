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

using Microsoft.Win32;

namespace WixSharp
{
    /// <summary>
    /// Defines WiX custom property assigned by MSI runtime during the installation to the RegistrySearch result.
    /// <para>
    /// <see cref="RegValueProperty"/> is used to set a property from the registry value when accessing the registry is inconvenient for
    /// a custom action.
    ///
    /// <para><c>RegistrySearch</c> returns raw data thus data will always contain prefix indicating data type: </para>
    /// <para><c>DWORD:</c> Starts with '#' optionally followed by '+' or '-'.</para>
    /// <para><c>REG_BINARY:</c> Starts with '#x' and the installer converts and saves each hexadecimal digit (nibble) as an ASCII character prefixed by '#x'.</para>
    /// <para><c>REG_EXPAND_SZ:</c> Starts with '#%'.</para>
    /// <para><c>REG_MULTI_SZ:</c> Starts with '[~]' and ends with '[~]'.</para>
    /// <para><c>REG_SZ:</c> No prefix, but if the first character of the registry value is '#', the installer escapes the character by prefixing it with another '#'.</para>
    /// </para>
    /// </summary>
    /// <remarks>In Wix# the preferred way of setting a property value is assigning it from <see cref="T:WixSharp.ManagedAction"/>.
    /// However <see cref="T:WixSharp.ManagedAction"/> may not be an available option if the target system does not have .NET installed.
    /// In such cases <see cref="RegValueProperty"/> should be used.</remarks>
    ///<example>The following is an example of installing <c>DotNET Manual.txt</c> file only if .NET v2.0 is installed.
    ///<code>
    ///
    /// var project =
    ///         new Project("MyProduct",
    ///             new Dir(@"%ProgramMenu%\My Company\My Product",
    ///                 new File(@"Files\DotNET Manual.txt")
    ///                     {
    ///                         Condition = new Condition("NETFRAMEWORK20=\"#1\"")
    ///                     },
    ///     ...
    ///             new RegValueProperty("NETFRAMEWORK20",
    ///                                  RegistryHive.LocalMachine,
    ///                                  @"Software\Microsoft\NET Framework Setup\NDP\v2.0.50727",
    ///                                  "Install",
    ///                                  "not_defined"),
    ///
    ///
    /// </code>
    /// </example>
    public partial class RegValueProperty : Property
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegValueProperty"/> class.
        /// </summary>
        public RegValueProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegValueProperty"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="root">The registry hive name.</param>
        /// <param name="key">The registry key name.</param>
        /// <param name="entryName">The registry entry name.</param>
        /// <param name="defaultValue">The registry entry default value.</param>
        public RegValueProperty(string name, RegistryHive root, string key, string entryName, string defaultValue = "")
        {
            Name = name;
            Root = root;
            EntryName = entryName;
            Key = key;
            Value = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegValueProperty"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="RegValueProperty"/> instance.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="root">The registry hive name.</param>
        /// <param name="key">The registry key name.</param>
        /// <param name="entryName">The registry entry name.</param>
        /// <param name="defaultValue">The registry entry default value.</param>
        public RegValueProperty(Id id, string name, RegistryHive root, string key, string entryName, string defaultValue = "")
        {
            Id = id.Value;
            Name = name;
            Root = root;
            EntryName = entryName;
            Key = key;
            Value = defaultValue;
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
        /// The registry entry name.
        /// <para>Default value is <c>String.Empty</c></para>
        /// </summary>
        public string EntryName = "";

        /// <summary>
        /// Instructs the search to look in the 64-bit registry when the value is 'yes'.
        /// When the value is 'no', the search looks in the 32-bit registry.
        /// <para>
        /// This is how WiX defines defaults.
        /// </para>
        /// <para>
        /// The default value is based on the platform set by the -arch switch to candle.exe or the
        /// InstallerPlatform property in a .wixproj MSBuild project: For x86 and ARM, the default
        /// value is 'no'. For x64 and IA64, the default value is 'yes'.
        /// </para>
        /// <para>
        /// However this WiX approach creates non-intuitive behaver when the build outcome may depend on the
        /// architecture of the build machine.</para>
        /// <para>Thus WixSharp always uses consistent default value for this property: <c>false</c>.</para>
        /// </summary>
        public bool Win64
        {
            get => win64;
            set { win64 = value; win64_SetByUser = true; }
        }

        // cannot use bool? as (while WiX allows this) it's not acceptable to skip this argument. Skipping will lead to
        // non-intuitive defaults (see comments above). And yet it's important to know if the value was explicitly
        // set by the user so AutoElements assignments do not overwrite it.
        bool win64 = false;

        internal bool win64_SetByUser = false;
    }
}