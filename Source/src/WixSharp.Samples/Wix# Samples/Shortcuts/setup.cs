//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.Xml;
using Microsoft.Win32;
using System.Windows.Forms;
using WixSharp;
using System.Linq;
using WixSharp.CommonTasks;

class Script
{
    static public void Main(string[] args)
    {
        try
        {
            File main_file;
            var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new InternetShortcut
                        {
                            Name = "Wix# project page",
                            Target = "https://github.com/oleg-shilo/wixsharp"
                        },
                        new Dir("Samples",
                            new File(@"AppFiles\MyApp.cs")),
                        main_file = new File(@"AppFiles\MyApp.exe",
                            new FileShortcut("MyApp", "INSTALLDIR"), //INSTALLDIR is the ID of "%ProgramFiles%\My Company\My Product"

                            // new FileShortcut("MyApp2", "INSTALLDIR") { IconFile = @"AppFiles\Icon.ico", WorkingDirectory = "%Temp%", Arguments = "333" },

                            new FileShortcut("MyApp", @"%Desktop%") { IconFile = @"AppFiles\Icon.ico", WorkingDirectory = "%Temp%", Arguments = "777" }),
                        new ExeFileShortcut("Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]")
                        {
                            WorkingDirectory = "%Temp%"
                        }
                        ),

                    new Dir(@"%ProgramMenu%\My Company\My Product",
                        new ExeFileShortcut("Samples", @"[" + @"%ProgramFiles%\My Company\My Product\Samples".ToDirID() + "]", ""),
                        new ExeFileShortcut("Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

            project.ResolveWildCards(ignoreEmptyDirectories: true)
                   .FindFile(f => f.Name.EndsWith("MyApp.exe", StringComparison.OrdinalIgnoreCase))
                   .First()
                   .AddShortcut(
                       new FileShortcut("MyApp_AnotherOnle", "INSTALLDIR")
                       {
                           IconFile = @"AppFiles\Icon.ico",
                           WorkingDirectory = "%Temp%",
                           Arguments = "777"
                       });

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.UI = WUI.WixUI_ProgressOnly;
            project.OutFileName = "setup";
            project.PreserveTempFiles = true;

            Compiler.BuildMsiCmd(project);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}