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

        project.UI = WUI.WixUI_InstallDir;
        // project.ManagedUI = ManagedUI.Default;

        project.Load += OnLoad;

        project.BeforeInstall += e =>
        {
            MessageBox.Show("e.InstallDir -> " + e.InstallDir, "BeforeInstall");
            e.Result = ActionResult.UserExit;
        };

        project.BuildMsi();
    }

    // This event handler will be executed only once since `project.LoadEventScheduling = LoadEventScheduling.OnMsiLaunch` by default.
    // However if you use setting `LoadEventScheduling.InUiAndExecute` the you may want to detect when this handler is executed second time
    // when UI is not suppressed. You can use `e.Session.HasDefaultValueFor("INSTALLDIR"))` to detect that.
    static void OnLoad(SetupEventArgs e)
    {
        if (e.Session.IsInstalling())
        {
            e.Session["INSTALLDIR"] = Environment.SpecialFolder.CommonDocuments.ToPath();
            e.Session["DIR2"] = @"C:\My Company";
        }
    }
}