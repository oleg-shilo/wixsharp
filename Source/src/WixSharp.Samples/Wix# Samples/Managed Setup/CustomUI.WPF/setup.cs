using MyProduct;
using System;
using System.Linq;
using WixSharp;
using WixSharp.UI.Forms;

public class Script
{
    static public void Main(string[] args)
    {
        var project = new ManagedProject("ManagedSetup",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File("readme.md")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()       // stock WinForm dialog
                                        .Add<FeaturesDialog>()      // stock WinForm dialog
                                        .Add<CustomDialogRawView>() // custom WPF dialog
                                        .Add<CustomDialogView>()    // custom WPF dialog (with Claiburn.Micro as MVVM)
                                        .Add<ProgressDialog>()      // stock WinForm dialog
                                        .Add<ExitDialog>();         // stock WinForm dialog

        project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        project.PreserveTempFiles = true;
        project.SourceBaseDir = @"..\..\";

        project.BuildMsi();
    }
}