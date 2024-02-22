using System.Diagnostics;
using System.Reflection;
using static System.Reflection.BindingFlags;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

var project =
    new Project("My Product",
        new Dir(@"%ProgramFiles%\My Company\My Product",
            new File("program.cs")),
        new ManagedAction(MyActions.CustomAction),
        new ManagedAction(MyActions.CustomAction2),
        new Property("PropName", "<your value>")); ;

project.PreserveTempFiles = true;

project.UI = WUI.WixUI_ProgressOnly;

project.BuildMsi();

public class MyActions
{
    [CustomAction]
    public static ActionResult CustomAction(Session session)
    {
        Win32.MessageBox("CA: " + session.Property("WIXSHARP_RUNTIME_DATA"), "WixSharp.Managed");
        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult CustomAction2(Session session)
    {
        SetupEventArgs args = session.ToEventArgs();

        Win32.MessageBox("CA2 " + args.MsiFile, "WixSharp.Managed");
        return ActionResult.UserExit;
    }
}

public class MyActions2
{
    [CustomAction]
    public static ActionResult CustomAction(Session session)
    {
        Win32.MessageBox("CA: " + session.Property("WIXSHARP_RUNTIME_DATA"), "WixSharp.Managed");
        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult CustomAction2(Session session)
    {
        SetupEventArgs args = session.ToEventArgs();

        Win32.MessageBox("CA2 " + args.MsiFile, "WixSharp.Managed");
        return ActionResult.UserExit;
    }
}