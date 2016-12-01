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
    /// Defines generic WiX CustomAction. 
    /// <para>
    /// This class does not contain any public constructor and is only to be used 
    /// as a base class for deriving specialized CustomActions (e.g. <see cref="ManagedAction"></see>)
    /// or for declaring heterogeneous collections.
    /// </para>
    /// </summary>
    /// <example>The following is an example of initializing an
    /// <c>Action</c> type:
    /// <code>
    /// var project = new Project()
    ///               {
    ///                 Actions = new WixSharp.Action[] 
    ///                 { 
    ///                     new ManagedAction(@"FindSQLServerInstance"),
    ///                     new QtCmdLineAction("notepad.exe", @"C:\boot.ini"))
    ///                  },
    ///                  ...
    /// </code>
    /// </example>
    public partial class Action : WixEntity
    {
        /// <summary>
        /// Global counter which is used as suffix for the automatically generated action name. This counter is used by
        /// Wix# engine to avoid naming collision and indicate <c>CustomActions</c> declaration order.
        /// </summary>
        protected static int count = 0;
        /// <summary>
        /// Default constructor. Creates instance of the <see cref="Action"></see> class.
        /// </summary>
        protected Action()
        {
        }
        /// <summary>
        /// Creates instance of the <see cref="Action"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Action"/> instance.</param>
        protected Action(Id id)
        {
            Id = id.Value;
        }
        /// <summary>
        /// Creates instance of the <see cref="Action"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="Action"/>.</param>
        protected Action(Return returnType, When when, Step step, Condition condition)
        {
            Return = returnType;
            Step = step;
            When = when;
            Condition = condition;
        }
        /// <summary>
        /// Creates instance of the <see cref="Action"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Action"/> instance.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="Action"/>.</param>
        protected Action(Id id, Return returnType, When when, Step step, Condition condition)
        {
            Id = id.Value;
            Return = returnType;
            Step = step;
            When = when;
            Condition = condition;
        }
        /// <summary>
        /// Creates instance of the <see cref="Action"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="Action"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        protected Action(Return returnType, When when, Step step, Condition condition, Sequence sequence)
        {
            Sequence = sequence;
            Return = returnType;
            Step = step;
            When = when;
            Condition = condition;
        }
        /// <summary>
        /// Creates instance of the <see cref="Action"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Action"/> instance.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="Action"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        protected Action(Id id, Return returnType, When when, Step step, Condition condition, Sequence sequence)
        {
            Id = id.Value;
            Sequence = sequence;
            Return = returnType;
            Step = step;
            When = when;
            Condition = condition;
        }
        /// <summary>
        /// Defines <see cref="Execute"/> the action associated with.
        /// </summary>
        public Execute Execute = Execute.immediate;
        /// <summary>
        /// This attribute specifies whether the Windows Installer, which executes as LocalSystem, should impersonate the user context of the
        /// installing user when executing this custom action. Typically the value should be <c>true</c>, except when the custom action needs 
        /// elevated privileges to apply changes to the machine.
        /// </summary>
        public bool? Impersonate;
        /// <summary>
        /// Defines <see cref="Sequence"/> the action belongs to.
        /// </summary>
        public Sequence Sequence = Sequence.InstallExecuteSequence;
        /// <summary>
        /// Defines <see cref="Return"/> type of the action.
        /// </summary>
        public Return Return = Return.asyncNoWait;
        /// <summary>
        /// Defines at what <see cref="Step"/> the action should be executed during the installation.
        /// </summary>
        public Step Step = Step.PreviousActionOrInstallInitialize;
        /// <summary>
        /// Defines order <see cref="When"/> the action should be executed with respect to the action <see cref="Step"/>.
        /// </summary>
        public When When = When.After;
        /// <summary>
        /// Defines the launch <see cref="Condition"/>, which is to be checked during the installation to 
        /// determine if the actions should be executed.
        /// </summary>
        public Condition Condition = Condition.NOT_Installed;
        /// <summary>
        /// The sequence number for this action. Mutually exclusive with Before, After, and OnExit of <see cref="When"/> field.
        /// </summary>
        public int? SequenceNumber;

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
