//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        var project =
            new Project("RebootTest",
                new ManagedAction("PromptToReboot"),
                new Error("9000", "You need to reboot the system.\nDo you want to reboot now?"));

        //You can also control rebooting via dedicated WiX/Wix# project properties
        // project.ScheduleReboot = new ScheduleReboot { InstallSequence = RebootInstallSequence.Both };
        // project.ForceReboot = new ForceReboot();
        // project.RebootSupressing = RebootSupressing.ReallySuppress;

        project.UI = WUI.WixUI_ProgressOnly;
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}

public class CustonActions
{
    [CustomAction]
    public static ActionResult PromptToReboot(Session session)
    {
        var record = new Record(1);
        record[1] = "9000";
        // Using `Record` only as example. Otherwise the direct WinForm message can be used instead.
        if (MessageResult.Yes == session.Message(InstallMessage.User | (InstallMessage)MessageButtons.YesNo | (InstallMessage)MessageIcon.Question, record))
        {
            Process.Start("shutdown.exe", "-r -t 30 -c \"Reboot has been requested from RebootTest.msi\"");
        }

        return ActionResult.Success;
    }
}