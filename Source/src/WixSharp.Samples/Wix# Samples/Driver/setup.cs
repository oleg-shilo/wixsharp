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

        // project.Include(WixExtension.Difx);
        project.LibFiles.Add(@"%userprofile%\.wix\extensions\WixToolset.DifxApp.wixext\4.0.0\wixext4\difxapp_x64.wixlib");

        project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}