using System.Windows;
using System.Windows.Interop;

public partial class MainView : Window
{
    MainViewModel viewModel;

    public MainView(ManagedBA bootstrapper)
    {
        InitializeComponent();
        var vindowHandle = new WindowInteropHelper(this).EnsureHandle();

        DataContext =
        viewModel = new MainViewModel(bootstrapper) { ViewHandle = vindowHandle };
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