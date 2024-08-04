//css_dir ..\..\;
// //css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Install Files\bin\Debug\net472\WixToolset.Dtf.WindowsInstaller.dll
//css_ref D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Install Files\bin\Debug\net472\WixToolset.Mba.Core.dll

//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI;

static class Script
{
    static public void Main()
    {
        Feature poFeature = new Feature("PO", "PO", "INSTALLDIR");

        var project = new ManagedProject("My Product",
            new Dir(@"%ProgramFiles%\My Company",
                new InstallDir("My Product",
                    new Files(poFeature, @"C:\Builds\a\*.*"),
                    new Files(poFeature, @"C:\Builds\b\*.*"))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.EmitRelativePaths = false;

        project.BuildMsi();
    }
}