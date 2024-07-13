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
        WixTools.SetWixVersion(Environment.CurrentDirectory, "4.0.2");

        // As of 22 Jul 2023
        // `Driver` support in WiX4 is somewhat incomplete.
        // The sample can build but it may fail during the install.
        // Please follow this discussion: https://github.com/orgs/wixtoolset/discussions/7571
        // It is related to the open defect https://github.com/wixtoolset/issues/issues/7625
        //
        // Bu careful if you want to ask WiX team questions there. They can get defensive and
        // down vote you if you reference WixSharp syntax in your questions or suggestions.

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

        project.LibFiles.Add(@"%userprofile%\.wix\extensions\WixToolset.DifxApp.wixext\4.0.2\wixext4\difxapp_x64.wixlib");

        // project.PreserveTempFiles = true;

        project.BuildMsi();
    }
}