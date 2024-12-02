using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using WixSharp.UI;

using io = System.IO;

public class Script
{
    static public void Main()
    {
        // This sample shows how to embed an executable wrapper around MSI file
        // It is the only way to display MSI own managed UI from the bootstrapper in WiX3 (and
        // in WIX4 if CustomBA is used)

        Console.WriteLine("Building MSI ...");

        var msi = BuildMsi();

        Build(msi);
        // ManualBuild(msi);
    }

    static public void Build(string msi)
    {
        // This sample does what ManualBuild does but in a single step. This is because there is no need to build
        // self-hosted msi as it is automatically built in MsiExePackage constructor.

        Console.WriteLine("Building Bootstrapper ...");

        var bootstrapper =
            new Bundle("Managed Product Bundle",
                       new MsiExePackage(msi)
                       {
                           Name = "ManagedProduct",
                       });

        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889a");

        // Use product search to detect if msi_exe is already present on the target system

        bootstrapper.Build("my_app.exe");
    }

    static public void ManualBuild(string msi)
    {
        var msi_exe = msi + ".exe";
        var msi_product_code = new MsiParser(msi).GetProductCode();

        (int exitCode, string output) = msi.CompileSelfHostedMsi(msi_exe);
        if (exitCode != 0)
        {
            Console.WriteLine("Error: " + output);
            return;
        }

        Console.WriteLine("Building Bootstrapper ...");

        var bootstrapper =
            new Bundle("Managed Product Bundle",
                       new ExePackage(msi_exe)
                       {
                           Name = "ManagedProduct",
                           InstallArguments = "/i",
                           UninstallArguments = "/x",
                           RepairArguments = "/fa",
                           DetectCondition = "(ProductInstalled <> \"2\")",
                           Compressed = true
                       });

        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889a");

        // Use product search to detect if msi_exe is already present on the target system

        bootstrapper.Include(WixExtension.Util);
        bootstrapper.AddWixFragment("Wix/Bundle",
            new UtilProductSearch
            {
                ProductCode = msi_product_code,
                Result = ProductSearchResultType.state,
                Variable = "ProductInstalled"
            });

        bootstrapper.Build("my_app.exe");
    }

    static public string BuildMsi()
    {
        var productProj =
            new ManagedProject("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("setup.cs")));

        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        productProj.Version = new Version("2.0.0.0");

        productProj.ManagedUI = ManagedUI.Default;

        return productProj.BuildMsi();
    }
}