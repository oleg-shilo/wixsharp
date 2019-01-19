namespace WixSharp
{
    /// <summary>
    /// Starts, stops, and removes services for parent Component.
    /// This element is used to control the state of a service installed by the MSI or MSM file by using the start, stop and remove attributes.
    /// </summary>
    public class SvcEvent : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Specifies when the service action occur. It can be one of the <see cref="SvcEventType"/> values.
        /// </summary>
        public SvcEventType Type;

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.install"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation without
        /// waiting for the completion.
        /// </summary>
        public static SvcEvent Install => new SvcEvent { Type = SvcEventType.install, Wait = false };

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.install"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with
        /// waiting for the completion.
        /// </summary>
        public static SvcEvent Install_Wait => new SvcEvent { Type = SvcEventType.install, Wait = true };

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.uninstall"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation
        /// without waiting for the completion.
        /// </summary>
        public static SvcEvent Uninstall => new SvcEvent { Type = SvcEventType.uninstall, Wait = false };

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.uninstall"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with
        /// waiting for the completion.
        /// </summary>
        public static SvcEvent Uninstall_Wait => new SvcEvent { Type = SvcEventType.uninstall, Wait = true };

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.both"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation without
        /// waiting for the completion.
        /// </summary>
        public static SvcEvent InstallUninstall => new SvcEvent { Type = SvcEventType.both, Wait = false };

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.both"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with
        /// waiting for the completion.
        /// </summary>
        public static SvcEvent InstallUninstall_Wait => new SvcEvent { Type = SvcEventType.both, Wait = true };

        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// Name of the service.
        /// </summary>
        [Xml]
        public new string Name { get => base.Name; set => base.Name = value; }

        /// <summary>
        /// Specifies whether the service should be removed by the DeleteServices action on install, uninstall or both.
        /// For 'install', the service will be removed only when the parent component is being installed (msiInstallStateLocal or msiInstallStateSource);
        /// for 'uninstall', the service will be removed only when the parent component is being removed (msiInstallStateAbsent);
        /// for 'both', the service will be removed in both cases.
        /// </summary>
        [Xml]
        public SvcEventType? Remove;

        /// <summary>
        /// Specifies whether the service should be started by the StartServices action on install, uninstall or both.
        /// For 'install', the service will be started only when the parent component is being installed (msiInstallStateLocal or msiInstallStateSource);
        /// for 'uninstall', the service will be started only when the parent component is being removed (msiInstallStateAbsent);
        /// for 'both', the service will be started in both cases.
        /// </summary>
        [Xml]
        public SvcEventType? Start;

        /// <summary>
        /// Specifies whether the service should be stopped by the StopServices action on install, uninstall or both.
        /// For 'install', the service will be stopped only when the parent component is being installed (msiInstallStateLocal or msiInstallStateSource);
        /// for 'uninstall', the service will be stopped only when the parent component is being removed (msiInstallStateAbsent);
        /// for 'both', the service will be stopped in both cases.
        /// </summary>
        [Xml]
        public SvcEventType? Stop;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvcEvent"/> class.
        /// </summary>
        public SvcEvent()
        {
        }

        /// <summary>
        /// The flag indicating if after triggering the service action the setup should wait until te action is completed.
        /// </summary>
        [Xml]
        public bool? Wait;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.XParent.Add(this.ToXElement("ServiceControl"));
        }
    }
}