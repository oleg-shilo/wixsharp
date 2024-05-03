using System.Runtime.CompilerServices;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

[assembly: InternalsVisibleTo(assemblyName: "setup.aot")]

var project =
    new ManagedProject("My Product",
        new Dir(@"%ProgramFiles%\My Company\My Product",
            new File("program.cs")),
        new Property("PropName", "<your value>"));

project.AddUIProject("Setup.UI");

project.Load += (e) =>
{
    Native.MessageBox("OnLoad", "WixSharp - .NET8");
    e.Result = ActionResult.Failure;
};

project.BuildMsi();