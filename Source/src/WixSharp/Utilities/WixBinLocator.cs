using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace WixSharp
{
    /// <summary>
    /// A class for probing directories of the build environment for presense of WiX tools.
    /// </summary>
    public static class WixBinLocator
    {
        /// <summary>
        /// Finds the WiX SDK location.
        /// </summary>
        /// <param name="wixLocation">The WiX location.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">WiX SDK binaries cannot be found. Please set WixSharp.Compiler.WixSdkLocation to valid path to the Wix SDK binaries.</exception>
        public static string FindWixSdkLocation(string wixLocation, bool throwOnError = true)
        {
            var wixSdkLocation = Path.GetFullPath(Utils.PathCombine(wixLocation, @"..\sdk"));

            if (!Directory.Exists(wixSdkLocation))
            {
                wixSdkLocation = Path.GetFullPath(Utils.PathCombine(wixLocation, "sdk")); //NuGet package shovels the dirs
                if (!Directory.Exists(wixSdkLocation))
                {
                    if (throwOnError)
                    {
                        throw new Exception("WiX SDK binaries cannot be found. Please set WixSharp.Compiler.WixSdkLocation to valid path to the Wix SDK binaries.");
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return wixSdkLocation;
        }

        /// <summary>
        /// Finds the WiX bin folder location.
        /// </summary>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <returns></returns>
        /// <exception cref="WixSharpException">WiX binaries cannot be found. Wix# is capable of automatically finding WiX tools only if " +
        ///                                             "WiX Toolset installed. In all other cases you need to set the environment variable " +
        ///                                             "WIXSHARP_WIXDIR or WixSharp.Compiler.WixLocation to the valid path to the WiX binaries.\n" +
        ///                                             "WiX binaries can be brought to the build environment by either installing WiX Toolset, " +
        ///                                             "downloading Wix# suite or by adding WixSharp.wix.bin NuGet package to your project.</exception>
        public static string FindWixBinLocation(bool throwOnError = true)
        {
            // See if the command line was set for this property
            var msBuildArgument = Environment.GetCommandLineArgs().FirstPrefixedValue("/WIXBIN:");
            if (msBuildArgument.IsNotEmpty() && Directory.Exists(msBuildArgument))
            {
                return Path.GetFullPath(msBuildArgument);
            }

            // Now check to see if the environment variable was set system wide
            var environmentVar = Environment.GetEnvironmentVariable("WIXSHARP_WIXDIR");
            if (environmentVar.IsNotEmpty() && Directory.Exists(environmentVar))
            {
                return Path.GetFullPath(environmentVar);
            }

            // Now check to see if the environment variable was set by the parent process
            environmentVar = Environment.GetEnvironmentVariable("WixLocation");
            if (environmentVar.IsNotEmpty() && Directory.Exists(environmentVar))
            {
                return Path.GetFullPath(environmentVar);
            }

            // Now check to see if the environment variable was set by the scoop installation
            environmentVar = Environment.GetEnvironmentVariable("WixToolPath");
            if (environmentVar.IsNotEmpty() && Directory.Exists(environmentVar))
            {
                return Path.GetFullPath(environmentVar);
            }

            // Now check to see if the environment variable was set by the WiX nuget package
            environmentVar = Environment.GetEnvironmentVariable("WixExtDir");
            if (environmentVar.IsNotEmpty() && Directory.Exists(environmentVar))
            {
                return Path.GetFullPath(environmentVar);
            }

            // Now check to see if the WIX install set an environment variable
            var wixEnvironmentVariable = Environment.ExpandEnvironmentVariables(@"%WIX%\bin");
            if (wixEnvironmentVariable.IsNotEmpty() && Directory.Exists(wixEnvironmentVariable))
            {
                return Path.GetFullPath(wixEnvironmentVariable);
            }

            // Now try the program files install location
            string wixInstallDir = Directory.GetDirectories(Utils.ProgramFilesDirectory, "Windows Installer XML v3*")
                                            .Order()
                                            .LastOrDefault();

            if (wixInstallDir.IsNotEmpty() && Directory.Exists(wixInstallDir))
            {
                return Path.GetFullPath(wixInstallDir.PathJoin("bin"));
            }

            // C:\Program Files (x86)\WiX Toolset v3.11\bin\candle.exe
            // Try a secondary location
            wixInstallDir = Directory.GetDirectories(Utils.ProgramFilesDirectory, "WiX Toolset v3*")
                                     .Order()
                                     .LastOrDefault();

            if (wixInstallDir.IsNotEmpty() && Directory.Exists(wixInstallDir))
            {
                return Path.GetFullPath(wixInstallDir.PathJoin("bin"));
            }

            //The global packages may be redirected with environment variable
            //https://docs.microsoft.com/en-us/nuget/consume-packages/managing-the-global-packages-and-cache-folders

            string wixBinPackageDir;
            var nugetPackagesEnvironmentVariable = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
            if (nugetPackagesEnvironmentVariable.IsNotEmpty() && Directory.Exists(nugetPackagesEnvironmentVariable))
            {
                wixBinPackageDir = Path.Combine(nugetPackagesEnvironmentVariable, "wixsharp.wix.bin");
            }
            else
            {
                wixBinPackageDir = @"%userprofile%\.nuget\packages\wixsharp.wix.bin".ExpandEnvVars();
            }

            if (Directory.Exists(wixBinPackageDir))
            {
                Version greatestWixBinVersion = System.IO.Directory.GetDirectories(wixBinPackageDir)
                                                                   .Select(dirPath => new Version(dirPath.PathGetFileName()))
                                                                   .OrderDescending()
                                                                   .FirstOrDefault();

                if (greatestWixBinVersion != null)
                {
                    return wixBinPackageDir.PathJoin(greatestWixBinVersion.ToString(), @"tools\bin");
                }
            }

            string samplesWixBinDir = @"..\..\Wix_bin\bin".PathGetFullPath();

            if (Directory.Exists(samplesWixBinDir))
                return samplesWixBinDir;

            samplesWixBinDir = @"..\..\..\Wix_bin\bin".PathGetFullPath();

            if (Directory.Exists(samplesWixBinDir))
                return samplesWixBinDir;

            samplesWixBinDir = @"..\..\..\..\Wix_bin\bin".PathGetFullPath();

            if (Directory.Exists(samplesWixBinDir))
                return samplesWixBinDir;

            samplesWixBinDir = @"..\..\..\..\..\Wix_bin\bin".PathGetFullPath();

            if (Directory.Exists(samplesWixBinDir))
                return samplesWixBinDir;

            if (throwOnError)
                throw new WixSharpException("WiX binaries cannot be found. Wix# is capable of automatically finding WiX tools only if " +
                                            "WiX Toolset installed. In all other cases you need to set the environment variable " +
                                            "WIXSHARP_WIXDIR or WixSharp.Compiler.WixLocation to the valid path to the WiX binaries.\n" +
                                            "WiX binaries can be brought to the build environment by either installing WiX Toolset, " +
                                            "downloading Wix# suite or by adding WixSharp.wix.bin NuGet package to your project.");
            return null;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public static class VSLocator
    {
        /// <summary>
        /// Searches for the specified Visual Studio project implementing custom Managed UI and adds corresponding 'include wxi'
        /// statement of the WXS definition.
        /// </summary>
        /// <param name="project">The project to add custom UI to.</param>
        /// <param name="projectName">Name of the custom UI definition project.</param>
        /// <exception cref="System.Exception">Cannot find UI project `{projectName}`. You may solve this problem by explicitly adding " +
        ///                     $"UI wxi file with `project.AddXmlInclude(@\"..\\{projectName}\\wix\\{projectName}.wxi\"`</exception>
        public static void AddUIProject(this ManagedProject project, string projectName)
        {
            var solutionDir = FindVsProjectPath().PathGetDirName();
            var uiProjectDir = System.IO.Directory
                .GetFiles(solutionDir, $"{projectName}*.csproj", System.IO.SearchOption.AllDirectories)
                .FirstOrDefault()?
                .PathGetDirName();

            var wxiFile = uiProjectDir.PathJoin($"wix\\{projectName}.wxi");

            if (uiProjectDir.IsEmpty() || !wxiFile.FileExists())
                throw new Exception(
                    $"Cannot find UI project `{projectName}`. You may solve this problem by explicitly adding " +
                    $"UI wxi file with `project.AddXmlInclude(@\"..\\{projectName}\\wix\\{projectName}.wxi\"`");

            project.AddXmlInclude(wxiFile);
        }

        static string FindVsProjectPath()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var outdir = asm.PathGetDirName();

            // running from the proj dir
            if (outdir.PathCombine("bin").PathExists() && outdir.PathCombine("obj").PathExists())
                return outdir;

            // running from the proj/bin/debug dir
            for (int i = 0; i < 6; i++)
            {
                if (outdir.PathGetFileName() == "bin")
                    return outdir.PathGetDirName();

                outdir = outdir.PathGetDirName();
            }

            return null;
        }
    }
}