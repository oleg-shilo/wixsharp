//css_ref ..\..\..\WixSharp.dll;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
using System;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main(string[] args)
    {
        // Enable the line below to indicate whether the EULA has been accepted.
        // This is required for the WiX v7 and higher. The expected value for WiX 7 is "wix7".
        // See https://docs.firegiant.com/wix/osmf/ for details.</remarks>
        // Note, that by enabling the line below you are legally entering EULA between you and WiX Toolset company. 
        // WixSharp only provides the mechanism for passing your acceptance of EULA to the WiX compiler `wix.exe`.
        // WixSharp.CommonTasks.WixTools.AcceptEulaFor = "wix7";

        // This is the fall back option to wix v6, which does not require explicit EULA acceptance.
        WixTools.SetWixVersion(Environment.CurrentDirectory, "6.0.2");
        WixExtension.UI.PreferredVersion = "6.0.2";

        var docs = new Feature("Documentation");
        var binaries = new Feature("Binaries");

        var project =
            new Project("MyProduct",

                new LaunchCondition("CUSTOM_UI=\"true\" OR REMOVE=\"ALL\"", "Please run setup.exe instead."),

                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(binaries, @"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(docs, @"Files\Docs\Manual.txt"))));

        project.UI = WUI.WixUI_Common;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}