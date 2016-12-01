//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        string msi = Compiler.BuildMsi(project);
        
        int exitCode = Tasks.DigitalySign(msi,
                                          "wixsharp.pfx",
                                          "http://timestamp.verisign.com/scripts/timstamp.dll",
                                          "my_password");

        if (exitCode != 0)
            Console.WriteLine("Could not sign the MSI file.");
        else
            Console.WriteLine("the MSI file was signed successfully.");
        
    }
}



