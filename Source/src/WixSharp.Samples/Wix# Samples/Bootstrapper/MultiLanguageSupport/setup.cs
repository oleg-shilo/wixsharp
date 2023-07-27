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
        WixTools.Torch = @"%userprofile%\.nuget\packages\wixsharp.wix.bin\3.14.0\tools\bin\torch.exe";

        // BuildMultilanguageMsi();
        BuildMsiWithLanguageSelectionBootstrapper();
    }

    static void BuildMultilanguageMsi()
    {
        // WiX4: does note have torch.exe distributed neither with SDK or any nuget package or a tool
        // Thus this sample does not work unless you specify location of the torch.exe
        // For example from the WiX3 nuget package

        // This sample shows how to embed language resources manually
        // You can also automate this by replacing all code after `project.GUID =...` with
        //     product.Language = "en-US,ru-RU";
        //     var msiFile = product.BuildMultilanguageMsi();

        var project = new Project("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File("setup.cs")));

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        //project.SourceBaseDir = "<input dir path>";
        //project.OutDir = "<output dir path>";

        project.Language = "en-US";
        string productMsi = project.BuildMsi();

        project.Language = "ru-RU";
        string mstFile = project.BuildLanguageTransform(productMsi, "ru-RU");

        productMsi.EmbedTransform(mstFile);
        productMsi.SetPackageLanguages("en-US,ru-RU".ToLcidList());
    }

    static public void BuildMsiWithLanguageSelectionBootstrapper()
    {
        // Compiler.PreserveTempFiles = true;

        // WIX4: not ported yet
        var product =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        product.Version = new Version("1.0.0.0");
        product.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258771");
        product.Language = BA.Languages; // "en-US,de-DE,ru-RU";

        product.OutFileName = $"{product.Name}.ml.v{product.Version}";

        var msiFile = product.BuildMultilanguageMsi();

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx462Web"),
                    new MsiPackage(msiFile)
                    {
                        Id = BA.MainPackageId,
                        Visible = true,
                        MsiProperties = "TRANSFORMS=[TRANSFORMS]"
                    });

        bootstrapper.SetVersionFromFile(msiFile);
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config");

        bootstrapper.Build(msiFile.PathChangeExtension(".exe"));
    }
}