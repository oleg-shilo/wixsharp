//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project =
            new ManagedProject("TestProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\1\MyApp.exe"),
                    new File(@"Files\1\MyApp.cs"),
                    new File(@"Files\1\readme.txt")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.Version = new Version("1.0.709.10040");

        project.MajorUpgrade = new MajorUpgrade
        {
            //AllowSameVersionUpgrades = true, //uncomment this if the the upgrade version is different by only the fourth field
            Schedule = UpgradeSchedule.afterInstallInitialize,
            DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
        };

        project.BeforeInstall += project_BeforeInstall;

        project.PreserveTempFiles = true;

        Compiler.BuildMsi(project, "setup.msi");
    }
    static void project_BeforeInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "BeforeInstall");
    }
}



