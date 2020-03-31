//css_ref ..\..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main()
    {
        List<string> UserSelectedItems = null;
        List<string> InitialUserSelectedItems = "1,2".Split(',').ToList();

        bool userChangedFeatures = UserSelectedItems?.JoinBy(",") != InitialUserSelectedItems.JoinBy(",");

        UserSelectedItems = "1,2".Split(',').ToList();

        userChangedFeatures = UserSelectedItems?.JoinBy(",") != InitialUserSelectedItems.JoinBy(",");

        var project = new Project()
        {
            UI = WUI.WixUI_ProgressOnly,
            Name = "CustomActionTest",

            Actions = new[]
            {
                new ManagedAction(CustomActions.MyAction, "%this%")
            }
        };

        // Debug.Assert(false);

        Compiler.BuildMsi(project);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        MessageBox.Show("Hello World!", "Embedded Managed CA");
        session.Log("Begin MyAction Hello World");

        return ActionResult.Success;
    }
}