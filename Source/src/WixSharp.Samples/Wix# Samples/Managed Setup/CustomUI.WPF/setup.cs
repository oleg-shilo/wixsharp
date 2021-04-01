using MyProduct;
using System;
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
        project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()    // stock WinForm dialog
                                        .Add<FeaturesDialog>()   // stock WinForm dialog
                                        .Add<CustomDialogView>() // custom WPF dialog
                                        .Add<ProgressDialog>()   // stock WinForm dialog
                                        .Add<ExitDialog>();      // stock WinForm dialog

        project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        project.DefaultRefAssemblies.AddRange(new[]
        {
            System.Reflection.Assembly.Load("Caliburn.Micro").Location,
            System.Reflection.Assembly.Load("Caliburn.Micro.Platform").Location,
            System.Reflection.Assembly.Load("Caliburn.Micro.Platform.Core").Location,
            System.Reflection.Assembly.Load("System.Windows.Interactivity").Location
        });

        project.PreserveTempFiles = true;
        project.SourceBaseDir = @"..\..\";

        project.BuildMsi();
    }
}