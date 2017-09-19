//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Linq;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;
using System.Xml.Linq;

class Script
{
    static public void Main(string[] args)
    {
        //uncomment the line below if the reg file contains unsupported type to be ignored
        //RegFileImporter.SkipUnknownTypes = true;

        var fullSetup = new Feature("MyApp Binaries");

        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(fullSetup, @"readme.txt")),
                new RegFile(fullSetup, "MyProduct.reg"), //RegFile does the same Tasks.ImportRegFile
                new RegValue(fullSetup, RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Message", "Hello"),
                new RegValue(fullSetup, RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Count", 777),
                new RegValue(fullSetup, RegistryHive.ClassesRoot, "test\\shell\\open\\command", "", "\"[INSTALLDIR]test.exe\" \"%1\""));

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