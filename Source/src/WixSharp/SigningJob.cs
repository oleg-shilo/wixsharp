using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WixSharp.Bootstrapper;
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
        /// Gets or sets a value indicating whether sign WixSharp own embedded assemblies. IE, WixSharp.UI.dll or
        /// native dll with the custom action (<c>*.CA.dll</c>).
        /// <p>Note, signing embedded assemblies have some logical constrains. IE you assembly being executed cannot be
        /// signed as it is locked by its own process. Thus you should either skip signing such assemblies form the
        /// <see cref="IDigitalSignature.Apply(string)"/> method, or sign the locked assemblies before you build the msi file.
        /// This will work because WixSharp detect already signed assemblies and gracefully skip them.</p>
        /// </summary>
        /// <value>
        ///   <c>true</c> if should sign the embedded assemblies; otherwise, <c>false</c>.
        /// </value>
        public bool SignEmbeddedAssemblies { get; set; } = false;

        /// <summary>
        /// Determines the behavior when an exception occurs during the signing of a specific file.
        /// <para>For example, <see cref="ExceptionHandler"/> can be used to skip and continue the process in DEBUG mode
        /// when it is not critical that all files are signed successfully.</para>
        /// </summary>
        public Action<string, Exception> ExceptionHandler { get; set; } = (file, ex) => throw ex;
    }

    /// <summary>
    /// Orchestrates signing files. It uses <see cref="IDigitalSignature"/> and <see cref="ISigningTool"/> for executing 
    /// the signing operation. It also handles the signable files being locked by signing the isolated copies of these files and 
    /// returning the location of the signed copy instead of the original file in such cases.
    /// </summary>
    public static class SigningJob
    {
        /// <summary>
        /// Signs eligible files using the specified digital signature.
        /// </summary>
        /// <param name="signature">The digital signature to apply. If <see langword="null"/>, no files are signed.</param>
        /// <param name="filesToSign">The file paths to consider for signing.</param>
        /// <returns>A mapping from each eligible file path to the path that was signed. Locked files may be isolated to a
        /// different path.</returns>
        public static Dictionary<string, string> SignFiles(this IDigitalSignature signature, params string[] filesToSign)
          => SignFiles(signature, filesToSign, null);
        /// <summary>
        /// Signs eligible files using the specified digital signature.
        /// </summary>
        /// <param name="signature">The digital signature to apply. If <see langword="null"/>, no files are signed.</param>
        /// <param name="filesToSign">The file paths to consider for signing.</param>
        /// <param name="options">Options that control file selection and error handling. If <see langword="null"/>, the compiler's default
        /// options are used.</param>
        /// <returns>A mapping from each eligible file path to the path that was signed. Locked files may be isolated to a
        /// different path.</returns>
        public static Dictionary<string, string> SignFiles(this IDigitalSignature signature, string[] filesToSign, SignAllFilesOptions options)
        {
            var actualFilesToSign = new Dictionary<string, string>();

            if (signature == null)
                return actualFilesToSign;

            options = options ?? Compiler.SignAllFilesOptions;


            // Sign all files
            foreach (var file in filesToSign)
            {
                try
                {
                    var filePath = file;

                    if (!filePath.FileExists())
                        continue;

                    // Skip if file is not supported
                    var fileExt = Path.GetExtension(filePath).ToLower();
                    var fileFormat = fileExt.TrimStart('.');

                    if (!options.SupportedFileFormats.Any(x => x.IsOneOf(fileExt, fileFormat)))
                        continue;

                    // Skip if file is already signed
                    if (options.SkipSignedFiles && VerifyFileSignature.IsSigned(filePath))
                        continue;

                    if (filePath.IsFileLocked())
                    {
                        var isolatedFile = Utils.IsolateFile(filePath);

                        var newName = isolatedFile;

                        Compiler.OutputWriteLine($"Payload file {file} is locked. Isolating it...");
                        actualFilesToSign[filePath] = isolatedFile;
                    }
                    else
                        actualFilesToSign[filePath] = filePath;
                }
                catch (Exception ex)
                {
                    options.ExceptionHandler?.Invoke(file, ex);
                }
            }

            if (actualFilesToSign.Any())
            {
                if (signature is ISigningTool tool && WixSharp.Compiler.AutoGeneration.PreferBatchSigning)
                {
                    tool.SignAndCheck(actualFilesToSign.Values.ToArray());
                }
                else
                {
                    foreach (var file in actualFilesToSign.Values)
                        signature.ApplyAndCheck(file);
                }
            }

            return actualFilesToSign;
        }

        internal static void CheckSigningResult(this int result, params string[] filesToSign)
        {
            if (result != 0)
            {
                var message = $"Signing at least one of the signable files failed. Return code: {result}. Check the output for errors.";
                if (filesToSign.Count() == 1)
                    message = $"Signing the file '{filesToSign.FirstOrDefault()}' failed. Return code: {result}.";

                throw new InvalidOperationException(message);
            }
        }

        internal static void ApplyAndCheck(this IDigitalSignature signature, string fileToSign)
        {
            var result = signature?.Apply(fileToSign);
            result?.CheckSigningResult(fileToSign);
        }

        internal static void SignAndCheck(this ISigningTool tool, string[] filesToSign)
        {
            var result = tool?.Sign(filesToSign);
            result?.CheckSigningResult(filesToSign);
        }

        /// <summary>
        /// Signs all files in the specified project that match the given options.
        /// </summary>
        /// <param name="project">The project whose files are to be signed.</param>
        /// <param name="options">The options to configure the signing process.</param>
        internal static void SignAllFiles(Project project, SignAllFilesOptions options)
        {
            if (project.DigitalSignature == null)
                return;

            project.ResolveWildCards(Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories);

            var actualFilesToSign = new List<string>();

            var filesToSign = project.AllFiles
                .Where(x => !x.Name.IsNullOrEmpty())
                .Select(x => new
                {
                    File = x,
                    OriginalPath = Utils.PathCombine(project.SourceBaseDir, x.Name)
                });



            var fileLocations = filesToSign.Select(x => x.OriginalPath).ToArray();

            var result = project.DigitalSignature.SignFiles(fileLocations, options);

            foreach (var item in filesToSign)
            {
                if (result.ContainsKey(item.OriginalPath))                  // the file may not be signed (e.g. bc it's already signed)
                {
                    var newPath = result[item.OriginalPath];
                    if (newPath != item.OriginalPath)                       // was locked and had to be isolated, so needs to be remapped
                    {
                        if (!item.File.Name.IsAbsolutePath())
                            newPath = newPath.MakeRelativeTo(project.SourceBaseDir);
                        item.File.MapTo(newPath);
                    }
                }
            }
        }

        /// <summary>
        /// Signs all files in the specified project that match the given options.
        /// </summary>
        /// <param name="project">The project whose files are to be signed.</param>
        /// <param name="options">The options to configure the signing process.</param>
        internal static void SignApplicationAndPayloads(Bundle project, SignAllFilesOptions options = null)
        {
            if (project.DigitalSignature == null)
                return;

            if (!project.SignApplicationFiles)
                return;

            var actualFilesToSign = new List<string>();

            var filesToSign = project.Application.Payloads
                .Where(x => !x.SourceFile.IsNullOrEmpty())
                .Select(x => new
                {
                    Payload = x,
                    OriginalPath = Utils.PathCombine(project.SourceBaseDir, x.SourceFile)
                });

            var fileLocations = filesToSign.Select(x => x.OriginalPath).ToArray();

            var result = project.DigitalSignature.SignFiles(fileLocations, options);

            foreach (var item in filesToSign)
            {
                if (result.ContainsKey(item.OriginalPath))                  // the file may not be signed (e.g. bc it's already signed)
                {
                    var newPath = result[item.OriginalPath];
                    if (newPath != item.OriginalPath)                       // was locked and had to be isolated, so needs to be remapped
                    {
                        if (!item.Payload.SourceFile.IsAbsolutePath())
                            newPath = newPath.MakeRelativeTo(project.SourceBaseDir);
                        item.Payload.MapTo(newPath);
                    }
                }
            }
        }

    }
}