//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Forms;

// Truly a throw away project for dev testing

static class Script
{
    static void prepare_dirs(string root)
    {
        for (int i = 0; i < 40; i++)
        {
            var dir = root.PathJoin(i.ToString());
            System.IO.Directory.CreateDirectory(dir);
            System.IO.File.WriteAllText(dir.PathJoin($"file_{i}.txt"), i.ToString());
        }
    }

    static void Issue_298()
    {
        var project = new Project("MyProduct",
            new Dir(@"%ProgramFiles%\My Company\My Product",
                new File("setup.cs")))
        {
            Platform = Platform.x64,
            GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b")
        };

        project.AddRegValue(new RegValue(RegistryHive.LocalMachine, @"Software\test", "foo_value", "bar") { Win64 = false });

        // Compiler.LightOptions += " -sice:ICE80";
        project.PreserveTempFiles = true;
        project.BuildMsiCmd();
    }

    static void Issue_298b()
    {
        var project =
            new Project("MyProduct",
                new RegValue(RegistryHive.LocalMachine, @"Software\test", "foo_value", "bar"));

        project.PreserveTempFiles = true;
        project.BuildMsi();

        // project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");

        // // Compiler.LightOptions += " -sice:ICE80";
        // project.BuildMsiCmd();
    }

    static public void Main(string[] args)
    {
        Issue_298b(); return;
        // Compiler.AutoGeneration.LegacyDefaultIdAlgorithm = true;

        var serverFeature = new Feature("Server");
        // var completeFeature = new Feature("Complete");
        // completeFeature.Add(serverFeature);

        Project project = new Project("TaxPacc",
                // new LaunchCondition("CUSTOM_UI=\"true\" OR REMOVE=\"ALL\"", "Please run setup.exe instead."),
                new Dir(@"%ProgramFiles%\TaxPacc",
                    new File("setup.cs")),

                    new Dir(serverFeature,
                    @"%CommonAppDataFolder%\TaxPacc\Server",
                        new DirPermission("serviceaccountusername", "serviceaccountdomain", GenericPermission.All)
                ));
        project.UI = WUI.WixUI_FeatureTree;
        project.PreserveTempFiles = true;
        project.BuildMsiCmd();
    }

    static public void Main1(string[] args)
    {
        var project = new ManagedProject("IsUninstallTest",
                            new Dir(@"%ProgramFiles%\UninstallTest",
                                new File(@"files\setup.cs")));

        project.AfterInstall += Project_AfterInstall;
        project.PreserveTempFiles = true;
        project.BuildWxs();
    }

    private static void Project_AfterInstall(SetupEventArgs e)
    {
        MessageBox.Show("Is Uninstalling: " + e.IsUninstalling);
        if (e.IsUninstalling)
        {
            // e.IsUninstalling is always false if the uninstall is triggered via executing the msi again
            // and click remove in the maintenance dialog
        }
    }

    static public void Main2(string[] args)
    {
        var project = new ManagedProject("MyProduct",
                            new Dir(@"C:\My Company\My Product",
                                new File("setup.cs")));

        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

        project.ManagedUI.ModifyDialogs.Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

        project.UIInitialized += (SetupEventArgs e) =>
            {
                if (e.IsInstalling && !e.IsUpgrading)
                {
                    e.Session["ALLUSERS"] = "2";
                    if (MessageBox.Show("Install for All?", e.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        e.Session["MSIINSTALLPERUSER"] = "0";
                    else
                        e.Session["MSIINSTALLPERUSER"] = "1";
                }
            };

        project.BuildMsi();
    }

    static public void Main3(string[] args)
    {
        var application = new Feature("Application") { Name = "Application", Description = "Application" };
        var drivers = new Feature("Drivers") { Name = "Drivers", Description = "Drivers", AttributesDefinition = $"Display = {FeatureDisplay.expand}" };
        var driver1 = new Feature("Driver 1") { Name = "Driver 1", Description = "Driver 1", IsEnabled = false };
        var driver2 = new Feature("Driver 2") { Name = "Driver 2", Description = "Driver 2" };

        var project =
            new ManagedProject("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(application, @"Files\Bin\MyApp.exe"),
                    new Dir("Drivers",
                        new Dir("Driver1",
                            new File(driver1, @"Files\Docs\Manual.txt")),
                        new Dir("Driver2",
                            new File(driver2, @"Files\Docs\Manual.txt")))));

        // project.Package.AttributesDefinition = "InstallPrivileges=elevated;AdminImage=yes;InstallScope=perMachine";
        // project.UI = WUI.WixUI_InstallDir;
        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                        .Add(Dialogs.Features)
                                        .Add(Dialogs.InstallDir)
                                        .Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

        //removing entry dialog
        project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                       .Add(Dialogs.Features)
                                       .Add(Dialogs.Progress)
                                       .Add(Dialogs.Exit);

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        drivers.Add(driver1);
        drivers.Add(driver2);

        project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}