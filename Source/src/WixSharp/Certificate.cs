using System;
using System.Xml.Linq;

namespace WixSharp
{
    //http://www.davidwhitney.co.uk/content/blog/index.php/2009/02/11/installing-certificates-using-wix-windows-installer-xml-voltive/
    //http://stackoverflow.com/questions/860996/wix-and-certificates-in-iis
    /// <summary>
    /// This class defines website certificate attributes. It is a close equivalent of Certificate WiX element.
    /// </summary>
    public class Certificate : WixEntity
    {
        #region constructors

        /// <summary>
        /// Creates an instance of Certificate
        /// </summary>
        public Certificate()
        {
        }

        /// <summary>
        /// Creates an instance of Certificate where the certificate is a binary resource
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="binaryKey">The binary key.</param>
        /// <exception cref="ArgumentNullException">
        /// name;name is a null reference or empty
        /// or
        /// binaryKey;binaryKey is a null reference or empty
        /// </exception>
        public Certificate(string name, StoreLocation storeLocation, StoreName storeName, string binaryKey)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name is a null reference or empty");
            if (string.IsNullOrEmpty(binaryKey)) throw new ArgumentNullException("binaryKey", "binaryKey is a null reference or empty");

            base.Name = name;

            Name = name;
            BinaryKey = binaryKey;
            StoreLocation = storeLocation;
            StoreName = storeName;
        }

        /// <summary>
        /// Creates an instance of Certificate where the certificate is a binary resource
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="binaryKey">The binary key.</param>
        public Certificate(Id id, string name, StoreLocation storeLocation, StoreName storeName, string binaryKey)
        : this(name, storeLocation, storeName, binaryKey)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of Certificate where the certificate is a binary resource
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="binaryKey">The binary key.</param>
        public Certificate(Feature feature, string name, StoreLocation storeLocation, StoreName storeName, string binaryKey)
        : this(name, storeLocation, storeName, binaryKey)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of Certificate where the certificate is a binary resource
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="binaryKey">The binary key.</param>
        public Certificate(Id id, Feature feature, string name, StoreLocation storeLocation, StoreName storeName, string binaryKey)
        : this(name, storeLocation, storeName, binaryKey)
        {
            Id = id;
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of Certificate where the certificate is requested or exists at the specified path
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="certificatePath">The certificate path.</param>
        /// <param name="authorityRequest">if set to <c>true</c> [authority request].</param>
        /// <exception cref="ArgumentNullException">name;name is a null reference or empty
        /// or
        /// certificatePath;certificatePath is a null reference or empty</exception>
        public Certificate(string name, StoreLocation storeLocation, StoreName storeName, string certificatePath, bool authorityRequest)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name is a null reference or empty");
            if (string.IsNullOrEmpty(certificatePath)) throw new ArgumentNullException("certificatePath", "certificatePath is a null reference or empty");

            base.Name = name;

            Name = name;
            CertificatePath = certificatePath;
            Request = authorityRequest;
            StoreLocation = storeLocation;
            StoreName = storeName;
        }

        /// <summary>
        /// Creates an instance of Certificate where the certificate is requested or exists at the specified path
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="certificatePath">The certificate path.</param>
        /// <param name="request">if set to <c>true</c> [request].</param>
        public Certificate(Id id, string name, StoreLocation storeLocation, StoreName storeName, string certificatePath, bool request)
        : this(name, storeLocation, storeName, certificatePath, request)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of Certificate where the certificate is requested or exists at the specified path
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="certificatePath">The certificate path.</param>
        /// <param name="request">if set to <c>true</c> [request].</param>
        public Certificate(Feature feature, string name, StoreLocation storeLocation, StoreName storeName, string certificatePath, bool request)
        : this(name, storeLocation, storeName, certificatePath, request)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of Certificate where the certificate is requested or exists at the specified path
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="certificatePath">The certificate path.</param>
        /// <param name="request">if set to <c>true</c> [request].</param>
        public Certificate(Id id, Feature feature, string name, StoreLocation storeLocation, StoreName storeName, string certificatePath, bool request)
        : this(name, storeLocation, storeName, certificatePath, request)
        {
            Id = id;
            Feature = feature;
        }

        #endregion constructors

        /// <summary>
        /// <see cref="Feature"></see> the WebSite belongs to.
        /// </summary>
        public Feature Feature;

        #region attributes

        /// <summary>
        /// The Id of a Binary instance that is the certificate to be installed
        /// </summary>
        public string BinaryKey;

        /// <summary>
        /// If the Request attribute is <c>false</c> then this attribute is the path to the certificate file outside of the package.
        /// If the Request attribute is <c>true</c> then this attribute is the certificate authority to request the certificate from.
        /// </summary>
        public string CertificatePath;

        /// <summary>
        /// The name of the certificate being installed
        /// </summary>
        public new string Name;

        /// <summary>
        /// Flag to indicate if the certificate should be overwritten.
        /// </summary>
        public bool? Overwrite;

        /// <summary>
        /// This attribute controls whether the CertificatePath attribute is a path to a certificate file (Request=<c>false</c>) or
        /// the certificate authority to request the certificate from (Request=<c>true</c>).
        /// </summary>
        public bool? Request;

        /// <summary>
        /// If the Binary stream or path to the file outside of the package is a password protected PFX file, the password for that PFX must be specified here.
        /// </summary>
        public string PFXPassword;

        /// <summary>
        /// Sets the certificate StoreLocation.
        /// </summary>
        public StoreLocation StoreLocation;

        /// <summary>
        /// Sets the certificate StoreName.
        /// </summary>
        public StoreName StoreName;

        #endregion attributes

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XContainer[] ToXml()
        {
            var element = new XElement(WixExtension.IIs.ToXNamespace() + "Certificate");

            element.SetAttribute("Id", Id)
                   .SetAttribute("BinaryKey", BinaryKey)
                   .SetAttribute("CertificatePath", CertificatePath)
                   .SetAttribute("Name", Name)
                   .SetAttribute("Overwrite", Overwrite)
                   .SetAttribute("PFXPassword", PFXPassword)
                   .SetAttribute("Request", Request)
                   .SetAttribute("StoreLocation", Enum.GetName(typeof(StoreLocation), StoreLocation))
                   .SetAttribute("StoreName", Enum.GetName(typeof(StoreName), StoreName));

            return new[] { element };
        }
    }
}