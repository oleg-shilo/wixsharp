#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

Permission is hereby granted,
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#endregion Licence...

using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// </summary>
    /// <example>
    /// <code>
    /// new IISVirtualDir
    /// {
    ///     Name = "MyWebApp3",
    ///     AppName = "Test3",
    ///     WebSite = new WebSite("NewSite3", "*:8083")
    ///     {
    ///         InstallWebSite = true,
    ///         Certificate = new IISCertificate
    ///         {
    ///             File = "MyServer.cert.pfx",
    ///             Name = "MyServerCert",
    ///             StoreLocation = IISCertificate.CertificateStoreLocation.localMachine,
    ///             StoreName = IISCertificate.CertificateStoreName.personal,
    ///             PFXPassword = "password123",
    ///             Request = false,
    ///             Overwrite = false
    ///         }
    ///     },
    ///     WebAppPool = new WebAppPool("MyWebApp3", "Identity=applicationPoolIdentity")
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="WixSharp.WixEntity" />
    public class IISCertificate : WixEntity
    {
        /// <summary>
        /// This attribute's value must be one of the following.
        /// </summary>
        public enum CertificateStoreLocation
        {
            currentUser,
            localMachine
        }

        public enum CertificateStoreName
        {
            /// <summary>
            /// Contains the certificates of certificate authorities that the user trusts to issue certificates to others. Certificates in these stores are normally supplied with the operating system or by the user's network administrator.
            /// </summary>
            ca,

            /// <summary>
            /// Use the "personal" value instead.
            /// </summary>
            my,

            /// <summary>
            /// Contains personal certificates. These certificates will usually have an associated private key. This store is often referred to as the "MY" certificate store.
            /// </summary>
            personal,

            request,

            /// <summary>
            /// Contains the certificates of certificate authorities that the user trusts to issue certificates to others. Certificates in these stores are normally supplied with the operating system or by the user's network administrator. Certificates in this store are typically self-signed.
            /// </summary>
            root,

            /// <summary>
            /// Contains the certificates of those that the user normally sends enveloped messages to or receives signed messages from.See MSDN documentation for more information.
            /// </summary>
            otherPeople,

            /// <summary>
            /// Contains the certificates of those directly trusted people and resources.See MSDN documentation for more information.
            /// </summary>
            trustedPeople,

            /// <summary>
            /// Contains the certificates of those publishers who are trusted. See MSDN documentation for more information.
            /// </summary>
            trustedPublisher
        }

        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixEntity"/>.
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para>
        /// <para>If the <see cref="Id"/> value is not specified explicitly by the user the Wix# compiler
        /// generates it automatically insuring its uniqueness.</para>
        /// <remarks>
        ///  Note: The ID auto-generation is triggered on the first access (evaluation) and in order to make the id
        ///  allocation deterministic the compiler resets ID generator just before the build starts. However if you
        ///  accessing any auto-id before the Build*() is called you can it interferes with the ID auto generation and eventually
        ///  lead to the WiX ID duplications. To prevent this from happening either:
        ///  <para> - Avoid evaluating the auto-generated IDs values before the call to Build*()</para>
        ///  <para> - Set the IDs (to be evaluated) explicitly</para>
        ///  <para> - Prevent resetting auto-ID generator by setting WixEntity.DoNotResetIdGenerator to true";</para>
        /// </remarks>
        /// </summary>
        /// <value>The id.</value>
        [Xml]
        public new string Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        /// <summary>
        /// Reference to a Binary element that will store the certificate as a stream inside the package. This attribute cannot be specified with the
        /// CertificatePath attribute.
        /// <para><see cref="IISCertificate.BinaryKey"/> cannot be used if property <see cref="IISCertificate.File"/> is set.</para>
        /// </summary>
        [Xml]
        public string BinaryKey;

        /// <summary>
        /// The location of the certificate file.
        /// <para>In a canonical WiX use-case the user must add a certificate file as a <c>Binary</c> XML element and then reference the element Id as
        /// the <c>BinaryKey</c> attribute of the <c>Certificate</c> element. However with WixSharp you can simply assing <see cref="IISCertificate.File"/>
        /// property to the certificalte file path and the compiler will automatically insert the required <c>Binary</c> element and reference it in all
        /// required places in the XML document.
        /// </para>
        /// <para><see cref="IISCertificate.File"/> cannot be used if property <see cref="IISCertificate.BinaryKey"/> is set.</para>
        /// </summary>
        public string File;

        /// <summary>
        /// If the Request attribute is "no" then this attribute is the path to the certificate file outside of the package. If the Request attribute is "yes" then this atribute
        /// is the certificate authority to request the certificate from. This attribute may be set via a formatted Property (e.g. [MyProperty]).
        /// </summary>
        [Xml]
        public string CertificatePath;

        /// <summary>
        /// Name of the certificate that will be installed or uninstalled in the specified store. This attribute may be set via a formatted Property (e.g. [MyProperty]).
        /// </summary>
        [Xml]
        public new string Name
        {
            get => base.Name;
            set => base.Name = value;
        }

        [Xml]
        public bool? Overwrite;

        /// <summary>
        /// If the Binary stream or path to the file outside of the package is a password protected PFX file, the password for that PFX must be specified here.
        /// This attribute may be set via a formatted Property (e.g. [MyProperty]).
        /// </summary>
        [Xml]
        public string PFXPassword;

        /// <summary>
        /// This attribute controls whether the CertificatePath attribute is a path to a certificate file (Request='no') or the certificate authority to request
        /// the certificate from (Request='yes').
        /// </summary>
        [Xml]
        public bool? Request;

        [Xml]
        public CertificateStoreLocation StoreLocation;

        [Xml]
        public CertificateStoreName StoreName;
    }
}