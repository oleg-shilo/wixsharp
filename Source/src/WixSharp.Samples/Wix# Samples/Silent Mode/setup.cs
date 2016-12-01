//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

//http://wix.sourceforge.net/manual-wix2/qtexec.htm

class Script
{
    static public void Main(string[] args)
    {
        Project project = 
            new Project("My Product",
                
                new Dir(@"%ProgramFiles%\My Company\My Product",
                         new File("Readme.txt")),
                
                new PathFileAction(
                            @"%WindowsFolder%\notepad.exe", 
                            "readme.txt", 
                            "INSTALLDIR", 
                            Return.asyncNoWait, 
                            When.After, 
                            Step.InstallFinalize, 
                            new Condition("(NOT Installed) AND (UILevel > 3)")) //execute this action during the installation but only if it is not silent mode (UILevel > 3)
            );

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b"); 
        project.SourceBaseDir = Environment.CurrentDirectory;
        project.OutFileName = "setup";

        Compiler.BuildMsi(project);
    }
}



