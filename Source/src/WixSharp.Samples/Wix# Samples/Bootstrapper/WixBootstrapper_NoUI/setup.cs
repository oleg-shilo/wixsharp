//css_dir ..\..\..\;
//css_ref WixSharp.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;

using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using sys = System.Reflection;
using WixSharp;
using WixSharp.Bootstrapper;
using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;
using System.Diagnostics;

public class InstallScript
{
    static public void Main(string[] args)
    {
        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")))
            {
                InstallScope = InstallScope.perMachine
            };

        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");
        productProj.LicenceFile = "License.rtf";
        var productMsi = productProj.BuildMsi();

        var bootstrapper =
                new Bundle("My Product Suite",
                    new PackageGroupRef("NetFx40Web"),
                    new MsiPackage(productMsi)
                    {
                        Id = "MyProductPackageId",
                        DisplayInternalUI = true
                    });

        bootstrapper.SuppressWixMbaPrereqVars = true; //needed because NetFx40Web also defines WixMbaPrereqVars
        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889c");

        //if primary package Id is not defined then the last package will be treated as the primary one
        bootstrapper.Application = new SilentBootstrapperApplication();

        //use this custom BA to modify its behavior in order to meet your requirements
        //bootstrapper.Application = new ManagedBootstrapperApplication("%this%");

        bootstrapper.PreserveTempFiles = true;
        bootstrapper.Build("app_setup");
    }
}