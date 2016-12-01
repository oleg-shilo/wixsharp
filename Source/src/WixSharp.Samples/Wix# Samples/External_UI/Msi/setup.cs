//css_ref ..\..\..\WixSharp.dll;
//css_ref ..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var docs = new Feature("Documentation");
        var binaries = new Feature("Binaries");

        var project =
            new Project("MyProduct",

                new LaunchCondition("CUSTOM_UI=\"true\" OR REMOVE=\"ALL\"", "Please run setup.exe instead."),

                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(binaries, @"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(docs, @"Files\Docs\Manual.txt"))));

        project.UI = WUI.WixUI_Common;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}