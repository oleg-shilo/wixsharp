//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new Project("MyProduct",
                          new Dir(@"D:\MyCompany\MyProduct",
                              new Files(@"files\*.*")));

        project.UI = WUI.WixUI_ProgressOnly;

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }
}
