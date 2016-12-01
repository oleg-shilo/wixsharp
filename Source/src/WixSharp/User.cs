using System;
using System.Xml.Linq;

namespace WixSharp
{

    /// <summary>
    /// Represents a WixUtilExtension User
    /// </summary>
    public class User : WixEntity
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of User
        /// </summary>
        public User() { }

        /// <summary>
        /// Creates an instance of User representing <paramref name="name" />
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">name;name is a null reference or empty</exception>
        public User(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates an instance of User representing <paramref name="name" />
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">name;name is a null reference or empty</exception>
        public User(Id id, string name)
            : this(name)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an instance of User representing <paramref name="name" />
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        public User(Id id, Feature feature, string name)
            : this(id, name)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of User representing <paramref name="name" />
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        public User(Feature feature, string name)
            : this(name)
        {
            Feature = feature;
        }

        /// <summary>
        /// Creates an instance of User representing <paramref name="name" />@<paramref name="domain" />
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="domain">The domain.</param>
        public User(Id id, string name, string domain)
            : this(id, name)
        {
            Domain = domain;
        }

        /// <summary>
        /// Creates an instance of User representing <paramref name="name" />@<paramref name="domain" />
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="domain">The domain.</param>
        public User(string name, string domain)
            : this(name)
        {
            Domain = domain;
        }

        /// <summary>
        /// Creates an instance of User representing <paramref name="name" />@<paramref name="domain" />
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="domain">The domain.</param>
        public User(Id id, Feature feature, string name, string domain)
            : this(id, feature, name)
        {
            Domain = domain;
        }

        /// <summary>
        /// Creates an instance of User representing <paramref name="name" />@<paramref name="domain" />
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="name">The name.</param>
        /// <param name="domain">The domain.</param>
        public User(Feature feature, string name, string domain)
            : this(feature, name)
        {
            Domain = domain;
        }

        #endregion

        /// <summary>
        /// <see cref="Feature"></see> the User belongs to.
        /// </summary>
        public Feature Feature;

        /// <summary>
        /// Requests that the User element is included inside a Component element - allowing the User to be installed.
        /// If any of the following properties are assigned (non-null), this property is ignored during compilation and assumed
        /// to be true:
        /// <list type="bullet">
        /// <item>CanNotChangePassword</item>
        /// <item>CreateUser</item>
        /// <item>Disabled</item>
        /// <item>FailIfExists</item>
        /// <item>LogonAsBatchJob</item>
        /// <item>LogonAsService</item>
        /// <item>PasswordExpired</item>
        /// <item>PasswordNeverExpires</item>
        /// <item>RemoveOnUninstall</item>
        /// <item>UpdateIfExists</item>
        /// <item>Vital</item>
        /// </list>
        /// </summary>
        public bool WixIncludeInComponent;

        #region Wix User attributes

        /// <summary>
        /// Maps to the CanNotChangePassword property of User
        /// </summary>
        public bool? CanNotChangePassword; //only allowed under a component

        /// <summary>
        /// Maps to the CreateUser property of User
        /// </summary>
        public bool? CreateUser; //only allowed under a component

        /// <summary>
        /// Maps to the Disabled property of User
        /// </summary>
        public bool? Disabled; //only allowed under a component

        /// <summary>
        /// Maps to the Domain property of User
        /// </summary>
        public string Domain;

        /// <summary>
        /// Maps to the FailIfExists property of User
        /// </summary>
        public bool? FailIfExists; //only allowed under a component

        /// <summary>
        /// Maps to the LogOnAsBatchJob property of User
        /// </summary>
        public bool? LogonAsBatchJob; //only allowed under a component

        /// <summary>
        /// Maps to the LogOnAsService property of User
        /// </summary>
        public bool? LogonAsService; //only allowed under a component

        /// <summary>
        /// Maps to the Password property of User
        /// </summary>
        public string Password;

        /// <summary>
        /// Maps to the PasswordExpired property of User
        /// </summary>
        public bool? PasswordExpired; //only allowed under a component

        /// <summary>
        /// Maps to the PasswordNeverExpires property of User
        /// </summary>
        public bool? PasswordNeverExpires; //only allowed under a component

        /// <summary>
        /// Maps to the RemoveOnUninstall property of User
        /// </summary>
        public bool? RemoveOnUninstall; //only allowed under a component

        /// <summary>
        /// Maps to the UpdateIfExists property of User
        /// </summary>
        public bool? UpdateIfExists; //only allowed under a component

        /// <summary>
        /// Maps to the Vital property of User
        /// </summary>
        public bool? Vital; //only allowed under a component

        #endregion

        /// <summary>
        /// Gets a value indicated if this User must be generated under a Component element or not.
        /// </summary>
        internal bool MustDescendFromComponent
        {
            get
            {
                return CanNotChangePassword.HasValue
                       || CreateUser.HasValue
                       || Disabled.HasValue
                       || FailIfExists.HasValue
                       || LogonAsBatchJob.HasValue
                       || LogonAsService.HasValue
                       || PasswordExpired.HasValue
                       || PasswordNeverExpires.HasValue
                       || RemoveOnUninstall.HasValue
                       || UpdateIfExists.HasValue
                       || Vital.HasValue
                       || WixIncludeInComponent;
            }
        }

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XContainer[] ToXml()
        {
            var userElement = new XElement(WixExtension.Util.ToXNamespace() + "User")
                                  .SetAttribute("Id", Id)
                                  .SetAttribute("Name", Name)
                                  .SetAttribute("CanNotChangePassword", CanNotChangePassword)
                                  .SetAttribute("CreateUser", CreateUser)
                                  .SetAttribute("Disabled", Disabled)
                                  .SetAttribute("Domain", Domain)
                                  .SetAttribute("FailIfExists", FailIfExists)
                                  .SetAttribute("LogonAsBatchJob", LogonAsBatchJob)
                                  .SetAttribute("LogonAsService", LogonAsService)
                                  .SetAttribute("Password", Password)
                                  .SetAttribute("PasswordExpired", PasswordExpired)
                                  .SetAttribute("PasswordNeverExpires", PasswordNeverExpires)
                                  .SetAttribute("RemoveOnUninstall", RemoveOnUninstall)
                                  .SetAttribute("UpdateIfExists", UpdateIfExists)
                                  .SetAttribute("Vital", Vital);

            return new[] { userElement };
        }
    }
}
