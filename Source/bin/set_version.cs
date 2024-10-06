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
        string version = File.ReadAllLines(@".\..\src\WixSharp\Properties\AssemblyInfo.version.cs")
                             .First(x => x.Contains("[assembly: AssemblyFileVersion"))
                             .Split('"')[1];

        Console.WriteLine("Current Release: " + version);

        version = string.Join(".", version.Split('.').Take(3).ToArray());

        Console.WriteLine("NuGet: " + version);
        patch_proj_file(@".\..\src\NET-Core\WixSharp.Core\WixSharp.Core.csproj", version);
        patch_proj_file(@".\..\src\NET-Core\WixSharp.Msi.Core\WixSharp.Msi.Core.csproj", version);
    }

    static void patch_proj_file(string file, string version)
    {
        var lines = File.ReadAllLines(file).Select(x =>
            {
                if (x.Contains("<PackageVersion>"))
                    return "    <PackageVersion>" + version + "</PackageVersion>";
                else
                    return x;
            }).ToArray();
        File.WriteAllLines(file, lines);
    }
}