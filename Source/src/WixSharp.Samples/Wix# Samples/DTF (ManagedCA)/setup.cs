//css_dir ..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

public class Script
{
    static public void Main(string[] args)
    {
        var project = new Project("CustomActionTest",
                new Dir(@"%ProgramFiles%\My Company\My Product", 
                    new File("setup.cs")),
                new ManagedAction(CustomActions.MyAction, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed));

        project.ControlPanelInfo.InstallLocation = "[INSTALLDIR]";
        project.ControlPanelInfo.NoModify = true;

        //project.Platform = Platform.x64;
        project.PreserveTempFiles = false;
        project.BuildMsi();
    }

    private static void Project_WixSourceGenerated(System.Xml.Linq.XDocument doc)
    {
        doc.FindAll("Custom")
           .Where(x => x.HasAttribute("Action", "Set_ARPNOMODIFY"))
           .First()
           .SetAttribute("After", "InstallInitialize");
    }

    [CustomAction]
    public static ActionResult SetInstallDir(Session session)
    {
        //set custom installdir
        ///This event is fired before native MSI UI loaded (disabled for demo purposes)
        //session["INSTALLDIR"] = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\7-Zip")
        //                                            .GetValue("Path")
        //                                            .ToString(); 
        return ActionResult.Success;
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult ValidateLicenceKey(Session session)
    {
        return ActionResult.Success;
    }

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