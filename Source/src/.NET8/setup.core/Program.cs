using System.Diagnostics;
using System.Reflection;
using static System.Reflection.BindingFlags;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

public class Script
{
    public static event EventHandler ScriptLoaded;

    public void Test()
    {
    }

    static public void TestSt(object o)
    {
        MsgBox("TestSt");
    }

    static void Main()
    {
        // https://github.com/dotnet/corert/blob/master/Documentation/using-corert/reflection-in-aot-mode.md

        // {
        //     var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
        //     var handlerClass = types.FirstOrDefault(x => x.Name.EndsWith("<>c"));
        //     var handlerMethod = string.Join("\n", handlerClass.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.InvokeMethod)
        //         //     .OfType<MethodInfo>()
        //         .Select(x => x.Name)
        //         .Where(x => x == "<Main>b__5_0")
        //         .ToArray());
        // }

        var sw = Stopwatch.StartNew();

        // var nativeThis = @"D:\dev\wixsharp-wix4\Source\src\.NET8\setup.core\aot\setup.core.aot.csproj".CompileAotAssembly();
        // var nativeThis = @"D:\dev\wixsharp-wix4\Source\src\.NET8\WixSharp.Core\WixSharp.Core.csproj".CompileAotAssembly();
        var nativeThis = "setup.core.csproj".CompileAotAssembly();
        // var nativeThis = typeof(Script).Assembly.Location.ConvertToAotAssembly();

        Console.WriteLine(sw.Elapsed);
        Console.WriteLine(nativeThis);

        Environment.SetEnvironmentVariable("thisAOT", nativeThis);

        // ScriptLoaded += Script_ScriptLoaded;
        ScriptLoaded += (x, y) =>
        {
            MsgBox("Delegate");
        };

        var project =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("program.cs")),
                new ManagedAction("Net7CA")
                {
                    ActionAssembly = nativeThis,
                    MethodName = "CustomActionCore",
                    CreateInteropWrapper = false,
                },
                new Property("PropName", "<your value>"));

        project.PreserveTempFiles = true;

        project.UI = WUI.WixUI_ProgressOnly;

        project.BuildMsi();
    }

    public static void Script_ScriptLoaded(object? sender, EventArgs e)
    {
        // System.Windows.Forms.MessageBox.Show("Script_ScriptLoaded");
    }

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);

    static void MsgBox(string message, string title = "WixSharp.Assert") => MessageBox(GetForegroundWindow(), message, title, 0);

    [CustomAction]
    public static ActionResult CustomActionCore(Session session)
    {
        try
        {
            var required_for_aot_inclusion_of_reflection_metadata = typeof(Script).GetMembers(Public | NonPublic | FlattenHierarchy | Static | Instance | InvokeMethod);

            var handlerClass = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name.EndsWith("<>c"));
            var handlerMethod = (MethodInfo)handlerClass?
                .GetMembers(Public | NonPublic | FlattenHierarchy | Static | Instance | InvokeMethod)?
                .FirstOrDefault(x => x.Name == "<Main>b__5_0");

            object instance = Activator.CreateInstance(handlerClass);
            handlerMethod?.Invoke(instance, new object[] { null, null });
        }
        catch (Exception e)
        {
            MsgBox(e.Message, "WixSharp.Managed");
        }
        return ActionResult.UserExit;
    }
}

// ----------------

public class AotInterop
{
    [UnmanagedCallersOnly(EntryPoint = "CustomActionCore")]
    public static uint CustomActionCore(IntPtr handle)
        => (uint)Script.CustomActionCore(Session.FromHandle(handle, false));
}