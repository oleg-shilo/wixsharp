//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core;
using System;
using System.Linq;
using Wix = WixSharp;
using WixSharp;
using System.Collections.Generic;

class Script
{
    static public void Main(string[] args)
    {
        var project =

            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\MyCompany\MyProduct",
                    new Files("*.*"),
                    new ExeFileShortcut("Uninstall MyProduct", "[System64Folder]msiexec.exe", "/x [ProductCode]")));
        
        project.SourceBaseDir = System.IO.Path.Combine(Environment.CurrentDirectory, "Release");
        
        Compiler.BuildMsi(project);
    }
}

