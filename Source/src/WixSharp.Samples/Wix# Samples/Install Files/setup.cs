//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        File f;
        var project =
            new Project("MyProduct",
                new Dir(new Id("MY_INSTALLDIR"), @"%ProgramFiles%\My Company\My Product",
                    f = new File("MyApp_file".ToId(),
                                 @"Files\Bin\MyApp.exe",
                                 new FileAssociation("cstm", "application/custom", "open", "\"%1\"")
                                 {
                                     Advertise = true,
                                     Icon = "wixsharp.ico"
                                 })
                    {
                        TargetFileName = "app.exe"
                    },
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt")
                        {
                            NeverOverwrite = true
                        })),
                    new Property("PropName", "<your value>"));

        project.SetVersionFrom("MyApp_file");
        // project.SetVersionFromFileId("MyApp_file");
        // project.SetVersionFromFile(@"Files\Bin\MyApp.exe");z
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.EmitConsistentPackageId = true;
        project.PreserveTempFiles = true;
        project.PreserveDbgFiles = true;

        project.EnableResilientPackage();

        project.Language = "en-US";

        project.PreserveTempFiles = true;

        project.WixSourceGenerated += Compiler_WixSourceGenerated;

        project.BuildMsi();
    }

    private static void Compiler_WixSourceGenerated(XDocument document)
    {
        //Will make MyApp.exe directory writable.
        //It is actually a bad practice to write to program files and this code is provided for sample purposes only.
        document.FindAll("Component")
                .Single(x => x.HasAttribute("Id", value => value.Contains("MyApp_file")))
                .AddElement("CreateFolder/Permission", "User=Everyone;GenericAll=yes");
    }
}