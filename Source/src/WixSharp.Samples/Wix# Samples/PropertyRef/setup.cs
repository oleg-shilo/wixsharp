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
                new PropertyRef("NETFRAMEWORK45"),
                new ManagedAction(CustomActions.MyAction, Return.check, When.After, Step.InstallInitialize, Condition.NOT_BeingRemoved)
                );

        project.IncludeWixExtension(WixExtension.NetFx);

        {
            int minimumFrameworkVersion = 47;
            //WixManagedProject.LaunchConditions.Add(new LaunchCondition($"Installed OR (NETFRAMEWORK45 AND NETFRAMEWORK45 >= \"#460798\")", $"{this.WixManagedProject.Name} requires Microsoft .NET Framework 4.7 or greater to be installed, but you are now using . Please install a supported Microsoft .NET Framework version, such as from https://www.microsoft.com/en-us/download/details.aspx?id=55167."));
            project.LaunchConditions.Add(new LaunchCondition("NETFRAMEWORK45 >= \"#460798\"", $"This requires Microsoft .NET Framework 4.7 or greater to be installed, but you are now using . Please install a supported Microsoft .NET Framework version, such as from https://www.microsoft.com/en-us/download/details.aspx?id=55167."));
        }


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
            MessageBox.Show(session["NETFRAMEWORK45"], "");
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "Error");
        }
        return ActionResult.Success;
    }
}