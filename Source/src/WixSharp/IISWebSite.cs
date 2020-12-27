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
    ///  This class allows creating an IIS Website without using IISVirtualDir.
    /// </summary>
    /// <seealso cref="WixSharp.WixEntity" />
    /// <seealso cref="WixSharp.IGenericEntity" />
    public sealed class IISWebSite : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public string Port { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IISWebSite"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="port">The port.</param>
        public IISWebSite(string description, string port)
        {
            Description = description;
            Port = port;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            XNamespace ns = "http://schemas.microsoft.com/wix/IIsExtension";
            var dirId = context.XParent.Attribute("Id").Value;

            var componentId = $"{Description}.Component.Id";
            var component = new XElement(XName.Get("Component"),
                new XAttribute("Id", componentId),
                new XAttribute("Guid", WixGuid.NewGuid(componentId)),
                new XAttribute("KeyPath", "yes"));

            context.XParent.Add(component);

            var webAppPoolId = $"{Description}.WebAppPool.Id";
            component.Add(new XElement(ns + "WebAppPool",
                new XAttribute("Id", webAppPoolId),
                new XAttribute("Name", Description),
                new XAttribute("Identity", "localSystem")));

            component.Add(new XElement(ns + "WebSite",
                new XAttribute("Id", $"{Description}.WebSite.Id"),
                new XAttribute("Description", Description),
                new XAttribute("Directory", dirId),
                new XElement(ns + "WebAddress",
                    new XAttribute("Id", "AllUnassigned"),
                    new XAttribute("Port", Port)),
                new XElement(ns + "WebApplication",
                    new XAttribute("Id", $"{Description}.WebSiteApplication.Id"),
                    new XAttribute("WebAppPool", webAppPoolId),
                    new XAttribute("Name", Description))));
        }
    }
}