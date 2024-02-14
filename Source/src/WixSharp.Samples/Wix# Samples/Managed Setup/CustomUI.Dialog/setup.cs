using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
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
        if (Environment.GetEnvironmentVariable("APPVEYOR") != null)
            return;

        if (args.Contains("/test")) //for demo only
        {
            UIShell.Play(ManagedUI.Default.InstallDialogs);
            return;
        }

        //Note if the property 'PASSWORD' is not preserved as deferred then it will not be available
        //from the Project_AfterInstall, which is a deferred custom action.
        var project = new ManagedProject("ManagedSetup",
                          new User
                          {
                              Name = Defaults.UserName,
                              Password = "[PASSWORD]",
                              Domain = "[DOMAIN]",
                              PasswordNeverExpires = true,
                              CreateUser = true
                          },
                          new Binary("CUSTOM_LNG".ToId(), "WixUI_fi-FI.wxl"),
                          new Property("PASSWORD", "pwd123") { IsDeferred = true });

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.LocalizationFile = "WixUI_de-de.wxl";
        project.Language = "de-de";
        project.SetNetFxPrerequisite("NETFRAMEWORK35='#1'", "Please install .NET 3.5 first.");
        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                        .Add<WixSharp.UI.WPF.UserNameDialog>()
                                        .Add<ProgressDialog>()
                                        .Add<ExitDialog>();

        //it effectively becomes a 'Repair' sequence
        project.ManagedUI.ModifyDialogs.Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        project.UILoaded += msi_UILoaded;
        project.UIInitialized += msi_UIInitialized;

        project.BeforeInstall += msi_BeforeInstall;
        project.AfterInstall += Project_AfterInstall;

        // just to demo how to detect unhandled exceptions in the ManagedProject costom
        // routines (CA and ManagedUI)
        project.UnhandledException += e =>
        {
            MessageBox.Show("Unhandled exception: " + e.Exception.Message, e.Session.Property("ProductName"));
            // Debug.Assert(false);
        };

        // project.PreserveTempFiles = true;

        project.BuildMsi();
    }

    static void msi_UILoaded(SetupEventArgs e)
    {
        //If required you can
        // - set the size of the shell view window
        // - scale the whole shell window and its content
        // - reposition controls on the current dialog
        // - subscribe to the current dialog changed event

        //e.ManagedUIShell.SetSize(700, 500);
        //e.ManagedUIShell.OnCurrentDialogChanged += ManagedUIShell_OnCurrentDialogChanged;
        //(e.ManagedUIShell.CurrentDialog as Form).Controls....
    }

    //private static void ManagedUIShell_OnCurrentDialogChanged(IManagedDialog dialog)
    //{
    //}
    static void msi_UIInitialized(SetupEventArgs e)
    {
        // WixUI_de-de.wxl does not contain UI data for the custom dialog but for the stock dialogs only.
        // This is how you can install localization if you prefer not to edit wxl file.

        MsiRuntime runtime = e.ManagedUI.Shell.MsiRuntime();

        bool isGerman = (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de");

        runtime.UIText["UserNameDlgTitle"] = isGerman ? "Benutzeranmeldeinformationen" : "User Credentials";
        runtime.UIText["UserNameDlgDescription"] = isGerman ? "Einstellungen für Anwendungsanmeldeinformationen" : "Application credentials settings";
        runtime.UIText["UserNameDlgNameLabel"] = isGerman ? "Nutzername" : "User Name";
        runtime.UIText["UserNameDlgPasswordLabel"] = isGerman ? "Passwort" : "Password";
        runtime.UIText["UserNameDlgDomainLabel"] = isGerman ? "Domain" : "Domain";
        runtime.UIText["UserNameDlgDomainTypeLabel"] = isGerman ? "Domänentyp" : "Domain Type";
        runtime.UIText["UserNameDlgLocalDomainLabel"] = isGerman ? "Lokal" : "Local";
        runtime.UIText["UserNameDlgNetworkDomainLabel"] = isGerman ? "Netzwerk" : "Network";
        runtime.UIText["CopyDataMenu"] = isGerman ? "Daten kopieren" : "Copy Data";
    }

    static void Project_AfterInstall(SetupEventArgs e)
    {
        //Debug.Assert(false);
        MessageBox.Show(e.Data["test"], "Project_AfterInstall");
        if (e.IsInstalling)
        {
            MessageBox.Show($"User '{Defaults.UserName}' with password '{e.Session.Property("PASSWORD")}' has been created");
        }
    }

    static void msi_BeforeInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.Session.Property("PASSWORD"), "msi_BeforeInstall");
        //Note: the property will not be from UserNameDialog if MSI UI is suppressed
        if (e.Session["DOMAIN"] == null)
            e.Session["DOMAIN"] = Environment.MachineName;
    }
}