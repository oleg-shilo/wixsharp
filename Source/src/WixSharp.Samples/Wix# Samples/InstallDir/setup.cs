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
        // Please for any further information refer to
        // the https://github.com/oleg-shilo/wixsharp/wiki/Deployment-scenarios/_edit#installation-directory

        var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\My Product",
                        new WixSharp.File(@"myapp.exe",
                            new FileShortcut("My App", @"%ProgramMenu%\My Product") { Advertise = true })));
        // var project =
        //         new Project("My Product",
        //             new InstallDir(@"%ProgramFiles%\My Product",
        //                 new WixSharp.File(@"myapp.exe",
        //                     new FileShortcut("My App", @"%ProgramMenu%\My Product") { Advertise = true })));

        project.UI = WUI.WixUI_InstallDir;
        project.PreserveTempFiles = true;
        
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