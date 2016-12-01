using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WixSharp.UI;

internal class ConsoleSetup
{
    [STAThread]
    static public void Main()
    {
        string msiFile = SetupDependencies();
        RunSetup(msiFile);
    }

    static void RunSetup(string msiFile)
    {
        var setup = new GenericSetup(msiFile, true);
        setup.ActionStarted += (s, e) => Console.WriteLine(setup.CurrentActionName);

        Console.WriteLine("The product is {0}INSTALLED\n\n", setup.IsCurrentlyInstalled ? "" : "NOT ");

        try
        {
            if (!setup.IsCurrentlyInstalled)
            {
                Console.WriteLine("Performing installation...\n");
                setup.ExecuteInstall(msiFile, "CUSTOM_UI=true");
            }
            else
            {
                Console.WriteLine("Performing uninstallation...\n");
                setup.ExecuteUninstall(msiFile);
            }

            Console.WriteLine("\nSetup is completed\n");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0};\nSee log file for details.\n", e.Message);
        }

        Process.Start(setup.LogFile);
    }

    static string SetupDependencies()
    {
        byte[] msiData = WixSharp.UI.Properties.Resources.MyProduct_msi;
        string msiFile = Path.Combine(Path.GetTempPath(), "MyProduct.msi");

        if (!File.Exists(msiFile) || new FileInfo(msiFile).Length != msiData.Length)
            File.WriteAllBytes(msiFile, msiData);

        AppDomain.CurrentDomain.AssemblyResolve +=
            (sender, args) => Assembly.Load(WixSharp.UI.Properties.Resources.WixSharp_Msi_dll);

        return msiFile;
    }
}