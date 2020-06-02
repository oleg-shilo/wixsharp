//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        //Note that if the install condition for the component can be set without interacting with user (e.g. analyzing registry)
        //as part InstallExecuteSequence. However if interaction is required (e.g. message box, checkbox) install condition should
        //be set form InstallUISequence.

        var project =
                new Project("My Product",
                    //Files and Shortcuts
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(@"AppFiles\MyApp.exe",
                            new FileShortcut("MyApp") { WorkingDirectory = "[INSTALLDIR]" }),
                        new ExeFileShortcut("Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]")),

                    new Dir(@"%ProgramMenu%\My Company\My Product",
                        new ExeFileShortcut("Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]"),
                        new ExeFileShortcut("MyApp", "[INSTALLDIR]MyApp.exe", arguments: "")),

                     new Dir(@"%Desktop%",
                        new ExeFileShortcut("MyApp", "[INSTALLDIR]MyApp.exe", arguments: "")
                        {
                            Condition = new Condition("INSTALLDESKTOPSHORTCUT=\"yes\"") //property based condition
                        }),

                    //setting property to be used in install condition
                    new Property("INSTALLDESKTOPSHORTCUT", "no"),
                    new Property("ALLUSERS", "1"),
                    new ManagedAction(CustomActions.MyAction, Return.ignore, When.Before, Step.LaunchConditions, Condition.NOT_Installed, Sequence.InstallUISequence));

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;
        project.OutFileName = "setup";
        project.PreserveTempFiles = true;

        Compiler.BuildMsi(project);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        if (DialogResult.Yes == MessageBox.Show("Do you want to install desktop shortcut", "Installation", MessageBoxButtons.YesNo))
            session["INSTALLDESKTOPSHORTCUT"] = "yes";

        return ActionResult.Success;
    }
}