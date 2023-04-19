//css_dir ..\..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
               new Project("MyMergeModule",
                   new Dir(@"%ProgramFiles%\My Company",
                       new File(@"File1.txt"),
                       new File(@"File2.txt")));

        project.UI = WUI.WixUI_ProgressOnly;

        var msm = Compiler.BuildMsm(project);
    }
}