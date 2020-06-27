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

using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Defines WiX <c>Condition</c>. <c>Condition</c> is normally associated with <c>CustomActions</c> or WiX elements (e.g. <c>Shortcut</c>).
    /// <para>
    /// <see cref="Condition"/> is nothing else but an XML friendly string wrapper, containing
    /// some predefined (commonly used) condition values. You can either use one of the
    /// predefined condition values (static members) or define your by specifying full string representation of
    /// the required WiX condition when calling the constructor or static method <c>Create</c>.
    /// </para>
    /// </summary>
    /// <example>The following is an example of initializing the Shortcut.<see cref="Shortcut.Condition"/>
    /// with custom value <c>INSTALLDESKTOPSHORTCUT="yes"</c> and
    /// the InstalledFileAction.<see cref="T:WixSharp.InstalledFileAction.Condition"/> with perefined value <c>NOT_Installed</c>:
    /// <code>
    /// var project =
    ///     new Project("My Product",
    ///     ...
    ///     new Dir(@"%Desktop%",
    ///         new WixSharp.Shortcut("MyApp", "[INSTALLDIR]MyApp.exe", "")
    ///         {
    ///            Condition = new Condition("INSTALLDESKTOPSHORTCUT=\"yes\"")
    ///         }),
    ///
    ///     new InstalledFileAction("MyApp.exe", "",
    ///                              Return.check,
    ///                              When.After,
    ///                              Step.InstallFinalize,
    ///                              Condition.NOT_Installed),
    ///         ...
    ///
    /// </code>
    /// </example>
    public class LaunchCondition : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LaunchCondition"/> class.
        /// It is made protected to encourage user to use non-default constructor to handle Escape Characters
        /// of the <c>value</c> string properly.
        /// </summary>
        public LaunchCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LaunchCondition"/> class with properties/fields initialized with specified parameters
        /// </summary>
        /// <param name="value">The value of the WiX condition expression.</param>
        /// <param name="message">The message to be displayed during the installation if <see cref="LaunchCondition"/> evaluated as <c>False</c>.</param>
        public LaunchCondition(string value, string message)
        {
            //Value = System.Security.SecurityElement.Escape(value);
            Value = value;
            Message = message;
        }

        /// <summary>
        ///  Returns the WiX <c>LaunchCondition</c> as a string.
        /// </summary>
        /// <returns>A string representing the condition.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        ///  Returns the WiX <c>Condition</c> as a <see cref="T:System.Xml.Linq.XCData"/>.
        ///  <remarks> Normally <c>Condition</c> is not designed to be parsed by the XML parser thus it should be embedded as CDATA</remarks>
        /// <code>
        /// &lt;Condition&gt;&lt;![CDATA[NETFRAMEWORK20="#0"]]&gt;&lt;/Condition&gt;
        /// </code>
        /// </summary>
        /// <returns>A CDATA string representing the condition.</returns>
        public XCData ToCData()
        {
            return new XCData(Value);
        }

        /// <summary>
        /// String value of WiX <c>Condition</c>.
        /// </summary>
        public string Value = "";

        /// <summary>
        /// The message to be displayed during the installation if <see cref="LaunchCondition"/> evaluated as <c>False</c>.
        /// </summary>
        public string Message = "";
    }
}