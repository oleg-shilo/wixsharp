//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using File = WixSharp.File;
using WixSharp;

class Script
{
    /// <summary>
    /// The following project has 5 Features.
    /// 1) The default Feature from Project.DefaultFeature.
    /// 2) FeatureA - a normal Feature and will always be installed.
    /// 3) FeatureB - a conditional Feature.
    ///               The Condition below will install FeatureB if and only if an install property 'Prop1' has a value equal to 1.
    /// equal to 1. If Prop1 has any other value, then FeatureB will not be installed alongside FeatureA.
    /// 4) FeatureC - a normal Feature, nested under the default Feature.
    /// 5) FeatureD - a conditional Feature, nested under the default Feature.
    ///               The Condition below will install FeatureD if and only if an install property 'Prop1' has a value not equal to 1.
    /// </summary>
    /// <remarks>
    /// The summary assumes a default INSTALLLEVEL equal to 1, where Features with Level = 2 will be disabled.
    /// </remarks>
    static public void Main()
    {
        //featureA - a normal feature
        var featureA = new Feature("Feature A");

        //featureB - a conditional feature
        var featureB = new Feature("Feature B");

        //featureC - a nested feature
        var featureC = new Feature("Feature C");

        //featureD - a nested, conditional feature;
        var featureD = new Feature("Feature D");

        //Note - Level can be set explicitly via Attributes or indirectly via IsEnabled
        //featureB.AttributesDefinition = "Level=2";
        featureB.IsEnabled = false;
        featureD.IsEnabled = true;

        //if the condition evaluates to true - the level of the parent feature is updated to the level of the FeatureCondition
        featureB.Condition = new FeatureCondition("PROP1 = 1", level: 1); //disabled to enabled
        featureD.Condition = new FeatureCondition("PROP1 = 1", level: 2); //enabled to disabled; to disable, set level equal to a value higher than INSTALLLEVEL property.

        // If you want to control all your features purely via FeatureCondition then you may need to
        // set IsEnabled = false for all your features so MSI will decide if the feature is to be installed only
        // on the base of the condition evaluation. Very good simple explanation can be found here: https://www.firegiant.com/wix/tutorial/getting-started/conditional-installation/

        var project =
            new Project("FeatureCondition",
                new Dir(@"%ProgramFiles%\My Company\Features",
                    new File(@"Files\default.txt"),
                    new File(featureA, @"Files\a.txt"),
                    new File(featureB, @"Files\b.txt"),
                    new File(featureC, @"Files\c.txt"),
                    new File(featureD, @"Files\d.txt")));

        //Note - to set your own default Feature
        //var featureDefault = new Feature("Default");
        //project.DefaultFeature = featureDefault;

        project.DefaultFeature.Children.Add(featureC);
        project.DefaultFeature.Children.Add(featureD);

        project.UI = WUI.WixUI_FeatureTree;

        project.LaunchConditions.Add(new LaunchCondition("PROP1 or Installed", "PROP1 is required"));
        project.PreserveTempFiles = true;

        Compiler.BuildMsi(project);
    }
}