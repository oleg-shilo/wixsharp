//css_ref ..\..\..\..\Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref DispalyMessage.dll;
using System;
using System.Windows.Forms;
using WixToolset.Dtf.WindowsInstaller;

namespace CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CustomAction(Session session)
        {
            DispalyMessage.SayHello();
            return ActionResult.Success;
        }
    }
}
