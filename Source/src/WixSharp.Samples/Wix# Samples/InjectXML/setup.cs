//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main()
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles64Folder%\My Company\My Product",
                    new EventSourceEx
                    {
                        Name = "ROOT Builder",
                        Log = "Application",
                        EventMessageFile = @"%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\EventLogMessages.dll"
                    },
                    new File("myapp_exe".ToId(), @"Files\Bin\MyApp.exe") { AttributesDefinition = "Component:Id=Component.myapp_exe" },
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        // Note: setting x64 is done via XML injection for demo purposes only.
        // The x64 install can be achieved by "project.Platform = Platform.x64;"

        // AddXmlInclude can also be applied to any WixShari entity (e.g. new File("...").AddXmlInclude("FileCommonProperies.wxi")

        project.Include(WixExtension.Util)
               .AddXmlInclude("CommonProperies.wxi")
               .AddXmlInclude("CommonProperies2.wxi");

        project.AddWixFragment("Wix/Product", XElement.Parse(@"
                        <Feature Id=""BinaryOnlyFeature"" Title=""Sample Product Feature"" Level=""1"">
                            <ComponentRef Id=""Component.myapp_exe"" />
                        </Feature>"));

        // project specific build event
        project.WixSourceGenerated += InjectImages;

        project.AddXml("Wix/Product", "<Property Id=\"Title\" Value=\"Properties Test\" />");

        project.AddXmlElement("Wix/Product", "Property", "Id=Gritting; Value=Hello World!");

        project.Media.Clear(); // clear default media as we will add it via MediaTemplate
        project.WixSourceGenerated += document =>
        {
            document.Root.Select("Product")
                         .AddElement("MediaTemplate", "CabinetTemplate=cab{0}.cab; CompressionLevel=mszip");
        };

        // global build event
        Compiler.WixSourceGenerated += document =>
            {
                document.Root.Select("Product/Package")
                             .SetAttributeValue("Platform", "x64");

                document.FindAll("Component")
                        .ForEach(e => e.SetAttributeValue("Win64", "yes"));

                // merge 'Wix/Product' elements of document with 'Wix/Product' elements of CommonProperies.wxs
                document.InjectWxs("CommonProperies.wxs");

                // the commented code below is the equivalent of project.AddXmlInclude(...)
                // document.FindSingle("Product").Add(new XProcessingInstruction("include", "CommonProperies.wxi"));
            };

        project.PreserveTempFiles = true;
        project.BuildMsi();
    }

    private static void InjectImages(System.Xml.Linq.XDocument document)
    {
        var productElement = document.Root.Select("Product");

        productElement.Add(new XElement("WixVariable",
                               new XAttribute("Id", "WixUIBannerBmp"),
                               new XAttribute("Value", @"Images\bannrbmp.bmp")));
        // alternative syntax
        productElement.AddElement("WixVariable", @"Id=WixUIDialogBmp;Value=Images\dlgbmp.bmp");
    }
}

/// <summary>
/// Very lean (ad hock) implementation of EventSource element (http://wixtoolset.org/documentation/manual/v3/xsd/util/eventsource.html)
/// Wix# already includes its own implementation of EvnetSource and this example only provided as a demo for adding support for the
/// WiX elements, which are not supported natively by Wix#.
/// </summary>
public class EventSourceEx : WixEntity, IGenericEntity
{
    [WixSharp.Xml]
    new public string Name;

    [WixSharp.Xml]
    public string Log;

    [WixSharp.Xml]
    public string EventMessageFile;

    public void Process(ProcessingContext context)
    {
        // reflect new dependency
        context.Project.Include(WixExtension.Util);

        this.CreateAndInsertParentComponent(context)
            .Add(this.ToXElement(WixExtension.Util, "EventSource"));
    }
}