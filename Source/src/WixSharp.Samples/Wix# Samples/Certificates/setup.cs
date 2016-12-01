//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Linq;
using System.Xml.Linq;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt") { NeverOverwrite = true })),
                new Binary(new Id("mycert"), @"Files\mycert.cer"),
                new Certificate("NameOfCert", StoreLocation.localMachine, StoreName.personal, "mycert"),
                new Certificate("NameOfCert2", StoreLocation.currentUser, StoreName.personal, @"Path\to\cert\file", authorityRequest:false));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
       
        Compiler.BuildMsi(project);
    }
}



