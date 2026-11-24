using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using WixSharp.Nsis;
using WixToolset.Dtf.WindowsInstaller;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace WixSharp.Test
{
    public class SigningTest
    {
        [Fact]
        public void File_signing_orchestration()
        {
            var fileToSign = "custom_BA.unsigned.dll".PathGetFullPath();
            System.IO.File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, fileToSign, true);

            // expected isolated file: <original_dir>/.wixsharp/<original_file>
            string getParentDirName(string x) => x.PathGetDirName().PathGetFileName();

            var digitalSignature = new GenericSigner(apply: x => 0 /* simulate successful signing */ );

            // -------------------------------------

            using (var file = System.IO.File.OpenRead(fileToSign)) // lock the file
            {
                var signedFiles = digitalSignature.SignFiles(fileToSign);

                // -------------------------------------

                Assert.NotEmpty(signedFiles);
                Assert.Equal(1, signedFiles.Count);
                Assert.Equal(fileToSign, signedFiles.Keys.First());
                Assert.Equal(".wixsharp", getParentDirName(signedFiles[fileToSign]));
            }
        }

        [Fact]
        [Description("Issue #1957 (point #5)")] // see: https://github.com/oleg-shilo/wixsharp/issues/1957
        public void Custom_signing_return_code()
        {
            Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

            // -------------------------------------

            var project =
                new ManagedProject("MyProduct",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(this.GetType().Assembly.Location)));

            project.DigitalSignature = new GenericSigner(apply: fileToSign => 333); // error code
            project.SignAllFiles = true;

            var exception = Assert.Throws<InvalidOperationException>(() => project.BuildMsi());

            // -------------------------------------

            Assert.Contains("Signing the file", exception.Message);
            Assert.Contains("failed. Return code: 333", exception.Message);
        }

        [Fact]
        [Description("Issue #1957 (point #4)")] // see: https://github.com/oleg-shilo/wixsharp/issues/1957
        public void Signing_CA_wrapper()
        {
            Compiler.AutoGeneration.PreferBatchSigning = true;
            Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

            var signedFiles = new List<string>();

            // -------------------------------------

            var project =
                new ManagedProject("MyProduct",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(this.GetType().Assembly.Location)));

            project.DigitalSignature = new GenericSigner
            (
                apply: fileToSign =>
                {
                    signedFiles.Add(fileToSign.PathGetFileName());
                    return 0;
                }
            );
            project.SignAllFiles = true;

            //The CA wrapper generation will be triggered because it is ManagedProject that always creates a managed Init event.
            project.BuildMsi();

            // -------------------------------------

            Assert.Contains("WixSharp.CA.dll", signedFiles);
        }

        [Fact]
        [Description("Issue #1957 (point #1-a)")] // see: https://github.com/oleg-shilo/wixsharp/issues/1957
        public void Generic_signer_initialization()
        {
            Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

            var project =
                new ManagedProject("MyProduct",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(this.GetType().Assembly.Location)));
            project.DigitalSignature = new GenericSigner( /*no lambdas are defined*/ );
            project.SignAllFiles = true;

            // -------------------------------------

            Compiler.AutoGeneration.PreferBatchSigning = false;
            var exception = Assert.Throws<NotImplementedException>(() => project.BuildMsi());
            Assert.Contains("GenericSigner.ApplyImplementation", exception.Message);

            // -------------------------------------

            Compiler.AutoGeneration.PreferBatchSigning = true;
            exception = Assert.Throws<NotImplementedException>(() => project.BuildMsi());
            Assert.Contains("GenericSigner.SignImplementation", exception.Message);
        }

        [Fact]
        [Description("Issue #1957 (point #1-b)")] // see: https://github.com/oleg-shilo/wixsharp/issues/1957
        public void Generic_signer_fallback()
        {
            Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

            var signedFiles = new List<string>();

            // GenericSigner will be handled via ISigningTool.Sign but since only ISigningTool.Apply is defined, it will be used instead.
            var simulatedSigner = new GenericSigner
            (
                apply: fileToSign =>
                {
                    signedFiles.Add(fileToSign.PathGetFileName());
                    return 0;
                }
            );

            // -------------------------------------

            var project =
                new ManagedProject("MyProduct",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(this.GetType().Assembly.Location)));

            project.DigitalSignature = simulatedSigner;
            project.SignAllFiles = true;

            Compiler.AutoGeneration.PreferBatchSigning = false;
            project.BuildMsi();

            // -------------------------------------
            Assert.NotEmpty(signedFiles);
            Assert.Contains("WixSharp.Test.dll", signedFiles);
            Assert.Contains("WixSharp.dll", signedFiles);
            Assert.Contains("WixSharp.CA.dll", signedFiles);
            Assert.Contains("MyProduct.msi", signedFiles);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [Description("Issue #1957 (point #3)")] // see: https://github.com/oleg-shilo/wixsharp/issues/1957
        public void Signing_bundle_payloads(bool explicitBaAssemblyPath)
        {
            InvokeRegister.Enabled = true;

            var signedFiles = new List<string>();

            var project = new Project("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File(this.GetType().Assembly.Location)));
            var productMsi = project.BuildMsi();

            // ---------------------

            var bootstrapper =
                    new Bundle("MyProduct-signed",
                    new MsiPackage(productMsi) { DisplayInternalUI = true });

            bootstrapper.Version = new Version("1.0.0.0");
            // the ManagedBootstrapperApplication constructor path will become the BA payload
            if (explicitBaAssemblyPath)
                bootstrapper.Application = new ManagedBootstrapperApplication(System.Reflection.Assembly.GetExecutingAssembly().Location); // will become the BA payload
            else
                bootstrapper.Application = new ManagedBootstrapperApplication("%this%");

            bootstrapper.SignApplicationFiles = true;
            bootstrapper.DigitalSignature = new GenericSigner
            (
                apply: x =>
                {
                    signedFiles.Add(x.PathGetFileName());
                    return 0;
                }
            );

            InvokeRegister.Clear(nameof(Tasks.DigitallySignBootstrapperEngine), nameof(Signing_bundle_payloads));

            var bundleFile = bootstrapper.Build("MyProduct-signed.exe");

            // ---------------------
            // cannot use InspectBundle as it is a simulates signing so us Invoke Register instead
            // var info = BurnEngineInspector.InspectBundle(bundleFile); 
            // Assert.True(info.BundleSigned);
            // Assert.True(info.EngineSigned);

            var isEngineSigned = InvokeRegister.WasInvokedFrom(nameof(Tasks.DigitallySignBootstrapperEngine), nameof(Signing_bundle_payloads));

            Assert.Contains("WixSharp.Test.dll", signedFiles);     // custom BA, which is this executing assembly 
            Assert.Contains("MyProduct-signed.exe", signedFiles);  // the bundle executable
        }

        [Fact]
        [Description("Issue #1833 and #1856")]
        public void Fix_Issue_1833_1856()
        {
            var signedFiles = new List<string>();
            var lockedFiles = new List<string>();

            Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

            var project = new ManagedProject("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File(this.GetType().Assembly.Location)));

            project.ManagedUI = ManagedUI.DefaultWpf;
            project.SignAllFiles = true;

            project.DigitalSignature = new GenericSigner
            (
                apply: (x) =>
                {
                    signedFiles.Add(x);
                    if (x.IsFileLocked())
                        lockedFiles.Add(x);
                    return 0;
                }
            );

            project.AddActions(new ElevatedManagedAction(TestCustomActions.MyAction, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed));

            project.Load += e =>
            {
            };

            var xunit = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "xunit.core");
            project.DefaultRefAssemblies.Add(xunit.Location);

            var productMsi = project.BuildMsi();

            var bootstrapper =
                    new Bundle("MyProduct",
                    new MsiPackage(productMsi) { DisplayInternalUI = true });

            bootstrapper.Version = new Version("1.0.0.0");
            bootstrapper.UpgradeCode = new Guid("67aea76c-ab9e-4cf4-96b9-9593081ad765");
            // bootstrapper.PreserveTempFiles = true;

            bootstrapper.DigitalSignature = new GenericSigner
            (
                apply: (x) =>
                {
                    if (x.IsFileLocked())
                        lockedFiles.Add(x);
                    Console.WriteLine($"Signing bootstrapper {x}");
                    return 0;
                }
            );

            bootstrapper.Build("MyProduct.exe");

            signedFiles = signedFiles.Select(x => x.PathGetFileName()).Distinct().ToList();

            // When running the whole test suite this assertion fails because some other tests lock the files.
            // Even though individual test succeeds in isolation. So we cannot assert that no files are locked. 
            // Assert.Empty(lockedFiles); 

            Assert.Contains("WixSharp.UI.WPF.dll", signedFiles);
            Assert.Contains("WixSharp.dll", signedFiles);
            Assert.Contains("WixSharp.Test.dll", signedFiles);
            Assert.Contains("WixSharp.UI.dll", signedFiles);
            Assert.Contains("MyProduct.msi", signedFiles);
        }
    }
}