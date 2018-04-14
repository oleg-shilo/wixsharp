//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

internal class Script
{
    static public void Main()
    {
        var project = new Project("Setup",
                new PropertyRef("NETFRAMEWORK20"),
                new ManagedAction(CustomActions.MyAction, Return.check, When.After, Step.InstallInitialize, Condition.NOT_BeingRemoved));

        project.Include(WixExtension.NetFx);

        Compiler.BuildMsi(project);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        try
        {
            MessageBox.Show(session["NETFRAMEWORK20"], "");
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "Error");
        }
        return ActionResult.Success;
    }
}