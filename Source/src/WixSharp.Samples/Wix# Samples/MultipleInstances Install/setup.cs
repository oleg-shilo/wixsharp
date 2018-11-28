//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using WixSharp;
using File = WixSharp.File;
using Microsoft.Deployment.WindowsInstaller;

public class Script
{
    static public void Main()
    {
        // to install a one of the predefined instances, run from commandline
        // msiexec /i "My Product.msi" MSINEWINSTANCE=1 TRANSFORMS=":SecondInstance"
        // SecondInstance is the name of the instance, has to be one of the instance names defined in the WixSourceGenerated event below.

        var project =
            new Project("My Product",

                new Dir("dynamic_installdir",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))),

                new Property("INSTANCEID", "Default"),
                new ManagedAction(Script.GetInstanceDir, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_BeingRemoved, Sequence.InstallUISequence));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.WixSourceGenerated += document =>
            {
                var instanceTransforms = new XElement("InstanceTransforms", new XAttribute("Property", "INSTANCEID"));

                foreach (var instance in "FirstInstance,SecondInstance,ThirdInstance".Split(','))
                    instanceTransforms.Add(
                        new XElement("Instance",
                            new XAttribute("Id", instance),
                            new XAttribute("ProductCode", Guid.NewGuid()),
                            new XAttribute("ProductName", "My Product " + instance)));

                document.Root.Select("Product").Add(instanceTransforms);
            };

        Compiler.BuildMsi(project);
    }

    [CustomAction]
    public static ActionResult GetInstanceDir(Session session)
    {
        session["INSTALLDIR"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                                             "My Company", "My Product " + session["INSTANCEID"]);
        return ActionResult.Success;
    }
}