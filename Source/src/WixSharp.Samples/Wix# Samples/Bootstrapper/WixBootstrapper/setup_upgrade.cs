//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref System.Xml.Linq.dll;
//css_ref ..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using io = System.IO;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

public class InstallScript
{
    static public void Main(string[] args)
    {
        Build_V1();
        Build_V2();
    }

    static public void Build_V1()
    {
        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")))
            { InstallScope = InstallScope.perMachine };

        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        productProj.Version = new Version("1.0.0.0");

        productProj.MajorUpgrade = new MajorUpgrade
        {
            Schedule = UpgradeSchedule.afterInstallInitialize,
            DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
        };

        string productMsi = productProj.BuildMsi();

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    new MsiPackage(productMsi) { DisplayInternalUI = true });

        bootstrapper.AboutUrl = "https://wixsharp.codeplex.com/";
        bootstrapper.IconFile = "app_icon.ico";
        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application.LogoFile = "logo.png";
        bootstrapper.Application.LicensePath = "licence.html";  //HyperlinkLicense app with embedded license file
        bootstrapper.IncludeWixExtension(WixExtension.Util);

        bootstrapper.Build("setup_v1.exe");
        //---------------------------------------------------------
        if (io.File.Exists(productMsi))
            io.File.Delete(productMsi);
    }

    static public void Build_V2()
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

        string productMsi = productProj.BuildMsi();

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    new MsiPackage(productMsi) { DisplayInternalUI = true });

        bootstrapper.AboutUrl = "https://wixsharp.codeplex.com/";
        bootstrapper.IconFile = "app_icon.ico";
        bootstrapper.Version = new Version("2.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application.LogoFile = "logo.png";
        bootstrapper.Application.LicensePath = "licence.html";  //HyperlinkLicense app with embedded license file
        bootstrapper.IncludeWixExtension(WixExtension.Util);

        bootstrapper.Build("setup_v2.exe");
        //---------------------------------------------------------
        if (io.File.Exists(productMsi))
            io.File.Delete(productMsi);
    }
}

