//css_dir ..\..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core;
//css_ref System.Xml.Linq;
//css_ref System.Xml;

using System;
using System.Data;
using System.Linq;
using WixSharp;

class Script
{
    static public void Main()
    {
        try
        {
            var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(@"..\SimpleService\MyApp.exe",
                                 new ServiceInstaller
                                 {
                                     Name = "WixSharp.TestSvc",
                                     StartOn = SvcEvent.Install, //set it to null if you don't want service to start as during deployment
                                     StopOn = SvcEvent.InstallUninstall_Wait,
                                     RemoveOn = SvcEvent.Uninstall_Wait,
                                     DelayedAutoStart = true,
                                     ServiceSid = ServiceSid.none,
                                     FirstFailureActionType = FailureActionType.restart,
                                     SecondFailureActionType = FailureActionType.restart,
                                     ThirdFailureActionType = FailureActionType.runCommand,
                                     ProgramCommandLine = "MyApp.exe -run",
                                     RestartServiceDelayInSeconds = 30,
                                     ResetPeriodInDays = 1,
                                     PreShutdownDelay = 1000 * 60 * 3,
                                     RebootMessage = "Failure actions do not specify reboot",
                                     DependsOn = new[]
                                     {
                                         new ServiceDependency("[Dnscache]"),
                                         new ServiceDependency("Dhcp"),
                                     },
                                 }),
                        new File(@"..\SimpleService\MyApp2.exe",
                                 new ServiceInstaller
                                 {
                                     PermissionEx = new PermissionEx
                                     {
                                         User = "Everyone",
                                         ServicePauseContinue = true,
                                         ServiceQueryStatus = true,
                                         ServiceStart = true,
                                         ServiceStop = true,
                                         ServiceUserDefinedControl = true
                                     },
                                     Name = "WixSharp.TestSvc2",
                                     StartOn = SvcEvent.Install, //set it to null if you don't want service to start as during deployment
                                     StopOn = SvcEvent.InstallUninstall_Wait,
                                     RemoveOn = SvcEvent.Uninstall_Wait,
                                     DelayedAutoStart = true,
                                     ServiceSid = ServiceSid.none,
                                     FirstFailureActionType = FailureActionType.restart,
                                     SecondFailureActionType = FailureActionType.restart,
                                     ThirdFailureActionType = FailureActionType.runCommand,
                                     ProgramCommandLine = "MyApp.exe -run",
                                     RestartServiceDelayInSeconds = 30,
                                     ResetPeriodInDays = 1,
                                     PreShutdownDelay = 1000 * 60 * 3,
                                     RebootMessage = "Failure actions do not specify reboot",
                                     DependsOn = new[]
                                     {
                                         new ServiceDependency("Dnscache"),
                                         new ServiceDependency("Dhcp"),
                                     }
                                 })));

            project.GUID = new Guid("6fe30b47-2577-43ad-9195-1861ba25889b");
            project.OutFileName = "setup";

            project.PreserveTempFiles = true;
            project.BuildMsi();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}