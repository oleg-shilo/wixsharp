using WixSharp;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

var project =
    new Project("My Product",
        new Dir(@"%ProgramFiles%\My Company\My Product",
            new File("program.cs")),
        new ManagedAction(Actions.CustomAction),
        new ManagedAction(Actions.CustomAction2),
        new Property("PropName", "<your value>")); ;

project.PreserveTempFiles = true;

project.UI = WUI.WixUI_ProgressOnly;

project.BuildMsi();

// -----------------------------------------------

public class Actions
{
    [CustomAction]
    public static ActionResult CustomAction(Session session)
    {
        Native.MessageBox("MSI Session\nINSTALLDIR: " + session.Property("INSTALLDIR"), "WixSharp - .NET8");

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult CustomAction2(Session session)
    {
        SetupEventArgs args = session.ToEventArgs();

        Native.MessageBox("WixSharp RuntimeData\nMsiFile: " + args.MsiFile, "WixSharp - .NET8");

        return ActionResult.UserExit;
    }
}