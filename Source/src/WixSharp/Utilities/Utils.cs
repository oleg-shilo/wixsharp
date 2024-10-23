// Ignore Spelling: Deconstruct

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using WixSharp.UI;
using WixToolset.Dtf.WindowsInstaller;

using IO = System.IO;

#pragma warning disable CA1416 // Validate platform compatibility

namespace WixSharp
{
    /// <summary>
    /// A utility for creating disconnected MSI session.
    /// </summary>
    public static class DisconnectedSession
    {
        /// <summary>
        /// Creates the instance of the disconnected Session.
        /// </summary>
        /// <returns></returns>
        public static Session Create()
        {
            var constr = typeof(Session).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault();

            var constrArgs = new object[] { (IntPtr)1, false };

            var result = constr.Invoke(constrArgs);

            return (Session)result;
        }
    }

    static class ReflectionExtensions
    {
        public static string[] GetRefAssembliesOf(string assembly)
        {
            try
            {
                return GetRefAssemblies(System.Reflection.Assembly.LoadFrom(assembly));
            }
            catch { return new string[0]; }
        }

        public static string[] GetRefAssemblies(this System.Reflection.Assembly assembly)
        {
            var dependencies = assembly
                .GetReferencedAssemblies()
                .Where(x => !x.Name.StartsWith("System"))
                .Select(x =>
                {
                    try
                    {
                        return System.Reflection.Assembly.ReflectionOnlyLoad(x.FullName).Location;
                    }
                    catch { return null; }
                })
                .Where(x => x.IsNotEmpty() && !x.StartsWith(Environment.SpecialFolder.Windows.GetPath(), true))
                .ToArray();

            return dependencies;
        }

        public static object Call(this MethodInfo method, params object[] args)
        {
            object instance = method.IsStatic ? null : Activator.CreateInstance(method.DeclaringType);
            return method.Invoke(instance, args);
        }
    }

    class WixItems : WixObject
    {
        public IEnumerable<WixObject> Items;

        public WixItems(IEnumerable<WixObject> items)
        {
            Items = items;
        }
    }

    /// <summary>
    /// Utility for generating self-hosted MSI executables.Such a tool is a simple launcher of the
    /// msiexec.exe with the embedded (as an exe resource) msi file.
    /// </summary>
    public static class ExeGen
    {
        /// <summary>Compiles the self hosted msi.</summary>
        /// <param name="msiFile">The msi file.</param>
        /// <param name="outFile">The out file.</param>
        /// <returns>
        /// </returns>
        public static (int exitCode, string output) CompileSelfHostedMsi(this string msiFile, string outFile)
        {
            var parser = new MsiParser(msiFile);
            var csc = LocateCsc();

            var name = parser.GetProductName();
            var version = parser.GetProductVersion();
            var productCode = parser.GetProductCode();

            var csFile = GenerateCSharpSource(Path.GetTempPath().PathCombine(msiFile.PathGetFileName()), name, version, productCode);

            try
            {
                // return csc.Run($"\"/res:{msiFile}\" \"-out:{outFile}\" /t:winexe /debug+ /define:DEBUG \"{csFile}\"", Path.GetDirectoryName(outFile));
                return csc.Run($"\"/res:{msiFile}\" \"-out:{outFile}\" /t:winexe \"{csFile}\"", Path.GetDirectoryName(outFile));
            }
            finally
            {
                IO.File.Delete(csFile);
            }
        }

        static string LocateCsc() =>
            Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework"), "csc.exe", SearchOption.AllDirectories)
                .OrderByDescending(x => x)
                .FirstOrDefault();

        static string GenerateCSharpSource(string outFile, string name, string version, string productCode)
        {
            var code = @"
using System;
using System.Resources;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;

[assembly: AssemblyTitle(""" + outFile.PathGetFileName() + @""")]
[assembly: AssemblyDescription(""Self-hosted " + outFile.PathGetFileNameWithoutExtension() + @".msi"")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany(""WixSharp"")]
[assembly: AssemblyProduct(""" + name + @""")]
[assembly: AssemblyCopyright("""")]
[assembly: AssemblyTrademark("""")]
[assembly: AssemblyCulture("""")]
[assembly: AssemblyVersion(""" + version + @""")]
[assembly: AssemblyFileVersion(""" + version + @""")]
class Program
{
    static int Main(string[] args)
    {
        // Debug.Assert(false);

        // string msi = GetMsiCacheName();
        string msi = Path.GetTempFileName();
        try
        {
            ExtractMsi(msi);
            string msi_args = args.Any() ? string.Join("" "", args) : ""/i"";

            Process p = Process.Start(""msiexec.exe"", msi_args + "" \"""" + msi + ""\"""");
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
            File.Delete(msi);
        }
    }

    static string GetMsiCacheName()
    {
        var p = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());
        var admin = p.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

        var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ""WixSharp"", ""Installer"", """ + productCode + @""");
        if (admin)
            dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), ""Installer"", """ + productCode + @""");

        Directory.CreateDirectory(dir);
        var msi = Path.Combine(dir, """ + name + @".msi"");

        File.WriteAllText(Path.Combine(dir, ""wixsharp.cache""), """");
        return msi;
    }

    static void ExtractMsi(string outFile)
    {
        Assembly asm = Assembly.GetExecutingAssembly();

        var names = asm.GetManifestResourceNames();
        using (Stream stream = asm.GetManifestResourceStream(names.First()))

        {
            if (stream != null)
            {
                byte[] resourceBytes = new byte[stream.Length];
                stream.Read(resourceBytes, 0, resourceBytes.Length);

                File.WriteAllBytes(outFile, resourceBytes);
            }
            else
            {
                Console.WriteLine(""Resource not found."");
            }
        }
    }
}";
            IO.File.WriteAllText(outFile, code);
            return outFile;
        }

        static (int exitCode, string output) Run(this string exe, string arguments, string workingDir)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = exe;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = workingDir;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                var output = new StringBuilder();

                output.AppendLine(process.StandardOutput.ReadToEnd());
                output.AppendLine(process.StandardError.ReadToEnd());

                process.WaitForExit();
                return (process.ExitCode, output.ToString());
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    [SuppressUnmanagedCodeSecurity, SecurityCritical]
    public static class Native
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);

        /// <summary>
        /// Displays the message box. This method is native and has no dependency on WinForms or WPF. Thus it is very
        /// useful when you need to show message box to the user from the AOT compiled assembly.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        public static int MessageBox(string message, string title = "") => MessageBox(GetForegroundWindow(), message, title, 0);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GetUserPreferredUILanguages(uint dwFlags, out uint pulNumLanguages, char[] pwszLanguagesBuffer, ref uint pcchLanguagesBuffer);

        /// <summary>
        /// Gets the preferred ISO two-letter UI languages.
        /// <para>On Windows Vista and later, the user UI language is the first language in the user preferred UI languages list.</para>
        /// <para>Source: https://github.com/MicrosoftDocs/win32/blob/docs/desktop-src/Intl/user-interface-language-management.md)</para>
        /// </summary>
        /// <returns>The list of preferred ISO two-letter UI languages</returns>
        public static string[] GetPreferredIsoTwoLetterUILanguages()
        {
            const uint MUI_LANGUAGE_NAME = 0x8; // Use ISO language (culture) name convention

            uint languagesCount, languagesBufferSize = 0;

            if (Native.GetUserPreferredUILanguages(MUI_LANGUAGE_NAME, out languagesCount, null, ref languagesBufferSize))
            {
                char[] languagesBuffer = new char[languagesBufferSize];
                if (Native.GetUserPreferredUILanguages(MUI_LANGUAGE_NAME, out languagesCount, languagesBuffer, ref languagesBufferSize))
                {
                    List<string> result = new List<string>((int)languagesCount);
                    string[] languages = new string(languagesBuffer, 0, (int)languagesBufferSize - 2).Split('\0');
                    // Console.WriteLine("GetUserPreferredUILanguages returns " + languages.Length + " languages:");
                    foreach (string language in languages)
                    {
                        //
                        // Register as ISO two letter language when format is xx-xx.
                        //
                        if (language.Length == 5 && language[2] == '-')
                        {
                            result.Add(language.Substring(0, 2));
                        }
                    }

                    return result.ToArray();
                }
                else
                {
                    Console.WriteLine("GetUserPreferredUILanguages(2) error: " + Marshal.GetLastWin32Error());

                    return null;
                }
            }
            else
            {
                Console.WriteLine("GetUserPreferredUILanguages(1) error: " + Marshal.GetLastWin32Error());

                return null;
            }
        }
    }
}