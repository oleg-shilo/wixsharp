using System;
using System.IO;
using System.Reflection;
using WixSharp.UI;

internal class Script
{
    [STAThread]
    static public void Main()
    {
        // This is nothing else but an equivalent of a bootstrapper for a single msi file.
        string msiFile = SetupDependencies();
        RunSetup(msiFile);
    }

    static void RunSetup(string msiFile)
    {
        new MsiSetupForm(msiFile).ShowDialog();
    }

    static string SetupDependencies()
    {
        string msiFile;

        byte[] msiData = WixSharp.UI.Properties.Resources.MyProduct_msi;
        msiFile = Path.Combine(Path.GetTempPath(), "MyProduct.msi");

        if (!File.Exists(msiFile) || new FileInfo(msiFile).Length != msiData.Length)
            File.WriteAllBytes(msiFile, msiData);

        AppDomain.CurrentDomain.AssemblyResolve +=
            (sender, args) => Assembly.Load(WixSharp.UI.Properties.Resources.WixSharp_Msi_dll);

        return msiFile;
    }
}