//css_ref ..\..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;

using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using System.Threading;

class Script
{
    static public void Main(string[] args)
    {
        var project = new Project()
        {
            UI = WUI.WixUI_ProgressOnly,
            Name = "CustomActionTest",

            Actions = new[]
            {
                new ManagedAction(CustonActions.MyAction, "%this%")
            }
        };

        Compiler.BuildMsi(project);
    }
}

public class CustonActions
{
[CustomAction]
public static ActionResult MyAction(Session session)
{
    MessageBox.Show(Thread.CurrentThread.GetApartmentState().ToString(), "Original Thread ApartmentState");

    var actionThread = new Thread((ThreadStart)
        delegate
        {
            MessageBox.Show(Thread.CurrentThread.GetApartmentState().ToString(), "New Thread ApartmentState");
            using(var dialog =  new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                DialogResult dlgResult = dialog.ShowDialog();
            }
        });
    
    actionThread.SetApartmentState(ApartmentState.STA);
    actionThread.Start();
    actionThread.Join();
    
    return ActionResult.Success;
}
}


