//css_dir ..\..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref WixSharp.UI;
//css_ref System.Core;
//css_ref System.Xml;

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

public static class Script
{
    static public void Main()
    {
        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"..\Files\bin\MyApp.exe")));

        project.ManagedUI = ManagedUI.Default; //Wix# ManagedUI

        Compiler.BuildMsi(project);
    }
}