using System;
using System.Diagnostics;
using IO = System.IO;
using Reflection=System.Reflection;

namespace WixSharp.NsisBootstrapper
{
    /// <summary>
    /// Defines native (un-managed) bootstrapper. The bootstrapper is created by the NSIS installer authoring tool.
    /// The path to NSIS installation is detected through the WIXSHARP_NSISDIR environment variable or installation in
    /// the default "%PROGRAMFILES(x86)%\NSIS" location.
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
    ///                        PrerequisiteFile = "C:\Users\Public\Public Downloads\dotnetfx.exe",
    ///                        PrimaryFile = "MyProduct.msi",
    ///                        OutputFile = "setup.exe",
    ///                        PrerequisiteRegKeyValue = @"HKLM:SOFTWARE\Microsoft\.NETFramework:InstallRoot",
    ///
    ///                        IconFile = "app_icon.ico",
    ///
    ///                        VersionInfo = new VersionInformation("1.2.3.4")
    ///                        {
    ///                            ProductName = "Test Application",
    ///                            LegalCopyright = "Copyright Test company",
    ///                            FileDescription = "Test Application",
    ///                            FileVersion = "1.2.3.4",
    ///                            CompanyName = "Test company",
    ///                            InternalName = "setup.exe",
    ///                            OriginalFilename = "setup.exe"
    ///                        }
    ///                    }
    ///                    .Build();
    /// </code>
    /// </example>
    public class NsisBootstrapper
    {
        /// <summary>
        /// Gets or sets the prerequisite file.
        /// </summary>
        /// <value>The prerequisite file.</value>
        public string PrerequisiteFile { get; set; }

        /// <summary>
        /// Gets or sets the primary setup file.
        /// </summary>
        /// <value>The primary setup file.</value>
        public string PrimaryFile { get; set; }

        /// <summary>
        /// Gets or sets the prerequisite registry key value. This value is used to determine if the <see cref="PrerequisiteFile"/> should be launched.
        /// <para>This value must comply with the following pattern: &lt;RegistryHive&gt;:&lt;KeyPath&gt;:&lt;ValueName&gt;.</para>
        /// <code>PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed";</code>
        /// Existence of the specified registry value at runtime is interpreted as an indication that the <see cref="PrerequisiteFile"/> has been already installed.
        /// Thus bootstrapper will execute <see cref="PrimaryFile"/> without launching <see cref="PrerequisiteFile"/> first.
        /// </summary>
        /// <value>The prerequisite registry key value.</value>
        public string PrerequisiteRegKeyValue { get; set; }

        /// <summary>
        /// Gets or sets the output file (bootstrapper) name.
        /// </summary>
        /// <value>The output file name.</value>
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the flag which allows you to disable verification of <see cref="PrerequisiteRegKeyValue"/> after the prerequisite setup is completed.
        /// <para>Normally if <c>bootstrapper</c> checkes if <see cref="PrerequisiteRegKeyValue"/> exists stright after the prerequisite installation and starts
        /// the primary setup only if it does.</para>
        /// <para>It is possible to instruct bootstrapper to continue with the primary setup regardless of the prerequisite installation outcome. This can be done
        /// by setting DoNotPostVerifyPrerequisite to <c>true</c> (default is <c>false</c>)</para>
        /// </summary>
        /// <value>The do not post verify prerequisite.</value>
        public bool DoNotPostVerifyPrerequisite { get; set; }

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

            string regRootKey = null;
            string regSubKey = null;
            string regValueName = null;

            if (PrerequisiteRegKeyValue != null)
            {
                var regKeyTokens = PrerequisiteRegKeyValue.Split(':');

                if (regKeyTokens.Length != 3)
                {
                    throw new ArgumentException($"PrerequisiteRegKeyValue: {PrerequisiteRegKeyValue}.\nThis value must comply with the following pattern: <RegistryHive>:<KeyPath>:<ValueName>.");
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

            using (var file = new IO.StreamWriter(nsiFile))
            {
                file.WriteLine("Unicode true");

                file.WriteLine("!include LogicLib.nsh");
                file.WriteLine("!include x64.nsh");
                file.WriteLine("!include FileFunc.nsh");

                var assembly = Reflection.Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("WixSharp.NsisBootstrapper.macros.nsh"))
                using (var reader = new IO.StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    var result = reader.ReadToEnd();
                    file.WriteLine(result);
                }

                if (IconFile != null)
                {
                    file.WriteLine($"Icon \"{IO.Path.GetFullPath(IconFile)}\"");
                }
                file.WriteLine($"OutFile \"{IO.Path.GetFullPath(OutputFile)}\"");

                file.WriteLine($"RequestExecutionLevel {ExecutionLevelToString(RequestExecutionLevel)}");
                file.WriteLine("SilentInstall silent");

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

                        file.WriteLine($"VIProductVersion \"{version}\"");
                    }

                    AddVersionKey(file, "ProductName", VersionInfo.ProductName);
                    AddVersionKey(file, "Comments", VersionInfo.Comments);
                    AddVersionKey(file, "CompanyName", VersionInfo.CompanyName);
                    AddVersionKey(file, "LegalTrademarks", VersionInfo.LegalTrademarks);
                    AddVersionKey(file, "LegalCopyright", VersionInfo.LegalCopyright);
                    AddVersionKey(file, "FileDescription", VersionInfo.FileDescription);
                    AddVersionKey(file, "FileVersion", VersionInfo.FileVersion);
                    AddVersionKey(file, "ProductVersion", VersionInfo.ProductVersion);
                    AddVersionKey(file, "InternalName", VersionInfo.InternalName);
                    AddVersionKey(file, "OriginalFilename", VersionInfo.OriginalFilename);
                    AddVersionKey(file, "PrivateBuild", VersionInfo.PrivateBuild);
                    AddVersionKey(file, "SpecialBuild", VersionInfo.SpecialBuild);
                }

                file.WriteLine("Function .onInit");

                file.WriteLine("InitPluginsDir");
                file.WriteLine("SetOutPath $PLUGINSDIR");

                if (PrerequisiteFile != null)
                {
                    file.WriteLine($"File \"{IO.Path.GetFullPath(PrerequisiteFile)}\"");
                }
                file.WriteLine($"File \"{IO.Path.GetFullPath(PrimaryFile)}\"");

                if (PrerequisiteFile != null)
                {
                    if (PrerequisiteRegKeyValue != null)
                    {
                        file.WriteLine($"!insertmacro REG_KEY_VALUE_EXISTS {regRootKey} \"{regSubKey}\" \"{regValueName}\"");
                        file.WriteLine("IfErrors 0 install_primary");
                    }

                    AddExecute(file, IO.Path.GetFileName(PrerequisiteFile), null);

                    if (PrerequisiteRegKeyValue != null && !DoNotPostVerifyPrerequisite)
                    {
                        file.WriteLine($"!insertmacro REG_KEY_VALUE_EXISTS {regRootKey} \"{regSubKey}\" \"{regValueName}\"");
                        file.WriteLine("IfErrors end 0");
                    }
                }

                file.WriteLine("install_primary:");
                file.WriteLine("${GetParameters} $R0");
                AddExecute(file, IO.Path.GetFileName(PrimaryFile), "$R0");
                file.WriteLine("goto end");

                file.WriteLine("end:");
                file.WriteLine("RMDir /r /REBOOTOK $PLUGINSDIR");
                file.WriteLine("FunctionEnd");

                file.WriteLine("Section");
                file.WriteLine("SectionEnd");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = nsisMake,
                Arguments = $"/INPUTCHARSET UTF8 {nsiFile} {OptionalArguments}",

                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            var process = Process.Start(startInfo);
            if (process != null)
            {
                process.WaitForExit();
                Console.WriteLine(process.StandardOutput.ReadToEnd());
            }

            if (!IO.File.Exists(OutputFile))
            {
                return null;
            }

            IO.File.Delete(nsiFile);
            return OutputFile;
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

            nsis = Environment.GetEnvironmentVariable("%PROGRAMFILESX86%"); // Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            if (nsis != null)
            {
                nsis = IO.Path.Combine(nsis, IO.Path.Combine("NSIS", makeNsisExe)); // IO.Path.Combine(nsis, "NSIS", makeNsisExe);
                if (IO.File.Exists(nsis))
                {
                    return nsis;
                }
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

        private static void AddExecute(IO.TextWriter writer, string fileName, string arguments)
        {
            var extension = IO.Path.GetExtension(fileName)?.ToUpper() ?? string.Empty;

            switch (extension)
            {
                case ".EXE":
                    writer.WriteLine($"ExecWait '\"$PLUGINSDIR\\{fileName}\" {arguments}'");
                    break;

                case ".MSI":
                    writer.WriteLine($"ExecWait '\"$%WINDIR%\\System32\\msiexec.exe\" /I \"$PLUGINSDIR\\{fileName}\" {arguments}'");
                    break;

                default:
                    writer.WriteLine($"ExecShell \"open\" '\"$PLUGINSDIR\\{fileName}\" {arguments}'");
                    break;
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
    }
}
