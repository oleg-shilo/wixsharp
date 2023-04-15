// using WixToolset.Mba.Core;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using WixToolset.Mba.Core;

using mba = WixToolset.Mba.Core;

[assembly: WixToolset.Mba.Core.BootstrapperApplicationFactory(typeof(WixToolset.WixBA.WixBAFactory))]

namespace WixToolset.WixBA
{
    public class WixBAFactory : BaseBootstrapperApplicationFactory
    {
        protected override mba.IBootstrapperApplication Create(mba.IEngine engine, mba.IBootstrapperCommand command)
        {
            MessageBox.Show("TETETETET");
            return new ManagedBA(engine, command);
        }
    }
}

public class ManagedBA : mba.BootstrapperApplication
{
    public ManagedBA(mba.IEngine engine, mba.IBootstrapperCommand command) : base(engine)
    {
        this.Command = command;
    }

    public mba.IEngine Engine => base.engine;
    public mba.IBootstrapperCommand Command;

    /// <summary>
    /// Entry point that is called when the bootstrapper application is ready to run.
    /// </summary>
    protected override void Run()
    {
        new MainView(this).ShowDialog();
        engine.Quit(0);
    }
}

public class MainViewModel : INotifyPropertyChanged
{
    protected void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public MainViewModel(ManagedBA bootstrapper)

    {
        this.IsBusy = false;

        this.Bootstrapper = bootstrapper;
        this.Bootstrapper.Error += this.OnError;
        this.Bootstrapper.ApplyComplete += this.OnApplyComplete;
        this.Bootstrapper.DetectPackageComplete += this.OnDetectPackageComplete;
        this.Bootstrapper.PlanComplete += this.OnPlanComplete;

        this.Bootstrapper.Engine.Detect();
    }

    void OnError(object sender, mba.ErrorEventArgs e)
    {
        MessageBox.Show(e.ErrorMessage);
    }

    bool installEnabled;

    public bool InstallEnabled
    {
        get { return installEnabled; }
        set
        {
            installEnabled = value;
            OnPropertyChanged("InstallEnabled");
        }
    }

    string userInput = "User input content...";

    public string UserInput
    {
        get => userInput;

        set
        {
            userInput = value;
            OnPropertyChanged("UserInput");
        }
    }

    bool uninstallEnabled;

    public bool UninstallEnabled
    {
        get { return uninstallEnabled; }
        set
        {
            uninstallEnabled = value;
            OnPropertyChanged("UninstallEnabled");
        }
    }

    bool isThinking;

    public bool IsBusy
    {
        get { return isThinking; }
        set
        {
            isThinking = value;
            OnPropertyChanged("IsBusy");
        }
    }

    public ManagedBA Bootstrapper { get; set; }

    public void InstallExecute()
    {
        IsBusy = true;

        Bootstrapper.Engine.SetVariableString("UserInput", UserInput, false);
        Bootstrapper.Engine.Plan(mba.LaunchAction.Install);
    }

    public void UninstallExecute()
    {
        IsBusy = true;
        Bootstrapper.Engine.Plan(mba.LaunchAction.Uninstall);
    }

    public void ExitExecute()
    {
        //Dispatcher.BootstrapperDispatcher.InvokeShutdown();
    }

    /// <summary>
    /// Method that gets invoked when the Bootstrapper ApplyComplete event is fired.
    /// This is called after a bundle installation has completed. Make sure we updated the view.
    /// </summary>
    void OnApplyComplete(object sender, mba.ApplyCompleteEventArgs e)
    {
        IsBusy = false;
        InstallEnabled = false;
        UninstallEnabled = false;
    }

    /// <summary>
    /// Method that gets invoked when the Bootstrapper DetectPackageComplete event is fired.
    /// Checks the PackageId and sets the installation scenario. The PackageId is the ID
    /// specified in one of the package elements (msipackage, exepackage, msppackage,
    /// msupackage) in the WiX bundle.
    /// </summary>
    void OnDetectPackageComplete(object sender, mba.DetectPackageCompleteEventArgs e)
    {
        if (e.PackageId == "MyProductPackageId")
        {
            if (e.State == mba.PackageState.Absent)
                InstallEnabled = true;
            else if (e.State == mba.PackageState.Present)
                UninstallEnabled = true;
        }
    }

    /// <summary>
    /// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
    /// If the planning was successful, it instructs the Bootstrapper Engine to
    /// install the packages.
    /// </summary>
    void OnPlanComplete(object sender, mba.PlanCompleteEventArgs e)
    {
        if (e.Status >= 0)
            Bootstrapper.Engine.Apply(System.IntPtr.Zero);
    }
}