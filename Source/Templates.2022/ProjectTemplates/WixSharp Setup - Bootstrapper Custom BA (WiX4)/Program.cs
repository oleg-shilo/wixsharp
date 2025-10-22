using System;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

namespace $safeprojectname$
{
    public class Program
    {
        static void Main()
        {
            // Ensure you are using a legacy WiX4 toolset
            if (WixTools.GlobalWixVersion.Major >= 5)
                WixTools.SetWixVersion(Environment.CurrentDirectory, "4.0.0");

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