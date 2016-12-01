using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WixSharp.UI
{
    class MyProductSetup : GenericSetup
    {
        public void StartInstall()
        {
            StartInstall("CUSTOM_UI=true");
        }

        public void StartRepair()
        {
            StartRepair("CUSTOM_UI=true");
        }

        public void StartUnInstall()
        {
            StartUninstall("CUSTOM_UI=true");
        }

        public MyProductSetup(string msiFile, bool enableLoging = true)
            : base(msiFile, enableLoging)
        {
        }
    }
}
