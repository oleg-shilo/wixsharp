//css_inc MsiInterop.cs
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowsInstaller;
using WixSharp.UI;

//http://msdn.microsoft.com/en-us/library/windows/desktop/aa370573(v=vs.85).aspx
//http://www.codeproject.com/Articles/5773/Wrapping-the-Windows-Installer-2-0-API

internal class Script
{
    [STAThread]
    static public void Main()
    {
        //just a sample
        string msiFile = @"..\Projects\WixSharp\Main\WixSharp.Samples\Wix# Samples\_CustomDialog\External_CLR_GUI\MsiInterop\MyProduct.msi";

        string msiParams = ""; //install
        msiParams = "REMOVE=ALL"; //uninstall

        var msi = new MsiParser(msiFile);
        var productCode = msi.GetProductCode();
        bool installed = msi.IsInstalled();

        //IntPtr product;
        //MsiExtensions.Invoke(() => MsiInterop.MsiOpenProduct(productCode, out product));

        Console.WriteLine("The product is {0}INSTALLED\n\n", installed ? "" : "NOT ");
        if (installed)
            msiParams = "REMOVE=ALL"; //uninstall
        else
            msiParams = ""; //install

        var session = new MsiSession();
        session.Execute(msiFile, msiParams);
    }
}