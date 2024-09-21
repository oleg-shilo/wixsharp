//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        // The sample will fail as wixsharp.pfx is not a real certificate
        // Uncomment last line of the code (`project.BuildMsi()`) if you want to run this sample

        Project project =
            new ManagedProject("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe")))
            {
                DigitalSignature = new DigitalSignature
                {
                    PfxFilePath = "wixsharp.pfx",
                    Password = "my_password",
                    Description = "MyProduct",
                    TimeUrl = new Uri("http://timestamp.verisign.com/scripts/timstamp.dll")
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

        // This is an optional step to sign all files in the project
        // The supported file formats are configured by the Compiler.SignAllFilesOptions.SupportedFileFormats property
        project.SignAllFiles = true;

        project.UI = WUI.WixUI_ProgressOnly;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        // project.BuildMsi();
    }
}