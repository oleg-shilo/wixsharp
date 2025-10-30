//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

class Script
{
    static public void Main()
    {
        // The sample will fail as wixsharp.pfx is not a real certificate
        // Uncomment last line of the code (`project.BuildMsi()`) if you want to run this sample

        Environment.CurrentDirectory = @"D:\dev\wixsharp4\Source\src\WixSharp.Samples\Wix# Samples\Signing";

        var project =
            new ManagedProject("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe")))
            {
                DigitalSignature = new DigitalSignature
                {
                    PfxFilePath = "TempTestCert.pfx",
                    Password = "password123",
                    Description = "MyProduct",
                    //TimeUrl = new Uri("http://timestamp.verisign.com/scripts/timstamp.dll")
                }

                /// alternative approach by using a store certificate
                // project.DigitalSignature = new DigitalSignature
                // {
                //     CertificateId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                //     CertificateStore = StoreType.sha1Hash,
                //     HashAlgorithm = HashAlgorithmType.sha256,
                //     Description = "Description",
                //     TimeUrl = new Uri("http://timestamp.verisign.com/scripts/timestamp.dll")
                // }
            };

        project.Load += e =>
        {
            if (e.IsInstalling)
            {
                MessageBox.Show("Installing...", "My Product");
            }
            // e.Result = ActionResult.UserExit;
        };

        project.AddActions(new ElevatedManagedAction(CustomActions.MyAction, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed));

        // This is an optional step to sign all files in the project
        // The supported file formats are configured by the Compiler.SignAllFilesOptions.SupportedFileFormats property
        project.SignAllFiles = true;
        Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

        project.UI = WUI.WixUI_ProgressOnly;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        // Debugger.Launch();

        project.BuildMsi();
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        session.HandleErrors(() => MessageBox.Show("Hello world. I am in a custom action"));
        return ActionResult.UserExit;
    }
}