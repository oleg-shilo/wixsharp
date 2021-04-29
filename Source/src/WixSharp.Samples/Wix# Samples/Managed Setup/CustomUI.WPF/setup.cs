using ConsoleApplication1;
using MyProduct;
using System;
using System.Linq;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

public class Script
{
    static public void Main(string[] args)
    {
        // var ttt = typeof(CustomDialogWith<CustomDialogPanel>)
        //            .Assembly.GetReferencedAssemblies()
        //            .SelectMany(a => System.Reflection.Assembly.Load(a)
        //                                                       .GetTypes()
        //                                                       .SelectMany(t => t.GenericTypeArguments
        //                                                                         .Select(t1 => t1.Assembly.GetName())))

        //            .Where(a => a.Name.StartsWith("WixSharp.") || a.Name.StartsWith("Cliburn."))
        //            .Select(a => System.Reflection.Assembly.Load(a.FullName))
        //            .Select(a => a.Location)
        //            .Distinct()
        //            .ToArray();
        // return;

        var project = new ManagedProject("ManagedSetup",
                      new Dir(@"%ProgramFiles%\My Company\My Product",
                          new File("readme.md")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()       // stock WinForm dialog
                                        .Add<FeaturesDialog>()      // stock WinForm dialog
                                        .Add<CustomDialogWith<CustomDialogPanel>>()    // custom WPF dialog (minimalistic);
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