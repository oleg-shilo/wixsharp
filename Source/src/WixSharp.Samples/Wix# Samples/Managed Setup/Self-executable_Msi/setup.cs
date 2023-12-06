using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Self_executable_Msi;

public static class Launcher
{
    static public int Main(string[] args)
    {
        // you can check if you are elevated here
        var msi = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(msi, Resources.ManagedSetup);
            string msi_args = args.Any() ? string.Join(" ", args) : "/i";

            var p = Process.Start("msiexec.exe", $"{msi_args} \"{msi}\"");
            p.WaitForExit();
            return p.ExitCode;
        }
        catch
        {
            // report the error
            return -1;
        }
        finally
        {
            try
            {
                if (File.Exists(msi))
                    File.Delete(msi);
            }
            catch { }
        }
    }
}