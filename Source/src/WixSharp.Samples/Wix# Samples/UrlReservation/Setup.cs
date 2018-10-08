//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref System.Xml.Linq.dll;
using System;
using WixSharp;

namespace FutoRollbackGeneration
{
    class Script
    {
        static void Main(string[] args)
        {
            try
            {
                var project =
                    new Project("My Product",
                        new Dir(@"%ProgramFiles%\My Company\My Product",
                            new File(@"File\MyApp.exe")
                            {
                                ServiceInstaller = new ServiceInstaller("WixSharp.TestSvc")
                                {
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
                                    },
                                    UrlReservations = new[]
                                    {
                                        new UrlReservation(new Id("SeervicesUrl"), "http://+:4131/url/device_service/", "*S-1-1-0", UrlReservationRights.all)
                                        {
                                            HandleExisting = UrlReservationHandleExisting.fail,
                                        },

                                        new UrlReservation(new Id("SeervicesUrl1"), "http://+:4191/url/device_service/", "*S-1-1-0")
                                        {
                                            HandleExisting = UrlReservationHandleExisting.ignore,
                                        },
                                    }
                                }
                            }),
                        new UrlReservation("http://+:2131/url/device_service/", "*S-1-1-0", UrlReservationRights.register));

                project.GUID = new Guid("EC18F80D-2528-4C85-848A-B485401B6523");
                project.Include(WixExtension.Util)
                       .Include(WixExtension.Http);

                Compiler.PreserveTempFiles = true;
                Compiler.BuildMsi(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}