//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        Build();          // explicit definition of the COM registration info

        BuildWithHeat();  // definition of the COM registration info is automatically produced by the WiX tools
                          // (e.g. new File("<path to COM server file").RegisterAsCom())
    }

    static public void Build()
    {
        // You can also use `CommonTasks.RegisterComAssembly` to register COM servers.
        // Which is not a WiX/MSI recommended approach for COM registration, but may still
        // be a good choice of the registration technique.

        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe",
                             new TypeLib
                             {
                                 Id = new Guid("6f330b47-2577-43ad-9095-1861ba25889b"),
                                 Language = 33,
                                 MajorVersion = 23
                             },
                             new ComRegistration
                             {
                                 Id = new Guid("6f330b47-2577-43ad-9095-1861ba25889b"),
                                 Description = "MY DESCRIPTION",
                                 ThreadingModel = ThreadingModel.apartment,
                                 Context = "InprocServer32",
                                 ProgIds = new[]
                                 {
                                     new ProgId
                                     {
                                         Id = "PROG.ID.1",
                                         Description ="Version independent ProgID ",
                                         ProgIds = new[]
                                         {
                                             new ProgId
                                             {
                                                 Id = "prog.id",
                                                 Description="some description"
                                             }
                                         }
                                     }
                                 }
                             })));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }

    static public void BuildWithHeat()
    {
        // The approach is implemented by yokioda: https://github.com/oleg-shilo/wixsharp/issues/1204

        // This shows how to register COM files like .dll or .ocx without the need to depend
        // on external programs like regsvr32.exe or the use of the frowned upon self registration.
        //
        // It utilizes heat.exe from the WiX toolkit to extract the registration data and
        // adds it to the installers .wxs file, therefore eliminating the need to go through
        // each file manually to add the entries to the generated .wxs file.
        //
        //
        // CreateComObjects - Used to create better readable entries with TypeLib and ProgId
        // entries but is more prone to cause errors, therefore disabled by default.
        // You can compare the two files in the .wxs to have a look for yourself.
        //
        // HeatArguments - If you want to customize the call to heat.exe.
        // You can find the available arguments at:
        // https://wixtoolset.org/documentation/manual/v3/overview/heat.html
        //
        // OverrideDefaults - Omits the default arguments for further customization.
        // By default heat.exe is called with '-ag' and '-svb6'
        //
        // HideWarnings - If you want to hide the warnings received from heat.exe.
        // You can also pass the argument '-sw<N>' to suppress all or specific warnings.

        // Register a single File.
        var project =
            new Project("MyProduct2",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    // Either use the extension,
                    new File(@"Files\bin\CSScriptLibrary.dll").RegisterAsCom(),
                    // or create a new WixEntity.
                    new File(@"Files\bin\CSScriptLibrary2.dll",
                        new RegisterAsCom()
                        {
                            CreateComObjects = true,
                            HeatArguments = new[] { "-gg" },
                            OverrideDefaults = true,
                            HideWarnings = true
                        })
                       /*,

                     // You can also register multiple files using either Files() or DirFiles().
                     // Again using the extension which does the work for you,
                     new Files(@"Files\*.*").RegisterAsCom(true),
                     // or by adding it to each file manually.
                     new Files(@"Files\*.*")
                     {
                         // OnProcess is called for each file when it is actually created
                         // and can be used to make changes to the individual files.
                         OnProcess = file =>
                         {
                             // Here you can either use the extension or add a WixEntity again.
                             file.Add(
                                 new RegisterAsCom()
                                 {
                                     HideWarnings = true
                                 }
                             );
                         }
                     },
                     // This is literally what the extension does for Files() and DirFiles().
                     new DirFiles(@"Files\*.*")
                     {
                         OnProcess = file =>
                         {
                             file.RegisterAsCom();
                         }
                     }
                     */
                       )
                       );

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}