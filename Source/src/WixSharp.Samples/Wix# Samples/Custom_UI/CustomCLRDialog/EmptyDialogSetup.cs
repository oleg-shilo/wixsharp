using Microsoft.Deployment.WindowsInstaller;
using System.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

public class EmptyDialogSetup
{
    public static void Build()
    {
        var project = new Project("CustomDialogTest",  
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File("setup.exe")));

        //Injects CLR dialog EmptyDialog between MSI dialogs InsallDirDlg and VerifyReadyDlg.
        //Passes custom action ShowCustomDialog for instantiating and popping up the CLR dialog.

        project.InjectClrDialog("ShowCustomDialog", NativeDialogs.InstallDirDlg, NativeDialogs.VerifyReadyDlg)
               .RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

        project.Actions.OfType<ManagedAction>()
                       .Single()
                       .AddRefAssembly(typeof(ExternalAsm.Utils).Assembly.Location);

        //or as the next commented code line
        //project.DefaultRefAssemblies.Add(typeof(ExternalAsm.Utils).Assembly.Location);

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    [CustomAction]
    public static ActionResult ShowCustomDialog(Session session)
    {
        return WixCLRDialog.ShowAsMsiDialog(new EmptyDialog(session));
    }
}
