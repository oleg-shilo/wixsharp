namespace WixSharp.Nsis
{
    /// <summary>
    /// Describes a payload to a bootstrapper.
    /// </summary>
    /// <seealso cref="NsisBootstrapper" />
    public class Payload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Payload"/> class.
        /// </summary>
        public Payload() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payload"/> class.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        public Payload(string sourceFile) { SourceFile = sourceFile; }

        /// <summary>
        /// The destination path and file name for this payload.
        /// The default is the source file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location of the source file.
        /// </summary>
        public string SourceFile { get; set; }
    }
}
