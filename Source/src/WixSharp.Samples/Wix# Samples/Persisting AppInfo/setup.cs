//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main()
    {
        ManagedSetup();
        //NativeSetup();
    }

    static public void NativeSetup()
    {
        var project =
            new Project("MyProduct",

                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))),

                new RegValueProperty("INSTALLDIR", RegistryHive.LocalMachine, @"Software\My Company\My Product", "InstallationDirectory"),
                new RegValue(RegistryHive.LocalMachine, @"Software\My Company\My Product", "InstallationDirectory", "[INSTALLDIR]") { AttributesDefinition = "Component:Permanent=yes" });

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }

    static void ManagedSetup()
    {
        var project =
            new Project("MyProduct",

                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))),

                new ManagedAction(CustomActions.ReadInstallDir, Return.ignore, When.Before, new Step("AppSearch"), Condition.NOT_Installed, Sequence.InstallExecuteSequence | Sequence.InstallUISequence) { Execute = Execute.firstSequence },
                new ElevatedManagedAction(CustomActions.SaveInstallDir, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult SaveInstallDir(Session session)
    {
        try
        {
            Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"Software\My Company\My Product")
                                                 .SetValue("InstallationDirectory", session.Property("INSTALLDIR"));
        }
        catch { }

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult ReadInstallDir(Session session)
    {
        try
        {
            session["INSTALLDIR"] = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\My Company\My Product")
                                                                         .GetValue("InstallationDirectory")
                                                                         .ToString();
        }
        catch { }

        return ActionResult.Success;
    }
}