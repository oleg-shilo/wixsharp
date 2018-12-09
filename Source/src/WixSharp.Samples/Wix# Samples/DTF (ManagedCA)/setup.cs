//css_dir ..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\WixToolset.Dtf.WindowsInstaller.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;

public class Script
{
    static public void Main()
    {
        var project = new ManagedProject("CustomActionTest",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("setup.cs")),
                new ManagedAction(CustomActions.MyAction, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed));

        //project.Platform = Platform.x64;
        project.PreserveTempFiles = true;
        // project.OutDir = "bin";

        project.BuildMsi();
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