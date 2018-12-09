//css_dir ..\..\;
//css_ref Wix_bin\SDK\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using WixSharp;

class Script
{
    static public void Main()
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe")))
            {
                DigitalSignature = new DigitalSignature
                {
                    PfxFilePath = "wixsharp.pfx",
                    Password = "my_password",
                    Description = "MyProduct",
                    TimeUrl = new Uri("http://timestamp.verisign.com/scripts/timstamp.dll")
                }
            };

        project.UI = WUI.WixUI_ProgressOnly;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}