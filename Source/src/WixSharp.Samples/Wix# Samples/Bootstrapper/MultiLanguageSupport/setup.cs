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
        var project = new Project("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File("Program.cs")));

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        //project.SourceBaseDir = "<input dir path>";
        //project.OutDir = "<output dir path>";

        //project.BuildMsi();

        project.Language = "en-US";
        string productMsi = project.BuildMsi();

        project.Language = "ru-RU";
        string mstFile = project.BuildLanguageTransform(productMsi, "ru-RU");

        productMsi.EmbedTransform(mstFile);
        productMsi.SetPackageLanguages("en-US,ru-RU".ToLcidList());
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
        product.Language = BA.Languages; // "en-US,de-DE,ru-RU";

        product.PreserveTempFiles = true;
        product.OutFileName = $"{product.Name}.ml.v{product.Version}";

        var msiFile = $"{product.OutFileName}.msi".PathGetFullPath();
        //var msiFile = product.BuildMultilanguageMsi();

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