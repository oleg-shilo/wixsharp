//css_dir ..\..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Windows.Forms;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
               new Project("MyMergeModule1",
                   new Dir(@"%ProgramFiles%\My Company",
                       new File(@"File3.txt")));

        project.UI = WUI.WixUI_ProgressOnly;

        // see https://stackoverflow.com/questions/3800958/wix-mysterious-and-hard-to-diagnose-ice-validation-errors-on-build-server-build
        Compiler.LightOptions += "-sice:ICE103 ";

        var msm = Compiler.BuildMsm(project);
    }
}
