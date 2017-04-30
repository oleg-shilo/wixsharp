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
        var project = new ManagedProject("MyProduct",
                          new Dir(new Id("ACTUAL_INSTALLDIR"), @"%ProgramFiles%\My Company\My Product", new File("setup.cs")),
                          new Dir(new Id("ACTUAL_TARGETDIR1"), @"%DesktopFolder%\My Company\My Product2", new Files(@"files\*.*"))
                          //new Dir(@"C:\MyCompany2", new File("setup.cs")),
                          //new Dir(@"C:\MyCompany\MyProduct",
                          //    new Files(@"files\*.*"))
                          );

        project.UI = WUI.WixUI_ProgressOnly;

        project.Load += Project_Load;

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    static void Project_Load(SetupEventArgs e)
    {
        e.Session["ACTUAL_INSTALLDIR"] = @"C:\My_Company2";
        e.Session["ACTUAL_TARGETDIR1"] = @"C:\My_Company\MyProduct";
    }
}