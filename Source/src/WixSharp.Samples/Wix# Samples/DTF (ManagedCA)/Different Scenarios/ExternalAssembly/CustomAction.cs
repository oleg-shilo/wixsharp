//css_ref ..\..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref DispalyMessage.dll;
using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

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
