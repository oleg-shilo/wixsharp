// Ignore Spelling: Wix

using System;
using System.Security.Cryptography;

namespace WixSharp
{
    /// <summary>
    /// Container with the parameters of the digital signature of the Bootstrapper.
    /// <p>It differs from its base class <see cref="DigitalSignature"/> as it can detect the bootstrapper file
    /// and detach the embedded bootstrapper engine, sign and reattach it before signing the file.</p>
    /// </summary>
    public class DigitalSignatureBootstrapper : DigitalSignature
    {
        /// <summary>
        /// Applies digital signature to a Bootstrapper files. If the file is a bootstrapper file then the embedded 
        /// engine will be signed first and then reattached to the bootstrapper. Otherwise, normal payloads will be signed as usual.
        /// </summary>
        /// <param name="fileToSign">The file to sign. The file can be an ordinary payload or a bootstrapper file.</param>
        /// <returns>Exit code of the <c>SignTool.exe</c> process.</returns>
        public override int Apply(string fileToSign)
        {
            // A bootstrapper file needs signing the embedded engine first and only then it's OK to
            // proceed with the signed engine reattached.
            // Note, this routine can also be used for signing ordinary payload files as well.
            if (fileToSign.IsBootstrapperFile())
            {
                Compiler.OutputWriteLine?.Invoke($"Signing the bootstrapper engine detached from {fileToSign.PathGetFileName()}...");

                CommonTasks.Tasks.DigitallySignBootstrapperEngine(
                    fileToSign, PfxFilePath, TimeUrl?.AbsoluteUri, Password,
                    PrepareOptionalArguments(), WellKnownLocations, CertificateStore, OutputLevel, HashAlgorithm)
                    .CheckSigningResult(fileToSign);

                fileToSign.WaitUntilFileUnlocked(); // antivirus may lock the file for a while after engine reattachment
            }

            return base.Apply(fileToSign);
        }
    }
}