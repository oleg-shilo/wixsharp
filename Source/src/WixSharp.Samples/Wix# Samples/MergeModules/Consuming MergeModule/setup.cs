//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.IO;
using File = WixSharp.File;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using WixSharp;
using System.Text.RegularExpressions;

class Script
{
    static public void Main()
    {
        var featureA = new Feature("Feature A", "Feature A description");
        var featureB = new Feature("Feature B", "Feature B description");
        var complete = new Feature("Complete");

        complete.Add(featureA)
                .Add(featureB);

        var project =
                new Project("MyMergeModuleSetup",
                    new MediaTemplate { CompressionLevel = CompressionLevel.none, EmbedCab = false },
                    new Dir(@"%ProgramFiles%\My Company",
                        // new File(featureA, @"Files\MainFile.txt"),
                        new Merge(featureB, @"Files\MyMergeModule.msm"),
                        new Merge(featureB, @"Files\MyMergeModule1.msm")),
                    new EnvironmentVariable("foo", "bar"));

        //must be cleared to work with MediaTemplate
        project.Media.Clear();
        project.DefaultFeature = complete;
        project.UI = WUI.WixUI_FeatureTree;
        project.InstallerVersion = 200; //you may want to change it to match MSM module installer version

        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}