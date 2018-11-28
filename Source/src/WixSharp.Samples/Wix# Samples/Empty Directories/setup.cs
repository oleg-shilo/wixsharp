//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using WixSharp;

class Script
{
    static public void Main()
    {
        AutoElements.LegacyDummyDirAlgorithm = true;
        // AutoElements.SupportEmptyDirectories = CompilerSupportState.Enabled;
        var common = new Feature("Common Files");
        var samples = new Feature("Samples");

        var project =
            new Project("MyProduct",
                new Dir(common, @"%ProgramFiles%\My Company\My Product",
                    new Dir(common, @"Docs\Manual"),
                    new Dir(samples, @"Samples")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_FeatureTree;

        project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}