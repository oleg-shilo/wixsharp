//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Text;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project =
            new Project("MyProduct",
                new FirewallException("notepad")  //global exception
                {
                    RemoteAddress = "127.0.0.1, 127.0.0.2, 127.0.0.3".Split(','),
                    Port = "8080",
                    //Program = "notepad.exe"
                },
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe",
                        new FirewallException("MyApp") //file specific exception
                        {
                            //Scope = FirewallExceptionScope.any,
                            RemoteAddress = "127.0.0.1, 127.0.0.2, 127.0.0.3".Split(',')
                        })));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.EmitConsistentPackageId = true;
        project.PreserveTempFiles = true;

        // Localization technique from the #748 "error for FirewallException"
        project.Codepage = "1251";
        // project.Language = "ru-ru;en-US"; // wix searches for the missing strings in the en-US culture.
        project.Language = "ru-ru";
        project.LocalizationFile = "FirewallExtension.ru.wxl";
        project.Encoding = Encoding.UTF8;

        project.BuildMsi();
    }
}