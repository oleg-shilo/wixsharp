//css_ref ..\..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Project project = new Project("My Product", 
            new Dir(@"%ProgramFiles%\My Company\My Product"),
            new ManagedAction(CustonActions.CABegin,
                              Return.ignore, 
                              When.Before, 
                              Step.LaunchConditions, 
                              Condition.NOT_Installed, 
                              Sequence.InstallUISequence),
            new ManagedAction(CustonActions.CAEnd,
                              Return.ignore,
                              When.After,
                              Step.InstallFinalize,
                              Condition.NOT_Installed,
                              Sequence.InstallExecuteSequence)); 
        
        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b"); 
        project.UI = WUI.WixUI_ProgressOnly;

        project.OutFileName = "setup"; 

        Compiler.BuildMsi(project);
    }
}

public class CustonActions
{
    [CustomAction]
    public static ActionResult CABegin(Session session)
    {
        MessageBox.Show("This is the first CustomAction!", "Embedded Managed CA"); 
        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult CAEnd(Session session)
    {
        MessageBox.Show("This is the second CustomAction!", "Embedded Managed CA");
        return ActionResult.Success;
    }
}


