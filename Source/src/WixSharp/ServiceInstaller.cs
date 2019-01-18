using System;
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
    public class ServiceInstaller : WixEntity, IGenericEntity
    {
        private SvcEvent startOn;
        private SvcEvent stopOn;
        private SvcEvent removeOn;

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
            Name = name;
        }

        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// This is the localizable name of the environment variable
        /// </summary>
        [Xml]
        public new string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                if (DisplayName == null)
                {
                    DisplayName = value;
                }

                if (Description == null)
                {
                    Description = value;
                }
            }
        }

        /// <summary>
        /// The display name of the service as it is listed in the Control Panel.
        /// If not specified the name of the service will be used instead.
        /// </summary>
        [Xml]
        public string DisplayName;

        /// <summary>
        /// The description of the service as it is listed in the Control Panel.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// The type of the service (e.g. kernel/system driver, process). The default value is <c>SvcType.ownProcess</c>.
        /// </summary>
        [Xml]
        public SvcType Type = SvcType.ownProcess;

        /// <summary>
        /// Defines the way service starts. The default value is <c>SvcStartType.auto</c>.
        /// </summary>
        [Xml]
        public SvcStartType Start = SvcStartType.auto;

        /// <summary>
        /// The error control associated with the service startup. The default value is <c>SvcErrorControl.normal</c>.
        /// </summary>
        [Xml]
        public SvcErrorControl ErrorControl = SvcErrorControl.normal;

        /// <summary>
        /// Associates 'start service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.Install</c>. Meaning "start the service when it is installed".
        /// <para>
        /// Set this member to <c>null</c> if you don't want to start the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent StartOn
        {
            get
            {
                return startOn;
            }
            set
            {
                if (value != null)
                {
                    value.Id = "Start" + Id;
                    value.Name = Name;
                    value.Start = value.Type;
                }

                startOn = value;
            }
        }

        /// <summary>
        /// Associates 'stop service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.InstallUninstall_Wait</c>. Meaning "stop the
        /// service when it is being installed and uninstalled and wait for the action completion".
        /// <para>
        /// Set this member to <c>null</c> if you don't want to stop the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent StopOn
        {
            get
            {
                return stopOn;
            }
            set
            {
                if (value != null)
                {
                    value.Id = "Stop" + Id;
                    value.Name = Name;
                    value.Stop = value.Type;
                }

                stopOn = value;
            }
        }

        /// <summary>
        /// Associates 'remove service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.Uninstall</c>. Meaning "remove the service when it is uninstalled".
        /// <para>
        /// Set this member to <c>null</c> if you don't want to remove the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent RemoveOn
        {
            get
            {
                return removeOn;
            }
            set
            {
                if (value != null)
                {
                    value.Id = "Remove" + Id;
                    value.Name = Name;
                    value.Remove = value.Type;
                }

                removeOn = value;
            }
        }

        /// <summary>
        /// Semicolon separated list of the names of the external service the service being installed depends on.
        /// It supposed to be names (not the display names) of a previously installed services.
        /// <para>For example: DependsOn = "Dnscache;Dhcp"</para>
        /// </summary>
        public ServiceDependency[] DependsOn;

        private ServiceConfig Config;

        private ServiceConfigUtil ConfigUtil;

        /// <summary>
        /// Fully qualified names must be used even for local accounts,
        /// e.g.: ".\LOCAL_ACCOUNT". Valid only when ServiceType is ownProcess.
        /// </summary>
        [Xml]
        public string Account;

        /// <summary>
        /// Contains any command line arguments or properties required to run the service.
        /// </summary>
        [Xml]
        public string Arguments;

        /// <summary>
        /// Determines whether the existing service description will be ignored. If 'yes', the service description will be null,
        /// even if the Description attribute is set.
        /// </summary>
        [Xml]
        public bool? EraseDescription;

        /// <summary>
        /// Whether or not the service interacts with the desktop.
        /// </summary>
        [Xml]
        public bool? Interactive;

        /// <summary>
        /// The load ordering group that this service should be a part of.
        /// </summary>
        [Xml]
        public string LoadOrderGroup;

        /// <summary>
        /// The password for the account. Valid only when the account has a password.
        /// </summary>
        [Xml]
        public string Password;

        /// <summary>
        /// The overall install should fail if this service fails to install.
        /// </summary>
        [Xml]
        public bool? Vital;

        #region ServiceConfig attributes

        /// <summary>
        /// Specifies whether an auto-start service should delay its start until after all other auto-start services.
        /// This property only affects auto-start services. If this property is not initialized the setting is not configured.
        /// </summary>
        public bool? DelayedAutoStart;

        ///public object FailureActionsWhen { get; set; } //note implementing util:serviceconfig instead
        /// <summary>	            ConfigUtil?.Process(newContext);
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

        #endregion ServiceConfig attributes

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

        #endregion Util:ServiceConfig attributes

        /// <summary>
        /// The URL reservations associated with the service
        /// </summary>
        public IGenericEntity[] UrlReservations = new IGenericEntity[0];

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            Config = new ServiceConfig
            {
                DelayedAutoStart = DelayedAutoStart,
                PreShutdownDelay = PreShutdownDelay,
                ServiceSid = ServiceSid,
                ConfigureServiceTrigger = ConfigureServiceTrigger,
            };

            ConfigUtil = new ServiceConfigUtil
            {
                FirstFailureActionType = FirstFailureActionType,
                SecondFailureActionType = SecondFailureActionType,
                ThirdFailureActionType = ThirdFailureActionType,
                ProgramCommandLine = ProgramCommandLine,
                RebootMessage = RebootMessage,
                ResetPeriodInDays = ResetPeriodInDays,
                RestartServiceDelayInSeconds = RestartServiceDelayInSeconds,
            };

            XElement ServiceInstaller = this.ToXElement("ServiceInstall");
            context.XParent.Add(ServiceInstaller);

            var newContext = new ProcessingContext
            {
                Project = context.Project,
                Parent = context.Project,
                XParent = ServiceInstaller,
                FeatureComponents = context.FeatureComponents,
            };

            if (DependsOn != null)
            {
                foreach (ServiceDependency dependency in DependsOn)
                {
                    dependency.Process(newContext);
                }
            }

            Config?.Process(newContext);

            if (UrlReservations != null)
            {
                foreach (IGenericEntity urlReservation in UrlReservations)
                {
                    urlReservation.Process(newContext);
                }
            }

            ConfigUtil?.Process(newContext);

            StopOn?.Process(context);
            StartOn?.Process(context);
            RemoveOn?.Process(context);
        }
    }
}