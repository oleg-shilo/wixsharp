//css_dir ..\..\..\;
//css_inc CustomSilentBA.cs
//css_ref WixSharp.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref Wix_bin\WixToolset.Mba.Core.dll
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;
using WixToolset.Mba.Core;

using sys = System.Reflection;

public class InstallScript
{
    static public void Main()
    {
        EnsureCompatibleWixVersion();

        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")))
            { };

        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");
        productProj.LicenceFile = "License.rtf";

        var productMsi = productProj.BuildMsi();

        var bootstrapper =
            new Bundle("My Product Suite",
                       // new PackageGroupRef("NetFx462Web"),
                       new MsiPackage(productMsi)
                       {
                           Id = "MyProductPackageId",
                           DisplayInternalUI = true,
                           Visible = true // show MSI entry in ARP
                       });

        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889c");

        // the following two assignments will hide Bundle entry form the Programs and Features (also known as Add/Remove Programs)
        bootstrapper.DisableModify = "yes";
        bootstrapper.DisableRemove = true;

        // use this custom BA to modify its behavior in order to meet your requirements
        bootstrapper.Application = new ManagedBootstrapperApplication("%this%");

        // alternatively you can use WiX stock BA that has no own UI but only shows MSI UI
        // bootstrapper.Application = new WixInternalUIBootstrapperApplication();

        // `SilentBootstrapperApplication` is WixSharp own older (from WiX3 stream) equivalent of
        // the new WiX `WixInternalUIBootstrapperApplication`.
        // bootstrapper.Application = new SilentBootstrapperApplication();
        // bootstrapper.Application = new SilentBootstrapperApplication("MyProductPackageId");

        // You can implement your own extension types and add them to the Bundle
        // bootstrapper.GenericItems.Add(new BalCondition { Condition = "some condition", Message = "Warning: ..." });

        // bootstrapper.PreserveTempFiles = true;

        bootstrapper.Build("app_setup");
    }

    static void EnsureCompatibleWixVersion()
    {
        // WiX5 has brought some breaking changes to the custom BA model.
        // The BA is now a separate process and the communication between the BA and the bundle is done via IPC.
        // https://wixtoolset.org/docs/fivefour/#burn   mentions the change (10 Apr 2024)
        // https://wixtoolset.org/docs/fivefour/oopbas/ describes the new model.(10 Apr 2024, though it's empty yet)
        // It is actually a good decision but it will require an extra effort for integrating it with WixSharp.
        // Thus, for now, Wix# is still using the WiX4 model of the custom BA.

        if (WixTools.GlobalWixVersion.Major == 5)
            WixTools.SetWixVersion(Environment.CurrentDirectory, "4.0.4");

        if (WixTools.GlobalWixVersion.Major == 4)
        {
            WixExtension.UI.PreferredVersion = "4.0.4";
            WixExtension.Bal.PreferredVersion = "4.0.2";
            WixExtension.NetFx.PreferredVersion = "4.0.2";
        }
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