using System.Linq.Expressions;
using System.Reflection;

using WixSharp;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

var project =
    new ManagedProject("My Product",
        new Dir(@"%ProgramFiles%\My Company\My Product",
            new File("program.cs")),
        new Property("PropName", "<your value>"));

project.PreserveTempFiles = true;

project.BeforeInstall += e =>
{
    Native.MessageBox("Before Instrall", "WixSharp - .NET8");
};

project.AfterInstall += e =>
{
    Native.MessageBox("After Instrall", "WixSharp - .NET8");
};

project.Load += (e) =>
{
    Native.MessageBox("OnLoad", "WixSharp - .NET8");
    // e.Result = ActionResult.Failure;
};

project.BuildMsi();

// -----------------------------------------------
