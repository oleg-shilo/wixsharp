using System.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

class Script
{
    static string root = Path.GetFullPath(@"..\..\");

    static public void Main()
    {
        var version = Directory.GetFiles(root + @"\bin", "WixSharp.*.*.*.*.7z", SearchOption.TopDirectoryOnly)
                               .Select(x => new Version(Path.GetFileName(x).Replace("WixSharp.", "").Replace(".7z", "")))
                               .OrderByDescending(x => x)
                               .FirstOrDefault();

        Console.WriteLine("Version: " + version);

        string releaseNotes = ValidateReleaseNotes(version.ToString());

        UpdateReleaseNotesAndVersion(root + @"\NuGet\WixSharp\WixSharp.nuspec", releaseNotes, version.ToString());
        UpdateReleaseNotesAndVersion(root + @"\NuGet\WixSharp\WixSharp.WPF.nuspec", releaseNotes, version.ToString());
        UpdateReleaseNotesAndVersion(root + @"\NuGet\WixSharp\WixSharp.bin.nuspec", releaseNotes, version.ToString());

        UpdateReleaseNotesAndVersion(root + @"\NuGet\WixSharp\WixSharp_wix4.nuspec", releaseNotes, version.ToString());
        UpdateReleaseNotesAndVersion(root + @"\NuGet\WixSharp\WixSharp_wix4.WPF.nuspec", releaseNotes, version.ToString());
        UpdateReleaseNotesAndVersion(root + @"\NuGet\WixSharp\WixSharp_wix4.bin.nuspec", releaseNotes, version.ToString());

        CopyFiles(root + @"\bin\WixSharp", "nbsbuilder.exe", "lib");
        CopyFiles(root + @"\bin\WixSharp", "WixSharp.dll", "lib");
        CopyFiles(root + @"\bin\WixSharp", "WixSharp.xml", "lib");
        CopyFiles(root + @"\bin\WixSharp", "WixSharp.UI.dll", @"lib");
        CopyFiles(root + @"\bin\WixSharp", "WixSharp.UI.xml", @"lib");
        CopyFiles(root + @"\bin\WixSharp", "WixSharp.UI.WPF.dll", @"lib");
        CopyFiles(root + @"\bin\WixSharp", "WixSharp.UI.WPF.xml", @"lib");
        CopyFiles(root + @"\bin\WixSharp", "WixSharp.Msi.dll", "lib");
        CopyFiles(root + @"\bin\WixSharp", "WixSharp.Msi.xml", @"lib");

        ValidateDllVersions(version.ToString());	

        Console.WriteLine("Done!");
    }

    static void UpdateReleaseNotesAndVersion(string specFile, string releaseNotes, string version)
    {
        var doc = XDocument.Load(specFile);
        var ttt = doc.Descendants().Where(x => x.Name.LocalName == "version");
        ttt = doc.Descendants().Where(x => x.Name.LocalName == "releaseNotes");
        doc.Descendants().Where(x => x.Name.LocalName == "version").First().Value = version;
        doc.Descendants().Where(x => x.Name.LocalName == "releaseNotes").First().Value = releaseNotes;

        var wixSharp_bins = doc.Descendants()
                               .Where(x => x.Name.LocalName == "dependency" &&
                                          (x.Attribute("id").Value == "WixSharp.bin" || x.Attribute("id").Value == "WixSharp_wix4.bin"));

        foreach (var dependencyElement in wixSharp_bins)
            dependencyElement.Attribute("version").Value = version;

        doc.Save(specFile);
    }

    static void ValidateDllVersions(string version)
    {
        var versions = Directory.GetFiles(Environment.CurrentDirectory, "WixSharp*.dll", SearchOption.AllDirectories)
                .Select(x => new { version = FileVersionInfo.GetVersionInfo(x).FileVersion, path = x });

        if (versions.Select(x => version).Distinct().Count() > 1 || versions.FirstOrDefault().version != version)
        {
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            throw new Exception("ERROR: Inconsistent dll versions: \n" + string.Join('\n', versions));
        }
        else
            Console.WriteLine("===\ndll versions: \n" + string.Join('\n', versions));
    }

    static string ValidateReleaseNotes(string version)
    {
        string releaseNotes = root + @"\bin\ReleaseNotes." + version + ".txt";

        if (!File.Exists(releaseNotes))
            File.WriteAllText(releaseNotes, "");

        string retval = File.ReadAllText(releaseNotes);

        if (string.IsNullOrEmpty(retval))
        {
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("Release notes are not ready!");
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Process.Start(releaseNotes);
        }

        return retval;
    }

    static void CopyFiles(string srcDir, string pattern, string destDir, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        foreach (string file in Directory.GetFiles(srcDir, pattern, option))
        {
            string path = destDir + "\\" + file.Substring(srcDir.Length + 1);
            File.Copy(file, path, true);
            Console.WriteLine(path);
        }
    }
}