//css_ref ..\..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\..\..\Wix_bin\SDK\WixToolset.Dtf.WindowsInstaller.dll;

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
                new ManagedAction("CustomAction")
                {
                    ActionAssembly = "CustomAction.dll",
                    RefAssemblies = new [] { "DispalyMessage.dll" }
                },
            },
        };

        Compiler.BuildMsi(project);
    }
}