//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

public class Script
{
    static public void Main()
    {
        Compiler.AutoGeneration.ValidateCAAssemblies = CAValidation.Disabled;

        var bin = new Feature("MyApp Binaries");
        var tools = new Feature("MyApp Tools");

        var project =
            new ManagedProject("ManagedSetup",
                //one of possible ways of setting custom INSTALLDIR (disabled for demo purposes)
                new ManagedAction(Script.SetInstallDir,
                                  Return.check,
                                  When.Before,
                                  Step.LaunchConditions,
                                  Condition.NOT_Installed,
                                  Sequence.InstallUISequence),
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(bin, @"..\Files\bin\MyApp.exe"),
                    new Dir(bin, "Docs",
                        new File(bin, "readme.txt"))),

                new Dir(new Id("TOOLSDIR"), tools, "Tools",
                    new File(tools, "setup.cs")),

                //reading TOOLSDIR from registry; the alternative ways is project_UIInit
                new RegValueProperty("TOOLSDIR",
                                     RegistryHive.CurrentUser,
                                     @"SOFTWARE\7-Zip",
                                     "Path",
                                     defaultValue: @"C:\My Company\tools")
                              );

        //project.ManagedUI = ManagedUI.Empty;
        project.ManagedUI = ManagedUI.Default; //Wix# ManagedUI
        //project.UI = WUI.WixUI_ProgressOnly; //native MSI UI

        project.UILoaded += project_UIInit;
        project.UIInitialized += Project_UIInitialized;
        project.Load += project_Load;
        project.BeforeInstall += project_BeforeInstall;
        project.AfterInstall += project_AfterInstall;
        project.DefaultDeferredProperties += ",ADDLOCAL";

        project.BeforeInstall += args =>
        {
            if (!args.IsUninstalling)
                Tasks.StopService("some_service", throwOnError: false);
        };

        project.AfterInstall += args =>
        {
            if (!args.IsUninstalling)
                Tasks.StartService("some_service", throwOnError: false);
        };

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        // project.PreserveTempFiles = true;

        Compiler.BuildMsi(project);
    }

    static void Project_UIInitialized(SetupEventArgs e)
    {
        // just an example of restarting the setup UI elevated. Old fashioned but... convenient and reliable.
        if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
        {
            MessageBox.Show(e.Session.GetMainWindow(), "You must start the msi file as admin");
            e.Result = ActionResult.Failure;

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = "msiexec.exe";
            startInfo.Arguments = "/i \"" + e.MsiFile + "\"";
            startInfo.Verb = "runas";

            Process.Start(startInfo);
        }
    }

    [CustomAction]
    public static ActionResult SetInstallDir(Session session)
    {
        //set custom installdir
        ///This event is fired before native MSI UI loaded (disabled for demo purposes)
        //session["INSTALLDIR"] = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\7-Zip")
        //                                            .GetValue("Path")
        //                                            .ToString();
        return ActionResult.Success;
    }

    static void project_UIInit(SetupEventArgs e)
    {
        MessageBox.Show(e.Session.GetMainWindow(), "Hello World! (CLR: v" + Environment.Version + ")", "Managed Setup - UIInit");
        e.Session["TOOLSDIR"] = @"C:\Temp\Doc";
        //set custom installdir
        //This event is fired before Wix# ManagedUI loaded (disabled for demo purposes)
        //e.Session["INSTALLDIR"] = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\7-Zip")
        //                                              .GetValue("Path")
        //                                              .ToString();

        SetEnvVersion(e.Session);
    }

    static void SetEnvVersion(Session session)
    {
        if (session["EnvVersion"].IsEmpty())
            session["EnvVersion"] = AppSearch.IniFileValue(Environment.ExpandEnvironmentVariables(@"%windir%\win.ini"),
                                                           "System",
                                                           "Version") ?? "<unknown>";
    }

    static void project_Load(SetupEventArgs e)
    {
        MessageBox.Show(e.Session.GetMainWindow(), "Hello World! (CLR: v" + Environment.Version + ")", "Managed Setup - Load");

        var msi = e.MsiFile;

        if (!e.IsInstalling && !e.IsUpgrading)
            SetEnvVersion(e.Session);

        //MSI doesn't preserve any e.Session properties if they are accessed from deferred actions (e.g. project_AfterInstall)
        //Wix# forces some of the properties to be persisted (via CustomActionData) by using user defined
        //project.DefaultDeferredProperties ("INSTALLDIR,UILevel" by default).
        //Alternatively you can save any data to the Wix# specific fully persisted data properties "bag" SetupEventArgs.Data.
        //SetupEventArgs.Data values can be set and accesses at any time from any custom action including deferred one.
        var conn = @"Data Source=.\SQLEXPRESS;Initial Catalog=RequestManagement;Integrated Security=SSPI";
        e.Data["persisted_data"] = conn;

        MessageBox.Show(e.Session.GetMainWindow(), e.ToString(), "Load " + e.Session["EnvVersion"]);
    }

    static void project_BeforeInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.Session.GetMainWindow(), e.ToString(), "BeforeInstall");
    }

    static void project_AfterInstall(SetupEventArgs e)
    {
        //Note AfterInstall is an event based on deferred Custom Action. All properties that have
        //been pushed to e.Session.CustomActionData with project.DefaultDeferredProperties are
        //also set as environment variables just before invoking this event handler.
        //Similarly the all content of e.Data is also pushed to the environment variables.
        MessageBox.Show(e.Session.GetMainWindow(),
                        e.ToString() +
                        "\npersisted_data = " + e.Data["persisted_data"] +
                        "\nEnvVar('INSTALLDIR') -> " + Environment.ExpandEnvironmentVariables("%INSTALLDIR%My App.exe") +
                        "\nADDLOCAL = " + e.Session.Property("ADDLOCAL"),
                        caption: "AfterInstall ");
        try
        {
            System.IO.File.WriteAllText(@"C:\Program Files (x86)\My Company\My Product\Docs\readme.txt", "test");
        }
        catch { }
    }
}