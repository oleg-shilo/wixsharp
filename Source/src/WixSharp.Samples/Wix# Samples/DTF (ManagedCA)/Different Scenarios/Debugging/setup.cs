//css_ref ..\..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;

using System;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new Project()
        {
            UI = WUI.WixUI_ProgressOnly,
            Name = "CustomActionTest",

            Actions = new[]
            {
                new ManagedAction(CustomActions.MyAction, "%this%")
            }
        };

        // uncomment the line below if you want to troubleshoot packaging assembly with MakeSfxCA.exe
        //var batchFile = Compiler.BuildPackageAsmCmd(typeof(Script).Assembly.Location);

        project.BuildMsi();
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
#if DEBUG
        System.Diagnostics.Debugger.Launch();
#endif
        session.Log("Begin CustomAction2 Hello World");

        return ActionResult.Success;
    }
}