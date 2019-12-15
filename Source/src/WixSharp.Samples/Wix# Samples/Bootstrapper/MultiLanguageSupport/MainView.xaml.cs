using System.Windows;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    void Install_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        Close();
    }
}