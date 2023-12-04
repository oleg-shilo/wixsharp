//css_include global-usings

using System.Diagnostics;

"------------------------------------".print();

CopyFiles(@"..\..\src\WixSharp.UI\ManagedUI\Forms", "*Dialog*.cs",
          @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Cusom UI\Dialogs",
          x => !x.Contains("InstallScope") &&
               !x.Contains("AdvancedFeatures") &&
               !x.Contains("scaling"),
               processFile);

CopyFiles(@"..\..\src\WixSharp.UI\ManagedUI\Forms", "*Dialog.resx",
          @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Cusom UI\Dialogs",
          x => !x.Contains("InstallScope") &&
               !x.Contains("AdvancedFeatures"));

CopyFiles(@"..\..\src\WixSharp.UI.WPF\Dialogs", "*Dialog.xaml*",
          @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI\Dialogs",
          null, processFile);

"------------------------------------".print();
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Cusom UI");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom Dialog");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup - Bootstrapper");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup - Custom CLR Dialog");
"------------------------------------".print();

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