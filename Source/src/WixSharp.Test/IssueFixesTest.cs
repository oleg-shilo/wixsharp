using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

#pragma warning disable MethodDocumentationHeader // The method must have a documentation header.

namespace WixSharp.Test
{
    public class IssueFixesTest
    {
        /// <summary>
        /// Fixes the issue 803.
        /// </summary>
        [Fact]
        [Description("Issue #803")]
        public void Fix_Issue_803()
        {
            // ensure all types that expose their properties for serialization with [XML] have these properties public
            // "SqlDb" and "root" are serializable but not assignable by user
            var classesWithFaultsFields =
                    typeof(Project).Assembly.GetTypes()
                        .SelectMany(t => t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                          .Where(p => p.Name != "SqlDb")
                                          .Where(p => p.GetCustomAttribute<WixSharp.XmlAttribute>() != null)
                                          .Select(x => $"{x.DeclaringType.Name}.{x.Name}"))
                .Concat(
                    typeof(Project).Assembly.GetTypes()
                        .Where(t => t != typeof(Error))
                        .SelectMany(t => t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                          .Where(p => p.Name != "root")
                                          .Where(p => p.GetCustomAttribute<WixSharp.XmlAttribute>() != null)
                                          .Select(x => $"{x.DeclaringType.Name}.{x.Name}")))
                .ToArray();

            Assert.Empty(classesWithFaultsFields);
        }

        [Fact]
        [Description("Issue #1114")]
        public void Fix_Issue_1114()
        {
            var project = new Project("MyProductSetup",
                new Dir(@"%ProgramFiles%\MyCompany\MyProduct\MyApp",
                        new Dir(
                            // new Feature("extra"),
                            @"Logs")));

            // project.PreserveTempFiles = true;
            project.WixSourceGenerated += document =>
            {
                var xml = document.ToString();
            };
            project.BuildWxs();
        }

        [Fact]
        [Description("Issue #1171")]
        public void Fix_Issue_1171()
        {
            var tempFile = System.IO.Path.GetTempFileName();
            try
            {
                System.IO.File.WriteAllLines(tempFile, new[]
                {
                    "Windows Registry Editor Version 5.00",
                    "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Test]",
                    "\"New Value #1\"=dword:00000001",
                    "\"New Value #2\"=hex(b):02,00,00,00,00,00,00,00",
                });
                var values = WixSharp.RegFileImporter.ImportFrom(tempFile);
            }
            finally
            {
                tempFile.DeleteIfExists();
            }
        }

        [Fact]
        [Description("Issue #1132")]
        public void Fix_Issue_1132()
        {
            var i = 0;

            var project = new Project("CableScheduler",
                new Dir(@"%ProgramFiles%\CADbloke\CableScheduler",
                new Files($"{Environment.CurrentDirectory}\\*.*", f => i++ < 1)),

                new Dir(@"%ProgramMenu%\CADbloke\CableScheduler",
                        new ExeFileShortcut("Cable Scheduler", $"[INSTALLDIR]Test.exe", "")));

            project.ResolveWildCards(pruneEmptyDirectories: true);

            var wsx = project.BuildWxs();
            var xml = System.IO.File.ReadAllText(wsx);

            Assert.True(xml.Contains("<Shortcut Id="));
        }

        [Fact]
        [Description("Issue #67")]
        public void Fix_Issue_67()
        {
            var project = new Project("MyProduct",
                              new Dir("%ProgramFiles%"));

            project.LicenceFile = "license.rtf";

            var file = project.BuildWxs();
        }

        [Fact]
        [Description("Issue #182")]
        public void Fix_Issue_182()
        {
            void assert(Platform? projPlatform, bool? is64Reg1, bool? is64Reg2, Action<XDocument, XElement, XElement, XElement> assertHandler)
            {
                var proj = new Project("MyProduct",
                                       new Dir(@"%ProgramFiles%\My Company\My Product",
                                           new File("setup.cs")));

                if (projPlatform != null)
                    proj.Platform = projPlatform;

                var regval = new RegValue(RegistryHive.LocalMachine, @"Software\test", "foo_value", "bar") { Win64 = is64Reg1 };
                proj.AddRegValue(regval);

                var regkey = new RegKey(null, RegistryHive.LocalMachine, @"Software\My Company\My Product", new RegValue("Message", "Hello"))
                {
                    Win64 = is64Reg2
                };
                proj.AddRegKey(regkey);

                proj.WixSourceGenerated += doc =>
                {
                    var cpmponents = doc.FindAll("Component");
                    var reg = cpmponents.FirstOrDefault(x => x.HasAttribute("Id", "Registry.1"));
                    var reg2 = cpmponents.FirstOrDefault(x => x.HasAttribute("Id", "Registry.2"));
                    var file = cpmponents.FirstOrDefault(x => x.HasAttribute("Id", value => value.StartsWith("Component.setup.cs")));
                    assertHandler(doc, reg, reg2, file);
                };

                proj.BuildMsiCmd();
            }

            assert(projPlatform: null, is64Reg1: true, is64Reg2: true,
                   assertHandler: (doc, reg, reg2, file) =>
                   {
                       Assert.True(reg.HasAttribute("Bitness", "always64"));
                       Assert.True(reg2.HasAttribute("Bitness", "always64"));
                       Assert.False(file.HasAttribute("Bitness", "always64"));
                   });

            assert(projPlatform: Platform.x64, is64Reg1: null, is64Reg2: null,
                   assertHandler: (doc, reg, reg2, file) =>
                   {
                       Assert.True(reg.HasAttribute("Bitness", "always64"));
                       Assert.True(reg2.HasAttribute("Bitness", "always64"));
                       Assert.True(file.HasAttribute("Bitness", "always64"));
                   });

            assert(projPlatform: Platform.x64, is64Reg1: false, is64Reg2: false,
                   assertHandler: (doc, reg, reg2, file) =>
                   {
                       Assert.False(reg.HasAttribute("Bitness", "always64"));
                       Assert.False(reg2.HasAttribute("Bitness", "always64"));
                       Assert.True(file.HasAttribute("Bitness", "always64"));
                   });
        }

        [Fact]
        [Description("WiX 4 Bugs (Feature condition) #1282")]
        public void WiX4_Issue_1_1282()
        {
            var feature1 = new Feature("feature1")
            {
                Condition = new FeatureCondition("PROP1 = 1", level: 2)
            };

            var project = new Project("MyProduct",
                                   new Dir(@"%ProgramFiles%\My Company\My Product",
                                       new File("setup.cs")
                                       {
                                           Feature = feature1
                                       }));

            var xmlFile = project.BuildWxs();
        }

        [Fact]
        [Description("Issue #995")]
        public void Fix_Issue_995()
        {
            Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories = true;

            try
            {
                var project =
                    new Project("myproduct",
                        new Dir("%Desktop%"),
                        new Dir("%ProgramFiles%"));

                project.BuildWxs();
            }
            finally
            {
                Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories = false;
            }
        }

        [Fact]
        [Description("Issue #656: ExeFileShortcut changing folder name ")]
        public void Fix_Issue_656()
        {
            // 'MySystem' should not be converted to 'MySystemFolder`
            var raw_path = @"[INSTALLDIR]\MySystem\MyProduct.exe";

            var normalized_path = raw_path.NormalizeWixString();

            Assert.Equal(raw_path, normalized_path);
        }

        [Fact]
        public void ListConsts()
        {
            var list = Compiler.GetMappedWixConstants(true);
        }

        [Fact]
        [Description("Issue #60")]
        public void Fix_Issue_60()
        {
            var project = new Project("MyProduct",
                              new Dir("%ProgramFiles%",
                              new File("abc.txt", new FilePermission("Guest", GenericPermission.All))));

            project.AddAction(new WixQuietExecAction("cmd.exe", "/c \"echo abc\""));

            var batchFile = project.BuildMsiCmd();
            string cmd = System.IO.File.ReadAllLines(batchFile).First();

            int firstPos = cmd.IndexOf("WixUtilExtension.dll");
            int lastPos = cmd.LastIndexOf("WixUtilExtension.dll");

            Assert.Equal(firstPos, lastPos);
        }

        [Fact]
        [Description("Issue #37")]
        public void Should_Preserve_ConstantsInAttrDefs()
        {
            var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\MyCompany",
                        new Dir("MyWebApp",
                            new File(@"MyWebApp\Default.aspx",
                            new IISVirtualDir
                            {
                                Name = "MyWebApp",
                                AppName = "Test",
                                WebSite = new WebSite("[IIS_SITE_NAME]", "[IIS_SITE_ADDRESS]:[IIS_SITE_PORT]"),
                                WebAppPool = new WebAppPool("MyWebApp", "Identity=applicationPoolIdentity")
                            }))));

            string wxs = project.BuildWxs();

            var address = XDocument.Load(wxs)
                                   .FindSingle("WebAddress");

            Assert.Equal("[IIS_SITE_ADDRESS]", address.ReadAttribute("IP"));
            Assert.Equal("[IIS_SITE_PORT]", address.ReadAttribute("Port"));
        }

        [Fact]
        [Description("Discussions #642332")]
        public void Should_Process_DirAttributes()
        {
            Dir dir1, dir2;

            var project =
                new Project("My Product",
                    dir1 = new Dir(@"%ProgramFiles%\MyCompany",
                        dir2 = new Dir("MyWebApp", new File("Default.aspx"))));

            dir1.AttributesDefinition = "DiskId=1";
            dir2.AttributesDefinition = "DiskId=2";

            string wxs = project.BuildWxs();

            var dirs = XDocument.Load(wxs)
                                .FindAll("Directory")
                                .Where(x => x.HasAttribute("DiskId"))
                                .ToArray();

            Assert.Equal(2, dirs.Count());

            Assert.True(dirs[0].HasAttribute("Name", "MyCompany"));
            Assert.True(dirs[0].HasAttribute("DiskId", "1"));

            Assert.True(dirs[1].HasAttribute("Name", "MyWebApp"));
            Assert.True(dirs[1].HasAttribute("DiskId", "2"));
        }

        [Fact]
        [Description("Discussions #642332")]
        public void Should_Process_DirAttributes_2()
        {
            var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\MyCompany",
                        new Dir("MyWebApp", new File("Default.aspx"))));

            project.Add(new EnvironmentVariable("someVar", "Some value") { AttributesDefinition = "DiskId=2" });

            string wxs = project.BuildWxs();

            var doc = XDocument.Load(wxs);
        }

        [Fact]
        [Description("Discussions #642263")]
        public void Should_CanInject_UserProfileNoiseAutomatically()
        {
            var project = new Project("TestProject",

                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File(@"Files\notepad.exe")),

                          new Dir(@"%CommonAppDataFolder%\Test Project",
                              new File(@"Files\TextFile.txt")),

                          new Dir(@"%PersonalFolder%\Test Project",
                              new File(@"Files\Baskets.bbd")));

            string wxs = project.BuildWxs();

            var doc = XDocument.Load(wxs);
        }

        [Fact]
        [Description("Discussions #642263")]
        public void Should_CanInject_UserProfileNoise()
        {
            var project = new Project("TestProject",

                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File(@"Files\notepad.exe")),

                          new Dir(@"%CommonAppDataFolder%\Test Project",
                              new File(@"Files\TextFile.txt")),

                          new Dir(@"%PersonalFolder%\Test Project",
                              new File(@"Files\Baskets.bbd")));

            project.WixSourceGenerated += xml =>
            {
                var dir = xml.FindAll("Directory")
                             .Where(x => x.HasAttribute("Name", "PersonalFolder"))
                             //.Where(x => x.HasAttribute("Name", v => v == "PersonalFolder"))
                             .SelectMany(x => x.FindAll("Component"))
                             .ForEach(comp => comp.InsertUserProfileRegValue());
            };
            string wxs = project.BuildWxs();

            var doc = XDocument.Load(wxs);
        }

        [Fact]
        [Description("Discussions #642263")]
        public void Should_Inject_RemoveFolder()
        {
            var project = new Project("TestProject",

                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File(@"Files\notepad.exe")),

                          new Dir(@"%CommonAppDataFolder%\Test Project",
                              new File(@"Files\TextFile.txt")),

                          new Dir(@"%PersonalFolder%\Test Project",
                              new File(@"Files\Baskets.bbd")));

            project.WixSourceGenerated += Project_WixSourceGenerated;
            string wxs = project.BuildWxs();

            var doc = XDocument.Load(wxs);
        }

        void Project_WixSourceGenerated(XDocument document)
        {
            // var dir = document.FindAll("StandardDirectory")
            var dir = document.FindAll("Directory")
                              .Where(x => x.HasAttribute("Name", "Test Project") && x.Parent.HasAttribute("Id", "PersonalFolder"))
                              .First();

            dir.FindFirst("Component")
               .AddElement("RemoveFolder", "On=uninstall; Id=" + dir.Attribute("Id").Value)
               .AddElement("RegistryValue", @"Root=HKCU; Key=Software\[Manufacturer]\[ProductName]; Type=string; Value=; KeyPath=yes");
        }

        [Fact]
        [Description("Post 576142#post1428674")]
        public void Should_Handle_NonstandardProductVersions()
        {
            var project = new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(this.GetType().Assembly.Location)));

            project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
            project.Version = new Version("2014.1.26.0");

            Compiler.BuildMsi(project);
        }

        [Fact]
        [Description("Issue #39")]
        public void Should_Handle_EmptyFeatures()
        {
            var binaries = new Feature("MyApp Binaries");
            var docs = new Feature("MyApp Documentation");
            var docs_01 = new Feature("Documentation 01");
            var docs_02 = new Feature("Documentation 02");
            var docs_03 = new Feature("Documentation 03");

            docs.Children.Add(docs_01);
            docs.Children.Add(docs_02);
            docs.Children.Add(docs_03);

            binaries.Children.Add(docs);

            var project = new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(binaries, @"Files\Bin\MyApp.exe"),
                    new Dir(docs, @"Docs\Manual",
                        new File(docs_01, @"Files\Docs\Manual_01.txt"),
                        new File(docs_02, @"Files\Docs\Manual_02.txt"),
                        new File(docs_03, @"Files\Docs\Manual_03.txt")
                           )
                       )
                                     );

            project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

            var wsxfile = project.BuildWxs();

            var doc = XDocument.Load(wsxfile);

            var product = doc.FindSingle(Compiler.ProductElementName);

            var rootFeature = doc.Select($"Wix/{Compiler.ProductElementName}/Feature");
            Assert.NotNull(rootFeature);

            var docsFeature = rootFeature.Elements()
                                         .FirstOrDefault(e => e.HasLocalName("Feature")
                                                         && e.HasAttribute("Title", value => value == "MyApp Documentation"));
            Assert.NotNull(docsFeature);

            var doc1Feature = docsFeature.Elements()
                                         .FirstOrDefault(e => e.HasLocalName("Feature")
                                                         && e.HasAttribute("Title", value => value == "Documentation 01"));
            Assert.NotNull(doc1Feature);

            var doc2Feature = docsFeature.Elements()
                                         .FirstOrDefault(e => e.HasLocalName("Feature")
                                                         && e.HasAttribute("Title", value => value == "Documentation 02"));
            Assert.NotNull(doc2Feature);

            var doc3Feature = docsFeature.Elements()
                                         .FirstOrDefault(e => e.HasLocalName("Feature")
                                                         && e.HasAttribute("Title", value => value == "Documentation 03"));
            Assert.NotNull(doc3Feature);
        }

        [Fact]
        [Description("Issue #49")]
        public void Should_Fix_Issue_49()
        {
            {
                var project = new Project("MyProduct");

                var rootDir = new Dir(@"%ProgramFiles%",
                                  new Dir(@"AAA\BBB",
                                      new File(this.GetType().Assembly.Location)));

                project.Dirs = new[] { rootDir };
                project.UI = WUI.WixUI_InstallDir;

                var msi = project.BuildMsi();
            }

            {
                var project = new Project("MyProduct");

                var rootDir = new Dir(@"C:\",
                                  new Dir(@"Program Files (x86)\AAA\BBB",
                                      new File(this.GetType().Assembly.Location)));

                project.Dirs = new[] { rootDir };
                project.UI = WUI.WixUI_InstallDir;

                var msi = project.BuildMsi();

                //var msi = project.BuildWxs();
            }
            {
                var project = new Project("MyProduct");

                var rootDir = new Dir(@"C:\Program Files (x86)",
                                  new Dir(@"AAA\BBB",
                                      new File(this.GetType().Assembly.Location)));

                project.Dirs = new[] { rootDir };

                project.BuildMsi();
            }
        }
    }
}