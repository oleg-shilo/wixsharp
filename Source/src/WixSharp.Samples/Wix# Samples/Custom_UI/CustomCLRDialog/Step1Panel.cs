using System;
using System.Windows.Forms;
using WixToolset.Dtf.WindowsInstaller;

namespace ConsoleApplication1
{
    public partial class Step1Panel : Form
    {
        public Step1Panel()
        {
            InitializeComponent();
        }

        public Step1Panel(Session session)
        {
            InitializeComponent();
        }
    }
}