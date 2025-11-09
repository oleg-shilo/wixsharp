using System;
using System.Diagnostics;
using System.Windows;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

// using WixSharp.UI.WPF;
using Xunit;
using IO = System.IO;

namespace WixSharp.Test
{
    static class TestExtensions
    {
        public static void Clear(this string path) => IO.File.WriteAllText(path, "");

        public static void AppendLine(this string path, string text)
        {
            if (path.FileExists())
            {
                if (text.IsNullOrEmpty())
                    IO.File.WriteAllText(path, "");
                else
                    IO.File.AppendAllLines(path, new[] { text });
            }
            else
                IO.File.WriteAllLines(path, new[] { text });
        }
    }

    public class UIAutomationTest
    {
        string automation = @"..\..\..\WixSharp.UIAutomation.Test\bin\Debug\net9.0-windows\WixSharp.UI.Automation.Test.exe";

        [Fact]
        public void Can_ScheduleLoadEvent_WithManagedUI()
        {
            var logFile = $"{nameof(Can_ScheduleLoadEvent_WithManagedUI)}.log".PathGetFullPath();

            var msi = buildLoadSchedulingMsi(project =>
                                             {
                                                 project.AddProperty(new Property("logFile", logFile));
                                                 project.ManagedUI = ManagedUI.DefaultWpf;
                                                 project.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch;
                                             });

            logFile.Clear();

            Process.Start(automation, $"ui-wpf.OnMsiLaunch \"{msi}\"").WaitForExit();

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(2, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("BeforeInstall event executed", log[1]);
        }

        // -------------------------------
        public static string buildLoadSchedulingMsi(Action<ManagedProject> build)
        {
            var project = new ManagedProject("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                  new File(System.Reflection.Assembly.GetExecutingAssembly().Location)));

            project.Load += (e) =>
                e.Session["logFile"].AppendLine("Load event executed");

            project.BeforeInstall += (e) =>
            {
                e.Session["logFile"].AppendLine("BeforeInstall event executed");
                e.Result = ActionResult.UserExit;
            };

            build(project);
            return project.BuildMsi();
        }
    }
}