#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2015 Oleg Shilo

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
using System.Collections.Generic;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Defines FileType association to be created for the file extension and the installed file/application (parent  <see cref="T:WixSharp.File"/>).
    /// </summary>
    ///<example>The following is an example of associating file extension ".my" with installed application MyViewer.exe.
    ///<code>
    /// var project =
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new Dir(@"Notepad",
    ///                 new File("MyViewer.exe",
    ///                     new FileAssociation("my", "application/my", "open", "\"%1\"")))),
    ///         new EnvironmentVariable("MYPRODUCT_DIR", "[INSTALLDIR]"),
    ///         new EnvironmentVariable("PATH", "[INSTALLDIR]") { Part = EnvVarPart.last });
    ///         ...
    ///
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class EnvironmentVariable : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value of the environment variable.</param>
        public EnvironmentVariable(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariable"/> class.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="EnvironmentVariable"/> instance.</param>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value of the environment variable.</param>
        public EnvironmentVariable(Id id, string name, string value)
        {
            this.Id = id.Value;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariable"/> class.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the environment variable should be included in.</param>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value of the environment variable.</param>
        public EnvironmentVariable(Feature feature, string name, string value)
        {
            this.Feature = feature;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariable"/> class.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="EnvironmentVariable"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the environment variable should be included in.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public EnvironmentVariable(Id id, Feature feature, string name, string value)
        {
            this.Id = id.Value;
            this.Feature = feature;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Specifies that the environment variable should be added to the system environment space. The default is 'no' which
        /// indicates the environment variable is added to the user environment space.
        /// </summary>
        public bool? System;

        /// <summary>
        /// Specifies that the environment variable should not be removed on uninstall.
        /// </summary>
        public bool? Permanent;

        /// <summary>
        /// The value to set into the environment variable. If this attribute is not set, the environment variable is removed
        /// during installation if it exists on the machine.
        /// </summary>
        public string Value = null;

        /// <summary>
        /// Specifies whether the environmental variable should be created, set or removed when the parent component is installed. <see cref="T:WixSharp.EnvironmentVariable.Action"/>.
        /// </summary>
        public EnvVarAction Action = EnvVarAction.set;

        /// <summary>
        /// Indicates how value should be set.
        /// </summary>
        public EnvVarPart? Part;

        /// <summary>
        /// Defines the installation <see cref="Condition"/>, which is to be checked during the installation to
        /// determine if the registry value should be created on the target system.
        /// </summary>
        public Condition Condition;

        /// <summary>
        /// Emits the XML.
        /// </summary>
        public XContainer ToXml()
        {
            var element = new XElement("Environment",
                              new XAttribute("Id", Id),
                              new XAttribute("Name", Name),
                              new XAttribute("Action", Action));

            if (Part.HasValue)
                element.Add(new XAttribute("Part", Part.Value));

            if (Value != null)
                element.Add(new XAttribute("Value", Value));

            if (System.HasValue)
                element.Add(new XAttribute("System", System.Value.ToYesNo()));

            if (Permanent.HasValue)
                element.Add(new XAttribute("Permanent", Permanent.Value.ToYesNo()));

            element.AddAttributes(Attributes);

            return element;
        }
    }
}