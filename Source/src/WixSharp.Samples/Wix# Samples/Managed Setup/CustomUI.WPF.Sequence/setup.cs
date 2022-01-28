using System;
using System.Linq;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

public class Script
{
    [STAThread]
    static public void Main(string[] args)
    {
        UIShell.Play(
            "WixSharp_UI_INSTALLDIR=INSTALLDIR", // needed for InstallDir dialog
            typeof(WixSharp.UI.Forms.InstallDirDialog),
            typeof(WixSharp.UI.WPF.InstallDirDialog),
            typeof(WixSharp.UI.WPF.WelcomeDialog),
            typeof(WixSharp.UI.WPF.LicenceDialog))
            ;
        // UIShell.Play(test.InstallDialogs);

        return;

        var project = new ManagedProject("ManagedSetup",
                      new Dir(@"%ProgramFiles%\My Company\My Product",
                          new File("readme.md")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add<WixSharp.UI.Forms.WelcomeDialog>()
                                        .Add<WixSharp.UI.Forms.LicenceDialog>()
                                        .Add<WixSharp.UI.WPF.LicenceDialog>()
                                        .Add<FeaturesDialog>()      // stock WinForm dialog
                                        .Add<ProgressDialog>()      // stock WinForm dialog
                                        .Add<ExitDialog>();         // stock WinForm dialog

        // new ManagedDialogs()
        //                                 .Add<WelcomeDialog>()
        //                                 .Add<LicenceDialog>()
        //                                 .Add<SetupTypeDialog>()
        //                                 .Add<FeaturesDialog>()
        //                                 .Add<InstallDirDialog>()
        //                                 .Add<ProgressDialog>()
        //                                 .Add<ExitDialog>(),

        project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        // project.PreserveTempFiles = true;
        project.SourceBaseDir = @"..\..\";

        project.BuildMsi();
    }
}