using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using sys = System.Reflection;
using WixSharp;
using WixSharp.CommonTasks;
using System;

public class Script
{
    static void Main()
    {
        var project =
             new Project("EmbeddedUI_Setup",
                 new Dir(@"%ProgramFiles%\My Company\My Product",
                     new File("readme.txt")));

        project.EmbeddedUI = new EmbeddedAssembly(sys.Assembly.GetExecutingAssembly().Location);

        project.CAConfigFile = "CustomAction.config"; // optional step just for demo
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}