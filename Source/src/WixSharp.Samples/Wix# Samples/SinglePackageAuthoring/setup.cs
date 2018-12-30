//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref WixSharp.UI;
using WixSharp;
using WixSharp.Forms;
using Win32 = Microsoft.Win32;

class Script
{
    static public void Main()
    {
        BuildMsi();
        //BuildMsiRemember();
    }

    static private void BuildMsi()
    {
        var project = new ManagedProject("MyProduct",
                    new Dir(@"C:\My Company\My Product",
                        new File("readme.txt")));
        project.ManagedUI = new ManagedUI();

        project.ManagedUI.InstallDialogs.Add(Dialogs.InstallScope)
                                        .Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

        project.ManagedUI.ModifyDialogs.Add(Dialogs.Progress)
                                       .Add(Dialogs.Exit);

        project.BuildMsi();
    }

    /// <summary>
    /// Remember if it was a Per-user or per-machine installation
    /// </summary>
    static private void BuildMsiRemember()
    {
        var project = new ManagedProject("MyProduct",
            new Dir(@"C:\My Company\My Product",
                new File("readme.txt")),
            new RegValue(RegistryHive.LocalMachine, @"SOFTWARE\My Company\My Product", "InstallDir", "[INSTALLDIR]") { Condition = new Condition("ALLUSERS=\"1\"") },
            new RegValue(RegistryHive.CurrentUser, @"SOFTWARE\My Company\My Product", "InstallPerUser", "yes") { Condition = new Condition("MSIINSTALLPERUSER=\"1\"") }
        );
        project.ManagedUI = new ManagedUI();

        project.ManagedUI.InstallDialogs.Add(Dialogs.InstallScope)
                                        .Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

        project.ManagedUI.ModifyDialogs.Add(Dialogs.Progress)
                                       .Add(Dialogs.Exit);

        //project.Version = new Version(2, 0);

        project.UIInitialized += (SetupEventArgs e) =>
        {
            if (e.IsInstalling && !e.IsUpgrading)
            {
                e.Session["ALLUSERS"] = "2";

                var installPerUser = (string)Win32.Registry.GetValue(Win32.Registry.CurrentUser.Name + @"\SOFTWARE\My Company\My Product", "InstallPerUser", "no");
                e.Session["MSIINSTALLPERUSER"] = installPerUser == "yes" ? "1" : "0";

                if (string.IsNullOrEmpty(installPerUser) || installPerUser == "no")
                {
                    var installDir = (string)Win32.Registry.GetValue(Microsoft.Win32.Registry.LocalMachine.Name + @"\SOFTWARE\My Company\My Product", "InstallDir", string.Empty);

                    if (!string.IsNullOrEmpty(installDir))
                    {
                        e.Session["INSTALLDIR"] = installDir;
                    }
                }
            }
        };

        project.BuildMsi();
    }
}