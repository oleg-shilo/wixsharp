//css_dir ..\..\;
//css_ref Wix_bin\SDK\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using WixSharp;
using System.Xml;
using System.Xml.Linq;

class Script
{
    static public void Main()
    {
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