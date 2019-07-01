//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"..\Install Files\Files\Bin\MyApp.exe") { AddCloseAction = true }),
                new Property("IP_ADRESS", "127.0.0.1"),
                new IniFile("config.ini", "INSTALLDIR", IniFileAction.createLine, "discovery", "enabled", "false"),
                new IniFile("config.ini", "INSTALLDIR", IniFileAction.createLine, "info", "enabled_server", "[IP_ADRESS]"));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("1ADF5503-75F1-4EBC-ADC5-8260C1808A5B");
        project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}