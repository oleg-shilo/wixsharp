using System.Diagnostics;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

public static class Script
{
    static public void Main()
    {
        // THIS SAMPOLE IS NOT PORTED TO WIX4 yet
        Compiler.WixLocation = @"..\..\..\..\..\Wix_bin\bin";

        ProductActivationDialogSetup.Build();
        //MultiStepDialogSetup.Build();
        //EmptyDialogSetup.Build();
    }
}