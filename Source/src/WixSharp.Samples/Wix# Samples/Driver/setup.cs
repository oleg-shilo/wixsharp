//css_dir ..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using io = System.IO;

class Script
{
    static void Main()
    {
        // Not supported in WiX4: warning WIX1130: The Driver element has been deprecated.
        // WiX team has no work around. See https://github.com/orgs/wixtoolset/discussions/7571

        //NOTE: 'driver.sys' is a mock but not a real driver. Thus running msi will fail.
        var project = new Project("MyProduct",
                          new Dir(@"%ProgramFiles%\My Company\My Device",
                              new File("driver.sys",
                                        new DriverInstaller
                                        {
                                            AddRemovePrograms = false,
                                            DeleteFiles = false,
                                            Legacy = true,
                                            PlugAndPlayPrompt = false,
                                            Sequence = 1,
                                            Architecture = DriverArchitecture.x64
                                        })));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.Include(WixExtension.Difx);
        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}