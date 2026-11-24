using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                var message =
                    $"Signing at least one of the signable files failed. Return code: {result}. Check the output for errors. " +
                    $"You may enable verbose output of signing tools with `Compiler.VerboseSigningOutput = true`";

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
        internal static void SignBootstrapperEngine(this IDigitalSignature digitalSignature, string fileToSign)
        {
            // A bootstrapper file needs signing the embedded engine first and only then it's OK to
            // proceed with the signed engine reattached.
            // Note, this routine can also be used for signing ordinary payload files as well.
            if (fileToSign.IsBootstrapperFile())
            {
                Compiler.OutputWriteLine?.Invoke($"Signing the bootstrapper engine detached from {fileToSign.PathGetFileName()}...");

                CommonTasks.Tasks.DigitallySignBootstrapperEngine(digitalSignature, fileToSign)
                    .CheckSigningResult(fileToSign);
            }
        }
    }

    /// <summary>
    /// Contains signing status and engine signature location information for a Burn bundle.
    /// </summary>
    public class BurnSignatureInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BurnSignatureInfo"/> structure.
        /// </summary>
        /// <param name="isBurnBundle">true if the file is a Burn bundle; otherwise, false.</param>
        /// <param name="engineSigned">true if the Burn engine is signed; otherwise, false.</param>
        /// <param name="bundleSigned">true if the bundle is signed; otherwise, false.</param>
        public BurnSignatureInfo(bool isBurnBundle, bool engineSigned, bool bundleSigned)
        {
            this.IsBurnBundle = isBurnBundle;
            this.EngineSigned = engineSigned;
            this.BundleSigned = bundleSigned;
        }

        /// <summary>
        /// Indicates whether the bundle is a Burn bundle.
        /// </summary>
        public bool IsBurnBundle;

        /// <summary>
        /// Indicates whether the engine is signed.
        /// </summary>
        public bool EngineSigned;

        /// <summary>
        /// Indicates whether the bundle is signed.
        /// </summary>
        public bool BundleSigned;
    }

    /// <summary>
    /// Provides inspection of WiX Burn bundles for engine and bundle signatures.
    /// </summary>
    /// <remarks>Reads the bundle's PE headers and WiX Burn section to determine whether the engine and bundle
    /// contain valid signatures.</remarks>
    public static class BurnEngineInspector
    {
        const uint BurnMagic = 0x00F14300;

        /// <summary>
        /// Inspects the specified bundle file for Burn engine and bundle signatures.
        /// </summary>
        public static BurnSignatureInfo InspectBundle(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader br = new BinaryReader(fs, Encoding.ASCII);

                // --- DOS header ---
                if (fs.Length < 0x40 || br.ReadUInt16() != 0x5A4D) // "MZ"
                    throw new InvalidDataException("Not a PE file.");
                fs.Position = 0x3C;
                uint peOffset = br.ReadUInt32();

                // --- PE signature + COFF header ---
                fs.Position = peOffset;
                if (br.ReadUInt32() != 0x00004550) // "PE\0\0"
                    throw new InvalidDataException("Invalid PE signature.");

                fs.Position = peOffset + 6;
                ushort numberOfSections = br.ReadUInt16();
                fs.Position = peOffset + 20;
                ushort sizeOfOptionalHeader = br.ReadUInt16();

                // --- Optional header: Security (certificate) data directory ---
                long optionalHeaderStart = peOffset + 24;
                fs.Position = optionalHeaderStart;
                ushort magic = br.ReadUInt16();
                long dataDirStart = optionalHeaderStart + (magic == 0x20B ? 112 : 96);
                long securityDirPos = dataDirStart + 4 * 8; // index 4 = IMAGE_DIRECTORY_ENTRY_SECURITY

                fs.Position = securityDirPos;
                uint bundleCertOffset = br.ReadUInt32(); // FILE offset for this directory
                uint bundleCertSize = br.ReadUInt32();
                bool bundleSigned = bundleCertOffset != 0 && bundleCertSize != 0;

                // --- Section table: look for ".wixburn" ---
                long sectionTable = optionalHeaderStart + sizeOfOptionalHeader;
                uint burnRawOffset = 0;
                bool foundBurn = false;

                for (int i = 0; i < numberOfSections; i++)
                {
                    long sectionPos = sectionTable + (long)i * 40;
                    fs.Position = sectionPos;
                    string name = Encoding.ASCII.GetString(br.ReadBytes(8)).TrimEnd('\0');
                    if (name != ".wixburn")
                        continue;

                    fs.Position = sectionPos + 20; // PointerToRawData
                    burnRawOffset = br.ReadUInt32();
                    foundBurn = true;
                    break;
                }

                if (!foundBurn)
                    return new BurnSignatureInfo(false, false, bundleSigned);

                // --- BURN_SECTION_HEADER ---
                fs.Position = burnRawOffset;
                uint burnMagic = br.ReadUInt32();
                br.ReadUInt32();                 // version
                br.ReadBytes(16);                // bundle GUID
                br.ReadUInt32();                 // stub size
                br.ReadUInt32();                 // original checksum
                uint origSigOffset = br.ReadUInt32();
                uint origSigSize = br.ReadUInt32();

                if (burnMagic != BurnMagic)
                    return new BurnSignatureInfo(false, false, bundleSigned);

                bool engineSigned = origSigOffset != 0 && origSigSize != 0;

                // Sanity check: does the recorded blob look like a WIN_CERTIFICATE?
                if (engineSigned)
                {
                    if ((long)origSigOffset + origSigSize > fs.Length)
                    {
                        engineSigned = false;
                    }
                    else
                    {
                        fs.Position = origSigOffset;
                        uint dwLength = br.ReadUInt32();
                        ushort revision = br.ReadUInt16();
                        ushort certType = br.ReadUInt16();
                        engineSigned = dwLength > 8 && dwLength <= origSigSize
                                       && revision == 0x0200   // WIN_CERT_REVISION_2_0
                                       && certType == 0x0002;  // WIN_CERT_TYPE_PKCS_SIGNED_DATA
                    }
                }

                return new BurnSignatureInfo(true, engineSigned, bundleSigned);
            }
        }
    }

}