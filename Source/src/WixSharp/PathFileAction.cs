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

using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Defines WiX CustomAction for executing file specified by the path on the target system. 
    /// </summary>
    /// <example>The following is an example of using <c>PathFileAction</c> to run
    /// executable present on the target system (<c>Notepad.exe</c>):
    /// <code>
    /// var project = 
    ///     new Project("My Product",
    ///     
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         
    ///             new File(@"AppFiles\MyApp.exe",
    ///                 new WixSharp.Shortcut("MyApp", @"%ProgramMenu%\My Company\My Product"),
    ///                 new WixSharp.Shortcut("MyApp", @"%Desktop%")),
    ///                 
    ///             new File(@"AppFiles\Readme.txt"),
    ///             
    ///        new PathFileAction(@"%WindowsFolder%\notepad.exe", 
    ///                            "Readme.txt", 
    ///                            @"%ProgramFiles%\My Company\My Product", 
    ///                            Return.asyncNoWait, 
    ///                            When.After, 
    ///                            Step.InstallFinalize, 
    ///                            Condition.NOT_Installed), 
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class PathFileAction : Action
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the file to be executed on the target system.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="workingDir">Working directory for the file execution.</param>
        public PathFileAction(string appPath, string args, string workingDir)
            : base()
        {
            AppPath = appPath;
            Args = args;
            WorkingDir = workingDir;
            Name = "Action" + (++count) + "_" + IO.Path.GetFileName(appPath);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="PathFileAction"/> instance.</param>
        /// <param name="appPath">Path to the file to be executed on the target system.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="workingDir">Working directory for the file execution.</param>
        public PathFileAction(Id id, string appPath, string args, string workingDir)
            : base(id)
        {
            AppPath = appPath;
            Args = args;
            WorkingDir = workingDir;
            Name = "Action" + (++count) + "_" + IO.Path.GetFileName(appPath);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the file to be executed on the target system.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="workingDir">Working directory for the file execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="PathFileAction"/>.</param>
        public PathFileAction(string appPath, string args, string workingDir, Return returnType, When when, Step step, Condition condition)
            : base(returnType, when, step, condition)
        {
            AppPath = appPath;
            Args = args;
            WorkingDir = workingDir;
            Name = "Action" + (++count) + "_" + IO.Path.GetFileName(appPath);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="PathFileAction"/> instance.</param>
        /// <param name="appPath">Path to the file to be executed on the target system.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="workingDir">Working directory for the file execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="PathFileAction"/>.</param>
        public PathFileAction(Id id, string appPath, string args, string workingDir, Return returnType, When when, Step step, Condition condition)
            : base(id, returnType, when, step, condition)
        {
            AppPath = appPath;
            Args = args;
            WorkingDir = workingDir;
            Name = "Action" + (++count) + "_" + IO.Path.GetFileName(appPath);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="appPath">Path to the file to be executed on the target system.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="workingDir">Working directory for the file execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="PathFileAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public PathFileAction(string appPath, string args, string workingDir, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(returnType, when, step, condition, sequence)
        {
            AppPath = appPath;
            Args = args;
            WorkingDir = workingDir;
            Name = "Action" + (++count) + "_" + IO.Path.GetFileName(appPath);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PathFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="PathFileAction"/> instance.</param>
        /// <param name="appPath">Path to the file to be executed on the target system.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="workingDir">Working directory for the file execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="PathFileAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public PathFileAction(Id id, string appPath, string args, string workingDir, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(id, returnType, when, step, condition, sequence)
        {
            AppPath = appPath;
            Args = args;
            WorkingDir = workingDir;
            Name = "Action" + (++count) + "_" + IO.Path.GetFileName(appPath);
        }
        /// <summary>
        /// Working directory for the file execution.
        /// </summary>
        public string WorkingDir = "";
        /// <summary>
        /// Path to the file to be executed on the target system.
        /// </summary>
        public string AppPath = "";
        /// <summary>
        /// The arguments to be passed to the file during the execution.
        /// </summary>
        public string Args = "";
    }
}
