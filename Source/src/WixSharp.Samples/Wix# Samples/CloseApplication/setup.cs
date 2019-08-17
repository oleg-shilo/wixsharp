//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"readme.txt")),
                new ManagedAction(CustonActions.MyAction, Return.ignore, When.After, Step.InstallFinalize, Condition.NOT_Installed),
                new CloseApplication(new Id("notepad"), "notepad.exe", true, false)
                {
                    Timeout = 15
                });

        project.GUID = new Guid("99EF6ABA-14C4-47A8-903E-1AE82BF052CA");
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}

public class CustonActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        System.Diagnostics.Process.Start("Notepad.exe", session["INSTALLDIR"] + @"\readme.txt");
        return ActionResult.Success;
    }
}