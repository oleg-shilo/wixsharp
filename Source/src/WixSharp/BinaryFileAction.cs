using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WixSharp
{

    /// <summary>
    /// Defines WiX BinaryFileAction for executing binary (not installed) file. 
    /// </summary>
    /// 
    /// <example>The following is an example of using <c>BinaryFileAction</c> to run
    /// executable <c>Echo.exe</c> with different arguments depending 
    /// in installation type (install/uninstall):
    /// <code>
    /// var project = 
    ///     new Project("My Product",
    ///         new Binary(new Id("EchoBin"), @"Files\Echo.exe"), 
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         
    ///             new File(binaries, @"AppFiles\MyApp.exe",
    ///                 new WixSharp.Shortcut("MyApp", @"%ProgramMenu%\My Company\My Product"),
    ///                 new WixSharp.Shortcut("MyApp", @"%Desktop%")),
    ///                 
    ///             
    ///         new BinaryFileAction("EchoBin", "/i", 
    ///                                 Return.check, 
    ///                                 When.After, 
    ///                                 Step.InstallFinalize, 
    ///                                 Condition.NOT_Installed),
    ///                                 
    ///         BinaryFileAction("EchoBin", "/u", 
    ///                                 Return.check, 
    ///                                 When.Before, 
    ///                                 Step.InstallFinalize, 
    ///                                 Condition.Installed), 
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public class BinaryFileAction : Action
    {
       
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        public BinaryFileAction(string key, string args)
        {
            Key = key;
            Args = args;
            Name = "Action" + (++count) + "_" + key;
            Step = Step.InstallExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="BinaryFileAction"/> instance.</param>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        public BinaryFileAction(Id id, string key, string args)
            : base(id)
        {
            Key = key; 
            Args = args;
            Name = "Action" + (++count) + "_" + key;
            Step = Step.InstallExecute; 
        }
   
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="BinaryFileAction"/>.</param>
        public BinaryFileAction(string key, string args, Return returnType, When when, Step step, Condition condition)
            : base(returnType, when, step, condition)
        {
            Key = key; 
            Args = args;
            Name = "Action" + (++count) + "_" + key;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="BinaryFileAction"/> instance.</param>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="BinaryFileAction"/>.</param>
        public BinaryFileAction(Id id, string key, string args, Return returnType, When when, Step step, Condition condition)
            : base(id, returnType, when, step, condition)
        {
            Key = key;
            Args = args;
            Name = "Action" + (++count) + "_" + key;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="BinaryFileAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public BinaryFileAction(string key, string args, Return returnType, When when, Step step, Condition condition, Sequence sequence)
            : base(returnType, when, step, condition, sequence)
        {
            Key = key;
            Args = args;
            Name = "Action" + (++count) + "_" + key;
        }
  
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="BinaryFileAction"/> instance.</param>
        /// <param name="key">The key (file name) of the installed file to be executed.</param>
        /// <param name="args">The arguments to be passed to the file during the execution.</param>
        /// <param name="returnType">The return type of the action.</param>
        /// <param name="when"><see cref="T:WixSharp.When"/> the action should be executed with respect to the <paramref name="step"/> parameter.</param>
        /// <param name="step"><see cref="T:WixSharp.Step"/> the action should be executed before/after during the installation.</param>
        /// <param name="condition">The launch condition for the <see cref="BinaryFileAction"/>.</param>
        /// <param name="sequence">The MSI sequence the action belongs to.</param>
        public BinaryFileAction(Id id, string key, string args, Return returnType, When when, Step step, Condition condition, Sequence sequence)
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
