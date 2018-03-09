//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using System.Windows.Forms;

class Script
{
    static public void Main(string[] args)
    {
        // Wix# recognizes the top level dir as the installation directory and automatically
        // assigns it the INSTALLDIR id.
        // If for whatever reason it is undesirable you can always designate the installation directory
        // by setting the Dir.IsInstallDir to true or use a dedicated 'InstallDir' class that does
        // it for you automatically.
        //
        // Please for any further infotrmation refer to
        // the https://github.com/oleg-shilo/wixsharp/wiki/Deployment-scenarios/_edit#installation-directory

        var project =
                new Project("My Product",
                    new Dir(new Id("INSTALL_DIR"), @"%ProgramFiles%\1My Product",
                        new WixSharp.File(@"myapp.exe",
                            new FileShortcut("My App", @"%ProgramMenu%\1My Product") { Advertise = true }))
                    // ,                    new Dir(@"%ProgramMenu%\1My Product")
                    );
        // new Dir(new Id("My_Product"), @"%ProgramMenu%\1My Product"));

        // var project = new Project("CustomActionTest",
        //                   new InstallDir(@"%ProgramFiles%\CustomActionTest",
        //                       new File("readme.txt")),

        //                   new ManagedAction(CustomActions.MyAction));

        var dir = project.FindDir(@"%ProgramFiles%\CustomActionTest");

        //project.Platform = Platform.x64;
        project.UI = WUI.WixUI_InstallDir;
        project.PreserveTempFiles = true;
        // project.BuildWxs();
        project.BuildMsi("setup.msi");
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        try
        {
            MessageBox.Show(session["INSTALLDIR"], "InstallDir (actual INSTALLDIR)");
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "Error");
        }
        return ActionResult.Success;
    }
}