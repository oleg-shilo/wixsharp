//css_dir ..\..\;
// //css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Install Files\bin\Debug\net472\WixToolset.Dtf.WindowsInstaller.dll
//css_ref D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Install Files\bin\Debug\net472\WixToolset.Mba.Core.dll

//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI;
using WixToolset.Dtf.WindowsInstaller;

static class Script
{
    static public void Main()
    {
        var project =
            new ManagedProject("MyProduct",
                new Dir(@"AppDataFolder\My Company\My Product",
                    new File("setup.cs")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.UI = WUI.WixUI_ProgressOnly;

        project.BeforeInstall += Project_BeforeInstall;
        project.AfterInstall += Project_AfterInstall; // is already elevated (deferred by default)

        bool installPerUser = false;
        if (installPerUser)
        {
            project.Scope = InstallScope.perUser;
            project.BeforeInstallEventExecution = EventExecution.ExternalElevatedProcess;
        }
        else
        {
            project.Scope = InstallScope.perMachine;
            project.BeforeInstallEventExecution = EventExecution.MsiSessionScopeDeferred;
            // you can use ExternalElevatedProcess too
        }

        project.BuildMsi();
    }

    private static void Project_BeforeInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "Project_BeforeInstall");
        e.Result = ActionResult.UserExit; // canceling the install here
    }

    private static void Project_AfterInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "AfterInstall");
    }
}