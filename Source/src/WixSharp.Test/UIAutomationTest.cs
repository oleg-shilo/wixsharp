using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Windows;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

// using WixSharp.UI.WPF;
using Xunit;
using IO = System.IO;

namespace WixSharp.Test
{
    [Collection("SequentialAdminTests")]
    public class ScheduleLoad_UIAutomationTest
    {
        string automation = @"..\..\..\WixSharp.UIAutomation.Test\bin\Debug\net9.0-windows\WixSharp.UI.Automation.Test.exe";
        string logFile = $"load_scheduling.log".PathGetFullPath();

        [Fact]
        public void WpfUI_OnMsiLaunch()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
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

        [AdminOnlyFact]
        public void WpfUI_InUiAndExecute()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.ManagedUI = ManagedUI.DefaultWpf;
                project.LoadEventScheduling = LoadEventScheduling.InUiAndExecute;
            });

            logFile.Clear();

            Process.Start(automation, $"ui-wpf.OnMsiLaunch \"{msi}\"").WaitForExit();

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(3, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("Load event executed", log[1]);
            Assert.Equal("BeforeInstall event executed", log[2]);
        }

        [AdminOnlyFact]
        public void WpfUI_InUiAndExecute_NoLoad()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.ManagedUI = ManagedUI.DefaultWpf;
                project.LoadEventScheduling = LoadEventScheduling.InUiAndExecute;
            },
            addLoad: false);

            logFile.Clear();

            Process.Start(automation, $"ui-wpf.OnMsiLaunch \"{msi}\"").WaitForExit();

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(1, log.Length);
            Assert.Equal("BeforeInstall event executed", log[0]);
        }

        [AdminOnlyFact]
        public void WpfUI_InUiAndExecute_NoUi()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.ManagedUI = ManagedUI.DefaultWpf;
                project.LoadEventScheduling = LoadEventScheduling.InUiAndExecute;
            });

            logFile.Clear();

            Process.Start("msiexec", $"/i \"{msi}\" /qn").WaitForExit(); // suppress UI

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(2, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("BeforeInstall event executed", log[1]);
        }

        [AdminOnlyFact]
        public void FormsUI_InUiAndExecute_NoUi()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.ManagedUI = ManagedUI.Default;
                project.LoadEventScheduling = LoadEventScheduling.InUiAndExecute;
            });

            logFile.Clear();

            Process.Start("msiexec", $"/i \"{msi}\" /qn").WaitForExit(); // suppress UI

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(2, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("BeforeInstall event executed", log[1]);
        }

        [AdminOnlyFact]
        public void FormsUI_InUiAndExecute()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.ManagedUI = ManagedUI.Default;
                project.LoadEventScheduling = LoadEventScheduling.InUiAndExecute;
            });

            logFile.Clear();

            Process.Start(automation, $"ui-wpf.OnMsiLaunch \"{msi}\"").WaitForExit();

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(3, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("Load event executed", log[1]);
            Assert.Equal("BeforeInstall event executed", log[2]);
        }

        [Fact]
        public void FormsUI_OnMsiLaunch()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.ManagedUI = ManagedUI.Default;
                project.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch;
            });

            logFile.Clear();

            Process.Start(automation, $"ui-wpf.OnMsiLaunch \"{msi}\"").WaitForExit();

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(2, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("BeforeInstall event executed", log[1]);
        }

        [AdminOnlyFact]
        public void NativeUI_InUiAndExecute_NoUi()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.UI = WUI.WixUI_ProgressOnly;
                project.LoadEventScheduling = LoadEventScheduling.InUiAndExecute;
            });

            logFile.Clear();

            Process.Start("msiexec", $"/i \"{msi}\" /qn").WaitForExit(); // suppress UI

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(2, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("BeforeInstall event executed", log[1]);
        }

        [AdminOnlyFact]
        public void NativeUI_InUiAndExecute()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.UI = WUI.WixUI_ProgressOnly;
                project.LoadEventScheduling = LoadEventScheduling.InUiAndExecute;
            });

            logFile.Clear();

            Process.Start("msiexec", $"/i \"{msi}\"").WaitForExit();

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(3, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("Load event executed", log[1]);
            Assert.Equal("BeforeInstall event executed", log[2]);
        }

        [Fact]
        public void NativeUI_OnMsiLaunch()
        {
            var msi = buildLoadSchedulingMsi(project =>
            {
                project.UI = WUI.WixUI_ProgressOnly;
                project.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch;
            });

            logFile.Clear();

            Process.Start("msiexec", $"/i \"{msi}\"").WaitForExit();

            var log = IO.File.ReadAllLines(logFile);

            Assert.Equal(2, log.Length);
            Assert.Equal("Load event executed", log[0]);
            Assert.Equal("BeforeInstall event executed", log[1]);
        }

        // ===========================================================================================================================
        string buildLoadSchedulingMsi(Action<ManagedProject> build, bool addLoad = true, [CallerMemberName] string caller = "")
        {
            var project = new ManagedProject("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                  new File(System.Reflection.Assembly.GetExecutingAssembly().Location)));

            project.AddProperty(new Property("logFile", logFile));

            if (addLoad)
                project.Load += (e) =>
                    e.Session["logFile"].AppendLine("Load event executed");

            project.BeforeInstall += (e) =>
            {
                e.Session["logFile"].AppendLine("BeforeInstall event executed");
                e.Result = ActionResult.UserExit;
            };

            build(project);
            project.OutFileName = caller;
            return project.BuildMsi();
        }
    }

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
}

public sealed class AdminOnlyFactAttribute : FactAttribute
{
    public AdminOnlyFactAttribute()
    {
        if (!WindowsIdentity.GetCurrent().IsAdmin())
            Skip = "Test requires Administrator privileges.";
    }
}