//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core;
using System;
using System.Linq;
using WixSharp;
using System.Collections.Generic;

using Wix = WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project =

            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\MyCompany\MyProduct",
                    new DirFiles(@"Release\Bin\*.*"),
                    new Dir("GlobalResources",
                        new DirFiles(@"Release\Bin\GlobalResources\*.*")),
                    new Dir("Images",
                        new DirFiles(@"Release\Bin\Images\*.*")),
                    new ExeFileShortcut("Uninstall MyProduct", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

        project.UI = WUI.WixUI_FeatureTree;
        project.GUID = new Guid("{AC19C6E4-9724-4e90-8AC7-6E69B4AB7562}");

        project.ResolveWildCards();

        var exeFile = project.AllFiles.Single(f => f.Name.EndsWith("some.exe"));

        exeFile.Shortcuts = new[] {
                                    new FileShortcut("some.exe", "INSTALLDIR"),
                                    new FileShortcut("some.exe", @"%Desktop%")
                                  };

        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}