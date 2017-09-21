//css_dir ..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

public class Script
{
    static public void Main(string[] args)
    {
        var project = new ManagedProject("CustomActionTest",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("setup.cs")),
                new ManagedAction(CustomActions.MyAction, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed));

        //project.Platform = Platform.x64;
        project.PreserveTempFiles = true;
        project.BuildMsi(@"E:\Galos\Projects\WixSharp\Source\src\WixSharp.Samples\Wix# Samples\DTF (ManagedCA)\tttt.msi");
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        MessageBox.Show("Hello World! (CLR: v" + Environment.Version + ")", "Embedded Managed CA (" + (Is64BitProcess ? "x64" : "x86") + ")");
        session.Log("Begin MyAction Hello World");

        return ActionResult.Success;
    }

    public static bool Is64BitProcess
    {
        get { return IntPtr.Size == 8; }
    }
}