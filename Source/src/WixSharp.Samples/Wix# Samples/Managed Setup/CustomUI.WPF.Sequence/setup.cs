using System;
using System.Linq;
using WixSharp;
using WixSharp.UI.WPF;

public class Script
{
    [STAThread]
    static public void Main(string[] args)
    {
        BuildMsi();
        // TestDialogs();
    }

    static void TestDialogs()
    {
        UIShell.Play(
            "WixSharp_UI_INSTALLDIR=INSTALLDIR", // required by InstallDirDialog for initialization of the demo MSI session
            typeof(WelcomeDialog),
            typeof(LicenceDialog),
            typeof(InstallDirDialog),
            typeof(MaintenanceTypeDialog),
            typeof(SetupTypeDialog),
            typeof(FeaturesDialog),
            typeof(ProgressDialog),
            typeof(ExitDialog));
    }

    static void BuildMsi()
    {
        var project = new ManagedProject("ManagedSetup",
                      new Dir(@"%ProgramFiles%\My Company\My Product",
                          new File("readme.md")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.ManagedUI = new ManagedUI();

        project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                        .Add<LicenceDialog>()
                                        .Add<FeaturesDialog>()
                                        .Add<ProgressDialog>()
                                        .Add<ExitDialog>();

        project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        // project.PreserveTempFiles = true;
        project.SourceBaseDir = @"..\..\";

        project.BuildMsi();
    }
}