using System;
using System.Diagnostics;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;
using io = System.IO;

public class Script
{
    // This WixSharp sample uses the official WiX5 BA sample (https://github.com/wixtoolset/wix/tree/HEAD/src/test/burn/WixToolset.WixBA)
    // as an external BA application.
    // NOTE: This sample demonstrates the new custom BA hosting model introduced in WiX5.
    // This model requires the BA to be a self-contained executable (not a DLL).
    // ------
    // Note, passing BootstrapperCore.config is optional and if not provided the default BootstrapperCore.config
    // will be used. The content of the default BootstrapperCore.config can be accessed via
    // ManagedBootstrapperApplication.DefaultBootstrapperCoreConfigContent.
    //
    // Note, that the DefaultBootstrapperCoreConfigContent may not be suitable for all build and runtime scenarios.
    // In such cases you may need to use custom BootstrapperCore.config as demonstrated below.
    // bootstrapper.Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config");
    // ------
    // If you add any more packages, you may need to update `OnDetectPackageComplete` (i.g. to show 'install' button
    // even if one package is already installed).

    static public void Main()
    {
        if (WixTools.GlobalWixVersion.Major < 5)
        {
            Console.WriteLine("This sample requires WiX5 or higher.");
            return;
        }

        // if you want to use msi with custom managed UI then use BuildMsiWithManaged() instead of BuildMsi()
        // You will also need to use MsiExePackage instead of MsiPackage.
        // See WixBootstrapper_MsiEmbeddedUI.csproj sample

        string productMsi = BuildMsi();
        // string productMsi = BuildMsiWithManaged();

        var bundle = new Bundle("My Product Bundle",
                         new MsiPackage(productMsi)
                         {
                             Id = "MyProductPackageId",
                             Visible = true,
                             DisplayInternalUI = true
                         });

        bundle.Version = new Version("1.0.0.0");
        bundle.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");

        // this is an official WiX5 BA sample
        // from https://github.com/wixtoolset/wix/tree/HEAD/src/test/burn/WixToolset.WixBA
        var customBa_Wix5_Sample = @"..\WiX5-Spike\WixToolset.WixBA\output\net472\WixToolset.WixBA.exe";
        bundle.Application = new ManagedBootstrapperApplication(customBa_Wix5_Sample);

        bundle.Build("my_setup.exe");
    }

    static string BuildMsi()
    {
        var productProj =
            new ManagedProject("My Product1",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        productProj.UI = WUI.WixUI_Mondo;
        productProj.Scope = InstallScope.perMachine;
        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258772");

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

    static string BuildMsiWithManaged()
    {
        var productProj =
            new ManagedProject("My Product1",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        productProj.ManagedUI = ManagedUI.Default;
        productProj.Scope = InstallScope.perMachine;
        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258772");

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