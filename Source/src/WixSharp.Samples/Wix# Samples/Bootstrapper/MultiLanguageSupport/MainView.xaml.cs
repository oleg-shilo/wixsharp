using System.Globalization;
using System.Linq;
using System.Windows;

public partial class MainView : Window
{
    public static string Languages = "en-US,de-DE,uk-UA";

    public CultureInfo SelectedLanguage { get; set; }

    public CultureInfo[] SupportedLanguages => Languages.Split(',')
                                                        .Select(x => new CultureInfo(x))
                                                        .ToArray();

    public MainView()
    {
        SelectedLanguage = SupportedLanguages.FirstOrDefault();

        InitializeComponent();
        this.DataContext = this;
    }

    void Install_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true;
        Close();
    }
}