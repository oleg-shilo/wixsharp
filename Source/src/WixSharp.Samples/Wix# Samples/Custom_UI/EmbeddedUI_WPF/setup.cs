using System;
using System.Diagnostics;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;
using sys = System.Reflection;

public class Script
{
    static void Main()
    {
        var project =
            new ManagedProject("EmbeddedUI_Setup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        project.EmbeddedUI = new EmbeddedAssembly(sys.Assembly.GetExecutingAssembly().Location);

        project.CAConfigFile = "CustomAction.config"; // optional step just for demo
        project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}