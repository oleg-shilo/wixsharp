//css_args /ac
using System.Diagnostics;
using System.Text;
using System.Reflection;
// using System.Windows.Forms;
using System.IO;
using System.Linq;
using System;

void main()
{
    string version = Assembly.LoadFrom(@"WixSharp\WixSharp.dll").GetName().Version.ToString();

    Directory.GetDirectories(@"WixSharp\Samples", "*", SearchOption.AllDirectories)
             .Where(dir =>
             {
                 return !dir.ToLower().Contains("sourcebasedir") && !dir.ToLower().Contains("wildcard files") &&
                        (dir.EndsWith(@"bin\debug", StringComparison.OrdinalIgnoreCase) ||
                         dir.EndsWith(@"bin\release", StringComparison.OrdinalIgnoreCase) ||
                         dir.EndsWith(@"obj\debug", StringComparison.OrdinalIgnoreCase) ||
                         dir.EndsWith(@"obj\release", StringComparison.OrdinalIgnoreCase));
             })
             .ToList()
             .ForEach(dir =>
             {
                 Console.WriteLine("del dir: " + dir);
                 Directory.Delete(dir, true);
             });

    var exclusions = "echo.exe;registrator.exe;registrator.exe;myapp.exe;some.exe;cscs.exe".Split(';');
    bool deleted = false;

    var filesToDelete = Directory.GetFiles(@"WixSharp\Samples", "*.exe", SearchOption.AllDirectories)
             .Where(x => !Path.GetDirectoryName(x).ToLower().EndsWith(@"wix_bin\bin"))
             .Where(x => !Path.GetDirectoryName(x).ToLower().EndsWith("appfiles"))
             .Where(x => !exclusions.Contains(Path.GetFileName(x.ToLower())))
             .ToList();

    if (filesToDelete.Any())
        Console.WriteLine("!!!!!!!!!!!!!  SOME FILES WILL BE DELETED !!!!!!!!! ");

    filesToDelete.ForEach(exe =>
        {
            Console.WriteLine("!!!!!!!!!!!!!   " + exe);
            File.Delete(exe);
            deleted = true;
        });

    string outputFile = Path.GetFullPath("WixSharp." + version + ".7z");
    string inputDir = Path.GetFullPath("WixSharp");

    string app = Path.GetFullPath(@".\.build\7z.exe");
    string args = @"a -r -t7z " + outputFile + " " + inputDir + @"\*.*";

    run(app, args);
}

void run(string app, string args)
{
    var p = new Process();
    p.StartInfo.FileName = app;
    p.StartInfo.Arguments = args;
    p.StartInfo.UseShellExecute = false;
    p.StartInfo.RedirectStandardOutput = true;
    p.StartInfo.CreateNoWindow = true;
    p.Start();

    string line = null;

    while (null != (line = p.StandardOutput.ReadLine()))
        Console.WriteLine(line);

    p.WaitForExit();
}