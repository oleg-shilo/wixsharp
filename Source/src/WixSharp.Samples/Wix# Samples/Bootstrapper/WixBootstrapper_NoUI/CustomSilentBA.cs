//using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

#if WIX4
using WixToolset.Bootstrapper;
#else

using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

#endif

[assembly: BootstrapperApplication(typeof(CustomSilentBA))]

public class CustomSilentBA : BootstrapperApplication
{
    protected override void Run()
    {
        //MessageBox.Show("CustomSilentBA just starting...");

        try
        {
            var done = new AutoResetEvent(false);

            DetectPackageComplete += (s, e) =>
            {
                //Debug.Assert(false);

                //Presence or absence of MyProductPackageId product will be a deciding factor
                //for initializing BA in Install, Uninstall or Modify mode.
                if (e.PackageId == "MyProductPackageId")
                {
                    if (e.State == PackageState.Absent)
                        this.Engine.Plan(LaunchAction.Install);
                    else if (e.State == PackageState.Present)
                        this.Engine.Plan(LaunchAction.Uninstall);
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