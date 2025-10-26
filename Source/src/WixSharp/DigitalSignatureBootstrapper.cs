using System;

namespace WixSharp
{
    /// <summary>
    /// Container with the parameters of the digital signature of the Bootstrapper
    /// </summary>
    public class DigitalSignatureBootstrapper : DigitalSignature
    {
        /// <summary>
        /// Applies digital signature to a Bootstrapper file
        /// </summary>
        /// <param name="bootstrapperFileToSign">The Bootstrapper file to sign.</param>
        /// <returns>Exit code of the <c>SignTool.exe</c> process.</returns>
        public override int Apply(string bootstrapperFileToSign)
        {
            Compiler.OutputWriteLine($"Signing bootstrapper {bootstrapperFileToSign} with DigitalSignature."); // full path will be printed by the signing tool

            var retValue = CommonTasks.Tasks.DigitalySignBootstrapper(bootstrapperFileToSign, PfxFilePath, TimeUrl?.AbsoluteUri, Password,
                PrepareOptionalArguments(), WellKnownLocations, CertificateStore, OutputLevel, HashAlgorithm);

            Compiler.OutputWriteLine(retValue != 0
                ? $"Could not sign the {bootstrapperFileToSign} bootstrapper file."
                : $"The bootstrapper file {bootstrapperFileToSign} was signed successfully.");

            return retValue;
        }
    }
}