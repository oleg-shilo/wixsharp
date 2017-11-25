//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe")))
            {
                DigitalSignature = new DigitalSignature
                {
                    // PfxFilePath = "wixsharp.pfx",
                    // Password = "my_password",
                    // Description = "MyProduct",
                    // TimeUrl = new Uri("http://timestamp.verisign.com/scripts/timstamp.dll")
                    UseCertificateStore = true,
                    TimeUrl = new Uri("http://timestamp.verisign.com/scripts/timstamp.dll"),
                    PfxFilePath = "Cert_Name", // Certificate name from Cert Store
                    OptionalArguments = "/fd SHA256",
                    WellKnownLocations = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin"
                }
            };

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}