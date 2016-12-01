//css_ref ..\..\..\WixSharp.dll;
//css_ref ..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.IO;
using WixSharp;

class InstallScript
{
    static public void Main(string[] args)
    {
        var msi = Compiler.BuildMsi(
            new Project("Fake CRT")
            {
                GUID = new Guid("6f330b47-2577-43ad-1195-1861ba25889b"),
                UI = WUI.WixUI_ProgressOnly
            });
    }
}



