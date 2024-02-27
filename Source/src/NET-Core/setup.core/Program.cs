using System.Reflection;
using static System.Reflection.BindingFlags;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

public class Script
{
    public static event EventHandler ScriptLoaded;

    public static void Main()
    {
        Environment.SetEnvironmentVariable("IDE", "true");

        Console.WriteLine(Environment.CurrentDirectory);

        var project =
            new ManagedProject("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("program.cs")),
                new ManagedAction(Actions.CustomAction),
                // new ManagedAction(Actions.CustomAction2),
                new Property("PropName", "<your value>"));

        project.PreserveTempFiles = true;

        // project.Load += Project_Load;
        project.Load += (e) =>
        {
            Native.MessageBox("lambda delegate", "WixSharp - .NET8");
        };

        Script.ScriptLoaded += (x, y) =>
        {
            Native.MessageBox("Lambda Delegate");
        };

        project.UI = WUI.WixUI_ProgressOnly;

        project.BuildMsi();
        // -----------------------------------------------
    }

    // static void Project_Load(SetupEventArgs e)
    // {
    //     Native.MessageBox("static delegate", "WixSharp - .NET8");
    //     e.Result = ActionResult.Failure;
    // }
}

// -----------------------------------------------
public class Actions
{
    static string ExportEventsEntryPoints()
    {
        return null;
    }

    const BindingFlags bindng = Public | NonPublic | FlattenHierarchy | Static | Instance | InvokeMethod;

    public static ActionResult WixSharp_Load_Action(Session session)
    {
        // https://github.com/dotnet/corert/blob/master/Documentation/using-corert/reflection-in-aot-mode.md
        var required_for_aot_inclusion_of_reflection_metadata = typeof(Script).GetMembers(bindng);
        return InvokeHandler(session, "Script+<>c", "<Main>b__3_0");
    }

    public static ActionResult InvokeHandler(Session session, string type, string method)
    {
        try
        {
            var handlerClass = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.FullName == type);
            var handlerMethod = (MethodInfo)handlerClass?
                .GetMembers(bindng)?
                .FirstOrDefault(x => x.Name == method);

            // var arg = AotInterop.Convert(session);
            Native.MessageBox($"{handlerClass.FullName}\n{handlerMethod.Name}", "WixSharp.Managed");

            if (handlerClass != null && handlerMethod != null)
            {
                if (handlerMethod.IsStatic)
                {
                    handlerMethod?.Invoke(null, new object[] { null, null });
                }
                else
                {
                    object instance = Activator.CreateInstance(handlerClass);
                    handlerMethod?.Invoke(instance, new object[] { null });
                }
            }
        }
        catch (Exception e)
        {
            Native.MessageBox(e.Message, "WixSharp.Managed");
        }

        return ActionResult.UserExit;
    }

    [CustomAction]
    public static ActionResult CustomAction(Session session)
    {
        Native.MessageBox("MSI Session\nINSTALLDIR: " + session.Property("INSTALLDIR"), "WixSharp - .NET8");

        WixSharp_Load_Action(session);

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