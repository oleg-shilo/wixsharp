//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

// Truly a throw away project for dev testing

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt")),
                    new Dir(@"Docs-x64\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }
}