using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WixSharp.Nsis.WinVer
{
    /// <summary>
    /// Adds to Nsis bootstrapper a block of code that compares current windows version with a set of specified versions.
    /// If version is not supported, shows an error MessageBox with a specified or default <see cref="ErrorMessage"/>.
    /// Also, can <see cref="TerminateInstallation"/> after showing error message (default = true).
    /// <example>
    /// <code>
    /// bootstrapper.OSValidation.MinVersion = WindowsVersionNumber._7;
    /// bootstrapper.OSValidation.UnsupportedVersions.Add(new WindowsVersion(WindowsVersionNumber._7, 0));
    /// bootstrapper.OSValidation.UnsupportedVersions.Add(new WindowsVersion(WindowsVersionNumber._8));
    /// </code>
    /// </example>
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class OSValidation
    {
        private const string DefaultErrorText = "This operating system is not supported.$\\nPlease, update your OS.";

        /// <summary>
        ///     Initializes a new instance of NotSupportedWindowsVersions
        /// </summary>
        internal OSValidation()
        {
        }

        /// <summary>
        /// Specifies an error message that is shown when OS is not supported
        /// <para></para>
        /// If not set, the following error message is shown:
        /// <para></para>
        /// This operating system is not supported.$\\nPlease, update your OS.
        /// </summary>
        public string ErrorMessage { get; set; } = DefaultErrorText;

        /// <summary>
        /// Terminate installation if current OS version is not supported. Default = true
        /// </summary>
        public bool TerminateInstallation { get; set; } = true;

        /// <summary>
        /// Minimal required windows version
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public WindowsVersionNumber? MinVersion { get; set; }

        /// <summary>
        /// Optional windows versions that are not supported
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once CollectionNeverUpdated.Global
        public IList<WindowsVersion> UnsupportedVersions { get; } = new List<WindowsVersion>();

        internal bool Any => MinVersion.HasValue || UnsupportedVersions.Any();

        internal string BuildVersionCheckScriptPart()
        {
            if (!Any)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                ConstructScript(writer);
                return writer.ToString();
            }
        }

        private void ConstructScript(StringWriter writer)
        {
            WriteMinVersion(writer);

            WriteUnsupportedVersions(writer);

            WriteMessageBox(writer);

            if (TerminateInstallation)
            {
                writer.WriteLine("goto end");
            }

            writer.WriteLine(MinVersion.HasValue ? "${EndUnless}" : "${EndIf}");
        }

        private void WriteMinVersion(StringWriter writer)
        {
            if (!MinVersion.HasValue)
            {
                writer.Write("${If} ");
                return;
            }

            writer.Write("${Unless} ${AtLeastWin");
            var minVersion = MinVersion.Value;
            writer.WriteLine($"{minVersion.GetDescription()}}}");

            if (UnsupportedVersions.Any())
            {
                writer.Write("${OrIf} ");
            }
        }

        private void WriteUnsupportedVersions(StringWriter writer)
        {
            for (var i = 0; i < UnsupportedVersions.Count; i++)
            {
                if (i > 0)
                {
                    writer.Write("${OrIf} ");
                }

                var unsupportedVersion = UnsupportedVersions[i];

                writer.Write("${IsWin");

                string winVersion = unsupportedVersion.VersionNumber.GetDescription();
                writer.WriteLine($"{winVersion}}}");

                if (unsupportedVersion.ServicePack != -1)
                {
                    writer.WriteLine($"${{AndIf}} ${{IsServicePack}} {unsupportedVersion.ServicePack}");
                }
            }
        }

        private void WriteMessageBox(StringWriter writer)
        {
            writer.WriteLine("!define MB_OK 0x00000000");
            writer.WriteLine("!define MB_ICONERROR 0x00000010");
            writer.WriteLine(
                $"System::Call 'USER32::MessageBox(i $hwndparent, t \"{ErrorMessage}\", t \"Error\", i ${{MB_OK}}|${{MB_ICONERROR}})i'");
        }
    }
}