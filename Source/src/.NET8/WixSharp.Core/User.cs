using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

public class Class3
{
    [DllImport("user32.dll")]
    static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    // [UnmanagedCallersOnly(EntryPoint = "CustomActionCore")]
    public static uint CustomActionCore(IntPtr handle)
    {
        // Debug.Assert(false);
        using Session session = Session.FromHandle(handle, false);

        // MessageBox(GetForegroundWindow(), "Hello from .NET Core Form! (007)", "WixSharp.Core.AUT", 0);

        MessageBox(GetForegroundWindow(), typeof(Class3).Assembly.GetName().Name, "WixSharp - " + typeof(Class3).Name, 0);
        //session.Log("CustomActionCore invoked");

        return (uint)ActionResult.UserExit;
    }
}

namespace WixSharp
{
    /// <summary>
    /// Represents a WixUtilExtension User
    /// </summary>
    public class User : WixEntity, IGenericEntity
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

        #endregion Constructors

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
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// Maps to the CanNotChangePassword property of User
        /// </summary>
        [Xml]
        public bool? CanNotChangePassword; //only allowed under a component

        /// <summary>
        /// Maps to the CreateUser property of User
        /// </summary>
        [Xml]
        public bool? CreateUser; //only allowed under a component

        /// <summary>
        /// Maps to the Disabled property of User
        /// </summary>
        [Xml]
        public bool? Disabled; //only allowed under a component

        /// <summary>
        /// Maps to the Domain property of User
        /// </summary>
        [Xml]
        public string Domain;

        /// <summary>
        /// Maps to the FailIfExists property of User
        /// </summary>
        [Xml]
        public bool? FailIfExists; //only allowed under a component

        /// <summary>
        /// Maps to the LogOnAsBatchJob property of User
        /// </summary>
        [Xml]
        public bool? LogonAsBatchJob; //only allowed under a component

        /// <summary>
        /// Maps to the LogOnAsService property of User
        /// </summary>
        [Xml]
        public bool? LogonAsService; //only allowed under a component

        /// <summary>
        /// A Formatted string that contains the name of the user account.
        /// </summary>
        [Xml]
        public new string Name { get => base.Name; set => base.Name = value; }

        /// <summary>
        /// Maps to the Password property of User
        /// </summary>
        [Xml]
        public string Password;

        /// <summary>
        /// Maps to the PasswordExpired property of User
        /// </summary>
        [Xml]
        public bool? PasswordExpired; //only allowed under a component

        /// <summary>
        /// Maps to the PasswordNeverExpires property of User
        /// </summary>
        [Xml]
        public bool? PasswordNeverExpires; //only allowed under a component

        /// <summary>
        /// Maps to the RemoveOnUninstall property of User
        /// </summary>
        [Xml]
        public bool? RemoveOnUninstall; //only allowed under a component

        /// <summary>
        /// Maps to the UpdateIfExists property of User
        /// </summary>
        [Xml]
        public bool? UpdateIfExists; //only allowed under a component

        /// <summary>
        /// Maps to the Vital property of User
        /// </summary>
        [Xml]
        public bool? Vital; //only allowed under a component

        #endregion Wix User attributes

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
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Util);

            if (MustDescendFromComponent)
            {
                this.CreateAndInsertParentComponent(context)
                    .Add(this.ToXElement(WixExtension.Util, "User"));
            }
            else
            {
                context.XParent.Add(this.ToXElement(WixExtension.Util, "User"));
            }
        }
    }
}