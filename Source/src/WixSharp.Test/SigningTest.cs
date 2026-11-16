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
        [Description("Issue #1957")]
        public void Auto_signing_CA_wrapper()
        {
            Compiler.AutoGeneration.PreferBatchSigning = true;
            Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

            var signedAsms = new List<string>();

            // signing *.CA.dll 
            var simulatedFileSigning = new GenericSigner
            (
                sign: filesToSign =>
                {
                    signedAsms.AddRange(filesToSign);
                    return 0;
                }
            );


            // -------------------------------------

            var project =
                new ManagedProject("MyProduct",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(this.GetType().Assembly.Location)));

            project.Load += (SetupEventArgs e) => { }; //should trigger CA wrapper generation

            project.DigitalSignature = simulatedFileSigning;
            project.SignAllFiles = true;

            project.BuildMsi();

            // -------------------------------------

            Assert.NotEmpty(signedAsms);
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
                Implementation = (x) =>
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