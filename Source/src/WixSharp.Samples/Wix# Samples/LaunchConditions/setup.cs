//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

class Script
{
    static public void Main()
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

    static public void CheckDotNetWithCustomActionTask()
    {
        var project = new ManagedProject("Setup",
            new Dir(@"%ProgramFiles%\My Company\My Product",
                new File(@"Files\MyApp.exe")));

        project.Load += e =>
        {
            if (e.IsInstalling)
            {
                // hard-codded but needs to be replaced with the real checking of registry
                // [localmachine]\Software\Microsoft\NET Framework Setup\NDP\v2.0.50727
                bool isCorrectNetFrameworkInstalled = false;

                if (!isCorrectNetFrameworkInstalled)
                {
                    string message = "Please install .NET 2.0 first.";
                    e.Session.Log(message);

                    if (e.UILevel > 4)
                        MessageBox.Show(message, e.ProductName);

                    e.Result = ActionResult.Failure;
                }
            }
        };

        Compiler.BuildMsi(project);
    }
}