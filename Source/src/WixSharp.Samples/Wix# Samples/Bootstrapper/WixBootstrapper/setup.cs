//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref System.Xml.Linq.dll;
//css_ref ..\..\..\Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
using System;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;
using io = System.IO;

public class InstallScript
{
    static public void Main()
    {
        var crtMsi = BuildCrtMsi();
        var productMsi = BuildMainMsi();

        // Simple(crtMsi);
        Standard(crtMsi, productMsi);
        // Complex(crtMsi, productMsi);
    }

    static public void Simple(string msi)
    {
        var bundle = new Bundle("My Product")
        {
            Version = Version.Parse("1.0.0.0"),
            UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb24889b")
        };

        // will show MSI UI instead of BA UI
        bundle.Application = new WixInternalUIBootstrapperApplication { LogoFile = "product_logo.png" };
        bundle.Chain.Add(new MsiPackage(msi));

        bundle.Build("my.exe");
    }

    static public void Standard(string dependencyMsi, string productMsi)
    {
        var bundle = new Bundle("My Product")
        {
            Version = Version.Parse("1.0.0.0"),
            UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb24889b")
        };

        bundle.Chain.Add(new PackageGroupRef("NetFx462Web"));
        bundle.Chain.Add(new MsiPackage(dependencyMsi));
        bundle.Chain.Add(new MsiPackage(productMsi));

        bundle.Application.Theme = Theme.rtfLicense;
        bundle.Application.LicensePath = "licence.rtf";
        bundle.Application.LogoFile = "product_logo.png";

        bundle.PreserveTempFiles = true;

        bundle.Build("my.exe");
    }

    static public void Complex(string crtMsi, string productMsi)
    {
        var msiOnlinePackage = new MsiPackage(crtMsi) //demo for downloadable msi package
        {
            Vital = true,
            Compressed = false,
            DownloadUrl = @"https://dl.dropboxusercontent.com/....../CRT.msi"
        };

        // Compiler.AutoGeneration.SuppressForBundleUndefinedIds = false;
        // Compiler.AutoGeneration.LegacyDefaultIdAlgorithm = false;

        var bootstrapper =
                new Bundle("My Product",
                    // new PackageGroupRef("NetFx462Web"),
                    //new ExePackage(@"..\redist\dotNetFx40_Full_x86_x64.exe") //just a demo sample
                    //{
                    //     Name = "Microsoft .NET Framework 4.0",
                    //     InstallCommand = "/passive /norestart",
                    //     Permanent = true,
                    //     Vital = true,
                    //     DetectCondition = "Netfx4FullVersion AND (NOT VersionNT64 OR Netfx4x64FullVersion)",
                    //     Compressed = true
                    //},

                    //msiOnlinePackage, // just a demo sample

                    new MsiPackage(crtMsi)
                    {
                        Visible = true,
                        MsiProperties = "INSTALLDIR=[InstallFolder]",
                        InstallCondition = "MyCheckbox<>0"
                    },
                    // new MspPackage("Patch.msp")
                    // {
                    //     Slipstream = false
                    // },
                    new MsiPackage(productMsi)
                    {
                        MsiProperties = "INSTALLDIR=c:\\",
                        Payloads = new[] { "script.dll".ToPayload() }
                    });

        bootstrapper.AboutUrl = "https://github.com/oleg-shilo/wixsharp/";
        bootstrapper.IconFile = "app_icon.ico";
        bootstrapper.Version = Tasks.GetVersionFromFile(productMsi); //will extract "product version"
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application.LogoFile = "product_logo.png";
        bootstrapper.Application.Theme = Theme.rtfLicense;
        // bootstrapper.Application.Payloads = new[] { "product_logo.png".ToPayload() };

        // adding themes
        // var themes = new[]
        //     {
        //         new Payload("managedsetup.wxl") { Name = "1033\thm.wxl" }
        //     };
        // bootstrapper.Application.Payloads = themes;

        // bootstrapper.Application.LicensePath = "licence.html"; //HyperlinkLicense app with embedded license file
        bootstrapper.Application.LicensePath = "licence.rtf"; // RtfLicense app with embedded license file
        //                                                       // bootstrapper.Application.LicensePath = "http://opensource.org/licenses/MIT"; //HyperlinkLicense app with online license file
        //                                                       // bootstrapper.Application.LicensePath = null; //HyperlinkLicense app with no license

        // if you want to use `WixStandardBootstrapperApplication.HyperlinkSidebarLicense`
        // you need to clear bootstrapper.Application.LicensePath and uncomment the next line
        // bootstrapper.Application.LogoSideFile = "logo.png";

        bootstrapper.Application.AttributesDefinition = "ShowVersion=yes"; // you can also use bootstrapper.Application.Show* = true;
        bootstrapper.Include(WixExtension.Util);

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

        string installDir = "%ProgramFiles(x86)%".PathJoin("CompanyName", "MyApp").ExpandEnvVars();
        bootstrapper.AddXmlElement("Wix/Bundle", "Log", "PathVariable=LogFileLocation");
        bootstrapper.Variables = new[]
        {
            new Variable("LogFileLocation", @"C:\temp\setup.log") { Overridable = true },
            new Variable("MyCheckbox", "0", VariableType.numeric) { Overridable = true },
            new Variable("MyCheckboxLabel", "Install CRT?", VariableType.@string) { Overridable = true },
            // note 'InstallFolder' is a special built-in variable that can be changed by the user from the options page
            new Variable("InstallFolder", installDir) { Overridable = true }
        };
        // or
        // bootstrapper.Variables = "BundleVariable=333".ToStringVariables();
        // bootstrapper.Variables = Variables.ToStringVariables("BundleVariable=333");

        bootstrapper.PreserveTempFiles = true;

        // bootstrapper.WixSourceGenerated += doc => doc.FindSingle("WixStandardBootstrapperApplication")
        //                                              .AddAttributes("ShowVersion=yes; ShowFilesInUse=no");

        var setup = bootstrapper.Build("app_setup");
        Console.WriteLine(setup);
    }

    static public string BuildMainMsi()
    {
        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt"),
                    new File("product_logo.png")))
            { };

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
            { };

        crtProj.UI = WUI.WixUI_InstallDir;
        crtProj.Load += CrtProj_Load;

        // crtProj.BeforeInstall += args =>
        // {
        //     if (args.IsUninstalling)
        //         MessageBox.Show(args.InstallDir, "Uninstalling...");
        //     else
        //         MessageBox.Show(args.InstallDir, "Installing...");
        // };
        return crtProj.BuildMsi();
    }

    private static void CrtProj_Load(SetupEventArgs e)
    {
        MessageBox.Show("DOINSTALL: " + e.Session["DOINSTALL"]);
    }
}