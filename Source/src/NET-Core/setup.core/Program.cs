using System.Reflection;

using WixSharp;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

var project =
    new ManagedProject("My Product",
        new Dir(@"%ProgramFiles%\My Company\My Product",
            new File("program.cs")),
        // new ManagedAction(Actions.CustomAction),
        new Property("PropName", "<your value>"));

project.PreserveTempFiles = true;

project.Load += Project_Load;
project.Load += (e) =>
{
    Native.MessageBox("lambda delegate", "WixSharp - .NET8");
    e.Result = ActionResult.Failure;
};

project.UI = WUI.WixUI_ProgressOnly;

project.BuildMsi();

// -----------------------------------------------

static void Project_Load(SetupEventArgs e)
{
    Native.MessageBox("static delegate", "WixSharp - .NET8");
    // e.Result = ActionResult.Failure;
}

// -----------------------------------------------

public class Actions
{
    [CustomAction]
    public static ActionResult CustomAction(Session session)
    {
        Native.MessageBox("MSI Session\nINSTALLDIR: " + session.Property("INSTALLDIR"), "WixSharp - .NET8");

        return ActionResult.Success;
    }
}