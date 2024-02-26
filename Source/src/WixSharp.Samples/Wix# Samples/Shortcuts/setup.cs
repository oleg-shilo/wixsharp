//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Linq;
using System.Xml;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        try
        {
            Compiler.AutoGeneration.LegacyDefaultIdAlgorithm = true;

            var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\Invantive Software BV\Invantive Build Tool 24.0",
                        new File(@"AppFiles\MyApp.exe",
                            new FileShortcut("MyApp", "INSTALLDIR"), //INSTALLDIR is the ID of "%ProgramFiles%\My Company\My Product"
                            new FileShortcut("MyApp", @"%Desktop%")
                            {
                                IconFile = @"AppFiles\Icon.ico",
                                WorkingDirectory = "Samples",
                                Description = "My Application",
                                Arguments = "777"
                            })),

                    new Dir(@"%ProgramMenu%\Invantive Software BV\Invantive Build Tool 24.0",
                        new DirectoryShortcut("Samples", "[Samples]"),
                        new ExeFileShortcut("Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.UI = WUI.WixUI_ProgressOnly;

            project.Platform = Platform.x64;
            project.PreserveTempFiles = true;

            Compiler.BuildMsi(project);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}