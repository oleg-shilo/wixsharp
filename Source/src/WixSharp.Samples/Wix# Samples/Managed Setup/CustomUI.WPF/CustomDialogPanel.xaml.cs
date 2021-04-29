using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI.WPF;

namespace ConsoleApplication1
{
    /// <summary>
    /// Interaction logic for CustomDialogPanel.xaml
    /// </summary>
    public partial class CustomDialogPanel : UserControl, IWpfDialogContent
    {
        public CustomDialogPanel()
        {
            InitializeComponent();
        }

        public void Init(CustomDialogBase parent)
        {
            var model = new CustomDialogPanelModel { ParentDialog = parent };
            ViewModelBinder.Bind(model, /*view*/this, null);

            // insert Validate button on the left from the "Back" button
            var validateButton = new Button
            {
                Content = "Validate",
                MinWidth = 73
            };
            validateButton.Click += (s, e) => model.Validate();

            parent.ButtonsPanel.Children.Insert(0, new Separator { Opacity = 0, Width = 30 });
            parent.ButtonsPanel.Children.Insert(0, validateButton);
        }
    }

    public class CustomDialogPanelModel : Caliburn.Micro.Screen
    {
        public ISession Session => ParentDialog?.ManagedFormHost.Runtime.Session;
        public ManagedForm Host => ParentDialog?.ManagedFormHost;
        public CustomDialogBase ParentDialog { get; set; }

        public string User => Environment.UserName;

        bool canProceed;

        public bool CanProceedIsChecked
        {
            get { return canProceed; }
            set
            {
                canProceed = value;
                NotifyOfPropertyChange(() => CanProceedIsChecked);

                if (ParentDialog != null)
                    ParentDialog.GoNextButton.IsEnabled = value;
            }
        }

        public void Validate()
        {
            MessageBox.Show("Performing validation...");
        }

        public void ShowReadme()
        {
            Process.Start("https://github.com/oleg-shilo/wixsharp");
        }
    }
}