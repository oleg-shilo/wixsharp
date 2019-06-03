//css_dir E:\PrivateData\Galos\Projects\WixSharp\Source\src\WixSharp.Samples\Wix# Samples\Registry;
//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        // uncomment the line below if the reg file contains unsupported type to be ignored
        // RegFileImporter.SkipUnknownTypes = true;

        var fullSetup = new Feature("MyApp Binaries");

        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(fullSetup, @"readme.txt")),
                // new RegFile(fullSetup, "_MyProduct.reg"), //RegFile does the same as Tasks.ImportRegFile
                new RegValue(fullSetup, RegistryHive.LocalMachine, @"Software\My Company\My Product", "LICENSE_KEY", "01020304")
                {
                    AttributesDefinition = "Type=binary",
                    Permissions = new[] { new Permission { User = "usr", Read = true } }
                },
                // HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\My Company\My Product
                new RegValue(fullSetup, RegistryHive.LocalMachine, @"Software\My Company\My Product", "Message", "Hello"),
                new RegValue(fullSetup, RegistryHive.LocalMachine, @"Software\My Company\My Product", "Count", 777),
                new RegValue(fullSetup, RegistryHive.ClassesRoot, @"test\shell\open\command", "", "\"[INSTALLDIR]test.exe\" \"%1\""),
                // new RemoveRegistryValue(fullSetup, @"Software\My Company\My Product"), // remove "My Product" value on install
                new RemoveRegistryKey(fullSetup, @"Software\My Company\My Product")); // remove the whole "My Product" on uninstall

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;

        // Below is the code sample for associating PermissionEx with a registry key
        // project.WixSourceGenerated += (doc) =>
        //              {
        //                  project.IncludeWixExtension(WixExtension.Util);

        //                  doc.FindAll("RegistryKey")
        //                     .ForEach(x => x.Add(WixExtension.Util.XElement("PermissionEx",
        //                                                                    "User=[WIX_ACCOUNT_USERS]; GenericAll=yes; CreateSubkeys=yes")));
        //              };
        project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}