//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System.Linq;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new Project("Setup",
            new Dir(@"%ProgramFiles%\My Company\My Product",
                new File(@"Files\MyApp.exe")),
            new Property("EXISTING_FILE",
                new DirectorySearch(@"%ProgramFiles%\My Company\My Product", true, 1,
                    new FileSearch("product.exe")))
            );

        // project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}