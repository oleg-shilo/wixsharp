using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace WpfSetup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Window splashscreen;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            splashscreen = new SplashScreen();
            splashscreen.Show();
            App.DoEvents();

            byte[] msiData = WpfSetup.Properties.Resources.MyProduct_msi;
            MsiFile = Path.Combine(Path.GetTempPath(), "MyProduct.msi");

            if (!File.Exists(MsiFile) || new FileInfo(MsiFile).Length != msiData.Length)
                File.WriteAllBytes(MsiFile, msiData);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.Load(WpfSetup.Properties.Resources.WixSharp_Msi_dll);
        }

        public static void HideSplashScreen()
        {
            splashscreen.Close();
        }

        static public string MsiFile { get; set; }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { })); 
        }

    }
}