using Self_executable_Msi;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

public static class Launcher
{
    static public int Main(string[] args)
    {
        var msi = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(msi, Resources.ManagedSetup);
            string msi_args = args.Any() ? string.Join(" ", args) : "/i";

            var p = Process.Start("msiexec.exe", $"{msi_args} \"{msi}\"");
            p.WaitForExit();
            return p.ExitCode;
        }
        catch (Exception)
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