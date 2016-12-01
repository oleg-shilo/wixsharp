using Microsoft.Deployment.WindowsInstaller;
using System.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

public class MultiStepDialogSetup
{
    public static void Build()
    {
        var project = new Project("CustomDialogTest",  
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File("setup.exe")));

        //Injects CLR dialog MultiStepCustomDialog between MSI dialogs InsallDirDlg and VerifyReadyDlg.
        //Passes custom action ShowMultiStepCustomDialog for instantiating and popping up the CLR dialog.
        project.InjectClrDialog("ShowMultiStepCustomDialog", NativeDialogs.InstallDirDlg, NativeDialogs.VerifyReadyDlg)
               .RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    [CustomAction]
    public static ActionResult ShowMultiStepCustomDialog(Session session)
    {
        return WixCLRDialog.ShowAsMsiDialog(new MultiStepCustomDialog(session));
    }
}
