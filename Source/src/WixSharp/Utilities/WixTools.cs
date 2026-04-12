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

        /// <summary>
        /// Gets the directory path of the specified NuGet package, selecting the preferred or latest available version.
        /// </summary>
        /// <remarks>If a preferred version is specified in the package definition or globally, it is
        /// selected if available; otherwise, the latest stable version is returned. Supports both standard and
        /// non-standard version formats, including pre-release versions.</remarks>
        /// <param name="name">The name of the NuGet package to locate. Cannot be null or empty.</param>
        /// <returns>The full directory path of the matching package version if found; otherwise, null.</returns>
        public static string PackageDir(string name)
        {
            var package = WixDtfPackages.FirstOrDefault(x => x.package == name);
            var preferredVersion = package.version ?? WixTools.GlobalWixVersion?.ToString() ?? "*";

            var packageDir = NuGetDir.PathCombine(name);

            if (!packageDir.PathExists())
                return null;

            var latestVersion = Directory.GetDirectories(NuGetDir.PathCombine(name))
                    .Select(x => new { Directory = x, Version = x.PathGetFileName().ToRawVersion() }) // raw version will ensure picking only the stable non pre-release versions                    .OrderBy(x => x.Version)
                    .LastOrDefault()?.Directory;

            var matchingVersion = Directory.GetDirectories(NuGetDir.PathCombine(name))
                    .Select(x => new { Directory = x, Version = x.PathGetFileName() }) // do not convert to Version here as it can be non-standard version like "5.0.0-preview.1" and we want to support it as well
                    .FirstOrDefault(x => x.Version == preferredVersion)?.Directory;

            return matchingVersion ?? latestVersion;
        }

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
                win10sdk = (Microsoft.Win32.Registry.GetValue(
                            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows Kits\Installed Roots", "KitsRoot10",
                            null) as string)?
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

                EnsureDtfTools();
                // find any heat.exe in the package versions. since it is exe, it does not matter what platform it targets (e.g. x86 vs x64)
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

                EnsureDtfTools();
                // find any heat.exe in the package versions. since it is exe, it does not matter what platform it targets (e.g. x86 vs x64)
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

                EnsureDtfTools();
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

                EnsureDtfTools();
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

        /// <summary>
        /// Gets the file path to the appropriate SfxCA.dll for the specified platform architecture.
        /// </summary>
        /// <remarks>This method selects the SfxCA.dll based on the specified architecture. If a cached
        /// path is available, it is returned; otherwise, the path is constructed dynamically. The returned path may
        /// depend on the presence and configuration of supporting tools.</remarks>
        /// <param name="is64">true to retrieve the path for the 64-bit version; false for the 32-bit version.</param>
        /// <returns>A string containing the full file path to the SfxCA.dll for the requested platform architecture.</returns>
        public static string SfxCAFor(bool is64)
        {
            if (is64 && SfxCAx64 != null)
                return SfxCAx64;

            if (!is64 && SfxCAx86 != null)
                return SfxCAx64;

            string platformDir = is64 ? "x64" : "x86";

            EnsureDtfTools();

            return PackageDir("wixtoolset.dtf.customaction").PathCombine($@"tools\{platformDir}\SfxCA.dll");
        }

        /// <summary>
        /// Sets the wix.exe version for the specific directory. The mapping of the executable is managed by the .NET Tool
        /// mechanism through "dotnet-tools.json" config file, which is created when this method is called.
        /// <para>IE `WixTools.SetWixVersion(Environment.CurrentDirectory, "5.0.1");`</para>
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

            if (!WixExtensionsDir.PathCombine(name).PathExists())
                return false;

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
                             .OrderByDescending(x => x.version)
                             .ToArray();

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
                    Compiler.Run("wix.exe", "--version", suppressEcho: true);
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
                        Compiler.Run("dotnet.exe", "--version", suppressEcho: true);
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

        /// <summary>
        /// Specifies the acceptance status of the End User License Agreement (EULA) as a string value.
        /// </summary>
        /// <remarks>Set this field to indicate whether the EULA has been accepted. This is required for the WiX v7 and higher. The expected value for
        /// WiX 7 is "wix7". See https://docs.firegiant.com/wix/osmf/ for details.</remarks>
        public static string AcceptEulaFor = "";

        static (string package, string version)[] wixDtfPackages = null;

        /// <summary>
        /// Gets or sets the NuGet package definitions for the required WiX DTF components.
        /// These components are the secondary WiX compilation tools that are distributed as NuGet packages and
        /// are essential for the functionality of the WiX DTF (Deployment Tools Foundation).
        /// They include tools like MakeSfxCA, Heat.exe etc.
        /// <remarks>If version is specified  as ""*"" the highest version of the package found in the nuget cache will be used.
        /// If none is found then the latest will be downloaded from the NuGet repository. Thus if the NuGet cache does not have
        /// the latest publicly available version of the package, you can update the cache with <see cref="WixTools.RestoreDtfPackages"/>.</remarks>
        /// </summary>
        public static (string package, string version)[] WixDtfPackages
        {
            set => wixDtfPackages = value;
            get
            {
                if (wixDtfPackages != null)
                    return wixDtfPackages;

                var wixVersion = WixTools.GlobalWixVersion?.ToString() ?? "*";

                wixDtfPackages = new[]
                {
                    (package: "WixToolset.Dtf.CustomAction",    version: wixVersion),
                    (package: "WixToolset.Dtf.WindowsInstaller",version: wixVersion),
                    (package: "WixToolset.Heat",                version: "*"),
                    (package: "WixToolset.Mba.Core",            version: "*")
                };

                return wixDtfPackages;
            }
        }

        static string WixDtfPackagesProjectFragment
            => WixDtfPackages
                .Select(x => $"<PackageReference Include=\"{x.package}\" Version=\"{x.version}\" />")
                .JoinBy(Environment.NewLine);

        /// <summary>
        /// Restores the DTF tools (installs NuGet packages) that are defined in the <see cref="WixDtfPackages"/> property.
        /// This method ensures that the required DTF tools are present and up to date by restoring the specified NuGet packages.
        /// </summary>
        /// <remarks>
        /// <para>Calling this method deploys the latest version of all packages that have their version sepcified by `*` .</para>
        /// operation will bring the overwrite any existing installation of the tools.</remarks>
        public static void RestoreDtfPackages() => EnsureDtfTools(force: true);

        internal static void EnsureDtfTools(bool force = false)
        {
            var projectDir = WixSharpToolDir.PathCombine("wix.tools");
            var publishDir = projectDir.PathCombine("publish");

            bool areDtfPackagesRestored = WixDtfPackages
                .All(x =>
                {
                    if (x.version == "*")
                    {
                        var packageDir = NuGetDir.PathCombine(x.package);
                        return packageDir.PathExists() && Directory.GetDirectories(packageDir).Any();
                    }
                    else
                    {
                        return Directory.Exists(NuGetDir.PathCombine(x.package, x.version));
                    }
                });

            if (areDtfPackagesRestored && publishDir.PathExists() && !force)
                return;

            Directory.CreateDirectory(projectDir);

            // create global lock here
            var mutexName = $"CSS_EnsureDtfTools_{projectDir.GetHashCode()}";
            bool hasLock = false;

            using (var mutex = new System.Threading.Mutex(false, mutexName))
            {
                try
                {
                    try
                    {
                        hasLock = mutex.WaitOne(TimeSpan.FromMinutes(10));
                    }
                    catch (System.Threading.AbandonedMutexException)
                    {
                        // Previous owner crashed; mutex is acquired by current thread.
                        hasLock = true;
                    }

                    var projectFile = projectDir.PathJoin("wix.tools.csproj");

                    var wixVersion = WixTools.GlobalWixVersion?.ToString() ?? "*";

                    var eulaGroup = AcceptEulaFor.IsNotEmpty() ? $"<PropertyGroup><AcceptEula>{AcceptEulaFor}</AcceptEula></PropertyGroup>" : "";

                    IO.File.WriteAllText(projectDir.PathJoin("dummy.cs"),
                         $@"using WixToolset.Dtf.WindowsInstaller;
                    public class CustomActions
                    {{
                        [CustomAction] public static ActionResult CustomAction1(Session session) => ActionResult.Success;
                    }}");

                    // Note, for WixToolset.Mba.Core we do not specify a version as WiX stopped releasing it with new WiX versions.
                    // The last package targets WiX v4.0.6 and it is still compatible with later WiX versions
                    var proj = $@"<Project Sdk=""Microsoft.NET.Sdk"">
                                                    <PropertyGroup>
                                                        <TargetFramework>net472</TargetFramework>
                                                    </PropertyGroup>

                                                        {eulaGroup}

                                                    <ItemGroup>
                                                        {WixDtfPackagesProjectFragment}
                                                    </ItemGroup>
                                                </Project>";
                    IO.File.WriteAllText(projectFile, proj);

                    var sw = Stopwatch.StartNew();
                    Compiler.OutputWriteLine("Restoring packages...");

                    // need to call restore explicitly as publish does not restore if the assets are already present (e.g. from previous publish)
                    // even if the project file was changed and has new package references

                    var output = Compiler.Run(dotnet, "restore", out int exitCode, projectDir, suppressEcho: true);

                    if (output.HasEulaError())
                        PrintEulaWarning();

                    if (exitCode == 0)
                    {
                        // need to publish to restore even if we are going to "publish"
                        // It is to handle the cases when in user updated project file manually (e.g. to get different version of tools)
                        // this is because we need to isolate assemblies and to ensure the packages are compatible (it will not publish otherwise)
                        output = Compiler.Run(dotnet, @"publish -o .\publish", projectDir);

                        if (output.HasEulaError()) // restore may swallow the EULA error
                            PrintEulaWarning();
                    }
                }
                finally
                {
                    if (hasLock)
                        mutex.ReleaseMutex();
                }
            }
        }

        internal static bool HasEulaError(this string compilerOutput)
        => (compilerOutput.Contains("error NU1102") && WixTools.GlobalWixVersion.Major > 6) ||
            // error WIX7015: You must accept the Open Source Maintenance Fee (OSMF) EULA to use WiX Toolset v7. For instructions, see https://wixtoolset.org/osmf/
            (compilerOutput.Contains("WIX7015"));

        internal static void PrintEulaWarning()
        {
            Compiler.OutputWriteLine("");
            Compiler.OutputWriteLine("WARNING: if you are using WiX v7 or later, you may need to acknowledge acceptance of WiX EULA." +
                $"You can do this by setting the `WixTools.{nameof(WixTools.AcceptEulaFor)}` property to the value that corresponds to the WiX version you are accepting the " +
                "ELUA for. IE \"wix7\". See https://docs.firegiant.com/wix/osmf/ for details. " +
                "Note, EULA represents the relationship between the user and the WiX Toolset vendor and it is " +
                "fully managed by the WiX team. While WixSharp only provides a mechanism for acknowledging the EULA in accordance with the WiX team's guidelines.");

            Compiler.OutputWriteLine("");
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
        /// Gets or sets the working dir.
        /// </summary>
        /// <value>
        /// The working dir.
        /// </value>
        public string WorkingDirectory { set; get; }

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
                process.StartInfo.WorkingDirectory = WorkingDirectory;
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

                if (exePath?.StartsWith("localtool:") == true)
                {
                    exePath = exePath.Split(':').Last();
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
                    process.StartInfo.WorkingDirectory = WorkingDirectory;
                    process.StartInfo.StandardOutputEncoding = this.Encoding ?? DefaultEncoding;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    if (onConsoleOut != null)
                    {
                        string line = null;
                        while (null != (line = process.StandardOutput.ReadLine()))
                        {
                            if (!line.IsEmpty())
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