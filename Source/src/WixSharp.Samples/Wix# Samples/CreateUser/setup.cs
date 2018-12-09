//css_dir ..\..\;
//css_ref Wix_bin\SDK\WixToolset.Dtf.WindowsInstaller.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using WixSharp;

class Script
{
    static void Main()
    {
        var project = new Project("MyProduct",
                          new User("USER")
                          {
                              Domain = Environment.MachineName, //or a domain name your setup process has rights to add users to
                              Password = "Password123",
                              PasswordNeverExpires = true,
                              CreateUser = true
                          });

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}