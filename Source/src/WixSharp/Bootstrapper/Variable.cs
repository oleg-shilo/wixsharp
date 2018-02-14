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

        public Variable(string name)
        {
            Name = name;
        }

        public Variable(string name, bool hidden)
        {
            Name = name;
            Hidden = hidden;
        }

        public Variable(string name, bool hidden, bool persisted)
        {
            Name = name;
            Hidden = hidden;
            Persisted = persisted;
        }

        public Variable(string name, bool hidden, bool persisted, VariableType type)
        {
            Name = name;
            Hidden = hidden;
            Persisted = persisted;
            Type = type;
        }

        public Variable(string name, VariableType type)
        {
            Name = name;
            Type = type;
        }

        public Variable(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public Variable(string name, string value, bool hidden)
        {
            Name = name;
            Value = value;
            Hidden = hidden;
        }

        public Variable(string name, string value, bool hidden, bool persisted)
        {
            Name = name;
            Value = value;
            Hidden = hidden;
            Persisted = persisted;
        }

        public Variable(string name, string value, bool hidden, bool persisted, VariableType type)
        {
            Name = name;
            Value = value;
            Hidden = hidden;
            Persisted = persisted;
            Type = type;
        }

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
            context.XParent.Add(this.ToXElement("Variable"));
        }
    }
}
