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
    /// Defines WiX InstalledFileAction for executing installed file. 
    /// </summary>
    /// 
    /// <example>The following is an example of using <c>InstalledFileAction</c> to run
    /// installed executable <c>Registrator.exe</c> with different arguments depending 
    /// in installation type (install/uninstall):
    /// <code>
    /// var project = 
    ///     new Project("My Product",
    ///     
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         
    ///             new File(binaries, @"AppFiles\MyApp.exe",
    ///                 new WixSharp.Shortcut("MyApp", @"%ProgramMenu%\My Company\My Product"),
    ///                 new WixSharp.Shortcut("MyApp", @"%Desktop%")),
    ///                 
    ///             new File(binaries, @"AppFiles\Registrator.exe"),
    ///             
    ///         new InstalledFileAction("Registrator.exe", "", 
    ///                                 Return.check, 
    ///                                 When.After, 
    ///                                 Step.InstallFinalize, 
    ///                                 Condition.NOT_Installed),
    ///                                 
    ///         new InstalledFileAction("Registrator.exe", "/u", 
    ///                                 Return.check, 
    ///                                 When.Before, 
    ///                                 Step.InstallFinalize, 
    ///                                 Condition.Installed), 
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class InstalledFileAction : Action
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        public InstalledFileAction(string key, string args)
            : base()
        {
            Key = key;
            Args = args;
            Name = "Action" + (++count) + "_" + key;
            Step = Step.InstallExecute;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="InstalledFileAction"/> instance.</param>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        public InstalledFileAction(Id id, string key, string args)
            : base(id)
        {
            Key = key; 
            Args = args;
            Name = "Action" + (++count) + "_" + key;
            Step = Step.InstallExecute; 
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="InstalledFileAction"/>.</param>
        public InstalledFileAction(string key, string args, Return returnType, When when, Step step, Condition condition)
            : base(returnType, when, step, condition)
        {
            Key = key; 
            Args = args;
            Name = "Action" + (++count) + "_" + key;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="InstalledFileAction"/> instance.</param>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="InstalledFileAction"/>.</param>
        public InstalledFileAction(Id id, string key, string args, Return returnType, When when, Step step, Condition condition)
            : base(id, returnType, when, step, condition)
        {
            Key = key;
            Args = args;
            Name = "Action" + (++count) + "_" + key;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="InstalledFileAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public InstalledFileAction(string key, string args, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(returnType, when, step, condition, sequence)
        {
            Key = key;
            Args = args;
            Name = "Action" + (++count) + "_" + key;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="InstalledFileAction"/> instance.</param>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="InstalledFileAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public InstalledFileAction(Id id, string key, string args, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(id, returnType, when, step, condition, sequence)
        {
            Key = key;
            Args = args;
            Name = "Action" + (++count) + "_" + key;
        }

        /// <summary>
        /// The key (file name) of the installed file to be executed.
        /// </summary>
        public string Key = "";
        /// <summary>
        /// The arguments to be passed to the file during the execution.
        /// </summary>
        public string Args = "";
    }
}
