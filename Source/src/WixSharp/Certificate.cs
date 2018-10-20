using System;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    //http://www.davidwhitney.co.uk/content/blog/index.php/2009/02/11/installing-certificates-using-wix-windows-installer-xml-voltive/
    //http://stackoverflow.com/questions/860996/wix-and-certificates-in-iis
    /// <summary>
    /// This class defines website certificate attributes. It is a close equivalent of Certificate WiX element.
    /// </summary>
    public class Certificate : WixEntity, IGenericEntity
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

        #region attributes

        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// The Id of a Binary instance that is the certificate to be installed
        /// </summary>
        [Xml]
        public string BinaryKey;

        /// <summary>
        /// If the Request attribute is <c>false</c> then this attribute is the path to the certificate file outside of the package.
        /// If the Request attribute is <c>true</c> then this attribute is the certificate authority to request the certificate from.
        /// </summary>
        [Xml]
        public string CertificatePath;

        /// <summary>
        /// The name of the certificate being installed
        /// </summary>
        [Xml]
        public new string Name;

        /// <summary>
        /// Flag to indicate if the certificate should be overwritten.
        /// </summary>
        [Xml]
        public bool? Overwrite;

        /// <summary>
        /// This attribute controls whether the CertificatePath attribute is a path to a certificate file (Request=<c>false</c>) or
        /// the certificate authority to request the certificate from (Request=<c>true</c>).
        /// </summary>
        [Xml]
        public bool? Request;

        /// <summary>
        /// If the Binary stream or path to the file outside of the package is a password protected PFX file, the password for that PFX must be specified here.
        /// </summary>
        [Xml]
        public string PFXPassword;

        /// <summary>
        /// Sets the certificate StoreLocation.
        /// </summary>
        [Xml]
        public StoreLocation? StoreLocation;

        /// <summary>
        /// Sets the certificate StoreName.
        /// </summary>
        [Xml]
        public StoreName? StoreName;

        #endregion attributes

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.IIs);

            this.CreateAndInsertParentComponent(context)
                .Add(this.ToXElement(WixExtension.IIs, "Certificate"));
        }
    }
}