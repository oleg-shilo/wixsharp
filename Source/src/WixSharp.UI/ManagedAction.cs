#region Licence...

//-----------------------------------------------------------------------------
// Date:	10/03/09	Time: 21:00
// Module:	WixSharp.cs
// Version: 0.1.20
//
// This module contains the definition of the Wix# classes.
//
// Written by Oleg Shilo (oshilo@gmail.com)
// Copyright (c) 2008-2017. All rights reserved.
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

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using WixSharp;

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

/// <summary>
/// utility class for generation of wxi reusable fragments.
/// </summary>
public static class WxiBuilder
{
    static string FindVsOutputPath()
    {
        var asm = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var outdir = asm.PathGetDirName();

        // running from the proj dir
        if (outdir.PathCombine("bin").PathExists() && outdir.PathCombine("obj").PathExists())
            return outdir.PathCombine("wix").EnsureDirExists();

        // running from the proj/bin/debug dir
        for (int i = 0; i < 6; i++)
        {
            if (outdir.PathGetFileName() == "bin")
                return outdir.PathGetDirName().PathCombine("wix").EnsureDirExists();

            outdir = outdir.PathGetDirName();
        }

        return null;
    }

    /// <summary>
    /// This method is to be used by user to define Managed UI that is later to be converted into reusable wxi fragment.
    /// </summary>
    /// <param name="initProject">The initialize project.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">project.OutDir is empty</exception>
    public static ManagedProject UI(System.Action<ManagedProject> initProject)
    {
        // Entry assembly may not be the one who is calling the builder so using StackTrace.
        var builderAssembly = new StackTrace(false).GetFrame(1).GetMethod().DeclaringType?.Assembly.Location;

        var projectName = builderAssembly.PathGetFileNameWithoutExtension();

        var project = new ManagedProject(projectName);

        project.PreserveTempFiles = true;

        if (!project.IsOutDirSet)
            project.OutDir = FindVsOutputPath();

        if (!project.IsOutDirSet)
            throw new System.Exception("project.OutDir is empty");

        var wxiPrefix = "custom_ui.";

        project.WixSourceGenerated += doc =>
        {
            // extract
            var children = doc.Root.Select("Package")
                .Descendants()
                .Where(x => x.LocalName().IsOneOf("UI", "CustomAction", "Binary", "Property", "InstallExecuteSequence"))
                .Where(x => x.Attributes().Any() || x.Descendants().Any())
                .ToArray();

            children.ForEach(x => doc.Root.Add(x));

            // rename with "namespace-like" prefix

            doc.Root.FindAll("Binary")
               .Where(x => x.HasAttribute("Id", "WixSharp_InitRuntime_Action_File"))
               .ForEach(x => x.SetAttribute("Id", wxiPrefix + "WixSharp_InitRuntime_Action_File"));

            doc.Root.FindAll("CustomAction").Where(x => x.HasAttribute("BinaryRef", "WixSharp_InitRuntime_Action_File"))
               .ForEach(x => x.SetAttribute("BinaryRef", wxiPrefix + "WixSharp_InitRuntime_Action_File"));

            doc.Root.FindAll("CustomAction").Where(x => x.HasAttribute("Id", "WixSharp_InitRuntime_Action"))
               .ForEach(x => x.SetAttribute("Id", wxiPrefix + "WixSharp_InitRuntime_Action"));

            doc.Root.Descendants().Where(x => x.HasLocalName("Custom") && x.HasAttribute("Action", "WixSharp_InitRuntime_Action"))
               .ForEach(x => x.SetAttribute("Action", wxiPrefix + "WixSharp_InitRuntime_Action"));

            // convert into Include root
            doc.Root.Select("Package").Remove();
            doc.Root.Name = doc.Root.Name.Namespace + "Include";
        };
        Compiler.EmitRelativePaths = false;

        initProject(project);

        string wsiFile = project.OutDir.PathJoin(projectName) + ".wxi";

        var file = project.BuildWxs(path: wsiFile);
        // Compiler.OutputWriteLine(System.IO.File.ReadAllText(file));
        return project;
    }
}