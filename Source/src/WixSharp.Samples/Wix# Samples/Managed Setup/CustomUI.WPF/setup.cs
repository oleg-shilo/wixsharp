using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI.Forms;

internal static class Defaults
{
    public const string UserName = "MP_USER";
}

public class Script
{
    static public void Main(string[] args)
    {
        Compiler.WixLocation = @"..\..\..\..\..\Wix_bin\bin";

        if (args.Contains("-test")) //for demo only
        {
            UIShell.Play(ManagedUI.Default.InstallDialogs);
            return;
        }

        var project = new ManagedProject("ManagedSetup",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File("readme.md")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                        .Add<FeaturesDialog>()
                                        .Add<MyProduct.UserNameDialog>()
                                        .Add<ProgressDialog>()
                                        .Add<ExitDialog>();

        project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        project.PreserveTempFiles = true;
        project.SourceBaseDir = @"..\..\";

        project.BuildMsi();
    }
}