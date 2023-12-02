using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using WindowsInstaller;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using io = System.IO;

public static class Script
{
    static void Main()
    {
        // Read more about localization here: https://github.com/oleg-shilo/wixsharp/wiki/Localization

        var project = new ManagedProject("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File("setup.cs")));

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        //project.SourceBaseDir = "<input dir path>";
        //project.OutDir = "<output dir path>";

        project.ManagedUI = ManagedUI.Default;

        var oneStepTransform = false;
        if (oneStepTransform)
        {
            project.Language = "en-US,uk-UA";
            var msiFile = project.BuildMultilanguageMsi();
        }
        else
        {
            project.Language = "en-US";
            string productMsi = project.BuildMsi();

            project.Language = "uk-UA";
            // uk-UA msi will be built (and used to create mst) in the next step
            string mstFile = project.BuildLanguageTransform(productMsi, project.Language);

            productMsi.EmbedTransform(mstFile);
            productMsi.SetPackageLanguages("en-US,uk-UA".ToLcidList());
        }
    }

    static public void Main1()
    {
        var product =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        product.InstallScope = InstallScope.perMachine;

        product.Version = new Version("1.0.0.0");
        product.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258771");
        product.Language = BA.Languages; // "en-US,de-DE,uk-UA";

        product.PreserveTempFiles = true;
        product.OutFileName = $"{product.Name}.ml.v{product.Version}";

        var msiFile = product.BuildMultilanguageMsi();

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    new MsiPackage(msiFile)
                    {
                        Id = BA.MainPackageId,
                        DisplayInternalUI = true,
                        Visible = true,
                        MsiProperties = "TRANSFORMS=[TRANSFORMS]"
                    });

        bootstrapper.SetVersionFromFile(msiFile);
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config");

        bootstrapper.SuppressWixMbaPrereqVars = true;
        bootstrapper.PreserveTempFiles = true;

        bootstrapper.Build(msiFile.PathChangeExtension(".exe"));
    }
}