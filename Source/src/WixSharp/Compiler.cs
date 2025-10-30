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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using WixSharp.Utilities;
using WixToolset.Dtf.WindowsInstaller;
using IO = System.IO;

// I am checking it for null anyway but when compiling AOT the output becomes too noisy
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file 

namespace WixSharp
{
    /// <summary>
    /// This class holds the settings for various WixSharp auto-generation aspects that are not directly related to the 
    /// WXS XML schema. IE enforcement of ComponentId uniqness, ID allocation, etc.
    /// <p>
    /// The detailed information about Wix# auto-generation can be found 
    /// here: http://www.csscript.net/WixSharp/ID_Allocation.html.
    /// </p>
    /// <p> There is also additional configuration settings class that controls pure XML generation aspects: 
    /// <see cref="WixSharp.AutoElements"/>.
    /// </p>
    /// </summary>
    public class AutoGenerationOptions
    {
        /// <summary>
        /// The ID of the root directory, which is automatically assigned to the first directory of the project directories.
        /// <para>
        /// The default value "INSTALLDIR" has special meaning as apart from being a traditional choice of the id for the root
        /// directory it is also used in binding root directory to the textbox of the <c>InstallDirDialog</c>.
        /// </para>
        /// <para>
        /// It is important to have the id as public (upper case). Otherwise WiX doesn't produce correct MSI.
        /// </para>
        /// The auto-assignment can be disabled by setting InstallDirDefaultId to null.
        /// </summary>
        public string InstallDirDefaultId = "INSTALLDIR";


        /// <summary>
        /// Extra parameters that is passed to the `dotnet publish` command when AOT compiling .NET Core custom actions.
        /// <code language="C#">
        /// // ignore the directory build props that are not compatible with AOT compilation
        /// WixSharp.Compiler.AutoGeneration.AotBuildParameters = "-p:ImportDirectoryBuildProps=false";
        /// </code>
        /// </summary>
        public string AotBuildParameters = "";

        /// <summary>
        /// Flag indicating if all system folders (e.g. %ProgramFiles%) should be auto-mapped into their x64 equivalents
        /// when 'project.Platform = Platform.x64'
        /// </summary>
        public bool Map64InstallDirs = true;

        /// <summary>
        /// Forces the WXS to be thread-safe. Default value is <c>false</c>.
        /// <para>
        /// If this field is useful when WixShar accesses some shared resources during WXS generation.
        /// For example: using <see cref="CustomIdAlgorithm"/> that uses project's "target path resolver"
        /// during multiple concurrent builds.
        /// </para>
        /// </summary>
        public bool IsWxsGenerationThreadSafe = false;

        internal object WxsGenerationSynchObject
        {
            get => IsWxsGenerationThreadSafe ? typeof(WixProject) : new object();
        }

        /// <summary>
        /// The custom algorithm for generating WiX elements IDs.
        /// </summary>
        ///<example>The following code demonstrates how to generate File IDs based is on the hash
        ///of the target path of the file being installed.
        ///<code>
        ///WixEntity.CustomIdAlgorithm =
        ///       entity =>
        ///       {
        ///           if (entity is File file)
        ///           {
        ///               var target_path = project.GetTargetPathOf(file);
        ///               var hash = target_path.GetHashCode32();
        ///
        ///               // WiX does not allow '-' char in ID. So need to use `Math.Abs`
        ///               return $"{target_path.PathGetFileName()}_{Math.Abs(hash)}";
        ///           }
        ///
        ///           return null; // pass to default ID generator
        ///       };
        /// </code>
        /// </example>
        public Func<WixEntity, string> CustomIdAlgorithm = null;

        /// <summary>
        /// The legacy default identifier algorithm
        /// </summary>
        public bool LegacyDefaultIdAlgorithm = false;

        /// <summary>
        /// Force Component ID uniqueness.
        /// </summary>
        public bool ForceComponentIdUniqueness = false;

        /// <summary>
        /// The option for adding managed custom action dependencies automatically.
        /// <p> This option is a convenient way to add all the dependencies automatically setting them manually. 
        /// The compiler analyzes the CA assembly dependencies and adds them to the CA binary that is packaged 
        /// with "WixToolset.Dtf.MakeSfxCA.exe". </p>
        /// <p>Note, this method may unnecessarily increase the size of the msi as not all CA dependency assemblies
        /// may be required at runtime (during the installation).</p>
        /// </summary>
        public bool AddManagedCustomActionDependencies = false;

        /// <summary>
        /// Remove media if no files are included
        /// </summary>
        public bool RemoveMediaIfNoFiles = true;

        /// <summary>
        /// The ignore empty directories during wild card resolution
        /// </summary>
        public bool IgnoreWildCardEmptyDirectories = false;

        /// <summary>
        /// Enable validating CA assemblies for all CA methods to be public instance method.
        /// <para>
        /// By default validation is performed in a temporary AppDomain (`CAValidation.InRemoteAppDomain`)
        /// as it is the only way to avoid locking the assembly being validated. Unfortunately
        /// `ReflectionOnlyLoadFrom` is incompatible with this task as it does not allow browsing members
        /// attributes.
        /// </para>
        /// <para>
        /// You may need to disable (`CAValidation.Disabled`) the validation if WixSharp is hosted on the
        /// runtime that does not support AppDomain unloading (e.g. .NET Core)</para>
        /// <para>
        /// If you want to perform validation without the use of temporary AppDomain you can chose the
        /// `CAValidation.InCurrentAppDomain` option. In this case the assembly being validated is loaded in
        /// the current AppDomain but from the memory without locking the assembly file. This option is valid
        /// but not recommended as it may lead to the unpredictable reflection scenarios. </para>
        /// </summary>
        public CAValidation ValidateCAAssemblies = CAValidation.InRemoteAppDomain;

        /// <summary>
        /// The suppress generation of the XML attribute 'id' for <see cref="Bootstrapper.Payload"/> undefined ids
        /// </summary>
        public bool SuppressForBundlePayloadUndefinedIds = true;

        /// <summary>
        /// The template for <see cref="WixSharp.Project.HashedTargetPathIdAlgorithm"/> auto-generated File id.
        /// <para>
        /// The default value is "{file_name}_{dir_hash}".
        /// </para>
        /// </summary>
        public string HashedTargetPathIdAlgorithm_FileIdMask = "{file_name}_{dir_hash}";
    }

    /// <summary>
    /// Settings class for controlling MSBuild integration.
    /// </summary>
    public class MSBuild
    {
        /// <summary>
        /// Indicates whether compiler should emit "&lt;projDir&gt;\wix\&lt;projName&gt;.g.wxs" file.
        /// <para> If set to <c>true</c> the Wix# compiler will also update the project file with the inclusion
        /// of the auto-generated file(s) with "Build Action" set to <c>None</c>.
        /// </para>
        /// </summary>
        static public bool EmitAutoGenFiles
        {
            get
            {
                var suppress_auto_gen = (Environment.GetEnvironmentVariable("WIXSHARP_SuppressMSBuildAutoGenFiles") ?? "false").ToLower();
                return suppress_auto_gen != "true";
            }

            set
            {
                if (value)
                    Environment.SetEnvironmentVariable("WIXSHARP_SuppressMSBuildAutoGenFiles", null);
                else
                    Environment.SetEnvironmentVariable("WIXSHARP_SuppressMSBuildAutoGenFiles", "true");
            }
        }
    }

    /// <summary>
    /// Delegate for  <see cref="Compiler"/> event <c>WixSourceGenerated</c>
    /// </summary>
    public delegate void XDocumentGeneratedDlgt(XDocument document);

    /// <summary>
    /// Delegate for  <see cref="WixProject.WixBuildCommandGenerated"/> event.
    /// </summary>
    public delegate string WixBuildCommandGeneratedDlgt(string command);

    /// <summary>
    /// Delegate for  <see cref="Compiler"/> event <c>WixSourceSaved</c>
    /// </summary>
    public delegate void XDocumentSavedDlgt(string fileName);

    /// <summary>
    /// Delegate for  <see cref="Compiler"/> event <c>WixSourceFormated</c>
    /// </summary>
    public delegate void XDocumentFormatedDlgt(ref string content);

    /// <summary>
    /// Represents Wix# compiler. This class is responsible for conversion of CLR object <see cref="Project"/> into WiX XML source file.
    /// <see cref="Compiler"/> allows building complete MSI or WiX source file. It also can prepare WiX source file and generate corresponding batch file
    /// for building MSI WiX way: <c>candle.exe</c> + <c>light.exe</c>.
    /// <para>
    /// This class contains only static members as it is to be used only for the actual MSI/WXS building operations:
    /// </para>
    /// </summary>
    ///
    /// <example>
    ///
    /// <list type="bullet">
    ///
    /// <item>
    /// <description>Building MSI file
    /// <code>
    /// var project = new Project();
    /// ...
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </description>
    /// </item>
    ///
    /// <item>
    /// <description>Building WiX source file only:
    /// <code>
    /// var project = new Project();
    /// ...
    /// Compiler.BuildWxs(project);
    /// </code>
    /// </description>
    /// </item>
    ///
    /// <item>
    /// <description>Preparing batch file for building MSI with WiX toolset:
    /// <code>
    /// var project = new Project();
    /// ...
    /// Compiler.BuildMsiCmd(project);
    /// </code>
    /// </description>
    /// </item>
    ///
    /// </list>
    ///
    /// </example>
    public partial class Compiler
    {
        /// <summary>
        /// The verbose build output (e.g. wix build command output).
        /// </summary>
        static public bool VerboseOutput;
        /// <summary>
        /// Contains settings for XML auto-generation aspects that are directly related to the 
        /// WXS XML schema. IE enforcement of ComponentId uniqness, ID allocation, etc.
        /// <p>
        /// The detailed information about Wix# auto-generation can be found 
        /// here: http://www.csscript.net/WixSharp/ID_Allocation.html.
        /// </p>
        /// <p> Note, there is also additional configuration settings class that controls pure XML generation aspects: 
        /// <see cref="WixSharp.AutoElements"/>. The reason for two classes is not strong and rather legacy-based. 
        /// The intention is to merge them together in the near future (as of 10-Jul-204).   
        /// </p>
        /// </summary>
        static public AutoGenerationOptions AutoGeneration { get => autoGeneration; }

        static internal AutoGenerationOptions autoGeneration = new AutoGenerationOptions();

        /// <summary>
        /// Occurs when WiX source code generated. Use this event if you need to modify generated XML (XDocument)
        /// before it is compiled into MSI.
        /// </summary>
        static public event XDocumentGeneratedDlgt WixSourceGenerated;

        /// <summary>
        /// Occurs when WiX source file is saved. Use this event if you need to do any post-processing of the generated/saved file.
        /// </summary>
        static public event XDocumentSavedDlgt WixSourceSaved;

        /// <summary>
        /// Occurs when WiX source file is formatted and ready to be saved. Use this event if you need to do any custom formatting of the XML content before
        /// it is saved by the compiler.
        /// </summary>
        static public event XDocumentFormatedDlgt WixSourceFormated;


        /// <summary>
        /// The assembly resolve handler. If it is set then it will be used for all assembly probing (AppDomain.AssemblyResolve event) performed in all
        /// AppDomains.
        /// </summary>
        static public ResolveEventHandler AssemblyResolve;

        /// <summary>
        /// WiX linker <c>Light.exe</c> options (e.g. " -sice:ICE30").
        /// <para>The default value is "-sw1076 -sw1079" (disable warning 1076 and 1079).</para>
        /// </summary>
        [Obsolete("WiX4 (and higher) does not use Candle/Light any more but wix.exe. Thus use WixOptions instead", true)]
        public static string LightOptions;

        /// <summary>
        /// WiX compiler <c>Candle.exe</c> options.
        /// <para>The default value is "-sw1076" (disable warning 1026).</para>
        /// </summary>
        [Obsolete("WiX4 (and higher) does not use Candle/Light any more but wix.exe. Thus use WixOptions instead", true)]
        public static string CandleOptions;

        /// <summary>
        /// The option for suppressing warnings during AOT compilation of .NET Core assemblies with the CustomActions.
        /// This option has no impact on normal .NET Framework assemblies as they are not AOT compiled. 
        /// <para>This option (<c>true</c> by default) is useful for hiding excessive warnings produced by AOT 
        /// compilation. But it makes sense to disable suppression if you are troubleshooting AOT compiles custom actions.</para>
        /// </summary>
        public static bool SuppressAotWarnings = true;

        /// <summary>
        /// WiX compiler <c>wix.exe</c> options (e.g. "-define DEBUG"). Available only in WiX v4.*
        /// <para>The default value is "-sw1026" (disable warning 1026).</para>
        /// </summary>
        public static string WixOptions = "-sw1026 -sw1076 -sw1079 -sw1149 ";

        /// <summary>
        /// Options for configuring the behavior of the file signing process.
        /// </summary>
        public static SignAllFilesOptions SignAllFilesOptions = new SignAllFilesOptions();

        static string autogeneratedWxsForVS = null;

        static Compiler()
        {
            EnsureVSIntegration();
        }

        static void EnsureVSIntegration()
        {
            try
            {
                // Debug.Assert(false);

                string projectName = Environment.GetCommandLineArgs().FirstPrefixedValue("/MSBUILD:", "/MBSBUILD:");

                if (Environment.GetEnvironmentVariable("ide") != null &&
                    MSBuild.EmitAutoGenFiles &&
                    projectName.IsNullOrEmpty())
                {
                    // VS2022 new integration model
                    projectName = Environment.GetCommandLineArgs().FirstOrDefault()?.PathGetFileNameWithoutExtension();
                }


                if (projectName.IsNotEmpty())
                {
                    //MSBuild always sets currdir to the project directory
                    string projFile = Environment.CurrentDirectory.PathJoin(projectName + ".csproj");

                    autogeneratedWxsForVS = Environment.CurrentDirectory.PathJoin("wix", projectName + ".g.wxs");

                    Environment.SetEnvironmentVariable("MSBUILD", "true");
                    Environment.SetEnvironmentVariable("MSBUILD_PROJ", projFile);

                    var doc = XDocument.Load(projFile);

                    if (doc.Root.Attribute("Sdk")?.Value == "Microsoft.NET.Sdk")
                    {
                        // new VS project format. No need to list the files
                        // the project folder will be automatically listed in the solution view
                    }
                    else
                    {
                        var ns = doc.Root.Name.Namespace;
                        string autogenItem = @"wix\$(ProjectName).g.wxs";
                        var auto_gen_elements = doc.Root.Descendants(ns + "None")
                                                        .Where(e => e.HasAttribute("Include", autogenItem));

                        bool injected = auto_gen_elements.Any();

                        if (MSBuild.EmitAutoGenFiles)
                        {

                            if (!injected)
                            {
                                var itemGroup = doc.Root.Descendants().Where(e => e.Name.LocalName == "Compile").First().Parent;
                                itemGroup.Add(new XElement(ns + "None", new XAttribute("Include", autogenItem)));
                                doc.Save(projFile);
                            }
                        }
                        else
                        {
                            if (injected)
                            {
                                auto_gen_elements.ToList().ForEach(e => e.Remove());
                                doc.Save(projFile);
                            }
                        }
                    }
                }
            }
            catch
            {
                // No need to do anything.
                // The whole purpose of EnsureVSIntegration is pure cosmetic - just to let user to access "*.g.wsx"
                // file from Visual Studio. There is a high chance of failure as the method depends on the project
                // file undocumented structure, which is subject to change by the third party(MS).
                // Even a complete failure of EnsureVSIntegration does not affect the MSI authoring outcome.
            }
        }

        /// <summary>
        /// Forces <see cref="Compiler"/> to preserve all temporary build files (e.g. *.wxs).
        /// <para>The default value is <c>false</c>: all temporary files are deleted at the end of the build/compilation.</para>
        /// <para>Note: if <see cref="Compiler"/> fails to build MSI the <c>PreserveTempFiles</c>
        /// value is ignored and all temporary files are preserved.</para>
        /// </summary>
        static public bool PreserveTempFiles = false;

        /// <summary>
        /// Forces <see cref="Compiler"/> to preserve all obj/pdb build files (e.g. *.wixobj and *.wixpdb).
        /// <para>The default value is <c>false</c>: all temporary files are deleted at the end of the build/compilation.</para>
        /// </summary>
        static public bool PreserveDbgFiles = false;

        /// <summary>
        /// The preferred consistency level of the auto-generated Component GUIDs.
        /// </summary>
        static public ComponentGuidConsistency PreferredComponentGuidConsistency = ComponentGuidConsistency.WithinAllVersions;
        /// <summary>
        /// Indicates whether compiler should emit relative or absolute paths in the WiX XML source.
        /// </summary>
        static public bool EmitRelativePaths = true;

        /// <summary>
        /// Gets or sets the GUID generator algorithm. You can use either one of the built-in algorithms or define your own.
        /// The default value is <see cref="GuidGenerators.Default"/>.
        /// <description>Possible WiX source file only:
        /// <code>
        /// //default built-in seeded GUID generator
        /// Compiler.GuidGenerator = GuidGenerators.Default;
        ///
        /// //sequential built-in GUID generator
        /// Compiler.GuidGenerator = GuidGenerators.Sequential;
        ///
        /// //Custom 'always-same' GUID generator
        /// Compiler.GuidGenerator = (seed) => Guid.Parse("9e2974a1-9539-4c5c-bef7-80fc35b9d7b0");
        ///
        /// //Custom random GUID generator
        /// Compiler.GuidGenerator = (seed) => Guid.NewGuid();
        /// </code>
        /// </description>
        /// </summary>
        /// <value>
        /// The GUID generator algorithm.
        /// </value>
        public static Func<object, Guid> GuidGenerator
        {
            get { return WixGuid.Generator; }
            set { WixGuid.Generator = value; }
        }

        /// <summary>
        /// The collection of temporary files created by compiler routine(s). These files will always be deleted unless Project/Compiler
        /// PreserveTempFiles property is set to true;
        /// </summary>
        public static List<string> TempFiles = new List<string>();

        /// <summary>
        /// Builds the MSI file from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <returns>Path to the built MSI file. Returns <c>null</c> if <c>MSI</c> cannot be built.</returns>
        static public string BuildMsi(Project project)
        {
            //Debugger.Launch();
            return Build(project, OutputType.MSI);
        }

        static string Build(Project project, OutputType type)
        {
            string outFile = IO.Path.GetFullPath(IO.Path.Combine(project.OutDir, project.OutFileName) + "." + type.ToString().ToLower());

            Utils.EnsureFileDir(outFile);

            WarnOnOutputPathCollision(outFile);

            if (IO.File.Exists(outFile))
                IO.File.Delete(outFile);

            Build(project, outFile, type);

            return IO.File.Exists(outFile) ? outFile : null;
        }

        /// <summary>
        /// Builds the WiX source file and generates batch file capable of building
        /// MSI with WiX toolset.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <returns>Path to the batch file.</returns>
        static public string BuildMsiCmd(Project project)
        {

            string cmdFile = IO.Path.GetFullPath(IO.Path.Combine(project.OutDir, "Build_" + project.OutFileName) + ".cmd");

            if (IO.File.Exists(cmdFile))
                IO.File.Delete(cmdFile);

            BuildMsiCmd(project, cmdFile);
            return cmdFile;
        }

        /// <summary>
        /// Builds the WiX source file and generates batch file capable of building
        /// MSM with WiX toolset.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <returns>Path to the batch file.</returns>
        static public string BuildMsmCmd(Project project)
        {
            string cmdFile = IO.Path.GetFullPath(IO.Path.Combine(project.OutDir, "Build_" + project.OutFileName) + ".cmd");

            if (IO.File.Exists(cmdFile))
                IO.File.Delete(cmdFile);

            BuildMsmCmd(project, cmdFile);

            return cmdFile;
        }

        /// <summary>
        /// Builds the WiX source file and generates batch file capable of building
        /// MSI with WiX toolset.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <param name="path">The path to the batch file to be build.</param>
        /// <returns>Path to the batch file.</returns>
        static public string BuildMsiCmd(Project project, string path)
        {
            BuildCmd(project, path, OutputType.MSI);
            return path;
        }


        /// <summary>
        /// Builds the WiX source file and generates batch file capable of building
        /// MSM with WiX toolset.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <param name="path">The path to the batch file to be build.</param>
        /// <returns>Path to the batch file.</returns>
        static public string BuildMsmCmd(Project project, string path)
        {
            BuildCmd(project, path, OutputType.MSM);
            return path;
        }

        static void BuildCmd(Project project, string path, OutputType type)
        {
            path = path.ExpandEnvVars();

            string batchFile = path;

            var exe = ExternalTool.Locate("wix.exe");

            if (exe == null)
            {
                var error = "`wix.exe` cannot be found. Ensure you installed it with `dotnet tool install --global wix`";
                Compiler.OutputWriteLine("Error: " + error);
                throw new ApplicationException(error);
            }

            string wxsFile = BuildWxs(project, type);

            string outDir = IO.Path.GetDirectoryName(wxsFile);
            string msiFile = IO.Path.ChangeExtension(wxsFile, "." + type.ToString().ToLower());


            string wixCmd = GenerateWixCommand(project, wxsFile);

            using (var sw = new IO.StreamWriter(batchFile))
            {
                sw.WriteLine("echo off");
                sw.WriteLine($"{exe.Replace("localtool:", "dotnet ")} build {wixCmd} -o \"{outDir.PathJoin(msiFile)}\"");
            }
        }

        /// <summary>
        /// Builds the MSI file from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <param name="path">The path to the MSI file to be build.</param>
        /// <returns>Path to the built MSI file.</returns>
        static public string BuildMsi(Project project, string path)
        {
            return Build(project, path.ExpandEnvVars(), OutputType.MSI);
        }

        /// <summary>
        /// Builds the MSM file from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <returns>Path to the built MSM file. Returns <c>null</c> if <c>msm</c> cannot be built.</returns>
        static public string BuildMsm(Project project)
        {
            return Build(project, OutputType.MSM);
        }

        /// <summary>
        /// Builds the MSM file from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <param name="path">The path to the MSM file to be build.</param>
        /// <returns>Path to the built MSM file.</returns>
        static public string BuildMsm(Project project, string path)
        {
            return Build(project, path.ExpandEnvVars(), OutputType.MSM);
        }

        /// <summary>
        /// Specifies the type of the setup binaries to build.
        /// </summary>
        public enum OutputType
        {
            /// <summary>
            /// MSI file.
            /// </summary>
            MSI,

            /// <summary>
            /// Merge Module (MSM) file.
            /// </summary>
            MSM
        }

        static void CopyAsAutogen(string source, string dest)
        {
            // Debug.Assert(false);
            string[] header = @"<!--
<auto-generated>
    This code was generated by WixSharp.
    Changes to this file will be lost if the code is regenerated.
    To suppress generation of this file remove `set ide=true` from the PostBuildEvent of the VS project
</auto-generated>
-->".Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var content = new List<string>(IO.File.ReadAllLines(source));
            content.InsertRange(1, header); //first line must be an XML declaration

            var dir = IO.Path.GetDirectoryName(dest);

            if (!IO.Directory.Exists(dir))
                IO.Directory.CreateDirectory(dir);

            if (IO.File.Exists(dest))
                IO.File.SetAttributes(dest, IO.FileAttributes.Normal); //just in case if it was set read-only

            IO.File.WriteAllLines(dest, content.ToArray());
        }

        static string GenerateWixCommand(WixProject project, string wxsFile)
        {
            // Debug.Assert(false);

            string extensionDlls = project.WixExtensions
                                   .Select(x => x.ExpandEnvVars())
                                   .Distinct()
                                   .JoinBy(" ", dll =>
                                   {
                                       var dllPath = dll;

                                       var preferredVersion = WixExtension.GetPreferredVersion(dll);

                                       WixTools.EnsureWixExtension(dll, preferredVersion);

                                       dllPath = WixTools.FindWixExtensionDll(dll, preferredVersion);

                                       bool isInNugetCache = dllPath?.StartsWith(WixTools.NuGetDir) == true;

                                       if (preferredVersion.IsEmpty())
                                       {
                                           if (!isInNugetCache &&
                                                dllPath.PathGetFileNameWithoutExtension() == dll && // not custom dll path
                                                WixTools.WixExtensionsDir.PathExists() &&
                                                WixTools.IsHighestAvailableVersion(dllPath)) // highest version in the WixTools.WixExtensionsDir
                                           {
                                               dllPath = dll; // let wix.exe to find the extension itself by specifying the extension ID only
                                           }
                                       }

                                       if (dllPath.IsEmpty())
                                       {
                                           var versionSuffix = "";
                                           if (!preferredVersion.IsEmpty())
                                               versionSuffix = "/" + preferredVersion;

                                           Compiler.OutputWriteLine($"Error: Cannot find WiX extension '{dll}'. " +
                                               $"WixSharp attempted to install the extension but did not succeed. Please install the " +
                                               $"extension manually with: `wix.exe extension add -g {dll}{versionSuffix}`");
                                       }

                                       return $"-ext \"{dllPath}\"";
                                   });

            string libFiles = project.LibFiles
                                     .Select(x => x.ExpandEnvVars())
                                     .Distinct()
                                     .JoinBy(" ", x => $"-lib \"{x}\"");

            string extraWxsFiles = project.WxsFiles
                                          .Select(x => x.ExpandEnvVars().PathGetFullPath())
                                          .Distinct()
                                          .JoinBy(" ", file => $"\"{file}\"");

            string localization = "";

            if (project.IsNonEnglish)
                localization += $" -culture {project.Language}";

            if (project.IsLocalized && project.LocalizationFile.PathExists())
                localization += $" -loc \"{project.LocalizationFile}\"";

            var candleOptions = (WixOptions + " " + project.WixOptions).Trim();

            var candleCmdLineParams = new StringBuilder();
            candleCmdLineParams.AppendFormat("{0} {1} {2} {3} \"{4}\" ", candleOptions, localization, extensionDlls, libFiles, wxsFile);

            var platform = (project as Project)?.Platform ?? (project as Bundle)?.Platform;

            if (platform.HasValue) // non-default architecture
                candleCmdLineParams.Append($" -arch {platform} ");

            if (extraWxsFiles.IsNotEmpty())
                candleCmdLineParams.Append(extraWxsFiles);

            var buildCmd = candleCmdLineParams.ToString().ExpandEnvVars();

            buildCmd = project.OnWixBuildCommandGenerated(buildCmd);

            return buildCmd;
        }

        static string Build(Project project, string path, OutputType type)
        {
            path = path.ExpandEnvVars();
            if (path.IsNotEmpty())
            {
                var ext = path.PathGetExtension();
                if (ext.IsEmpty())
                    path += "." + type.ToString().ToLower();
            }

            string oldCurrDir = Environment.CurrentDirectory;

            try
            {
                // System.Diagnostics.Debug.Assert(false);
                Compiler.TempFiles.Clear();

                // Sign all files if the project option is enabled
                if (project.SignAllFiles)
                {
                    if (project.DigitalSignature == null)
                    {
                        if (project.signAllFilesSet)
                            Compiler.OutputWriteLine("WARNING: DigitalSignature is not set but SignAllFiles is enabled. Please set DigitalSignature property of the project.");
                    }
                    else
                    {
                        if (!project.signAllFilesSet)
                            Compiler.OutputWriteLine(
                                "WARNING: DigitalSignature is set but SignAllFiles is not. The default SignAllFiles value 'true' is used.");
                        ProjectFileSigner.SignAllFiles(project, Compiler.SignAllFilesOptions);
                    }
                }
                else if (project.DigitalSignature != null)
                {
                    Compiler.OutputWriteLine("WARNING: DigitalSignature is set but SignAllFiles is disabled. Not all files are going to be signed, consider setting SignAllFiles property to true.");
                }

                string compiler = ExternalTool.Locate("wix.exe");

                if (compiler == null)
                {
                    var error = "`wix.exe` cannot be found. Ensure you installed it with `dotnet tool install --global wix`";
                    Compiler.OutputWriteLine("Error: " + error);
                    throw new ApplicationException(error);
                }


                string wxsFile = BuildWxs(project, type);

                if (autogeneratedWxsForVS != null && MSBuild.EmitAutoGenFiles)
                    CopyAsAutogen(wxsFile, autogeneratedWxsForVS);

                if (!project.SourceBaseDir.IsEmpty())
                    Environment.CurrentDirectory = project.SourceBaseDir;

                Compiler.OutputWriteLine($"Compiling {wxsFile.PathGetFileName()}");
                Compiler.OutputWriteLine("Source base directory: " + Environment.CurrentDirectory);


                string outDir = IO.Path.GetDirectoryName(wxsFile);
                string outFile = IO.Path.ChangeExtension(wxsFile, "." + type.ToString().ToLower());

                if (path.IsNotEmpty())
                    outFile = IO.Path.GetFullPath(path);

                if (IO.File.Exists(outFile))
                    IO.File.Delete(outFile);

                string compileCmd;
                compileCmd = $"build {GenerateWixCommand(project, wxsFile)} -o \"{outFile}\"";

                if (Compiler.VerboseOutput)
                {
                    Compiler.OutputWriteLine("<- Compiling");
                    Compiler.OutputWriteLine(compiler + " " + compileCmd);
                    Compiler.OutputWriteLine("->");
                }


                var output = Run(compiler, compileCmd);

                ProcessOutput(output);

                if (IO.File.Exists(outFile))
                {
                    Compiler.TempFiles.Add(wxsFile);

                    Compiler.OutputWriteLine("\n----------------------------------------------------------\n");
                    Compiler.OutputWriteLine(type + " file has been built: " + path + "\n");
                    Compiler.OutputWriteLine((type == OutputType.MSI ? " ProductName: " : " ModuleName: ") + project.Name);

                    if (project.SetVersionFromIdValue.IsEmpty())
                        Compiler.OutputWriteLine(" Version    : " + project.Version);
                    else
                        Compiler.OutputWriteLine($" Version    : {project.ExtractVersionFrom(project.SetVersionFromIdValue)} (overwritten from {project.SetVersionFromIdValue} file)");
                    //Compiler.OutputWriteLine($" Version    : {project.Version} (will be overwritten from {project.SetVersionFromIdValue} file)");

                    Compiler.OutputWriteLine(" ProductId  : {" + project.ProductId + "}");
                    Compiler.OutputWriteLine(" UpgradeCode: {" + project.UpgradeCode + "}\n");
                    if (!project.AutoAssignedInstallDirPath.IsEmpty())
                    {
                        Compiler.OutputWriteLine(" Auto-generated InstallDir ID:");
                        Compiler.OutputWriteLine("   " + Compiler.AutoGeneration.InstallDirDefaultId + "=" + project.AutoAssignedInstallDirPath);
                    }

                    if (!PreserveDbgFiles && !project.PreserveDbgFiles)
                    {
                        IO.Directory.GetFiles(outDir, "*.wixobj")
                                    .ForEach(file => file.DeleteIfExists());

                        IO.Path.ChangeExtension(wxsFile, ".wixpdb").DeleteIfExists();
                        IO.Path.ChangeExtension(path, ".wixpdb").DeleteIfExists();
                    }

                    project.DigitalSignature?.Apply(outFile);
                }


                if (!PreserveTempFiles && !project.PreserveTempFiles)
                    Compiler.TempFiles.ForEach(file => file.DeleteIfExists());
            }
            finally
            {
                Environment.CurrentDirectory = oldCurrDir;
            }

            return path;
        }

        static void ProcessOutput(string output)
        {
            // wix.exe : error WIX0144: The extension 'WixToolset.UI.wixext' could not be found. Checked paths: WixToolset.UI.wixext,
            // C:\Users\user\.wix\extensions\WixToolset.UI.wixext\5.0.0-rc.2\wixext4\WixToolset.UI.wixext.dll

            if (output.Contains("error WIX0144") &&
                output.Contains("WixToolset.UI.wixext") &&
                output.Contains("5.0.0-rc.2"))
            {
                Compiler.OutputWriteLine("\nWARNING: It looks like your msi requires WixToolset.UI.wixext and the latest version found on this PC " +
                    "is v5.0.0-rc.2.\nThis extension version is known as being broken so you have to use any other version released by the WiX team.\n" +
                    "You can do this by installing a newer version of this extension or by specifying the desired (even older) working version " +
                    "explicitly in your msi build script (e.g. setup.cs) before calling `project.BuiuldMsi(...)`:\n\n" +
                    "  WixExtension.UI.PreferredVersion = \"4.0.4\";\n");
            }
        }

        /// <summary>
        /// Builds the WiX source file (*.wxs) from the specified <see cref="Project"/> instance for further compiling into MSI file.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <returns>Path to the built WXS file.</returns>
        public static string BuildWxs(Project project)
        {
            return BuildWxs(project, OutputType.MSI);
        }

        /// <summary>
        /// Builds the WiX source file (*.wxs) from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <param name="type">The type (<see cref="OutputType"/>) of the setup file to be defined in the source file (MSI vs. MSM).</param>
        /// <returns>Path to the built WXS file.</returns>
        public static string BuildWxs(Project project, OutputType type)
        {
            lock (AutoGeneration.WxsGenerationSynchObject)
            {
                var oldAlgorithm = AutoGeneration.CustomIdAlgorithm;
                try
                {
                    project.ResetAutoIdGeneration(supressWarning: false);

                    AutoGeneration.CustomIdAlgorithm = project.CustomIdAlgorithm ?? AutoGeneration.CustomIdAlgorithm;

                    string file = IO.Path.GetFullPath(IO.Path.Combine(project.OutDir, project.OutFileName) + ".wxs");

                    if (IO.File.Exists(file))
                        IO.File.Delete(file);

                    BuildWxs(project, file, type);
                    return file;
                }
                finally
                {

                    AutoGeneration.CustomIdAlgorithm = oldAlgorithm;
                    project.ResetAutoIdGeneration(supressWarning: true);
                }
            }
        }

        /// <summary>
        /// Builds the WiX source file (*.wxs) from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <param name="path">The path to the WXS file to be build.</param>
        /// <param name="type">The type (<see cref="OutputType"/>) of the setup file to be defined in the source file (MSI vs. MSM).</param>
        /// <returns>Path to the built WXS file.</returns>
        public static string BuildWxs(Project project, string path, OutputType type)
        {
            if (Compiler.ClientAssembly.IsEmpty())
            {
                Compiler.ClientAssembly = Compiler.FindClientAssemblyInCallStack();
            }
            XDocument doc = GenerateWixProj(project);

            IndjectCustomUI(project.CustomUI, doc);

            AutoElements.InjectAutoElementsHandler(doc, project);
            DefaultWixSourceGeneratedHandler(doc);
            AutoElements.NormalizeFilePaths(doc, project.SourceBaseDir, EmitRelativePaths);

            if (type == OutputType.MSM)
            {
                //remove all pure MSI elements
                ConvertMsiToMsm(doc);
            }

            //issue#63 Inconsistent XML namespace usage in generated Wix source
            doc.AddDefaultNamespaces();

            project.InvokeWixSourceGenerated(doc);
            WixSourceGenerated?.Invoke(doc);

            doc.AddDefaultNamespaces();

            string xml = doc.ToString(); // this will remove XML declaration line (`<?xml...`) as WiX4 requires. Otherwise use XmlWriter
                                         // 
                                         // var sb = new StringBuilder();
                                         // var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = project.Encoding, Indent = true };
                                         // using (XmlWriter xw = XmlWriter.Create(sb, settings))
                                         //     doc.Save(xw);
                                         // string xml = sb.ToString();


            DefaultWixSourceFormatedHandler(ref xml);

            project.InvokeWixSourceFormated(ref xml);
            WixSourceFormated?.Invoke(ref xml);

            using (var sw = new IO.StreamWriter(path, false, project.Encoding))
                sw.WriteLine(xml);

            Compiler.OutputWriteLine("\n----------------------------------------------------------\n");
            Compiler.OutputWriteLine("Wix project file has been built: " + path + "\n");

            project.InvokeWixSourceSaved(path);
            WixSourceSaved?.Invoke(path);

            return path;
        }

        //This class is needed to overcome the StringWriter limitation when encoding cannot be changed
        internal class StringWriterWithEncoding : IO.StringWriter
        {
            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            Encoding encoding = Encoding.Default;

            public override Encoding Encoding { get { return encoding; } }
        }

        static void IndjectCustomUI(Controls.CustomUI customUI, XDocument doc)
        {
            if (customUI != null)
                doc.Root.Select(Compiler.ProductElementName).Add(customUI.ToXElement());
        }

        /// <summary>
        /// The default <see cref="Compiler.WixSourceGenerated"/> event handler.
        /// </summary>
        /// <param name="doc">The XDocument object representing WiX XML source code.</param>
        public static void DefaultWixSourceGeneratedHandler(XDocument doc)
        {
            XElement[] extract(string name)
                => doc.Root.Select(Compiler.ProductElementName)
                      .Elements()
                      .Where(e =>
                          {
                              if (name.StartsWith("*"))
                                  return e.Name.LocalName.EndsWith(name.Substring(1));
                              else
                                  return e.Name.LocalName == name;
                          })
                      .ToArray();

            //order references to Product.Elements()
            var orderedElements = extract("CustomAction")
                                 .Concat(extract("Binary"))
                                 .Concat(extract("UIRef"))
                                 .Concat(extract("Property"))
                                 .Concat(extract("Feature"))
                                 .Concat(extract("*Sequence"));

            //move elements to be ordered to the end of the doc
            foreach (var e in orderedElements)
            {
                e.Remove();
                doc.Root.Select(Compiler.ProductElementName).Add(e);
            }

            // Remove empty Cabs
            //  warning WIX1079: The cabinet 'CustomActionTest.cab' does not contain any files. If
            //  this installation contains no files, this warning can likely be safely ignored. Otherwise,
            //  please add files to the cabinet or remove it.

            if (!doc.Root.FindAll("File").Any() && Compiler.AutoGeneration.RemoveMediaIfNoFiles)
                doc.Root
                   .FindAll("Media")
                   .Where(x => x.Attributes().Any(a => a.LocalName().StartsWith(WixSharpXmlContextPrefix)))
                   .ForEach(x => x.Remove());

            // Remove any WixSharp context elements

            var temAttributes = doc.Root
                .Descendants()
                .SelectMany(x => x.Attributes()
                                  .Where(a => a.LocalName().StartsWith(WixSharpXmlContextPrefix)));

            temAttributes.ForEach(a => a.Remove());

        }

        internal static string WixSharpXmlContextPrefix = "wixsharp_";

        static void ConvertMsiToMsm(XDocument doc)
        {
            XElement[] extract(string name) => (from e in doc.Root.Select(Compiler.ProductElementName).Elements()
                                                where e.Name.LocalName == name
                                                select e).ToArray();

            var elementsToRemove = extract("Feature")
                                   .Concat(
                                   extract("Media"));

            //var elementsToRemove1 = extract("Media");
            foreach (var e in elementsToRemove)
                e.Remove();

            XElement product = doc.Root.Select(Compiler.ProductElementName);
            product.Remove();

            XElement module = doc.Root.AddElement(new XElement("Module", product.Elements()));
            module.CopyAttributeFrom("Id", product, "Name")
                  .CopyAttributeFrom(product, "Codepage")
                  .CopyAttributeFrom(product, "Language")
                  .CopyAttributeFrom(product, "Version");
            module.SetAttributeValue("Guid", product.Attribute("ProductCode").Value);

            XElement info = module.Select("SummaryInformation");
            info.CopyAttributeFrom(product, "Manufacturer");
        }

        /// <summary>
        /// The default <see cref="Compiler.WixSourceFormated"/> event handler.
        /// </summary>
        /// <param name="xml">The XML text string representing WiX XML source code.</param>
        public static void DefaultWixSourceFormatedHandler(ref string xml)
        {
            //very superficial formatting

            var mergeSections = new[] { "<Wix ", "<Media ", "<File ", "<MultiStringValue>", "<RemoveFolder", "<RemoteAddress>" };
            var splitSections = new[] { "</Product>", "</Module>" };

            StringBuilder sb = new StringBuilder();
            using (var sr = new IO.StringReader(xml))
            using (var sw = new IO.StringWriter(sb))
            {
                string line = "";
                string prevLine = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (prevLine.Trim().IsEmpty() && line.Trim().IsEmpty())
                        continue;

                    string lineTrimmed = line.Trim();
                    string prevLineTrimmed = prevLine.Trim();

                    if (!prevLine.Trim().IsEmpty() && prevLine.GetLeftIndent() == line.GetLeftIndent())
                    {
                        var delimiters = " ".ToCharArray();
                        var prevLineTokens = prevLine.Trim().Split(delimiters, 2);
                        var lineTokens = lineTrimmed.Split(delimiters, 2);
                        if (prevLineTokens.First() != lineTokens.First())
                        {
                            bool preventSpliting = false;
                            foreach (var token in mergeSections)
                                if (preventSpliting = lineTrimmed.StartsWith(token))
                                    break;

                            if (!preventSpliting)
                                sw.WriteLine();
                        }
                    }
                    else
                    {
                        if (lineTrimmed.StartsWith("<Component ")) //start of another component
                            sw.WriteLine();
                        else if (lineTrimmed == "</Directory>" && prevLineTrimmed == "</Component>") //last component
                            sw.WriteLine();
                    }

                    foreach (var token in splitSections)
                        if (line.Trim().StartsWith(token))
                            sw.WriteLine();

                    sw.WriteLine(line);
                    prevLine = line;
                }
            }

            xml = sb.ToString();
        }

        static void ResetCachedContent()
        {
            autogeneratedShortcutLocations.Clear();
        }

        /// <summary>
        /// Generates WiX XML source file the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> instance.</param>
        /// <returns>Instance of XDocument class representing in-memory WiX XML source file.</returns>
        public static XDocument GenerateWixProj(Project project)
        {
            project.GenerateProductGuids();

            project.Preprocess();

            ProjectValidator.Validate(project);

            project.ControlPanelInfo.AddMembersTo(project);

            project.AutoAssignedInstallDirPath = "";

            ResetCachedContent();

            string extraNamespaces = project.WixNamespaces.Distinct()
                                                          .Select(x => x.StartsWith("xmlns:") ? x : "xmlns:" + x)
                                                          .ConcatItems(" ");

            XDocument doc;
            XElement product;

            doc = XDocument.Parse($@"<?xml version=""1.0"" encoding=""utf-8""?>
                                         <Wix xmlns=""http://wixtoolset.org/schemas/v4/wxs"" {extraNamespaces}>
                                             <Package Compressed=""yes"">
                                                <SummaryInformation Codepage=""{project.Codepage}"" />
                                             </Package>
                                         </Wix>");

            product = doc.Root.Select("Package"); // in WiX v4 `Product` is merged with `Package`
            product.SetAttribute("ProductCode", project.ProductId);

            product.SetAttribute("Name", project.Name)
                   .SetAttribute("Language", project.Language.FirstLcid())
                   .SetAttribute("Version", project.Version)
                   .SetAttribute("UpgradeCode", project.UpgradeCode);

            if (project.ControlPanelInfo.Manufacturer.IsNotEmpty())
                product.SetAttribute("Manufacturer", project.ControlPanelInfo.Manufacturer);

            if (project.FailWhenDeferred)
            {
                product.AddElement(WixExtension.Util.XElement("FailWhenDeferred", null));
                project.Include(WixExtension.Util);
            }

            product.AddAttributes(project.Attributes);

            product.Select("SummaryInformation")
                   .SetAttribute("Description", project.Description);

            XElement package = doc.Root.Select("Package");

            package
                   .SetAttribute("Platform", project.Platform)
                   .SetAttribute("Scope", project.Scope)
                   .SetAttribute("InstallerVersion", project.InstallerVersion);

            if (project.EmitConsistentPackageId)
                package.CopyAttributeFrom(product, "Id");

            package.AddAttributes(project.Package.Attributes);
            foreach (Media item in project.Media)
                product.Add(item.ToXml((project as WixEntity).Id));

            ProcessWixVariables(project, product);
            ProcessLaunchConditions(project, product);

            //extend wDir

            XElement dirs = product;

            var featureComponents = new Dictionary<Feature, List<string>>(); //feature vs. component IDs
            var autoGeneratedComponents = new List<string>(); //component IDs
            var defaultFeatureComponents = new List<string>(); //default Feature (Complete setup) components

            ProcessDirectories(project, featureComponents, defaultFeatureComponents, autoGeneratedComponents, dirs);
            ProcessRegKeys(project, featureComponents, defaultFeatureComponents, product);
            ProcessProperties(project, product, featureComponents);
            ProcessCustomActions(project, product);

            //it is important to call ProcessBinaries after all other Process* (except ProcessFeatures)
            //as they may insert some implicit "binaries"
            ProcessBinaries(project, product);

            // Generic entities/elements; Doesn't matter when to process them. They are XML aware and place themselves
            // correctly in the XDocumant anyway.
            var context = new ProcessingContext
            {
                Project = project,
                Parent = project,
                XParent = product,
                FeatureComponents = featureComponents
            };

            foreach (IGenericEntity item in project.GenericItems)
                item.Process(context);

            ProcessFeatures(project, product, featureComponents, autoGeneratedComponents, defaultFeatureComponents);

            //special properties
            if (project.UI != WUI.WixUI_ProgressOnly)
            {
                string installDirId = project.ActualInstallDirId;

                if (installDirId.IsEmpty())
                {
                    var rootDir = GetTopLevelDir(product);
                    if (installDirId != null)
                        installDirId = rootDir.Attribute("Id").Value;
                }

                //if UIRef is set to WIXUI_INSTALLDIR must also add
                //<Property Id="WIXUI_INSTALLDIR" Value="directory id" />
                if (project.UI == WUI.WixUI_InstallDir)
                {

                    if (installDirId.IsEmpty())
                    {
                        // it's not an error but only a warning as the user
                        // may assign WIXUI_INSTALLDIR via XML injection
                        Compiler.OutputWriteLine("\n************\n" +
                                                 "Warning: Wix# cannot determine the installation directory to be auto assigned to WIXUI_INSTALLDIR.\n" +
                                                 "Mark a single Dir object as an installation directory with Dir.IsInstallDir or with the designated Id value.\n" +
                                                 "The designated Id is a value of Compiler.AutoGeneration.InstallDirDefaultId ('INSTALLDIR' by default).\n" +
                                                 "************\n");
                    }

                    product.AddElement("Property")
                           .SetAttribute("Id=WIXUI_INSTALLDIR")
                           .SetAttribute("Value", installDirId);

                    product.AddElement("Property")
                           .SetAttribute("Id=INSTALLDIR")
                           .SetAttribute("Secure=yes"); // Set INSTALLDIR as Secure by default #1774

                }

                product.AddElement(WixExtension.UI.XElement("WixUI", $"Id={project.UI}"));

                project.Include(WixExtension.UI);
            }

            if (project.LocalizeErrorAndProgressText)
            {
                product.AddElement("UIRef", "Id=WixUI_ErrorProgressText");
                project.Include(WixExtension.UI);
            }

            if (project.UI == WUI.WixUI_Common)
                product.AddElement("UIRef", $"Id={project.UI}");

            if (project.EmbeddedUI != null)
            {
                string bynaryPath = project.EmbeddedUI.Name;
                if (project.EmbeddedUI is EmbeddedAssembly)
                {
                    var asmBin = project.EmbeddedUI as EmbeddedAssembly;

                    bynaryPath = asmBin.Name.PathChangeDirectory(project.OutDir.PathGetFullPath())
                                            .PathChangeExtension(".CA.dll");

                    var refAsms = asmBin.RefAssemblies.Combine(typeof(Session).Assembly.Location)
                                                      .Concat(project.DefaultRefAssemblies)
                                                      .Distinct()
                                                      .ToArray();

                    PackageManagedAsm(asmBin.Name, bynaryPath, refAsms, project.OutDir, project.CustomActionConfig, null, true, signing: project.ContentSigningSignature);
                }

                product.AddElement("UI")
                       .Add(new XElement("EmbeddedUI",
                                new XAttribute("Id", project.EmbeddedUI.Id),
                                new XAttribute("SourceFile", bynaryPath)));
            }

            if (!project.BannerImage.IsEmpty())
            {
                product.Add(new XElement("WixVariable",
                    new XAttribute("Id", "WixUIBannerBmp"),
                    new XAttribute("Value", Utils.PathCombine(project.SourceBaseDir, project.BannerImage))));
            }

            if (!project.BackgroundImage.IsEmpty())
            {
                product.Add(new XElement("WixVariable",
                    new XAttribute("Id", "WixUIDialogBmp"),
                    new XAttribute("Value", Utils.PathCombine(project.SourceBaseDir, project.BackgroundImage))));
            }

            if (!project.LicenceFile.IsEmpty())
            {
                if (!AllowNonRtfLicense && !project.LicenceFile.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                    throw new ApplicationException("License file must have 'rtf' file extension. Specify 'Compiler.AllowNonRtfLicense=true' to overcome this constraint.");

                product.Add(
                       new XElement("WixVariable",
                       new XAttribute("Id", "WixUILicenseRtf"),
                       new XAttribute("Value", Utils.PathCombine(project.SourceBaseDir, project.LicenceFile)),
                       new XAttribute("xmlns", "")));
            }

            PostProcessMsm(project, product);
            ProcessUpgradeStrategy(project, product);
            ProcessReboot(project, product);

            return doc;
        }

        /// <summary>
        /// Defines if license file can be have non RTF extension.
        /// </summary>
        static public bool AllowNonRtfLicense = false;

        //auto-assign InstallDirDefaultId id for installation directory (the first directory that has multiple items)
        static string AutoAssignInstallDirId(Dir[] wDirs, string dirId)
        {
            if (wDirs.Count() != 0)
            {
                Dir firstDirWithItems = wDirs.First();

                string logicalPath = firstDirWithItems.Name;
                while (firstDirWithItems.Shortcuts.None() &&
                       firstDirWithItems.Dirs.SingleItem() &&
                       firstDirWithItems.Files.None())
                {
                    firstDirWithItems = firstDirWithItems.Dirs.First();
                    logicalPath += "\\" + firstDirWithItems.Name;
                }

                if ((!firstDirWithItems.IsIdSet() || firstDirWithItems.isAutoId) &&
                    dirId.IsNotEmpty())
                {
                    if (firstDirWithItems.Id.IsWixConstant())
                        EmitInstallDirCollisionWarning(dirId, logicalPath);

                    firstDirWithItems.Id = dirId;
                    return logicalPath;
                }
            }
            return null;
        }

        static void EmitInstallDirCollisionWarning(string dirId, string logicalPath)
        {
            Compiler.OutputWriteLine($"WARNING: Auto assigned installation directory ID '{dirId}' collides with the existing directory ID '{logicalPath}'.\n" +
                                      "If it was not intended disable auto assignment by setting 'Compiler.AutoGeneration.InstallDirDefaultId' to null.\n" +
                                      "Or set  'Dir.IsInstallDir = true' for the installation directory.\r" +
                                      "Or instead of 'new Dir(...' use 'new InstallDir(...' for the installation directory.");
        }

        static XElement GetTopLevelDir(XElement product)
        {
            XElement dir = product.Elements("Directory").First();
            XElement prevDir = null;
            while (dir != null)
            {
                prevDir = dir;
                if (dir.Elements("Component").Count() == 0) //just a subdirectory without any installable items
                    dir = dir.Elements("Directory").FirstOrDefault();
                else
                    return dir; //dir containing installable items (e.g. files or shortcuts)
            }

            return prevDir;
        }

        static XElement GetTopLevelPermanentDir(XElement product, bool is64)
        {
            // "Program Files" dir is guaranteed to be on all OS flavors
            var dirName = is64 ? "ProgramFiles64Folder" : "ProgramFilesFolder";

            XElement dir = product.Descendants("Directory").FirstOrDefault(d => d.HasAttribute("Name", dirName));
            if (dir != null)
            {
                return dir;
            }
            else
            {
                // nothing has been found so create it
                return product.Elements("Directory").First().AddElement("Directory", $"Id={dirName};Name={dirName}");
            }
        }

        static void ProcessFeatures(Project project, XElement product, Dictionary<Feature, List<string>> featureComponents, List<string> autoGeneratedComponents, List<string> defaultFeatureComponents)
        {
            if (!featureComponents.ContainsKey(project.DefaultFeature))
            {
                featureComponents[project.DefaultFeature] = new List<string>(defaultFeatureComponents); //assign defaultFeatureComponents to the project's default Feature
            }
            else
            {
                foreach (string comp in defaultFeatureComponents)
                    featureComponents[project.DefaultFeature].Add(comp);
            }

            var feature2XML = new Dictionary<Feature, XElement>();

            // generate disconnected XML nodes
            var featuresWithComponents = featureComponents.Keys.ToArray();
            var allFeatures = featuresWithComponents.AllChildren(x => x.Children)
                                                    .Distinct()
                                                    .ToArray();

            // featuresWithComponents may still have parents that are not associated with any component
            allFeatures = allFeatures.Concat(allFeatures.Select(x => x.Parent).Where(x => x != null))
                                     .Distinct()
                                     .ToArray();


            var allNonComponentFeatures = allFeatures.Where(x => !featuresWithComponents.Contains(x))
                                                     .ToArray();

            featuresWithComponents.ForEach(wFeature =>
            {
                var comps = featureComponents[wFeature];

                XElement xFeature = wFeature.ToXml();

                foreach (string componentId in featureComponents[wFeature])
                    xFeature.Add(new XElement("ComponentRef",
                                    new XAttribute("Id", componentId)));

                foreach (string componentId in autoGeneratedComponents)
                    xFeature.Add(new XElement("ComponentRef",
                                    new XAttribute("Id", componentId)));

                feature2XML.Add(wFeature, xFeature);
            });

            allNonComponentFeatures.ForEach(feature =>
            {
                feature2XML.Add(feature, feature.ToXml());
            });

            //establish relationships
            foreach (Feature wFeature in allFeatures)
            {
                if (wFeature.Children.Count != 0)
                {
                    foreach (Feature wChild in wFeature.Children)
                    {
                        wChild.Parent = wFeature;
                        XElement xFeature = feature2XML[wFeature];

                        if (feature2XML.ContainsKey(wChild))
                        {
                            XElement xChild = feature2XML[wChild];
                            xFeature.Add(xChild);
                        }
                    }
                }
            }

            //remove childless features as they have non practical value
            feature2XML.Keys
                       .Where(x => !feature2XML[x].HasElements)
                       .ToArray()
                       .ForEach(key => feature2XML.Remove(key));

            var topLevelFeatures = feature2XML.Keys
                                              .Where(x => x.Parent == null)
                                              .Select(x => feature2XML[x])
                                              .ToArray();

            foreach (XElement xFeature in topLevelFeatures)
            {
                product.AddElement(xFeature);
            }
        }

        static void ProcessReboot(Project project, XElement product)
        {
            if (project.ForceReboot != null)
            {
                var sequenceElement = product.SelectOrCreate("InstallExecuteSequence"); //only allowed sequence
                sequenceElement.Add(project.ForceReboot.ToXml());
            }

            if (project.ScheduleReboot != null)
            {
                var scheduleReboot = project.ScheduleReboot.ToXml();

                void addToUi()
                {
                    var installUI = product.FindAll("InstallUISequence").FirstOrDefault();
                    if (installUI != null)
                        installUI.Add(scheduleReboot);
                    else
                        Console.Write("Warning: ScheduleReboot is configured to be placed in InstallUISequence, which is not defined.");
                }

                void addToExecute()
                {
                    var installExecute = product.SelectOrCreate("InstallExecuteSequence");
                    installExecute.Add(scheduleReboot);
                }

                if (project.ScheduleReboot.InstallSequence == RebootInstallSequence.InstallExecute)
                    addToExecute();
                else if (project.ScheduleReboot.InstallSequence == RebootInstallSequence.InstallUI)
                    addToUi();
                else if (project.ScheduleReboot.InstallSequence == RebootInstallSequence.Both)
                {
                    addToUi();
                    addToExecute();
                }
            }
        }

        static void ProcessUpgradeStrategy(Project project, XElement product)
        {
            if (project.MajorUpgrade != null)
            {
                product.Add(project.MajorUpgrade.ToXml());
            }

            if (project.MajorUpgradeStrategy != null)
            {
                string ExpandVersion(string version)
                    => version == "%this%" ? project.Version.ToString() : version;

                var upgradeElement = product.AddElement(
                    new XElement("Upgrade",
                       new XAttribute("Id", project.UpgradeCode)));

                if (project.MajorUpgradeStrategy.UpgradeVersions != null)
                {
                    VersionRange versions = project.MajorUpgradeStrategy.UpgradeVersions;

                    var upgradeVersion = upgradeElement.AddElement("UpgradeVersion");

                    upgradeVersion.SetAttribute("Minimum", ExpandVersion(versions.Minimum))
                                  .SetAttribute("Maximum", ExpandVersion(versions.Maximum))
                                  .SetAttribute("IncludeMinimum", versions.IncludeMinimum)
                                  .SetAttribute("IncludeMaximum", versions.IncludeMaximum)
                                  .SetAttribute("OnlyDetect", versions.OnlyDetect)
                                  .SetAttribute("Property", "UPGRADEFOUND")
                                  .SetAttribute("MigrateFeatures", versions.MigrateFeatures);
                }

                if (project.MajorUpgradeStrategy.PreventDowngradingVersions != null)
                {
                    VersionRange versions = project.MajorUpgradeStrategy.PreventDowngradingVersions;

                    var upgradeVersion = upgradeElement.AddElement("UpgradeVersion");

                    upgradeVersion.SetAttribute("Minimum", ExpandVersion(versions.Minimum))
                                  .SetAttribute("Maximum", versions.Maximum)
                                  .SetAttribute("IncludeMinimum", versions.IncludeMinimum)
                                  .SetAttribute("IncludeMaximum", versions.IncludeMaximum)
                                  .SetAttribute("OnlyDetect", versions.OnlyDetect)
                                  .SetAttribute("Property", "NEWPRODUCTFOUND")
                                  .SetAttribute("MigrateFeatures", versions.MigrateFeatures);

                    var installExec = product.SelectOrCreate("InstallExecuteSequence");
                    var installUI = product.SelectOrCreate("InstallUISequence");

                    bool preventDowngrading = (project.MajorUpgradeStrategy.NewerProductInstalledErrorMessage != null);

                    if (preventDowngrading)
                    {
                        product.Add(new XElement("CustomAction",
                                        new XAttribute("Id", "PreventDowngrading"),
                                        new XAttribute("Error", project.MajorUpgradeStrategy.NewerProductInstalledErrorMessage)));

                        installExec.Add(new XElement("Custom",
                                            new XAttribute("Condition", "NEWPRODUCTFOUND"),
                                            new XAttribute("Action", "PreventDowngrading"),
                                            new XAttribute("After", "FindRelatedProducts")));

                        installUI.Add(new XElement("Custom",
                                          new XAttribute("Condition", "NEWPRODUCTFOUND"),
                                          new XAttribute("Action", "PreventDowngrading"),
                                          new XAttribute("After", "FindRelatedProducts")));
                    }

                    installExec.Add(new XElement("RemoveExistingProducts",
                                        new XAttribute("After", project.MajorUpgradeStrategy.RemoveExistingProductAfter.ToString())));
                }
            }
        }

        static Dictionary<string, Feature> autogeneratedShortcutLocations = new Dictionary<string, Feature>();

        static void ProcessDirectory(Dir wDir, Project wProject, Dictionary<Feature, List<string>> featureComponents,
            List<string> defaultFeatureComponents, List<string> autoGeneratedComponents, XElement parent)
        {
            XElement dirItem = AddDir(parent, wDir);

            bool isEmptyDir = wDir.Files.None() &&
                              wDir.Shortcuts.None() &&
                              wDir.Dirs.None() &&
                              wDir.GenericItems.None() &&
                              wDir.MergeModules.None() &&
                              wDir.Permissions.None() &&
                              wDir.IISVirtualDirs.None() &&
                              wDir.ODBCDataSources.None();

            if (isEmptyDir)
            {
                var existingCompElement = dirItem.Elements("Component");

                if (existingCompElement.Count() == 0 && AutoElements.SupportEmptyDirectories == CompilerSupportState.Enabled)
                {
                    string compId = wDir.GenerateComponentId(wProject, ".EmptyDirectory");

                    if (wDir.ActualFeatures.Any())
                    {
                        featureComponents.Map(wDir.ActualFeatures, compId);
                    }
                    else
                    {
                        defaultFeatureComponents.Add(compId);
                    }

                    dirItem.AddElement(
                        new XElement("Component",
                            new XAttribute("Id", compId),
                            new XAttribute("Guid", WixGuid.NewGuid(compId))));
                }
            }
            else
            {
                if (wDir.ActualFeatures.Any())
                {
                    string compId = wDir.GenerateComponentId(wProject);

                    featureComponents.Map(wDir.ActualFeatures, compId);
                    dirItem.AddElement(
                            new XElement("Component",
                                new XAttribute("Id", compId),
                                new XAttribute("Guid", WixGuid.NewGuid(compId))));
                }

                ProcessDirectoryFiles(wDir, wProject, featureComponents, defaultFeatureComponents, dirItem);
                ProcessDirectoryShortcuts(wDir, wProject, featureComponents, defaultFeatureComponents, dirItem);
                ProcessOdbcSources(wDir, wProject, featureComponents, defaultFeatureComponents, dirItem);
                ProcessMergeModules(wDir, dirItem, featureComponents, defaultFeatureComponents);
                ProcessDirPermissions(wDir, wProject, featureComponents, defaultFeatureComponents, dirItem);
                ProcessIISVirtualDirs(wDir, wProject, featureComponents, defaultFeatureComponents, dirItem);
                ProcessGenericItems(wDir, wProject, featureComponents, defaultFeatureComponents, dirItem);

                foreach (Dir subDir in wDir.Dirs)
                    ProcessDirectory(subDir, wProject, featureComponents, defaultFeatureComponents, autoGeneratedComponents, dirItem);
            }
        }

        static void ProcessIISVirtualDirs(Dir wDir, Project wProject, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents, XElement dirItem)
        {
            if (wDir.IISVirtualDirs.Any())
            {
                string compId = wDir.GenerateComponentId(wProject, ".VirtDir");

                if (wDir.ActualFeatures.Any())
                    featureComponents.Map(wDir.ActualFeatures, compId);
                else
                    defaultFeatureComponents.Add(compId);

                var comp = dirItem.AddElement("Component")
                                  .SetAttribute("Id", compId)
                                  .SetAttribute("Guid", WixGuid.NewGuid(compId));

                InsertIISElements(dirItem, comp, wDir.IISVirtualDirs, wProject);
            }
        }
        static void ProcessDirectoryFiles(Dir wDir, Project wProject, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents, XElement dirItem)
        {
            //insert files in the last leaf directory node
            foreach (File wFile in wDir.Files)
            {
                string fileId = wFile.Id;
                string compId = wFile.GenerateComponentId(wProject);

                if (wFile.ActualFeatures.Any())
                {
                    featureComponents.Map(wFile.ActualFeatures, compId);
                }
                else
                {
                    defaultFeatureComponents.Add(compId);
                }

                XElement comp = dirItem.AddElement(
                    new XElement("Component",
                        new XAttribute("Id", compId),
                        new XAttribute("Guid", WixGuid.NewGuid(compId))));

                if (wFile.Condition != null)
                    comp.AddAttributes("Condition=" + wFile.Condition.ToXValue())
                        .AddAttributes(wFile.Condition.Attributes);

                XElement file = comp.AddElement(
                    new XElement("File",
                        new XAttribute("Id", fileId),
                        new XAttribute("Source", Utils.PathCombine(wProject.SourceBaseDir, wFile.Name)))
                        .AddAttributes(wFile.Attributes));

                if (wFile.OverwriteOnInstall)
                {
                    string removeFileName = wFile.TargetFileName.IsNotEmpty() ?
                                            wFile.TargetFileName :
                                            wFile.Name.PathGetFileName();

                    comp.AddElement("RemoveFile",
                                  $@"Id=Remove_{fileId};
                                     Name={removeFileName};
                                     On=both");
                }

                if (wFile is FontFile)
                {
                    var font = (wFile as FontFile);
                    file.SetAttribute("TrueType", font.TrueType)
                        .SetAttribute("FontTitle", font.FontTitle);
                }

                var context = new ProcessingContext
                {
                    Project = wProject,
                    Parent = wFile,
                    XParent = comp,
                    FeatureComponents = featureComponents
                };

                wFile.ServiceInstallers?.ForEach(x => x.Process(context));

                if (wFile is Assembly assembly && assembly.RegisterInGAC)
                {
                    file.Add(new XAttribute("KeyPath", "yes"),
                             new XAttribute("Assembly", ".net"),
                             new XAttribute("AssemblyManifest", fileId),
                             new XAttribute("ProcessorArchitecture", assembly.ProcessorArchitecture.ToString()));
                }

                //insert file associations
                foreach (FileAssociation wFileAssociation in wFile.Associations)
                {
                    XElement progId;
                    comp.Add(progId = new XElement("ProgId",
                                          new XAttribute("Id", wFileAssociation.Extension + ".file"),
                                          new XAttribute("Advertise", wFileAssociation.Advertise.ToYesNo()),
                                          new XAttribute("Description", wFileAssociation.Description),
                                          new XElement("Extension",
                                              new XAttribute("Id", wFileAssociation.Extension),
                                              new XAttribute("ContentType", wFileAssociation.ContentType),
                                              new XElement("Verb",
                                                  wFileAssociation.Advertise ?
                                                     new XAttribute("Sequence", wFileAssociation.SequenceNo) :
                                                     new XAttribute("TargetFile", fileId),
                                                  new XAttribute("Id", wFileAssociation.Command),
                                                  new XAttribute("Command", wFileAssociation.Command),
                                                  new XAttribute("Argument", wFileAssociation.Arguments)))));

                    if (wFileAssociation.Icon != null)
                    {
                        if (wFileAssociation.Advertise)
                        {
                            var icon = new IconFile { SourceFile = wFileAssociation.Icon };

                            progId.Parent(Compiler.ProductElementName)
                                  .AddElement(icon.ToXElement("Icon"));

                            progId.SetAttribute("Icon", icon.Id);
                        }
                        else
                        {
                            progId.Add(
                                new XAttribute("Icon", wFileAssociation.Icon != "" ? wFileAssociation.Icon : fileId),
                                new XAttribute("IconIndex", wFileAssociation.IconIndex));
                        }
                    }
                }

                //insert file owned shortcuts
                foreach (Shortcut wShortcut in wFile.Shortcuts)
                {
                    string locationDirId;

                    if (wShortcut.Location.IsEmpty())
                    {
                        locationDirId = wDir.Id;
                    }
                    else
                    {
                        Dir locationDir = wProject.FindDir(wShortcut.Location);

                        if (locationDir == null)
                        {
                            // may be location is a path but a dir Id
                            // Triggered by Fileshortcuts starting with an integer #307
                            locationDir = wProject.AllDirs.FirstOrDefault(x => x.Id == wShortcut.Location);
                        }

                        if (locationDir != null)
                        {
                            locationDirId = locationDir.Id;
                        }
                        else
                        {
                            if (!autogeneratedShortcutLocations.ContainsKey(wShortcut.Location))
                                autogeneratedShortcutLocations.Add(wShortcut.Location, wShortcut.Feature);

                            locationDirId = wShortcut.Location.Expand();
                        }
                    }

                    string workingDir = wShortcut.WorkingDirectory.IsNotEmpty() ?
                                            wShortcut.WorkingDirectory :
                                            dirItem.Attribute("Id").Value;

                    var shortcutId = wShortcut.Id;
                    if (wShortcut.isAutoId)
                        shortcutId = "Shortcut." + wFile.Id + "." + shortcutId; // to show the file parent

                    var shortcutElement =
                        new XElement("Shortcut",
                            new XAttribute("Id", shortcutId),
                            new XAttribute("WorkingDirectory", workingDir),
                            new XAttribute("Arguments", wShortcut.Arguments),
                            new XAttribute("Directory", locationDirId),
                            new XAttribute("Name", wShortcut.Name.IsNullOrEmpty() ? IO.Path.GetFileNameWithoutExtension(wFile.Name) : wShortcut.Name + ".lnk"));

                    wShortcut.EmitAttributes(shortcutElement);
                    wShortcut.EmitShortcutProperties(shortcutElement);

                    file.Add(shortcutElement);
                }

                // insert file related IIS virtual directories
                InsertIISElements(dirItem, comp, wFile.IISVirtualDirs, wProject);

                // insert file owned permissions
                ProcessFilePermissions(wProject, wFile, file);

                // Expand/serialize file owned generic items and insert the in to the `File` element
                ProcessGenericItems(wFile, wProject, featureComponents, defaultFeatureComponents, file);
            }
        }

        static void ProcessDirectoryShortcuts(Dir wDir, Project wProject, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents, XElement dirItem)
        {
            //insert directory owned shortcuts
            foreach (Shortcut wShortcut in wDir.Shortcuts)
            {
                string compId = wShortcut.GenerateComponentId(wProject);
                if (wShortcut.ActualFeatures.Any())
                    featureComponents.Map(wShortcut.ActualFeatures, compId);
                else
                    defaultFeatureComponents.Add(compId);

                XElement comp = dirItem.AddElement(
                   new XElement("Component",
                       new XAttribute("Id", compId),
                       new XAttribute("Guid", WixGuid.NewGuid(compId))));

                if (wShortcut.Condition != null)
                    comp.AddAttributes("Condition=" + wShortcut.Condition.ToXValue())
                        .AddAttributes(wShortcut.Condition.Attributes);

                string workingDir = wShortcut.WorkingDirectory.IsNotEmpty() ? wShortcut.WorkingDirectory : GetShortcutWorkingDirectory(wShortcut.Target);

                XElement sc = comp.AddElement(
                   new XElement("Shortcut",
                       new XAttribute("Id", wDir.Id + "." + wShortcut.Id),
                       //new XAttribute("Directory", wDir.Id), //not needed for Wix# as this attributed is required only if the shortcut is not nested under a Component element.
                       new XAttribute("WorkingDirectory", workingDir),
                       new XAttribute("Target", wShortcut.Target.NormalizeWixString()),
                       new XAttribute("Arguments", wShortcut.Arguments),
                       new XAttribute("Name", wShortcut.Name + ".lnk")));

                wShortcut.EmitAttributes(sc);
                wShortcut.EmitShortcutProperties(sc);
            }
        }

        static void ProcessOdbcSources(Dir wDir, Project wProject, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents, XElement dirItem)
        {
            foreach (ODBCDataSource wODBCDataSource in wDir.ODBCDataSources)
            {
                string dsnId = wODBCDataSource.Id;
                string compId = wODBCDataSource.GenerateComponentId(wProject);

                if (wODBCDataSource.ActualFeatures.Any())
                    featureComponents.Map(wODBCDataSource.ActualFeatures, compId);
                else
                    defaultFeatureComponents.Add(compId);

                XElement comp = dirItem.AddElement(
                   new XElement("Component",
                       new XAttribute("Id", compId),
                       new XAttribute("Guid", WixGuid.NewGuid(compId))));

                XElement dsn = comp.AddElement(
                    new XElement("ODBCDataSource",
                        new XAttribute("Id", wODBCDataSource.Id),
                        new XAttribute("Name", wODBCDataSource.Name),
                        new XAttribute("DriverName", wODBCDataSource.DriverName),
                        new XAttribute("KeyPath", wODBCDataSource.KeyPath.ToYesNo()),
                        new XAttribute("Registration", (wODBCDataSource.PerMachineRegistration ? "machine" : "user"))));

                dsn.AddAttributes(wODBCDataSource.Attributes);

                foreach (Property prop in wODBCDataSource.Properties)
                {
                    dsn.AddElement(
                        new XElement("Property",
                                    new XAttribute("Id", prop.Name),
                                    new XAttribute("Value", prop.Value)));
                }
            }
        }

        static void ProcessFilePermissions(Project wProject, File wFile, XElement file)
        {
            if (wFile.Permissions.Any())
            {
                var utilExtension = WixExtension.Util;
                wProject.Include(utilExtension);

                foreach (var permission in wFile.Permissions)
                {
                    var element = new XElement(utilExtension.ToXNamespace() + "PermissionEx");
                    permission.EmitAttributes(element);
                    file.Add(element);
                }
            }
        }

        static void ProcessDirPermissions(Dir wDir, Project wProject, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents, XElement dirItem)
        {
            if (wDir.Permissions.Any())
            {
                var utilExtension = WixExtension.Util;
                wProject.Include(utilExtension);

                foreach (var permission in wDir.Permissions)
                {
                    string compId = permission.GenerateComponentId(wProject);

                    if (permission.ActualFeatures.Any())
                    {
                        featureComponents.Map(wDir.ActualFeatures, compId);
                    }
                    else
                    {
                        // since permissions belong to the directory
                        // it makes sense to use the parent features if the has not its own features set
                        if (wDir.ActualFeatures.Any())
                            featureComponents.Map(wDir.ActualFeatures, compId);
                        else
                            defaultFeatureComponents.Add(compId);
                    }

                    var permissionElement = new XElement(utilExtension.ToXNamespace() + "PermissionEx");
                    permission.EmitAttributes(permissionElement);
                    dirItem.Add(
                        new XElement("Component",
                            new XAttribute("Id", compId),
                            new XAttribute("Guid", WixGuid.NewGuid(compId)),
                            new XElement("CreateFolder",
                                permissionElement)));
                }
            }
        }

        static void ProcessGenericItems(Dir wDir, Project wProject, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents, XElement dirItem)
        {
            if (wDir.GenericItems.Any())
            {
                var context = new ProcessingContext
                {
                    Project = wProject,
                    Parent = wDir,
                    XParent = dirItem,
                    FeatureComponents = featureComponents
                };

                string[] getAllComponents() => dirItem.Document.Root.Descendants("Component").Select(x => x.Attribute("Id").Value).ToArray();

                var componentsBefore = getAllComponents();

                foreach (IGenericEntity item in wDir.GenericItems)
                    item.Process(context);

                var componentsAfter = getAllComponents();
                var compsWithFeature = featureComponents.SelectMany(x => x.Value)
                                                        .Concat(defaultFeatureComponents)
                                                        .Distinct();

                var newComponetsWithoutFeature = componentsAfter.Except(componentsBefore)
                                                                .Except(compsWithFeature);

                defaultFeatureComponents.AddRange(newComponetsWithoutFeature);
            }
        }

        static void ProcessGenericItems(File file, Project wProject, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents, XElement fileItem)
        {
            if (file.GenericItems.Any())
            {
                var context = new ProcessingContext
                {
                    Project = wProject,
                    Parent = file,
                    XParent = fileItem,
                    XParentComponent = fileItem.Component(),
                    FeatureComponents = featureComponents
                };

                string[] getAllComponents() => fileItem.Document.Root.Descendants("Component").Select(x => x.Attribute("Id").Value).ToArray();

                var componentsBefore = getAllComponents();

                foreach (IGenericEntity item in file.GenericItems)
                    item.Process(context);

                var componentsAfter = getAllComponents();
                var compsWithFeature = featureComponents.SelectMany(x => x.Value)
                                                        .Concat(defaultFeatureComponents)
                                                        .Distinct();

                var newComponetsWithoutFeature = componentsAfter.Except(componentsBefore)
                                                                .Except(compsWithFeature);

                defaultFeatureComponents.AddRange(newComponetsWithoutFeature);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "needed to maintain API signatures consistency")]
        static void ProcessMergeModules(Dir wDir, XElement dirItem, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents)
        {
            foreach (Merge msm in wDir.MergeModules)
            {
                XElement product = dirItem.Parent(Compiler.ProductElementName);

                // In WiX4 package.Attribute("Languages") does not longer exist. 
                string language = product.Attribute("Language").Value;
                string diskId = product.Select("Media")?.Attribute("Id")?.Value ?? "1"; // see Issue #362 (Merge Modules cause NullRefException when MediaTemplate is used) discussion

                XElement merge = dirItem.AddElement(
                    new XElement("Merge",
                        new XAttribute("Id", msm.Id),
                        new XAttribute("FileCompression", msm.FileCompression.ToYesNo()),
                        new XAttribute("Language", language),
                        new XAttribute("SourceFile", msm.SourceFile),
                        new XAttribute("DiskId", diskId)
                        )
                        .AddAttributes(msm.Attributes));

                //MSM features are very different as they are not associated with the component but with the merge module thus
                //it's not possible to add the feature 'child' MergeRef neither to featureComponents nor defaultFeatureComponents.
                //Instead we'll let PostProcessMsm to handle it because ProcessFeatures cannot do this. It will ignore this feature
                //as it has no nested elements.
                //Not elegant at all but works.
                //In the future it can be improved by allowing MergeRef to be added in the global collection featureMergeModules/DefaultfeatureComponents.
                //Then PostProcessMsm will not be needed.
                foreach (Feature feature in msm.ActualFeatures)
                    if (!featureComponents.ContainsKey(feature))
                        featureComponents[feature] = new List<string>();
            }
        }

        static void PostProcessMsm(Project project, XElement product)
        {
            var modules = from dir in project.AllDirs
                          from msm in dir.MergeModules
                          select new
                          {
                              Features = msm.ActualFeatures,
                              MsmId = msm.Id
                          };

            var features = product.Descendants("Feature")
                                  .ToDictionary(x => x.Attribute("Id").Value);

            foreach (var item in modules)
            {
                XElement xFeature = null;

                if (!item.Features.Any())
                {
                    if (features.ContainsKey("Complete"))
                        xFeature = features["Complete"];
                    else
                        throw new Exception("Merge Module " + item.MsmId + " does not belong to any feature and \"Complete\" feature is not found");
                }
                else
                {
                    foreach (Feature itemFeature in item.Features)
                        if (features.ContainsKey(itemFeature.Id))
                        {
                            xFeature = features[itemFeature.Id];
                        }
                        else
                        {
                            xFeature = product.AddElement(itemFeature.ToXml());
                            features.Add(itemFeature.Id, xFeature);
                        }
                }

                xFeature?.Add(new XElement("MergeRef",
                                 new XAttribute("Id", item.MsmId)));
            }
        }

        static void ProcessDirectories(Project wProject, Dictionary<Feature, List<string>> featureComponents,
            List<string> defaultFeatureComponents, List<string> autoGeneratedComponents, XElement dirs)
        {
            wProject.ResolveWildCards(Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories);

            if (wProject.Dirs.Count() == 0)
            {
                //WIX/MSI does not like no-directory deployments thus create fake one
                string dummyDir = @"%ProgramFiles%";

                if (AutoElements.LegacyDummyDirAlgorithm)
                {
                    dummyDir = @"%ProgramFiles%\WixSharp\DummyDir";

                    if (wProject.Is64Bit)
                        dummyDir = dummyDir.Map64Dirs();

                    wProject.Dirs = new[] { new Dir(dummyDir) };
                }
                else
                {
                    // need to set ID so it's not replaced with "INSTALLDIR"
                    wProject.Dirs = new[] { new Dir(new Id("ProgramFilesFolder"), dummyDir) };
                }
            }

            Dir[] wDirs = wProject.Dirs;

            //ensure user assigned install dir is marked with the dedicated ID

            //explicitly defined install dir (via bool IsInstallDir)
            var userDefinedInstallDir = wProject.AllDirs
                                                .FirstOrDefault(x => x.IsInstallDir);

            if (userDefinedInstallDir != null)
            {
                if (!userDefinedInstallDir.IsIdSet())
                {
                    if (userDefinedInstallDir.Id.IsWixConstant())
                        EmitInstallDirCollisionWarning(Compiler.AutoGeneration.InstallDirDefaultId, userDefinedInstallDir.Id);
                    userDefinedInstallDir.Id = Compiler.AutoGeneration.InstallDirDefaultId;
                }

                wProject.ActualInstallDirId = userDefinedInstallDir.Id;
            }
            else
            {
                //implicitly defined install dir (via dedicated id value)
                //important to use RawId to avoid triggering Id premature auto-generation
                if (wProject.AllDirs.Any(x => x.RawId == Compiler.AutoGeneration.InstallDirDefaultId))
                    wProject.ActualInstallDirId = Compiler.AutoGeneration.InstallDirDefaultId;
            }

            if (wProject.ActualInstallDirId.IsEmpty())
            {
                string dirLogicalPath = AutoAssignInstallDirId(wDirs, Compiler.AutoGeneration.InstallDirDefaultId);
                if (dirLogicalPath.IsNotEmpty())
                {
                    wProject.ActualInstallDirId = Compiler.AutoGeneration.InstallDirDefaultId;
                    wProject.AutoAssignedInstallDirPath = dirLogicalPath;
                }
            }

            if (wProject.ActualInstallDirId.IsEmpty())
            {
                wProject.ActualInstallDirId = wProject.GetLogicalInstallDir()?.Id;
            }

            // MSI doesn't allow absolute path to be assigned via name. Instead it requires it to be set via
            // SetProperty custom action. And what is even more weird the id of such a dir has to be public
            // (capitalized). Thus the id auto-assignment cannot be used as it creates non public id(s).
            var absolutePathDirs = wProject.AllDirs.Where(x => (!x.IsIdSet() || x.RawId.IsPublicWixProperty()) && x.Name.IsAbsolutePath()).ToArray();
            foreach (var item in absolutePathDirs)
                if (!item.Id.IsPublicWixProperty())
                    item.Id = $"TARGETDIR{absolutePathDirs.FindIndex(item) + 1}";


            foreach (Dir wDir in wDirs)
            {
                ProcessDirectory(wDir, wProject, featureComponents, defaultFeatureComponents, autoGeneratedComponents, dirs);
            }

            foreach (string dirPath in autogeneratedShortcutLocations.Keys)
            {
                Feature feature = autogeneratedShortcutLocations[dirPath];

                //be careful as some parts of the auto-generated director may already exist
                XElement existingParent = null;
                string dirToAdd = dirPath;
                string[] dirsToSearch = Dir.ToFlatPathTree(dirPath);
                foreach (string path in dirsToSearch)
                {
                    Dir d = wProject.FindDir(path);
                    if (d != null)
                    {
                        dirToAdd = dirPath.Substring(path.Length + 1);
                        existingParent = dirs.FindDirectory(path.ExpandWixEnvConsts());
                        break;
                    }
                }

                if (existingParent != null)
                {
                    Dir dir = new Dir(feature, dirToAdd);
                    ProcessDirectory(dir, wProject, featureComponents, defaultFeatureComponents, autoGeneratedComponents, existingParent);
                }
                else
                {
                    Dir dir = new Dir(feature, dirPath);
                    ProcessDirectory(dir, wProject, featureComponents, defaultFeatureComponents, autoGeneratedComponents, dirs);
                }
            }
        }

        static void ProcessRegKeys(Project wProject, Dictionary<Feature, List<string>> featureComponents, List<string> defaultFeatureComponents, XElement product)
        {
            //From Wix documentation it is not clear how to have RegistryKey outside of the Directory element
            //thus let's use the top level directory element for the stand alone registry collection
            if (wProject.RegValues.Length != 0)
            {
                var count = 0;
                var keyPathSet = false;
                foreach (RegValue regVal in wProject.RegValues)
                {
                    if (wProject.Platform.HasValue)
                    {
                        if (regVal.Win64.HasValue)
                        {
                            if (regVal.IsWin64 && !wProject.Is64Bit)
                                regVal.AttributesDefinition += ";Component:Bitness=always64";
                            else if (!regVal.IsWin64 && wProject.Is64Bit)
                                regVal.AttributesDefinition += ";Component:Bitness=always32";
                        }
                        else
                        {
                            if (wProject.Is64Bit)
                                regVal.AttributesDefinition += ";Component:Bitness=always64";
                        }
                    }
                    else //equivalent of wProject.Platform == Platform.x86 as MSI will be hosted by x86 msiexec.exe
                    {
                        if (regVal.IsWin64)
                            regVal.AttributesDefinition += ";Component:Bitness=always64";
                    }

                    count++;
                    string compId = wProject.ComponentId($"Registry.{count}");

                    //all registry of this level belong to the same component
                    if (regVal.ActualFeatures.Any())
                        featureComponents.Map(regVal.ActualFeatures, compId);
                    else
                        defaultFeatureComponents.Add(compId);

                    // WiX/MSI requires the registry to belong to directory
                    // (either by nesting XML element or by having Directory attribute in the registry key component)
                    // Instead of placing the reg component into the user dir element place it in the ProgramFiles
                    // dir element. This dir is not going to be created nor deleted during the installation.
                    // This technique is only used to satisfy WiX/MSI constraint for `Component.RegistryKey` to belong
                    // to the directory.
                    // GetTopLevelPermanentDir works slightly better than GetTopLevelDir because the registry key in this
                    // case belongs to a permanent dir but not to the user dir. It is still shocking to enclose an entity
                    // that is a system wide to the directory. But the containment is slightly better in this case.

                    bool is64 = regVal.Win64 ?? wProject.Is64Bit;

                    XElement topLevelDir = GetTopLevelPermanentDir(product, is64);

                    XElement comp = topLevelDir.AddElement(
                                                   new XElement("Component",
                                                       new XAttribute("Id", compId),
                                                       new XAttribute("Guid", WixGuid.NewGuid(compId))));

                    if (regVal.Condition != null)
                        comp.AddAttributes("Condition=" + regVal.Condition.ToXValue())
                            .AddAttributes(regVal.Condition.Attributes);

                    XElement regValEl;
                    XElement regKeyEl;
                    regKeyEl = comp.AddElement(
                            new XElement("RegistryKey",
                                new XAttribute("Root", regVal.Root),
                                regValEl = new XElement("RegistryValue")
                                               .SetAttribute("Id", regVal.Id)
                                               .SetAttribute("Type", regVal.RegTypeString)
                                               .SetAttribute("KeyPath", keyPathSet)
                                               .AddAttributes(regVal.Attributes)));

                    if (regVal.Permissions != null)
                        foreach (Permission permission in regVal.Permissions)
                            regValEl.AddElement(permission.ToXElement("Permission"));

                    if (!regVal.Key.IsEmpty())
                        regKeyEl.Add(new XAttribute("Key", regVal.Key));

                    string stringValue = regVal.RegValueString;
                    if (AutoElements.ExpandWixConstsInRegValue)
                        stringValue = regVal.RegValueString.ExpandWixEnvConsts().UnEscapeEnvars();

                    if (regValEl.Attribute("Type").Value == "multiString")
                    {
                        foreach (string line in stringValue.GetLines())
                        {
                            var lineEl = new XElement("MultiStringValue");
                            lineEl.SetAttribute("Value", line);
                            regValEl.Add(lineEl);
                        }
                    }
                    else
                        regValEl.Add(new XAttribute("Value", stringValue));

                    if (regVal.RegistryKeyAction != RegistryKeyAction.none)
                    {
                        regKeyEl.Add(new XAttribute("Action", regVal.RegistryKeyAction.ToString()));
                    }
                    if (regVal.ForceCreateOnInstall)
                    {
                        regKeyEl.Add(new XAttribute("ForceCreateOnInstall", regVal.ForceCreateOnInstall.ToYesNo()));
                    }
                    if (regVal.ForceDeleteOnUninstall)
                    {
                        regKeyEl.Add(new XAttribute("ForceDeleteOnUninstall", regVal.ForceDeleteOnUninstall.ToYesNo()));
                    }
                    if (regVal.Name != "")
                        regValEl.Add(new XAttribute("Name", regVal.Name));

                    keyPathSet = true;
                }
            }
        }

        static void ProcessLaunchConditions(Project project, XElement product)
        {
            //<Launch Condition="NETFRAMEWORK35=&quot;#1&quot;" Message="Please install .NET 3.5 first." />
            foreach (var condition in project.LaunchConditions)
                product.Add(new XElement("Launch",
                                new XAttribute("Condition", condition.Value),
                                new XAttribute("Message", condition.Message))
                                .AddAttributes(condition.Attributes));
        }

        internal static void ProcessWixVariables(WixProject project, XElement product)
        {
            foreach (var key in project.WixVariables.Keys)
            {
                product.AddElement("WixVariable", "Id=" + key + ";Value=" + project.WixVariables[key]);
            }
        }

        static void InsertWebSite(WebSite webSite, string dirID, XElement parent, Project project)
        {
            XNamespace ns = WixExtension.IIs.ToXNamespace();

            XElement xWebSite = parent.AddElement(new XElement(ns + "WebSite",
                                                       new XAttribute("Id", webSite.Id),
                                                       new XAttribute("Description", webSite.Description),
                                                       new XAttribute("Directory", dirID)));

            xWebSite.SetAttribute("WebApplication", webSite.WebApplication)
                    .AddAttributes(webSite.Attributes);



            var count = 0;
            foreach (WebSite.WebAddress address in webSite.Addresses)
            {
                count++;
                var compId = webSite.Id + "_Address" + count;
                XElement xAddress = xWebSite.AddElement(new XElement(ns + "WebAddress",
                                                            new XAttribute("Id", compId),
                                                            new XAttribute("IP", address.Address),
                                                            new XAttribute("Port", address.Port)));

                xAddress.AddAttributes(address.Attributes)
                        .SetAttribute("Header", address.Header);
            }

            if (webSite.Certificate != null)
            {
                var certificate = webSite.Certificate.ToXElement(WixExtension.IIs, "Certificate");

                if (!parent.FindAll("Certificate").Any(x => x.HasAttribute("Id", webSite.Certificate.Id)))
                {
                    parent.AddElement(certificate);

                    if (webSite.Certificate.File.IsNotEmpty())
                    {
                        // since user requested not to reference `Binary` element but use the file it is WixSharp responsibility to
                        // include the file as to the `Product` element as a new `Binary` XML element.

                        var binary = new Binary(webSite.Certificate.File); // will generate a correct unique Id

                        string bynaryPath = project.SourceBaseDir.PathJoin(binary.Name);

                        parent.Document
                              .FindFirst(Compiler.ProductElementName)
                              .AddElement("Binary", $"Id={binary.Id};SourceFile={bynaryPath}");

                        certificate.SetAttribute(CABinarykeyAttrName, binary.Id);
                    }
                }

                xWebSite.AddElement(ns + "CertificateRef", $"Id={webSite.Certificate.Id}");
            }
        }

        static void InsertIISElements(XElement dirItem, XElement component, IISVirtualDir[] wVDirs, Project project)
        {
            if (!wVDirs.Any())
                return;

            //http://ranjithk.com/2009/12/17/automating-web-deployment-using-windows-installer-xml-wix/

            XNamespace ns = WixExtension.IIs.ToXNamespace();

            string dirID = dirItem.Attribute("Id").Value;
            var xProduct = component.Parent(Compiler.ProductElementName);

            var uniqueProductWebSites = new List<WebSite>();
            var uniqueComponentWebSites = new List<WebSite>();

            bool wasInserted = false;
            foreach (IISVirtualDir wVDir in wVDirs)
            {
                wasInserted = true;

                XElement xWebApp;
                var xVDir = component.AddElement(new XElement(ns + "WebVirtualDir",
                                                     new XAttribute("Id", wVDir.Id),
                                                     new XAttribute("Alias", wVDir.Alias.IsEmpty() ? wVDir.Name : wVDir.Alias),
                                                     new XAttribute("Directory", dirID),
                                                     new XAttribute("WebSite", wVDir.WebSite.Id),
                                                     xWebApp = new XElement(ns + "WebApplication",
                                                                   new XAttribute("Id", wVDir.AppName.Expand() + "WebApplication"),
                                                                   new XAttribute("Name", wVDir.AppName))));
                xVDir.AddAttributes(wVDir.Attributes);

                xWebApp.SetAttribute("AllowSessions", wVDir.AllowSessions)
                       .SetAttribute("Buffer", wVDir.Buffer)
                       .SetAttribute("ClientDebugging", wVDir.ClientDebugging)
                       .SetAttribute("DefaultScript", wVDir.DefaultScript)
                       .SetAttribute("Isolation", wVDir.Isolation)
                       .SetAttribute("ParentPaths", wVDir.ParentPaths)
                       .SetAttribute("ScriptTimeout", wVDir.ScriptTimeout)
                       .SetAttribute("ServerDebugging", wVDir.ServerDebugging)
                       .SetAttribute("SessionTimeout", wVDir.SessionTimeout);

                //do not create WebSite on IIS but install WebApp into existing
                if (!wVDir.WebSite.InstallWebSite)
                {
                    if (!uniqueProductWebSites.Contains(wVDir.WebSite))
                        uniqueProductWebSites.Add(wVDir.WebSite);
                }
                else
                {
                    if (!uniqueComponentWebSites.Contains(wVDir.WebSite))
                    {
                        InsertWebSite(wVDir.WebSite, dirID, component, project);
                        uniqueComponentWebSites.Add(wVDir.WebSite);
                    }
                }

                if (wVDir.WebAppPool != null)
                {
                    var id = wVDir.Name.Expand() + "_AppPool";

                    xWebApp.Add(new XAttribute("WebAppPool", id));

                    var xAppPool = component.AddElement(new XElement(ns + "WebAppPool",
                                                            new XAttribute("Id", id),
                                                            new XAttribute("Name", wVDir.WebAppPool.Name)));

                    xAppPool.AddAttributes(wVDir.WebAppPool.Attributes);
                }

                if (wVDir.WebDirProperties != null)
                {
                    var propId = wVDir.Name.Expand() + "_WebDirProperties";

                    var xDirProp = xProduct.AddElement(new XElement(ns + "WebDirProperties",
                                                           new XAttribute("Id", propId)));

                    xDirProp.AddAttributes(wVDir.WebDirProperties.Attributes);

                    xVDir.Add(new XAttribute("DirProperties", propId));
                }
            }

            foreach (WebSite webSite in uniqueProductWebSites)
            {
                InsertWebSite(webSite, dirID, xProduct, project);
            }

            if (wasInserted)
            {
                project.Include(WixExtension.IIs);
            }
        }

        static void ProcessProperties(Project wProject, XElement product, Dictionary<Feature, List<string>> featureComponents)
        {
            var properties = wProject.Properties.ToList();

            if (wProject.RebootSupressing != null)
            {
                properties.Add(new Property("REBOOT", wProject.RebootSupressing.ToString()));
            }

            if (wProject.MajorUpgrade != null && wProject.ReinstallMode.IsNotEmpty())
            {
                properties.Add(new Property("REINSTALLMODE", wProject.ReinstallMode));
            }

            foreach (var prop in properties)
            {
                if (prop is PropertyRef)
                {
                    var propRef = (prop as PropertyRef);

                    if (propRef.Id.IsEmpty())
                        throw new Exception("'" + typeof(PropertyRef).Name + "'.Id must be set before compiling the project.");

                    product.Add(new XElement("PropertyRef",
                                    new XAttribute("Id", propRef.Id)));
                }
                else if (prop is RegValueProperty)
                {
                    var rvProp = (prop as RegValueProperty);

                    XElement RegistrySearchElement;
                    XElement xProp = product.AddElement(
                                new XElement("Property",
                                    new XAttribute("Id", rvProp.Name),
                                    RegistrySearchElement = new XElement("RegistrySearch",
                                        new XAttribute("Id", rvProp.Name + "_RegSearch"),
                                        new XAttribute("Root", rvProp.Root),
                                        new XAttribute("Key", rvProp.Key),
                                        new XAttribute("Type", "raw")
                                        ))
                                    .AddAttributes(rvProp.Attributes));

                    if (!rvProp.Value.IsEmpty())
                        xProp.Add(new XAttribute("Value", rvProp.Value));

                    if (rvProp.EntryName != "")
                        RegistrySearchElement.SetAttribute("Name", rvProp.EntryName);

                    if (rvProp.win64_SetByUser)
                    {
                        RegistrySearchElement.SetAttribute("Bitness", rvProp.Win64 ? "always64" : "always32");
                    }
                    else
                    {
                        // otherwise `default` - the same as package x64/x32 
                        if (wProject.Is64Bit)
                            RegistrySearchElement.SetAttribute("Bitness", "always64");
                        else
                            RegistrySearchElement.SetAttribute("Bitness", rvProp.Win64 ? "always64" : "always32");
                    }
                }
                else
                {
                    var parentElement = product.AddElement(new XElement("Property")
                                               .SetAttribute("Id", prop.Name)
                                               .SetAttribute("Value", prop.Value)
                                               .SetAttribute("Hidden", prop.Hidden)
                                               .SetAttribute("Secure", prop.Secure)
                                               .AddAttributes(prop.Attributes));

                    if (prop.GenericItems.Any())
                    {
                        var context = new ProcessingContext
                        {
                            Project = wProject,
                            Parent = prop,
                            XParent = parentElement,
                            FeatureComponents = featureComponents
                        };

                        foreach (IGenericEntity item in prop.GenericItems)
                            item.Process(context);
                    }
                }
            }
        }

        static void ProcessBinaries(Project wProject, XElement product)
        {
            foreach (var bin in wProject.Binaries)
            {
                string bynaryKey = bin.Id;
                string bynaryPath = Utils.PathCombine(wProject.SourceBaseDir, bin.Name);

                if (bin is EmbeddedAssembly)
                {
                    var asmBin = bin as EmbeddedAssembly;

                    bynaryPath = asmBin.Name.PathChangeDirectory(wProject.OutDir.PathGetFullPath())
                                            .PathChangeExtension(".CA.dll");

                    PackageManagedAsm(asmBin.Name, bynaryPath, asmBin.RefAssemblies.Concat(wProject.DefaultRefAssemblies).Distinct().ToArray(), wProject.OutDir, wProject.CustomActionConfig, signing: wProject.ContentSigningSignature);
                }

                product.Add(new XElement("Binary",
                                new XAttribute("Id", bynaryKey),
                                new XAttribute("SourceFile", bynaryPath))
                                .AddAttributes(bin.Attributes));
            }
        }

        /// <summary>
        /// Processes the custom actions.
        /// </summary>
        /// <param name="wProject">The w project.</param>
        /// <param name="product">The product.</param>
        /// <exception cref="System.Exception">Step.PreviousAction is specified for the very first 'Custom Action'.\nThere cannot be any previous action as it is the very first one in the sequence.</exception>
        static void ProcessCustomActions(Project wProject, XElement product)
        {
            foreach (Action wAction in wProject.Actions)
            {
                object wActionCondition = wAction.ToXmlCondition().Value;

                string step = wAction.Step.ToString();
                string roollbackActionId = wAction.Id + "_Rollback";

                string lastActionName = null;
                var existingSequence = product.FindAll(wAction.Sequence.ToString()).FirstOrDefault();
                if (existingSequence != null)
                    lastActionName = existingSequence.FindAll("Custom")
                                                     .Select(x => x.Attribute("Action").Value)
                                                     .LastOrDefault();

                if (wAction.When == When.After && wAction.Step == Step.PreviousAction)
                {
                    step = lastActionName ?? throw new Exception("Step.PreviousAction is specified for the very first 'Custom Action'.\nThere cannot be any previous action as it is the very first one in the sequence.");
                }
                else if (wAction.When == When.After && wAction.Step == Step.PreviousActionOrInstallFinalize)
                    step = lastActionName ?? Step.InstallFinalize.ToString();
                else if (wAction.When == When.After && wAction.Step == Step.PreviousActionOrInstallInitialize)
                    step = lastActionName ?? Step.InstallInitialize.ToString();

                List<XElement> sequences = new List<XElement>();

                if (wAction.Sequence != Sequence.NotInSequence)
                {
                    foreach (var item in wAction.Sequence.GetValues())
                        sequences.Add(product.SelectOrCreate(item));
                }

                var uis = new List<XElement> { product.SelectOrCreate("UI") };

                XAttribute sequenceNumberAttr = wAction.SequenceNumber.HasValue ?
                                                    new XAttribute("Sequence", wAction.SequenceNumber.Value) :
                                                    new XAttribute(wAction.When.ToString(), step);

                if (wAction.ProgressText.IsNotEmpty())
                {
                    uis.ForEach(ui =>
                        ui.Add(new XElement("ProgressText",
                            new XAttribute("Message", wAction.ProgressText),
                            new XAttribute("Action", wAction.Id))));
                }
                if (wAction.RollbackProgressText != null)
                {
                    uis.ForEach(ui =>
                        ui.Add(new XElement("ProgressText",
                            new XAttribute("Message", wAction.RollbackProgressText),
                            new XAttribute("Action", roollbackActionId))));
                }

                if (wAction is SetPropertyAction wSetPropAction)
                {
                    var actionId = wSetPropAction.Id;

                    product.AddElement(
                        new XElement("CustomAction",
                            new XAttribute("Id", actionId),
                            new XAttribute("Property", wSetPropAction.PropName),
                            new XAttribute("Value", wSetPropAction.Value))
                            .SetAttribute("Return", wAction.Return)
                            .SetAttribute("Impersonate", wAction.Impersonate)
                            .SetAttribute("Execute", wAction.Execute)
                            .AddAttributes(wAction.Attributes));

                    sequences.ForEach(sequence =>
                        sequence.Add(new XElement("Custom",
                                         new XAttribute("Condition", wActionCondition),
                                         new XAttribute("Action", actionId),
                                         sequenceNumberAttr)));
                }
                else if (wAction is ScriptFileAction)
                {
                    var wScriptAction = (ScriptFileAction)wAction;

                    sequences.ForEach(sequence =>
                         sequence.Add(new XElement("Custom",
                                          new XAttribute("Condition", wActionCondition),
                                          new XAttribute("Action", wAction.Id),
                                          sequenceNumberAttr)));

                    product.Add(new XElement("Binary",
                                    new XAttribute("Id", wAction.Name.Expand() + "_File"),
                                    new XAttribute("SourceFile", Utils.PathCombine(wProject.SourceBaseDir, wScriptAction.ScriptFile))));

                    product.Add(new XElement("CustomAction",
                                    new XAttribute("Id", wAction.Id),
                                    new XAttribute(CABinarykeyAttrName, wAction.Name.Expand() + "_File"),
                                    new XAttribute("VBScriptCall", wScriptAction.Procedure))
                                    .SetAttribute("Return", wAction.Return)
                                    .SetAttribute("Impersonate", wAction.Impersonate)
                                    .SetAttribute("Execute", wAction.Execute)
                                    .AddAttributes(wAction.Attributes));

                    if ((wScriptAction.Execute == Execute.deferred) && wScriptAction.Rollback.IsNotEmpty())
                    {
                        sequences.ForEach(sequence =>
                            sequence.Add(new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", roollbackActionId),
                                new XAttribute("Before", wAction.Id))));

                        product.AddElement(new XElement("CustomAction",
                                new XAttribute("Id", roollbackActionId),
                                new XAttribute(CABinarykeyAttrName, wAction.Name.Expand() + "_File"),
                                new XAttribute("VBScriptCall", wScriptAction.RollbackArg))
                            .SetAttribute("Return", wAction.Return)
                            .SetAttribute("Impersonate", wAction.Impersonate)
                            .SetAttribute("Execute", Execute.rollback)
                            .AddAttributes(wAction.Attributes));
                    }
                }
                else if (wAction is ScriptAction)
                {
                    var wScriptAction = (ScriptAction)wAction;

                    sequences.ForEach(sequence =>
                        sequence.Add(new XElement("Custom",
                                       wAction.ToXmlCondition(),
                                       new XAttribute("Action", wAction.Id),
                                       sequenceNumberAttr)));

                    product.Add(new XElement("CustomAction",
                                    new XCData(wScriptAction.Code),
                                    new XAttribute("Id", wAction.Id),
                                    new XAttribute("Script", "vbscript"))
                                    .SetAttribute("Return", wAction.Return)
                                    .SetAttribute("Impersonate", wAction.Impersonate)
                                    .SetAttribute("Execute", wAction.Execute)
                                    .AddAttributes(wAction.Attributes));

                    if ((wScriptAction.Execute == Execute.deferred) && wScriptAction.Rollback.IsNotEmpty())
                    {
                        sequences.ForEach(sequence =>
                            sequence.Add(new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", roollbackActionId),
                                new XAttribute("Before", wAction.Id))));

                        product.AddElement(new XElement("CustomAction",
                                new XCData(wScriptAction.RollbackArg),
                                new XAttribute("Id", roollbackActionId))
                            .SetAttribute("Return", wAction.Return)
                            .SetAttribute("Impersonate", wAction.Impersonate)
                            .SetAttribute("Execute", Execute.rollback)
                            .AddAttributes(wAction.Attributes));
                    }
                }
                else if (wAction is ManagedAction)
                {
                    var wManagedAction = (ManagedAction)wAction;
                    var asmFile = Utils.PathCombine(wProject.SourceBaseDir, wManagedAction.ActionAssembly);
                    var packageFile = asmFile.PathChangeDirectory(wProject.OutDir.PathGetFullPath())
                                             .PathChangeExtension(".CA.dll");

                    if (wManagedAction.IsNetCore)
                    {
                        // Only generate the accurate AOT compiled assembly (native CA dll) but do not compile this assembly.
                        // Why? Because the user may already supply this assembly under its expected name thus we only need to
                        // compile the assembly if it does not exist yet (a few lines below). 
                        packageFile = asmFile.ConvertToAotAssembly(previewOnly: true, buildParameters: Compiler.AutoGeneration.AotBuildParameters);
                    }

                    var existingBinary = product.Descendants("Binary")
                                            .Where(e => e.Attribute("SourceFile").Value == packageFile)
                                            .FirstOrDefault();

                    string bynaryKey;

                    if (existingBinary == null)
                    {
                        string nativeCAdll = asmFile;

                        if (wManagedAction.IsNetCore)
                        {
                            nativeCAdll = asmFile.ConvertToAotAssembly(buildParameters: Compiler.AutoGeneration.AotBuildParameters);
                        }
                        else if (wManagedAction.CreateInteropWrapper)
                        {
                            var assemblies = wManagedAction
                                .RefAssemblies
                                .Concat(wProject.DefaultRefAssemblies)
                                .Distinct().ToArray();

                            PackageManagedAsm(
                                asmFile,
                                packageFile,
                                assemblies,
                                wProject.OutDir,
                                wProject.CustomActionConfig,
                                wProject.Platform,
                                false,
                                signing: wProject.ContentSigningSignature);

                            nativeCAdll = packageFile;
                        }

                        bynaryKey = wAction.Name.Expand() + "_File";
                        product.Add(new XElement("Binary",
                                        new XAttribute("Id", bynaryKey),
                                        new XAttribute("SourceFile", nativeCAdll)));
                    }
                    else
                    {
                        bynaryKey = existingBinary.Attribute("Id").Value;
                    }

                    //map managed action properties
                    if ((wManagedAction.Execute == Execute.deferred || wManagedAction.Execute == Execute.commit || wManagedAction.Execute == Execute.rollback) && wManagedAction.UsesProperties != null)
                    {
                        string mapping = wManagedAction.ExpandAllUsedProperties();

                        if (!mapping.IsEmpty())
                        {
                            var setPropValuesId = "Set_" + wAction.Id + "_Props";

                            product.Add(new XElement("CustomAction",
                                            new XAttribute("Id", setPropValuesId),
                                            new XAttribute("Property", wAction.Id),
                                            new XAttribute("Value", mapping)));

                            var stepAttr = wAction.SequenceNumber.HasValue ?
                                                    new XAttribute("Sequence", wAction.SequenceNumber.Value) :
                                                    new XAttribute("After", "InstallInitialize");


                            if (AutoElements.ScheduleDeferredActionsAfterTunnelingTheirProperties || wAction.RawId == nameof(ManagedProjectActions.WixSharp_AfterInstall_Action))
                            {
                                // Inject fetching properties CA just before the deferred action AfterInstrallEventHandler.
                                // This might be a good practice to do for all deferred actions. However it's hard to predict the
                                // full impact of the auto-change for all user defined deferred actions. So limit the technique
                                // to the deferred actions defined by WixSharp itself only.

                                stepAttr = sequenceNumberAttr;

                                if (setPropValuesId == "Set_WixSharp_Load_Action_Props" &&
                                    sequenceNumberAttr.LocalName() == "Before" &&
                                    sequenceNumberAttr.Value == "AppSearch")
                                {
                                    // Set_[action]_Props for it is always scheduled just before [action] itself.
                                    // However WixSharp_Load_Action is scheduled so early in the sequence (before:AppSearch)
                                    // that any Set_[action]_Props becomes illegal and the compiler exception asking to schedule it
                                    // later in the sequence. Thus if WixSharp_Load_Action is deferred then move it to the
                                    // before:InstallFiles
                                    // This is the fix for #1810
                                    stepAttr.Value = "InstallFiles";
                                }

                                sequenceNumberAttr = new XAttribute("After", setPropValuesId);
                            }

                            sequences.ForEach(sequence =>
                                sequence.Add(new XElement("Custom",
                                                 new XAttribute("Action", setPropValuesId),
                                                 stepAttr)));
                        }
                    }

                    sequences.ForEach(item => item.Add(new XElement("Custom",
                                                           new XAttribute("Condition", wActionCondition),
                                                           new XAttribute("Action", wAction.Id),
                                                           sequenceNumberAttr)));

                    product.Add(new XElement("CustomAction",
                                    new XAttribute("Id", wAction.Id),
                                    new XAttribute(CABinarykeyAttrName, bynaryKey),
                                    new XAttribute("DllEntry", wManagedAction.MethodName))
                                    .SetAttribute("Return", wAction.Return)
                                    .SetAttribute("Impersonate", wAction.Impersonate)
                                    .SetAttribute("Execute", wAction.Execute)
                                    .AddAttributes(wAction.Attributes));

                    if ((wManagedAction.Execute == Execute.deferred) && wManagedAction.Rollback.IsNotEmpty())
                    {
                        string mapping = wManagedAction.RollbackArg == null
                            ? wManagedAction.ExpandAllUsedProperties()
                            : wManagedAction.RollbackExpandAllUsedProperties();
                        if (!mapping.IsEmpty())
                        {
                            product.Add(new XElement("SetProperty",
                                new XAttribute("Id", roollbackActionId),
                                new XAttribute("Before", roollbackActionId),
                                new XAttribute("Sequence", "execute"),
                                new XAttribute("Value", mapping)));
                        }

                        sequences.ForEach(sequence =>
                            sequence.Add(new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", roollbackActionId),
                                new XAttribute("Before", wAction.Id))));

                        product.AddElement(
                            new XElement("CustomAction",
                                    new XAttribute("Id", roollbackActionId),
                                    new XAttribute(CABinarykeyAttrName, bynaryKey),
                                    new XAttribute("DllEntry", wManagedAction.Rollback))
                                .SetAttribute("Return", wAction.Return)
                                .SetAttribute("Impersonate", wAction.Impersonate)
                                .SetAttribute("Execute", Execute.rollback)
                                .AddAttributes(wAction.Attributes));
                    }
                }
                else if (wAction is CustomActionRef)
                {
                    var wCustomActionRef = (CustomActionRef)wAction;
                    sequences.ForEach(sequence =>
                        sequence.Add(new XElement("Custom",
                            new XAttribute("Condition", wActionCondition),
                            new XAttribute("Action", wCustomActionRef.Id),
                            new XAttribute(wCustomActionRef.When.ToString(), wCustomActionRef.Step))));

                    product.Add(new XElement("CustomActionRef",
                        new XAttribute("Id", wCustomActionRef.Id)));
                }
                else if (wAction is WixQuietExecAction)
                {
                    var quietExecAction = (WixQuietExecAction)wAction;
                    var cmdLineActionId = wAction.Id;
                    var setCmdLineActionId = "Set_" + cmdLineActionId;

                    product.AddElement(
                        new XElement("CustomAction")
                            .SetAttribute("Id", setCmdLineActionId)
                            .SetAttribute("Property", wAction.Execute == Execute.immediate ? quietExecAction.CommandLineProperty : wAction.Id)
                            .SetAttribute("Value", "\"" + quietExecAction.AppPath.ExpandCommandPath() + "\" " + quietExecAction.Args.ExpandCommandPath())
                            .AddAttributes(quietExecAction.Attributes));

                    product.AddElement(
                        new XElement("CustomAction")
                            .SetAttribute("Id", cmdLineActionId)
                            .SetAttribute(CABinarykeyAttrName, "Wix4UtilCA_X86") // in WiX4: WixCA -> WixUiCa_X86 (just a guess)
                            .SetAttribute("DllEntry", quietExecAction.ActionName)
                            .SetAttribute("Return", wAction.Return)
                            .SetAttribute("Impersonate", wAction.Impersonate)
                            .SetAttribute("Execute", wAction.Execute)
                            .AddAttributes(wAction.Attributes));

                    sequences.ForEach(sequence =>
                        sequence.Add(
                            new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", setCmdLineActionId),
                                sequenceNumberAttr)));

                    sequences.ForEach(sequence =>
                        sequence.Add(
                            new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", cmdLineActionId),
                                new XAttribute("After", setCmdLineActionId))));

                    if ((quietExecAction.Execute == Execute.deferred) && quietExecAction.Rollback.IsNotEmpty())
                    {
                        product.Add(new XElement("SetProperty",
                            new XAttribute("Id", roollbackActionId),
                            new XAttribute("Before", roollbackActionId),
                            new XAttribute("Sequence", "execute"),
                            new XAttribute("Value", "\"" + quietExecAction.Rollback.ExpandCommandPath() + "\" " + quietExecAction.RollbackArg.ExpandCommandPath())));

                        sequences.ForEach(sequence =>
                            sequence.Add(new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", roollbackActionId),
                                new XAttribute("Before", wAction.Id))));

                        product.AddElement(
                            new XElement("CustomAction")
                                .SetAttribute("Id", roollbackActionId)
                                .SetAttribute(CABinarykeyAttrName, "WixUiCa_X86") // in WiX4: WixCA -> WixUiCa_X86 (just a guess)
                                .SetAttribute("DllEntry", quietExecAction.ActionName)
                                .SetAttribute("Return", wAction.Return)
                                .SetAttribute("Impersonate", wAction.Impersonate)
                                .SetAttribute("Execute", Execute.rollback)
                                .AddAttributes(wAction.Attributes));
                    }

                    wProject.Include(WixExtension.Util);
                }
                else if (wAction is InstalledFileAction)
                {
                    var fileAction = (InstalledFileAction)wAction;

                    sequences.ForEach(sequence =>
                        sequence.Add(
                            new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", wAction.Id),
                                sequenceNumberAttr)));

                    product.AddElement(
                        new XElement("CustomAction",
                                new XAttribute("Id", wAction.Id),
                                new XAttribute("ExeCommand", fileAction.Args.ExpandCommandPath()),
                                new XAttribute("FileRef", fileAction.Key))
                            .SetAttribute("Return", wAction.Return)
                            .SetAttribute("Impersonate", wAction.Impersonate)
                            .SetAttribute("Execute", wAction.Execute)
                            .AddAttributes(wAction.Attributes));

                    if ((fileAction.Execute == Execute.deferred) && fileAction.Rollback.IsNotEmpty())
                    {
                        sequences.ForEach(sequence =>
                            sequence.Add(new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", roollbackActionId),
                                new XAttribute("Before", wAction.Id))));

                        product.AddElement(
                            new XElement("CustomAction",
                                    new XAttribute("Id", roollbackActionId),
                                    new XAttribute("ExeCommand", fileAction.RollbackArg == null
                                        ? fileAction.Args.ExpandCommandPath()
                                        : fileAction.RollbackArg.ExpandCommandPath()),
                                    new XAttribute("FileRef", fileAction.Rollback))
                                .SetAttribute("Return", wAction.Return)
                                .SetAttribute("Impersonate", wAction.Impersonate)
                                .SetAttribute("Execute", Execute.rollback)
                                .AddAttributes(wAction.Attributes));
                    }
                }
                else if (wAction is BinaryFileAction)
                {
                    var binaryAction = (BinaryFileAction)wAction;

                    sequences.ForEach(sequence =>
                        sequence.Add(
                            new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", wAction.Id),
                                sequenceNumberAttr)));

                    product.AddElement(
                        new XElement("CustomAction",
                                new XAttribute("Id", wAction.Id),
                                new XAttribute("ExeCommand", binaryAction.Args.ExpandCommandPath()),
                                new XAttribute(CABinarykeyAttrName, binaryAction.Key))
                            .SetAttribute("Return", wAction.Return)
                            .SetAttribute("Impersonate", wAction.Impersonate)
                            .SetAttribute("Execute", wAction.Execute)
                            .AddAttributes(wAction.Attributes));

                    if ((binaryAction.Execute == Execute.deferred) && binaryAction.Rollback.IsNotEmpty())
                    {
                        sequences.ForEach(sequence =>
                            sequence.Add(new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", roollbackActionId),
                                new XAttribute("Before", wAction.Id))));

                        product.AddElement(
                            new XElement("CustomAction",
                                    new XAttribute("Id", roollbackActionId),
                                    new XAttribute("ExeCommand", binaryAction.RollbackArg.IsEmpty()
                                        ? binaryAction.Args.ExpandCommandPath()
                                        : binaryAction.RollbackArg.ExpandCommandPath()),
                                new XAttribute(CABinarykeyAttrName, binaryAction.Rollback))
                                .SetAttribute("Return", wAction.Return)
                                .SetAttribute("Impersonate", wAction.Impersonate)
                                .SetAttribute("Execute", Execute.rollback)
                                .AddAttributes(wAction.Attributes));
                    }
                }
                else if (wAction is PathFileAction)
                {
                    var fileAction = (PathFileAction)wAction;

                    sequences.ForEach(sequence =>
                        sequence.Add(
                            new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", wAction.Id),
                                sequenceNumberAttr)));

                    var actionElement = product.AddElement(
                        new XElement("CustomAction",
                                new XAttribute("Id", wAction.Id),
                                new XAttribute("ExeCommand", "\"" + fileAction.AppPath.ExpandCommandPath() + "\" " + fileAction.Args.ExpandCommandPath()))
                            .SetAttribute("Return", wAction.Return)
                            .SetAttribute("Impersonate", wAction.Impersonate)
                            .SetAttribute("Execute", wAction.Execute)
                            .AddAttributes(fileAction.Attributes));

                    Dir installedDir = Array.Find(wProject.Dirs, (x) => x.Name == fileAction.WorkingDir);
                    actionElement.Add(installedDir != null
                        ? new XAttribute("Directory", installedDir.Id)
                        : new XAttribute("Directory", fileAction.WorkingDir.Expand()));

                    if ((fileAction.Execute == Execute.deferred) && fileAction.Rollback.IsNotEmpty())
                    {
                        sequences.ForEach(sequence =>
                            sequence.Add(new XElement("Custom",
                                new XAttribute("Condition", wActionCondition),
                                new XAttribute("Action", roollbackActionId),
                                new XAttribute("Before", wAction.Id))));

                        var rollbackActionElement = product.AddElement(
                            new XElement("CustomAction",
                                    new XAttribute("Id", roollbackActionId),
                                    new XAttribute("ExeCommand", fileAction.RollbackArg.IsEmpty()
                                        ? "\"" + fileAction.AppPath.ExpandCommandPath() + "\" " + fileAction.Args.ExpandCommandPath()
                                        : "\"" + fileAction.AppPath.ExpandCommandPath() + "\" " + fileAction.RollbackArg.ExpandCommandPath()))
                                .SetAttribute("Return", wAction.Return)
                                .SetAttribute("Impersonate", wAction.Impersonate)
                                .SetAttribute("Execute", Execute.rollback)
                                .AddAttributes(fileAction.Attributes));

                        rollbackActionElement.Add(installedDir != null
                            ? new XAttribute("Directory", installedDir.Id)
                            : new XAttribute("Directory", fileAction.WorkingDir.Expand()));
                    }
                }
            }
        }

        internal static string FindClientAssemblyInCallStack()
        {
            var st = new StackTrace(false);
            for (int i = 0; i < st.FrameCount; i++)
            {
                var asm = st.GetFrame(i).GetMethod().DeclaringType?.Assembly;
                if (asm != System.Reflection.Assembly.GetExecutingAssembly())
                {
                    if (asm == "".GetType().Assembly || asm.FullName.StartsWith("mscorlib."))
                    {
                        // being executed as a release-compiled script
                        string compiledScriptLocation = Environment.GetEnvironmentVariables().Keys.Cast<string>().FirstOrDefault(x => x.StartsWith("location:"));

                        return Environment.GetEnvironmentVariable("EntryScriptAssembly")
                            ?? Environment.GetEnvironmentVariable(compiledScriptLocation)
                            ;
                    }
                    else
                        return asm.GetLocation();
                }
            }
            return null;
        }

        static string clientAssembly;

        /// <summary>
        /// Path to the <c>WixSharp.dll</c> client assembly. Typically it is the Wix# setup script assembly.
        /// <para>This value is used to resolve <c>%this%</c> of the <see cref="ManagedAction"/>. If this value is not specified
        /// <see cref="Compiler"/> will set it to the caller of its <c>Build</c> method.</para>
        /// </summary>
        static public string ClientAssembly
        {
            get { return clientAssembly; }
            set
            {
                //Debug.Assert(false);
                clientAssembly = value;

                var isCSScriptExecution = Environment.GetEnvironmentVariable("CSScriptRuntime") != null;

                if (isCSScriptExecution && clientAssembly.EndsWith("cscs.exe", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                        try
                        {
                            var attr = (System.Reflection.AssemblyDescriptionAttribute)item.GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false).FirstOrDefault();
                            if (attr != null && IO.File.Exists(attr.Description))
                            {
                                clientAssembly = item.Location;
                                break;
                            }
                        }
                        catch { }
                }
            }
        }

        /// <summary>
        /// Flag indicating whether to include PDB file of the assembly implementing ManagedCustomAction into MSI.  Default value is <c>False</c>.
        /// <para>If set to <c>false</c> PDB file will not be included and debugging of such CustomAction will not be possible.</para>
        /// </summary>
        static public bool IgnoreClientAssemblyPDB = false;



        /// <summary>
        /// Builds the batch file for packaging the assembly containing managed CA or UI.
        /// </summary>
        /// <param name="asm">The assembly to be packaged.</param>
        /// <param name="nativeDll">The package file (native DLL) to be build.</param>
        /// <param name="refAssemblies">The referenced assemblies.</param>
        /// <param name="outDir">The out dir.</param>
        /// <param name="configFilePath">The app config file path.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="embeddedUI">if set to <c>true</c> the assembly as an 'EmbeddedUI' assembly.</param>
        /// <param name="signature">The signing interface for file signing.</param>
        /// <returns>
        /// Batch file path.
        /// </returns>
        static public string BuildPackageAsmCmd(string asm, string nativeDll = null, string[] refAssemblies = null, string outDir = null, string configFilePath = null, Platform? platform = null, bool embeddedUI = false, IDigitalSignature signature = null)
        {
            string batchFile = IO.Path.Combine(outDir, "Build_CA_DLL.cmd");

            PackageManagedAsm(asm,
                              nativeDll ?? asm.PathChangeExtension(".CA.dll"),
                              refAssemblies ?? new string[0],
                              outDir ?? asm.PathGetDirName(),
                              configFilePath,
                              platform,
                              embeddedUI,
                              batchFile,
                              signature);
            return batchFile;
        }
        /// <summary>
        /// Packages the assembly containing managed CA or UI.
        /// </summary>
        /// <param name="asm">The assembly to be packaged.</param>
        /// <param name="nativeDll">The package file (native DLL) to be build.</param>
        /// <param name="refAssemblies">The referenced assemblies.</param>
        /// <param name="outDir">The out dir.</param>
        /// <param name="configFilePath">The app config file path.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="embeddedUI">if set to <c>true</c> the assembly as an 'EmbeddedUI' assembly.</param>
        /// <param name="signature">The signing.</param>
        /// <returns>
        /// Package file path.
        /// </returns>
        static public string BuildPackageAsm(string asm, string nativeDll = null, string[] refAssemblies = null, string outDir = null, string configFilePath = null, Platform? platform = null, bool embeddedUI = false, IDigitalSignature signature = null)
        {
            nativeDll = nativeDll ?? IO.Path.ChangeExtension(asm, ".CA.dll");

            PackageManagedAsm(asm, nativeDll ?? IO.Path.ChangeExtension(asm, ".CA.dll"), refAssemblies ?? new string[0], outDir ?? Environment.CurrentDirectory, configFilePath, platform, embeddedUI, signing: signature);
            return IO.Path.GetFullPath(nativeDll);
        }

        static void PackageManagedAsm(string asm, string nativeDll, string[] refAssemblies, string outDir, string configFilePath, Platform? platform = null, bool embeddedUI = false, string batchFile = null, IDigitalSignature signing = null)
        {
            bool is64 = false;
            if (platform.HasValue && platform.Value == Platform.x64)
                is64 = true;

            string makeSfxCA = WixTools.MakeSfxCA;
            string sfxcaDll = WixTools.SfxCAFor(is64);
            string dtfWinInstaller = WixTools.DtfWindowsInstaller;

            outDir = IO.Path.GetFullPath(outDir);

            var outDll = IO.Path.GetFullPath(nativeDll);
            var asmFile = IO.Path.GetFullPath(asm);

            var clientAsmPath = Utils.ResolveClientAsm();

            if (asmFile.EndsWith("%this%"))
                asmFile = clientAsmPath;
            else
                asmFile = asmFile.IsolateAsm();

            var requiredAsms = new List<string>(refAssemblies);


            if (Compiler.AutoGeneration.AddManagedCustomActionDependencies)
            {
#if !NETCORE
                string[] dependencies = (string[])Utils.ExecuteInTempDomain<AsmReflector>(asmReflector => asmReflector.GetRefAssemblies(asmFile));

                // dependencies are loaded in the temp domain and their location may not be original (preferred). IE in the temp folder
                // so we need to resolve them to the original locations if possible
                var localAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                               .Select(x =>
                                               {
                                                   try
                                                   {
                                                       return x.Location;
                                                   }
                                                   catch
                                                   {
                                                       return null;
                                                   }
                                               })
                                               .Where(x => x.IsNotEmpty());

                for (int i = 0; i < dependencies.Length; i++)
                {
                    var localAsmPath = localAssemblies.Where(x => x.PathGetFileName() == dependencies[i].PathGetFileName()).FirstOrDefault();
                    if (localAsmPath != null)
                        dependencies[i] = localAsmPath;
                }

                dependencies.Where(x => !requiredAsms.Contains(x))
                            .ForEach(x => requiredAsms.Add(x));
#else
                throw new NotImplementedException($"The option {nameof(Compiler.AutoGeneration.AddManagedCustomActionDependencies)} is not implemented on .NET Core");
#endif
            }

            if (refAssemblies.Any(x => x.PathGetFileName() == "WixSharp.UI.dll") &&
                !refAssemblies.Any(x => x.PathGetFileName() == "WixToolset.Mba.Core.dll"))
            {
                string localWixToolsetMbaCore = dtfWinInstaller.PathChangeFileName("WixToolset.Mba.Core.dll");

                if (localWixToolsetMbaCore.PathExists())
                {
                    requiredAsms.Add(localWixToolsetMbaCore);
                }
                else
                {
                    localWixToolsetMbaCore = WixTools.WixToolsetMbaCore;
                    if (localWixToolsetMbaCore.PathExists())
                        requiredAsms.Add(localWixToolsetMbaCore);
                    else
                        requiredAsms.Add("WixToolset.Mba.Core.dll");

                }
            }

            //if WixSharp was "linked" with the client assembly not as script file but as external assembly
            string wixSharpAsm = System.Reflection.Assembly.GetExecutingAssembly().Location;

            if (!clientAsmPath.SamePathAs(wixSharpAsm) && !asmFile.SamePathAs(wixSharpAsm))
            {
                if (!requiredAsms.Contains(wixSharpAsm))
                    requiredAsms.Add(wixSharpAsm);
            }

            var assemblyToPackage = asmFile;

            var referencedAssembliesToPackage = requiredAsms
                .OrderBy(x => x)
                .Select(file => file.IsolateAsm())
                .Where(refAasmFile => !asmFile.SamePathAs(refAasmFile))
                .ToList();

            if (signing != null && Compiler.SignAllFilesOptions.SignEmbeddedAssemblies)
            {
                var assembliesToSign = referencedAssembliesToPackage.ToArray().Combine(assemblyToPackage).Distinct().ToArray();
                foreach (string file in assembliesToSign)
                {
                    if (Compiler.SignAllFilesOptions.SkipSignedFiles && VerifyFileSignature.IsSigned(file))
                    {
                        Compiler.OutputWriteLine($"-- Skipping signing of the already signed file: {file}");
                        continue;
                    }

                    signing.Apply(file);
                }
            }

            var configFile = outDir.PathCombine(embeddedUI ? "EmbeddedUI.config" : "CustomAction.config");

            if (configFilePath.IsNotEmpty())
            {
                if (configFile.PathGetFullPath() != configFilePath.PathGetFullPath())
                {
                    IO.File.Copy(configFilePath, configFile, true);
                    Compiler.TempFiles.Add(configFile);
                }
            }
            else
            {
                using (var writer = new IO.StreamWriter(configFile))
                    writer.Write(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
                                                <configuration>
                                                    <startup useLegacyV2RuntimeActivationPolicy=""true"">
                                                        <supportedRuntime version=""v" + Environment.Version.ToNoRevisionString() + @"""/>
                                                        <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.5""/>
                                                        <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.0""/>
                                                        <supportedRuntime version=""v2.0.50727""/>
                                                        <supportedRuntime version=""v2.0.50215""/>
                                                        <supportedRuntime version=""v1.1.4322""/>
                                                    </startup>
                                                </configuration>");
                Compiler.TempFiles.Add(configFile);
            }

            var referencedAssemblies = referencedAssembliesToPackage.Select(x => x.Enquote()).JoinBy(" ");

            string pdbFileArgument = null;
            if (!IgnoreClientAssemblyPDB && asmFile.PathChangeExtension(".pdb").FileExists())
                pdbFileArgument = $"{asmFile.PathChangeExtension(".pdb").Enquote()} ";

            outDll.DeleteIfExists();

            var makeSfxCA_args = new[]
            {
                   outDll.Enquote(),
                   sfxcaDll.Enquote(),
                   asmFile.Enquote(),
                   configFile.Enquote(),
                   pdbFileArgument ?? "",
                   referencedAssemblies,
                   dtfWinInstaller.Enquote()
            }
            .JoinBy(" ");

            ProjectValidator.ValidateCAAssembly(asmFile);

            if (Compiler.VerboseOutput)
            {
                Compiler.OutputWriteLine($"<- Packing managed CA ({asmFile.PathGetFileName()}):");
                Compiler.OutputWriteLine(makeSfxCA + " " + makeSfxCA_args);
                Compiler.OutputWriteLine("->");
            }

            if (batchFile == null)
            {
                var output = Run(makeSfxCA, makeSfxCA_args);

                if (!IO.File.Exists(outDll))
                    throw new ApplicationException("Cannot package ManagedCA assembly(" + asm + ")");

                Compiler.TempFiles.Add(outDll);
            }
            else
            {
                IO.File.WriteAllText(batchFile, $"\"{makeSfxCA}\" {makeSfxCA_args}{Environment.NewLine}pause");
            }
        }

        /// <summary>
        /// This property contains the name of the logical 'Product' XML element. The need for this property arises 
        /// from the changes in WiX v4.0 where 'Product' element was merged with "Package" element.
        /// <p>ThiOn WiX v4.* it is <c>Package</c></p>
        /// <p>On WiX v3.* it is <c>Product</c></p>
        /// <p>On WiX v4.* it is <c>Package</c></p>
        /// </summary>
        public static string ProductElementName => "Package";

        /// <summary>
        /// This property contains the name of the logical 'BinaryRef' XML atribute of the Custom Action element. The need for this property arises 
        /// from the changes in WiX v4.0 where 'BinaryRef' attribute was been renamed into "BinaryRef".
        /// <p>ThiOn WiX v4.* it is <c>Package</c></p>
        /// <p>On WiX v3.* it is <c></c></p>
        /// <p>On WiX v4.* it is <c></c></p>
        /// </summary>
        internal static string CABinarykeyAttrName => "BinaryRef";

        /// <summary>
        /// Gets list of the mapped WiX constants.
        /// </summary>
        /// <param name="include64Specific">if set to <c>true</c> [include64 specific].</param>
        /// <returns></returns>
        public static string[] GetMappedWixConstants(bool include64Specific)
        {
            var result = EnvironmentConstantsMapping.Keys.ToList();
            if (include64Specific)
                result.AddRange(EnvironmentFolders64Mapping.Keys);

            result.Sort();

            return result.Distinct().ToArray();
        }

        internal static Dictionary<string, string> EnvironmentFolders64Mapping = new Dictionary<string, string>
        {
            { "%ProgramFilesFolder%", "%ProgramFiles64Folder%" },
            { "%ProgramFiles%", "%ProgramFiles64%" },
            { "%CommonFilesFolder%", "%CommonFiles64Folder%" },
            { "%SystemFolder%", "%System64Folder%" },
            { "%CommonFiles%", "%CommonFiles64%" },
            { "%System%", "%System64%" },
        };

        internal static Dictionary<string, string> EnvironmentConstantsMapping = new Dictionary<string, string>
        {
            { "%AdminToolsFolder%", "AdminToolsFolder" },
            { "%AppDataFolder%", "AppDataFolder" },
            { "%CommonAppDataFolder%", "CommonAppDataFolder" },
            { "%CommonFiles64Folder%", "CommonFiles64Folder" },
            { "%CommonFilesFolder%", "CommonFilesFolder" },
            { "%DesktopFolder%", "DesktopFolder" },
            { "%FavoritesFolder%", "FavoritesFolder" },
            { "%FontsFolder%", "FontsFolder" },
            { "%LocalAppDataFolder%", "LocalAppDataFolder" },
            { "%MyPicturesFolder%", "MyPicturesFolder" },
            { "%PersonalFolder%", "PersonalFolder" },
            { "%ProgramFiles64Folder%", "ProgramFiles64Folder" },
            { "%ProgramFilesFolder%", "ProgramFilesFolder" },
            { "%ProgramMenuFolder%", "ProgramMenuFolder" },
            { "%SendToFolder%", "SendToFolder" },
            { "%StartMenuFolder%", "StartMenuFolder" },
            { "%StartupFolder%", "StartupFolder" },
            { "%System16Folder%", "System16Folder" },
            { "%System64Folder%", "System64Folder" },
            { "%SystemFolder%", "SystemFolder" },
            { "%TempFolder%", "TempFolder" },
            { "%TemplateFolder%", "TemplateFolder" },
            { "%WindowsFolder%", "WindowsFolder" },
            { "%WindowsVolume%", "WindowsVolume" },
            { "%AdminTools%", "AdminToolsFolder" },
            { "%AppData%", "AppDataFolder" },
            { "%CommonAppData%", "CommonAppDataFolder" },
            { "%CommonFiles64%", "CommonFiles64Folder" },
            { "%CommonFiles%", "CommonFilesFolder" },
            { "%Desktop%", "DesktopFolder" },
            { "%Favorites%", "FavoritesFolder" },
            { "%Fonts%", "FontsFolder" },
            { "%LocalAppData%", "LocalAppDataFolder" },
            { "%MyPictures%", "MyPicturesFolder" },
            { "%Personal%", "PersonalFolder" },
            { "%ProgramFiles64%", "ProgramFiles64Folder" },
            { "%ProgramFiles%", "ProgramFilesFolder" },
            { "%ProgramMenu%", "ProgramMenuFolder" },
            { "%SendTo%", "SendToFolder" },
            { "%StartMenu%", "StartMenuFolder" },
            { "%Startup%", "StartupFolder" },
            { "%System16%", "System16Folder" },
            { "%System64%", "System64Folder" },
            { "%System%", "SystemFolder" },
            { "%Temp%", "TempFolder" },
            { "%Template%", "TemplateFolder" },
            { "%Windows%", "WindowsFolder" }
        };



        static XElement AddDir(XElement parent, Dir wDir)
        {
            bool isAbsolutePathDir = IO.Path.IsPathRooted(wDir.Name);

            string name;
            // name needs to be escaped but only if it is not an absolute path.
            // Absolute path is going to be handled via auto-injection and
            if (isAbsolutePathDir)
                name = wDir.Name;
            else
                //Issue #45 Can't install dll to windows/system32
                name = wDir.Name.ExpandWixEnvConsts();

            if (!wDir.IsIdSet())
            {
                //Special folder defined either directly or by Wix# environment constant
                //e.g. %ProgramFiles%, [ProgramFilesFolder] -> ProgramFilesFolder
                if (Compiler.EnvironmentConstantsMapping.ContainsKey(wDir.Name) ||                              // %ProgramFiles%
                    Compiler.EnvironmentConstantsMapping.ContainsValue(wDir.Name) ||                            // ProgramFilesFolder
                    Compiler.EnvironmentConstantsMapping.ContainsValue(wDir.Name.TrimStart('[').TrimEnd(']')))  // [ProgramFilesFolder]
                {
                    wDir.Id = wDir.Name.Expand();
                }
                else
                {
                    if (AutoElements.LegacyDirIdAlgorithm)
                        wDir.Id = parent.Attribute("Id").Value + "." + wDir.Name.Expand(doNotFixStartDigit: true);
                    // otherwise the id will be auto-assigned the same way as for other WixEntities
                }
            }

            var r_id = wDir.RawId;
            var id = wDir.Id;

            var newSubDir = new XElement("Directory",
                                new XAttribute("Id", wDir.Id),
                                new XAttribute("Name", name));

            if (!wDir.IsAutoParent())
                newSubDir.AddAttributes(wDir.Attributes);

            Dir autoRoot = wDir.GetRootAutoParent();
            if (autoRoot != null)
                newSubDir.AddAttributes(autoRoot.Attributes);

            parent.AddElement(newSubDir);

            return newSubDir;
        }

        /// <summary>
        /// Custom handler for Compiler output. The default value is <c>Console.WriteLine</c>.
        /// </summary>
        public static Action<string> OutputWriteLine = Console.WriteLine;

        /// <summary>
        /// Delegate for receiving WiX tools output (e.g. compiler/linker).
        /// </summary>
        /// <param name="data">The data.</param>
        public delegate void ToolsOutputReceivedEventHandler(string data);

        /// <summary>
        /// Occurs when WiX tools (e.g. compiler/linker) output received.
        /// </summary>
        public static event ToolsOutputReceivedEventHandler ToolsOutputReceived;

        internal static object WiX_Tools = new object();

        /// <summary>
        /// Runs the specified executable.
        /// <p>WixSharp has a few helper methods for running executables (e.g. <see cref="WixSharp.CommonTasks.ExternalTool.ConsoleRun()"/>).
        /// However this method allows detecting and running the .NET tools installed locally within the project file structure. IE if you 
        /// installed a specific version of wix.exe for a project and then run it manually:</p>
        /// <code >
        /// string wix = ExternalTool.Locate("wix.exe"); // may return "localtool:wix"
        /// var retval = Run(wix, ...
        /// </code>
        /// 
        /// </summary>
        /// <param name="exe">The executable.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="workingDir">The working dir.</param>
        /// <param name="suppressEcho">if set to <c>true</c> [suppress echo].</param>
        /// <returns></returns>
        public static string Run(string exe, string args, string workingDir = null, bool suppressEcho = false)
        {
            lock (WiX_Tools)
            {
                var runOutput = new StringBuilder();
                string file = exe.IsAbsolutePath() ? exe : ExternalTool.Locate(exe);

                using (Process p = new Process())
                {
                    if (file?.StartsWith("localtool:") == true)
                    {
                        var toolName = file.Split(':').Last();
                        p.StartInfo.FileName = WixTools.dotnet;
                        p.StartInfo.Arguments = $"{toolName} {args}";
                    }
                    else
                    {
                        p.StartInfo.FileName = file;
                        p.StartInfo.Arguments = args;
                    }

                    Trace.WriteLine("\"" + p.StartInfo.FileName + "\" " + p.StartInfo.Arguments);

                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.WorkingDirectory = workingDir ?? Environment.CurrentDirectory;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();

                    void OnOutputLine(string line)
                    {
                        if (!suppressEcho)
                        {
                            Compiler.OutputWriteLine(line);
                            Trace.WriteLine(line);
                        }

                        ToolsOutputReceived?.Invoke(line + Environment.NewLine);
                        runOutput.AppendLine(line);
                    }

                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        string line = "";
                        while (null != (line = p.StandardError.ReadLine()))
                        {
                            lock (p)
                            {
                                OnOutputLine(line);
                            }
                        }
                    });

                    {
                        string line = null;
                        while (null != (line = p.StandardOutput.ReadLine()))
                        {
                            lock (p)
                            {
                                OnOutputLine(line);
                            }
                        }
                    }

                    p.WaitForExit();
                    return runOutput.ToString();
                }
            }
        }

        static string BuildSuppressWarningsArgs(int[] IDs)
        {
            var sb = new StringBuilder();
            foreach (int id in IDs)
                sb.Append("-sw{0} ".FormatWith(id));
            return sb.ToString();
        }

        /// <summary>
        /// Represents a wildcard running on the
        /// <see cref="T:System.Text.RegularExpressions"/> engine.
        /// </summary>
        /// <remarks>
        /// This class was developed and described by <c>reinux</c> in "Converting Wildcards to Regexes"
        /// on CodeProject (<c>http://www.codeproject.com/KB/recipes/wildcardtoregex.aspx</c>).
        /// </remarks>
        public class Wildcard : Regex
        {
            /// <summary>
            /// Initializes a wildcard with the given search pattern.
            /// </summary>
            /// <param name="pattern">The wildcard pattern to match.</param>
            public Wildcard(string pattern)
                : base(WildcardToRegex(pattern))
            {
            }

            /// <summary>
            /// Initializes a wildcard with the given search pattern and options.
            /// </summary>
            /// <param name="pattern">The wildcard pattern to match.</param>
            /// <param name="options">A combination of one or more
            /// <see cref="T:System.Text.RegexOptions"/>.</param>
            public Wildcard(string pattern, RegexOptions options)
                : base(WildcardToRegex(pattern), options)
            {
            }

            /// <summary>
            /// Converts a wildcard to a regex.
            /// </summary>
            /// <param name="pattern">The wildcard pattern to convert.</param>
            /// <returns>A regex equivalent of the given wildcard.</returns>
            public static string WildcardToRegex(string pattern)
            {
                return "^" + Regex.Escape(pattern).
                 Replace("\\*", ".*").
                 Replace("\\?", ".") + "$";
            }
        }

        static string GetShortcutWorkingDirectory(string targetPath)
        {
            string workingDir = targetPath;
            var pos = workingDir.LastIndexOfAny(@"\/]".ToCharArray());
            if (pos != -1)
                workingDir = workingDir.Substring(0, pos)
                                       .Replace("[", "")
                                       .Replace("]", "");
            return workingDir;
        }
    }
}
