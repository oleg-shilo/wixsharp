//css_dir ..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using io = System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static void Main()
    {
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

        project.BuildMsi();
    }
}



