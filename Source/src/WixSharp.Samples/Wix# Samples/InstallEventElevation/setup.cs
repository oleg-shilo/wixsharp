using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI;

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

        // project.Scope = InstallScope.perMachine;
        project.Scope = InstallScope.perUser;

        project.BeforeInstall += Project_BeforeInstall;
        project.AfterInstall += Project_AfterInstall;

        // project.DefaultRefAssemblies.Add(@"..\..\..\MsiEventHost\output\MsiEventHost.exe");
        // project.DefaultDeferredProperties = ",ManagedProjectElevatedEvents";
        // project.AddProperty(new Property(
        //     "ManagedProjectElevatedEvents", "BeforeInstall|AfterInstall"));

        project.BeforeInstallEventExecution = EventExecution.ExternalElevatedProcess;
        project.BuildMsi();
    }

    private static void Project_BeforeInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "Project_BeforeInstall");
        // e.Result = WixToolset.Dtf.WindowsInstaller.ActionResult.UserExit;
    }

    private static void Project_AfterInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "AfterInstall");
    }
}