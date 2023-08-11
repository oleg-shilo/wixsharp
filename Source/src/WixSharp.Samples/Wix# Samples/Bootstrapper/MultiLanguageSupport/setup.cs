using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;

using io = System.IO;

public static class Script
{
    [STAThread]
    static void Main()
    {
        if (appFileName == "setup.exe")
            RunAsBA();          // msi-running session
        else
            RunAsMsiBuilder();  // msi-building session
    }

    static string appFileName => System.Reflection.Assembly.GetExecutingAssembly().Location.PathGetFileName();

    static void RunAsMsiBuilder()
    {
        // A custom BA with the language selection will be prepared
        BuildMsiWithLanguageSelectionBootstrapper();

        // a custom msi launcher with the language selection will be prepared
        // It is an alternative technique for building a simplistic managed bootstrapper that is nothing else but simple
        // .NET executable.
        {
            // var msiFile = BuildMultilanguageMsi();

            // aggregate deployment files
            // var appFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // io.File.Copy(msiFile, msiFile.PathGetDirName().PathCombine("bin", msiFile.PathGetFileName()), true);
            // io.File.Copy(appFile, msiFile.PathGetDirName().PathCombine("bin", "setup.exe"), true);
            // io.File.Copy(appFile.PathChangeFileName("WixSharp.dll"), msiFile.PathGetDirName().PathCombine("bin", "WixSharp.dll"), true);
            // io.File.Copy(appFile.PathChangeFileName("WixSharp.Msi.dll"), msiFile.PathGetDirName().PathCombine("bin", "WixSharp.Msi.dll"), true);
            // io.File.Copy(appFile.PathChangeFileName("WixToolset.Mba.Core.dll"), msiFile.PathGetDirName().PathCombine("bin", "WixToolset.Mba.Core.dll"), true);

            // Console.WriteLine("=========================");
            // Console.WriteLine($" The setup files are aggregated in {msiFile.PathGetDirName().PathCombine("bin")}.");
            // Console.WriteLine("=========================");
        }
    }

    static void RunAsBA()
    {
        // Debug.Assert(false);
        // A poor-man BA. Provided only as an example for showing how to let user select the language and run the corresponding localized msi.
        // BuildMsiWithLanguageSelectionBootstrapper with a true BA is a better choice but currently WiX4 has a defect preventing
        // showing msi internal UI from teh custom BA.
        ConsoleHelper.HideConsoleWindow();

        var msiFile = io.Path.GetFullPath("MyProduct.msi");
        try
        {
            var installed = AppSearch.IsProductInstalled("{6fe30b47-2577-43ad-9095-1861ca25889c}");
            if (installed)
            {
                Process.Start("msiexec", $"/x \"{msiFile}\"");
            }
            else
            {
                var view = new MainView();
                if (view.ShowDialog() == true)
                {
                    if (view.SupportedLanguages.FirstOrDefault().LCID == view.SelectedLanguage.LCID) // default language
                        Process.Start("msiexec", $"/i \"{msiFile}\"");
                    else
                        Process.Start("msiexec", $"/i \"{msiFile}\" TRANSFORMS=:{view.SelectedLanguage.LCID}");
                }
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    static string BuildMultilanguageMsi()
    {
        var project = new Project("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File("setup.cs")));

        project.ProductId = new Guid("6fe30b47-2577-43ad-9095-1861ca25889c");
        project.UpgradeCode = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");

        var msiFile = project.BuildMultilanguageMsiFor(MainView.Languages);

        // The code below shows how to embed language resources manually instead of calling `BuildMultilanguageMsiFor`
        // project.Language = "en-US";
        // string productMsi = project.BuildMsi();

        // project.Language = "uk-UA";
        // string mstFile = project.BuildLanguageTransform(productMsi, project.Language);

        // productMsi.EmbedTransform(mstFile);
        // productMsi.SetPackageLanguages("en-US,uk-UA".ToLcidList());

        return msiFile;
    }

    static public void BuildMsiWithLanguageSelectionBootstrapper()
    {
        // Compiler.PreserveTempFiles = true;

        var product =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        product.Version = new Version("1.0.0.0");
        product.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258771");
        product.OutFileName = $"{product.Name}.ml.v{product.Version}";
        var msiFile = product.BuildMultilanguageMsiFor(MainView.Languages);

        var bootstrapper =
                new Bundle("My Product Bundle",
                    new PackageGroupRef("NetFx462Web"),
                    new MsiPackage(msiFile)
                    {
                        Id = BA.MainPackageId,
                        DisplayInternalUI = true, // WiX4 bug: Internal UI is not displayed for custom BAs (https://github.com/orgs/wixtoolset/discussions/7655)
                        Visible = true,
                        MsiProperties = "TRANSFORMS=[TRANSFORMS]"
                    });

        bootstrapper.SetVersionFromFile(msiFile);

        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application = new ManagedBootstrapperApplication("%this%");

        bootstrapper.Application.AddPayload(typeof(WixSharp.Extensions).Assembly.Location);

        // bootstrapper.PreserveTempFiles = true;

        bootstrapper.Build(msiFile.PathChangeExtension(".exe"));
    }
}

class ConsoleHelper
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void HideConsoleWindow() => ShowWindow(GetConsoleWindow(), 0);
}