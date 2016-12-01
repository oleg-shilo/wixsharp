//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project = new Project("MainProduct",
                          new ManagedAction(CustonActions.MyAction, Return.ignore, When.After, Step.InstallInitialize, Condition.NOT_Installed));

        var msiFile = Compiler.BuildMsi(project);

        if (System.IO.File.Exists(msiFile))
        {
            new NativeBootstrapper
            {
                PrerequisiteFile = "Prerequisite.msi",
                PrimaryFile = msiFile,
                OutputFile = "setup.exe",
                PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed"
            }
            .Build();
        }
    }
}

public class CustonActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        MessageBox.Show("Hello World!", "Embedded Managed CA");
        session.Log("Begin MyAction Hello World");

        return ActionResult.Success;
    }
}



