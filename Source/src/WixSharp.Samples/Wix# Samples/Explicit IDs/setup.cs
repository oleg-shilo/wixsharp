//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Linq;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project =
            new Project("MyProduct",
                new Dir(new Id("PRODUCT_INSTALLDIR"), @"%ProgramFiles%\My Company\My Product",
                    new File(new Id("App_File"), @"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(new Id("Manual_File"), @"Files\Docs\Manual.txt"))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }

    static public void AlternativeSyntaxBad()
    {
        //extremely inconvenient but technically valid
        var project =
            new Project("MyProduct",
                new Dir
                {
                    Name = "%ProgramFiles%".Expand(),
                    Dirs = new[] { new Dir {
                                       Name = "My Company",
                                       Dirs = new[] { new Dir {
                                                          Id = "PRODUCT_INSTALLDIR",
                                                          Name = "My Product",
                                                          Files = new[] { new File { Id = "App_File", Name = @"Files\Bin\MyApp.exe" } },
                                                          Dirs = new[] { new Dir {
                                                                             Name = "Docs",
                                                                             Dirs = new[] { new Dir {
                                                                                                Name = "Manual",
                                                                                                Files = new[] { new File { Id="Manual_File", Name=@"Files\Docs\Manual.txt" } } } }} } } }}}
                });

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildWxs(project);
    }

    static public void AlternativeSyntax()
    {
        //less inconvenient
        var project =
             new Project("MyProduct",
                 new Dir(@"%ProgramFiles%\My Company\My Product",
                     new File(@"Files\Bin\MyApp.exe") { Id = "App_File" },
                     new Dir(@"Docs\Manual",
                         new File(@"Files\Docs\Manual.txt") { Id = "Manual_File" })));

        //Note: the first dir in the project constructor is converted
        //in the sequence of nested dirs based on the 'path'. There the first dir is 
        //%ProgramFiles% and the last one is 'My Product'.
        //We need to set the Id to the 'My Product' 
        project.AllDirs.Single(d=>d.Name == "My Product").Id = "PRODUCT_INSTALLDIR";

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildWxs(project);
    }
}



