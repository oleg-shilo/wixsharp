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
    // ------
    // NOTE: This sample demonstrates the new old BA hosting model obsoleted in WiX5.
    // This model requires the BA to be a DLL (not an external exe).
    // ------
    // This WixSharp sample uses a simple (Hello-World style) custom BA implemented in the same assembly as the bundle builder
    // You can also use System.Reflection.Assembly.GetExecutingAssembly().Location instead of "%this%"
    // The UI implementation is based on the work of BRYANPJOHNSTON
    // http://bryanpjohnston.com/2012/09/28/custom-wix-managed-bootstrapper-application/
    // ------
    // Note, passing BootstrapperCore.config is optional and if not provided the default BootstrapperCore.config
    // will be used. The content of the default BootstrapperCore.config can be accessed via
    // ManagedBootstrapperApplication.DefaultBootstrapperCoreConfigContent.
    //
    // Note that the DefaultBootstrapperCoreConfigContent may not be suitable for all build and runtime scenarios.
    // In such cases you may need to use custom BootstrapperCore.config as demonstrated below.
    // bootstrapper.Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config");
    // ------
    // If you add any more packages, you may need to update `OnDetectPackageComplete` (i.g. to show 'install' button
    // even if one package is already installed).

    static public void Main()
    {
        // Need to ensure older WiX4 packages are used
        WixTools.SetWixVersion(Environment.CurrentDirectory, "4.0.0");

        // if you want to use msi with custom managed UI then use BuildMsiWithManaged() instead of BuildMsi()
        // You will also need to use MsiExePackage instead of MsiPackage.
        // See WixBootstrapper_MsiEmbeddedUI.csproj sample

        string productMsi = BuildMsi();
        // string productMsi = BuildMsiWithManaged();

        // -------------------------------------

        var bundle = new Bundle("My Product Bundle",
                         new MsiPackage(productMsi)
                         {
                             Id = "MyProductPackageId",
                             Visible = true,
                             DisplayInternalUI = true
                         });

        bundle.Version = new Version("1.0.0.0");
        bundle.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");

        bundle.Application = new ManagedBootstrapperApplication("%this%");
        bundle.Application.Payloads = bundle.Application.Payloads.Combine([new Payload(bundle.GetType().Assembly.Location)]); // WixSharp.dll

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