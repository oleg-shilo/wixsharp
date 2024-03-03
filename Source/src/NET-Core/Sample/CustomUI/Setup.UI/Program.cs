//css_dir ..\..\..\;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Forms;

public static class Program
{
    static public void Main(string[] args)
    {
        WxiBuilder.UI(project =>
        {
            AutoElements.EnableUACRevealer = true;
            AutoElements.UACWarning = "Wait for UAC prompt to appear on the taskbar.";

            project.ManagedUI = new ManagedUI();

            //removing all entry dialogs and installdir
            project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                            .Add(Dialogs.Licence)
                                            .Add(Dialogs.Features)
                                            .Add(Dialogs.SetupType)
                                            .Add(Dialogs.InstallDir)
                                            .Add(Dialogs.Progress)
                                            .Add(Dialogs.Exit);

            //removing entry dialog
            project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                           .Add(Dialogs.Features)
                                           .Add(Dialogs.Progress)
                                           .Add(Dialogs.Exit);

            project.ManagedUI.Icon = "app.ico";
            project.UIInitialized += e => MessageBox.Show("UIInitialized", "WixSharp");
            project.UILoaded += e => MessageBox.Show("Project_UILoaded", "WixSharp");
        });
    }
}