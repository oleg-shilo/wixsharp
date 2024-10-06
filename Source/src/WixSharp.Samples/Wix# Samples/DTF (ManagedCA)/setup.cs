//css_winapp
//css_ng dotnet
//css_args  -rx -netfx
//css_dir ..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;
using WixToolset.Dtf.WindowsInstaller;
using Action = WixSharp.Action;

public class Script
{
    static public void Main(string[] args)
    {
        if (args.Contains("-remove"))
        {
            RemoveFiles(args[1]);
        }
        else
        {
            var project = new ManagedProject("CustomActionTest",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File("setup.cs")),
                    new ManagedAction(CustomActions.MyAction, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed),
                    new ManagedAction(CustomActions.InvokeRemoveFiles, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed)
                    {
                        Execute = Execute.deferred
                    },
                    new ElevatedManagedAction(CustomActions.MyCustomAction, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed)
                    {
                        UsesProperties = "WIXSHARP_RUNTIME_DATA"
                    },
                    new Error("9000", "Hello World! (CLR: v[2]) Embedded Managed CA ([3])"));

            // project.PreserveTempFiles = true;
            // project.OutDir = "bin";
            // Compiler.VerboseOutput = true;

            project.BuildMsi();
        }
    }

    static void RemoveFiles(string installdir)
    {
        MessageBox.Show("Clearing install dir: " + installdir);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult InvokeRemoveFiles(Session session)
    {
        var startInfo = new ProcessStartInfo();

        startInfo.UseShellExecute = true;
        startInfo.FileName = typeof(CustomActions).Assembly.Location;
        startInfo.Arguments = "-remove \"" + session.Property("INSTALLDIR") + "\"";
        startInfo.Verb = "runas";

        MessageBox.Show(typeof(CustomActions).Assembly.Location);

        Process
            .Start(startInfo)
            .WaitForExit();

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        // Debug.Assert(false);
        var args = session.ToEventArgs();
        args.Data["SQLSERVER"] = "test";

        args.Data.SaveTo(session);
        // session["WIXSHARP_RUNTIME_DATA"] = args.Data.ToString();

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

    [CustomAction]
    public static ActionResult MyCustomAction(Session session)
    {
        // Debug.Assert(false);
        try
        {
            var value = session.ExtractAppData()["SQLSERVER"];
            MessageBox.Show(value);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
        return ActionResult.Success;
    }
}