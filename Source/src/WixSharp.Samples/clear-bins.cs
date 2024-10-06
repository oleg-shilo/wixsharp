//css_include global-usings

using CSScripting;

var samples_dirs = Directory.GetFiles(".\\", "*csproj", SearchOption.AllDirectories).Select(Path.GetDirectoryName);

void delFiles(string path, string pattern)
{
    Directory
        .GetFiles(path, pattern)
        .ForEach(delFile);
}
void delFile(string path)
{
    if (File.Exists(path))
        File.Delete(path);
}

void delDir(params string[] pathTokens)
{
    var path = Path.Combine(pathTokens);

    if (Directory.Exists(path))
    {
        Directory
            .GetFiles(path, "*")
            .ForEach(delFile);

        Directory
            .GetDirectories(path, "*", SearchOption.AllDirectories)
            .OrderBy(x => x)
            .ForEach(x => delDir(x));

        Directory.Delete(path);
    }
}

foreach (var dir in samples_dirs)
{
    Console.WriteLine(dir);
    delDir(dir, "obj");
    delDir(dir, "bin");
    delDir(dir, "wix");
    delFiles(dir, "*.cmd.log");
    delFiles(dir, "*.msi");
}