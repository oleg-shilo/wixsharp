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
        EnsureCompatibleWixVersion();

        // if you want to use msi with custom UI managed UIthen use BuildMsiWithManaged() instead of BuildMsi()
        // You will also need to use MsiExePackage instead of MsiPackage.
        // See WixBootstrapper_MsiEmbeddedUI.csproj sample

        string productMsi = BuildMsi();

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
        // bundle.Application.AddPayload(typeof(ManagedBootstrapperApplication).Assembly.Location.ToPayload()); // needed for WiX5
        // bundle.Application = new ManagedBootstrapperApplication(@"D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Bootstrapper\WiX4-Spike\Bundle1\WixToolset.WixBA\bin\Debug\net472\win-x86\WixToolset.WixBA.dll");

        Compiler.VerboseOutput = true;

        bundle.Build("my_setup.exe");
    }

    static void EnsureCompatibleWixVersion()
    {
        // WiX5 has brought some breaking changes to the custom BA model.
        // The BA is now a separate process and the communication between the BA and the bundle is done via IPC.
        // https://wixtoolset.org/docs/fivefour/#burn   mentions the change (10 Apr 2024)
        // https://wixtoolset.org/docs/fivefour/oopbas/ describes the new model.(10 Apr 2024, though it's empty yet)
        // It is actually a good decision but it will require an extra effort for integrating it with WixSharp.
        // Thus, for now, Wix# is still using the WiX4 model of the custom BA.

        if (WixTools.GlobalWixVersion.Major == 5)
            WixTools.SetWixVersion(Environment.CurrentDirectory, "4.0.4");

        if (WixTools.GlobalWixVersion.Major == 4)
        {
            WixExtension.UI.PreferredVersion = "4.0.4";
            WixExtension.Bal.PreferredVersion = "4.0.2";
            WixExtension.NetFx.PreferredVersion = "4.0.2";
        }
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