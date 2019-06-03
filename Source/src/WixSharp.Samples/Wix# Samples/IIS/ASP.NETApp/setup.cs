//css_dir ..\..\..\;
//css_ref WixSharp.dll;
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
        var myWebSite = new WebSite("My Web Site", "*:81")
        {
            //to be created instead of existing one to be reused
            InstallWebSite = true
        };

        var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\MyCompany",
                        new Dir("MyWebApp",
                            new Dir("AdminWeb2",
                                    new IISVirtualDir
                                    {
                                        Name = "MyWebApp2",
                                        AppName = "Test2",
                                        WebSite = new WebSite("NewSite2", "*:8082") { InstallWebSite = true },
                                        WebAppPool = new WebAppPool("MyWebApp2", "Identity=applicationPoolIdentity")
                                    },
                                    new Files(@"MyWebApp\AdminWeb2\*.aspx")),

                                new Dir("AdminWeb3",
                                    new File(@"MyWebApp\AdminWeb3\Default.aspx",
                                        new IISVirtualDir
                                        {
                                            Name = "MyWebApp3",
                                            AppName = "Test3",
                                            WebSite = new WebSite("NewSite3", "*:8083") { InstallWebSite = true },
                                            WebAppPool = new WebAppPool("MyWebApp3", "Identity=applicationPoolIdentity")
                                        }
                                            )
                                       ),
                            new File(@"MyWebApp\Default.aspx",
                                new IISVirtualDir
                                {
                                    Alias = "MyWebApp",
                                    AppName = "Test",
                                    WebSite = myWebSite
                                },
                                new IISVirtualDir
                                {
                                    Alias = "MyWebApp1",
                                    AppName = "Test1",
                                    WebSite = myWebSite
                                }
                                    ),
                            new File(@"MyWebApp\Default.aspx.cs"),
                            new File(@"MyWebApp\Default.aspx.designer.cs"),
                            new File(@"MyWebApp\Web.config"))));

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;
        project.OutFileName = "setup";

        project.PreserveTempFiles = true;
        //uncomment the line below if you want to associate the web site with the app pool via WebApplicaion element
        //project.WixSourceGenerated += Project_WixSourceGenerated;
        project.BuildMsi();
    }

    static void Project_WixSourceGenerated(XDocument document)
    {
        var webSite = document.FindAll("WebSite")
                              .Where(x => x.HasAttribute("Description", "NewSite3"))
                              .First();
        var webApp = webSite.Parent.FindSingle("WebApplication");
        webApp.MoveTo(webSite);
    }
}