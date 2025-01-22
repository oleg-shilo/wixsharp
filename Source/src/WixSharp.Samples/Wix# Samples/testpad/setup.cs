//using Test1Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using WixSharp;

class Constants
{
    public static string PluginVersion = "2.3.0";
}

namespace Test1.installer.wixsharp
{
    class Program
    {
#if DEBUG
        private static readonly string Configuration = "Debug";
#else
    private static readonly string Configuration = "Release";
#endif

        static string companyName = "Demo Inc.";
        static string productName = "DllExample";
        static string productVersion = "1.0.0";

        static void Main()
        {
            Environment.CurrentDirectory = @"D:\dev\wixsharp4\Source\src\WixSharp.Samples\Wix# Samples\Install Files";
            var msixTemplate = @".\MyProduct.msix.xml";

            var startInfo = new ProcessStartInfo
            {
                FileName = "MsixPackagingTool.exe",
                Arguments = $@"create-package --template {msixTemplate} -v",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    string line = null;
                    while (null != (line = process.StandardOutput.ReadLine()))
                        Console.WriteLine(line);

                    string error = process.StandardError.ReadToEnd();
                    if (!error.IsEmpty())
                        Console.WriteLine(error);
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void Main1()
        {
            var project = new ManagedProject(productName,
                              new Dir(@"%ProgramFiles%\" + companyName + @"\" + productName + @"\" + productVersion,
                                  Files.FromBuildDir(@"D:\dev\support\wixsharp-issues\DllErrorExample\Contents")));

            project.GUID = new Guid("5de17d40-9e25-49fe-a835-36d7e0b64062");
            project.Version = new Version(productVersion);

            project.ManagedUI = ManagedUI.DefaultWpf; // all stock UI dialogs

            project.BuildMsi();
        }

        public static void Test1()
        {
            return; // REMOVE THIS LINE TO ENABLE BUILDING

            Environment.CurrentDirectory = @"D:\dev\support\wixsharp-issues\Test1\WixSharp Setup1\WixSharp Setup1";
            Constants.PluginVersion = "2.4.0";

            string Version = Constants.PluginVersion; // READ FROM Test1 LIBRARY
            Guid ProductId = GenerateProductId("Test1" + Constants.PluginVersion);
            Guid UpgradeCode = new Guid("6476F6DF-EB27-4CAB-9790-5FE5F1C39731"); // DO NOT TOUCH

            Project project =
            new Project("Test1",
                new Media { EmbedCab = true }, // copied from old installer, don't know what it does
                CreateRevitAddinDir(2020)// ,
                                         // CreateRevitAddinDir(2021),
                                         // CreateRevitAddinDir(2022),
                                         // CreateRevitAddinDir(2023),
                                         // CreateRevitAddinDir(2024),
                                         // CreateRevitAddinDir(2025)
                       );

            project.Scope = InstallScope.perUser;

            project.Name = "Test1 Revit Plugin";
            project.ProductId = ProductId;
            project.UpgradeCode = UpgradeCode;
            //project.GUID = new Guid("6476F6DF-EB27-4CAB-9790-5FE5F1C39735");

            project.Version = new Version(Version);
            project.Description = "Revit Plugin to interact with Test1.com";
            project.ControlPanelInfo.Manufacturer = "Test1 Inc";
            project.ControlPanelInfo.ProductIcon = @".\Assets\icon.ico";
            project.ControlPanelInfo.UrlInfoAbout = "https://www.Test1.com";

            project.MajorUpgrade = new MajorUpgrade
            {
                DowngradeErrorMessage = "A newer version of Test1 Plugin is already installed.",
            };

            project.UI = WUI.WixUI_Minimal;

            project.WixVariables = new Dictionary<string, string>
            {
                { "WixUILicenseRtf", @".\Assets\License.rtf" },
                { "WixUIBannerBmp", @".\Assets\Banner.png" },
                { "WixUIDialogBmp", @".\Assets\Background.png" }
            };

            project.OutFileName = $"Test1.installer-V{Version}{(Configuration == "Debug" ? "-dev" : "")}";

            project.SourceBaseDir = @"D:\dev\support\wixsharp-issues\Test1\WixSharp Setup1\WixSharp Setup1";

            // Compiler.PreserveTempFiles = true;
            Compiler.EmitRelativePaths = false;
            Compiler.PreferredComponentGuidConsistency = ComponentGuidConsistency.WithinSingleVersion;
            // var ttt = WixGuid.NewGuid("text");

            //WixGuid.ConsistentGenerationStartValue = UpgradeCode;

            project.BuildMsi();
        }

        private static Dir CreateRevitAddinDir(int year)
        {
            string framework = GetFrameworkForYear(year);

            return new Dir($@"%AppDataFolder%\Autodesk\Revit\Addins\{year}",
                new File(@"..\Test1\Test1.addin"),
                new Dir("Test1",
                    Files.FromBuildDir($@"..\Test1\bin\{Configuration}{year}\{framework}"),
                    new Dir("Resources",
                        new Files(@"..\Test1\Resources\*.*"))));
        }

        private static Guid GenerateProductId(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

                // Ensure the byte array is exactly 16 bytes long, as required for a GUID.
                // SHA256 generates 32 bytes, so we take the first 16 bytes.
                byte[] guidBytes = new byte[16];
                Array.Copy(hashBytes, guidBytes, 16);

                // Construct the GUID from the 16-byte array.
                return new Guid(guidBytes);
            }
        }

        private static string GetFrameworkForYear(int year)
        {
            return year < 2025 ? "net48" : "net8.0-windows";
        }
    }
}

//using System;
//using WixSharp;

//namespace WixSharp_Setup1
//{
//    public class Program
//    {
//        static void Main()
//        {
//            var project = new Project("MyProduct",
//                              new Dir(@"%ProgramFiles%\My Company\My Product",
//                                  new File("Program.cs")));

//            project.GUID = new Guid("e4c1d973-9881-498f-8b24-b61bcaee05d0");
//            //project.SourceBaseDir = "<input dir path>";
//            //project.OutDir = "<output dir path>";

//            project.BuildMsi();
//        }
//    }
//}