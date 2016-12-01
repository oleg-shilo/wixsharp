//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {        
    var project =
        new Project("MyProduct",
            new Dir(@"%ProgramFiles%\My Company\My Product",
                new File(@"Files\Bin\MyApp.exe"),
                new File(@"Files\Docs\Manual.txt",
                    new FilePermission("Everyone", GenericPermission.Read | GenericPermission.Execute),
                    new FilePermission("Administrator")
                        {
                            Append = true,
                            Delete = true,
                            Write = true
                        }),
                new Dir(@"Docs"),
                new Dir(@"Docs2",
                    new DirPermission("Everyone", GenericPermission.All)),
                new Dir("Empty")));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.EmitConsistentPackageId = true;
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}