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

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// This class defines website attributes. It is a close equivalent of WebSite WiX element.
    /// </summary>
    public partial class WebSite : WixEntity
    {
        public IISCertificate Certificate;

        /// <summary>
        /// This is the name of the web site that will show up in the IIS management console.
        /// </summary>
        public string Description = "";

        /// <summary>
        /// Indicates if the WebSite is to be installed (created on IIS) or existing WebSite should be used to install the corresponding
        /// WebApplication. The default <see cref="InstallWebSite"/> value is <c>false</c>
        /// <para>Developers should be aware of the WebSite installation model imposed by WiX/MSI and use <see cref="InstallWebSite"/> carefully.</para>
        /// <para>If <see cref="InstallWebSite"/> value is set to <c>false</c> the parent WebApplication (<see cref="T:WixSharp.IISVirtualDir"/>)
        /// will be installed in the brand new (freshly created) WebSite or in the existing one if a site with the same address/port combination already exists
        /// on IIS). The undesirable side affect of this deployment scenario is that if the existing WebSite was used to install the WebApplication it will be
        /// deleted on IIS during uninstallation even if this WebSite has other WebApplications installed.</para>
        /// <para>The "safer" option is to set <see cref="InstallWebSite"/> value to <c>true</c> (default value). In this case the WebApplication will
        /// be installed in an existing WebSite with matching address/port. If the match is not found the installation will fail. During the uninstallation
        /// only installed WebApplication will be removed from IIS.</para>
        /// </summary>
        public bool InstallWebSite = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSite" /> class.
        /// </summary>
        public WebSite()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSite"/> class.
        /// </summary>
        /// <param name="id">The name.</param>
        /// <param name="description">The description of the web site (as it shows up in the IIS management console).</param>
        public WebSite(Id id, string description)
        {
            this.Id = id;
            this.Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSite"/> class.
        /// </summary>
        /// <param name="description">The description of the web site (as it shows up in the IIS management console).</param>
        public WebSite(string description)
        {
            this.Name = "WebSite"; //to become a prefix of the auto-generated Id
            this.Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSite"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public WebSite(Id id)
        {
            this.Id = id;
        }

        ///// <summary>
        ///// Collection of <see cref="T:WebSite.Certificate"/> associated with website.
        ///// </summary>
        //public Certificate[] Certificates = new Certificate[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSite"/> class.
        /// </summary>
        /// <param name="description">The description of the web site (as it shows up in the IIS management console).</param>
        /// <param name="addressDefinition">The address definition.</param>
        public WebSite(string description, string addressDefinition)
        {
            this.Name = "WebSite"; //to become a prffix of the auto-generated Id
            this.Description = description;
            this.AddressesDefinition = addressDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSite"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="description">The description.</param>
        /// <param name="addressDefinition">The address definition.</param>
        public WebSite(Id id, string description, string addressDefinition)
        {
            this.Id = id;
            this.AddressesDefinition = addressDefinition;
            this.Description = description;
        }

        internal void ProcessAddressesDefinition()
        {
            if (!AddressesDefinition.IsEmpty())
            {
                List<WebAddress> addressesToAdd = new List<WebAddress>();

                foreach (string addressDef in AddressesDefinition.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        string[] tokens = addressDef.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        string address = tokens[0];
                        string port = tokens[1];
                        if (tokens[1].ContainsWixConstants())
                        {
                            addressesToAdd.Add(new WebAddress { Address = address, AttributesDefinition = "Port=" + port });
                        }
                        else
                        {
                            addressesToAdd.Add(new WebAddress { Address = address, Port = Convert.ToInt32(port) });
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid AddressesDefinition", e);
                    }
                }

                this.addresses = addressesToAdd.ToArray();
            }
        }

        /// <summary>
        /// This class defines website address. It is a close equivalent of WebAddress WiX element.
        /// </summary>
        public partial class WebAddress : WixEntity
        {
            /// <summary>
            ///  The IP address for the web address. To specify the "All Unassigned" IP address, do not specify
            ///  this attribute or specify its value as "*". The IP address is also used to determine if the WebSite is already installed.
            ///  The IP address must match exactly (empty value matches "All Unassigned") unless "*" is used which will match any existing IP (including "All Unassigned").
            /// </summary>
            public string Address = "*";

            /// <summary>
            /// Sets the port number.
            /// </summary>
            public int Port = 0;

            /// <summary>
            /// Optional attributes of the <c>WebAddress Element</c> (e.g. Secure:YesNoPath).
            /// </summary>
            /// <example>
            /// <code>
            /// var address = new WebAddress
            /// {
            ///     Port = 80,
            ///     Attributes = new Dictionary&lt;string, string&gt; { { "Secure", "Yes" } };
            ///     ...
            /// </code>
            /// </example>
            public new Dictionary<string, string> Attributes { get { return base.Attributes; } set { base.Attributes = value; } }
        }

        /// <summary>
        /// Specification for auto-generating the <see cref="T:WebSite.WebAddresses"/> collection.
        /// <para>If <see cref="AddressesDefinition"/> is specified, the existing content of <see cref="Addresses"/> will be ignored
        /// and replaced with the auto-generated one at compile time.</para>
        /// </summary>
        /// <example>
        /// <c>webSite.AddressesDefinition = "*:80;*90";</c> will be parsed and converted to an array of <see cref="T:WixSharp.WebSite.WebAddress"/> as follows:
        /// <code>
        /// ...
        /// webSite.Addresses = new []
        ///     {
        ///         new WebSite.WebAddress
        ///         {
        ///             Address = "*",
        ///             Port = 80
        ///         },
        ///         new WebSite.WebAddress
        ///         {
        ///             Address = "*",
        ///             Port = 80
        ///         }
        ///     }
        /// </code>
        /// </example>
        public string AddressesDefinition = "";

        /// <summary>
        /// Reference to a WebApplication that is to be installed as part of this web site.
        /// </summary>
        public string WebApplication = null;

        /// <summary>
        /// Collection of <see cref="T:WebSite.WebAddresses"/> associated with website.
        /// <para>
        /// The user specified values of <see cref="Addresses"/> will be ignored and replaced with the
        /// auto-generated addresses if <see cref="AddressesDefinition"/> is specified either directly or via appropriate <see cref="WebSite"/> constructor.
        /// </para>
        /// </summary>
        public WebAddress[] Addresses
        {
            get
            {
                ProcessAddressesDefinition();
                return addresses;
            }
            set
            {
                addresses = value;
            }
        }

        WebAddress[] addresses = new WebAddress[0];
    }

    /// <summary>
    /// This class defines WebAppPool WiX element. It is used to specify the application pool for this application in IIS 6 applications.
    /// </summary>
    public partial class WebAppPool : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebAppPool"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="attributesDefinition">The attributes definition. This parameter is used to set encapsulated <see cref="T:WixSharp.WixEntity.AttributesDefinition"/>.</param>
        public WebAppPool(string name, string attributesDefinition)
        {
            base.Name = name;
            base.AttributesDefinition = attributesDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAppPool"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public WebAppPool(string name)
        {
            base.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAppPool"/> class.
        /// </summary>
        public WebAppPool()
        {
        }
    }

    /// <summary>
    /// This class defines WebDirProperites WiX element. The class itself has no distinctive behaviour nor schema. It is fully relying on
    /// encapsulated <see cref="T:WixSharp.WixEntity.AttributesDefinition"/>.
    /// </summary>
    public partial class WebDirProperties : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDirProperties"/> class.
        /// </summary>
        /// <param name="attributesDefinition">The attributes definition. This parameter is used to set encapsulated <see cref="T:WixSharp.WixEntity.AttributesDefinition"/>.</param>
        public WebDirProperties(string attributesDefinition)
        {
            base.AttributesDefinition = attributesDefinition;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="WebDirProperties"/>.
        /// </summary>
        /// <param name="attributesDefinition">The attributes definition.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator WebDirProperties(string attributesDefinition)
        {
            return new WebDirProperties(attributesDefinition);
        }
    }

    /// <summary>
    /// This class defines IIS Virtual Directory. It is a close equivalent of WebVirtualDirectory WiX element.
    /// </summary>
    public class IISVirtualDir : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IISVirtualDir" /> class.
        /// </summary>
        public IISVirtualDir()
        {
            base.Name = "VirtDir"; //to become a prffix of the auto-generated Id
        }

        /// <summary>
        /// WebSite in which this virtual directory belongs.
        /// </summary>
        public WebSite WebSite = null;

        #region WebVirtualDir Element Attributes

        /// <summary>
        /// Gets or sets the application name, which is the URL relative path used to access this virtual directory.
        /// <para>
        /// It is a full equivalent of <see cref="WixSharp.IISVirtualDir.Alias"/>.
        /// </para>
        /// </summary>
        /// <value>The name.</value>
        public new string Name
        {
            get { return Alias; }
            set { Alias = value; }
        }

        /// <summary>
        /// Sets the application name, which is the URL relative path used to access this virtual directory. If not set, the <see cref="AppName"/> will be used.
        /// </summary>
        public string Alias = "";

        #endregion WebVirtualDir Element Attributes

        //IISVirtualDir-to-WebAppliction is one-to-one relationship

        #region WebApplication Element attributes

        /// <summary>
        /// Sets the name of this Web application.
        /// </summary>
        public string AppName = "MyWebApp"; //WebApplication element attribute

        /// <summary>
        /// Sets the Enable Session State option. When enabled, you can set the session timeout using the SessionTimeout attribute.
        /// </summary>
        public bool? AllowSessions;// YesNoDefaultType //WebApplication element attribute

        /// <summary>
        /// Sets the option that enables response buffering in the application, which allows ASP script to set response headers anywhere in the script.
        /// </summary>
        public bool? Buffer;// YesNoDefaultType //WebApplication element attribute

        /// <summary>
        /// Enable ASP client-side script debugging.
        /// </summary>
        public bool? ClientDebugging;// YesNoDefaultType //WebApplication element attribute

        /// <summary>
        /// Sets the default script language for the site.
        /// </summary>
        public DefaultScript? DefaultScript; //WebApplication element attribute

        /// <summary>
        /// Sets the application isolation level for this application for pre-IIS 6 applications.
        /// </summary>
        public Isolation? Isolation;	//WebApplication element attribute

        /// <summary>
        /// Sets the parent paths option, which allows a client to use relative paths to reach parent directories from this application.
        /// </summary>
        public bool? ParentPaths;// YesNoDefaultType //WebApplication element attribute

        /// <summary>
        /// Sets the timeout value for executing ASP scripts.
        /// </summary>
        public int? ScriptTimeout;	//WebApplication element attribute

        /// <summary>
        /// Enable ASP server-side script debugging.
        /// </summary>
        public bool? ServerDebugging;// YesNoDefaultType //WebApplication element attribute

        /// <summary>
        /// Sets the timeout value for sessions in minutes.
        /// </summary>
        public int? SessionTimeout;	//WebApplication element attribute

        /// <summary>
        /// References a WebAppPool instance to use as the application pool for this application in IIS 6 applications.
        /// </summary>
        public WebAppPool WebAppPool; //WebApplication element attribute

        /// <summary>
        /// WebDirProperites used by one or more WebSites.
        /// </summary>
        public WebDirProperties WebDirProperties;

        #endregion WebApplication Element attributes
    }
}