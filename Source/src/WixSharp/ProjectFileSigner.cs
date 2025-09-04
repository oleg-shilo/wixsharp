using System;
using System.IO;
using System.Linq;
using WixSharp.Utilities;

using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Options for configuring the behavior of the file signing process.
    /// </summary>
    public class SignAllFilesOptions
    {
        /// <summary>
        /// Gets or sets the supported file formats for signing.
        /// </summary>
        public string[] SupportedFileFormats { get; set; } =
        {
            ".dll", ".exe", ".cab", ".msi"
        };

        /// <summary>
        /// Determines whether to skip signing already signed files.
        /// </summary>
        public bool SkipSignedFiles { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether sign WixSharp own embedded assemblies. IE, WixSharp.UI.dll.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should sign the embedded assemblies; otherwise, <c>false</c>.
        /// </value>
        public bool SignEmbeddedAssemblies { get; set; } = true;

        /// <summary>
        /// Determines the behavior when an exception occurs during the signing of a specific file.
        /// <para>For example, <see cref="ExceptionHandler"/> can be used to skip and continue the process in DEBUG mode
        /// when it is not critical that all files are signed successfully.</para>
        /// </summary>
        public Action<string, Exception> ExceptionHandler { get; set; } = (file, ex) => throw ex;
    }

    /// <summary>
    /// Provides functionality to sign all files in a project.
    /// </summary>
    static class ProjectFileSigner
    {
        /// <summary>
        /// Signs all files in the specified project that match the given options.
        /// </summary>
        /// <param name="project">The project whose files are to be signed.</param>
        /// <param name="options">The options to configure the signing process.</param>
        public static void SignAllFiles(Project project, SignAllFilesOptions options)
        {
            if (project.DigitalSignature == null)
                return;

            // Resolve wildcards
            project.ResolveWildCards(Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories);

            // Sign all files
            foreach (var file in project.AllFiles)
            {
                try
                {
                    if (file.Name.IsNullOrEmpty())
                        continue;

                    var filePath = Utils.PathCombine(project.SourceBaseDir, file.Name);

                    if (!IO.File.Exists(filePath))
                        continue;

                    // Skip if file is not supported
                    if (!options.SupportedFileFormats.Contains(Path.GetExtension(filePath).ToLowerInvariant()))
                        continue;

                    // Skip if file is already signed
                    if (options.SkipSignedFiles && VerifyFileSignature.IsSigned(filePath))
                        continue;

                    project.DigitalSignature?.Apply(filePath);
                }
                catch (Exception ex)
                {
                    options.ExceptionHandler?.Invoke(file.Name, ex);
                }
            }
        }
    }
}