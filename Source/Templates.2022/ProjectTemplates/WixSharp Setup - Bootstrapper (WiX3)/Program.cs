using System;
using WixSharp;
using WixSharp.Bootstrapper;

namespace $safeprojectname$
{
    class Program
{
    static void Main()
    {
        string productMsi = BuildMsi();

        var bootstrapper =
          new Bundle("MyProduct",
              // new PackageGroupRef("NetFx40Web"),
              new MsiPackage(productMsi) { DisplayInternalUI = true });

        bootstrapper.Version = new Version("1.0.0.0");
     	#warning "DON'T FORGET to replace this with a freshly generated GUID and remove this `#warning` statement."
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25844b");
        // bootstrapper.Application = new SilentBootstrapperApplication();
        // bootstrapper.PreserveTempFiles = true;

        bootstrapper.Build("MyProduct.exe");
    }

    static string BuildMsi()
    {
        var project = new Project("MyProduct",
                         new Dir(@"%ProgramFiles%\My Company\My Product",
                             new File("Program.cs")));

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        //project.SourceBaseDir = "<input dir path>";
        //project.OutDir = "<output dir path>";

        return project.BuildMsi();
    }
}
}