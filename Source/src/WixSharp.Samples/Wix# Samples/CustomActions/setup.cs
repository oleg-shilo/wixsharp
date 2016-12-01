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
                    new File(@"Files\Registrator.exe"))
            },

            Binaries = new []
            {
                new Binary(new Id("EchoBin"), @"Files\Echo.exe") 
            },

            Actions = new WixSharp.Action[] 
            { 	
                //execute installed application
                new InstalledFileAction("Registrator.exe", "/u", Return.check, When.Before, Step.InstallFinalize, Condition.Installed),
                new InstalledFileAction("Registrator.exe", "", Return.check, When.After, Step.InstallFinalize, Condition.NOT_Installed), 
                //{AttributesDefinition="Custom:Sequence=1"}, //just a demo of how you can add an attribute to the 'Custom' element associated with the 'CustomAction' 

                //execute existing application
                new PathFileAction(@"%WindowsFolder%\notepad.exe", @"C:\boot.ini", "INSTALLDIR", Return.asyncNoWait, When.After, Step.PreviousAction, Condition.NOT_Installed),

                //execute VBS code
                new ScriptAction(@"MsgBox ""Executing VBScript code...""", Return.ignore, When.After, Step.PreviousAction, Condition.NOT_Installed),
                
                //execute embedded VBS file
                new ScriptFileAction(@"Files\Sample.vbs", "Execute" , Return.ignore, When.After, Step.PreviousAction, "NOT Installed"), //raw string can be used as Condition as well

                new BinaryFileAction("EchoBin", "Executing Binary file...", Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed)
                {
                    Execute = Execute.deferred
                }
            }
        };

        Compiler.PreserveTempFiles = true;

        var file = Compiler.BuildMsi(project);
    }
}



