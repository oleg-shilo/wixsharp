//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using WixSharp.Utilities;
using WixToolset.Dtf.WindowsInstaller;
using IO = System.IO;

static class Script
{
    static public void Main()
    {
        // The sample is likely to fail as TempTestCert.pfx is not a term certificate that may expire.
        // If it is the case, either use a proper certificate ot regenerate a temporary one by following the instructions
        // from the certificates.readme.md file in this folder.

        System.Diagnostics.Debugger.Launch();

        var genericFileSigning = new DigitalSignature
        {
            PfxFilePath = "TempTestCert.pfx",
            Password = "password123",
            Description = "MyProductMsi",
        };

        var bundleFileSigning = new DigitalSignatureBootstrapper
        {
            PfxFilePath = "TempTestCert.pfx",
            Password = "password123",
            Description = "MyProductBundleMsi",
        };

        var simulatedFileSigning = new GenericSigner
        {
            Implementation = (x) =>
            {
                Console.WriteLine($"Signing {x} (simulation)");
                return 0;
            }
        };

        Compiler.SignAllFilesOptions.SupportedFileFormats = new[] { ".msi", ".exe", ".dll" };
        Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

        // -------------------------------------

        var project =
            new ManagedProject("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe")));

        project.UI = WUI.WixUI_ProgressOnly;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.Load += (SetupEventArgs e) => e.Result = ActionResult.UserExit;
        project.SignAllFiles = true;
        project.DigitalSignature = genericFileSigning;

        string builtProject = project.BuildMsi();

        // -------------------------------------

        var msiPackage = new MsiPackage(builtProject);

        Bundle bundle = new Bundle("MyProductBundle", msiPackage)
        {
            Version = new Version(0, 1, 0, 0),
            DigitalSignature = bundleFileSigning
        };

        var customBA = GetSignedCustomBA(genericFileSigning);

        bundle.Application = new ManagedBootstrapperApplication(customBA);

        bundle.Build("MyProductBundleSetup.exe");
    }

    static string GetSignedCustomBA(DigitalSignature digitalSignature)
    {
        // Preserve the original sample BA file.

        var customBa_Wix5_Sample = @"..\Bootstrapper\WiX5-Spike\WixToolset.WixBA\output\net472\WixToolset.WixBA.exe";
        var customBa_Wix5_Signed = customBa_Wix5_Sample.PathChangeExtension(".signed.exe");

        if (!customBa_Wix5_Sample.FileExists())
            throw new Exception($"The custom BA sample file '{customBa_Wix5_Sample}' is missing. Build the {customBa_Wix5_Sample.PathGetFileNameWithoutExtension()} project first.");

        if (VerifyFileSignature.IsSigned(customBa_Wix5_Sample))
        {
            customBa_Wix5_Signed = customBa_Wix5_Sample;
            Console.WriteLine("Custom BA is already signed.");
        }
        else
        {
            IO.File.Copy(customBa_Wix5_Sample, customBa_Wix5_Signed, true);
            digitalSignature.Apply(customBa_Wix5_Signed);
        }

        return customBa_Wix5_Signed;
    }
}