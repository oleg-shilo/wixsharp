//css_ref ..\..\..\WixSharp.dll;
//css_ref ..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main(string[] args)
    {
        var project =
               new Project("My Product",
                   new Dir(@"%ProgramFiles%\MyCompany",
                       new Dir(@"MyWebApp",
                           new File(@"MyWebApp\Default.aspx"),
                           new File(@"MyWebApp\Default.aspx.cs"),
                           new File(@"MyWebApp\Default.aspx.designer.cs"),
                           new File(@"MyWebApp\Web.config"))));

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");

        project.IncludeWixExtension(WixExtension.IIs);
        project.WixSourceGenerated += Compiler_WixSourceGenerated;

        project.PreserveTempFiles = true;

        project.BuildMsi();
    }

    static void Compiler_WixSourceGenerated(System.Xml.Linq.XDocument document)
    {
        XElement aspxFileComponent = (from e in document.FindAll("File")
                                      where e.Attribute("Source").Value.EndsWith("Default.aspx")
                                      select e)
                                     .First()
                                     .Parent;

        string dirID = aspxFileComponent.Parent.Attribute("Id").Value;

        XNamespace ns = WixExtension.IIs.ToXNamespace();

        aspxFileComponent.Add(new XElement(ns + "WebVirtualDir",
                                  new XAttribute("Id", "MyWebApp"),
                                  new XAttribute("Alias", "MyWebApp"),
                                  new XAttribute("Directory", dirID),
                                  new XAttribute("WebSite", "DefaultWebSite"),
                                  new XElement(ns + "WebApplication",
                                      new XAttribute("Id", "TestWebApplication"),
                                      new XAttribute("Name", "Test"))));

        document.Root.Select("Product")
                     .Add(new XElement(ns + "WebSite",
                              new XAttribute("Id", "DefaultWebSite"),
                              new XAttribute("Description", "Default Web Site"),
                              new XAttribute("Directory", dirID),
                              new XElement(ns + "WebAddress",
                                   new XAttribute("Id", "AllUnassigned"),
                                    new XAttribute("Port", "80"))));
    }
}



