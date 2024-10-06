// Ignore Spelling: Deconstruct

using System;

namespace WixSharp
{
    /// <summary>
    /// The attribute indicating the type member being mapped to XML element. Used by Wix# compiler
    /// to emit XML base on CLR types.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class XmlAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlAttribute"/> class.
        /// </summary>
        public XmlAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public XmlAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlAttribute"/> class.
        /// </summary>
        /// <param name="isCData">if set to <c>true</c> [is c data].</param>
        public XmlAttribute(bool isCData)
        {
            IsCData = isCData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isCData">if set to <c>true</c> [is c data].</param>
        public XmlAttribute(string name, bool isCData)
        {
            Name = name;
            IsCData = isCData;
        }

        /// <summary>
        /// Gets or sets the name of the mapped XML element.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        internal bool IsCData { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; set; }
    }
}