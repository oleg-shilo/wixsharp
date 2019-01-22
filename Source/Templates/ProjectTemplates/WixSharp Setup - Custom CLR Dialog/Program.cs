using System;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

// DON'T FORGET to update NuGet package "WixSharp".
// NuGet console: Update-Package WixSharp
// NuGet Manager UI: updates tab

namespace $safeprojectname$
{
    public class Program
    {
        static void Main()
        {
            // This project type has been superseded with the EmbeddedUI based "WixSharp Managed Setup - Custom Dialog"
            // project type. Which provides by far better final result and user experience.
            // However due to the Burn limitations (see this discussion: https://wixsharp.codeplex.com/discussions/645838)
            // currently "Custom CLR Dialog" is the only working option for having bootstrapper silent UI displaying
            // individual MSI packages UI implemented in managed code.

            var project = new Project("MyProduct",
                             new Dir(@"%ProgramFiles%\My Company\My Product",
                                 new File("Program.cs")));

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");

            //Schedule custom dialog between InsallDirDlg and VerifyReadyDlg standard MSI dialogs.
            project.InjectClrDialog("ShowCustomDialog", NativeDialogs.InstallDirDlg, NativeDialogs.VerifyReadyDlg);
            //remove LicenceDlg
            project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
            //reference assembly that is needed by the custom dialog
            //project.DefaultRefAssemblies.Add(<External Asm Location>);

            //project.SourceBaseDir = "<input dir path>";
            project.OutDir = "bin";

            project.BuildMsi();
        }

        [CustomAction]
        public static ActionResult ShowCustomDialog(Session session)
        {
            return WixCLRDialog.ShowAsMsiDialog(new CustomDialog(session));
        }
    }
}