#region Licence...

//-----------------------------------------------------------------------------
// Date:	10/03/09	Time: 21:00
// Module:	WixSharp.cs
// Version: 0.1.20
//
// This module contains the definition of the Wix# classes.
//
// Written by Oleg Shilo (oshilo@gmail.com)
// Copyright (c) 2008-2013. All rights reserved.
//
// Redistribution and use of this code in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright notice,
//	this list of conditions and the following disclaimer.
// 2. Neither the name of an author nor the names of the contributors may be used
//	to endorse or promote products derived from this software without specific
//	prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//	Caution: Bugs are expected!
//----------------------------------------------

#endregion Licence...

namespace WixSharp
{
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
    ///                 new ManagedAction(@"MyManagedAction"),
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
    public partial class ShowClrDialogAction : ManagedAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowClrDialogAction"/> class.
        /// </summary>
        public ShowClrDialogAction()
        {
            this.Sequence = Sequence.NotInSequence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowClrDialogAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">Name of the CustomAction. The name should match the method implementing the custom action functionality.</param>
        public ShowClrDialogAction(string name)
            : base(name)
        {
            this.Sequence = Sequence.NotInSequence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowClrDialogAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ShowClrDialogAction"/> instance.</param>
        /// <param name="name">Name of the CustomAction. The name should match the method implementing the custom action functionality.</param>
        public ShowClrDialogAction(Id id, string name)
            : base(name)
        {
            Id = id;
            this.Sequence = Sequence.NotInSequence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowClrDialogAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">Name of the CustomAction. The name should match the method implementing the custom action functionality.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        public ShowClrDialogAction(string name, string actionAssembly)
            : base(name)
        {
            this.ActionAssembly = actionAssembly;
            this.Sequence = Sequence.NotInSequence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowClrDialogAction"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="ShowClrDialogAction"/> instance.</param>
        /// <param name="name">Name of the CustomAction. The name should match the method implementing the custom action functionality.</param>
        /// <param name="actionAssembly">Path to the assembly containing the CustomAction implementation. Specify <c>"%this%"</c> if the assembly
        /// is in the Wix# script.</param>
        public ShowClrDialogAction(Id id, string name, string actionAssembly)
            : base(name)
        {
            this.Id = id;
            this.ActionAssembly = actionAssembly;
            this.Sequence = Sequence.NotInSequence;
        }
    }
}