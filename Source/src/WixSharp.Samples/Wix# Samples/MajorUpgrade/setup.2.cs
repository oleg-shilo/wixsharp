//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

public class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\2\MyApp.exe"),
                    new File(@"Files\2\MyApp.cs"),
                    new File(@"Files\2\manual.pdf")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.Version = new Version("1.0.714.10040");
        project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
        project.MajorUpgradeStrategy.PreventDowngradingVersions.OnlyDetect = false;

        //Of course you can use 'bool VersionRange.MigrateFeatures'. The following is just an 
        //example of how to access WiX attributes if they are not covered by Wix#
        Compiler.WixSourceGenerated += doc => doc.Root
                                                 .Select("Product/Upgrade/UpgradeVersion")
                                                 .AddAttributes("MigrateFeatures=yes");
        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project, "setup.2.msi");
    }
}


