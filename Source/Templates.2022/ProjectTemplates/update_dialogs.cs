//css_include global-usings

using System.Diagnostics;

"------------------------------------".print();

updateWinFormDialogs(
    @"..\..\src\WixSharp.UI\ManagedUI\Forms",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Cusom UI (WiX3)\Dialogs");

updateWinFormDialogs(
    @"..\..\src\WixSharp.UI\ManagedUI\Forms",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Cusom UI (WiX4)\Dialogs");

updateWpfDialogs(
    @"..\..\src\WixSharp.UI.WPF\Dialogs",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI (WiX3)\Dialogs");

updateWpfDialogs(
    @"..\..\src\WixSharp.UI.WPF\Dialogs",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI (WiX4)\Dialogs");

"------------------------------------".print();
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup (WiX4)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Cusom UI (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Cusom UI (WiX4)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom Dialog (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom Dialog (WiX4)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI (WiX4)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup - Bootstrapper (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup - Bootstrapper (WiX4)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup (WiX4)");
"------------------------------------".print();

void updateWpfDialogs(string inputDir, string outDir)
{
    CopyFiles(inputDir, "*Dialog.xaml*", outDir, null, processFile);
}
void updateWinFormDialogs(string inputDir, string outDir)
{
    CopyFiles(inputDir, "*Dialog*.cs", outDir,
        x => !x.Contains("InstallScope") &&
                !x.Contains("AdvancedFeatures") &&
                !x.Contains("scaling"),
                processFile);

    CopyFiles(inputDir, "*Dialog.resx", outDir,
              x => !x.Contains("InstallScope") &&
                   !x.Contains("AdvancedFeatures"));
}
void packageDir(string inputDir)
{
    string app = Path.GetFullPath(@"..\..\bin\.build\7z.exe");
    string workingDir = Path.GetFullPath(inputDir);

    var dir = Path.GetFullPath(inputDir);
    var pcgName = dir + ".zip";

    string args = $"a -r -tzip \"{pcgName}\" \"{dir}\\*.*\"";

    run(app, args, workingDir);
}

void processFile(string file)
{
    if (file.EndsWith(".cs"))
    {
        var code = File.ReadAllText(file);
        File.WriteAllText(file, code
            .Replace("namespace WixSharp.UI.Forms", @"using WixSharp;
using WixSharp.UI.Forms;

namespace $safeprojectname$.Dialogs")

            .Replace("namespace WixSharp.UI.WPF", @"using WixSharp.UI.WPF;

namespace $safeprojectname$")
                         );
    }
}

void CopyFiles(string srcDir, string pattern, string destDir, Func<string, bool> filter, Action<string> process = null, SearchOption option = SearchOption.TopDirectoryOnly)
{
    if (!Directory.Exists(destDir))
        Directory.CreateDirectory(destDir);

    foreach (string file in Directory.GetFiles(srcDir, pattern, option))
    {
        if (filter == null || filter(file))
        {
            string path = destDir + "\\" + file.Substring(srcDir.Length + 1);

            File.Copy(file, path, true);

            process?.Invoke(path);

            Console.WriteLine(path);
        }
    }
}

void run(string app, string args, string workDir)
{
    var p = new Process();
    p.StartInfo.FileName = app;
    p.StartInfo.Arguments = args;
    p.StartInfo.WorkingDirectory = workDir;
    p.StartInfo.UseShellExecute = false;
    p.StartInfo.RedirectStandardOutput = true;
    p.StartInfo.CreateNoWindow = true;
    p.Start();

    string line = null;

    while (null != (line = p.StandardOutput.ReadLine()))
        Console.WriteLine(line);

    p.WaitForExit();
}