//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main(string[] args)
    {
        //Both methods produce the sameWiX/MSI 
        //CheckDotNetByAnalysingRegistryValue();
        //CheckDotNetWithBuildinTasObsolete(); 

        //And of course you can use PropertyRef("NETFRAMEWORK20"), see PropertyRef sample for details
        CheckDotNetWithBuildinTask();
    }

    static void CheckDotNetByAnalysingRegistryValue()
    {
        var project =
            new Project("Setup",
                new LaunchCondition("NET20=\"#1\"", "Please install .NET 2.0 first."),

                new Dir(@"%ProgramFiles%\My Company\My Product",
                     new File(@"Files\MyApp.exe")),

                new RegValueProperty("NET20", RegistryHive.LocalMachine, @"Software\Microsoft\NET Framework Setup\NDP\v2.0.50727", "Install", "0"));

        Compiler.BuildMsi(project);
    }

    static public void CheckDotNetWithBuildinTask()
    {
        var project = new Project("Setup",
           new Dir(@"%ProgramFiles%\My Company\My Product",
                new File(@"Files\MyApp.exe")));

        project.SetNetFxPrerequisite("NETFRAMEWORK20='#1'", "Please install .NET 2.0 first.");
        //project.SetNetFxPrerequisite(Condition.Net20_Installed, "Please install .NET 2.0 first.");
        //project.SetNetFxPrerequisite("NETFRAMEWORK45 >= '#378389'", "Please install .Net 4.5 First");
        //project.SetNetFxPrerequisite("NETFRAMEWORK30_SP_LEVEL and NOT NETFRAMEWORK30_SP_LEVEL='#0'", "Please install .NET 2.0 Service Pack first.");

        Compiler.BuildMsi(project);
    }
}






