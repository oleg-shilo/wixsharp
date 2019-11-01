//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;
using IO = System.IO;

class Script
{
    static public void Main()
    {
        try
        {
            var project =
                new ManagedProject("My Product",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(@"Files\MyApp.exe"),
                        new File(@"Files\MyApp.exe.config",
                            new XmlFile(XmlFileAction.setValue, @"//configuration/appSettings/add[\[]@key='AppName'[\]]/@value", "My App"),
                            new XmlFile(XmlFileAction.setValue, @"//configuration/connectionStrings/add[\[]@name='Server1'[\]]/@connectionString", "DataSource=(localdb)/v11.0;IntegratedSecurity=true"),
                            new XmlFile(XmlFileAction.setValue, @"//configuration/connectionStrings/add[\[]@name='Server1'[\]]/@providerName", "System.Data.SqlClient"))),
                    new ElevatedManagedAction(CustomActions.OnInstall, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed)
                    {
                        UsesProperties = "CONFIG_FILE=[INSTALLDIR]MyApp.exe.config, APP_FILE=[INSTALLDIR]MyApp.exe, DATABASE_CONNECTION_STRING=[DATABASE_CONNECTION_STRING]"
                    });

            project.GUID = new Guid("6fe30b47-2577-43ad-9195-1861ba25889b");
            project.AfterInstall += Project_AfterInstall;
            Compiler.PreserveTempFiles = true;
            Compiler.BuildMsi(project);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void Project_AfterInstall(SetupEventArgs e)
    {
        //Project_AfterInstall is arguably is the most convenient method for editing config file.
        //This event handler is nothing else but a dedicated elevated (deferred) ManagedAction
        if (e.IsInstalling)
        {
            //checking if a specific feature was installed. MSI way is to write the name of the feature to the ADDLOCAL property.
            //ADDLOCAL must be accessed via 'Property' method as otherwise (e.Session[<prop_name>]) the value is not available
            bool isFeatureInstalled = e.Session.Property("ADDLOCAL").Split(',').Contains("<name of the feature>");

            string configFile = IO.Path.Combine(e.InstallDir, "MyApp.exe.config");

            // disabled for demo purposes
            // CustomActions.UpdateAsAppConfig(configFile);
            // CustomActions.UpdateAsXml(configFile);
            // CustomActions.UpdateAsText(configFile);
            // CustomActions.UpdateWithWixSharp(configFile);
        }
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult OnInstall(Session session)
    {
        //Note if your custom action requires non-GAC assembly then you need deploy it too.
        //You can do it by setting ManagedAction.RefAssemblies.
        //See "Wix# Samples\DTF (ManagedCA)\Different Scenarios\ExternalAssembly" sample for details.

        System.Diagnostics.Debugger.Launch();

        session.Log("------------- " + session.Property("INSTALLDIR"));
        session.Log("------------- " + session.Property("CONFIG_FILE"));
        session.Log("------------- " + session.Property("APP_FILE"));

        return session.HandleErrors(() =>
        {
            string configFile = session.Property("INSTALLDIR") + "MyApp.exe.config";

            //alternative ways of extracting 'deferred' properties
            //configFile = session.Property("APP_FILE") + ".config";
            //configFile = session.Property("CONFIG_FILE");

            UpdateAsAppConfig(configFile);

            //alternative implementations for the config manipulations
            UpdateAsXml(configFile);
            UpdateAsText(configFile);
            UpdateWithWixSharp(configFile);

            MessageBox.Show(GetContext());
        });
    }

    static string GetContext()
    {
        if (WindowsIdentity.GetCurrent().IsAdmin())
            return "Admin User";
        else
            return "Normal User";
    }

    static public void UpdateAsAppConfig(string configFile)
    {
        var config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = configFile }, ConfigurationUserLevel.None);

        config.AppSettings.Settings["AppName"].Value = "My App";

        var section = config.ConnectionStrings;
        section.ConnectionStrings["Server1"].ConnectionString = "DataSource=(localdb)/v11.0;IntegratedSecurity=true";
        section.ConnectionStrings["Server1"].ProviderName = "System.Data.SqlClient";

        config.Save();
    }

    static public void UpdateAsXml2(string configFile, bool installing = true)
    {
        var config = XDocument.Load(configFile);

        if (installing)
            config.Select("configuration")
                  .AddElement("userSettings")
                  .AddElement("MyApp.Properties.Settings")
                  .AddElement("setting", "name=InputPath;serializeAs=String")
                  .AddElement("value", null, @"C:\Input");
        else
            config.Select("configuration/userSettings/MyApp.Properties.Settings")
                  .Parent.Remove();
        // ?.Parent.Remove(); // in C#7

        config.Save(configFile);
    }

    static public void UpdateAsXml(string configFile)
    {
        var config = XDocument.Load(configFile);

        config.XPathSelectElement("//configuration/appSettings/add[@key='AppName']").Attribute("value").Value = "My App";
        config.XPathSelectElement("//configuration/connectionStrings/add[@name='Server1']").Attribute("connectionString").Value = "DataSource=(localdb)/v11.0;IntegratedSecurity=true";
        config.XPathSelectElement("//configuration/connectionStrings/add[@name='Server1']").Attribute("providerName").Value = "System.Data.SqlClient";

        config.Save(configFile);
    }

    static public void UpdateAsText(string configFile)
    {
        string configuration = IO.File.ReadAllText(configFile);

        configuration = configuration.Replace("{$AppName}", "My App")
                                     .Replace("{$ConnectionString}", "DataSource=(localdb)/v11.0;IntegratedSecurity=true")
                                     .Replace("{$ProviderName}", "System.Data.SqlClient");

        IO.File.WriteAllText(configFile, configuration);
    }

    static public void UpdateWithWixSharp(string configFile)
    {
        Tasks.SetConfigAttribute(configFile, "//configuration/appSettings/add[@key='AppName']/@value", "My App");
        Tasks.SetConfigAttribute(configFile, "//configuration/connectionStrings/add[@name='Server1']/@connectionString", "DataSource=(localdb)/v11.0;IntegratedSecurity=true");
        Tasks.SetConfigAttribute(configFile, "//configuration/connectionStrings/add[@name='Server1']/@providerName", "System.Data.SqlClient");
    }
}