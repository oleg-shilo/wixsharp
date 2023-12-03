using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace WixBootstrapper_UI
{
    public partial class MainDialog : Form
    {
        MainViewModel viewModel;

        public MainDialog(BootstrapperApplication bootstrapper)
        {
            InitializeComponent();
            viewModel = new MainViewModel(bootstrapper);
        }

        void install_Click(object sender, EventArgs e)
        {
            viewModel.InstallExecute();
        }

        void uninstall_Click(object sender, EventArgs e)
        {
            viewModel.UninstallExecute();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}