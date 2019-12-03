using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

#if WIX4
using WixToolset.Bootstrapper;
#else

using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

#endif

[assembly: BootstrapperApplication(typeof(ManagedBA))]

public class ManagedBA : BootstrapperApplication
{
    /// <summary>
    /// Entry point that is called when the bootstrapper application is ready to run.
    /// </summary>
    protected override void Run()
    {
        new MainView(this).ShowDialog();
        Engine.Quit(0);
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

    public MainViewModel(BootstrapperApplication bootstrapper)

    {
        this.IsBusy = false;

        this.Bootstrapper = bootstrapper;
        this.Bootstrapper.Error += this.OnError;
        this.Bootstrapper.ApplyComplete += this.OnApplyComplete;
        this.Bootstrapper.DetectPackageComplete += this.OnDetectPackageComplete;
        this.Bootstrapper.PlanComplete += this.OnPlanComplete;

        this.Bootstrapper.Engine.Detect();
    }

    void OnError(object sender, ErrorEventArgs e)
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

    public BootstrapperApplication Bootstrapper { get; set; }

    public void InstallExecute()
    {
        IsBusy = true;

        Bootstrapper.Engine.StringVariables["UserInput"] = UserInput;
        Bootstrapper.Engine.Plan(LaunchAction.Install);
    }

    public void UninstallExecute()
    {
        IsBusy = true;
        Bootstrapper.Engine.Plan(LaunchAction.Uninstall);
    }

    public void ExitExecute()
    {
        //Dispatcher.BootstrapperDispatcher.InvokeShutdown();
    }

    /// <summary>
    /// Method that gets invoked when the Bootstrapper ApplyComplete event is fired.
    /// This is called after a bundle installation has completed. Make sure we updated the view.
    /// </summary>
    void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
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
    void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
    {
        if (e.PackageId == "MyProductPackageId")
        {
            if (e.State == PackageState.Absent)
                InstallEnabled = true;
            else if (e.State == PackageState.Present)
                UninstallEnabled = true;
        }
    }

    /// <summary>
    /// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
    /// If the planning was successful, it instructs the Bootstrapper Engine to
    /// install the packages.
    /// </summary>
    void OnPlanComplete(object sender, PlanCompleteEventArgs e)
    {
        if (e.Status >= 0)
            Bootstrapper.Engine.Apply(System.IntPtr.Zero);
    }
}