using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using static WixSharp.CommonTasks.AppSearch;
using WixToolset.Dtf.WindowsInstaller;
using io = System.IO;

public class Program
{
    static public void Main(string[] args)
    {
        if (Environment.GetEnvironmentVariable("IDE") == null)
        {
            BAHost.Run(args); // run as a bundle application
        }
        else
        {
            BuildScript.Run(args); // run as a bootstrapper build script
        }
    }
}

class BAHost
{
    [DllImport("kernel32.dll")]
    static extern bool FreeConsole();

    static public void Run(string[] args)
    {
        FreeConsole(); // closes the console window; alternatively compile this app as a windows application
        var application = new ManagedBA();
        WixToolset.BootstrapperApplicationApi.ManagedBootstrapperApplication.Run(application);
    }
}

public class BuildScript
{
    // This WixSharp sample uses a simple (Hello-World style) custom BA implemented in the same assembly as the bundle builder
    // You can also use System.Reflection.Assembly.GetExecutingAssembly().Location instead of "%this%"
    // NOTE: This sample demonstrates the new custom BA hosting model introduced in WiX5.
    // This model requires the BA to be a self-contained executable (not a DLL). In this case the BA is built into
    // the same executable as the bundle builder script. If you want to build the BA into a separate executable
    // have a look at the WixBootstrapper_UI_external sample.
    // ------
    // Note, passing BootstrapperCore.config is optional and if not provided the default BootstrapperCore.config
    // will be used. The content of the default BootstrapperCore.config can be accessed via
    // ManagedBootstrapperApplication.DefaultBootstrapperCoreConfigContent.
    //
    // Note, that the DefaultBootstrapperCoreConfigContent may not be suitable for all build and runtime scenarios.
    // In such cases you may need to use custom BootstrapperCore.config as demonstrated below.
    // bootstrapper.Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config");
    // ------
    // If you add any more packages, you may need to update `OnDetectPackageComplete` (i.g. to show 'install' button
    // even if one package is already installed).
    // ------
    // The UI implementation is based on the work of BRYANPJOHNSTON
    // http://bryanpjohnston.com/2012/09/28/custom-wix-managed-bootstrapper-application/

    static public void Run(string[] args)
    {
        if (WixTools.GlobalWixVersion.Major < 5)
        {
            Console.WriteLine("This sample requires WiX5 or higher.");
            return;
        }

        // if you want to use msi with custom managed UI then use BuildMsiWithManagedUI()
        // instead of BuildMsi(). You will also need to use MsiExePackage instead of MsiPackage.
        // See WixBootstrapper_MsiEmbeddedUI.csproj sample

        string productMsi = BuildMsi();
        // return;
        //string productMsi = BuildMsiWithManagedUI();

        var bundle = new Bundle("My Product Bundle",
                         new MsiPackage(productMsi)
                         {
                             Id = "MyProductPackageId",
                             Visible = true,
                             DisplayInternalUI = true
                         });

        bundle.Version = new Version("1.0.0.0");
        bundle.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bundle.Application = new ManagedBootstrapperApplication("%this%");

        bundle.DigitalSignature = new DigitalSignature
        {
            PfxFilePath = "D:\\dev\\wixsharp4\\Source\\src\\WixSharp.Samples\\Wix# Samples\\SigningBundle\\TempTestCert.pfx",
            Password = "password123",
            Description = "MyProductMsi",
        };

        bundle.OutDir = System.Reflection.Assembly.GetExecutingAssembly().Location.PathGetDirName();
        System.Diagnostics.Debugger.Launch();
        bundle.OutFileName = "my_setup";
        bundle.PreserveTempFiles = true;
        var ttt = bundle.Build();
        // bundle.Build("my_setup.exe");
    }

    static string BuildMsi()
    {
        var productProj =
            new ManagedProject("My Product1",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        productProj.UI = WUI.WixUI_Mondo;
        productProj.Scope = InstallScope.perMachine;
        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258772");

        productProj.Load += (SetupEventArgs e) =>
        {
            if (e.IsInstalling)
            {
                MessageBox.Show("Installing...", "My Product");
            }
            else if (e.IsUninstalling)
            {
                MessageBox.Show("Uninstalling...", "My Product");
            }
        };
        productProj.AfterInstall += (SetupEventArgs e) =>
        {
            MessageBox.Show("MSI session is completed", "My Product");
        };

        productProj.AddActions(new ElevatedManagedAction(CustomActions.MyAction, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed));
        // since adding custom actions will trigger dependency checking by MakeSfxCA and this app is referencing the
        // WixToolset.BootstrapperApplicationApi assembly, you will need to add it as a DefaultRefAssemblies even though
        // productProj dos not need it but only the bundle. This is a constrain of the current MakeSfxCA implementation.
        productProj.DefaultRefAssemblies.Add(typeof(WixToolset.BootstrapperApplicationApi.ManagedBootstrapperApplication).Assembly.Location);
        productProj.DefaultRefAssemblies.Add(typeof(CustomActionAttribute).Assembly.Location);

        productProj.SignAllFiles = true;
        Compiler.SignAllFilesOptions.SignEmbeddedAssemblies = true;

        productProj.DigitalSignature = new DigitalSignature
        {
            PfxFilePath = "D:\\dev\\wixsharp4\\Source\\src\\WixSharp.Samples\\Wix# Samples\\SigningBundle\\TempTestCert.pfx",
            Password = "password123",
            Description = "MyProductMsi",
        };

        // System.Diagnostics.Debugger.Launch();
        productProj.PreserveTempFiles = true;
        // productProj.OutDir = System.Reflection.Assembly.GetExecutingAssembly().Location.PathGetDirName();

        return productProj.BuildMsi();
    }

    static string BuildMsiWithManagedUI()
    {
        var productProj =
            new ManagedProject("My Product1",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        productProj.ManagedUI = ManagedUI.Default;
        productProj.Scope = InstallScope.perMachine;
        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258772");

        productProj.Load += (SetupEventArgs e) =>
        {
            if (e.IsInstalling)
            {
                MessageBox.Show("Installing...", "My Product");
            }
            else if (e.IsUninstalling)
            {
                MessageBox.Show("Uninstalling...", "My Product");
            }
        };
        productProj.AfterInstall += (SetupEventArgs e) =>
        {
            MessageBox.Show("MSI session is completed", "My Product");
        };

        return productProj.BuildMsi();
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        return session.HandleErrors(() => MessageBox.Show("Hello world. I am in a custom action"));
    }
}