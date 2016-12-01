//css_include ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project =   
            new Project()
            {
                Name = "MyProduct",
                GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b"),
                Dirs = new[]
                {
                    new Dir(@"%ProgramFiles%\My Company\My Product")
                    {
                        Files = new []
                        {
                            new File(@"Files\Docs\Manual.txt"),
                            new File(@"Files\Bin\MyApp.exe")
                        }
                    }
                }
            };

        Compiler.BuildMsiCmd(project);
    }
}
