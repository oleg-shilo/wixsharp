//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new Project("My Product",
                         new Dir(@"%ProgramFiles%\My Company\My Product",
                             new ODBCDataSource("DsnName", "SQL Server", true, true,
                                 new Property("Database", "MyDb"),
                                 new Property("Server", "MyServer"))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}