using System;
using WixSharp;
using System.Runtime.InteropServices;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

namespace $safeprojectname$
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Starting from version 5.0 WiX uses new BA hosting model. 
            // Read more about it here:https://github.com/oleg-shilo/wixsharp/wiki/Wix%23-Bootstrapper-integration-notes

            if (Environment.GetEnvironmentVariable("IDE") == null)
            {
                BAHost.Run(args); // running as a bundle application during setup
            }
            else
            {
                BuildScript.Run(args); // run as a bootstrapper build script
            }
        }

        public class BAHost
        {
            [DllImport("kernel32.dll")]
            static extern bool FreeConsole();

            static public void Run(string[] args)
            {
                FreeConsole(); // closes the console window; alternatively compile this app as a windows application

                var application = new ManagedBA();
                WixToolset.BootstrapperApplicationApi.ManagedBootstrapperApplication.Run(application);
            }
        }

        public class BuildScript
        {
            static public void Run(string[] args)
            {
                if (WixTools.GlobalWixVersion.Major < 5)
                {
                    Console.WriteLine("This sample requires WiX5 or higher.");
                    return;
                }

                string productMsi = BuildMsi();

                var bootstrapper =
                    new Bundle("MyProduct",
                        new MsiPackage(productMsi)
                        {
                            Id = "MyProductPackageId",
                            DisplayInternalUI = true
                        });

                bootstrapper.Version = new Version("1.0.0.0");
                bootstrapper.UpgradeCode = new Guid("$guid1$");
                bootstrapper.Application = new ManagedBootstrapperApplication("%this%");

                bootstrapper.Build("MyProduct.exe");
            }

            static string BuildMsi()
            {
                var project = new Project("MyProduct",
                                  new Dir(@"%ProgramFiles%\My Company\My Product",
                                      new File("Program.cs")));

                project.GUID = new Guid("$guid1$");

                return project.BuildMsi();
            }
        }
    }
}