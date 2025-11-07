//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref WixSharp.UI.dll;
using System;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI;
using WixToolset.Dtf.WindowsInstaller;

class Script
{
    static public void Main()
    {
        var project =
            new ManagedProject("MyProduct",
                new Dir(new Id("INSTALLDIR"), "dummy", new File("setup.cs")),
                new Dir(new Id("DIR2"), "dummy", new Files(@"files\*.*")));

        // project.UI = WUI.WixUI_ProgressOnly;
        // project.ManagedUI = ManagedUI.Default;
        project.UI = WUI.WixUI_InstallDir;

        project.Load += OnLoad;
        // project.LoadEventExecution = EventExecution.MsiSessionScopeDeferred;
        Compiler.AutoGeneration.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch;

        project.BeforeInstall += (SetupEventArgs e) =>
        {
            MessageBox.Show("e.InstallDir -> " + e.InstallDir, "BeforeInstall");
            e.Result = ActionResult.UserExit;
        };

        // Debugger.Launch();
        project.BuildMsi();
    }

    static void OnLoad(SetupEventArgs e)
    {
        // Debug.Assert(false);

        if (e.Session.IsInstalling() && e.Session.HasDefaultValueFor("INSTALLDIR"))
        {
            e.Session["INSTALLDIR"] = Environment.SpecialFolder.CommonDocuments.ToPath();
            e.Session["DIR2"] = @"C:\My Company";
        }
    }
}