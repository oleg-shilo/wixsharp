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
        // MSI/WiX upgrade support (e.g. via MajorUpgrade element) works correctly regardless of the UI technology.
        // However it hooks into UI sequence to show upgrade failure message only with native UI but not with the
        // EmbeddedUI (ManagedUI). Thus prompting the user about version incompatibility (downgrading) and skipping
        // the rest of the UI steps needs to be done explicitly if EmbeddedUI (ManagedUI) is in place.
        //
        // Note: in silent mode neither native nor ManagedUI is engaged so MajorUpgrade logs the error and exit
        // without any prompt if incompatibility is detected.

        UniversalApproach();
        // NativeUIApproach();
        // ManagedUIAproach();
        // ManagedUICustomCheckAproach();
    }

    static ManagedProject CreateProject()
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

        return project;
    }

    static void NativeUIApproach()
    {
        ManagedProject project = CreateProject();

        Compiler.BuildMsi(project, "setup.msi");
    }

    static void UniversalApproach()
    {
        ManagedProject project = CreateProject();

        // MajorUpgrade.Default has ScheduleManagedUICheck set to 'true'
        // and this in turn will trigger internal call to ScheduleDowngradeUICheck
        // if project.ManagedUI is set.

        project.ManagedUI = ManagedUI.Default;
        project.MajorUpgrade = MajorUpgrade.Default;

        Compiler.BuildMsi(project, "setup.msi");
    }

    static public void ManagedUIAproach()
    {
        ManagedProject project = CreateProject();

        // Note the `ScheduleDowngradeUICheck` method parameters in the code below are for demo purpose only.
        // They can be completely omitted as in this sample they are identical to their default values.

        project.ManagedUI = ManagedUI.Default;
        project.ScheduleDowngradeUICheck(
           "Later version of the product is already installed : ${installedVersion}",
            (thisVersion, installedVersion) => thisVersion <= installedVersion);

        Compiler.BuildMsi(project, "setup.msi");
    }

    static public void ManagedUICustomCheckAproach()
    {
        ManagedProject project = CreateProject();

        // Note the `project.UIInitialized += ...` code below is for demo purpose only. It to demonstrates custom handling
        // of downgrade condition. This code can be replaced with a single equivalent call `project.ScheduleDowngradeUICheck();`

        project.ManagedUI = ManagedUI.Default;
        project.UIInitialized += (SetupEventArgs e) =>
        {
            Version installedVersion = e.Session.LookupInstalledVersion();
            Version thisVersion = e.Session.QueryProductVersion();

            if (thisVersion <= installedVersion)
            {
                MessageBox.Show("Later version of the product is already installed : " + installedVersion);

                e.ManagedUI.Shell.ErrorDetected = true;
                e.Result = ActionResult.UserExit;
            }
        };

        Compiler.BuildMsi(project, "setup.msi");
    }
}