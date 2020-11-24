namespace WixSharp
{
    /// <summary>
    /// Sets ACLs on File, Registry, CreateFolder, or ServiceInstall. When under a Registry element, this cannot be used if the Action attribute's value is remove or removeKeyOnInstall. This element has no Id attribute. The table and key are taken from the parent element.
    /// </summary>
    /// <seealso cref="WixSharp.WixEntity" />
    public class PermissionEx : WixEntity
    {
        [Xml]
        public bool? Append;

        [Xml]
        public bool? ChangePermission;

        /// <summary>
        /// For a directory, the right to create a subdirectory. Only valid under a 'CreateFolder' parent.
        /// </summary>
        /// </summary>
        [Xml]
        public bool? CreateChild;

        /// <summary>
        /// For a directory, the right to create a file in the directory. Only valid under a 'CreateFolder' parent.
        /// </summary>
        /// </summary>
        [Xml]
        public bool? CreateFile;

        [Xml]
        public bool? CreateLink;

        [Xml]
        public bool? CreateSubkeys;

        [Xml]
        public bool? Delete;

        /// <summary>
        /// For a directory, the right to delete a directory and all the files it contains, including read-only files. Only valid under a 'CreateFolder' parent.
        /// </summary>
        [Xml]
        public bool? DeleteChild;

        [Xml]
        public string Domain;

        [Xml]
        public bool? EnumerateSubkeys;

        [Xml]
        public bool? Execute;

        [Xml]
        public bool? GenericAll;

        [Xml]
        public bool? GenericExecute;

        /// <summary>
        /// specifying this will fail to grant read access
        /// </summary>
        [Xml]
        public bool? GenericRead;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? GenericWrite;

        /// <summary>
        /// WiX element description is not available
        /// </summary>

        [Xml]
        public bool? Notify;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? Read;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? ReadAttributes;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? ReadExtendedAttributes;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? ReadPermission;

        /// <summary>
        /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServiceChangeConfig;

        /// <summary>
        /// Required to call the EnumDependentServices function to enumerate all the services dependent on the service. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServiceEnumerateDependents;

        /// <summary>
        /// Required to call the ControlService function to ask the service to report its status immediately. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServiceInterrogate;

        /// <summary>
        /// Required to call the ControlService function to pause or continue the service. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServicePauseContinue;

        /// <summary>
        /// Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServiceQueryConfig;

        /// <summary>
        /// Required to call the QueryServiceStatus function to ask the service control manager about the status of the service. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServiceQueryStatus;

        /// <summary>
        /// Required to call the StartService function to start the service. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServiceStart;

        /// <summary>
        /// Required to call the ControlService function to stop the service. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServiceStop;

        /// <summary>
        /// Required to call the ControlService function to specify a user-defined control code. Only valid under a 'ServiceInstall' parent.
        /// </summary>
        [Xml]
        public bool? ServiceUserDefinedControl;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? Synchronize;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? TakeOwnership;

        /// <summary>
        /// For a directory, the right to traverse the directory. By default, users are assigned the BYPASS_TRAVERSE_CHECKING privilege, which ignores the FILE_TRAVERSE access right. Only valid under a 'CreateFolder' parent.
        /// </summary>
        [Xml]
        public bool? Traverse;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public string User;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? Write;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? WriteAttributes;

        /// <summary>
        /// WiX element description is not available
        /// </summary>
        [Xml]
        public bool? WriteExtendedAttributes;
    }
}