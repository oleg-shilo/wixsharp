using System;
using System.Linq;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

public class Script
{
    static public void Main(string[] args)
    {
        var project = new ManagedProject("ManagedSetup",
                      new Dir(@"%ProgramFiles%\My Company\My Product",
                          new File("readme.md")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                        .Add<LicenceDialog>()
                                        .Add<WixSharp.UI.WPF.LicenseDialog>()
                                        .Add<FeaturesDialog>()      // stock WinForm dialog
                                        .Add<ProgressDialog>()      // stock WinForm dialog
                                        .Add<ExitDialog>();         // stock WinForm dialog

        project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        // project.PreserveTempFiles = true;
        project.SourceBaseDir = @"..\..\";

        project.BuildMsi();
    }
}