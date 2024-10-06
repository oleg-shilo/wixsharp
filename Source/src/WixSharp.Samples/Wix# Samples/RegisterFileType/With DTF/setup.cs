//css_dir ..\..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;

class Script
{
    static public void Main(string[] args)
    {
        var project = new Project("Setup",
                new ElevatedManagedAction(CustonActions.RegisterFileType, Return.ignore, When.After, Step.InstallInitialize, Condition.NOT_BeingRemoved),
                new ElevatedManagedAction(CustonActions.UnRegisterFileType, Return.ignore, When.After, Step.InstallInitialize, Condition.Installed));

        Compiler.PreserveTempFiles = true;

        Compiler.BuildMsi(project);
    }
}

public class CustonActions
{
    static RegistryKey OpenOrCreateKey(string path)
    {
        var key = Registry.ClassesRoot.OpenSubKey(path, true);

        if (key == null)
        {
            Registry.ClassesRoot.CreateSubKey(path);
            key = Registry.ClassesRoot.OpenSubKey(path, true);
        }
        return key;
    }

    [CustomAction]
    public static ActionResult RegisterFileType(Session session)
    {
        try
        {
            using (RegistryKey key = OpenOrCreateKey(@".my"))
                key.SetValue("", "myfile");

            using (RegistryKey key = OpenOrCreateKey(@"myfile\shell\open\command"))
                key.SetValue("", Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\NOTEPAD.EXE %1"));
        }
        catch { }

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UnRegisterFileType(Session session)
    {
        try
        {
            Registry.ClassesRoot.DeleteSubKeyTree(".my");
            Registry.ClassesRoot.DeleteSubKeyTree(".myfile");
        }
        catch { }

        return ActionResult.Success;
    }
}