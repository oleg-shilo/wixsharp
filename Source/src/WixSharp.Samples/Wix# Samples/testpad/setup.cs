//using PirrosLibrary;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using WixSharp;

class Constants
{
    public static string PluginVersion = "2.3.0";
}

namespace Pirros.installer.wixsharp
{
    class Program
    {
#if DEBUG
        private static readonly string Configuration = "Debug";
#else
    private static readonly string Configuration = "Release";
#endif

        public static void Main()
        {
            return; // REMOVE THIS LINE TO ENABLE BUILDING

            Environment.CurrentDirectory = @"D:\dev\support\wixsharp-issues\Pirros\WixSharp Setup1\WixSharp Setup1";
            Constants.PluginVersion = "2.4.0";

            string Version = Constants.PluginVersion; // READ FROM PIRROS LIBRARY
            Guid ProductId = GenerateProductId("Pirros" + Constants.PluginVersion);
            Guid UpgradeCode = new Guid("6476F6DF-EB27-4CAB-9790-5FE5F1C39731"); // DO NOT TOUCH

            Project project =
            new Project("Pirros",
                new Media { EmbedCab = true }, // copied from old installer, don't know what it does
                CreateRevitAddinDir(2020)// ,
                                         // CreateRevitAddinDir(2021),
                                         // CreateRevitAddinDir(2022),
                                         // CreateRevitAddinDir(2023),
                                         // CreateRevitAddinDir(2024),
                                         // CreateRevitAddinDir(2025)
                       );

            project.Scope = InstallScope.perUser;

            project.Name = "Pirros Revit Plugin";
            project.ProductId = ProductId;
            project.UpgradeCode = UpgradeCode;
            //project.GUID = new Guid("6476F6DF-EB27-4CAB-9790-5FE5F1C39735");

            project.Version = new Version(Version);
            project.Description = "Revit Plugin to interact with Pirros.com";
            project.ControlPanelInfo.Manufacturer = "Pirros Inc";
            project.ControlPanelInfo.ProductIcon = @".\Assets\icon.ico";
            project.ControlPanelInfo.UrlInfoAbout = "https://www.pirros.com";

            project.MajorUpgrade = new MajorUpgrade
            {
                DowngradeErrorMessage = "A newer version of Pirros Plugin is already installed.",
            };

            project.UI = WUI.WixUI_Minimal;

            project.WixVariables = new Dictionary<string, string>
            {
                { "WixUILicenseRtf", @".\Assets\License.rtf" },
                { "WixUIBannerBmp", @".\Assets\Banner.png" },
                { "WixUIDialogBmp", @".\Assets\Background.png" }
            };

            project.OutFileName = $"Pirros.installer-V{Version}{(Configuration == "Debug" ? "-dev" : "")}";

            project.SourceBaseDir = @"D:\dev\support\wixsharp-issues\Pirros\WixSharp Setup1\WixSharp Setup1";

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
                new File(@"..\Pirros\Pirros.addin"),
                new Dir("Pirros",
                    Files.FromBuildDir($@"..\Pirros\bin\{Configuration}{year}\{framework}"),
                    new Dir("Resources",
                        new Files(@"..\Pirros\Resources\*.*"))));
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