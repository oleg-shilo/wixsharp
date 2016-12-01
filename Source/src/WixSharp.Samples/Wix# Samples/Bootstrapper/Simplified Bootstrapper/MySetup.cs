//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using System.IO;
using WixSharp;

class InstallScript
{
    static public void Main(string[] args)
    {
        var msi = Compiler.BuildMsi(
            new Project("MySetup")
            {
                GUID = new Guid("6f330b47-2577-43ad-1195-1861ba258877")
            });
    }
}



