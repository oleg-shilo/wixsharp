//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_inc InputForm.cs;
//css_ref System.Core.dll;

using Microsoft.Win32;
using System;
using System.Windows.Forms;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;

internal class Script
{
    static public void Main(string[] args)
    {
        var project =
            new Project("CustomActionTest",

                new Dir(@"%ProgramFiles%\My Company\My Product",
                         new File(@"readme.txt")),

                // new WixQuietExecAction("notepad.exe", "[WEBPOOL_NAME]"),
                new ManagedAction(CustonActions.MyAction, Return.ignore, When.After, Step.InstallInitialize, Condition.NOT_Installed));

        project.Properties = new[] { new Property("WEBPOOL_NAME", "empty") };
        project.UI = WUI.WixUI_ProgressOnly;

        Compiler.BuildMsi(project);
    }
}

public class CustonActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        using (InputForm inputBox = new InputForm())
        {
            if (inputBox.ShowDialog() != DialogResult.OK)
                return ActionResult.UserExit;

            session["WEBPOOL_NAME"] = inputBox.WebPoolName;
            return ActionResult.Success;
        }
    }
}