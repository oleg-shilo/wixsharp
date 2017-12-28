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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Deployment.WindowsInstaller;

namespace WixSharp
{
    /// <summary>
    /// Method delegate implementing WiX CustomAction.
    /// </summary>
    public delegate ActionResult CustomActionMethod(Session session);

    /// <summary>
    /// Defines WiX Managed CustomAction.
    /// <para>
    /// Managed CustomAction can be defined either in the Wix# script or in the external assembly or C# file.
    /// The only requirements for any C# method to be qualified for being Managed CustomAcyion is to
    /// have DTF Action signature <c>public static ActionResult MyManagedAction(Session session)</c>, and be
    /// marked with <c>[CustomAction]</c> attribute.
    /// </para>
    /// <para>
    /// If Managed CustomAction depends on any assembly, which will not be registered with GAC on the
    /// target system such assembly needs to be listed in the <see cref="ManagedAction.RefAssemblies"/>.
    /// </para>
    /// <remarks>
    /// <see cref="ManagedAction"/> often needs to be executed with the elevated privileges. Thus after instantiation it will have
    /// <see cref="Action.Impersonate"/> set to <c>false</c> and <see cref="Action.Execute"/> set to <c>Execute.deferred</c> to allow elevating.
    /// </remarks>
    /// </summary>
    /// <example>The following is an example of using <c>MyManagedAction</c> method of the class
    /// <c>CustomActions</c> as a Managed CustomAction.
    /// <code>
    /// class Script
    /// {
    ///     static public void Main(string[] args)
    ///     {
    ///         var project =
    ///             new Project("My Product",
    ///
    ///                 new Dir(@"%ProgramFiles%\My Company\My Product",
    ///
    ///                     new File(@"AppFiles\MyApp.exe",
    ///                         new WixSharp.Shortcut("MyApp", @"%ProgramMenu%\My Company\My Product"),
    ///                         new WixSharp.Shortcut("MyApp", @"%Desktop%")),
    ///
    ///                     new File(@"AppFiles\Readme.txt"),
    ///
    ///                 new ManagedAction(CustomActions.MyManagedAction),
    ///
    ///                 ...
    ///
    ///         Compiler.BuildMsi(project);
    ///     }
    /// }
    ///
    /// public class CustomActions
    /// {
    ///     [CustomAction]
    ///     public static ActionResult MyManagedAction(Session session)
    ///     {
    ///         MessageBox.Show("Hello World!", "Managed CA");
    ///         return ActionResult.Success;
    ///     }
    /// }
    /// </code>
    /// </example>
    public partial class ManagedAction : Action
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class.
        /// </summary>
        public ManagedAction()
        {
            Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">Name of the CustomAction. The name should match the method implementing the custom action functionality.</param>
        public ManagedAction(string name)
            : base()
        {
            Name = name;
            MethodName = name;
            Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">Name of the CustomAction. The name should match the method implementing the custom action functionality.</param>
        /// <param name="rollback">Name of the Rollback CustomAction. The name should match the method implementing the custom action functionality</param>
        public ManagedAction(string name, string rollback)
            : base()
        {
            Name = name;
            MethodName = name;
            Rollback = rollback;
            Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        public ManagedAction(CustomActionMethod action)
            : base()
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(CustomActionMethod action, CustomActionMethod rollback)
            : base()
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
            Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        public ManagedAction(Id id, CustomActionMethod action)
            : base(id)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(Id id, CustomActionMethod action, CustomActionMethod rollback)
            : base(id)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
            Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        public ManagedAction(CustomActionMethod action, string actionAssembly)
            : base()
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            ActionAssembly = actionAssembly;
            base.Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(CustomActionMethod action, string actionAssembly, CustomActionMethod rollback)
            : base()
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
            ActionAssembly = actionAssembly;
            base.Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        public ManagedAction(Id id, CustomActionMethod action, string actionAssembly)
            : base(id)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            ActionAssembly = actionAssembly;
            base.Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(Id id, CustomActionMethod action, string actionAssembly, CustomActionMethod rollback)
            : base(id)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
            ActionAssembly = actionAssembly;
            base.Return = Return.check;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        public ManagedAction(CustomActionMethod action, Return returnType, When when, Step step, Condition condition)
            : base(returnType, when, step, condition)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(CustomActionMethod action, Return returnType, When when, Step step, Condition condition, CustomActionMethod rollback)
            : base(returnType, when, step, condition)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        public ManagedAction(CustomActionMethod action, string actionAssembly, Return returnType, When when, Step step, Condition condition)
            : base(returnType, when, step, condition)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            ActionAssembly = actionAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(CustomActionMethod action, string actionAssembly, Return returnType, When when, Step step, Condition condition, CustomActionMethod rollback)
            : base(returnType, when, step, condition)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
            ActionAssembly = actionAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        public ManagedAction(Id id, CustomActionMethod action, Return returnType, When when, Step step, Condition condition)
            : base(id, returnType, when, step, condition)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(Id id, CustomActionMethod action, Return returnType, When when, Step step, Condition condition, CustomActionMethod rollback)
            : base(id, returnType, when, step, condition)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        public ManagedAction(Id id, CustomActionMethod action, string actionAssembly, Return returnType, When when, Step step, Condition condition)
            : base(id, returnType, when, step, condition)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            ActionAssembly = actionAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(Id id, CustomActionMethod action, string actionAssembly, Return returnType, When when, Step step, Condition condition, CustomActionMethod rollback)
            : base(id, returnType, when, step, condition)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
            ActionAssembly = actionAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public ManagedAction(CustomActionMethod action, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(returnType, when, step, condition, sequence)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(CustomActionMethod action, Return returnType, When when, Step step, Condition condition, Sequence sequence, CustomActionMethod rollback)
            : base(returnType, when, step, condition, sequence)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public ManagedAction(CustomActionMethod action, string actionAssembly, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(returnType, when, step, condition, sequence)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            ActionAssembly = actionAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(CustomActionMethod action, string actionAssembly, Return returnType, When when, Step step, Condition condition, Sequence sequence, CustomActionMethod rollback)
            : base(returnType, when, step, condition, sequence)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
            ActionAssembly = actionAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public ManagedAction(Id id, CustomActionMethod action, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(id, returnType, when, step, condition, sequence)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(Id id, CustomActionMethod action, Return returnType, When when, Step step, Condition condition, Sequence sequence, CustomActionMethod rollback)
            : base(id, returnType, when, step, condition, sequence)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public ManagedAction(Id id, CustomActionMethod action, string actionAssembly, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(id, returnType, when, step, condition, sequence)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            ActionAssembly = actionAssembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ManagedAction"/> instance.</param>
        /// <param name="action">The full name of static CustomAction method.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ManagedAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        /// <param name="rollback">The full name of static CustomAction rollback method.</param>
        public ManagedAction(Id id, CustomActionMethod action, string actionAssembly, Return returnType, When when, Step step, Condition condition, Sequence sequence, CustomActionMethod rollback)
            : base(id, returnType, when, step, condition, sequence)
        {
            string name = action.Method.Name;
            Name = name;
            MethodName = name;
            Rollback = rollback.Method.Name;
            ActionAssembly = actionAssembly;
        }

        /// <summary>
        /// Collection of path strings for dependency assemblies to be included in MSI. <c>RefAssemblies</c> should be used if the Managed CustomAction
        /// depends on any assembly, which will not be registered with GAC on the target system.
        /// </summary>
        public string[] RefAssemblies = new string[0];

        internal int GetRefAssembliesHashCode(IEnumerable<string> defaultAssemblies)
        {
            return RefAssemblies.Concat(defaultAssemblies)
                                .Select(a => System.IO.Path.GetFullPath(a).ToLower())
                                .Distinct()
                                .OrderBy(a => a)
                                .GetItemsHashCode();
        }

        /// <summary>
        /// Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.
        /// <para>Default value is <c>%this%</c>.</para>
        /// </summary>
        public string ActionAssembly = "%this%";

        /// <summary>
        /// Name of the method implementing the Managed CustomAction action functionality.
        /// </summary>
        public string MethodName = "";

        /// <summary>
        /// Comma separated list of properties which the custom action is intended to use. Set this property if you are implementing the 'deferred' (as well as 'rollback'  and 'commit')  action.
        /// <remarks>
        /// <para>Deferred custom actions cannot access any session property as the session is terminated at the time of the action execution (limitation of MSI).
        /// The standard way of overcoming this limitation is to create a new custom action for setting the property, set the property name to the name of the deferred action,
        /// set the property value to the specially formatted map, schedule the execution of the custom action and access the mapped properties only via <see cref="T:Microsoft.Deployment.WindowsInstaller.Session.CustomActionData"/>.
        /// </para>
        /// <para> All this can be done in a single hit with Wix# as it fully automates creation of the all mapping infrastructure.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <example>The following is the example of passing the location of the MyApp.exe file in the deferred managed action.
        /// <code>
        /// var project =
        ///     new Project("My Product",
        ///         new Dir(@"%ProgramFiles%\My Company\My Product",
        ///             new File(@"Files\MyApp.exe"),
        ///             new File(@"Files\MyApp.exe.config")),
        ///         new ElevatedManagedAction("ConfigureProduct", Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed)
        ///         {
        ///             UsesProperties = "INSTALLDIR"
        ///         });
        ///
        /// ...
        ///
        /// [CustomAction]
        /// public static ActionResult ConfigureProduct(Session session)
        /// {
        ///     string configFile = session.Property("INSTALLDIR") + "MyApp.exe.config";
        ///     ...
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>Note that you don't even have to specify <c>UsesProperties = "INSTALLDIR"</c> as by default Wix# always maps INSTALLDIR and
        /// UILevel for all deferred managed actions. The default mapping is controlled by <see cref="ManagedAction.DefaultUsesProperties"/>.</para>
        /// <para>It is also possible to map the 'deferred' properties in the typical WiX way: </para>
        /// <code>UsesProperties = "CONFIG_FILE=[INSTALLDIR]MyApp.exe.config, APP_FILE=[INSTALLDIR]MyApp.exe"</code>
        /// </remarks>
        public string UsesProperties;

        /// <summary>
        /// The default properties mapped for use with the 'deferred' (as well as 'rollback'  and 'commit') custom actions. See <see cref="ManagedAction.UsesProperties"/> for the details.
        /// <para>The default value is "INSTALLDIR,UILevel,ProductCode"</para>
        /// </summary>
        public static string DefaultUsesProperties = "INSTALLDIR,UILevel,ProductCode";

        internal string RollbackExpandAllUsedProperties()
        {
            var allProps = (RollbackArg + "," + DefaultUsesProperties);
            var result = string.Join(";", allProps.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => PrepProperty(x))
                .ToArray());
            return result;
        }

        internal string ExpandAllUsedProperties()
        {
            var allProps = (UsesProperties + "," + DefaultUsesProperties);
            var result = string.Join(";", allProps.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => PrepProperty(x))
                .ToArray());
            return result;
        }

        private string PrepProperty(string x)
        {
            if (x.Contains('=')) //e.g. INSTALLDIR=[INSTALLDIR]
            {
                string[] split = x.Split('=');
                if (split.Length == 2)
                {
                    return string.Format("{0}={1}", split[0].Trim(), split[1].Trim());
                }
                else
                {
                    return x.Trim();
                }
            }
            else
                return string.Format("{0}=[{0}]", x.Trim());
        }
    }
}