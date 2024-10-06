//css_ref ..\..\..\..\Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
using System;
using System.Windows.Forms;
using WixToolset.Dtf.WindowsInstaller;

namespace CustomAction1
{
    public class CustomActionsTTT
    {
        [CustomAction]
        public static ActionResult CustomAction1(Session session)
        {
            MessageBox.Show("Hello World!", "Managed CA");
            return ActionResult.Success;
        }
    }
}