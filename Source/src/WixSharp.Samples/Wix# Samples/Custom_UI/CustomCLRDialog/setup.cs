using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using WixSharp;
using WixSharp.CommonTasks;

public static class Script
{
    static public void Main()
    {
        Compiler.WixLocation = @"..\..\..\..\..\Wix_bin\bin";

        ProductActivationDialogSetup.Build();
        //MultiStepDialogSetup.Build();
        //EmptyDialogSetup.Build();
    }
}