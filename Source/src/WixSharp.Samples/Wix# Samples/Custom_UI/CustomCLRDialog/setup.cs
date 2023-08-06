using System.Diagnostics;
using WixSharp;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

public static class Script
{
    static public void Main()
    {
        // WIX4: THIS SAMPLE IS PORTED TO WIX4 BUT MAY OR MAY NOT WORK CORRECTLY

        // WixSharp support for manipulating native MSI dialogs has been implemented at early stage of WixSharp evolution.
        // Before the introduction of managed UI and specifically for WiX3.
        // While this sample compiles (and runs) under WiX4 there is no warranty that it will work the same way as in WiX3.
        //
        // The currently recommended WixSharp UI customization technique is to use Managed UI (WinForm or WPF). Thus this
        // is a legacy sample for demo purposes only.

        ProductActivationDialogSetup.Build();

        //MultiStepDialogSetup.Build();
        //EmptyDialogSetup.Build();
    }
}