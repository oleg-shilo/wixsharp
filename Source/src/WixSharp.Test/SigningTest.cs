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
        [Description("Issue #1957 (point #5)")]
        public void Custom_signing_return_code()
        {
            // see: https://github.com/oleg-shilo/wixsharp/issues/1957
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
        [Description("Issue #1957 (point #4)")]
        public void Signing_CA_wrapper()
        {
            // see: https://github.com/oleg-shilo/wixsharp/issues/1957
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
        [Description("Issue #1957 (point #1-a)")]
        public void Generic_signer_initialization()
        {
            // see: https://github.com/oleg-shilo/wixsharp/issues/1957
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
        [Description("Issue #1957 (point #1-b)")]
        public void Generic_signer_fallback()
        {
            // see: https://github.com/oleg-shilo/wixsharp/issues/1957
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
            {
                ApplyImplementation = (x) =>
                {
                    if (x.IsFileLocked())
                        lockedFiles.Add(x);
                    Console.WriteLine($"Signing bootstrapper {x}");
                    return 0;
                }
            };

            bootstrapper.Build("MyProduct.exe");

            signedFiles = signedFiles.Select(x => x.PathGetFileName()).Distinct().ToList();

            Assert.Empty(lockedFiles);

            Assert.Contains("WixSharp.UI.WPF.dll", signedFiles);
            Assert.Contains("WixSharp.dll", signedFiles);
            Assert.Contains("WixSharp.Test.dll", signedFiles);
            Assert.Contains("WixSharp.UI.dll", signedFiles);
            Assert.Contains("MyProduct.msi", signedFiles);
        }
    }
}