using System;
using System.Globalization;
using System.Linq;
using ConsoleApplication1;
using WixSharp;
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
        project.ManagedUI.InstallDialogs.Add<WixSharp.UI.Forms.WelcomeDialog>()     // stock WinForm dialog
                                        .Add<CustomDialogView>()                    // custom WPF dialog (with Claiburn.Micro as MVVM)
                                        .Add<CustomDialogWith<CustomDialogPanel>>() // custom WPF dialog (minimalistic);
                                        .Add<FeaturesDialog>()                      // stock WinForm dialog
                                        .Add<CustomDialogRawView>()                 // custom WPF dialog
                                        .Add<WixSharp.UI.Forms.ProgressDialog>()    // stock WinForm dialog
                                        .Add<WixSharp.UI.Forms.ExitDialog>();       // stock WinForm dialog

        project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        project.UIInitialized += e =>
        {
            // Since the default MSI localization data has no entry for 'CustomDlgTitle' (and other custom labels) we
            // need to add this new content dynamically. Alternatively, you can use WiX localization files (wxl).

            MsiRuntime runtime = e.ManagedUI.Shell.MsiRuntime();

            runtime.UIText["CustomDlgTitle"] = "My Custom Dialog";
            runtime.UIText["CustomDlgTitleDescription"] = "My Custom Dialog Description";
        };

        // project.PreserveTempFiles = true;
        project.SourceBaseDir = @"..\..\";

        project.BuildMsi();
    }
}