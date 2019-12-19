using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

#if WIX4
using WixToolset.Bootstrapper;
#else

using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

#endif

[assembly: BootstrapperApplication(typeof(BA))]

public class BA : BootstrapperApplication
{
    public static string MainPackageId = "MyProductPackageId";
    public static string Languages = "en-US,de-DE,ru-RU";

    public CultureInfo SelectedLanguage { get; set; }

    public CultureInfo[] SupportedLanguages => Languages.Split(',')
                                                        .Select(x => new CultureInfo(x))
                                                        .ToArray();

    public BA()
    {
        SelectedLanguage = SupportedLanguages.FirstOrDefault();
        this.Error += (s, e) => MessageBox.Show(e.ErrorMessage);
        this.ApplyComplete += (s, e) =>
        {
            Engine.Quit(0);
        };
    }

    LaunchAction Detect()
    {
        var done = new AutoResetEvent(false);

        var launchAction = LaunchAction.Unknown;

        this.DetectPackageComplete += (object sender, DetectPackageCompleteEventArgs e) =>
        {
            if (e.PackageId == BA.MainPackageId)
            {
                if (e.State == PackageState.Absent)
                    launchAction = LaunchAction.Install;
                else if (e.State == PackageState.Present)
                    launchAction = LaunchAction.Uninstall;

                done.Set();
            }
        };

        this.Engine.Detect();

        done.WaitOne();

        return launchAction;
    }

    /// <summary>
    /// Entry point that is called when the bootstrapper application is ready to run.
    /// </summary>
    protected override void Run()
    {
        // Debug.Assert(false);

        var launchAction = this.Detect();

        if (launchAction == LaunchAction.Install)
        {
            var view = new MainView { DataContext = this };
            var result = view.ShowDialog();

            if (result == true)
            {
                bool defaultLanguage = SelectedLanguage.LCID == SupportedLanguages.FirstOrDefault()?.LCID;

                if (!defaultLanguage)
                    Engine.StringVariables["TRANSFORMS"] = $":{SelectedLanguage.LCID}";

                Engine.Plan(launchAction);
                Engine.Apply(new WindowInteropHelper(view).Handle);

                Dispatcher.CurrentDispatcher.VerifyAccess();
                Dispatcher.Run();
            }
        }
        else
        {
            // You can also show a small form with selection of the next action "Modify/Repair" vs "Uninstall"
            if (MessageBox.Show("Do you want to uninstall?", "My Product", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Engine.Plan(launchAction);
                Engine.Apply(IntPtr.Zero);

                Dispatcher.CurrentDispatcher.VerifyAccess();
                Dispatcher.Run();
            }
        }

        Engine.Quit(0);
    }
}