//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System.IO;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.Nsis;

public static class Script
{
    public static void Main(string[] args)
    {
        // If `Prerequisite.msi` does not exist yet execute <Wix# Samples>\Bootstrapper\NsisBootstrapper\Build.cmd

        var project = new ManagedProject("MainProduct",
                          new ManagedAction(CustomActions.MyAction, Return.ignore, When.After, Step.InstallInitialize, Condition.NOT_Installed));

        project.ManagedUI = ManagedUI.Default;

        var msiFile = project.BuildMsi();

        if (System.IO.File.Exists(msiFile))
        {
            // General sample
            BuildGeneralSample(msiFile);

            // Powershell script sample
            BuildScriptSample(msiFile, @"Assets\script.ps1");

            // BAT script sample
            BuildScriptSample(msiFile, @"Assets\script.bat");

            // CMD script sample
            BuildScriptSample(msiFile, @"Assets\script.cmd");

            // VBScript script sample
            BuildScriptSample(msiFile, @"Assets\script.vbs");

            // JScript script sample
            BuildScriptSample(msiFile, @"Assets\script.js");
        }
    }

    public static void BuildGeneralSample(string msiFile)
    {
        var bootstrapper = new NsisBootstrapper
        {
            DoNotPostVerifyPrerequisite = false,
            PrerequisiteFile = "Prerequisite.msi",
            PrimaryFile = msiFile,
            OutputFile = "MyProduct.exe",
            PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed",

            IconFile = "app_icon.ico",

            VersionInfo = new VersionInformation("1.0.0.0")
            {
                ProductName = "Test Application",
                LegalCopyright = "Copyright Test company",
                FileDescription = "Test Application",
                FileVersion = "1.0.0",
                CompanyName = "Test company",
                InternalName = "setup.exe",
                OriginalFilename = "setup.exe"
            },

            SplashScreen = new SplashScreen("wixsharp.bmp")
        };

        bootstrapper.Build();
    }

    public static void BuildScriptSample(string msiFile, string prerequisiteFile)
    {
        var bootstrapper = new NsisBootstrapper
        {
            PrerequisiteFile = prerequisiteFile,
            PrimaryFile = msiFile,
            OutputFile = $"MyProduct{Path.GetExtension(prerequisiteFile)}.exe"
        };

        bootstrapper.Build();
    }
}

public static class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        MessageBox.Show("Hello World!", "Embedded Managed CA");
        session.Log("Begin MyAction Hello World");

        return ActionResult.Success;
    }
}