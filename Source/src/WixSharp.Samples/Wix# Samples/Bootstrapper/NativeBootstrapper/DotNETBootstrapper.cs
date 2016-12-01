//css_dir ..\..\..\;
//css_ref WixSharp.dll;
//css_ref System.Core.dll;

using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        var setup = new NativeBootstrapper();
        setup.PrerequisiteFile = @"C:\Users\Admin\Downloads\dotnetfx.exe";
        setup.PrimaryFile = "MainProduct.msi";
        setup.OutputFile = "dotnet_setup.exe";
        setup.PrerequisiteRegKeyValue = @"HKLM:SOFTWARE\Microsoft\.NETFramework:InstallRoot"; //note reg key value InstallRoot will exist if "any .NET version installed"

        setup.Build();
    }
}



