using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Bootstrapper;
using WixToolset.Dtf.WindowsInstaller;
using io = System.IO;

public class Script
{
    //The UI implementation is based on the work of BRYANPJOHNSTON
    //http://bryanpjohnston.com/2012/09/28/custom-wix-managed-bootstrapper-application/

    static public void Main()
    {
        var productProj =
            new ManagedProject("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        productProj.Scope = InstallScope.perMachine;
        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");

        productProj.Load += (SetupEventArgs e) =>
        {
            MessageBox.Show(e.Session.Property("USERINPUT"), "User Input");
            MessageBox.Show(e.Session.Property("REGISTRYINPUT"), "Registry Input");
            e.Result = ActionResult.Failure;
        };

        string productMsi = productProj.BuildMsi();

        //------------------------------------

        var bootstrapper =
            new Bundle("My Product",
                       new PackageGroupRef("NetFx462Web"),
                       // new ExePackage(@"hello.exe") //just a demo sample
                       // {
                       //     Name = "WixCustomAction_cmd",
                       //     InstallCommand = "-install",
                       //     // Permanent = true,
                       //     Compressed = true
                       // },

                       new MsiPackage(productMsi)
                       {
                           Id = "MyProductPackageId",
                           MsiProperties = "USERINPUT=[UserInput];REGISTRYINPUT=[RegistryInput];"
                       }
                      );

        bootstrapper.Variables = "UserInput=none; RegistryInput=none".ToStringVariables();
        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889a");
        //WixVariables["WixMbaPrereqPackageId"] = "Netfx4Full";

        bootstrapper.Include(WixExtension.Util);
        bootstrapper.AddWixFragment("Wix/Bundle",
                                    new UtilRegistrySearch
                                    {
                                        Root = RegistryHive.CurrentUser,
                                        Result = SearchResult.value,
                                        Key = @"SOFTWARE\WixSharp\BootstrapperData\My Product",
                                        Value = "RegistryInput",
                                        Variable = "RegistryInput"
                                    });

        bootstrapper.Application = new ManagedBootstrapperApplication("%this%");

        // You can also use System.Reflection.Assembly.GetExecutingAssembly().Location instead of "%this%"
        // Note, passing BootstrapperCore.config is optional and if not provided the default BootstrapperCore.config
        // will be used. The content of the default BootstrapperCore.config can be accessed via
        // ManagedBootstrapperApplication.DefaultBootstrapperCoreConfigContent.
        //
        // Note that the DefaultBootstrapperCoreConfigContent may not be suitable for all build and runtime scenarios.
        // In such cases you may need to use custom BootstrapperCore.config as demonstrated below.
        // bootstrapper.Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config");

        bootstrapper.PreserveTempFiles = true;
        // bootstrapper.SuppressWixMbaPrereqVars = true;

        bootstrapper.OutFileName = "my_app";
        bootstrapper.Build("my_app.exe");
        io.File.Delete(productMsi);
    }
}