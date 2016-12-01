//css_include ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var docFile = new File(@"Files\Docs\Manual.txt");
        var exeFile = new File(@"Files\Bin\MyApp.exe");
        var dir = new Dir(@"%ProgramFiles%\My Company\My Product");
        var project = new Project();

        dir.Files = new[] { docFile, exeFile };
        project.Dirs = new[] { dir };

        project.Name = "MyProduct";
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        
        Compiler.BuildMsiCmd(project);
    }
}
