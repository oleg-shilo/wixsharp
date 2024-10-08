//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        // an alternative approach using InstallUtils can be found here: https://github.com/oleg-shilo/wixsharp/issues/892
        var project = new Project()
        {
            Name = "CustomActionTest",
            UI = WUI.WixUI_ProgressOnly,

            Dirs = new[]
            {
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new Assembly(@"CSScriptLibrary.dll", true,
                        new NativeImage { Platform = NativeImagePlatform.x86}))
            }
        };
        Compiler.BuildMsi(project);
    }
}