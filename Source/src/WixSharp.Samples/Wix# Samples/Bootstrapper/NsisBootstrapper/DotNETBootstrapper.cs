//css_dir ..\..\..\;
//css_ref WixSharp.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;

using System;
using WixSharp.Nsis;

class Script
{
    static public void Main()
    {
        var setup = new NsisBootstrapper();
        setup.DoNotPostVerifyPrerequisite = true;
        setup.PrerequisiteFile = "NDP451-KB2859818-Web.exe";
        setup.PrimaryFile = "MainProduct.msi";
        setup.OutputFile = "MainProduct.exe";
        setup.PrerequisiteRegKeyValue = @"HKLM:SOFTWARE\Microsoft\.NETFramework\v4.0.30319\SKUs\.NETFramework,Version=v4.5:";

        setup.Build();
    }
}



