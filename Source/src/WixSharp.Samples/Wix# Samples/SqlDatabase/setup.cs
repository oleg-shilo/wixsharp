//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe")),
                //define users
                new User(new Id("MyOtherUser"), "James") { CreateUser = true, Password = "Password1"},
                //define sql
                new SqlDatabase("MyDatabase0", ".\\SqlExpress", SqlDbOption.CreateOnInstall));
                    //new SqlString("alter login Bryce with password = 'Password1'", ExecuteSql.OnInstall)));
                
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}



