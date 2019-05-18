using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Xunit;
using File = WixSharp.File;

namespace WixSharp.Test
{
    public class TestData
    {
        public static string Path(string path = "")
        {
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(@"..\..\TestData", path));
        }
    }

    public class DeclarationSyntaxTests
    {
        [Fact]
        public void Test()
        {
            var featureA = new Feature("Feature A");
            var featureB = new Feature("Feature B");

            var ex = Assert.Throws<ApplicationException>(() =>
            {
                var project = new Project("SetupLettersDiagnostics",
                                          new Dir(@"%ProgramFiles%\test",
                                                  new File(featureA, Path.Combine("", "nunit.exe")),
                                                  new File(featureA, Path.Combine("", "Lib", "nunit-console-runner.dll"),
                                                           new Merge(featureB, "MyMergeModule.msm"))));
            });

            Assert.True(ex.Message.StartsWith("WixSharp.Merge is unexpected."));
        }
    }

    public class XmlInjectionTests
    {
        //[Fact(Skip = "as non-production test")]
        public void Test()
        {
            Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

            project.ControlPanelInfo.ProductIcon = TestData.Path(@"Files\app_icon.ico");
            project.Version = new Version("2015.0.41.0");
            project.SourceBaseDir = TestData.Path();

            //Compiler.WixSourceGenerated += Compiler_WixSourceGenerated;
            var wxs = Compiler.BuildMsi(project);
        }

        void Compiler_WixSourceGenerated(System.Xml.Linq.XDocument document)
        {
            var product = document.Root.Select("Product");
            product.AddElement("Icon", "Id=app_icon.ico;SourceFile=" + TestData.Path(@"app_icon.ico"));
            product.AddElement("Property", "Id=ARPPRODUCTICON;Value=app_icon.ico");
        }
    }
}