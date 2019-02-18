using System.Linq;
using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Describes a burn engine variable to define.
    /// </summary>
    public class Variable : WixEntity, IGenericEntity
    {
        /// <summary>
        /// The name for the variable.
        /// </summary>
        [Xml]
        public new string Name;

        /// <summary>
        /// Starting value for the variable.
        /// </summary>
        [Xml]
        public string Value;

        /// <summary>
        /// Whether the value of the variable should be hidden.
        /// </summary>
        [Xml]
        public bool? Hidden;

        /// <summary>
        /// Whether the variable should be persisted.
        /// </summary>
        [Xml]
        public bool Persisted = true;

        /// <summary>
        /// Type of the variable, inferred from the value if not specified.
        /// </summary>
        [Xml]
        public VariableType? Type;

        /// <summary>
        /// When set to "yes", lets the user override the variable's default value by specifying another value on the
        /// command line, in the form Variable=Value. Otherwise, WixStdBA won't overwrite the default value and will
        /// log "Ignoring attempt to set non-overridable variable: 'BAR'." (http://schemas.microsoft.com/wix/BalExtension)
        /// </summary>
        [Xml(Namespace = WixExtension.BalNamespace)]
        public bool? Overridable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Variable(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hidden">if set to <c>true</c> [hidden].</param>
        public Variable(string name, bool hidden)
        {
            Name = name;
            Hidden = hidden;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hidden">if set to <c>true</c> [hidden].</param>
        /// <param name="persisted">if set to <c>true</c> [persisted].</param>
        public Variable(string name, bool hidden, bool persisted)
        {
            Name = name;
            Hidden = hidden;
            Persisted = persisted;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hidden">if set to <c>true</c> [hidden].</param>
        /// <param name="persisted">if set to <c>true</c> [persisted].</param>
        /// <param name="type">The type.</param>
        public Variable(string name, bool hidden, bool persisted, VariableType type)
        {
            Name = name;
            Hidden = hidden;
            Persisted = persisted;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public Variable(string name, VariableType type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public Variable(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="hidden">if set to <c>true</c> [hidden].</param>
        public Variable(string name, string value, bool hidden)
        {
            Name = name;
            Value = value;
            Hidden = hidden;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="hidden">if set to <c>true</c> [hidden].</param>
        /// <param name="persisted">if set to <c>true</c> [persisted].</param>
        public Variable(string name, string value, bool hidden, bool persisted)
        {
            Name = name;
            Value = value;
            Hidden = hidden;
            Persisted = persisted;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="hidden">if set to <c>true</c> [hidden].</param>
        /// <param name="persisted">if set to <c>true</c> [persisted].</param>
        /// <param name="type">The type.</param>
        public Variable(string name, string value, bool hidden, bool persisted, VariableType type)
        {
            Name = name;
            Value = value;
            Hidden = hidden;
            Persisted = persisted;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public Variable(string name, string value, VariableType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Bal);
            context.XParent.Add(this.ToXElement("Variable"));
        }
    }

    /// <summary>
    /// Extension method class for working with <see cref="Variable"/>.
    /// </summary>
    public static class Variables
    {
        /// <summary>
        /// Converts key/value map into array of <see cref="Variable"/>. The map entry format
        /// is as follows: &lt;key&gt;=&lt;value&gt;[;&lt;key&gt;=&lt;value&gt;].
        /// </summary>
        /// <param name="variablesDefinition">The variables definition.</param>
        /// <returns></returns>
        public static Variable[] ToStringVariables(this string variablesDefinition)
        {
            return variablesDefinition.ToDictionary()
                                      .Select(entry => new Variable(entry.Key, entry.Value))
                                      .ToArray();
        }
    }
}