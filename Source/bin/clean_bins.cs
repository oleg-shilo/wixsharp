//css_args /ac
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.IO;
using System.Linq;
using System;

void main()
{
    var root = @"WixSharp\Samples";

    cleanBuildDirs(root);

    DeleteFiles(root, "*.cmd.log");
    DeleteFiles(root, "*.msi");
    DeleteFiles(root, "*.CA.dll");
    DeleteFiles(root, "*.cs.dll");
    DeleteFiles(root, "*.pdb");
    DeleteFiles(root, "*.dll.wxs");
    DeleteFiles(root, "*.dll");
    DeleteFiles(root, "*.exe");
    DeleteFiles(root, "*-old*");
    DeleteFiles(root, "*.exe.wxs");
    DeleteFiles(root, "*.user");
    DeleteFiles(root, "CustomAction.config");

    foreach (var wixDir in Directory.GetFiles(root, "*.g.wxs", SearchOption.AllDirectories).Select(x => Path.GetDirectoryName(x)))
        DeleteDir(wixDir);
}

void DeleteFiles(string dir, string pattern)
{
    foreach (var file in Directory.GetFiles(dir, pattern, SearchOption.AllDirectories))
        File.Delete(file);
}

void DeleteDir(string dir)
{
    if (!Directory.Exists(dir))
        return;

    var dirsToDelete = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories)
                                .OrderByDescending(x => x);

    foreach (var subDir in dirsToDelete)
    {
        foreach (var file in Directory.GetFiles(subDir, "*"))
        {
            File.Delete(file);
        }

        Directory.Delete(subDir);
    }
    foreach (var file in Directory.GetFiles(dir, "*"))
        File.Delete(file);
    Directory.Delete(dir);
}

void cleanBuildDirs(string rootDir)
{
    var buidDirs = Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories)
             .Where(dir =>
             {
                 return !dir.ToLower().Contains("sourcebasedir") && !dir.ToLower().Contains("wildcard files") &&
                        (dir.EndsWith(@"bin\debug", StringComparison.OrdinalIgnoreCase) ||
                         dir.EndsWith(@"bin\release", StringComparison.OrdinalIgnoreCase) ||
                         dir.EndsWith(@"obj\debug", StringComparison.OrdinalIgnoreCase) ||
                         dir.EndsWith(@"obj\release", StringComparison.OrdinalIgnoreCase));
             })
             .Select(x => Path.GetDirectoryName(x))
             .Distinct()
             .OrderBy(x => x);

    foreach (var dir in buidDirs)
    {
        DeleteDir(dir);
        continue;

        var dirsToDelete = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories).ToList();
        dirsToDelete.Add(dir);
        dirsToDelete = dirsToDelete.OrderByDescending(x => x).ToList();

        dirsToDelete.ForEach(x =>
           {
               Console.WriteLine("    >  " + x);
               foreach (var file in Directory.GetFiles(x, "*"))
               {
                   // Console.WriteLine("       >> " + file);
                   File.Delete(file);
               }

               Directory.Delete(x, true);
           });

        Console.WriteLine("---");
    }
}