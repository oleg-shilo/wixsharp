using System;
using System.Linq;
using System.Reflection;
using WixSharp;
using WixSharp.UI.WPF;

using Custom = WixSharp.UI.WPF.Sequence;

public class Script
{
    [STAThread]
    static public void Main(string[] args)
    {
        BuildMsi();
        // TestDialogs();
    }

    static void BuildMsi()
    {
        var project = new ManagedProject("ManagedSetup",
                      new Dir(@"%ProgramFiles%\My Company\My Product",
                          new File("readme.md")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        // custom WPF dialogs
        project.ManagedUI = new ManagedUI();

        project.ManagedUI.InstallDialogs.Add<Custom.WelcomeDialog>()
                                        .Add<Custom.LicenceDialog>()
                                        .Add<Custom.FeaturesDialog>()
                                        .Add<Custom.InstallDirDialog>()
                                        .Add<Custom.ProgressDialog>()
                                        .Add<Custom.ExitDialog>();

        project.ManagedUI.ModifyDialogs.Add<Custom.MaintenanceTypeDialog>()
                                       .Add<Custom.ProgressDialog>()
                                       .Add<Custom.ExitDialog>();

        // custom WPF dialog (this project):        Custom.ProgressDialog
        // stock WPF dialog (WixSharp.UI.WPF.dll):  WixSharp.UI.WPF.ProgressDialog

        // project.ManagedUI = ManagedWpfUI.Default;   // WPF based dialogs
        // project.ManagedUI = ManagedUI.DefaultWpf;   // the same as ManagedWpfUI.Default

        // project.ManagedUI = ManagedUI.Default;      // WinForm based dialogs

        // project.PreserveTempFiles = true;

        project.BuildMsi();
    }

    static void TestDialogs()
    {
        UIShell.Play(
            "WixSharp_UI_INSTALLDIR=INSTALLDIR", // required by InstallDirDialog for initialization of the demo MSI session
            typeof(Custom.WelcomeDialog),
            typeof(Custom.LicenceDialog),
            typeof(Custom.InstallDirDialog),
            typeof(Custom.MaintenanceTypeDialog),
            typeof(Custom.SetupTypeDialog),
            typeof(Custom.FeaturesDialog),
            typeof(Custom.ProgressDialog),
            typeof(Custom.ExitDialog));
    }
}