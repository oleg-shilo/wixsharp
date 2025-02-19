//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll
//css_ref Wix_bin\WixToolset.Mba.Core.dll
//css_ref WixSharp.Msi.dll

//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.UI;

static class Script
{
    static public void Main(string[] args)
    {
        var project =
            new ManagedProject("MyProduct",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(@"Files\Bin\MyApp.exe")
                        {
                            TargetFileName = "app.exe"
                        }));

        project.GUID = new Guid("6fe30b47-2577-43ad-9a95-1861ba25889b");

        var msi = project.BuildMsi();

        project.UpdateTemplate(@".\MyProduct.msix.xml", msi);

        if (WindowsIdentity.GetCurrent().IsAdmin())
        {
            msi.ConvertToMsix(@".\MyProduct.msix.xml");
        }
        else
        {
            Console.WriteLine("Error: you need run the build process as Administrator if you want to build the MSIX setup.");
        }
    }
}

static class Msix
{
    public static void UpdateTemplate(this Project project, string msixTemplate, string msi)
    {
        XNamespace ns = "http://schemas.microsoft.com/msix/msixpackagingtool/template/1910";

        var doc = XDocument.Load(msixTemplate);

        doc.Root.FindFirst("SaveLocation")
           .SetAttribute("PackagePath", msi.PathChangeExtension(".msix"))
           .SetAttribute("TemplatePath", msixTemplate.PathChangeExtension(".g.xml"));

        doc.Root.FindFirst("Installer")
           .SetAttribute("Path", msi)
           // the dir where msi will be installed so MsixPackagingTool can monitor it
           .SetAttribute("InstallLocation", @"C:\Program Files (x86)\My Company");

        doc.Root.FindFirst("PackageInformation")
           .SetAttribute("PackageName", project.Name)
           .SetAttribute("PackageDisplayName", project.Name)
           .SetAttribute(ns + "PackageDescription", project.Name)
           .SetAttribute("PublisherName", "CN=" + project.ControlPanelInfo.Manufacturer)
           .SetAttribute("PublisherDisplayName", project.ControlPanelInfo.Manufacturer)
           .SetAttribute("Version", project.Version);

        doc.Save(msixTemplate);
    }

    public static void ConvertToMsix(this Project project, string msi, string msixTemplate)
    {
        var productCode = "{" + project.ProductId + "}";
    }

    public static void ConvertToMsix(this string msi, string msixTemplate)
    {
        // Note MsixPackagingTool builds msix by installing msi and analyzing system changes and then embedding detected
        // changes (e.g. files) in the produced msix.
        // Thus it is important to clean up the system after the msi installation.

        using (var msiInfo = new MsiParser(msi))
        {
            var productCode = msiInfo.GetProductCode();

            if (MsiParser.IsInstalled(productCode))
                "msiexec".Run("/x " + productCode + " /q");

            var startInfo = new ProcessStartInfo
            {
                FileName = "MsixPackagingTool.exe",
                Arguments = @"create-package --template " + msixTemplate, //  use "-v" for more detailed build output
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    string line = null;
                    while (null != (line = process.StandardOutput.ReadLine()))
                        Console.WriteLine(line);

                    string error = process.StandardError.ReadToEnd();
                    if (!error.IsEmpty())
                        Console.WriteLine(error);
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message + ". Ensure you have installed MsixPackagingTool and MSIX driver.");
            }
            finally
            {
                if (MsiParser.IsInstalled(productCode))
                    "msiexec".Run("/x " + productCode + " /q");
            }
        }
    }
}