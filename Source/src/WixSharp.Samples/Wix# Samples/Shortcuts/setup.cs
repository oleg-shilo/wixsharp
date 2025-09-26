//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

class Script
{
    static public void Main()
    {
        try
        {
            var project =
                new ManagedProject("My Product",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new InternetShortcut
                        {
                            Name = "Wix# project page",
                            Target = "https://github.com/oleg-shilo/wixsharp"
                        },
                        new Dir("Samples",
                            new File(@"AppFiles\MyApp.cs",
                                new FileShortcut("MyApp", @"%StartMenuFolder%")
                                {
                                    IconFile = @"AppFiles\Icon.ico",
                                    WorkingDirectory = "Samples",
                                    Arguments = "777",
                                    Description = "My Application"
                                })),

                        new File(@"AppFiles\MyApp.exe",
                            new FileShortcut("MyApp", "INSTALLDIR"), //INSTALLDIR is the ID of "%ProgramFiles%\My Company\My Product"
                            new FileShortcut("MyApp", @"%StartMenuFolder%")
                            {
                                IconFile = @"AppFiles\Icon.ico",
                                WorkingDirectory = "Samples",
                                Arguments = "777",
                                Description = "My Application"
                            },
                            new FileShortcut("MyApp2", @"%ProgramMenu%\My Company\My Product"))
                           //,
                           // // new ExeFileShortcut("Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]")
                           // new ExeFileShortcut("MyApp Setup", @"%ProgramFiles%\dotnet\dotnet.exe",
                           // "\"[INSTALLDIR]AIS Manager Setup.dll\"")
                           // {
                           //     WorkingDirectory = "%Temp%"
                           // }
                           ),

                    new Dir(@"%ProgramMenu%\My Company\My Product",
                        new DirectoryShortcut("Samples", "[Samples]"),
                        new ExeFileShortcut("Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

            // Create a directory for the startup folder
            var autoStartDir = new Dir("[StartupFolder]");

            // Add the directory to the project
            project.AddDirs(autoStartDir);

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.UI = WUI.WixUI_ProgressOnly;

            project.Platform = Platform.x64;
            // project.OutFileName = "setup";
            // project.PreserveTempFiles = true;

            Compiler.BuildMsi(project);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}