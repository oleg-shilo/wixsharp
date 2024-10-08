//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
// css_ref Wix_bin\WixToolset.Mba.Core.dll;
//css_ref C:\Users\oleg.shilo\.nuget\packages\wixtoolset.mba.core\4.0.1;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Forms;
using WixSharp.UI;

class Script
{
    static public void Main()
    {
        var project = new ManagedProject("MyProduct",
                      new Dir(@"C:\My Company\My Product",
                          new File("readme.txt")));

        project.ManagedUI = new ManagedUI();

        project.ManagedUI.InstallDialogs.Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

        project.ManagedUI.ModifyDialogs.Add(Dialogs.Progress)
                                       .Add(Dialogs.Exit);

        project.UIInitialized += (SetupEventArgs e) =>
        {
            if (e.IsInstalling && !e.IsUpgradingInstalledVersion)
            {
                e.Session["ALLUSERS"] = "2";
                if (MessageBox.Show("Install for 'All Users'?", e.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    e.Session["MSIINSTALLPERUSER"] = "0"; // per-machine
                else
                    e.Session["MSIINSTALLPERUSER"] = "1"; // per-user
            }
        };

        project.BuildMsi();
    }
}