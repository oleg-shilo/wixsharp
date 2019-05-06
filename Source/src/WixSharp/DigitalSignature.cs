using System;
using System.Security;

namespace WixSharp
{
    /// <summary>
    /// Container with the parameters of the digital signature
    /// </summary>
    public class DigitalSignature
    {
        private SecureString _password;

        /// <summary>Specify the signing certificate in a file. If this file is a PFX with a password, the password may be supplied
        /// with the <see cref="Password"/> property.
        /// </summary>
        public string PfxFilePath { get; set; }

        /// <summary>The timestamp server's URL. If this option is not present (pass to null), the signed file will not be timestamped.
        /// A warning is generated if timestamping fails.
        /// </summary>
        public Uri TimeUrl { get; set; }

        /// <summary>
        /// The password to use when opening the PFX file. Should be <c>null</c> if no password required.
        /// </summary>
        public string Password
        {
            get { return _password?.ToInsecureString(); }
            set { _password = value?.ToSecureString(); }
        }

        /// <summary>
        /// Description of the signed content.
        /// Is passed to the /d parameter of the <c>SignTool.exe</c>
        /// UAC uses this Description when asks the user about user rights elevation.
        /// UAC uses temporary file name instead if no Description provided.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Extra arguments to pass to the <c>SignTool.exe</c> utility.
        /// </summary>
        public string OptionalArguments { get; set; }

        /// <summary>
        /// The optional ';' separated list of directories where SignTool.exe can be located.
        /// If this parameter is not specified WixSharp will try to locate the SignTool in the built-in well-known locations (system PATH)
        /// </summary>
        public string WellKnownLocations { get; set; }

        /// <summary>
        /// A flag indicating if the value of <see cref="WixSharp.DigitalSignature.PfxFilePath"/> is a name of the subject of the signing certificate
        /// from the certificate store (as opposite to the certificate file). This value can be a substring of the entire subject name.
        /// </summary>
        public bool UseCertificateStore { get; set; }

        /// <summary>
        /// A flag indicating the output level of the <c>SignTool.exe</c> utility.
        /// </summary>
        public SignOutputLevel OutputLevel { get; set; }

        /// <summary>
        /// Applies digital signature to a file
        /// </summary>
        /// <param name="fileToSign">The file to sign.</param>
        /// <returns>Exit code of the <c>SignTool.exe</c> process.</returns>
        public virtual int Apply(string fileToSign)
        {
            var retValue = CommonTasks.Tasks.DigitalySign(fileToSign, PfxFilePath, TimeUrl?.AbsoluteUri, Password,
                PrepareOptionalArguments(), WellKnownLocations, UseCertificateStore, false, OutputLevel);

            Console.WriteLine(retValue != 0
                ? $"Error: Could not sign the {fileToSign} file."
                : $"The file {fileToSign} was signed successfully.");

            return retValue;
        }

        /// <summary>
        /// Preparing optional arguments by adding custom arguments with current signer specific
        /// </summary>
        /// <returns>Final version of the optional arguments</returns>
        protected virtual string PrepareOptionalArguments()
        {
            if (Description.IsNullOrEmpty())
                return OptionalArguments;

            return $"{OptionalArguments} /d \"{Description}\"";
        }
    }
}