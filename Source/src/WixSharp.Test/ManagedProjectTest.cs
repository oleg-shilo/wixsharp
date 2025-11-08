using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WixSharp.UI.ManagedUI;

// using WixSharp.UI.WPF;
using Xunit;

namespace WixSharp.Test
{
    public class ManagedUiTest
    {
        [Fact]
        public void OnlySingleWpfDialogHost_Defined_Test()
        {
            // only one type in WixSharp.UI.WPF should implemnt tis interface. It is a WinForm based WPF host.
            var host = System.Reflection.Assembly.Load("WixSharp.UI.WPF")
                                                 .GetTypes()
                                                 .Single(t => t.Implements<IWpfDialogHost>());

            Assert.Equal("WpfDialogHost", host.Name);
        }

        [Fact]
        public void CanCreate_WpfDialogHost_Test()
        {
            var shell = new UIShell();

            var host = shell.CreateDefaultWpfDialgHost();

            Assert.Equal("WpfDialogHost", host.GetType().Name);
        }

        [Fact]
        public void Can_Extract_WixLocalization_Files_Test()
        {
            var localizationFile = ManagedUI.LocalizationFilesLocation.PathCombine("WixUI_uk-UA.wxl");
            Assert.True(localizationFile.FileExists());
        }

        [Fact]
        public void CanAutoRef_WpfDialogs_Test()
        {
            var dialog = System.Reflection.Assembly.Load("WixSharp.UI.WPF").GetTypes().FirstOrDefault(x => x.Name == "WpfDialogMock");

            var refAssemblies = dialog.Assembly.GetReferencedAssemblies();
        }
    }

    public class ManagedProjectTest
    {
        [Fact]
        public void CanHandle_ResourceEncoding_Test()
        {
            //WixUI_en_us is a WiX source file that is just added to Wix# codebase as resource.
            //This file can easily come with the wrong encoding. So we need to unsure it can be parsed
            //during the installation.
            var xml = Resources.WixUI_en_us.GetString(System.Text.Encoding.UTF8);
            XDocument.Parse(xml);
        }

        [Fact]
        public void Can_Localize_UIString()
        {
            var resources = new Dictionary<string, string> { { "aaa", "AAA" }, { "bbb", "BBB [ccc] DDD" }, { "ccc", "CCC" } };
            Func<string, string> localizer =
                key =>
                {
                    if (resources.ContainsKey(key))
                        return resources[key];
                    else
                        return key;
                };

            var text = "123 [aaa] 321 [bbb]";

            var locText = text.LocalizeWith(localizer);

            Assert.Equal("123 AAA 321 BBB CCC DDD", locText);
        }

        [Fact]
        public void Can_ScheduleLoadEvent_WithManagedUI()
        {
            void Test(System.Action<ManagedProject> build, System.Action<XDocument> test)
            {
                var project = new ManagedProject("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                  new File(this.GetType().Assembly.Location)));

                project.ManagedUI = ManagedUI.DefaultWpf;
                build(project);
                project.WixSourceGenerated += (doc) => test(doc);
                project.BuildWxs();
            }
            // -------------------------------
            Test(
                project =>
                {
                    project.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch;
                    project.Load += (e) => { };
                },
                doc =>
                {
                    Assert.True(
                        doc.FindAll("Property").FirstOrDefault(x => x.HasAttribute("Id", "LoadEventScheduling"))?.HasAttribute("Value", "OnMsiLaunch"));

                    Assert.True(
                        doc.FindFirst("InstallExecuteSequence")?.FindAll("Custom").Any(x => x.HasAttribute("Action", "WixSharp_Load_Action")));

                    Assert.Null(
                        doc.FindFirst("InstallUISequence"));
                });
            // -------------------------------
            Test(
                project =>
                {
                    project.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch;
                    // project.Load += (e) => { };
                },
                doc =>
                {
                    Assert.True(
                        doc.FindAll("Property").FirstOrDefault(x => x.HasAttribute("Id", "LoadEventScheduling"))?.HasAttribute("Value", "OnMsiLaunch"));

                    Assert.False(
                        doc.FindFirst("InstallExecuteSequence")?.FindAll("Custom").Any(x => x.HasAttribute("Action", "WixSharp_Load_Action")));

                    Assert.Null(
                        doc.FindFirst("InstallUISequence"));
                });
        }

        [Fact]
        public void Can_ScheduleLoadEvent_WithNativeUI()
        {
            void Test(System.Action<ManagedProject> build, System.Action<XDocument> test)
            {
                var project = new ManagedProject("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                  new File(this.GetType().Assembly.Location)));

                project.UI = WUI.WixUI_InstallDir;
                build(project);
                project.WixSourceGenerated += (doc) => test(doc);
                project.BuildWxs();
            }
            // -------------------------------
            Test(
                project =>
                {
                    project.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch;
                    project.Load += (e) => { };
                },
                doc =>
                {
                    Assert.True(
                        doc.FindAll("Property").FirstOrDefault(x => x.HasAttribute("Id", "LoadEventScheduling"))?.HasAttribute("Value", "OnMsiLaunch"));

                    Assert.True(
                        doc.FindFirst("InstallExecuteSequence").FindAll("Custom").Any(x => x.HasAttribute("Action", "WixSharp_Load_Action")));

                    Assert.True(
                        doc.FindFirst("InstallUISequence").FindAll("Custom").Any(x => x.HasAttribute("Action", "WixSharp_Load_Action")));
                });

            // -------------------------------
            Test(
                project =>
                {
                    project.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch;
                    // project.Load += (e) => { };
                },

                doc =>
                {
                    Assert.False(
                        doc.FindAll("Property").FirstOrDefault(x => x.HasAttribute("Id", "LoadEventScheduling"))?.HasAttribute("Value", "OnMsiLaunch") == true);

                    Assert.False(
                        doc.FindFirst("InstallExecuteSequence")?.FindAll("Custom").Any(x => x.HasAttribute("Action", "WixSharp_Load_Action")));

                    Assert.Null(
                        doc.FindFirst("InstallUISequence"));
                });
        }
    }
}