//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System.Linq;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new Project("Setup",

            new Dir(@"%ProgramFiles%\My Company\My Product",
                new File(@"Files\MyApp.exe"),
                new File(@"Files\DotNET 2.0 Manual.txt") { Condition = new Condition("NETFRAMEWORK20=\"#1\"") },
                new File(@"Files\DotNET 3.0 Manual.txt") { Condition = new Condition("NETFRAMEWORK30=\"#1\"") },
                new File(@"Files\DotNET 3.5 Manual.txt") { Condition = new Condition("NETFRAMEWORK35=\"#1\"") }),

            new RegValueProperty("NETFRAMEWORK20",
                                 RegistryHive.LocalMachine, @"Software\Microsoft\NET Framework Setup\NDP\v2.0.50727", "Install", "0"),

            new RegValueProperty("NETFRAMEWORK30",
                                 RegistryHive.LocalMachine, @"Software\Microsoft\NET Framework Setup\NDP\v3.0", "Install", "0"),

            new RegValueProperty("NETFRAMEWORK35",
                                 RegistryHive.LocalMachine, @"Software\Microsoft\NET Framework Setup\NDP\v3.5", "Install", "0"),

            new Property("SQL_BROWSER_LOCATION",
                new RegistrySearch(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\services\SQLBrowser", "ImagePath", RegistrySearchType.file,
                    new FileSearch("sqlbrowser.exe"))));

        project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}