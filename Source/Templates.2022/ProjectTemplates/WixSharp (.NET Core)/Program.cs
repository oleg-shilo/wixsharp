using System.Runtime.CompilerServices;
using WixSharp;
using WixToolset.Dtf.WindowsInstaller;
using File = WixSharp.File;

[assembly: InternalsVisibleTo(assemblyName: "$safeprojectname$.aot")] // assembly name + '.aot suffix

var project =
    new ManagedProject("My Product",
        new Dir(@"%ProgramFiles%\My Company\My Product",
            new File("program.cs")),
        new Property("PropName", "<your value>"));

// project.AddUIProject("$safeprojectname$.UI"); // name of the 'Custom UI Library' project in the solution

#warning "DON'T FORGET to replace this with a freshly generated GUID and remove this `#warning` statement."
project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");

project.Load += (e) =>
{
    Native.MessageBox("OnLoad", "WixSharp - .NET8");
    e.Result = ActionResult.Failure;
};

project.BuildMsi();