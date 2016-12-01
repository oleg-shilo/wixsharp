//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project = new Project()
        {
            Name = "CustomActionTest",
            UI = WUI.WixUI_ProgressOnly,

            Dirs = new[]
            { 
            	new Dir(@"%ProgramFiles%\My Company\My Product",
                    new Assembly(@"CSScriptLibrary.dll", true))
            }
        };
        Compiler.BuildMsi(project);
    }
}



