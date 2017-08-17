//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI.Forms;

class Script
{
    static public void Main(string[] args)
    {
        // ManagedUIAproach();
        NativeUIApproach();
    }

    static void NativeUIApproach()
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
            //AllowSameVersionUpgrades = true, //uncomment this if the upgrade version is different by only the fourth field
            Schedule = UpgradeSchedule.afterInstallInitialize,
            DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
        };

        Compiler.BuildMsi(project, "setup.msi");
    }

    static public void ManagedUIAproach()
    {
        var project =
            new ManagedProject("TestProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\1\MyApp.exe"),
                    new File(@"Files\1\MyApp.cs"),
                    new File(@"Files\1\readme.txt")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.Version = new Version("1.0.209.10040");

        project.MajorUpgrade = new MajorUpgrade
        {
            //AllowSameVersionUpgrades = true, //uncomment this if the upgrade version is different by only the fourth field
            Schedule = UpgradeSchedule.afterInstallInitialize,
            DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
        };

        // If you want to use ManagedUI, you will need to search for any existing product installation
        // from the UI code (e.g. event handler). This is because WiX MajorUpgrade is only integrated with
        // native MSI UI and does not know how to prompt user about the version incompatibility if EmbeddedUI
        // (ManagedUI) is in place.
        // Note: in silent mode neither native nor ManagedUI is engaged so MajorUpgrade logs the error and exit
        // without any prompt if incompatibility is detected.
        project.ManagedUI = ManagedUI.Default;
        project.UIInitialized += Project_UIInitialized;

        Compiler.BuildMsi(project, "setup.msi");
    }

    static void Project_UIInitialized(SetupEventArgs e)
    {
        Version installedVersion = e.Session.LookupInstalledVersion();
        Version thisVersion = e.Session.QueryProductVersion();

        if (installedVersion != null && installedVersion >= thisVersion)
        {
            MessageBox.Show("Later version of the product is already installed : " + installedVersion);
            e.ManagedUI.Shell.ErrorDetected = true;
            e.ManagedUI.Shell.GoTo<ExitDialog>();
        }
    }
}