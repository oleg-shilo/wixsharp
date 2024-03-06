using System;
using System.Windows.Forms;
using WixSharp;
using $safeprojectname$.Dialogs;
using WixToolset.Dtf.WindowsInstaller;

class Program
{
    static void Main()
    {
        WxiBuilder.UI(project =>
        {
            AutoElements.EnableUACRevealer = true;
            AutoElements.UACWarning = "Wait for UAC prompt to appear on the taskbar.";

            //custom set of standard UI dialogs
            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                            .Add<LicenceDialog>()
                                            .Add<SetupTypeDialog>()
                                            .Add<FeaturesDialog>()
                                            .Add<InstallDirDialog>()
                                            .Add<ProgressDialog>()
                                            .Add<ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
                                           .Add<FeaturesDialog>()
                                           .Add<ProgressDialog>()
                                           .Add<ExitDialog>();

            project.ManagedUI.Icon = "app.ico";
            project.UIInitialized += e => MessageBox.Show("UIInitialized", "$safeprojectname$");
            project.UILoaded += e => MessageBox.Show("Project_UILoaded", "$safeprojectname$");
        });
    }
}