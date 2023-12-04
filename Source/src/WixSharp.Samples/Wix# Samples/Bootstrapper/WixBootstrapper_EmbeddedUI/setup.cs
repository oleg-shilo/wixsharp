using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Bootstrapper;
using io = System.IO;

public class Script
{
    static public void Main()
    {
        var bootstrapper =
            new Bundle("My Product",
                       new ExePackage(@"..\..\..\..\Managed Setup\Self-executable_Msi\ManagedSetup.exe")
                       {
                           Name = "ManagedSetup",
                           InstallCommand = "/i",
                           UninstallCommand = "/x",
                           // Protocol = Protocol.burn,
                           RepairCommand = "/fa",
                           DetectCondition = "MyAppInstalled",
                           Compressed = true
                       });

        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889a");

        // Use file search to detect if MyApp is already present on the target system
        // Alternatively you can use UtilRegistrySearch to detect the MyApp by testing registry

        bootstrapper.Include(WixExtension.Util);
        bootstrapper.AddWixFragment("Wix/Bundle",
                                    new UtilFileSearch
                                    {
                                        Path = @"[ProgramFilesFolder]My Company\My Product\MyApp.exe",
                                        Result = SearchResult.exists,
                                        Variable = "MyAppInstalled"
                                    });

        bootstrapper.PreserveTempFiles = true;
        bootstrapper.Build("my_app.exe");
    }
}