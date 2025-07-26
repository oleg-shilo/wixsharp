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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WixSharp;

using IO = System.IO;

namespace WixSharp.CommonTasks
{
    /// <summary>
    ///
    /// </summary>
    public static class WixTools
    {
        static internal string NuGetDir => Environment.GetEnvironmentVariable("NUGET_PACKAGES") ?? @"%userprofile%\.nuget\packages".ExpandEnvVars();

        /// <summary>
        /// Gets or sets the WiX sharp tool dir.
        /// <para>By default it points to <c>%userprofile%\.wix\.wixsharp</c> where the WiX tools (e.g. <c>WixToolset.Dtf.WindowsInstaller.dll</c>
        /// are deployed. This setting should only be used if you cannot achieve the desired configuration with the default settings
        /// or WixTools.* properties (e.g. WixTools.DtfWindowsInstaller).
        /// </para>
        /// </summary>
        /// <value>
        /// The WiX sharp tool dir.
        /// </value>
        static public string WixSharpToolDir { set; get; } = @"%userprofile%\.wix\.wixsharp".ExpandEnvVars();

        /// <summary>
        /// Gets or sets the WiX extensions dir.
        /// <para>By default it points to <c>%userprofile%\.wix\extensions</c> where all WiX extensions are deployed
        /// by the WiX tool <c>wix.exe</c>. This setting is useful when you want to deploy WiX tools and extensions
        /// manually (e.g. on CI). IE if you deployed WiX extension via NuGet manager then
        /// <see cref="WixExtensionsDir"/> needs to be set to <c>%userprofile%\.nuget\packages</c>.
        /// </para>
        /// </summary>
        /// <value>
        /// The WiX extensions dir.
        /// </value>
        static public string WixExtensionsDir { set; get; } = @"%userprofile%\.wix\extensions".ExpandEnvVars();

        static string PackageDir(string name)
            => Directory.GetDirectories(NuGetDir.PathCombine(name))
                        .Select(x => new { Directory = x, Version = x.PathGetFileName().ToRawVersion() })
                        .OrderBy(x => x.Version)
                        .LastOrDefault()?.Directory;

        /// <summary>
        /// Gets or sets the well known locations for probing the tool's exe file.
        /// <para>
        /// By default probing is conducted in the locations defined in the system environment variable <c>PATH</c>.
        /// By setting <c>WellKnownLocations</c> you can add some extra probing locations.
        /// </para>
        /// </summary>
        /// <value>The well known locations.</value>
        static public List<string> WellKnownLocations = DiscoverWellKnownLocations()
               .Concat(new[]
                {
                    Environment.SpecialFolder.ProgramFilesX86.GetPath().PathJoin(@"Windows Kits\10\App Certification Kit"),
                    Environment.SpecialFolder.ProgramFilesX86.GetPath().PathJoin(@"Windows Kits\8.1\bin\x86"),
                    Environment.SpecialFolder.ProgramFilesX86.GetPath().PathJoin(@"Windows Kits\8.0\bin\x86"),
                    Environment.SpecialFolder.ProgramFilesX86.GetPath().PathJoin(@"Microsoft SDKs\ClickOnce\SignTool"),
                    Environment.SpecialFolder.ProgramFilesX86.GetPath().PathJoin(@"Microsoft SDKs\Windows\v7.1A\Bin"),
                    Environment.SpecialFolder.ProgramFiles.GetPath().PathJoin(@"Microsoft SDKs\Windows\v6.0A\bin")
                })
               .ToList();

        static IEnumerable<string> DiscoverWellKnownLocations()
        {
            //  @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.15063.0\x86",
            var win10sdk = Environment.SpecialFolder.ProgramFilesX86.GetPath().PathJoin("Windows Kits", "10", "bin");
            if (!win10sdk.PathExists())
                win10sdk = ((string)Microsoft.Win32.Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows Kits\Installed Roots", "KitsRoot10", null))
                    .PathJoin("bin");

            return Directory.Exists(win10sdk)
                ? Directory
                    .GetDirectories(win10sdk, "10.*")
                    .OrderByDescending(x => new Version(x.PathGetFileName()))
                    .Select(x => x.PathCombine("x86"))
                : Enumerable.Empty<string>();
        }

        static string signTool;

        /// <summary>
        /// The path to the sign tool (sign.exe or insignia exe). It may not be distributed with WiX SDK.
        /// </summary>
        static public string SignTool
        {
            set => signTool = value;
            get
            {
                if (signTool == null)
                {
                    var exe = "signtool.exe";
                    signTool = ExternalTool.Locate(exe) ??
                               WellKnownLocations.Select(x => x.Trim().PathCombine(exe)).FirstOrDefault(IO.File.Exists);

                    if (signTool == null)
                    {
                        throw new Exception($"Cannot find {exe}. " +
                            $"Please set its exact location either via `WixSharp.CommonTasks.WixTools.SignTool` property. " +
                            $"Or help WixSharp locating it by adding possible path to `WixSharp.CommonTasks.WixTools.WellKnownLocations`");
                    }
                }
                return signTool;
            }
        }

        static string makeSfxCA = null;

        /// <summary>
        /// Gets or sets the location of the `WixToolset.Dtf.MakeSfxCA.exe`.
        /// </summary>
        public static string MakeSfxCA
        {
            set => makeSfxCA = value;
            get
            {
                if (makeSfxCA != null)
                    return makeSfxCA;

                EnsureDtfTool();
                return Directory.GetFiles(PackageDir("wixtoolset.dtf.customaction"), "WixToolset.Dtf.MakeSfxCA.exe", SearchOption.AllDirectories).FirstOrDefault();
            }
        }

        static string heat = null;

        /// <summary>
        /// Gets or sets the location of `heat.exe`.
        /// </summary>
        public static string Heat
        {
            set => heat = value;
            get
            {
                if (heat != null)
                    return heat;

                EnsureDtfTool("wixtoolset.heat");
                return Directory.GetFiles(PackageDir("wixtoolset.heat"), "heat.exe", SearchOption.AllDirectories).FirstOrDefault();
            }
        }

        static string dtfWindowsInstaller = null;

        /// <summary>
        /// Gets or sets the location of `WixToolset.Dtf.WindowsInstaller.dll`.
        /// </summary>
        public static string DtfWindowsInstaller
        {
            set => dtfWindowsInstaller = value;
            get
            {
                if (dtfWindowsInstaller != null)
                    return dtfWindowsInstaller;

                var localAsm = System.Reflection.Assembly.GetExecutingAssembly().GetLocation().PathChangeFileName("WixToolset.Dtf.WindowsInstaller.dll");
                if (IO.File.Exists(localAsm))
                    return localAsm;

                var loadedAsm = System.AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(x => x.GetName().Name.StartsWith("WixToolset.Dtf.WindowsInstaller"))?
                    .GetLocation();

                if (loadedAsm != null && IO.File.Exists(loadedAsm))
                    return loadedAsm;

                EnsureDtfTool();
                return WixSharpToolDir.PathCombine(@"wix.tools\publish\WixToolset.Dtf.WindowsInstaller.dll");
            }
        }

        static string wixToolsetMbaCore;

        /// <summary>
        /// Gets or sets the location of `WixToolset.Mba.Core.dll`.
        /// </summary>
        public static string WixToolsetMbaCore
        {
            set => wixToolsetMbaCore = value;
            get
            {
                if (wixToolsetMbaCore != null)
                    return wixToolsetMbaCore;

                EnsureDtfTool();
                return PackageDir("wixtoolset.mba.core").PathCombine(@"lib\netstandard2.0\WixToolset.Mba.Core.dll");
            }
        }

        /// <summary>
        /// The location of x64 version of `SfxCA.dll`.
        /// </summary>
        public static string SfxCAx64;

        /// <summary>
        /// The location of x86 version of `SfxCA.dll`.
        /// </summary>
        public static string SfxCAx86;

        internal static string SfxCAFor(bool is64)
        {
            if (is64 && SfxCAx64 != null)
                return SfxCAx64;

            if (!is64 && SfxCAx86 != null)
                return SfxCAx64;

            EnsureDtfTool();

            string platformDir = is64 ? "x64" : "x86";

            return PackageDir("wixtoolset.dtf.customaction").PathCombine($@"tools\{platformDir}\SfxCA.dll");
        }

        /// <summary>
        /// Sets the wix.exe version for the specific directory. The mapping of the executable is managed by the .NET Tool
        /// mechanism through "dotnet-tools.json" config file, which is created when this method is called.
        /// <para>Note, if version argument is null the config file will be deleted.</para>
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="version">The version.</param>
        static public void SetWixVersion(string directory, string version)
        {
            try
            {
                globalWixVersion = null;

                var configDir = directory.PathCombine(".config");
                var configFile = configDir.PathCombine("dotnet-tools.json");

                if (version == null)
                {
                    if (!Directory.GetFiles(configDir).Any() && !Directory.GetDirectories(configDir).Any())
                        configDir.DeleteIfExists();
                    else
                        configFile.DeleteIfExists();

                    return;
                }

                var config = @"{
  ""version"": 1,
  ""isRoot"": true,
  ""tools"": {
    ""wix"": {
      ""version"": """ + version + @""",
      ""commands"": [
        ""wix""
      ]
    }
  }
}";
                configDir.EnsureDirExists();
                IO.File.WriteAllText(configFile, config);
            }
            finally
            {
                var newVersion = GlobalWixVersion; // just to force the version to be re-read
            }
        }

        static Version globalWixVersion;

        /// <summary>
        /// Gets the version of .NET tool "wix.exe".
        /// Note the version can be set by <see cref="SetWixVersion(string, string)"/>.
        /// </summary>
        /// <value>
        /// The global wix version.
        /// </value>
        public static Version GlobalWixVersion
        {
            get
            {
                if (globalWixVersion == null)
                {
                    // for the global wix install it is important to run wix without path
                    // so ShellExecute picks the global installation based on the PATH

                    // IE: 5.0.0+41e11442
                    globalWixVersion = Compiler.Run(WixTools.wix_global, "--version", null, true).Trim().Split('+').First().SemanticVersionToVersion();

                    if (globalWixVersion.IsUninitialized())
                    {
                        Compiler.Run(dotnet, "tool restore");
                        globalWixVersion = Compiler.Run(WixTools.wix_global, "--version", null, true).Trim().Split('+').First().SemanticVersionToVersion();
                    }

                    Compiler.OutputWriteLine($"Wix version: {globalWixVersion}");
                }

                return globalWixVersion;
            }
        }

        internal static bool IsHighestAvailableVersion(string pathToextensionFile)
        {
            // C:\Users\oleg\.wix\extensions\WixToolset.Bal.wixext\4.0.2\wixext4\WixToolset.Bal.wixext.dll
            var version = pathToextensionFile.PathGetDirName().PathGetDirName().PathGetFileName().SemanticVersionToVersion();
            var name = pathToextensionFile.PathGetDirName().PathGetDirName().PathGetDirName().PathGetFileName();

            var highestVersion = Directory
                                 .GetDirectories(WixExtensionsDir.PathCombine(name))
                                 .Select(x => x.PathGetFileName().SemanticVersionToVersion())
                                 .OrderByDescending(x => x)
                                 .FirstOrDefault();

            return (version == highestVersion);
        }

        /// <summary>
        /// Finds the WiX extension DLL in the <see cref="WixExtensionsDir"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        static public string FindWixExtensionDll(string name, string version = null)
        {
            return FindWixExtensionDll(WixExtensionsDir, name, version) ?? // the extension is installed with the wix.exe
                   FindWixExtensionDll(NuGetDir, name, version);           // the extension is installed as a nuget package
        }

        static string FindWixExtensionDll(string extensionDir, string name, string version)
        {
            if (!extensionDir.PathCombine(name).PathExists())
                return null;

            // C:\Users\user\.wix\extensions\WixToolset.UI.wixext\5.0.0\wixext5\WixToolset.UI.wixext.dll
            // C:\Users\user\.wix\extensions\WixToolset.UI.wixext\4.0.4\wixext4\WixToolset.UI.wixext.dll
            // C:\Users\user\.wix\extensions\WixToolset.Bal.wixext\4.0.2\wixext4\WixToolset.Bal.wixext.dll
            // C:\Users\user\.wix\extensions\WixToolset.Bal.wixext\5.0.0\wixext5\WixToolset.BootstrapperApplications.wixext.dll
            var candidates = Directory
                             .GetDirectories(extensionDir.PathCombine(name))
                             .Select(x => new
                             {
                                 version = x.PathGetFileName().SemanticVersionToVersion(),
                                 compatibleVersion = x.PathCombine($"wixext{GlobalWixVersion.Major}"),
                                 file = x.PathCombine($"wixext{GlobalWixVersion.Major}", name + ".dll"),
                             })
                             .Select(x => new
                             {
                                 x.version,
                                 file = x.file.PathExists()
                                     ? x.file
                                     : x.compatibleVersion.PathExists()
                                         ? Directory.GetFiles(x.compatibleVersion, "WixToolset.*.wixext.dll").FirstOrDefault()
                                         : null,
                             })
                             .Where(x => version == null || x.version.ToString() == version)
                             .OrderByDescending(x => x.version);

            return candidates.FirstOrDefault(x => IO.File.Exists(x.file))?.file;
        }

        internal static void EnsureWixExtension(string name, string version = null)
        {
            if (version.IsEmpty())
            {
                if (FindWixExtensionDll(name).IsEmpty())
                    Compiler.Run(WixTools.wix_global, $"extension add -g {name}");
            }
            else
            {
                if (FindWixExtensionDll(name, version).IsEmpty())
                    Compiler.Run(WixTools.wix_global, $"extension add -g {name}/{version}");
            }
        }

        static internal string wix
        {
            get
            {
                // cannot cache wix.exe location as it might change depending to the current directory
                // (e.g. if it is deployed as .NET local tool)
                string wix = ExternalTool.Locate("wix.exe") ?? wix_global;
                return wix;
            }
        }

        static internal string wix_global
        {
            get
            {
                try
                {
                    Compiler.Run("wix.exe", "--version", supressEcho: true);
                    return "wix.exe";
                }
                catch
                {
                    var error = "`wix.exe` cannot be found. Ensure you installed it with `dotnet tool install --global wix`";
                    Compiler.OutputWriteLine("Error: " + error);
                    throw new ApplicationException(error);
                }
            }
        }

        static string dotnet_exe = null;

        static internal string dotnet
        {
            get
            {
                if (dotnet_exe == null)
                {
                    try
                    {
                        Compiler.Run("dotnet.exe", "--version", supressEcho: true);
                        dotnet_exe = "dotnet.exe";
                    }
                    catch
                    {
                        var error = "dotnet.exe is not found. Please install .NET SDK v8.0 or higher.";
                        Compiler.OutputWriteLine("Error: " + error);
                        throw new ApplicationException(error);
                    }
                }
                return dotnet_exe;
            }
        }

        internal static void EnsureDtfTool(string package = null)
        {
            var projectDir = WixSharpToolDir.PathCombine("wix.tools");
            var publishDir = projectDir.PathCombine("publish");

            if (Directory.Exists(NuGetDir.PathCombine(package ?? "wixtoolset.dtf.customaction")) &&
                Directory.Exists(publishDir))
                return;

            Directory.CreateDirectory(projectDir);

            var projectFile = projectDir.PathJoin("wix.tools.csproj");

            // if (!IO.File.Exists(projectFile))
            IO.File.WriteAllText(projectFile, $@"<Project Sdk=""Microsoft.NET.Sdk"">
                                                      <PropertyGroup>
                                                        <TargetFramework>net472</TargetFramework>
                                                      </PropertyGroup>

                                                      <ItemGroup>
                                                        <PackageReference Include=""WixToolset.Dtf.CustomAction"" Version=""*"" />
                                                        <PackageReference Include=""WixToolset.Dtf.WindowsInstaller"" Version=""*"" />
                                                        <PackageReference Include=""WixToolset.Heat"" Version=""*"" />
                                                      </ItemGroup>
                                                    </Project>");

            IO.File.WriteAllText(projectDir.PathJoin("dummy.cs"),
                 $@"using WixToolset.Dtf.WindowsInstaller;
                    public class CustomActions
                    {{
                        [CustomAction] public static ActionResult CustomAction1(Session session) => ActionResult.Success;
                    }}");

            var sw = Stopwatch.StartNew();
            Console.WriteLine("Restoring packages...");

            // need to publish to restore even if we are going to "publish"
            // It is to handle the cases when in user updated project file manually (e.g. to get different version of tools)
            Compiler.Run(dotnet, "restore", projectDir);

            // need to publish to isolate assemblies
            Compiler.Run(dotnet, @"publish -o .\publish", projectDir);
        }
    }

    /// <summary>
    /// A generic utility class for running console application tools (e.g. compilers, utilities)
    /// </summary>
    public class ExternalTool
    {
        static string[] defaultLookupDirs => new[] { Environment.CurrentDirectory }.Combine(Environment.GetEnvironmentVariable("PATH").Split(';')).ToArray();

        /// <summary>
        /// Locates the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static string Locate(string file)
        {
            if (file.StartsWith("localtool:")) // there is no need to locate it as it will be invoked with `dotnet` launcher
                return file;

            if (file.PathGetFileNameWithoutExtension() == "wix" &&
                file.PathGetDirName().IsEmpty() &&
                IsLocalTool("wix")) // currently we expect only wix.exe to be a local tool
            {
                // a special case as it can be installed as a global tool (location is listed in PATH)
                // or it can be specially versioned wix.exe based on the presence of dotnet-tools.json
                // file in one of current dir the parent folders.
                // Such tool installation is called `local`. Of course it is still globally installed but
                // it is local with respect to invoking it from the current dir.

                return "localtool:wix";
            }

            var possibleLocations = defaultLookupDirs.Select(x => x.Trim().PathCombine(file));
            return possibleLocations.FirstOrDefault(IO.File.Exists);
        }

        static bool IsLocalTool(string tool)
        {
            var probingDir = Environment.CurrentDirectory;
            do
            {
                var toolManifest = probingDir.PathJoin(".config", "dotnet-tools.json");
                if (toolManifest.FileExists())
                {
                    if (IO.File.ReadAllLines(toolManifest).Select(x => x.Trim()).Any(x => x == "\"wix\": {"))
                        return true; // contains wix configuration
                }
                probingDir = probingDir.PathGetDirName();
            } while (probingDir.PathGetFileName().IsNotEmpty()); // not the root dir of the drive

            return false;
        }

        /// <summary>
        /// The default console out handler. It can be used when you want to have fine control over
        /// STD output of the external tool.
        /// </summary>
        /// <example>The following is an example of masking the word 'secret' in the output text.
        /// <code>
        /// ExternalTool.ConsoleOut = (line) => Console.WriteLine(line.Replace("secret", "******"))
        /// var tool = new ExternalTool
        /// {
        ///     ExePath = "tool.exe",
        ///     Arguments = "-a -b",
        /// };
        /// tool.ConsoleRun();
        /// </code>
        /// </example>
        public Action<string> ConsoleOut = Compiler.OutputWriteLine;

        /// <summary>
        /// Controls echoing the executed command in the ConsoleOut.
        /// </summary>
        public bool EchoOn = true;

        /// <summary>
        /// Gets or sets the encoding to be used to process external executable output.
        /// By default it is the value of <see cref="ExternalTool.DefaultEncoding"/>.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public Encoding Encoding;

        /// <summary>
        /// Gets or sets the default encoding to be used to process external executable output.
        /// By default it is the value of <c>System.Text.Encoding.Default</c>.
        /// </summary>
        public static Encoding DefaultEncoding = Encoding.Default;

        /// <summary>
        /// Gets or sets the path to the exe file of the tool to be executed.
        /// </summary>
        /// <value>The exe path.</value>
        public string ExePath { set; get; }

        /// <summary>
        /// Gets or sets the arguments for the exe file of the tool to be executed.
        /// </summary>
        /// <value>The arguments.</value>
        public string Arguments { set; get; }

        /// <summary>
        /// Gets or sets the well known locations for probing the exe file.
        /// <para>
        /// By default probing is conducted in the locations defined in the system environment variable <c>PATH</c>.
        /// By setting <c>WellKnownLocations</c>
        /// you can add some extra probing locations. The directories must be separated by the ';' character.
        /// </para>
        /// </summary>
        /// <value>The well known locations.</value>
        public string WellKnownLocations { set; get; }

        /// <summary>
        /// Runs the exec file with the console output completely hidden and discarded.
        /// </summary>
        /// <returns>The process exit code.</returns>
        public int WinRun()
        {
            string systemPathOriginal = Environment.GetEnvironmentVariable("PATH");
            try
            {
                Environment.SetEnvironmentVariable("PATH", systemPathOriginal + ";" + Environment.ExpandEnvironmentVariables(this.WellKnownLocations ?? ""));

                var process = new Process();
                process.StartInfo.FileName = this.ExePath;
                process.StartInfo.Arguments = this.Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.StandardOutputEncoding = this.Encoding ?? DefaultEncoding;
                process.Start();

                process.WaitForExit();
                return process.ExitCode;
            }
            finally
            {
                Environment.SetEnvironmentVariable("PATH", systemPathOriginal);
            }
        }

        /// <summary>
        /// Runs the exec file with the console and redirects the output into the current process console output.
        /// </summary>
        /// <returns>The process exit code.</returns>
        public int ConsoleRun()
        {
            return ConsoleRun(this.ConsoleOut);
        }

        /// <summary>
        /// Runs the exec file with the console and returns the output text.
        /// </summary>
        /// <returns>The process console output.</returns>
        public string GetConsoleRunOutput()
        {
            var buf = new StringBuilder();
            this.ConsoleRun(line => buf.AppendLine(line));
            return buf.ToString();
        }

        /// <summary>
        /// Runs the exec file with the console and intercepts and redirects the output into the user specified delegate.
        /// </summary>
        /// <param name="onConsoleOut">The on console out.</param>
        /// <returns>The process exit code.</returns>
        public int ConsoleRun(Action<string> onConsoleOut)
        {
            string systemPathOriginal = Environment.GetEnvironmentVariable("PATH");
            try
            {
                Environment.SetEnvironmentVariable("PATH", Environment.ExpandEnvironmentVariables(this.WellKnownLocations ?? "") + ";" + "%WIXSHARP_PATH%;" + systemPathOriginal);

                string exePath = Locate(this.ExePath);

                if (exePath == null)
                {
                    onConsoleOut("Error: Cannot find " + this.ExePath);
                    onConsoleOut("Make sure it is in the System PATH or WIXSHARP_PATH environment variables or WellKnownLocations member/parameter is initialized properly. ");
                    return 1;
                }

                if (EchoOn)
                    onConsoleOut("Execute:\n\"" + this.ExePath + "\" " + this.Arguments);

                using (var process = new Process())
                {
                    process.StartInfo.FileName = exePath;
                    process.StartInfo.Arguments = this.Arguments;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.StandardOutputEncoding = this.Encoding ?? DefaultEncoding;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    if (onConsoleOut != null)
                    {
                        string line = null;
                        while (null != (line = process.StandardOutput.ReadLine()))
                        {
                            onConsoleOut(line);
                        }

                        string error = process.StandardError.ReadToEnd();
                        if (!error.IsEmpty())
                            onConsoleOut(error);
                    }
                    process.WaitForExit();
                    return process.ExitCode;
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable("PATH", systemPathOriginal);
            }
        }
    }
}