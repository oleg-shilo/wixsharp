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
    static public void Main(string[] args)
    {
        Compiler.AutoGeneration.LegacyDefaultIdAlgorithm = false;

        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(new Id("MyApp_file"), @"Files\Bin\MyApp.exe"),
                    new File(@"Files\КубПЭУ.dll"),
                    new File(@"Files\Хелпер.dll"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt")
                        {
                            NeverOverwrite = true
                        })),
                    new Property("PropName", "<your value>"));

        project.SetVersionFrom("MyApp_file");
        // project.SetVersionFromFileId("MyApp_file");
        // project.SetVersionFromFile(@"Files\Bin\MyApp.exe");

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.EmitConsistentPackageId = true;
        project.PreserveTempFiles = true;
        project.PreserveDbgFiles = true;

        project.Language = "en-US";

        project.WixSourceGenerated += Compiler_WixSourceGenerated;

        project.BuildMsi();
    }

    static void Compiler_WixSourceGenerated(XDocument document)
    {
        //Will make MyApp.exe directory writable.
        //It is actually a bad practice to write to program files and this code is provided for sample purposes only.
        document.FindAll("Component")
                .Single(x => x.HasAttribute("Id", value => value.EndsWith("MyApp_file")))
                .AddElement("CreateFolder/Permission", "User=Everyone;GenericAll=yes");
    }
}