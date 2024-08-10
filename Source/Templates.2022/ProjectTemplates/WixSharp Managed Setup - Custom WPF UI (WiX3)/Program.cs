using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.WPF;

namespace $safeprojectname$
{
    internal class Program
    {
        static void Main()
        {
            var project = new ManagedProject("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                  new File("Program.cs")));

            project.GUID = new Guid("$guid1$");

            // project.ManagedUI = ManagedUI.DefaultWpf; // all stock UI dialogs

            //custom set of UI WPF dialogs
            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs.Add<$safeprojectname$.WelcomeDialog>()
                                            .Add<$safeprojectname$.LicenceDialog>()
                                            .Add<$safeprojectname$.FeaturesDialog>()
                                            .Add<$safeprojectname$.InstallDirDialog>()
                                            .Add<$safeprojectname$.ProgressDialog>()
                                            .Add<$safeprojectname$.ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<$safeprojectname$.MaintenanceTypeDialog>()
                                           .Add<$safeprojectname$.FeaturesDialog>()
                                           .Add<$safeprojectname$.ProgressDialog>()
                                           .Add<$safeprojectname$.ExitDialog>();

            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }
    }
}