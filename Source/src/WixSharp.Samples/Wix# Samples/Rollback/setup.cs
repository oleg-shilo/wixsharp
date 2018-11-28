//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new Project
        {
            Name = "CustomActionTest",
            UI = WUI.WixUI_ProgressOnly,

            Dirs = new[]
            {
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("registrator_exe".ToId(), @"Files\Registrator.exe"))
            },
            Actions = new WixSharp.Action[]
            {
                new InstalledFileAction("registrator_exe", "", Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed, "registrator_exe", "/u") {Execute = Execute.deferred},
                new InstalledFileAction("registrator_exe", "/u", Return.check, When.Before, Step.RemoveFiles, Condition.Installed, "registrator_exe", "") {Execute = Execute.deferred},

                new ElevatedManagedAction(CustomActions.Install, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed, CustomActions.Rollback)
                {
                    UsesProperties = "Prop=Install", // need to tunnel properties since ElevatedManagedAction is a deferred action
                    RollbackArg = "Prop=Rollback"
                },

                new CustomActionRef("WixFailWhenDeferred", When.Before, Step.InstallFinalize, "1"),
            }
        };

        project.Include(WixExtension.Util);

        Compiler.PreserveTempFiles = true;

        Compiler.BuildMsi(project);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult Install(Session session)
    {
        MessageBox.Show(session.Property("Prop"), "Install");

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult Rollback(Session session)
    {
        MessageBox.Show(session.Property("Prop"), "Rollback");

        return ActionResult.Success;
    }
}