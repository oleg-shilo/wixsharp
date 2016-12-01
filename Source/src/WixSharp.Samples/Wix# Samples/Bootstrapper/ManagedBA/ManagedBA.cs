using System;
using System.Threading;
using System.Linq;
using System.Windows;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Diagnostics;

namespace Bootstrapper
{

    //////////////////////////////////////
    //                                  //
    //                                  //
    //                                  //
    //                                  //
    //        WORK IN PROGRESS          //
    //                                  //
    //                                  //
    //                                  //
    //                                  //
    //////////////////////////////////////

    public class ManagedBA : BootstrapperApplication
    {
        AutoResetEvent done = new AutoResetEvent(false);

        // entry point for our custom UI
        protected override void Run()
        {
            //Debug.Assert(false);
            MessageBox.Show("Ta-da!");
            //ApplyComplete += ManagedBA_ApplyComplete;
            //DetectPackageComplete += ManagedBA_DetectPackageComplete;
            //PlanComplete += ManagedBA_PlanComplete;
            //Engine.Detect();
            //done.WaitOne();
            Engine.Quit(0);
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
        /// If the planning was successful, it instructs the Bootstrapper Engine to 
        /// install the packages.
        /// </summary>
        void ManagedBA_PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (e.Status >= 0)
                this.Engine.Apply(System.IntPtr.Zero);
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper DetectPackageComplete event is fired.
        /// Checks the PackageId and sets the installation scenario. The PackageId is the ID
        /// specified in one of the package elements (msipackage, exepackage, msppackage,
        /// msupackage) in the WiX bundle.
        /// </summary>
        void ManagedBA_DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId == "MyProduct")
            {
                if (e.State == PackageState.Absent)
                {
                    MessageBox.Show("Installing");
                    this.Engine.Plan(LaunchAction.Install);
                }
                else if (e.State == PackageState.Present)
                {
                    MessageBox.Show("UnInstalling");
                    this.Engine.Plan(LaunchAction.Uninstall);
                }
            }
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper ApplyComplete event is fired.
        /// This is called after a bundle installation has completed. Make sure we updated the view.
        /// </summary>
        void ManagedBA_ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            done.Set();
        }
    }

    
}
