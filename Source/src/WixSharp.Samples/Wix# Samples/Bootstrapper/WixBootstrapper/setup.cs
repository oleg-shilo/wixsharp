//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref System.Xml.Linq.dll;
//css_ref ..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using io = System.IO;

public class InstallScript
{
    static public void Main()
    {
        string productMsi = BuildMainMsi();
        string crtMsi = BuildCrtMsi();

        //---------------------------------------------------------

        var msiOnlinePackage = new MsiPackage(crtMsi) //demo for downloadable msi package
        {
            Vital = true,
            Compressed = false,
            DisplayInternalUI = true,
            DownloadUrl = @"https://dl.dropboxusercontent.com/....../CRT.msi"
        };

        // Compiler.AutoGeneration.SuppressForBundleUndefinedIds = false;
        // Compiler.AutoGeneration.LegacyDefaultIdAlgorithm = false;

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    //new ExePackage(@"..\redist\dotNetFx40_Full_x86_x64.exe") //just a demo sample
                    //{
                    //     Name = "Microsoft .NET Framework 4.0",
                    //     InstallCommand = "/passive /norestart",
                    //     Permanent = true,
                    //     Vital = true,
                    //     DetectCondition = "Netfx4FullVersion AND (NOT VersionNT64 OR Netfx4x64FullVersion)",
                    //     Compressed = true
                    //},

                    //msiOnlinePackage, //just a demo sample

                    new MsiPackage(crtMsi)
                    {
                        DisplayInternalUI = true,
                        Visible = true,
                        MsiProperties = "PACKAGE_PROPERTY=[BundleVariable]"
                    },
                    new MsiPackage(productMsi)
                    {
                        DisplayInternalUI = true,
                        Payloads = new[] { "script.dll".ToPayload() }
                    });

        bootstrapper.AboutUrl = "https://github.com/oleg-shilo/wixsharp/";
        bootstrapper.IconFile = "app_icon.ico";
        bootstrapper.Version = Tasks.GetVersionFromFile(productMsi); //will extract "product version"
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application.LogoFile = "logo.png";

        // adding themes
        // var themes = new[]
        //     {
        //         new Payload("managedsetup.wxl") { Name = "1033\thm.wxl" }
        //     };
        // bootstrapper.Application.Payloads = themes;

        bootstrapper.Application.LicensePath = "licence.html";  //HyperlinkLicense app with embedded license file
        // bootstrapper.Application.LicensePath = "licence.rtf"; //RtfLicense app with embedded license file
        // bootstrapper.Application.LicensePath = "http://opensource.org/licenses/MIT"; //HyperlinkLicense app with online license file
        // bootstrapper.Application.LicensePath = null; //HyperlinkLicense app with no license

        bootstrapper.Application.AttributesDefinition = "ShowVersion=yes; ShowFilesInUse=yes";
        bootstrapper.Include(WixExtension.Util);

        bootstrapper.IncludeWixExtension(@"WixDependencyExtension.dll", "dep", "http://schemas.microsoft.com/wix/DependencyExtension");
        bootstrapper.AttributesDefinition = "{dep}ProviderKey=01234567-8901-2345-6789-012345678901";

        // The code below sets WiX variables 'Netfx4FullVersion' and 'AdobeInstalled'. Note it has no affect on
        //the runtime behaviour and 'FileSearch' and "RegistrySearch" are only provided as an example.
        bootstrapper.AddWixFragment("Wix/Bundle",
                                    new UtilRegistrySearch
                                    {
                                        Root = RegistryHive.LocalMachine,
                                        Key = @"SOFTWARE\Microsoft\Net Framework Setup\NDP\v4\Full",
                                        Value = "Version",
                                        Variable = "Netfx4FullVersion"
                                    },
                                    new UtilFileSearch
                                    {
                                        Path = @"[ProgramFilesFolder]Adobe\adobe.exe",
                                        Result = SearchResult.exists,
                                        Variable = "AdobeInstalled"
                                    });

        // bootstrapper.AddXml("Wix/Bundle", "<Log PathVariable=\"LogFileLocation\"/>");

        bootstrapper.AddXmlElement("Wix/Bundle", "Log", "PathVariable=LogFileLocation");
        bootstrapper.Variables = new[] { new Variable("LogFileLocation", @"C:\temp\setup.log") { Overridable = true } };
        // or
        // bootstrapper.Variables = "BundleVariable=333".ToStringVariables();
        // bootstrapper.Variables = Variables.ToStringVariables("BundleVariable=333");

        bootstrapper.PreserveTempFiles = true;

        // Add MspPackage manually (demo only).
        // In the future releases the direct support for MspPackage element will be added.
        // bootstrapper.WixSourceGenerated += (doc) => doc.FindSingle("Chain")
        //                                                .AddElement("MspPackage",
        //                                                            "SourceFile=Patch.msp; Slipstream=no");

        // bootstrapper.WixSourceGenerated += doc => doc.FindSingle("WixStandardBootstrapperApplication")
        //                                              .AddAttributes("ShowVersion=yes; ShowFilesInUse=no");

        //in real life scenarios suppression may need to be enabled (see SuppressWixMbaPrereqVars documentation)
        //bootstrapper.SuppressWixMbaPrereqVars = true;
        var setup = bootstrapper.Build("app_setup");
        Console.WriteLine(setup);
        //---------------------------------------------------------

        if (io.File.Exists(productMsi)) io.File.Delete(productMsi);
        if (io.File.Exists(crtMsi)) io.File.Delete(crtMsi);
    }

    static public string BuildMainMsi()
    {
        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt"),
                    new File("logo.png")))
            { InstallScope = InstallScope.perMachine };

        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        productProj.Version = new Version("2.0.0.0");
        productProj.MajorUpgrade = new MajorUpgrade
        {
            Schedule = UpgradeSchedule.afterInstallInitialize,
            DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
        };

        productProj.PreserveTempFiles = true;

        return productProj.BuildMsi();
    }

    static public string BuildCrtMsi()
    {
        var crtProj =
            new ManagedProject("CRT",
                new Dir(@"%ProgramFiles%\My Company\CRT",
                    new File("readme.txt")))
            { InstallScope = InstallScope.perMachine };
        crtProj.Load += CrtProj_Load;
        return crtProj.BuildMsi();
    }

    private static void CrtProj_Load(SetupEventArgs e)
    {
        MessageBox.Show("DOINSTALL: " + e.Session["DOINSTALL"]);
    }
}