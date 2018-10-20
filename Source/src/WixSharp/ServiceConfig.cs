using System;

namespace WixSharp
{
    /// <summary>
    /// Configures a service being installed or one that already exists. This element's functionality is available starting with MSI 5.0.
    /// </summary>
    public class ServiceConfig : WixEntity, IGenericEntity
    {
        internal ServiceConfig()
        {}

        /// <summary>
        /// Specifies whether an auto-start service should delay its start until after all other auto-start services.
        /// This property only affects auto-start services. If this property is not initialized the setting is not configured.
        /// </summary>
        [Xml]
        public bool? DelayedAutoStart;

        //public object FailureActionsWhen { get; set; } //note implementing util:serviceconfig instead
        
        /// <summary>
        /// Specifies time in milliseconds that the Service Control Manager (SCM) waits after notifying
        /// the service of a system shutdown. If this attribute is not present the default value, 3 minutes, is used.
        /// </summary>
        [Xml]
        public int? PreShutdownDelay;

        /// <summary>
        /// Specifies the service SID to apply to the service
        /// </summary>
        [Xml]
        public ServiceSid ServiceSid;

        /// <summary>
        /// Specifies whether to configure the service when the parent Component is installed.
        /// </summary>
        [Xml]
        public bool? OnInstall;

        /// <summary>
        /// Specifies whether to configure the service when the parent Component is reinstalled.
        /// </summary>
        [Xml]
        public bool? OnReinstall;

        /// <summary>
        /// Specifies whether to configure the service when the parent Component is uninstalled.
        /// </summary>
        [Xml]
        public bool? OnUninstall;

        /// <summary>
        /// Specifies whether to configure the service when the parent Component is installed, reinstalled, or uninstalled
        /// </summary>
        /// <remarks>
        /// Defaults to ConfigureServiceTrigger.Install.
        /// Strictly applies to the configuration of properties: DelayedAutoStart, PreShutdownDelay, ServiceSid.
        /// </remarks>
        public ConfigureServiceTrigger ConfigureServiceTrigger = ConfigureServiceTrigger.Install;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            switch (ConfigureServiceTrigger)
            {
                case ConfigureServiceTrigger.None:
                    break;
                case ConfigureServiceTrigger.Install:
                    OnInstall = true;
                    OnReinstall = false;
                    OnUninstall = false;
                    break;
                case ConfigureServiceTrigger.Reinstall:
                    OnInstall = false;
                    OnReinstall = true;
                    OnUninstall = false;
                    break;
                case ConfigureServiceTrigger.Uninstall:
                    OnInstall = false;
                    OnReinstall = false;
                    OnUninstall = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            context.XParent.Add(this.ToXElement(GetType().Name));
        }
    }

    /// <summary>
    /// Specifies the service SID to apply to the service.
    /// Valid values are "none", "restricted", "unrestricted" or a
    /// Formatted property that resolves to "0" (for "none"),
    /// "3" (for "restricted") or "1" (for "unrestricted").
    /// </summary>
    public class ServiceSid : StringEnum<ServiceSid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSid"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ServiceSid(string value) : base(value) { }

#pragma warning disable 1591
        public static ServiceSid none = new ServiceSid("none");
        public static ServiceSid restricted = new ServiceSid("restricted");
        public static ServiceSid unrestricted = new ServiceSid("unrestricted");
#pragma warning restore 1591
    }
}
