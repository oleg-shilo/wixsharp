//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        // The sample will fail as wixsharp.pfx is not a real certificate
        // Uncomment the section at the bottom of this file if you want to run this sample

        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe")));

        project.UI = WUI.WixUI_ProgressOnly;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        string builtProject = project.BuildMsi();

        var msiPackage = new MsiPackage(builtProject)
        {
            DisplayInternalUI = true
        };

        Bundle bundle = new Bundle("MyProductBundle", msiPackage)
        {
            Version = new Version(0, 1, 0, 0),

            DigitalSignature = new GenericSigner
            {
                Implementation = (x) =>
                {
                    Console.WriteLine($"Signing bootstrapper {x} (simulation)");
                    return 0;
                }
            }

            // TODO: Uncomment this section bellow if you want to run this sample with real signing
            // DigitalSignature = new DigitalSignatureBootstrapper
            // {
            //     PfxFilePath = "wixsharp.pfx",
            //     Password = "my_password",
            //     Description = "MyProduct",
            //     TimeUrl = new Uri("http://timestamp.digicert.com")
            // }
        };

        bundle.Build("MyProductBundleSetup.exe");
    }
}