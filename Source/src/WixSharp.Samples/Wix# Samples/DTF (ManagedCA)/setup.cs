//css_dir ..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

#if Wix4
using WixToolset.Dtf.WindowsInstaller;
#else

using Microsoft.Deployment.WindowsInstaller;

#endif

using WixSharp;
using WixSharp.CommonTasks;

public class Script
{
    static public void Main()
    {
        var project = new ManagedProject("CustomActionTest",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("setup.cs")),
                new ManagedAction(CustomActions.MyAction, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed),
                new Error("9000", "Hello World! (CLR: v[2]) Embedded Managed CA ([3])"));

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
        Record record = new Record(3);

        record[1] = "9000";
        record[2] = Environment.Version;
        record[3] = Is64BitProcess ? "x64" : "x86";

        session.Message(InstallMessage.User | (InstallMessage)MessageButtons.OK | (InstallMessage)MessageIcon.Information, record);

        //MessageBox.Show("Hello World! (CLR: v" + Environment.Version + ")", "Embedded Managed CA (" + (Is64BitProcess ? "x64" : "x86") + ")");
        session.Log("Begin MyAction Hello World");

        return ActionResult.Success;
    }

    public static bool Is64BitProcess
    {
        get { return IntPtr.Size == 8; }
    }
}