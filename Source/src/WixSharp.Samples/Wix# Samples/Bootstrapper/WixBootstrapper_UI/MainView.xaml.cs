using System.Windows;
using WixToolset.Bootstrapper;

public partial class MainView : Window
{
    MainViewModel viewModel;

    public MainView(BootstrapperApplication bootstrapper)
    {
        InitializeComponent();
        DataContext =
        viewModel = new MainViewModel(bootstrapper);
    }

    void Install_Click(object sender, RoutedEventArgs e)
    {
        viewModel.InstallExecute();
    }

    void Uninstall_Click(object sender, RoutedEventArgs e)
    {
        viewModel.UninstallExecute();
    }

    void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}