//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref WixSharp.UI.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Forms;
using WixToolset.Dtf.WindowsInstaller;

class Script
{
    static void Main()
    {
        var project =
            new ManagedProject("ElevatedSetupUI",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        project.ManagedUI = ManagedUI.Default;
        project.UIInitialized += (SetupEventArgs e) =>
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show(e.Session.GetMainWindow(), "You must start the msi file as admin", e.ProductName);
                e.Result = ActionResult.Failure;

                var startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = "msiexec.exe";
                startInfo.Arguments = "/i \"" + e.MsiFile + "\"";
                startInfo.Verb = "runas";

                Process.Start(startInfo);
            }
        };

        // For native UI you will need to add managed action implementing restart logic as above
        // project.AddAction(new ManagedAction(CustomActions.RestartIfNotAdmin,
        //                                     Return.check,
        //                                     When.Before,
        //                                     Step.AppSearch,
        //                                     Condition.NOT_Installed,
        //                                     Sequence.InstallUISequence));

        Compiler.BuildMsi(project);
    }
}