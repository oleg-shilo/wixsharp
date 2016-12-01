//css_dir ..\..\..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main(string[] args)
    {
        string package = Compiler.BuildPackageAsm("%this%", "myaction.CA.dll");

        BuildWithInclude(package);
        //BuildWithInjection(package);
    }

    static void BuildWithInclude(string package)
    {
        var project = new Project("CustomActionTest",
                          new Binary(new Id("MyAction_File"), package));

        project.AddXmlInclude("myaction.wxi");
        project.BuildMsi();
    }

    static void BuildWithInjection(string package)
    {
        var project = new Project("CustomActionTest",
                          new Binary(new Id("MyAction_File"), package));

        project.WixSourceGenerated +=
                document =>
                {
                    var product = document.Select("Wix/Product");
               
                    product.AddElement("CustomAction",
                                      @"Id=MyAction;
                                        BinaryKey=MyAction_File;
                                        DllEntry=MyAction;
                                        Impersonate=yes;
                                        Execute=immediate;
                                        Return=check");
               
                    var custom = product.SelectOrCreate("InstallExecuteSequence")
                                        .AddElement("Custom", 
                                                    "Action=MyAction;After=InstallInitialize",
                                                    "(NOT Installed)");
                };

        project.BuildMsi();
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        MessageBox.Show("Hello World! (CLR: v" + Environment.Version + ")", "Embedded Managed CA (" + (Is64BitProcess ? "x64" : "x86") + ")");
        session.Log("Begin MyAction Hello World");

        return ActionResult.Success;
    }

    public static bool Is64BitProcess
    {
        get { return IntPtr.Size == 8; }
    }
}