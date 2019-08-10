//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        Project project =
            new Project("MyProduct",
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

        project.UI = WUI.WixUI_ProgressOnly;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}