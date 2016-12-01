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
#endregion

namespace WixSharp
{
    /// <summary>
    /// Defines reference to the Wix custom property.
    /// <para>
    /// Sometimes it desirable to access WiX properties defined in the extension modules. <see cref="PropertyRef"/> is designed for such cases. You can use it as an ordinary WiX property
    /// but you do not need to set it up (e.g. define initial value) as it is already done in the corresponding extension module.
    /// </para>
    /// <para>
    /// In a way <c>PropertyRef</c> is similar to C++ #include as it makes possible to access some entities defined in the external modules.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <see cref="PropertyRef"/> inherits from <see cref="Property"/> because of their logical relationship and usability pattern, not because of any "Parent/Child" connection.
    /// </remarks>
    ///<example>The following is an example of using <c>NETFRAMEWORK20</c> property defined in the <c>WiXNetFxExtension</c> WiX extension.
    ///<code>
    ///
    /// var project = new Project("Setup",
    ///     new PropertyRef("NETFRAMEWORK20"),
    ///     new ManagedAction("MyAction", Return.check, When.After, Step.InstallInitialize, Condition.NOT_BeingRemoved));
    ///
    /// project.IncludeWixExtension(WixExtension.NetFx);
    ///
    /// Compiler.BuildMsi(project);
    ///
    /// ...
    ///
    /// public class CustomActions
    ///{
    ///    [CustomAction]
    ///    public static ActionResult MyAction(Session session)
    ///    {
    ///        MessageBox.Show(session["NETFRAMEWORK20"], "");
    ///        return ActionResult.Success;
    ///    }
    ///}
    /// </code>
    /// </example>
    public partial class PropertyRef : Property
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRef"/> class.
        /// </summary>
        public PropertyRef()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRef"/> class.
        /// </summary>
        /// <param name="id"><c>Id</c> of the property being referenced.</param>
        public PropertyRef(string id)
        {
            Id = id;
        }

        /// <summary>
        /// <c>Id</c> of the property being referenced.
        /// </summary>
        public new string Id { get; set; }
    }

    /// <summary>
    /// Defines WiX custom property.
    /// <para>
    /// Traditional property usage involves defining the property by instantiating the <see cref="Property"/>
    /// class (in the <see cref="Project"/> structure) and then setting it from CustomAction (e.g. <see cref="SetPropertyAction"/>).
    ///<para>Property value can be analyzed and used in <c>CustomActions</c> or it can be a base for the <see cref="Condition"/>
    ///of item to be installed.</para>
    /// </para>
    /// </summary>
    ///<example>The following is an example of installing <c>MyApp.exe</c> file.
    ///<code>
    /// string installDirId = @"%ProgramFiles%\CustomActionTest".ToDirID();
    ///
    /// var project = new Project()
    /// {
    ///     Dirs = new[] { new Dir(@"%ProgramFiles%\PropertiesTest") },
    ///
    ///     Actions = new Action[]
    ///     {
    ///         new SetPropertyAction("IDIR", "[" + installDirId + "]"),
    ///     },
    ///
    ///     Properties = new[]
    ///     {
    ///         new Property("ALLUSERS", "1"),
    ///         new Property("IDIR", "empty")
    ///     }
    ///
    ///     ...
    ///
    /// [CustomAction]
    /// public static ActionResult MyAction(Session session)
    /// {
    ///     MessageBox.Show(session["IDIR"]);
    ///     return ActionResult.Success;
    /// }
    /// </code>
    /// </example>
    public partial class Property : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        public Property()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public Property(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The initial value of the property.</param>
        public Property(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Property"/> instance.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The initial value of the property.</param>
        public Property(Id id, string name, string value)
        {
            Id = id.Value;
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The initial value of the property.
        /// </summary>
        public string Value = "";


        /// <summary>
        /// The flag indicating if the property needs to be 'preserved' for use with the deferred custom actions.
        /// <para>
        /// Setting this flag to 'true' has the same effect as adding the property name to <see cref="ManagedAction.UsesProperties"/>.
        /// </para>
        /// </summary>
        public bool IsDeferred = false;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
