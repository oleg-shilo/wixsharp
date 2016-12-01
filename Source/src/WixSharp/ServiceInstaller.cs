using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    ///  Defines service installer for the file being installed. It encapsulates functionality provided
    ///  by <c>ServiceInstall</c> and <c>ServiceConfig</c> WiX elements.
    /// </summary>
    /// <example>The following sample demonstrates how to install service:
    /// <code>
    /// File service;
    /// var project =
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             service = new File(@"..\SimpleService\MyApp.exe")));
    ///
    /// service.ServiceInstaller = new ServiceInstaller
    ///                            {
    ///                                Name = "WixSharp.TestSvc",
    ///                                StartOn = SvcEvent.Install,
    ///                                StopOn = SvcEvent.InstallUninstall_Wait,
    ///                                RemoveOn = SvcEvent.Uninstall_Wait,
    ///                            };
    ///  ...
    ///
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class ServiceInstaller : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInstaller"/> class.
        /// </summary>
        public ServiceInstaller()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInstaller"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServiceInstaller(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The error control associated with the service startup. The default value is <c>SvcErrorControl.normal</c>.
        /// </summary>
        public SvcErrorControl ErrorControl = SvcErrorControl.normal;

        /// <summary>
        /// Defines the way service starts. The default value is <c>SvcStartType.auto</c>.
        /// </summary>
        public SvcStartType StartType = SvcStartType.auto;

        /// <summary>
        /// The display name of the service as it is listed in the Control Panel.
        /// If not specified the name of the service will be used instead.
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// The description of the service as it is listed in the Control Panel.
        /// </summary>
        public string Description;

        /// <summary>
        /// The type of the service (e.g. kernel/system driver, process). The default value is <c>SvcType.ownProcess</c>.
        /// </summary>
        public SvcType Type = SvcType.ownProcess;

        /// <summary>
        /// Associates 'start service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.Install</c>. Meaning "start the service when it is installed". 
        /// <para>
        /// Set this member to <c>null</c> if you don't want to start the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent StartOn = SvcEvent.Install;
        /// <summary>
        /// Associates 'stop service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.InstallUninstall_Wait</c>. Meaning "stop the  
        /// service when it is being installed and uninstalled and wait for the action completion". 
        /// <para>
        /// Set this member to <c>null</c> if you don't want to stop the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent StopOn = SvcEvent.InstallUninstall_Wait;
        /// <summary>
        /// Associates 'remove service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.Uninstall</c>. Meaning "remove the service when it is uninstalled". 
        /// <para>
        /// Set this member to <c>null</c> if you don't want to remove the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent RemoveOn = SvcEvent.Uninstall;

        /// <summary>
        /// Semicolon separated list of the names of the external service the service being installed depends on. 
        /// It supposed to be names (not the display names) of a previously installed services.
        /// <para>For example: DependsOn = "Dnscache;Dhcp"</para>
        /// </summary>
        public string DependsOn = "";

        /// <summary>
        /// Fully qualified names must be used even for local accounts, 
        /// e.g.: ".\LOCAL_ACCOUNT". Valid only when ServiceType is ownProcess.
        /// </summary>
        public string Account;

        /// <summary>
        /// Contains any command line arguments or properties required to run the service.
        /// </summary>
        public string Arguments;

        /// <summary>
        /// Determines whether the existing service description will be ignored. If 'yes', the service description will be null, 
        /// even if the Description attribute is set.
        /// </summary>
        public bool? EraseDescription;

        /// <summary>
        /// Whether or not the service interacts with the desktop.
        /// </summary>
        public bool? Interactive;

        /// <summary>
        /// The load ordering group that this service should be a part of.
        /// </summary>
        public string LoadOrderGroup;

        /// <summary>
        /// The password for the account. Valid only when the account has a password.
        /// </summary>
        public string Password;

        /// <summary>
        /// The overall install should fail if this service fails to install.
        /// </summary>
        public bool? Vital;

        /// <summary>
        /// Renders ServiceInstaller properties to appropriate WiX elements
        /// </summary>
        /// <param name="project">
        /// Project instance - may be modified to include WixUtilExtension namespace,
        /// if necessary.
        /// </param>
        /// <returns>
        /// Collection of XML entities to be added to the project XML result
        /// </returns>
        internal object[] ToXml(Project project)
        {
            var result = new List<XElement>();

            result.Add(ServiceInstallToXml(project));

            if (StopOn != null)
                result.Add(SvcEventToXml("Stop", StopOn));

            if (StartOn != null)
                result.Add(SvcEventToXml("Start", StartOn));

            if (RemoveOn != null)
                result.Add(SvcEventToXml("Remove", RemoveOn));

            return result.ToArray();
        }

        XElement ServiceInstallToXml(Project project)
        {
            var serviceInstall =
                new XElement("ServiceInstall",
                    new XAttribute("Id", Id),
                    new XAttribute("Name", Name),
                    new XAttribute("DisplayName", DisplayName ?? Name),
                    new XAttribute("Description", Description ?? DisplayName ?? Name),
                    new XAttribute("Type", Type),
                    new XAttribute("Start", StartType),
                    new XAttribute("ErrorControl", ErrorControl));

            serviceInstall.SetAttribute("Account", Account)
                          .SetAttribute("Arguments", Arguments)
                          .SetAttribute("Interactive", Interactive)
                          .SetAttribute("LoadOrderGroup", LoadOrderGroup)
                          .SetAttribute("Password", Password)
                          .SetAttribute("EraseDescription", EraseDescription)
                          .SetAttribute("Vital", Vital)
                          .AddAttributes(Attributes);

            foreach (var item in DependsOn.Split(';'))
            {
                if (item.IsNotEmpty())
                    serviceInstall.AddElement("ServiceDependency", "Id=" + item);
            }

            if (DelayedAutoStart.HasValue
                || PreShutdownDelay.HasValue
                || ServiceSid != null)
            {
                var serviceConfig = new XElement("ServiceConfig");

                serviceConfig.SetAttribute("DelayedAutoStart", DelayedAutoStart)
                             .SetAttribute("PreShutdownDelay", PreShutdownDelay)
                             .SetAttribute("ServiceSid", ServiceSid)

                             .SetAttribute("OnInstall", ConfigureServiceTrigger.Install.PresentIn(ConfigureServiceTrigger))
                             .SetAttribute("OnReinstall", ConfigureServiceTrigger.Reinstall.PresentIn(ConfigureServiceTrigger))
                             .SetAttribute("OnUninstall", ConfigureServiceTrigger.Uninstall.PresentIn(ConfigureServiceTrigger));

                serviceInstall.Add(serviceConfig);
            }

            if (IsFailureActionSet
                || ProgramCommandLine != null
                || RebootMessage != null
                || ResetPeriodInDays.HasValue
                || RestartServiceDelayInSeconds.HasValue)
            {
                project.IncludeWixExtension(WixExtension.Util);

                var serviceConfig = new XElement(WixExtension.Util.ToXNamespace() + "ServiceConfig");

                serviceConfig.SetAttribute("FirstFailureActionType", FirstFailureActionType)
                             .SetAttribute("SecondFailureActionType", SecondFailureActionType)
                             .SetAttribute("ThirdFailureActionType", ThirdFailureActionType)
                             .SetAttribute("ProgramCommandLine", ProgramCommandLine)
                             .SetAttribute("RebootMessage", RebootMessage)
                             .SetAttribute("ResetPeriodInDays", ResetPeriodInDays)
                             .SetAttribute("RestartServiceDelayInSeconds", RestartServiceDelayInSeconds);

                serviceInstall.Add(serviceConfig);
            }

            return serviceInstall;
        }

        bool IsFailureActionSet
        {
            get
            {
                return FirstFailureActionType != FailureActionType.none 
                       || SecondFailureActionType != FailureActionType.none
                       || ThirdFailureActionType!= FailureActionType.none;
            }
        }

        XElement SvcEventToXml(string controlType, SvcEvent value)
        {
            return new XElement("ServiceControl",
                       new XAttribute("Id", controlType + this.Id),
                       new XAttribute("Name", this.Name),
                       new XAttribute(controlType, value.Type),
                       new XAttribute("Wait", value.Wait.ToYesNo()));
        }

        #region ServiceConfig attributes

        /// <summary>
        /// Specifies whether an auto-start service should delay its start until after all other auto-start services.
        /// This property only affects auto-start services. If this property is not initialized the setting is not configured.
        /// </summary>
        public bool? DelayedAutoStart;

        //public object FailureActionsWhen { get; set; } //note implementing util:serviceconfig instead

        /// <summary>
        /// Specifies time in milliseconds that the Service Control Manager (SCM) waits after notifying 
        /// the service of a system shutdown. If this attribute is not present the default value, 3 minutes, is used.
        /// </summary>
        public int? PreShutdownDelay;

        /// <summary>
        /// Specifies the service SID to apply to the service
        /// </summary>
        public ServiceSid ServiceSid;

        /// <summary>
        /// Specifies whether to configure the service when the parent Component is installed, reinstalled, or uninstalled
        /// </summary>
        /// <remarks>
        /// Defaults to ConfigureServiceTrigger.Install. 
        /// Strictly applies to the configuration of properties: DelayedAutoStart, PreShutdownDelay, ServiceSid.
        /// </remarks>
        public ConfigureServiceTrigger ConfigureServiceTrigger = ConfigureServiceTrigger.Install;

        #endregion

        #region Util:ServiceConfig attributes

        /// <summary>
        /// Action to take on the first failure of the service
        /// </summary>
        public FailureActionType FirstFailureActionType = FailureActionType.none;

        /// <summary>
        /// If any of the three *ActionType attributes is "runCommand", 
        /// this specifies the command to run when doing so.
        /// </summary>
        public string ProgramCommandLine;

        /// <summary>
        /// If any of the three *ActionType attributes is "reboot", 
        /// this specifies the message to broadcast to server users before doing so.
        /// </summary>
        public string RebootMessage;

        /// <summary>
        /// Number of days after which to reset the failure count to zero if there are no failures.
        /// </summary>
        public int? ResetPeriodInDays;

        /// <summary>
        /// If any of the three *ActionType attributes is "restart", 
        /// this specifies the number of seconds to wait before doing so.
        /// </summary>
        public int? RestartServiceDelayInSeconds;

        /// <summary>
        /// Action to take on the second failure of the service.
        /// </summary>
        public FailureActionType SecondFailureActionType = FailureActionType.none;

        /// <summary>
        /// Action to take on the third failure of the service.
        /// </summary>
        public FailureActionType ThirdFailureActionType = FailureActionType.none;

        #endregion

    }

    /// <summary>
    /// 
    /// Flags for indicating when the service should be configured.
    /// </summary>
    [Flags]
    public enum ConfigureServiceTrigger
    {
#pragma warning disable 1591
        /// <summary>
        /// Not a valid value for ServiceConfig.On(Install, Reinstall, Uninstall)
        /// </summary>
        None = 0,
        Install = 1,
        Reinstall = 2,
        Uninstall = 4
#pragma warning restore 1591

    }

    /// <summary>
    /// Defines details of the setup event triggering the service actions to be performed during the service installation.
    /// </summary>
    public class SvcEvent
    {
        /// <summary>
        /// Specifies when the service action occur. It can be one of the <see cref="SvcEventType"/> values.
        /// </summary>
        public SvcEventType Type;
        /// <summary>
        /// The flag indicating if after triggering the service action the setup should wait until te action is completed.
        /// </summary>
        public bool Wait;

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.install"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation without 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent Install = new SvcEvent { Type = SvcEventType.install, Wait = false };

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.install"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent Install_Wait = new SvcEvent { Type = SvcEventType.install, Wait = true };
        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.uninstall"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation 
        /// without waiting for the completion.
        /// </summary>
        static public SvcEvent Uninstall = new SvcEvent { Type = SvcEventType.uninstall, Wait = false };
        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.uninstall"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent Uninstall_Wait = new SvcEvent { Type = SvcEventType.uninstall, Wait = true };
        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.both"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation without 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent InstallUninstall = new SvcEvent { Type = SvcEventType.both, Wait = false };
        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.both"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent InstallUninstall_Wait = new SvcEvent { Type = SvcEventType.both, Wait = true };
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


    /// <summary>
    /// Possible values for ServiceInstall.(First|Second|Third)FailureActionType
    /// </summary>
    public enum FailureActionType
    {
#pragma warning disable 1591
        none,
        reboot,
        restart,
        runCommand
#pragma warning restore 1591
    }

}