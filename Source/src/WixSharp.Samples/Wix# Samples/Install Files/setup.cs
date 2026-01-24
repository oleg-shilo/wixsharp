//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref Wix_bin\WixToolset.Mba.Core.dll
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI;

static class Script
{
    [STAThread]
    static public void Main(string[] args)
    {
        if (args.Contains("-test")) //for demo only
        {
            ManagedUI.DefaultWpf.PlayInstallDialogs();
            // ManagedUI.Default.PlayInstallDialogs();
            return;
        }

        File f;

        var project =
            new ManagedProject("MyProduct",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        f = new File("MyApp_file".ToId(),
                                @"Files\Bin\MyApp.exe",
                                new FileAssociation("cstm", "application/custom", "open", "\"%1\"")
                                {
                                    Advertise = true,
                                    Icon = "wixsharp.ico"
                                })
                        {
                            TargetFileName = "app.exe"
                        },
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt")
                        {
                            NeverOverwrite = true,
                            Condition = new Condition("IS64=yes")
                        })),
                    new Property("PropName", "test"));

        project.SetVersionFrom("MyApp_file");

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        Compiler.EmitRelativePaths = false;
        // possible UIs
        project.ManagedUI = ManagedUI.Default;
        // project.ManagedUI = ManagedUI.DefaultWpf;
        // project.UI = WUI.WixUI_Mondo;
        // project.UI = WUI.WixUI_InstallDir;

        AutoElements.UseModernFolderBrowserDialog = true;

        project.EmitConsistentPackageId = true;
        project.Scope = InstallScope.perMachine;
        // project.PreserveTempFiles = true;
        // project.PreserveDbgFiles = true;

        project.SetNetPrerequisite("10.0.0", RuntimeType.desktop, RollForward.minor, Platform.x64);

        project.EnableUninstallFullUI();
        project.EnableResilientPackage();

        project.Language = "en-US";

        project.WixSourceGenerated += Compiler_WixSourceGenerated;

        // project.WixSourceGenerated += doc =>
        // {
        //     doc.FindFirst("StandardDirectory").SetAttribute($"Id=CommonDocumentsFolder");
        // };
        // project.Include(WixExtension.Util);

        project.WixBuildCommandGenerated += cmd =>
        {
            Console.WriteLine("WixBuildCommandGenerated: " + cmd);
            return cmd;
        };
        project.BuildMsi();
    }

    private static void Compiler_WixSourceGenerated(XDocument document)
    {
        //Will make MyApp.exe directory writable.
        //It is actually a bad practice to write to program files and this code is provided for sample purposes only.
        document.FindAll("Component")
                .Single(x => x.HasAttribute("Id", value => value.Contains("MyApp_file")))
                .AddElement("CreateFolder/Permission", "User=Everyone;GenericAll=yes");
    }
}