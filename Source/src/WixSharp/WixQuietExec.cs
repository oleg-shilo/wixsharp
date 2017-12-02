#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2016 Oleg Shilo

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

using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Defines WiX <c>QtExecCmdLineAction</c> CustomAction.
    /// <para>
    /// This class is loseley mapped to the <c>WixQuietExec</c> WiX element. WixQuietExec superseeds older CAQuietExec as well as fixes a few
    /// runtime artefacts WiX CAQuietExec was associated with.
    /// </para>
    /// <para><see cref="WixQuietExecAction"/> executes specified application with optional arguments.
    /// You do not have to specify full path to the application to be executed as long as its directory
    /// is well-known (e.g. listed in system environment variable <c>PATH</c>) on the target system.</para>
    /// <remarks>
    /// <see cref="WixQuietExecAction"/> often needs to be executed with the elevated privileges. Thus after instantiation it will have
    /// <see cref="Action.Impersonate"/> set to <c>false</c> and <see cref="Action.Execute"/> set to <c>Execute.deferred</c> to allow elevating.
    /// </remarks>
    /// </summary>
    ///
    /// <example>The following is a complete setup script defining <see cref="WixQuietExecAction"/> for
    /// opening <c>bool.ini</c> file in <c>Notepad.exe</c>:
    /// <code>
    ///static public void Main(string[] args)
    ///{
    ///    var project = new Project()
    ///    {
    ///        UI = WUI.WixUI_ProgressOnly,
    ///        Name = "CustomActionTest",
    ///        Actions = new[] { new WixQuietExecAction("notepad.exe", @"C:\boot.ini") },
    ///     };
    ///
    ///     Compiler.BuildMsi(project);
    /// }
    /// </code>
    /// </example>
    public partial class WixQuietExecAction : Action
    {
        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class.
        /// </summary>
        public WixQuietExecAction()
        {
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        public WixQuietExecAction(string appPath, string args)
            : base()
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
            Return = Return.check;
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="rollback">Path to the application to be executed on rollback. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="rollbackArg">The arguments to be passed to the application during the execution on rollback.</param>
        public WixQuietExecAction(string appPath, string args, string rollback, string rollbackArg)
            : base()
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
            Rollback = rollback;
            RollbackArg = rollbackArg;
            Return = Return.check;
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="WixQuietExecAction"/> instance.</param>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        public WixQuietExecAction(Id id, string appPath, string args)
            : base(id)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
            Return = Return.check;
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="WixQuietExecAction"/> instance.</param>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="rollback">Path to the application to be executed on rollback. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="rollbackArg">The arguments to be passed to the application during the execution on rollback.</param>
        public WixQuietExecAction(Id id, string appPath, string args, string rollback, string rollbackArg)
            : base(id)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
            Rollback = rollback;
            RollbackArg = rollbackArg;
            Return = Return.check;
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="WixQuietExecAction"/>.</param>
        public WixQuietExecAction(string appPath, string args, Return returnType, When when, Step step, Condition condition)
            : base(returnType, when, step, condition)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="WixQuietExecAction"/>.</param>
        /// <param name="rollback">Path to the application to be executed on rollback. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="rollbackArg">The arguments to be passed to the application during the execution on rollback.</param>
        public WixQuietExecAction(string appPath, string args, Return returnType, When when, Step step, Condition condition, string rollback, string rollbackArg)
            : base(returnType, when, step, condition)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
            Rollback = rollback;
            RollbackArg = rollbackArg;
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="WixQuietExecAction"/> instance.</param>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="WixQuietExecAction"/>.</param>
        public WixQuietExecAction(Id id, string appPath, string args, Return returnType, When when, Step step, Condition condition)
            : base(id, returnType, when, step, condition)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="WixQuietExecAction"/> instance.</param>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="WixQuietExecAction"/>.</param>
        /// <param name="rollback">Path to the application to be executed on rollback. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="rollbackArg">The arguments to be passed to the application during the execution on rollback.</param>
        public WixQuietExecAction(Id id, string appPath, string args, Return returnType, When when, Step step, Condition condition, string rollback, string rollbackArg)
            : base(id, returnType, when, step, condition)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
            Rollback = rollback;
            RollbackArg = rollbackArg;
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="WixQuietExecAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public WixQuietExecAction(string appPath, string args, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(returnType, when, step, condition, sequence)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="WixQuietExecAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        /// <param name="rollback">Path to the application to be executed on rollback. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="rollbackArg">The arguments to be passed to the application during the execution on rollback.</param>
        public WixQuietExecAction(string appPath, string args, Return returnType, When when, Step step, Condition condition, Sequence sequence, string rollback, string rollbackArg)
            : base(returnType, when, step, condition, sequence)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
            Rollback = rollback;
            RollbackArg = rollbackArg;
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="WixQuietExecAction"/> instance.</param>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="WixQuietExecAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public WixQuietExecAction(Id id, string appPath, string args, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(id, returnType, when, step, condition, sequence)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
        }

        /// <summary>
        /// Executes a new instance of the <see cref="WixQuietExecAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="WixQuietExecAction"/> instance.</param>
        /// <param name="appPath">Path to the application to be executed. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="args">The arguments to be passed to the application during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="WixQuietExecAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        /// <param name="rollback">Path to the application to be executed on rollback. This can be a file name only if the location of the application is well-known.</param>
        /// <param name="rollbackArg">The arguments to be passed to the application during the execution on rollback.</param>
        public WixQuietExecAction(Id id, string appPath, string args, Return returnType, When when, Step step, Condition condition, Sequence sequence, string rollback, string rollbackArg)
            : base(id, returnType, when, step, condition, sequence)
        {
            AppPath = appPath;
            Args = args;
            Name = "WixQuietExec_" + IO.Path.GetFileName(appPath);
            Rollback = rollback;
            RollbackArg = rollbackArg;
        }

        /// <summary>
        /// Path to the application to be executed. This can be a file name only if the location of the application is well-known.
        /// </summary>
        public string AppPath = "";

        /// <summary>
        /// The arguments to be passed to the application during the execution.
        /// </summary>
        public string Args = "";

        /// <summary>
        /// WixQuietExecCmdLine or QtExecCmdLine
        /// </summary>
        public string CommandLineProperty = "WixQuietExecCmdLine";

        /// <summary>
        /// WixQuietExec or CAQuietExec
        /// </summary>
        public string ActionName = "WixQuietExec";
    }
}