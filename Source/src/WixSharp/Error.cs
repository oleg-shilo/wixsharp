namespace WixSharp
{
    /// <summary>
    /// Implements `Error` element that can be used to define and customize runtime error messages.
    /// </summary>
    /// <seealso cref="WixSharp.WixEntity" />
    /// <seealso cref="WixSharp.IGenericEntity" />
    public class Error : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="message">The message.</param>
        public Error(string id, string message)
        {
            Id = id;
            Message = message;
        }

        /// <summary>
        /// Number of the error for which a message is being provided.
        /// </summary>
        [Xml]
        private new string Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        /// <summary>
        /// Error message.
        /// </summary>
        [Xml(true)]
        private string Message { get; set; }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            var ui = context.XParent.SelectOrCreate("UI");

            ui.Add(this.ToXElement());
        }
    }
}