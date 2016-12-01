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
                    new File(@"CSScriptLibrary.dll"))
            },

            Actions = new[] 
            { 	
                new PathFileAction(@"%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\bin\gacutil.exe", "/u CSScriptLibrary", 
                                   "INSTALLDIR", //or explicit ID of "new Dir(new Id("my_dir_id"), @"%ProgramFiles%\My..."
                                   Return.check, When.Before, Step.InstallFinalize, Condition.Installed),

                new PathFileAction(@"%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\bin\gacutil.exe", "/i CSScriptLibrary.dll", 
                                   "INSTALLDIR", //or explicit ID of "new Dir(new Id("my_dir_id"), @"%ProgramFiles%\My..."
                                   Return.check, When.After, Step.InstallFinalize, Condition.NOT_Installed),
            }
        };
        project.BuildMsi();
    }
}



