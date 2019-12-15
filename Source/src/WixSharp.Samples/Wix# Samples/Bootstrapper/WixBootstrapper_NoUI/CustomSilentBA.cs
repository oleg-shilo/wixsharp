//using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using WixSharp;

#if WIX4
using WixToolset.Bootstrapper;
#else

using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

#endif

[assembly: BootstrapperApplication(typeof(CustomSilentBA))]

public class CustomSilentBA : BootstrapperApplication
{
    string DowngradeWarningMessage = "A later version of the package (PackageId: {0}) is already installed. Setup will now exit.";

    protected override void Run()
    {
        // MessageBox.Show("CustomSilentBA just starting...");

        try
        {
            var done = new AutoResetEvent(false);

            DetectPackageComplete += (s, e) =>
            {
                //Presence or absence of MyProductPackageId product will be a deciding factor
                //for initializing BA in Install, Uninstall or Modify mode.
                if (e.PackageId == "")
                {
                    switch (e.State)
                    {
                        case PackageState.Obsolete:
                            this.Engine.Log(LogLevel.Error, string.Format(DowngradeWarningMessage, e.PackageId));
                            MessageBox.Show(string.Format(DowngradeWarningMessage, e.PackageId), this.Engine.StringVariables["WixBundleName"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    this.Engine.Apply(IntPtr.Zero);
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
        Engine.Quit(0);
    }
}