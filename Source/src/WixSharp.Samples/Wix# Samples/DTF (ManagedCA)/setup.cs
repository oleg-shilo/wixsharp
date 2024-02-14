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
using WixToolset.Dtf.WindowsInstaller;

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
                    new ManagedAction(CustomActions.InvokeRemoveFiles, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed),
                    new Error("9000", "Hello World! (CLR: v[2]) Embedded Managed CA ([3])"));

            // project.PreserveTempFiles = true;
            // project.OutDir = "bin";
            // Compiler.VerboseOutput = true;

            project.BuildMsi();
        }
    }

    static void RemoveFiles(string installdir)
    {
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

        Process.Start(startInfo);

        return ActionResult.Success;
    }

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