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
                    new File("readme.txt")));

        project.UI = WUI.WixUI_Minimal;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.ControlPanelInfo.Comments = "Simple test msi";
        project.ControlPanelInfo.Readme = "https://github.com/oleg-shilo/wixsharp/manual";
        project.ControlPanelInfo.HelpLink = "https://github.com/oleg-shilo/wixsharp/support";
        project.ControlPanelInfo.HelpTelephone = "111-222-333-444";
        project.ControlPanelInfo.UrlInfoAbout = "https://github.com/oleg-shilo/wixsharp/About";
        project.ControlPanelInfo.UrlUpdateInfo = "https://github.com/oleg-shilo/wixsharp/update";
        project.ControlPanelInfo.ProductIcon = "app_icon.ico";
        project.ControlPanelInfo.Contact = "Product owner";
        project.ControlPanelInfo.Manufacturer = "My Company";
        project.ControlPanelInfo.InstallLocation = "[INSTALLDIR]";
        project.ControlPanelInfo.NoModify = true;
        //project.ControlPanelInfo.NoRepair = true,
        //project.ControlPanelInfo.NoRemove = true,
        //project.ControlPanelInfo.SystemComponent = true, //if set will not be shown in Control Panel

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }
}