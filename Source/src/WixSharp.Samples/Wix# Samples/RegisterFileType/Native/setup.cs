//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
         Project project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new Dir(@"Notepad",
                            new File(@"C:\WINDOWS\system32\notepad.exe",
                                new FileAssociation("my", "application/my", "open", "\"%1\"")))));  //can also be used as "new FileAssociation("my")"

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;
        project.OutFileName = "setup";
		
        Compiler.BuildMsi(project);
    }
}




