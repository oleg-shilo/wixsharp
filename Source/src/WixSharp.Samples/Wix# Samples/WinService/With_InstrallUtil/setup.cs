//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using System.Linq;
using WixSharp.CommonTasks;
using IO = System.IO;

class Script
{
    static public void Main()
    {
        try
        {
            var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(@"..\SimpleService\MyApp.exe")),
                    new ElevatedManagedAction(CustomActions.InstallService, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed),
                    new ElevatedManagedAction(CustomActions.UnInstallService, Return.check, When.Before, Step.RemoveFiles, Condition.BeingUninstalled));

            project.GUID = new Guid("6fe30b47-2577-43ad-9195-1861ba25889b");
            project.OutFileName = "setup";

            project.BuildMsi();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult InstallService(Session session)
    {
        return session.HandleErrors(() =>
        {
            Tasks.InstallService(session.Property("INSTALLDIR") + "MyApp.exe", true);
            Tasks.StartService("WixSharp.SimpleService", false);
        });
    }

    [CustomAction]
    public static ActionResult UnInstallService(Session session)
    {
        return session.HandleErrors(() =>
        {
            //Tasks.StopService("WixSharp.SimpleService", false); //no need to call as system stop the service on uninstall anyway
            Tasks.InstallService(session.Property("INSTALLDIR") + "MyApp.exe", false);
        });
    }
}