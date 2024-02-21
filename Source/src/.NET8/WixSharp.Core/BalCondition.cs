using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Conditions for a bundle.
    /// The condition is specified in the inner text of the element.
    /// </summary>
    public class BalCondition : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Conditions for a bundle.
        /// </summary>
        public string Condition;

        /// <summary>
        /// Set the value to the text to display when the condition fails and the installation must be terminated.
        /// </summary>
        public string Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalCondition" /> class.
        /// </summary>
        public BalCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BalCondition" /> class.
        /// </summary>
        /// <param name="condition">The Condition.</param>
        /// <param name="message">The Message</param>
        public BalCondition(string condition, string message)
        {
            Condition = condition;
            Message = message;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Bal);

            var element = new XElement(WixExtension.Bal.ToXName("Condition"), Condition)
                .SetAttribute("Message", Message);

            context.XParent.Add(element);
        }
    }
}
