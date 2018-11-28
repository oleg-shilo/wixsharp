//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        // Note you can detect at runtime if the feature has been marked for installation by using condition
        // like this Condition.Create("ADDLOCAL >< \"your_feature_name\"")

        var binaries = new Feature("MyApp Binaries", "Application binaries");
        var docs = new Feature("MyApp Documentation");
        var docsLight = new Feature("MyApp Light Documentation");
        var tuts = new Feature("MyApp Tutorial");

        docs.Add(tuts);
        binaries.Add(docs);
        binaries.Add(docsLight);

        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(binaries, @"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(docs, @"Files\Docs\Manual.txt"),
                        new File(@"Files\Docs\Tutorial.txt")
                        {
                            Features = new[] { docsLight, tuts }
                        }),
                    new Dir(docs, "logdocs", new DirPermission("Everyone", GenericPermission.All)),
                    new Dir(tuts, "logtuts", new DirPermission("Everyone", GenericPermission.All))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_FeatureTree;

        project.DefaultFeature = binaries; //this line is optional

        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}