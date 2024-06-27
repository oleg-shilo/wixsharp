#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2016 Oleg Shilo

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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
#pragma warning disable 1591

    public enum FirewallExceptionProfile
    {
        domain,
        @private,
        @public,
        all
    }

    public enum FirewallExceptionProtocol
    {
        tcp,
        udp
    }

    public enum FirewallExceptionScope
    {
        any,
        localSubnet
    }

#pragma warning restore 1591

    /// <summary>
    /// Registers an exception for a program or a specific port and protocol in the Windows Firewall on
    /// Windows XP SP2, Windows Server 2003 SP1, and later.
    /// </summary>
    /// <remarks>
    /// FirewallException is a Wix# representation of WiX FirewallException Element (Firewall Extension)
    /// </remarks>
    /// <example>The following is an example of defining FirewallException for the file being installed.
    /// <code>
    /// var project =
    ///     new Project("MyProduct",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File(@"Files\Bin\MyApp.exe",
    ///                 new FirewallException("MyApp")
    ///                 {
    ///                     RemoteAddress = "127.0.0.1, 127.0.0.2, 127.0.0.3".Split(',')
    ///                 }
    ///                 ...
    /// </code>
    /// </example>
    public class FirewallException : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// Description for this firewall rule displayed in Windows Firewall manager in Windows Vista and later.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// Identifier of a file to be granted access to all incoming ports and protocols. If you use File, you cannot also use Program.
        /// If you use File and also Port or Protocol in the same FirewallException element,
        /// the exception will fail to install on Windows XP and Windows Server 2003. IgnoreFailure="yes" can be used to ignore the resulting failure,
        /// but the exception will not be added.
        /// </summary>
        [Xml]
        public string File;

        /// <summary>
        /// If "true", failures to register this firewall exception will be silently ignored. If "false" (the default),
        /// failures will cause rollback.
        /// </summary>
        [Xml]
        public bool? IgnoreFailure;

        /// <summary>
        /// Name of this firewall exception, visible to the user in the firewall control panel.
        /// </summary>
        [Xml]
        public new string Name { get { return base.Name; } set { base.Name = value; } }

        /// <summary>
        /// Port to allow through the firewall for this exception.
        /// If you use Port and also File or Program in the same FirewallException element, the exception will fail to
        /// install on Windows XP and Windows Server 2003. IgnoreFailure="yes" can be used to ignore the resulting failure,
        /// but the exception will not be added.
        /// </summary>
        [Xml]
        public string Port;

        /// <summary>
        /// Profile type for this firewall exception. Default is "all". This attribute's value must be one of the following:
        /// <list type="bullet">
        /// <item><description>domain</description></item>
        /// <item><description>private</description></item>
        /// <item><description>public</description></item>
        /// <item><description>all</description></item>
        /// </list>
        /// </summary>
        [Xml]
        public FirewallExceptionProfile? Profile;

        /// <summary>
        /// Path to a target program to be granted access to all incoming ports and protocols. Note that this is a formatted
        /// field, so you can use [#fileId] syntax to refer to a file being installed. If you use Program, you cannot also use File.
        /// If you use Program and also Port or Protocol in the same FirewallException element, the exception will fail to install
        /// on Windows XP and Windows Server 2003. IgnoreFailure="yes" can be used to ignore the resulting failure, but the
        /// exception will not be added.
        /// </summary>
        [Xml]
        public string Program;

        /// <summary>
        /// IP protocol used for this firewall exception. If Port is defined, "tcp" is assumed if the protocol is not specified.
        /// <para>If you use Protocol and also File or Program in the same FirewallException element, the exception will fail to
        /// install on Windows XP and Windows Server 2003. IgnoreFailure="yes" can be used to ignore the resulting failure, but
        /// the exception will not be added.This attribute's value must be one of the following:
        /// </para>
        /// <list type="bullet">
        /// <item><description>tcp</description></item>
        /// <item><description>udp</description></item>
        /// </list>
        /// </summary>
        [Xml]
        public FirewallExceptionProtocol? Protocol;

        /// <summary>
        /// Grouping for the firewall exception.
        /// </summary>
        [Xml]
        public string Grouping;

        /// <summary>
        /// The scope of this firewall exception, which indicates whether incoming connections can come from any computer
        /// including those on the Internet or only those on the local network subnet. To more precisely specify allowed remote
        /// address, specify a custom scope using RemoteAddress child elements. This attribute's value must be one of the
        /// following:
        /// <list type="bullet">
        /// <item><description>any</description></item>
        /// <item><description>localSubnet</description></item>
        /// </list>
        /// </summary>
        [Xml]
        public FirewallExceptionScope? Scope;

        /// <summary>
        /// A remote address to which the port or program can listen. Address formats vary based on the version of Windows and
        /// Windows Firewall the program is being installed on. For Windows XP SP2 and Windows Server 2003 SP1, see
        /// RemoteAddresses Property. For Windows Vista and Windows Server 2008, see RemoteAddresses Property.
        /// </summary>
        public string[] RemoteAddress = new string[0];

        /// <summary>
        /// Indicates whether the firewall exception should be enabled.
        /// </summary>
        [Xml]
        public bool Enabled;

        /// <summary>
        /// ICMP types and codes for the firewall exception.
        /// </summary>
        [Xml]
        public bool IcmpTypesAndCodes;

        /// <summary>
        /// Interface for the firewall exception.
        /// </summary>
        [Xml]
        public string Interface;

        /// <summary>
        /// Local user authorized list for the firewall exception.
        /// </summary>
        [Xml]
        public string LocalUserAuthorizedList;

        /// <summary>
        /// Local user owner for the firewall exception.
        /// </summary>
        [Xml]
        public string LocalUserOwner;

        /// <summary>
        /// If "yes", registers an outbound firewall rule.The default is "no".
        /// </summary>
        [Xml]
        public bool Outbound;

        /// <summary>
        /// Remote machine authorized list for the firewall exception.
        /// </summary>
        [Xml] public string RemoteMachineAuthorizedList;

        /// <summary>
        /// Remote port for the firewall exception.
        /// </summary>
        [Xml] public string RemotePort;

        /// <summary>
        /// Remote user authorized list for the firewall exception.
        /// </summary>
        [Xml] public string RemoteUserAuthorizedList;

        /// <summary>
        /// Local AppPackageId for the firewall exception.
        /// </summary>
        [Xml] public string LocalAppPackageId;

        /// <summary>
        /// Service for the firewall exception.Cannot be specified when the firewall exception is a child of a service element.
        /// </summary>
        [Xml]
        public string Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirewallException" /> class.
        /// </summary>
        public FirewallException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirewallException" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public FirewallException(Id id)
        {
            base.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirewallException" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FirewallException(string name)
        {
            base.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirewallException" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public FirewallException(Id id, string name)
        {
            base.Id = id;
            base.Name = name;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Fire);

            var firewallElement = this.ToXElement(WixExtension.Fire, "FirewallException");

            RemoteAddress.ForEach(address =>
            {
                firewallElement.Add(WixExtension.Fire.XElement("RemoteAddress", new XAttribute("Value", address.Trim())));
            });

            var findComponent = context.XParent.FindFirst("Component");

            if (findComponent != null)
            {
                // candidate for `CreateAndInsertParentComponent(context)`

                XElement newComponent = this.CreateParentComponent();
                newComponent.Add(firewallElement);
                findComponent.Parent?.Add(newComponent);

                MapComponentToFeatures(newComponent.Attr("Id"), ActualFeatures, context);
            }
            else
            {
                context.XParent.Add(firewallElement);
            }
        }
    }
}