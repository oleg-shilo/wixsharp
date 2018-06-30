using System;

namespace WixSharp.Nsis
{
    /// <summary>
    /// Defines version information.
    /// </summary>
    public class VersionInformation
    {
        /// <summary>
        /// Creates instance of the <see cref="VersionInformation"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="productVersion">The version of the product.</param>
        public VersionInformation(string productVersion)
        {
            ProductVersion = productVersion ?? throw new ArgumentNullException(nameof(productVersion));
        }

        /// <summary>
        /// Gets or sets the name of the product this file is distributed with.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the comments associated with the file.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the name of the company that produced the file.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets all copyright notices that apply to the specified file.
        /// </summary>
        public string LegalCopyright { get; set; }

        /// <summary>
        /// Gets or sets the description of the file.
        /// </summary>
        public string FileDescription { get; set; }

        /// <summary>
        /// Gets or sets the file version number.
        /// </summary>
        public string FileVersion { get; set; }

        /// <summary>
        /// Gets or sets the version of the product this file is distributed with.
        /// </summary>
        public string ProductVersion { get; set; }

        /// <summary>
        /// Gets or sets the internal name of the file, if one exists.
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// Gets or sets the trademarks and registered trademarks that apply to the file.
        /// </summary>
        public string LegalTrademarks { get; set; }

        /// <summary>
        /// Gets or sets the name the file was created with.
        /// </summary>
        public string OriginalFilename { get; set; }

        /// <summary>
        /// Gets or sets information about a private version of the file.
        /// </summary>
        public string PrivateBuild { get; set; }

        /// <summary>
        /// Gets or sets the special build information for the file.
        /// </summary>
        public string SpecialBuild { get; set; }
    }
}