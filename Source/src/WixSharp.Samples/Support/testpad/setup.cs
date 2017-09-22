//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Forms;

// Truly a throw away project for dev testing

class Script
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

    static public void Main(string[] args)
    {
        var doc = new XDocument();

        doc.SelectOrCreate("configuration/userSettings/MyApp.Properties.Settings/setting")
           .SetAttributes("name=InputPath;serializeAs=String")
           .SetElementValue("value", @"C:\Input");

        var last_element = doc.SelectOrCreate($"configuration/userSettings/MyApp.Properties.Settings/setting")
                              .SetAttributes("name=InputPath;serializeAs=String")
                              .SetElementValue("value", null, @"C:\Input");

        // prepare_dirs("dirs"); return;

        // var project = new ManagedProject("MyProduct",
        //                  new Dir(@"C:\MyCompany2", new File("setup.cs")),
        //                  new Dir(@"C:\MyCompany\MyProduct",
        //                      new Files(@"dirs\*.*")));

        var dirs = System.IO.Directory.GetFiles("dirs", "*", System.IO.SearchOption.AllDirectories)
                                      .Select(x => x.PathGetFullPath())
                                      .Select(x => new Dir(x.PathGetDirName(),
                                                        new File(x)))
                                      .ToArray();
        var project = new ManagedProject("MyProduct", dirs);

        project.UI = WUI.WixUI_ProgressOnly;

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    static public void Main1(string[] args)
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