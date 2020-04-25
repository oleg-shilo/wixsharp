using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WixSharp.CommonTasks;
using IO = System.IO;
using Reflection = System.Reflection;

namespace WixSharp.Nsis
{
    /// <summary>
    /// Defines native (un-managed) bootstrapper. The bootstrapper is created by the NSIS installer authoring tool.
    /// The path to NSIS installation is detected through the WIXSHARP_NSISDIR environment variable or installation in
    /// the default "%ProgramFiles(x86)%\NSIS" location.
    /// The primary usage of <see cref="NsisBootstrapper"/> is to build bootstrappers for automatically installing .NET
    /// for executing MSIs containing managed Custom Actions (<see cref="ManagedAction"/>).
    /// <para></para>
    /// <remarks>
    /// NsisBootstrapper is subject to the following limitations:
    /// <list type="bullet">
    /// <item><description>Only Win32 native bootstrapper can be built.</description></item>
    /// <item><description>Only one <c>Prerequisite</c> cen be defined.</description></item>
    /// </list>
    /// </remarks>
    /// </summary>
    /// <example>The following is an example of defining and building bootstrapper for installing MyProduct.msi and
    /// .NET setup (dotnetfx.exe) as prerequisite installation.
    /// <para></para>
    /// <code>
    /// string setup = new NsisBootstrapper
    ///                    {
    ///                        Prerequisite = {
    ///                           FileName = "C:\Users\Public\Public Downloads\dotnetfx.exe",
    ///                           RegKeyValue = @"HKLM:SOFTWARE\Microsoft\.NETFramework:InstallRoot"
    ///                        }
    ///                        Primary = {FileName = "MyProduct.msi"},
    ///
    ///                        OutputFile = "setup.exe",
    ///
    ///                        IconFile = "app_icon.ico",
    ///
    ///                        VersionInfo = new VersionInformation("1.0.0.0")
    ///                        {
    ///                            ProductName = "Test Application",
    ///                            LegalCopyright = "Copyright Test company",
    ///                            FileDescription = "Test Application",
    ///                            FileVersion = "1.0.0.0",
    ///                            CompanyName = "Test company",
    ///                            InternalName = "setup.exe",
    ///                            OriginalFilename = "setup.exe"
    ///                        }
    ///                    }
    ///                    .Build();
    /// </code>
    /// </example>
    public class NsisBootstrapper : NsisBootstrapperLegacy
    {
        public override PrerequisitePackage Prerequisite { get; } = new PrerequisitePackage();

        public override PrimaryPackage Primary { get; } = new PrimaryPackage();

        /// <summary>
        /// Gets or sets the output file (bootstrapper) name.
        /// </summary>
        /// <value>The output file name.</value>
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the optional arguments for the bootstrapper compiler.
        /// </summary>
        /// <value>The optional arguments.</value>
        public string OptionalArguments { get; set; }

        /// <summary>
        /// Path to an icon that will replace the default icon in the output file (bootstrapper)
        /// </summary>
        /// <value>The icon file.</value>
        public string IconFile { get; set; }

        /// <summary>
        /// Gets the version information of the output file (bootstrapper).
        /// </summary>
        /// <value>The version information.</value>
        public VersionInformation VersionInfo { get; set; }

        /// <summary>
        /// Gets or sets the requested execution level. The value is embedded in the installer XML manifest.
        /// </summary>
        /// <value>The requested execution level.</value>
        public RequestExecutionLevel RequestExecutionLevel { get; set; }

        /// <summary>
        /// Occurs when NSI source code is generated. Use this event if you need to modify the generated source code
        /// before it is compiled into EXE.
        /// </summary>
        public event Action<StringBuilder> NsiSourceGenerated;

        /// <summary>
        /// Gets or sets the simple splash screen for the output file (bootstrapper).
        /// </summary>
        /// <value>The simple splash screen for the output file (bootstrapper)</value>
        public SplashScreen SplashScreen { get; set; }

        /// <summary>
        /// Builds bootstrapper file.
        /// </summary>
        /// <returns>Path to the built bootstrapper file. Returns <c>null</c> if bootstrapper cannot be built.</returns>
        public string Build()
        {
            try
            {
                return BuildInternal();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: " + e);
                return null;
            }
        }

        private string BuildInternal()
        {
            var nsisMake = DetectNsisMake();

            VerifyMinimumSupportedVersion(nsisMake);

            string regRootKey = null;
            string regSubKey = null;
            string regValueName = null;

            if (Prerequisite.RegKeyValue != null)
            {
                var regKeyTokens = Prerequisite.RegKeyValue.Split(':');

                if (regKeyTokens.Length != 3)
                {
                    throw new ArgumentException($"Prerequisite.RegKeyValue: {Prerequisite.RegKeyValue}.\nThis value must comply with the following pattern: <RegistryHive>:<KeyPath>:<ValueName>.");
                }

                regRootKey = regKeyTokens[0];
                regSubKey = regKeyTokens[1];
                regValueName = regKeyTokens[2];
            }

            var nsiFile = IO.Path.ChangeExtension(OutputFile, ".nsi");
            if (nsiFile == null)
            {
                throw new InvalidOperationException("NSI script file name can't be null.");
            }

            var builder = new StringBuilder();
            using (var writer = new IO.StringWriter(builder))
            {
                writer.WriteLine("Unicode true");

                AddIncludes(writer);

                AddMacros(writer);

                if (IconFile != null)
                {
                    writer.WriteLine($"Icon \"{IO.Path.GetFullPath(IconFile)}\"");
                }
                writer.WriteLine($"OutFile \"{IO.Path.GetFullPath(OutputFile)}\"");

                writer.WriteLine($"RequestExecutionLevel {ExecutionLevelToString(RequestExecutionLevel)}");
                writer.WriteLine("SilentInstall silent");

                AddVersionInformation(writer);

                writer.WriteLine("Function .onInit");

                writer.WriteLine("InitPluginsDir");

                // Read command line parameters
                writer.WriteLine("${GetParameters} $R0");

                AddSplashScreen(writer);

                AddPrerequisiteFile(writer, regRootKey, regSubKey, regValueName);

                AddPrimaryFile(writer);

                writer.WriteLine("end:");
                writer.WriteLine("FunctionEnd");

                writer.WriteLine("Section");
                writer.WriteLine("SectionEnd");
            }

            NsiSourceGenerated?.Invoke(builder);

            IO.File.WriteAllText(nsiFile, builder.ToString());

            var output = ExecuteNsisMake(nsisMake, $"/INPUTCHARSET UTF8 {nsiFile} {OptionalArguments}");
            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine(output);
            }

            // Return null if the OutputFile is failed to generate
            if (!IO.File.Exists(OutputFile))
            {
                return null;
            }

            if (!Compiler.PreserveTempFiles)
            {
                IO.File.Delete(nsiFile);
            }
            return OutputFile;
        }

        private static void AddIncludes(IO.StringWriter writer)
        {
            writer.WriteLine("!include LogicLib.nsh");
            writer.WriteLine("!include x64.nsh");
            writer.WriteLine("!include FileFunc.nsh");
        }

        private static void AddMacros(IO.StringWriter writer)
        {
            var assembly = Reflection.Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.Nsis.macros.nsh";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new IO.StreamReader(stream ?? throw new InvalidOperationException($"Error: \"{resourceName}\" cannot be found.")))
            {
                var result = reader.ReadToEnd();
                writer.WriteLine(result);
            }
        }

        private void AddPrerequisiteFile(IO.StringWriter writer, string regRootKey, string regSubKey, string regValueName)
        {
            if (Prerequisite.FileName == null)
                return;

            if (Prerequisite.RegKeyValue != null)
            {
                writer.WriteLine($"!insertmacro REG_KEY_VALUE_EXISTS {regRootKey} \"{regSubKey}\" \"{regValueName}\"");
                writer.WriteLine("IfErrors 0 primary");
            }

            var arguments = Prerequisite.Arguments;
            if (Prerequisite.OptionName != null)
            {
                writer.WriteLine($"${{GetOptions}} \"$R0\" \"{Prerequisite.OptionName}\" $R1");
                arguments = AppendArgument(arguments, "$R1");
            }
            AddExpandEnvStringsCommand(writer, ref arguments);

            AddFileCommand(writer, Prerequisite.FileName);
            AddExecuteCommand(writer, IO.Path.GetFileName(Prerequisite.FileName), arguments, Prerequisite.CreateNoWindow, "$0");

            if (Prerequisite.RegKeyValue != null && Prerequisite.PostVerify)
            {
                writer.WriteLine($"!insertmacro REG_KEY_VALUE_EXISTS {regRootKey} \"{regSubKey}\" \"{regValueName}\"");
                writer.WriteLine("IfErrors end 0");
            }
        }

        private void AddPrimaryFile(IO.StringWriter writer)
        {
            writer.WriteLine("primary:");
            if (Primary.OptionName != null)
            {
                writer.WriteLine($"${{GetOptions}} \"$R0\" \"{Primary.OptionName}\" $R1");
                // Skip copying the original command line options
                writer.WriteLine("IfErrors 0 +2");
            }
            // In case the primary files options are not passed in the command line,
            // copy the original command line options
            writer.WriteLine("StrCpy $R1 $R0");

            string arguments = AppendArgument(Primary.Arguments, "$R1");
            AddExpandEnvStringsCommand(writer, ref arguments);

            AddFileCommand(writer, Primary.FileName);
            AddExecuteCommand(writer, IO.Path.GetFileName(Primary.FileName), arguments, Primary.CreateNoWindow, "$0");

            // Set exit code
            writer.WriteLine("SetErrorlevel $0");
            writer.WriteLine("goto end");
        }

        private void AddVersionInformation(IO.StringWriter writer)
        {
            // Version Information
            if (VersionInfo != null)
            {
                if (VersionInfo.ProductVersion != null)
                {
                    var version = new Version(VersionInfo.ProductVersion);
                    version = new Version(
                        version.Major != -1 ? version.Major : 0,
                        version.Minor != -1 ? version.Minor : 0,
                        version.Build != -1 ? version.Build : 0,
                        version.Revision != -1 ? version.Revision : 0);

                    writer.WriteLine($"VIProductVersion \"{version}\"");
                }

                AddVersionKey(writer, "ProductName", VersionInfo.ProductName);
                AddVersionKey(writer, "Comments", VersionInfo.Comments);
                AddVersionKey(writer, "CompanyName", VersionInfo.CompanyName);
                AddVersionKey(writer, "LegalTrademarks", VersionInfo.LegalTrademarks);
                AddVersionKey(writer, "LegalCopyright", VersionInfo.LegalCopyright);
                AddVersionKey(writer, "FileDescription", VersionInfo.FileDescription);
                AddVersionKey(writer, "FileVersion", VersionInfo.FileVersion);
                AddVersionKey(writer, "ProductVersion", VersionInfo.ProductVersion);
                AddVersionKey(writer, "InternalName", VersionInfo.InternalName);
                AddVersionKey(writer, "OriginalFilename", VersionInfo.OriginalFilename);
                AddVersionKey(writer, "PrivateBuild", VersionInfo.PrivateBuild);
                AddVersionKey(writer, "SpecialBuild", VersionInfo.SpecialBuild);
            }
        }

        private static string ExecuteNsisMake(string nsisMake, string arguments)
        {
            var compiler = new ExternalTool
            {
                ExePath = nsisMake,
                Arguments = arguments,
                EchoOn = false
            };

            var output = compiler.GetConsoleRunOutput();
            return output;
        }

        private static string DetectNsisMake()
        {
            const string makeNsisExe = "makensis.exe";

            var nsis = Environment.GetEnvironmentVariable("WIXSHARP_NSISDIR");
            if (nsis != null)
            {
                nsis = IO.Path.Combine(nsis, makeNsisExe);
                if (IO.File.Exists(nsis))
                {
                    return nsis;
                }
            }

            // Environment.Is64BitOperatingSystem
            // Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            nsis = Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            nsis = IO.Path.Combine(nsis, IO.Path.Combine("NSIS", makeNsisExe)); // IO.Path.Combine(nsis, "NSIS", makeNsisExe);
            if (IO.File.Exists(nsis))
            {
                return nsis;
            }

            throw new InvalidOperationException("Cannot detect NSIS location.");
        }

        private static void AddVersionKey(IO.TextWriter writer, string name, object value)
        {
            if (value != null)
            {
                writer.WriteLine($"VIAddVersionKey \"{name}\" \"{value}\"");
            }
        }

        private static void AddExecuteCommand(IO.TextWriter writer, string fileName, string arguments, bool createNoWindow, string exitCode)
        {
            var extension = IO.Path.GetExtension(fileName)?.ToUpper() ?? string.Empty;

            bool useShellExecute = false;
            string text = string.Empty;
            switch (extension)
            {
                case ".EXE":
                    text = $"\"$PLUGINSDIR\\{fileName}\"";
                    text = AppendArgument(text, arguments);
                    break;

                case ".MSI":
                    text = $"\"$%WINDIR%\\System32\\msiexec.exe\" /I \"$PLUGINSDIR\\{fileName}\"";
                    text = AppendArgument(text, arguments);
                    break;

                case ".PS1":
                    text = $"\"powershell.exe\" -NoProfile -ExecutionPolicy Bypass -File \"$PLUGINSDIR\\{fileName}\"";
                    text = AppendArgument(text, arguments);
                    break;

                case ".BAT":
                case ".CMD":
                    text = $"\"$%WINDIR%\\System32\\cmd.exe\" /C \"$PLUGINSDIR\\{fileName}\"";
                    text = AppendArgument(text, arguments);
                    break;

                case ".VBS":
                case ".JS":
                    text = $"\"$%WINDIR%\\System32\\wscript.exe\" \"$PLUGINSDIR\\{fileName}\"";
                    text = AppendArgument(text, arguments);
                    break;

                default:
                    useShellExecute = true;
                    break;
            }

            if (useShellExecute)
            {
                text = $"ExecShell \"\" \"$PLUGINSDIR\\{fileName}\"";
                if (arguments != null)
                {
                    text = AppendArgument(text, $"\"{arguments}\"");
                }
                writer.WriteLine(text);
            }
            else if (!createNoWindow)
            {
                text = $"ExecWait '{text}'";
                text = AppendArgument(text, exitCode);
                writer.WriteLine(text);
            }
            else
            {
                text = $"nsExec::Exec '{text}'";
                writer.WriteLine(text);
                writer.WriteLine($"Pop {exitCode}");
            }
        }

        private static void AddFileCommand(IO.StringWriter writer, string fileName)
        {
            writer.WriteLine($@"File ""/oname=$PLUGINSDIR\{IO.Path.GetFileName(fileName)}"" ""{IO.Path.GetFullPath(fileName)}""");
        }

        private static void AddExpandEnvStringsCommand(IO.StringWriter writer, ref string arguments)
        {
            if (!arguments.IsNullOrEmpty())
            {
                writer.WriteLine($"ExpandEnvStrings $R1 '{arguments}'");
                arguments = "$R1";
            }
        }

        private static string ExecutionLevelToString(RequestExecutionLevel level)
        {
            switch (level)
            {
                case RequestExecutionLevel.None:
                    return "none";
                case RequestExecutionLevel.RunAsInvoker:
                    return "user";
                case RequestExecutionLevel.HighestAvailable:
                    return "highest";
                case RequestExecutionLevel.RequireAdministrator:
                    return "admin";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Version ParseNsisVersion(string text)
        {
            var groups = Regex.Matches(text.ToUpperInvariant(), @"v?([\d\.]+)(A|B|RC)?(\d*)?(-.+)?")
                .Cast<Match>()
                .SelectMany(m => m.Groups.Cast<Group>().Select(x => x.Value))
                .ToArray();

            var groupVersion = groups[1];
            var groupStage = groups[2];
            var groupRevision = groups[3];

            var version = !string.IsNullOrEmpty(groupVersion) ? new Version(groupVersion) : new Version();

            // 0 for alpha (status)
            // 1 for beta (status)
            // 2 for release candidate
            // 3 for (final) release
            int revision = 3 << 16; // Release
            if (!string.IsNullOrEmpty(groupStage))
            {
                int number = !string.IsNullOrEmpty(groupRevision) ? int.Parse(groupRevision) : 0;
                switch (groupStage)
                {
                    case "A":
                        revision = number;
                        break;

                    case "B":
                        revision = (1 << 16) + number;
                        break;

                    case "RC":
                        revision = (2 << 16) + number;
                        break;
                }
            }
            return new Version(
                version.Major != -1 ? version.Major : 0,
                version.Minor != -1 ? version.Minor : 0,
                version.Build != -1 ? version.Build : 0,
                revision);
        }

        private static void VerifyMinimumSupportedVersion(string nsisMake)
        {
            const string MinimumSupportedVersion = "3.0b3";

            var output = ExecuteNsisMake(nsisMake, "/VERSION").Trim();
            if (output.IsEmpty())
            {
                throw new InvalidOperationException("Failed to detect the NSIS version.");
            }

            if (ParseNsisVersion(output) < ParseNsisVersion(MinimumSupportedVersion))
            {
                throw new ArgumentOutOfRangeException($"NSIS minimum supported version: \"{MinimumSupportedVersion}\", detected version: \"{output}\".");
            }
        }

        private static string AppendArgument(string s, string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                return string.IsNullOrEmpty(s) ? arg : s + " " + arg;
            }

            return s;
        }

        private void AddSplashScreen(IO.StringWriter writer)
        {
            if (SplashScreen != null)
            {
                AddFileCommand(writer, SplashScreen.FileName);
                writer.WriteLine($@"splash::show {SplashScreen.Delay.TotalMilliseconds} ""$PLUGINSDIR\{IO.Path.GetFileNameWithoutExtension(SplashScreen.FileName)}""");
                // $0 has '1' if the user closed the splash screen early,
                // '0' if everything closed normally, and '-1' if some error occurred.
                writer.WriteLine("Pop $0");
            }
        }
    }
}
