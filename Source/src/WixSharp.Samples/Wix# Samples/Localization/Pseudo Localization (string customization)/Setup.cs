//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using System.Xml;
using System.Resources;
using System.Text;
using WixSharp;

class Script
{
    static public int Main()
    {
        try
        {
            
            Project project =
                new Project("LocalizationTest",
                    new Dir(@"%ProgramFiles%\LocalizationTest",
                        new ExeFileShortcut("Uninstall Localization Test", 
                                            "[System64Folder]msiexec.exe", 
                                            "/x [ProductCode]")));
            
            project.LocalizationFile = "wixui.wxl";
            project.Encoding = Encoding.UTF8;
            project.UI = WUI.WixUI_Mondo;
            project.SourceBaseDir = Environment.CurrentDirectory;
            project.OutFileName = "Setup";

            Compiler.BuildMsi(project);

        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }
        return 0;
    }
}




