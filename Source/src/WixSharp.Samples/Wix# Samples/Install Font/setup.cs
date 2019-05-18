//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt") { ComponentId = "asm" }),
                new Dir("%Fonts%",
                    new FontFile("FreeSansBold.ttf")));

        //The same can be achieved with File and custom attributes
        //new File("FreeSansBold.ttf") { AttributesDefinition="TrueType=yes"}));

        project.UI = WUI.WixUI_ProgressOnly;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}