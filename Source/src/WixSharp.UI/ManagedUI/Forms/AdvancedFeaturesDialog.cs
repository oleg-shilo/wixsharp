using System;
using System.Linq;
#if WIX4
// using WixToolset.Bootstrapper;
#else
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Microsoft.Deployment.WindowsInstaller;
#endif

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