using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;

namespace WixSharp
{
    /// <summary>
    /// Container for signing context as well as the implementation of the signing algotrithm (<see cref="IDigitalSignature.Apply(string)"/> ).
    /// </summary>
    public interface IDigitalSignature
    {
        /// <summary>
        /// Applies digital signature to a file
        /// </summary>
        /// <param name="fileToSign">The file to sign.</param>
        /// <returns>Exit code of the signing tool.</returns>
        int Apply(string fileToSign);
    }

    /// <summary>
    /// Container with the parameters of the digital signature
    /// </summary>
    public class DigitalSignature1 : IDigitalSignature
    {
        private SecureString _password;

        /// <summary>Specify the signing certificate in a file. If this file is a PFX with a password, the password may be supplied
        /// with the <see cref="Password"/> property.
        /// </summary>
        public string PfxFilePath
        {
            get => CertificateId;
            set => CertificateId = value;
        }

        /// <summary>
        /// The identifier used to locate the certificate
        /// </summary>
        public string CertificateId { get; set; }

        /// <summary>
        /// Selects the hash algorithm to apply. Default sha1
        /// </summary>
        public HashAlgorithmType HashAlgorithm { get; set; } = HashAlgorithmType.sha256;

        /// <summary>The timestamp server's URL. If this option is not present (pass to null), the signed file will not be timestamped.
        /// A warning is generated if timestamping fails.
        /// </summary>
        public Uri TimeUrl
        {
            get => TimeUrls.FirstOrDefault();

            set
            {
                TimeUrls.Clear();
                if (value != null)
                    TimeUrls.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the time server URLs.
        /// <para>This list of urls is useful if you want to try multiple time servers in case some of them is failing.
        /// </para>
        /// <para>
        /// You can customize the way the servers are accessed by signing tool. Use <see cref="DigitalSignature.MaxTimeUrlRetry"/>
        /// and <see cref="DigitalSignature.UrlRetrySleep"/> for that.</para>
        /// </summary>
        /// <value>
        /// The time server urls.
        /// </value>
        public List<Uri> TimeUrls { get; set; } = new List<Uri>();

        /// <summary>
        /// Gets or sets the maximum count of retrying an individual time server from <see cref="DigitalSignature.TimeUrls"/>.
        /// </summary>
        /// <value>
        /// The maximum time URL retry.
        /// </value>
        public int MaxTimeUrlRetry { get; set; } = 3;

        /// <summary>
        /// Gets or sets the delay (in milliseconds) between retries when accessing an individual time server from <see cref="DigitalSignature.TimeUrls"/>.
        /// </summary>
        /// <value>
        /// The URL retry sleep.
        /// </value>
        public int UrlRetrySleep { get; set; } = 500;

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
        public bool UseCertificateStore
        {
            get => CertificateStore == StoreType.commonName || CertificateStore == StoreType.sha1Hash;
            set => CertificateStore = value ? StoreType.commonName : StoreType.file;
        }

        /// <summary>
        /// Where to read the certificate from.
        /// </summary>
        public StoreType CertificateStore { get; set; } = StoreType.file;

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
            int retValue = -1;

            int apply(string url) =>
                CommonTasks.Tasks.DigitalySign(fileToSign, CertificateId, TimeUrl?.AbsoluteUri, Password,
                                               PrepareOptionalArguments(), WellKnownLocations, CertificateStore, OutputLevel, HashAlgorithm);

            Console.WriteLine("Signing with DigitasSignature");

            if (TimeUrls.Any())
                foreach (Uri uri in TimeUrls)
                {
                    for (int i = 0; i < MaxTimeUrlRetry && retValue != 0; i++)
                    {
                        retValue = apply(uri?.AbsoluteUri);
                        Console.WriteLine("Retrying applying DigitalSignature");
                        Thread.Sleep(UrlRetrySleep);
                    }

                    if (retValue == 0)
                        break;
                }
            else
                retValue = apply(null);

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