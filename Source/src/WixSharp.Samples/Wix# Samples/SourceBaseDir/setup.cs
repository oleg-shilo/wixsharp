//css_dir ..\..\;
//css_ref Wix_bin\SDK\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core;
using System;
using System.Xml;
using System.Xml.Linq;
using WixSharp;

class Script
{
    static public void Main()
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