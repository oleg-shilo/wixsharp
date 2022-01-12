using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using System.Diagnostics;

public partial class CustomDialog : WixCLRDialog
{
    public CustomDialog()
    {
        InitializeComponent();
    }

    public CustomDialog(Session session)
        : base(session)
    {
        InitializeComponent();
    }

    void backBtn_Click(object sender, EventArgs e)
    {
        MSIBack();

    }

    void nextBtn_Click(object sender, EventArgs e)
    {
        MSINext();
    }

    void cancelBtn_Click(object sender, EventArgs e)
    {
        MSICancel();
    }

    void button1_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Test from Custon CLR Dialog", "Wix#");
    }
}
