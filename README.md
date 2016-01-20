# Wix# (WixSharp) - managed interface for WiX

_Framework for building a complete MSI or WiX source code by using script files written with the C# syntax._

The project is currently being prepared for migration to GitHub from CodePlex: [Wix#](https://wixsharp.codeplex.com/).

The following is a simple code sample just to give you the idea bout the product:

```c#
using System;
using WixSharp;
 
class Script
{
    static public void Main(string[] args)
    {
        var project = new Project("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File(@"Files\Docs\Manual.txt"),
                              new File(@"Files\Bin\MyApp.exe")));
 
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
 
        Compiler.BuildMsi(project);
    }
}
```