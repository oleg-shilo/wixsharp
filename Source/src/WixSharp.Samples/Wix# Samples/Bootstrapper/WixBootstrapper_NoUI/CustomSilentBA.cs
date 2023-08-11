//using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WixSharp;
using WixToolset.Mba.Core;

[assembly: BootstrapperApplicationFactory(typeof(WixToolset.WixBA.WixBAFactory))]

namespace WixToolset.WixBA
{
    public class WixBAFactory : BaseBootstrapperApplicationFactory
    {
        protected override IBootstrapperApplication Create(IEngine engine, IBootstrapperCommand command)
        {
            MessageBox.Show("CustomSilentBA");
            return new CustomSilentBA(engine, command);
        }
    }
}

public class CustomSilentBA : WixToolset.Mba.Core.BootstrapperApplication
{
    string DowngradeWarningMessage = "A later version of the package (PackageId: {0}) is already installed. Setup will now exit.";

    public CustomSilentBA(IEngine engine, IBootstrapperCommand command) : base(engine)
    {
        this.DetectBegin += OnDetectBegin;
        this.PlanMsiPackage += (object sender, PlanMsiPackageEventArgs e) =>
        {
            if (e.PackageId == "MyProductPackageId")
                e.UiLevel = e.Action == ActionState.Uninstall ?
                                INSTALLUILEVEL.ProgressOnly :
                                INSTALLUILEVEL.Full;
        };
        this.Command = command;
    }

    public IEngine Engine { get { return base.engine; } }
    public IBootstrapperCommand Command;
    RegistrationType detecteRegistrationType = RegistrationType.None;

    void OnDetectBegin(object sender, DetectBeginEventArgs e)
    {
        detecteRegistrationType = e.RegistrationType;
    }

    protected override void Run()
    {
        MessageBox.Show("Starting...", "CustomSilentBA");

        try
        {
            var done = new AutoResetEvent(false);

            DetectPackageComplete += (s, e) =>
            {
                //Presence or absence of MyProductPackageId product will be a deciding factor
                //for initializing BA in Install, Uninstall or Modify mode.
                if (e.PackageId == "MyProductPackageId")
                {
                    if (e.Cached)
                    {
                        if (detecteRegistrationType == RegistrationType.None)
                            this.Engine.Plan(LaunchAction.Install);
                        else
                            this.Engine.Plan(LaunchAction.Uninstall);
                    }
                    else
                    {
                        switch (e.State)
                        {
                            case PackageState.Obsolete:
                                this.Engine.Log(LogLevel.Error, string.Format(DowngradeWarningMessage, e.PackageId));
                                MessageBox.Show(string.Format(DowngradeWarningMessage, e.PackageId), this.Engine.GetVariableString("WixBundleName"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                Engine.Quit(0);
                                break;

                            case PackageState.Absent:
                                this.Engine.Plan(LaunchAction.Install);
                                break;

                            case PackageState.Present:
                                this.Engine.Plan(LaunchAction.Uninstall);
                                break;
                        }
                    }
                }
            };

            DetectComplete += (s, e) =>
            {
                if (this.Command.Action == LaunchAction.Uninstall)
                {
                    // needed for handling update scenarios
                    Engine.Log(LogLevel.Verbose, "Invoking automatic plan for uninstall");
                    Engine.Plan(LaunchAction.Uninstall);
                }
            };

            PlanComplete += (s, e) =>
            {
                if (e.Status >= 0)
                    this.Engine.Apply(GetForegroundWindow()); // IntPtr.Zero is no longer valid value in WiX4
            };

            ApplyComplete += (s, e) =>
            {
                done.Set();
            };

            Engine.Detect();

            done.WaitOne();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
        }
        MessageBox.Show("Done...", "CustomSilentBA");
        Engine.Quit(0);
    }

    [DllImport("User32.dll")]
    static extern IntPtr GetForegroundWindow();
}