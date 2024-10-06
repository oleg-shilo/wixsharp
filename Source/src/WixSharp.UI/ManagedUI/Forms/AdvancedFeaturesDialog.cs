using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WixToolset.Dtf.WindowsInstaller;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Features dialog.
    /// </summary>
    public partial class AdvancedFeaturesDialog : FeaturesDialog, IManagedDialog
    {
        //At this stage it is a full equivalent of FeaturesDialog
        //Though in the future it can be extended to match the default MSI FeaturesDialog
        //(context menu and icon instead of checkbox)
    }
}