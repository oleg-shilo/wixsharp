//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Diagnostics;

static class Script
{
    static public void Main()
    {
        var project = new Project("CustomActionTest",

                new ManagedAction(CustomActions.RunAsAdminInstall, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed, Sequence.InstallUISequence),
                new ManagedAction(CustomActions.MyCheckSql, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed, Sequence.InstallUISequence),
                new ManagedAction(CustomActions.MyCheckMvc4, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed, Sequence.InstallUISequence),
                new ManagedAction(CustomActions.CompareVersionAtUpgrade, Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed, Sequence.InstallUISequence),

                new ManagedAction(CustomActions.MyAdminAction, Return.check, When.After, Step.InstallInitialize, Condition.NOT_Installed, Sequence.AdminExecuteSequence));

        Compiler.BuildMsi(project);
    }
}

public static class CustomActions
{
    [CustomAction]
    public static ActionResult RunAsAdminInstall(Session session)
    {
        MessageBox.Show("RunAsAdminInstall", "Embedded Managed CA");
        session.Log("Begin RunAsAdminInstall Hello World");

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult MyCheckSql(Session session)
    {
        MessageBox.Show("MyCheckSql", "Embedded Managed CA");
        session.Log("Begin MyCheckSql Hello World");

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult MyCheckMvc4(Session session)
    {
        MessageBox.Show("MyCheckMvc4", "Embedded Managed CA");
        session.Log("Begin MyCheckMvc4 Hello World");

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult CompareVersionAtUpgrade(Session session)
    {
        MessageBox.Show("CompareVersionAtUpgrade", "Embedded Managed CA");
        session.Log("Begin CompareVersionAtUpgrade Hello World");

        return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult MyAdminAction(Session session)
    {
        MessageBox.Show("Hello World!!!!!!!!!!!", "Embedded Managed CA (Admin)");
        session.Log("Begin MyAdminAction Hello World");

        return ActionResult.Success;
    }
}