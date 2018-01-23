//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref WixSharp.UI.dll;
using System;
using System.Windows.Forms;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new ManagedProject("MyProduct",
                          new Dir(@"C:\MyCompany2",
                              new File("setup.cs")),
                          new Dir(@"C:\MyCompany\MyProduct",
                              new Files(@"files\*.*")));

        project.UI = WUI.WixUI_ProgressOnly;
        project.AfterInstall += Project_AfterInstall;

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    static void Project_AfterInstall(SetupEventArgs e)
    {
        MessageBox.Show("e.InstallDir -> " + e.InstallDir + "\n" +
                        "EnvVar('INSTALLDIR') -> " + Environment.GetEnvironmentVariable("INSTALLDIR"),
                        "AfterInstall");
    }
}