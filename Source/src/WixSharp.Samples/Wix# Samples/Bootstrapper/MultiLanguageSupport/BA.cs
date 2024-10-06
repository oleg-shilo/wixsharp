using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using WixSharp;
using WixToolset.Mba.Core;

[assembly: BootstrapperApplicationFactory(typeof(WixToolset.WixBA.WixBAFactory))]

namespace WixToolset.WixBA
{
    public class WixBAFactory : BaseBootstrapperApplicationFactory
    {
        protected override IBootstrapperApplication Create(IEngine engine, IBootstrapperCommand command)
        {
            // Debug.Assert(false);
            return new BA(engine, command);
        }
    }
}

public class BA : BootstrapperApplication
{
    public IEngine Engine => base.engine;
    public IBootstrapperCommand Command;

    public static string MainPackageId = "MyProductPackageId";

    public BA(IEngine engine, IBootstrapperCommand command) : base(engine)
    {
        this.Command = command;
        this.Error += (s, e) => MessageBox.Show(e.ErrorMessage);

        this.DetectBegin += (s, e) =>
            detectedRegistrationType = e.RegistrationType;

        this.PlanMsiPackage += (s, e) =>
        {
            if (e.PackageId == BA.MainPackageId)
                e.UiLevel = (e.Action == ActionState.Uninstall) ?
                                INSTALLUILEVEL.ProgressOnly :
                                INSTALLUILEVEL.Full;
        };

        this.ApplyComplete += (s, e) =>
            Engine.Quit(0);
    }

    RegistrationType detectedRegistrationType = RegistrationType.None;

    LaunchAction Detect()
    {
        var done = new AutoResetEvent(false);

        var launchAction = LaunchAction.Unknown;

        this.DetectPackageComplete += (object sender, DetectPackageCompleteEventArgs e) =>
        {
            if (e.PackageId == BA.MainPackageId)
            {
                if (e.Cached)
                {
                    if (detectedRegistrationType == RegistrationType.None)
                        launchAction = LaunchAction.Install;
                    else
                        launchAction = LaunchAction.Uninstall;
                }
                else
                {
                    if (e.State == PackageState.Absent)
                        launchAction = LaunchAction.Install;
                    else if (e.State == PackageState.Present)
                        launchAction = LaunchAction.Uninstall;
                }

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
            var view = new MainView();
            var result = view.ShowDialog();

            if (result == true)
            {
                bool defaultLanguage = view.SelectedLanguage.LCID == view.SupportedLanguages.FirstOrDefault()?.LCID;

                if (!defaultLanguage)
                    Engine.SetVariableString("TRANSFORMS", $":{view.SelectedLanguage.LCID}", false);

                Engine.Plan(launchAction);
                Engine.Apply(GetForegroundWindow());

                Dispatcher.CurrentDispatcher.VerifyAccess();
                Dispatcher.Run();
            }
        }
        else
        {
            // You can also show a small form with selection of the next action "Modify/Repair" vs "Uninstall"
            if (MessageBox.Show("Do you want to uninstall?", "My Product", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Engine.Plan(LaunchAction.Uninstall);
                Engine.Apply(GetForegroundWindow());

                Dispatcher.CurrentDispatcher.VerifyAccess();
                Dispatcher.Run();
            }
        }

        Engine.Quit(0);
    }

    [DllImport("User32.dll")]
    static extern IntPtr GetForegroundWindow();
}