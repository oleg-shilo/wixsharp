using System;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

public class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

        //Injects CLR dialog EmptyDialog between MSI dialogs InsallDirDlg and VerifyReadyDlg.
        //Passes custom action ShowCustomDialog for instantiating and popping up the CLR dialog.
        project.InjectClrDialog("ShowCustomDialog", Dialogs.InstallDirDlg, Dialogs.VerifyReadyDlg);

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
    
    [CustomAction]
    public static ActionResult ShowCustomDialog(Session session)
    {
        return WixCLRDialog.ShowAsMsiDialog(new EmptyDialog(session));
    }
}



