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
    static public void Main()
    {
        Build();
        //BuildWithAttributes();
    }

    static public void Build()
    {
        //note %ProgramFiles% will be mapped into %ProgramFiles64Folder% as the result of project.Platform = Platform.x64;
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))),
                new RegValue(RegistryHive.LocalMachine, @"Software\My Company\My Product", "Message", "Hello"));

        project.PreserveTempFiles = true;

        // uncomment this line if you want to make the build of the x64 vs x86 controlled by the external condition.
        // if (Environment.GetEnvironmentVariable("buid_as_64") != null)
        project.Platform = Platform.x64;

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.BuildMsi();
    }

    static public void BuildWithAttributes()
    {
        //this sample is inly useful for the demonstration of how to work with AttributesDefinition and XML injection

        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles64Folder%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe") { AttributesDefinition = "Component:Win64=yes" },
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt") { AttributesDefinition = "Component:Win64=yes" })));

        project.Package.AttributesDefinition = "Platform=x64";
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        //Alternatively you can set Component Attribute for all files together (do not forget to remove "Component:Win64=yes" from file's AttributesDefinition)

        //either before XML generation
        //foreach (var file in project.AllFiles)
        //    file.Attributes.Add("Component:Win64", "yes");

        //or do it as a post-generation step
        //project.Compiler.WixSourceGenerated += new XDocumentGeneratedDlgt(Compiler_WixSourceGenerated);

        Compiler.BuildMsi(project);
    }

    static void Compiler_WixSourceGenerated(XDocument document)
    {
        document.Descendants("Component")
                .ForEach(comp=>comp.SetAttributeValue("Win64", "yes"));
    }
}



