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

        project.UI = WUI.WixUI_Minimal;

        var msiFile = project.BuildMsi();

        if (System.IO.File.Exists(msiFile))
        {
            // Uncomment to preserve temp NSI files.
            // Compiler.PreserveTempFiles = true;

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

            // Arguments sample
            BuildArgumentsSample(msiFile);
        }
    }

    public static void BuildGeneralSample(string msiFile)
    {
        var bootstrapper = new NsisBootstrapper
        {
            Prerequisite =
            {
                FileName = "Prerequisite.msi",
                PostVerify = true,
                RegKeyValue = @"HKLM:Software\My Company\My Product:Installed"
            },

            Primary = {FileName = msiFile},

            OutputFile = "MyProduct.exe",
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
            Prerequisite = {FileName = prerequisiteFile},
            Primary = {FileName = msiFile},
            OutputFile = $"MyProduct{Path.GetExtension(prerequisiteFile)}.exe"
        };

        bootstrapper.Build();
    }

    // Arguments passing examples:
    //  MyProductArgs.exe /primary:"/qn" /prerequisite:"/passive /norestart"
    //  MyProductArgs.exe /primary:"PARAM1=VAL1"
    //  MyProductArgs.exe /qn
    public static void BuildArgumentsSample(string msiFile)
    {
        var bootstrapper = new NsisBootstrapper
        {
            Prerequisite =
            {
                // See @"Assets\DotNet.ps1" for VBS example
                FileName = @"Assets\DotNet.ps1",
                OptionName = "/prerequisite:",
                Arguments = @"/log %TEMP%\MyProductArgs_DotNetLog.html",
                CreateNoWindow = false,
                // Condition to check for presence of .Net 4.5 Framework.
                // PrerequisiteRegKeyValue = @"HKLM:SOFTWARE\Microsoft\.NETFramework\v4.0.30319\SKUs\.NETFramework,Version=v4.5:"

                Payloads =
                {
                    new Payload(@"Assets\Payload.txt"),
                    new Payload(@"Assets\Payload.txt") { Name = @"Destination\Payload.txt" }
                }
            },

            Primary =
            {
                FileName = msiFile,
                OptionName = "/primary:",
                // $EXEPATH - The full path of the installer executable.
                Arguments = @"/L*V %TEMP%\MyProductArgs_Msi.log EXEPATH=""$EXEPATH"""
            },

            OutputFile = "MyProductArgs.exe"
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

        string exeFile = session.Property("EXEPATH");
        if (!exeFile.IsNullOrEmpty())
        {
            session.Log("Exe file path: " + exeFile);
        }

        return ActionResult.Success;
    }
}