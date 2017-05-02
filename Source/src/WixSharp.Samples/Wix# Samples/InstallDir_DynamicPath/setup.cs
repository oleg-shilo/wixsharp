//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref WixSharp.UI.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project =
            new ManagedProject("MyProduct",
                new Dir(new Id("DIR1"), "root1", new File("setup.cs")),
                new Dir(new Id("DIR2"), "root2", new Files(@"files\*.*")));

        project.UI = WUI.WixUI_ProgressOnly;
        project.Load += Project_Load;

        project.BuildMsi();
    }

    static void Project_Load(SetupEventArgs e)
    {
        e.Session["DIR1"] = @"C:\My Company1";
        e.Session["DIR2"] = @"C:\My Company2";
    }
}