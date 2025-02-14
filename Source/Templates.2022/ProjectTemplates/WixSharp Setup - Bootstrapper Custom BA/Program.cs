using System;
using WixSharp;
using WixSharp.Bootstrapper;

namespace $safeprojectname$
{
    public class Program
    {
        static void Main()
        {
            // If you are using WiX5 tools then your Custom BA needs to be built as a separate assembly because as WiX5 uses completely different hosting model.
            // See this sample for WiX5 Custom BA https://github.com/oleg-shilo/wixsharp/tree/master/Source/src/WixSharp.Samples/Wix%23%20Samples/Bootstrapper/WiX5-Spike/WixToolset.WixBA
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