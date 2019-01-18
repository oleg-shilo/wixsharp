//css_dir ..\..\..\;
//css_ref WixSharp.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.Bootstrapper;
using sys = System.Reflection;

public class InstallScript
{
    static public void Main()
    {
        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")))
            {
                InstallScope = InstallScope.perMachine
            };

        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");
        productProj.LicenceFile = "License.rtf";
        var productMsi = productProj.BuildMsi();

        var bootstrapper =
            new Bundle("My Product Suite",
                       new PackageGroupRef("NetFx40Web"),
                       new MsiPackage(productMsi)
                       {
                           Id = "MyProductPackageId",
                           DisplayInternalUI = true,
                           Visible = true // show MSI entry in ARP
                       });

        bootstrapper.SuppressWixMbaPrereqVars = true; //needed because NetFx40Web also defines WixMbaPrereqVars
        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889c");

        // the following two assignments will hide Bundle entry form the Programs and Features (also known as Add/Remove Programs)
        bootstrapper.DisableModify = "yes";
        bootstrapper.DisableRemove = true;

        // if primary package Id is not defined then the last package will be treated as the primary one
        // bootstrapper.Application = new SilentBootstrapperApplication();

        // use this custom BA to modify its behavior in order to meet your requirements
        bootstrapper.Application = new ManagedBootstrapperApplication("%this%");

        // You can implement your own extension types and add them to the Bundle
        // bootstrapper.GenericItems.Add(new BalCondition { Condition = "some condition", Message = "Warning: ..." });

        bootstrapper.PreserveTempFiles = true;
        bootstrapper.Build("app_setup");
    }
}

/// <summary>
///
/// </summary>
public class BalCondition : WixEntity, IGenericEntity
{
    /// <summary>
    /// The condition expression
    /// </summary>
    public string Condition;

    /// <summary>
    /// The condition message
    /// </summary>
    public string Message;

    public void Process(ProcessingContext context)
    {
        context.Project.Include(WixExtension.Bal); //indicate that candle needs to use WixBlExtension.dll

        var element = new XElement(WixExtension.Bal.ToXName("Condition"), Condition)
                                   .SetAttribute("Message", Message);

        context.XParent.Add(element);
    }
}