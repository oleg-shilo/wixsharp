//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        // WixQuietExecAction is not supported in WiX4
        var project = new Project()
        {
            UI = WUI.WixUI_ProgressOnly,
            Name = "CustomActionTest",
            Actions = new[] { new WixQuietExecAction("notepad.exe", @"C:\boot.ini") }
        };

        project.BuildMsi();
    }
}