using System;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

public partial class EmptyDialog : WixCLRDialog
{

    public EmptyDialog()
    {
        InitializeComponent();
    }

    public EmptyDialog(Session session)
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
}
