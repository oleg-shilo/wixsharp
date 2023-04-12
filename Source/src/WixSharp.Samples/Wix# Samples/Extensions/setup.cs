//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;

using System;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

/// <summary>
/// <para>
/// Note, RemoveFolderEx in this sample is chosen as a good candidate for demonstrating how to integrate a WiX extension
/// with Wix#. RemoveFolderEx class can be reused in other projects without any limitations. Though do not expect any specific
/// behavior from the sample at runtime. The canonical WiX RemoveFolderEx use-case is very complicated and convoluted.
/// In order for RemoveFolderEx to work you will need to schedule an extra custom action to create a property
/// DIR_PATH_PROPERTY_NAME. Or read the property value from the registry.
/// </para>
/// <para>
/// Thus RemoveFolderEx is here only to demonstrate the WiX extensions integration technique but not how to remove folders.
/// </para>
/// In fact ManagedProject events is by far a superior choice for the task of removing folders. The following is the sample
/// of removing the installation directory reliably during uninstall:
/// <code>
/// var project = new ManagedProject("CustomActionTest"...
///
/// project.AfterInstall += (e) =>
///                         {
///                             if (e.IsUninstalling)
///                                 try
///                                 {
///                                     System.IO.Directory.Delete(e.InstallDir, true);
///                                 }
///                                 catch { /*log error if required*/ }
///                         };
/// </code>
///
/// </summary>

class Script
{
    static public void Main()
    {
        var project = new Project("CustomActionTest",
                          new Dir(@"%ProgramFiles%\CustomActionTest",
                              new RemoveFolderEx { On = InstallEvent.uninstall, Property = "DIR_PATH_PROPERTY_NAME" },
                              new File("readme.txt")));

        project.UI = WUI.WixUI_ProgressOnly;

        project.PreserveTempFiles = true;
        project.BuildMsi();
    }
}

public enum InstallEvent
{
    install,
    uninstall,
    both
}

/// <summary>
/// Good information about RemoveFolderEx can be found here:
/// <para>-  http://wixtoolset.org/documentation/manual/v3/xsd/util/removefolderex.html </para>
/// <para>-  https://www.hass.de/content/wix-how-use-removefolderex-your-xml-scripts </para>
/// </summary>
public class RemoveFolderEx : WixEntity, IGenericEntity
{
    [Xml]
    public InstallEvent? On;

    [Xml]
    public string Property;

    [Xml]
    public new string Id
    {
        get { return base.Id; }
        set { base.Id = value; }
    }

    /// <summary>
    /// The method demonstrates the correct way of integrating RemoveFolderEx.
    /// <para>
    /// The sample also shows various XML manipulation techniques available with Fluent XElement extensions:
    /// <para>- Auto XML serialization of CLR object with serializable members marked with XMLAttribute.</para>
    /// <para>- XML namespace-transparent lookup method FindSingle.</para>
    /// </para>
    /// </summary>
    /// <param name="context"></param>
    public void Process(ProcessingContext context)
    {
        context.Project.Include(WixExtension.Util); //indicate that candle needs to use WixUtilExtension.dll

        XElement element = this.ToXElement(WixExtension.Util.ToXName("RemoveFolderEx"));

        context.XParent
               .FindFirst("Component")
               .Add(element);
    }

    /// <summary>
    /// This method is for demo purposes only. It show an alternative implementation of the
    /// Process(ProcessingContext) with placing the new element inside of the component.
    /// <para>
    /// The sample also shows various XML manipulation techniques available with Fluent XElement extensions:
    /// <para>- AddElement method. Returns new added element.</para>
    /// <para>- SetAttribute method. Returns the element object, which the attribute has been set to.</para>
    /// </para>
    /// </summary>
    /// <param name="context"></param>
    public void PseudoProcessWithNewComponent(ProcessingContext context)
    {
        context.Project.Include(WixExtension.Util);

        XElement element = this.ToXElement(WixExtension.Util.ToXName("RemoveFolderEx"));

        context.XParent
               .AddElement("Component")
               .SetAttribute("Id", "TestComponent")
               .SetAttribute("Guid", Guid.NewGuid())
               .Add(element);
    }

    /// <summary>
    /// This method is for demo purposes only. It shows an alternative implementation of the
    /// Process(ProcessingContext) with placing the new element inside of the component and
    /// associates the component with the new feature 'Test Feature'.
    /// <para>
    /// The sample also shows various XML manipulation techniques available with Fluent XElement extensions:
    /// <para>- Lookup for parent with specified name.</para>
    /// <para>- Passing attribute definition string instead of creating attributes manually.</para>
    /// </para>
    /// </summary>
    /// <param name="context"></param>
    public void PseudoProcessWithNewComponentAndFeature(ProcessingContext context)
    {
        context.Project.Include(WixExtension.Util);

        XElement element = this.ToXElement(WixExtension.Util.ToXName("RemoveFolderEx"));

        context.XParent
               .AddElement("Component", "Id=TestComponent;Guid=" + Guid.NewGuid())
               .Add(element);

        context.XParent
               .Parent("Product")
               .AddElement("Feature", "Id=TestFeature;Title=Test Feature;Absent=allow;Level=1")
               .AddElement("ComponentRef", "Id=TestComponent");
    }
}