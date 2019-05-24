//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Linq;
using WixSharp;

class Script
{
    static public void Main()
    {
        SimpleScenario();
    }

    static void SimpleScenario()
    {
        Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories = true;

        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new Files(@"..\Release Folder\test\*.exe")
                    {
                        OnProcess = file =>
                        {
                            file.OverwriteOnInstall = true;
                        }
                    },
        new ExeFileShortcut("Uninstall My Product", "[System64Folder]msiexec.exe", "/x [ProductCode]"))); ;

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1561ba25889b");

        project.BuildMsi();
    }

    static void ComplexScenario()
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",

                    //new Dir("Documentation", new Files(@"\\BUILDSERVER\My Product\Release\Documentation\*.*")), //uncomment if you have a real remote files to install

                    new Files(@"..\Release Folder\Release\*.*",
                              f => !f.EndsWith(".obj") &&
                                   !f.EndsWith(".pdb"))
                    {
                        AttributesDefinition = "ReadOnly=no" //all files will be marked with this attribute
                    },

                    new ExeFileShortcut("Uninstall My Product", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1561ba25889b");

        project.ResolveWildCards(ignoreEmptyDirectories: true)
               .FindFirstFile("MyApp.exe")
               .Shortcuts = new[]
               {
                   new FileShortcut("MyApp.exe", "INSTALLDIR"),
                   new FileShortcut("MyApp.exe", "%Desktop%")
               };

        Compiler.PreserveTempFiles = true;
        Compiler.EmitRelativePaths = false;
        project.BuildMsi();
    }
}