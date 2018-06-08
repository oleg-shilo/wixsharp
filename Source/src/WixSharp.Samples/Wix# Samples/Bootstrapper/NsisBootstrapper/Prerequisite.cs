//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;

using System;
using Microsoft.Win32;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project =
                   new Project("Prerequisite",
                       new Dir(@"%ProgramFiles%\My Company\My Product"),
                       new RegValue(RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Installed", "Yes"));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;

        Compiler.BuildMsi(project);
    }
}




