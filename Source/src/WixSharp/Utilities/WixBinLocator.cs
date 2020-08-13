using System;
using System.Linq;
using System.IO;

namespace WixSharp
{
    static class WixBinLocator
    {
        public static string FindWixSdkLocation(string wixLocation)
        {
            var wixSdkLocation = Path.GetFullPath(Utils.PathCombine(wixLocation, @"..\sdk"));

            if (!Directory.Exists(wixSdkLocation))
            {
                wixSdkLocation = Path.GetFullPath(Utils.PathCombine(wixLocation, "sdk")); //NuGet package shovels the dirs
                if (!Directory.Exists(wixSdkLocation))
                    throw new Exception("WiX SDK binaries cannot be found. Please set WixSharp.Compiler.WixSdkLocation to valid path to the Wix SDK binaries.");
            }

            return wixSdkLocation;
        }

        public static string FindWixBinLocation()
        {
            // See if the command line was set for this property
            var msBuildArgument = Environment.GetCommandLineArgs().FirstPrefixedValue("/WIXBIN:");
            if (msBuildArgument.IsNotEmpty() && Directory.Exists(msBuildArgument))
            {
                return Path.GetFullPath(msBuildArgument);
            }

            // Now check to see if the environment variable was set
            var environmentVar = Environment.GetEnvironmentVariable("WIXSHARP_WIXDIR");
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

            string wixBinPackageDir = @"%userprofile%\.nuget\packages\wixsharp.wix.bin".ExpandEnvVars();

            if (Directory.Exists(wixBinPackageDir))
            {
                Version greatestWixBinVersion = System.IO.Directory.GetDirectories(wixBinPackageDir)
                                                                   .Select(dirPath => new Version(dirPath.PathGetFileName()))
                                                                   .OrderDescending()
                                                                   .FirstOrDefault();

                if (greatestWixBinVersion != null)
                {
                    wixBinPackageDir.PathJoin(greatestWixBinVersion.ToString(), @"tools\bin");
                }
            }

            throw new WixSharpException("WiX binaries cannot be found. Wix# is capable of automatically finding WiX tools only if " +
                                        "WiX Toolset installed. In all other cases you need to set the environment variable " +
                                        "WIXSHARP_WIXDIR or WixSharp.Compiler.WixLocation to the valid path to the WiX binaries.\n" +
                                        "WiX binaries can be brought to the build environment by either installing WiX Toolset, " +
                                        "downloading Wix# suite or by adding WixSharp.wix.bin NuGet package to your project.");
        }
    }
}