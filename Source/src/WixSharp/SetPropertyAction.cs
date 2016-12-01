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
    /// Defines WiX CustomAction for setting MSI property. 
    /// </summary>
    /// 
    /// <example>The following is an example of using <c>SetPropertyAction</c> for assigning
    /// string value <c>"Hello World!"</c> to property <c>Gritting</c>:
    /// <code>
    /// var project = 
    ///     new Project("My Product",
    ///     
    ///         new Property("Gritting", "empty"),        
    ///         new SetPropertyAction("Gritting", "Hello World!"),
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class SetPropertyAction : Action
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyAction"/> class.
        /// </summary>
        public SetPropertyAction()
        {
            Return = Return.check;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">Name of the property to be assigned.</param>
        /// <param name="value">Value to be set to the property.</param>
        public SetPropertyAction(string name, string value)
        {
            PropName = name;
            Value = value;
            Return = Return.check;
            Name = "Action" + (++count) + "SetProp_" + name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="SetPropertyAction"/> instance.</param>
        /// <param name="name">Name of the property to be assigned.</param>
        /// <param name="value">Value to be set to the property.</param>
        public SetPropertyAction(Id id, string name, string value)
            : base(id)
        {
            PropName = name;
            Value = value;
            Return = Return.check;
            Name = "Action" + (++count) + "SetProp_" + name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">Name of the property to be assigned.</param>
        /// <param name="value">Value to be set to the property.</param>
        /// <param name="returnType">The return type of the action.</param>
        public SetPropertyAction(string name, string value, Return returnType)
        {
            PropName = name;
            Value = value;
            Return = returnType;
            Name = "Action" + (++count) + "SetProp_" + name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="SetPropertyAction"/> instance.</param>
        /// <param name="name">Name of the property to be assigned.</param>
        /// <param name="value">Value to be set to the property.</param>
        /// <param name="returnType">The return type of the action.</param>
        public SetPropertyAction(Id id, string name, string value, Return returnType)
            : base(id)
        {
            PropName = name;
            Value = value;
            Return = returnType;
            Name = "Action" + (++count) + "SetProp_" + name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">Name of the property to be assigned.</param>
        /// <param name="value">Value to be set to the property.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="SetPropertyAction"/>.</param>
        public SetPropertyAction(string name, string value, Return returnType, When when, Step step, Condition condition)
            : base(returnType, when, step, condition)
        {
            PropName = name;
            Value = value;
            Name = "Action" + (++count) + "SetProp_" + name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="SetPropertyAction"/> instance.</param>
        /// <param name="name">Name of the property to be assigned.</param>
        /// <param name="value">Value to be set to the property.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="SetPropertyAction"/>.</param>
        public SetPropertyAction(Id id, string name, string value, Return returnType, When when, Step step, Condition condition)
            : base(id, returnType, when, step, condition)
        {
            PropName = name;
            Value = value;
            Name = "Action" + (++count) + "SetProp_" + name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertyAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="SetPropertyAction"/> instance.</param>
        /// <param name="name">Name of the property to be assigned.</param>
        /// <param name="value">Value to be set to the property.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="SetPropertyAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public SetPropertyAction(Id id, string name, string value, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(id, returnType, when, step, condition, sequence)
        {
            PropName = name;
            Value = value;
            Name = "Action" + (++count) + "SetProp_" + name;
        }

        /// <summary>
        /// Value to set the property to.
        /// </summary>
        public string Value = "";
        /// <summary>
        /// Name of the property to be set.
        /// </summary>
        public string PropName = "";
    }
}
