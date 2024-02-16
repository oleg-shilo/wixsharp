//css_winapp
//css_ng dotnet
//css_args  -rx -netfx
//css_dir ..\..\;
//css_ref System.Core.dll;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;
using WixToolset.Dtf.WindowsInstaller;
using Action = WixSharp.Action;

public class Script
{
    static public void Main(string[] args)
    {
        var project = new ManagedProject("CustomActionTest",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("setup.cs")),

                new ManagedAction("Net7CA")
                {
                    ActionAssembly = CompileAotAssembly(@".\CustomAction\NetCoreCustomAction.csproj"),
                    MethodName = "CustomActionCore",
                    CreateInteropWrapper = false,
                });

        project.BuildMsi();
    }

    static string CompileAotAssembly(string projFile)
    {
        var projDir = projFile.PathGetDirName();
        var outDir = "outdir";
        var asmFileName = projFile.PathGetFileNameWithoutExtension() + ".dll";

        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"publish /p:NativeLib=Shared -r win-x64 -c release -o {outDir}";
            process.StartInfo.WorkingDirectory = projFile.PathGetDirName();
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
                return projDir.PathCombine(outDir, asmFileName);

            return "<unknown>";
        }
    }
}