//css_args
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics;

// using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

class app
{
    static void Main()
    {
        string version = Assembly.LoadFrom(@"WixSharp\WixSharp.dll").GetName().Version.ToString();
        Console.WriteLine($"Current Release: {version}");

        var releaseNotes = $"ReleaseNotes.{version}.txt";

        if (!File.Exists(releaseNotes))
        {
            var lastRelease = Directory
                .GetFiles(@".\", "ReleaseNotes.*.txt")
                .Select(x => Path.GetFileNameWithoutExtension(x).Replace("ReleaseNotes.", ""))
                .Where(x => x != version)
                .Select(x => { try { return new Version(x); } catch { return new Version(); } })
                .Order()
                .Select(x => x.ToString())
                .LastOrDefault();

            Console.WriteLine($"Last Release: {lastRelease}");

            File.AppendAllLines(releaseNotes, new[] { $"Release v{version}" });
            run("git", $"log --pretty=format:'%s' v{lastRelease}..HEAD", line => File.AppendAllLines(releaseNotes, new[] { line }));
            Task.Run(() => run("notepad", releaseNotes));
            Thread.Sleep(1000);
        }

        Console.WriteLine("---");

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

    static void run(string app, string args = "", Action<string> onLine = null)
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
            if (onLine == null)
                Console.WriteLine(line);
            else
                onLine(line);

        p.WaitForExit();
    }
}