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
                    new File("readme.txt")))
            { InstallScope = InstallScope.perMachine };

        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");
        string productMsi = productProj.BuildMsi();

        //------------------------------------

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    new MsiPackage(productMsi) { Id = "MyProductPackageId", DisplayInternalUI = true });

        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889a");
        
        // You can also use System.Reflection.Assembly.GetExecutingAssembly().Location instead of "%this%"
        // Note, passing "BootstrapperCore.config" is optional and provided for demo purposes only
        bootstrapper.Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config"); 

        bootstrapper.PreserveTempFiles = true;
        bootstrapper.SuppressWixMbaPrereqVars = true;

        bootstrapper.Build("my_app.exe");
        io.File.Delete(productMsi);
    }
}