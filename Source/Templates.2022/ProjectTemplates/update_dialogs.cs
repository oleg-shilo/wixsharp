//css_include global-usings

using System.Diagnostics;

"------------------------------------".print();

updateWinFormDialogs(
    @"..\..\src\WixSharp.UI\ManagedUI\Forms",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom UI (WiX3)\Dialogs");

updateWinFormDialogs(
    @"..\..\src\WixSharp.UI\ManagedUI\Forms",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom UI\Dialogs");

updateWinFormDialogs(
    @"..\..\src\WixSharp.UI\ManagedUI\Forms",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp - Custom UI Library\Dialogs");

updateWpfDialogs(
    @"..\..\src\WixSharp.UI.WPF\Dialogs",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI (WiX3)\Dialogs");

updateWpfDialogs(
    @"..\..\src\WixSharp.UI.WPF\Dialogs",
    @"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI\Dialogs");

var proj3Files = Directory.GetFiles(@"..\..\Templates.2022\ProjectTemplates", "*.csproj", SearchOption.AllDirectories).Where(x => x.Contains(" (WiX3)")); // WiX3
var projFiles = proj3Files.Select(x => x.Replace(" (WiX3)", "")); // WiX4+

updateWixSharpPackages(proj3Files, "1.26.0");
updateWixSharpPackages(projFiles, "2.6.2");

foreach (var file in Directory.GetFiles(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI (WiX3)\Dialogs",
        "*.xaml.cs"))
{
    var code = File.ReadAllText(file);
    File.WriteAllText(file, code.Replace("\"WixSharpUI_Bmp", "\"WixUI_Bmp"));
}

"------------------------------------".print();
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom UI (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom UI");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp - Custom UI Library");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom Dialog (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom Dialog");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Managed Setup - Custom WPF UI");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup - Bootstrapper (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup - Bootstrapper");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup - Bootstrapper Custom BA (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup - Bootstrapper Custom BA");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup (WiX3)");
packageDir(@"..\..\Templates.2022\ProjectTemplates\WixSharp Setup");
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
    var pckgName = dir + ".zip";

    if (File.Exists(pckgName))
        File.Delete(pckgName);

    string args = $"a -r -tzip \"{pckgName}\" \"{dir}\\*.*\"";

    run(app, args, workingDir);
}

void processFile(string file)
{
    // if (file.EndsWith(".cs"))
    {
        var code = File.ReadAllText(file);
        File.WriteAllText(file, code
            .Replace("namespace WixSharp.UI.Forms", $"using WixSharp;{NewLine}{NewLine}" +
                                                    $"using WixSharp.UI.Forms;{NewLine}{NewLine}" +
                                                    $"namespace $safeprojectname$.Dialogs")
            .Replace("namespace WixSharp.UI.WPF", $"using WixSharp.UI.WPF;{NewLine}{NewLine}" +
                                                  $"namespace $safeprojectname$")

            .Replace("xmlns:wixsharp=\"clr-namespace:WixSharp.UI.WPF\"", @"xmlns:wixsharp=""clr-namespace:WixSharp.UI.WPF;assembly=WixSharp.UI.WPF""")
            .Replace("x:Class=\"WixSharp.UI.WPF", @"x:Class=""$safeprojectname$")
            .Replace("using WixToolset.Dtf.WindowsInstaller;",
                     file.Contains("WiX3", StringComparison.OrdinalIgnoreCase) ?
                         "using Microsoft.Deployment.WindowsInstaller;" :
                         "using WixToolset.Dtf.WindowsInstaller;")
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

void updateWixSharpPackages(IEnumerable<string> projFiles, string version)
{
    foreach (string file in projFiles)
    {
        Console.WriteLine("Package for: " + file);
        var lines = File.ReadLines(file);
        lines = lines.Select(line =>
        {
            // <PackageReference Include="WixSharp" Version="1.25.2" />
            // WixSharp.wix.bin - should be excluded
            if (!line.Contains("WixSharp.wix.bin") && line.Contains("PackageReference") && line.Contains("Include=\"WixSharp"))
            {
                var currentVersion = line.Split(" ").Where(x => x.Trim().Contains("Version=\"")).First();
                Console.WriteLine($"    {currentVersion} => {version} ");
                return line.Replace(currentVersion, $"Version=\"{version}\"");
            }
            else
                return line;
        }).ToArray();

        File.WriteAllLines(file, lines);
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