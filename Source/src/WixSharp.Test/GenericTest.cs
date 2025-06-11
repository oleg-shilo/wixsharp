extern alias WixSharpMsi;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;
using static WixSharp.SetupEventArgs;
using WixSharp.UI;
using WixToolset.Dtf.WindowsInstaller;
using Xunit;
using io = System.IO;
using WixMsi = WixSharpMsi::WixSharp;

namespace WixSharp.Test
{
    public class GenericTest
    {
        [Fact]
        public void RelativePath()
        {
            var path = @"E:\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Content\readme.txt";
            var baseDir = @"E:\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Install Files";

            var result = path.MakeRelativeTo(baseDir);
            Assert.Equal(@"..\Content\readme.txt", result);
        }

        [Fact]
        public void SemanticVersionToVersion()
        {
            var expected = new Version(1, 0, 0);
            var actual = "1.0.0-beta.1".SemanticVersionToVersion();
            Assert.Equal(expected, actual);

            expected = new Version(4, 0, 4);
            actual = "4.0.4+41e11442".SemanticVersionToVersion();
            Assert.Equal(expected, actual);

            expected = new Version(4, 0, 4);
            actual = "4.0.4 rc.2".SemanticVersionToVersion();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindWixExtensionDll()
        {
            if ("APPVEYOR".GetEnvVar().IsEmpty())
            {
                var actual = WixTools.FindWixExtensionDll("WixToolset.Bal.wixext");
                Assert.True(actual.IsNotEmpty());

                actual = WixTools.FindWixExtensionDll("WixToolset.UI.wixext");
                Assert.True(actual.IsNotEmpty());

                actual = WixTools.FindWixExtensionDll("WixToolset.UI.wixext", "6.0.0");
                Assert.True(actual.IsNotEmpty());
            }
        }

        [Fact]
        public void ComServerInstall()
        {
            //string dll = @"WixSharp\src\WixSharp.Samples\Wix# Samples\InstallCom\ATLSimpleServ.dll";

            //var wxs = Tasks.EmmitComWxs(dll);
        }

        [Fact]
        public void AppDataEscaping()
        {
            var data = new AppData();

            var original = @"Data Source=.\SQLEXPRESS;Initial Catalog=RequestManagement;Integrated Security=SSPI";
            var expected = "";

            data["test"] = original.EscapeKeyValue();

            var serialized = data.ToString();
            var deserialized = new AppData().InitFrom(serialized);

            expected = deserialized["test"].UnescapeKeyValue();

            Assert.Equal(expected, original);
        }

        [Fact]
        public void AttributesInjection()
        {
            var expectedNamespace = "http://schemas.microsoft.com/wix/DependencyExtension";
            var expectedName = "TTT";
            var expectedValue = "33333";

            var project =
                new Project("MyProduct",
                    new Dir(@"%ProgramFiles%\My Product",
                        new File("MyApp.exe") { AttributesDefinition = "{dep}" + $"{expectedName}={expectedValue}" }));
            project.IncludeWixExtension(@"WixDependencyExtension.dll", "dep", expectedNamespace);

            project.WixSourceGenerated += (XDocument doc) =>
            {
                var attr = doc.FindAll("File")
                              .SelectMany(x => x.Attributes())
                              .FirstOrDefault(a => a.Value == expectedValue);

                Assert.NotNull(attr);
                Assert.Equal(expectedName, attr.Name.LocalName);
                Assert.Equal(expectedNamespace, attr.Name.Namespace);
            };

            project.OutFileName = nameof(AttributesInjection);

            var wxsFile = project.BuildWxs();
        }

        [Fact]
        public void AttributesInjection2()
        {
            var expectedNamespace = "http://schemas.microsoft.com/wix/DependencyExtension";
            var expectedName = "TTT";
            var expectedValue = "33333";

            var project =
                new Project("MyProduct",
                    new Dir(@"%ProgramFiles%\My Product",
                        new File("MyApp.exe") { AttributesDefinition = "Component:{dep}" + $"{expectedName}={expectedValue}" }));

            project.IncludeWixExtension(@"WixDependencyExtension.dll", "dep", expectedNamespace);

            project.WixSourceGenerated += (XDocument doc) =>
            {
                var attr = doc.FindAll("Component")
                              .SelectMany(x => x.Attributes())
                              .FirstOrDefault(a => a.Value == expectedValue);

                Assert.NotNull(attr);
                Assert.Equal(expectedName, attr.Name.LocalName);
                Assert.Equal(expectedNamespace, attr.Name.Namespace);
            };

            project.OutFileName = nameof(AttributesInjection2);

            var wxsFile = project.BuildWxs();
        }

        [Fact]
        public void TestWxlCompatibilityOfCodebase()
        {
            // wixsharp\Source\src
            var dir = @"..\..\..";

            var files = io.Directory.GetFiles(dir, "*.wxl", io.SearchOption.AllDirectories)
                                    .Where(x => !x.Contains("Wix_bin") && x.PathGetFileName() != ".wxl") // WixSharp and test data files
                                    .Where(x => XDocument.Load(x).Root.GetDefaultNamespace().NamespaceName == "http://schemas.microsoft.com/wix/2006/localization")
                                    .ToArray();

            files.ForEach(x => Debug.WriteLine(x));

            // files.ForEach(x => x.Wxl3_to_Wxl4(x));

            Assert.Equal(0, files.Count());
        }

        [Fact]
        public void Can_Init_ManagedUI_Licalization_From_WiX3_Wxl()
        {
            var data = new ResourcesData();

            // var wxlFile = @"D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Managed Setup\MultiLanguageUI\WixUI_de-DE.wxl".MakeRelativeTo(Environment.CurrentDirectory);
            var wxlFile = @"..\..\..\WixSharp.Samples\Wix# Samples\Managed Setup\MultiLanguageUI\WixUI_de-DE.wxl";
            data.InitFromWxl(io.File.ReadAllBytes(wxlFile));
        }

        [Fact]
        public void AttributesInjection3()
        {
            var expectedNamespace = "http://schemas.microsoft.com/wix/DependencyExtension";
            var expectedName = "TTT";
            var expectedValue = "33333";

            var project =
                new Project("MyProduct",
                    new Dir(@"%ProgramFiles%\My Product",
                        new File("MyApp.exe") { AttributesDefinition = "{" + expectedNamespace + "}" + $"{expectedName}={expectedValue}" }));

            project.WixSourceGenerated += (XDocument doc) =>
            {
                var attr = doc.FindAll("File")
                              .SelectMany(x => x.Attributes())
                              .FirstOrDefault(a => a.Value == expectedValue);

                Assert.NotNull(attr);
                Assert.Equal(expectedName, attr.Name.LocalName);
                Assert.Equal(expectedNamespace, attr.Name.Namespace);
            };

            project.OutFileName = nameof(AttributesInjection3);
            var wxsFile = project.BuildWxs();
        }

        [Fact]
        public void AttributesInjection4()
        {
            var expectedName = "TTT";
            var expectedValue = "33333";

            var project =
                new Project("MyProduct",
                    new Dir(@"%ProgramFiles%\My Product",
                        new File("MyApp.exe") { AttributesDefinition = $"Component:{expectedName}={expectedValue}" }));

            project.WixSourceGenerated += (XDocument doc) =>
            {
                var attr = doc.FindAll("Component")
                              .SelectMany(x => x.Attributes())
                              .FirstOrDefault(a => a.Value == expectedValue);

                Assert.NotNull(attr);
                Assert.Equal(expectedName, attr.Name.LocalName);
            };

            project.OutFileName = nameof(AttributesInjection4);
            var wxsFile = project.BuildWxs();
        }

        [Fact]
        public void AttributesInjection5()
        {
            var project =
                new Project("MyProduct",
                    new Dir(@"%ProgramFiles%\My Product",
                        new File("MyApp.exe") { AttributesDefinition = "Component:{http://schemas.microsoft.com/wix/DependencyExtension}NNN=vvv" }));

            project.OutFileName = nameof(AttributesInjection5);

            Assert.Throws<Exception>(() => project.BuildWxs());
        }

        // [Fact] //xUnit/VSTest runtime doesn't play nice with MSI interop
        public void AppSearchTest()
        {
            var keyExist = AppSearch.RegKeyExists(Registry.LocalMachine, @"System\CurrentControlSet\services");
            var fakeKeyExist = AppSearch.RegKeyExists(Registry.LocalMachine, "TTTT");
            var regValue = AppSearch.GetRegValue(Registry.ClassesRoot, ".txt", null);
            var code = AppSearch.GetProductCode("Windows Live Photo Common");
            var name = AppSearch.GetProductName("{1D6432B4-E24D-405E-A4AB-D7E6D088CBC9}");
            var version = AppSearch.GetProductVersion("{1D6432B4-E24D-405E-A4AB-D7E6D088CBC9}");
            var installed = AppSearch.IsProductInstalled("{1D6432B4-E24D-405E-A4AB-D7E6D088CBC9}");
            var products = AppSearch.GetProducts();

            Assert.True(keyExist);
            Assert.False(fakeKeyExist);
            Assert.Equal("txtfile", regValue);
            Assert.Equal("16.4.3528", version.ToString());
            Assert.Equal("{1D6432B4-E24D-405E-A4AB-D7E6D088CBC9}", code.FirstOrDefault());
            Assert.Equal("Windows Live Photo Common", name);
            Assert.True(installed); //may fail on some machines
            Assert.True(products.Any());
        }

        //[Fact]
        public void MsiParser_InstallDir()
        {
            string msi = @"E:\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Shortcuts\setup.msi";
            var parser = new WixMsi.UI.MsiParser(msi);
            string installDirProperty = "INSTALLDIR";
            string dir = parser.GetDirectoryPath(installDirProperty);
        }

        [Fact]
        public void FirewallException()
        {
            var obj = new FirewallException(new Id("idTest"), "Test")
            {
                Description = "descr",
                File = "fff",
                IgnoreFailure = true,
                Port = "80",
                Profile = FirewallExceptionProfile.@public,
                Program = "prog",
                Protocol = FirewallExceptionProtocol.tcp,
                Scope = FirewallExceptionScope.localSubnet,
            };

            XElement xml = obj.ToXElement("FirewallException");

            Assert.Equal("descr", xml.Attribute("Description").Value);
            Assert.Equal("fff", xml.Attribute("File").Value);
            Assert.Equal("yes", xml.Attribute("IgnoreFailure").Value);
            Assert.Equal("80", xml.Attribute("Port").Value);
            Assert.Equal("public", xml.Attribute("Profile").Value);
            Assert.Equal("prog", xml.Attribute("Program").Value);
            Assert.Equal("tcp", xml.Attribute("Protocol").Value);
            Assert.Equal("localSubnet", xml.Attribute("Scope").Value);
            Assert.Equal("idTest", xml.Attribute("Id").Value);
            Assert.Equal("Test", xml.Attribute("Name").Value);
        }

        [Fact]
        public void EnumExtensions()
        {
            var test = ConfigureServiceTrigger.Install | ConfigureServiceTrigger.Reinstall;

            Assert.True(ConfigureServiceTrigger.Install.PresentIn(test));
            Assert.False(ConfigureServiceTrigger.Uninstall.PresentIn(test));
        }

        [Fact]
        public void ArrayExtensions()
        {
            var project = new Project();
            project.Actions = new Action[] { new ManagedAction() };
            //should not throw
            project.AddAction(new InstalledFileAction("", ""));
            project.AddActions(project.Actions);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AttributesDeterminism(bool attributeDefinitionInitiallyContainsData)
        {
            var obj = new TestEntity();

            if (attributeDefinitionInitiallyContainsData)
            {
                obj.ComponentId = "Component.TestEntity.1";
            }

            var initialCount = obj.Attributes.Count;

            obj.Attributes.Add("TestAttribute", "TestValue");

            // SetComponentPermanent is implemented by using attributes injection of `WixEntity.Attributes`
            // But the implementation should interfere with neither `WixEntity.Attributes` nor `WixEntity.AttributesDefinition`
            obj.SetComponentPermanent(true);

            Assert.Null(obj.AttributesDefinition);
            Assert.Equal(initialCount + 1, obj.Attributes.Count);
        }

        class TestEntity : WixEntity
        {
            [Xml]
            public new string Id { get => base.Id; set => base.Id = value; }

            [Xml]
            public string FieldA;
        }

        [Fact]
        public void MapToXmlAttributes()
        {
            var entity = new TestEntity
            {
                FieldA = "test_value"
            };

            var xml = entity.ToXElement("Test");
        }

        [Fact]
        public void Test()
        {
            Compiler.GuidGenerator = GuidGenerators.Default;
            Compiler.GuidGenerator = GuidGenerators.Sequential;
            Compiler.GuidGenerator = (seed) => Guid.Parse("9e2974a1-9539-4c5c-bef7-80fc35b9d7b0");
            Compiler.GuidGenerator = (seed) => Guid.NewGuid();
        }

        //[Fact]
        public void FeaturesAPI()
        {
            var installedPackage = new WixToolset.Dtf.WindowsInstaller.ProductInstallation("A6801CC8-AC2A-4BF4-BEAA-6EE4DCF17056");
            if (!installedPackage.IsInstalled)
            {
            }
            else
            {
                foreach (var currentInstallFeature in installedPackage.Features)
                {
                    if (currentInstallFeature.State == InstallState.Local)
                    {
                        Debug.WriteLine(string.Format("Migrating feature {0} - marking as Present", currentInstallFeature.FeatureName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Migrating feature {0} - marking as Absent", currentInstallFeature.FeatureName));
                    }
                }
            }
        }

        [Fact]
        public void Shoud_Expend_Media()
        {
            var media = new Media { CompressionLevel = CompressionLevel.high };

            string xml = media.ToXml(projectId: "myprod").ToString().ToSingleQuots();
            Assert.Equal("<Media Id='1' Cabinet='myprod.cab' CompressionLevel='high' EmbedCab='yes' />", xml);

            media.Cabinet = null;
            xml = media.ToXml().ToString().ToSingleQuots();
            Assert.Equal("<Media Id='1' CompressionLevel='high' EmbedCab='yes' />", xml);

            media.DiskPrompt = "prompt";
            xml = media.ToXml().ToString().ToSingleQuots();
            Assert.Equal("<Media Id='1' CompressionLevel='high' DiskPrompt='prompt' EmbedCab='yes' />", xml);
        }

        [Fact]
        public void Shoud_Resolve_WixVars()
        {
            string asWixVarToPath(string name) => name.AsWixVarToPath();

            var adminToolsFolder = asWixVarToPath("AdminToolsFolder");
            var appDataFolder = asWixVarToPath("AppDataFolder");
            var commonAppDataFolder = asWixVarToPath("CommonAppDataFolder");
            var commonFiles64Folder = asWixVarToPath("CommonFiles64Folder");
            var commonFilesFolder = asWixVarToPath("CommonFilesFolder");
            var desktopFolder = asWixVarToPath("DesktopFolder");
            var favoritesFolder = asWixVarToPath("FavoritesFolder");
            var programFiles64Folder = asWixVarToPath("ProgramFiles64Folder");
            var programFilesFolder = asWixVarToPath("ProgramFilesFolder");
            var myPicturesFolder = asWixVarToPath("MyPicturesFolder");
            var sendToFolder = asWixVarToPath("SendToFolder");
            var localAppDataFolder = asWixVarToPath("LocalAppDataFolder");
            var personalFolder = asWixVarToPath("PersonalFolder");
            var startMenuFolder = asWixVarToPath("StartMenuFolder");
            var startupFolder = asWixVarToPath("StartupFolder");
            var programMenuFolder = asWixVarToPath("ProgramMenuFolder");
            var system16Folder = asWixVarToPath("System16Folder");
            var system64Folder = asWixVarToPath("System64Folder");
            var systemFolder = asWixVarToPath("SystemFolder");
            var templateFolder = asWixVarToPath("TemplateFolder");
            var windowsVolume = asWixVarToPath("WindowsVolume");
            var windowsFolder = asWixVarToPath("WindowsFolder");
            var fontsFolder = asWixVarToPath("FontsFolder");
            var tempFolder = asWixVarToPath("TempFolder");

            bool isValid(string dir, string ending) => io.Directory.Exists(dir) && dir.EndsWith(ending, StringComparison.OrdinalIgnoreCase);

            //expected to be tested on OS Vista or above from the x86 runtime
            Assert.True(isValid(adminToolsFolder, "Administrative Tools"));
            Assert.True(isValid(appDataFolder, @"AppData\Roaming"));
            Assert.True(isValid(commonAppDataFolder, "ProgramData"));
            Assert.True(isValid(commonFiles64Folder, "Common Files"));
            Assert.True(isValid(commonFilesFolder, "Common Files"));
            Assert.True(isValid(desktopFolder, "Desktop"));
            Assert.True(isValid(favoritesFolder, "Favorites"));
            Assert.True(isValid(programFiles64Folder, "Program Files"));
            if (Environment.Is64BitProcess)
                Assert.True(isValid(programFilesFolder, "Program Files"));
            else
                Assert.True(isValid(programFilesFolder, "Program Files (x86)"));
            Assert.True(isValid(myPicturesFolder, "Pictures"));
            Assert.True(isValid(sendToFolder, "SendTo"));
            Assert.True(isValid(localAppDataFolder, "Local"));
            Assert.True(isValid(personalFolder, "Documents"));
            Assert.True(isValid(startMenuFolder, "Start Menu"));
            Assert.True(isValid(startupFolder, "Startup"));
            Assert.True(isValid(programMenuFolder, "Programs"));
            Assert.True(isValid(system16Folder, "system"));
            Assert.True(isValid(system64Folder, "system32"));
            Assert.True(isValid(systemFolder, "SysWow64"));
            Assert.True(isValid(templateFolder, "Templates"));
            Assert.True(isValid(windowsVolume, @"C:\"));
            Assert.True(isValid(windowsFolder, @"C:\Windows"));
            Assert.True(isValid(fontsFolder, @"C:\Windows\Fonts"));
            Assert.True(isValid(tempFolder, System.IO.Path.GetTempPath()));
        }

        [Fact]
        public void Should_Compare_CollectionByItems()
        {
            var itemsA = new[] { "a", "b" };
            var itemsC = new[] { "b", "c" };
            var itemsB = new List<string>(new[] { "a", "b" });

            Assert.Equal(itemsB.GetItemsHashCode(), itemsA.GetItemsHashCode());
            Assert.NotEqual(itemsC.GetItemsHashCode(), itemsA.GetItemsHashCode());
            Assert.NotEqual(new[] { "a" }.GetItemsHashCode(), itemsA.GetItemsHashCode());
        }

        [Fact]
        public void Should_Combine_Sequences()
        {
            var s1 = Sequence.InstallUISequence;
            var s2 = Sequence.InstallExecuteSequence;

            var result1 = s1 + s2;
            Assert.Equal("InstallUISequence|InstallExecuteSequence", result1.ToString());

            var result2 = s1 | s2;
            Assert.Equal("InstallUISequence|InstallExecuteSequence", result2.ToString());
        }

        [Fact]
        void StringEnum_Test()
        {
            Assert.Equal("newVal", new MyVals("newVal"));
            Assert.Equal("firstVal", MyVals.First);
            Assert.Equal("secondVal", MyVals.Second);
            Assert.Equal("thirdVal", MyVals.Third);

            MyVals empty = null;
            Assert.Equal(null, empty);
            Assert.True(empty == null);
            Assert.True(empty != MyVals.First);

            Assert.True(MyVals.Third == "thirdVal");
            Assert.True("thirdVal" == MyVals.Third);

            Assert.False(MyVals.Third == "");
            Assert.True(MyVals.Third != "");

            Assert.False("" == MyVals.Third);
            Assert.True("" != MyVals.Third);

            Assert.False(new MyVals("test") != new MyVals("test"));
            Assert.True(new MyVals("test") == new MyVals("test"));

            Assert.Equal("thirdVal", MyVals.Third);
            Assert.Equal("thirdVal", (string)MyVals.Third);

            Assert.True(MyVals.Third.Equals("thirdVal"));
            Assert.False(MyVals.Third.Equals(null));
            Assert.True(MyVals.Third.Equals(new MyVals("thirdVal")));
            Assert.True(MyVals.Third.Equals(MyVals.Third));
        }

        class MyVals : StringEnum<MyVals>
        {
            public MyVals(string value) : base(value)
            {
            }

            public static MyVals First = new MyVals("firstVal");
            public static MyVals Second = new MyVals("secondVal");
            public static MyVals Third = new MyVals("thirdVal");
        }
    }
}