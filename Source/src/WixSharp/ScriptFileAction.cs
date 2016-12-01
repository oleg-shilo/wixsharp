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
    /// Defines WiX CustomAction for executing embedded VBScript file. 
    /// </summary>
    /// 
    /// <example>The following is an example of using <c>ScriptFileAction</c> to run
    /// embedded VBScript file (from <c>CustomActions\Sample.vbs</c>):
    /// <code>
    /// var project = 
    ///     new Project("My Product",
    ///     
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File(binaries, @"AppFiles\MyApp.exe",
    ///                 new WixSharp.Shortcut("MyApp", @"%ProgramMenu%\My Company\My Product"),
    ///                 new WixSharp.Shortcut("MyApp", @"%Desktop%")),
    ///                 
    ///         new ScriptAction(@"CustomActions\Sample.vbs", "Execute", 
    ///                          Return.ignore, 
    ///                          When.After, 
    ///                          Step.InstallFinalize, 
    ///                          Condition.NOT_Installed),
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class ScriptFileAction : Action
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="file">VBScript file to be executed.</param>
        /// <param name="procedure">Name of the procedure (from the <paramref name="file"/>) to be executed.</param>
        public ScriptFileAction(string file, string procedure)
            : base()
        {
            ScriptFile = file;
            Procedure = procedure;
            Name = "Action" + (++count) + "_VBScriptFile";
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ScriptFileAction"/> instance.</param>
        /// <param name="file">VBScript file to be executed.</param>
        /// <param name="procedure">Name of the procedure (from the <paramref name="file"/>) to be executed.</param>
        public ScriptFileAction(Id id, string file, string procedure)
            : base(id)
        {
            ScriptFile = file;
            Procedure = procedure;
            Name = "Action" + (++count) + "_VBScriptFile";
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="file">VBScript file to be executed.</param>
        /// <param name="procedure">Name of the procedure (from the <paramref name="file"/>) to be executed.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ScriptFileAction"/>.</param>
        public ScriptFileAction(string file, string procedure, Return returnType, When when, Step step, Condition condition)
            : base(returnType, when, step, condition)
        {
            ScriptFile = file;
            Procedure = procedure;
            Name = "Action" + (++count) + "_VBScriptFile";
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ScriptFileAction"/> instance.</param>
        /// <param name="file">VBScript file to be executed.</param>
        /// <param name="procedure">Name of the procedure (from the <paramref name="file"/>) to be executed.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ScriptFileAction"/>.</param>
        public ScriptFileAction(Id id, string file, string procedure, Return returnType, When when, Step step, Condition condition)
            : base(id, returnType, when, step, condition)
        {
            ScriptFile = file;
            Procedure = procedure;
            Name = "Action" + (++count) + "_VBScriptFile";
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="file">VBScript file to be executed.</param>
        /// <param name="procedure">Name of the procedure (from the <paramref name="file"/>) to be executed.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ScriptFileAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public ScriptFileAction(string file, string procedure, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(returnType, when, step, condition, sequence)
        {
            ScriptFile = file;
            Procedure = procedure;
            Name = "Action" + (++count) + "_VBScriptFile";
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ScriptFileAction"/> instance.</param>
        /// <param name="file">VBScript file to be executed.</param>
        /// <param name="procedure">Name of the procedure (from the <paramref name="file"/>) to be executed.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="ScriptFileAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public ScriptFileAction(Id id, string file, string procedure, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(id, returnType, when, step, condition, sequence)
        {
            ScriptFile = file;
            Procedure = procedure;
            Name = "Action" + (++count) + "_VBScriptFile";
        }

        /// <summary>
        /// VBScript file to be executed.
        /// </summary>
        public string ScriptFile = "";
        /// <summary>
        /// Name of the procedure (from the <see cref="ScriptFileAction.ScriptFile"/>) to be executed.
        /// </summary>
        public string Procedure = "";
    }
}
