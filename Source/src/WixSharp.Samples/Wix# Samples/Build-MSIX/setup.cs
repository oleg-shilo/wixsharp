//css_dir ..\..\;
// //css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Install Files\bin\Debug\net472\WixToolset.Dtf.WindowsInstaller.dll
//css_ref D:\dev\Galos\wixsharp-wix4\Source\src\WixSharp.Samples\Wix# Samples\Install Files\bin\Debug\net472\WixToolset.Mba.Core.dll

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
    static public void Main()
    {
        // "msiexec".Run("/uninstall MyProduct.msi -");
        // return;
        var project =
            new ManagedProject("MyProduct",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(@"Files\Bin\MyApp.exe")
                        {
                            TargetFileName = "app.exe"
                        }));

        project.ManagedUI = ManagedUI.DefaultWpf; // all stock UI dialogs
        project.GUID = new Guid("6fe30b47-2577-43ad-9a95-1861ba25889b");

        var msi = @".\MyProduct.msi";
        project.UpdateTemplate(msi, @"D:\dev\wixsharp4\Source\src\WixSharp.Samples\Wix# Samples\Build-MSIX\MyProduct.msix.xml");
        return;
        // var msi = project.BuildMsi();

        // var msi = @".\MyProduct.msi";

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
    public static void UpdateTemplate(this Project project, string msi, string msixTemplate)
    {
        var doc = XDocument.Load(msixTemplate);
        doc.Save(msixTemplate);
    }

    public static void ConvertToMsix(this Project project, string msi, string msixTemplate)
    {
        var productCode = "{" + project.ProductId + "}";
    }

    public static void ConvertToMsix(this string msi, string msixTemplate)
    {
        using (var msiInfo = new MsiParser(msi))
        {
            var productCode = msiInfo.GetProductCode();

            if (MsiParser.IsInstalled(productCode))
                "msiexec".Run($"/x {productCode} /q");

            var startInfo = new ProcessStartInfo
            {
                FileName = "MsixPackagingTool.exe",
                Arguments = $@"create-package --template {msixTemplate} -v",
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
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (MsiParser.IsInstalled(productCode))
                    "msiexec".Run($"/x {productCode} /q");
            }
        }
    }
}