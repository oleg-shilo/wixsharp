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

        var msm = Compiler.BuildMsm(project);
    }
}