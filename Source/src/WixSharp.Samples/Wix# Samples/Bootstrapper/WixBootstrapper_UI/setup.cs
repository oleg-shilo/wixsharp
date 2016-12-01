using System;
using WixSharp;
using WixSharp.Bootstrapper;
using io = System.IO;

public class Script
{
    //The UI implementation is based on the work of BRYANPJOHNSTON
    //http://bryanpjohnston.com/2012/09/28/custom-wix-managed-bootstrapper-application/

    static public void Main(string[] args)
    {
        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt"))) { InstallScope = InstallScope.perMachine };

        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");
        string productMsi = productProj.BuildMsi();

        //------------------------------------

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    new MsiPackage(productMsi) { Id = "MyProductPackageId", DisplayInternalUI = true });

        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889a");
        bootstrapper.Application = new ManagedBootstrapperApplication("%this%"); // you can also use System.Reflection.Assembly.GetExecutingAssembly().Location

        bootstrapper.PreserveTempFiles = true;

        bootstrapper.Build();
        io.File.Delete(productMsi);
    }
}
