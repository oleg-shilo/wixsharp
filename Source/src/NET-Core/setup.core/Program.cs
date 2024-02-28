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

// project.Load += Project_Load;
project.Load += (e) =>
{
    Native.MessageBox("OnLoad", "WixSharp - .NET8");
    // e.Result = ActionResult.Failure;
};

project.UI = WUI.WixUI_ProgressOnly;

ValidateAotReadiness(project);

project.BuildMsi();

// -----------------------------------------------
static void ValidateAotReadiness(Project project)
{
    // check if the types declaring event handlers are public or handlers assemblies are
    // listed in this assembly `InternalsVisibleTo` attribute (usually in AssemblyInfo.cs file)
}

static void Project_Load(SetupEventArgs e)
{
    Native.MessageBox("static delegate", "WixSharp - .NET8");
    // e.Result = ActionResult.Failure;
}