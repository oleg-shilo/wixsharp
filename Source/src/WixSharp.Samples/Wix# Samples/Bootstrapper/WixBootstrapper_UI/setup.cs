using System;
using System.Diagnostics;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Bootstrapper;
using WixToolset.Dtf.WindowsInstaller;
using io = System.IO;

public class Script
{
    //The UI implementation is based on the work of BRYANPJOHNSTON
    //http://bryanpjohnston.com/2012/09/28/custom-wix-managed-bootstrapper-application/

    // This WixSharp sample uses a simple (Hello-World style) custom BA implemented in the same assembly as the bundle builder
    // You can also use System.Reflection.Assembly.GetExecutingAssembly().Location instead of "%this%"

    // Alternatively you can use a sample BA application from WiX Toolset codebase
    // https://github.com/wixtoolset/wix/tree/develop/src/test/burn/WixToolset.WixBA
    // bootstrapper.Application = new ManagedBootstrapperApplication(@"..\WiX4-Spike\Bundle1\WixToolset.WixBA\bin\Debug\net472\win-x86\WixToolset.WixBA.dll");

    // Note, passing BootstrapperCore.config is optional and if not provided the default BootstrapperCore.config
    // will be used. The content of the default BootstrapperCore.config can be accessed via
    // ManagedBootstrapperApplication.DefaultBootstrapperCoreConfigContent.
    //
    // Note that the DefaultBootstrapperCoreConfigContent may not be suitable for all build and runtime scenarios.
    // In such cases you may need to use custom BootstrapperCore.config as demonstrated below.
    // bootstrapper.Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config");

    // If you add any more packages, you may need to update `OnDetectPackageComplete` (i.g. to show 'install' button
    // even if one package is already installed).

    static public void Main()
    {
        string productMsi = BuildMsi();

        var bundle = new Bundle("My Product Bundle",
                         new MsiPackage(productMsi)
                         {
                             Id = "MyProductPackageId",
                             DisplayInternalUI = true
                         });

        bundle.Version = new Version("1.0.0.0");
        bundle.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889a");
        bundle.Application = new ManagedBootstrapperApplication("%this%");
        // bundle.Application = new ManagedBootstrapperApplication(@"D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Bootstrapper\WiX4-Spike\Bundle1\WixToolset.WixBA\bin\Debug\net472\win-x86\WixToolset.WixBA.dll");

        bundle.Build("my_app.exe");
    }

    static string BuildMsi()
    {
        var productProj =
            new ManagedProject("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        productProj.Scope = InstallScope.perMachine;
        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");

        productProj.Load += (SetupEventArgs e) =>
        {
            if (e.IsInstalling)
            {
                MessageBox.Show("Installing...", "My Product");
            }
            else if (e.IsUninstalling)
            {
                MessageBox.Show("Uninstalling...", "My Product");
            }
        };
        productProj.AfterInstall += (SetupEventArgs e) =>
        {
            MessageBox.Show("MSI session is completed", "My Product");
        };

        return productProj.BuildMsi();
    }
}