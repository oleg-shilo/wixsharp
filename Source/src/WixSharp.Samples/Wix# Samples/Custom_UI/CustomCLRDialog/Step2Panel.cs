using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WixToolset.Dtf.WindowsInstaller;

namespace ConsoleApplication1
{
    public partial class Step2Panel : Form
    {
        public Step2Panel()
        {
            InitializeComponent();
        }

        public Step2Panel(Session session)
        {
            InitializeComponent();
        }
    }
}