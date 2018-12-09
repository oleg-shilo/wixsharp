//css_dir ..\..\;
//css_ref Wix_bin\SDK\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using WixSharp;
using System.Xml;
using System.Xml.Linq;

class Script
{
    static public void Main()
    {
        var project = new Project()
        {
            UI = WUI.WixUI_ProgressOnly,
            Name = "CustomActionTest",
            Actions = new[] { new WixQuietExecAction("notepad.exe", @"C:\boot.ini") }
        };

        project.BuildMsi();
    }
}