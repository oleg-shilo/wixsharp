using System.Reflection;
using static System.Reflection.BindingFlags;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

// public class Script
// {
//     public static void Main()
//     {
//         Environment.SetEnvironmentVariable("IDE", "true");

Console.WriteLine(Environment.CurrentDirectory);

var project =
    new ManagedProject("My Product",
        new Dir(@"%ProgramFiles%\My Company\My Product",
            new File("program.cs")),
        new ManagedAction(Actions.CustomAction),
        new Property("PropName", "<your value>"));

project.PreserveTempFiles = true;

project.Load += Project_Load;
project.Load += (e) =>
{
    Native.MessageBox("lambda delegate", "WixSharp - .NET8");
};

project.UI = WUI.WixUI_ProgressOnly;

project.BuildMsi();

// -----------------------------------------------

static void Project_Load(SetupEventArgs e)
{
    Native.MessageBox("static delegate", "WixSharp - .NET8");
}

// -----------------------------------------------
public class Actions
{
    const BindingFlags bindng = Public | NonPublic | FlattenHierarchy | Static | Instance | InvokeMethod;

    public static ActionResult WixSharp_Load_Action(Session session)
    {
        // https://github.com/dotnet/corert/blob/master/Documentation/using-corert/reflection-in-aot-mode.md
        var required_for_aot_inclusion_of_reflection_metadata = typeof(Program).GetMembers(bindng);

        WixSharp.ManagedProjectActions.WixSharp_Load_Action(session);
        return ActionResult.UserExit;
    }

    [CustomAction]
    public static ActionResult CustomAction(Session session)
    {
        Native.MessageBox("MSI Session\nINSTALLDIR: " + session.Property("INSTALLDIR"), "WixSharp - .NET8");

        WixSharp_Load_Action(session);

        return ActionResult.Success;
    }

    public static ActionResult CustomAction2(Session session)
    {
        SetupEventArgs args = session.ToEventArgs();

        Native.MessageBox("WixSharp RuntimeData\nMsiFile: " + args.MsiFile, "WixSharp - .NET8");

        return ActionResult.UserExit;
    }
}