//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"readme.txt")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        
        if (args.Length != 0)
        {
        	switch(args[0])
        	{
        		case "ProgressOnly": project.UI = WUI.WixUI_ProgressOnly; break;
        		case "Minimal": project.UI = WUI.WixUI_Minimal; break;
        		case "InstallDir": project.UI = WUI.WixUI_InstallDir; break;
        		case "FeatureTree": project.UI = WUI.WixUI_FeatureTree; break;
        		case "Mondo": project.UI = WUI.WixUI_Mondo; break;
        	}
        }
        
        Compiler.BuildMsi(project);
    }
}



