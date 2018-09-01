using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using WixSharp;
using WixSharp.CommonTasks;

public static class Script
{
    static public void Main()
    {
        ProductActivationDialogSetup.Build();
        //MultiStepDialogSetup.Build();  
        //EmptyDialogSetup.Build(); 
    }
}
