using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using System.Diagnostics;

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

    void button1_Click(object sender, EventArgs e)
    {
        ExternalAsm.Utils.Who();
    }

    protected override IntPtr GetMsiForegroundWindow()
    {
        var window = Process.GetProcessesByName("msiexec")
                            .Where(p => p.MainWindowHandle != IntPtr.Zero)
                            .Select(p => p.MainWindowHandle)
                            .FirstOrDefault();

        if (window != default(IntPtr))
            return window;
        else
            return base.GetMsiForegroundWindow();
    }
}
