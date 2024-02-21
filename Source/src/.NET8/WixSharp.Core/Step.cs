namespace WixSharp
{
    //Standard Actions Reference: https://msdn.microsoft.com/en-us/library/aa372023(v=vs.85).aspx

    /// <summary>
    /// Specifies predefined values for <see cref="Action.Step"/>,
    /// which controls order of <c>Custom Action</c> to be executed.
    /// <para><c>Before</c> or <c>After</c> switch for <c>Custom Action</c> is controlled by <see cref="When"/>.</para>
    /// </summary>
    public class Step
    {
        /// <summary>
        /// A top-level action used for an administrative installation.
        /// </summary>
        public static Step ADMIN = new Step("ADMIN");

        /// <summary>
        /// A top-level action called to install or remove advertised components.
        /// </summary>
        public static Step ADVERTISE = new Step("ADVERTISE");

        /// <summary>
        /// Validates that the free space specified by AVAILABLEFREEREG exists in the registry.
        /// </summary>
        public static Step AllocateRegistrySpace = new Step("AllocateRegistrySpace");

        /// <summary>
        /// Searches for previous versions of products and determines that upgrades are installed.
        /// </summary>
        public static Step AppSearch = new Step("AppSearch");

        /// <summary>
        /// Binds executables to imported DLLs.
        /// </summary>
        public static Step BindImage = new Step("BindImage");

        /// <summary>
        /// Uses file signatures to validate that qualifying products are installed on a system before an upgrade installation is performed.
        /// </summary>
        public static Step CCPSearch = new Step("CCPSearch");

        /// <summary>
        /// Ends the internal installation costing process begun by the CostInitialize action.
        /// </summary>
        public static Step CostFinalize = new Step("CostFinalize");

        /// <summary>
        /// Starts the installation costing process.
        /// </summary>
        public static Step CostInitialize = new Step("CostInitialize");

        /// <summary>
        /// Creates empty folders for components.
        /// </summary>
        public static Step CreateFolders = new Step("CreateFolders");

        /// <summary>
        /// Creates shortcuts.
        /// </summary>
        public static Step CreateShortcuts = new Step("CreateShortcuts");

        /// <summary>
        /// Removes system services.
        /// </summary>
        public static Step DeleteServices = new Step("DeleteServices");

        /// <summary>
        /// Disables rollback for the remainder of the installation.
        /// </summary>
        public static Step DisableRollback = new Step("DisableRollback");

        /// <summary>
        /// Duplicates files installed by the InstallFiles action.
        /// </summary>
        public static Step DuplicateFiles = new Step("DuplicateFiles");

        /// <summary>
        /// Checks the EXECUTEACTION property to determine which top-level action begins the execution sequence, then runs that action.
        /// </summary>
        public static Step ExecuteAction = new Step("ExecuteAction");

        /// <summary>
        /// Initializes disk cost calculation with the installer. Disk costing is not finalized until the CostFinalize action is executed.
        /// </summary>
        public static Step FileCost = new Step("FileCost");

        /// <summary>
        /// Detects correspondence between the Upgrade table and installed products.
        /// </summary>
        public static Step FindRelatedProducts = new Step("FindRelatedProducts");

        /// <summary>
        /// Used in the action sequence to prompt the user for a restart of the system during the installation.
        /// </summary>
        public static Step ForceReboot = new Step("ForceReboot");

        /// <summary>
        /// A top-level action called to install or remove components.
        /// </summary>
        public static Step INSTALL = new Step("INSTALL");

        /// <summary>
        /// Copies the installer database to the administrative installation point.
        /// </summary>
        public static Step InstallAdminPackage = new Step("InstallAdminPackage");

        /// <summary>
        /// Runs a script containing all operations in the action sequence since either the start of the installation or the last InstallFinalize action. Does not end the transaction.
        /// </summary>
        public static Step InstallExecute = new Step("InstallExecute");

        /// <summary>
        /// Copies files from the source to the destination directory.
        /// </summary>
        public static Step InstallFiles = new Step("InstallFiles");

        /// <summary>
        /// Runs a script containing all operations in the action sequence since either the start of the installation or the last InstallFinalize action. Marks the end of a transaction.
        /// </summary>
        public static Step InstallFinalize = new Step("InstallFinalize");

        /// <summary>
        /// Marks the beginning of a transaction.
        /// </summary>
        public static Step InstallInitialize = new Step("InstallInitialize");

        /// <summary>
        /// The InstallSFPCatalogFile action installs the catalogs used by Windows Me for Windows File Protection.
        /// </summary>
        public static Step InstallSFPCatalogFile = new Step("InstallSFPCatalogFile");

        /// <summary>
        /// Verifies that all volumes with attributed costs have sufficient space for the installation.
        /// </summary>
        public static Step InstallValidate = new Step("InstallValidate");

        /// <summary>
        /// Processes the IsolatedComponent table
        /// </summary>
        public static Step IsolateComponents = new Step("IsolateComponents");

        /// <summary>
        /// Evaluates a set of conditional statements contained in the LaunchCondition table that must all evaluate to True before the installation can proceed.
        /// </summary>
        public static Step LaunchConditions = new Step("LaunchConditions");

        /// <summary>
        /// Migrates current feature states to the pending installation.
        /// </summary>
        public static Step MigrateFeatureStates = new Step("MigrateFeatureStates");

        /// <summary>
        /// Locates existing files and moves or copies those files to a new location.
        /// </summary>
        public static Step MoveFiles = new Step("MoveFiles");

        /// <summary>
        /// Configures a service for the system.
        /// Windows Installer 4.5 and earlier:  Not supported.
        /// </summary>
        public static Step MsiConfigureServices = new Step("MsiConfigureServices");

        /// <summary>
        /// Manages the advertisement of common language runtime assemblies and Win32 assemblies that are being installed.
        /// </summary>
        public static Step MsiPublishAssemblies = new Step("MsiPublishAssemblies");

        /// <summary>
        /// Manages the advertisement of common language runtime assemblies and Win32 assemblies that are being removed.
        /// </summary>
        public static Step MsiUnpublishAssemblies = new Step("MsiUnpublishAssemblies");

        /// <summary>
        /// Installs the ODBC drivers, translators, and data sources.
        /// </summary>
        public static Step InstallODBC = new Step("InstallODBC");

        /// <summary>
        /// Registers a service with the system.
        /// </summary>
        public static Step InstallServices = new Step("InstallServices");

        /// <summary>
        /// Queries the Patch table to determine which patches are applied to specific files and then performs the byte-wise patching of the files.
        /// </summary>
        public static Step PatchFiles = new Step("PatchFiles");

        /// <summary>
        /// Registers components, their key paths, and component clients.
        /// </summary>
        public static Step ProcessComponents = new Step("ProcessComponents");

        /// <summary>
        /// Advertises the components specified in the PublishComponent table.
        /// </summary>
        public static Step PublishComponents = new Step("PublishComponents");

        /// <summary>
        /// Writes the feature state of each feature into the system registry
        /// </summary>
        public static Step PublishFeatures = new Step("PublishFeatures");

        /// <summary>
        /// Publishes product information with the system.
        /// </summary>
        public static Step PublishProduct = new Step("PublishProduct");

        /// <summary>
        /// Manages the registration of COM class information with the system.
        /// </summary>
        public static Step RegisterClassInfo = new Step("RegisterClassInfo");

        /// <summary>
        /// The RegisterComPlus action registers COM+ applications.
        /// </summary>
        public static Step RegisterComPlus = new Step("RegisterComPlus");

        /// <summary>
        /// Registers extension related information with the system.
        /// </summary>
        public static Step RegisterExtensionInfo = new Step("RegisterExtensionInfo");

        /// <summary>
        /// Registers installed fonts with the system.
        /// </summary>
        public static Step RegisterFonts = new Step("RegisterFonts");

        /// <summary>
        /// Registers MIME information with the system.
        /// </summary>
        public static Step RegisterMIMEInfo = new Step("RegisterMIMEInfo");

        /// <summary>
        /// Registers product information with the installer and stores the installer database on the local computer.
        /// </summary>
        public static Step RegisterProduct = new Step("RegisterProduct");

        /// <summary>
        /// Registers OLE ProgId information with the system.
        /// </summary>
        public static Step RegisterProgIdInfo = new Step("RegisterProgIdInfo");

        /// <summary>
        /// Registers type libraries with the system.
        /// </summary>
        public static Step RegisterTypeLibraries = new Step("RegisterTypeLibraries");

        /// <summary>
        /// Registers user information to identify the user of a product.
        /// </summary>
        public static Step RegisterUser = new Step("RegisterUser");

        /// <summary>
        /// Deletes files installed by the DuplicateFiles action.
        /// </summary>
        public static Step RemoveDuplicateFiles = new Step("RemoveDuplicateFiles");

        /// <summary>
        /// Modifies the values of environment variables.
        /// </summary>
        public static Step RemoveEnvironmentStrings = new Step("RemoveEnvironmentStrings");

        /// <summary>
        /// Removes installed versions of a product.
        /// </summary>
        public static Step RemoveExistingProducts = new Step("RemoveExistingProducts");

        /// <summary>
        /// Removes files previously installed by the InstallFiles action.
        /// </summary>
        public static Step RemoveFiles = new Step("RemoveFiles");

        /// <summary>
        /// Removes empty folders linked to components set to be removed.
        /// </summary>
        public static Step RemoveFolders = new Step("RemoveFolders");

        /// <summary>
        /// Deletes .ini file information associated with a component specified in the IniFile table.
        /// </summary>
        public static Step RemoveIniValues = new Step("RemoveIniValues");

        /// <summary>
        /// Removes ODBC data sources, translators, and drivers.
        /// </summary>
        public static Step RemoveODBC = new Step("RemoveODBC");

        /// <summary>
        /// Removes an application's registry keys that were created from the Registry table..
        /// </summary>
        public static Step RemoveRegistryValues = new Step("RemoveRegistryValues");

        /// <summary>
        /// Manages the removal of an advertised shortcut whose feature is selected for uninstallation.
        /// </summary>
        public static Step RemoveShortcuts = new Step("RemoveShortcuts");

        /// <summary>
        /// Determines the source location and sets the SourceDir property.
        /// </summary>
        public static Step ResolveSource = new Step("ResolveSource");

        /// <summary>
        /// Uses file signatures to validate that qualifying products are installed on a system before an upgrade installation is performed.
        /// </summary>
        public static Step RMCCPSearch = new Step("RMCCPSearch");

        /// <summary>
        /// Prompts the user for a system restart at the end of the installation.
        /// </summary>
        public static Step ScheduleReboot = new Step("ScheduleReboot");

        /// <summary>
        /// Processes modules in the SelfReg table and registers them if they are installed.
        /// </summary>
        public static Step SelfRegModules = new Step("SelfRegModules");

        /// <summary>
        /// Unregisters the modules in the SelfReg table that are set to be uninstalled.
        /// </summary>
        public static Step SelfUnregModules = new Step("SelfUnregModules");

        /// <summary>
        /// Runs the actions in a table specified by the SEQUENCE property.
        /// </summary>
        public static Step SEQUENCE = new Step("SEQUENCE");

        /// <summary>
        /// Checks the system for existing ODBC drivers and sets target directory for new ODBC drivers.
        /// </summary>
        public static Step SetODBCFolders = new Step("SetODBCFolders");

        /// <summary>
        /// Starts system services.
        /// </summary>
        public static Step StartServices = new Step("StartServices");

        /// <summary>
        /// Stops system services.
        /// </summary>
        public static Step StopServices = new Step("StopServices");

        /// <summary>
        /// Manages the unadvertisement of components from the PublishComponent table and removes information about published components.
        /// </summary>
        public static Step UnpublishComponents = new Step("UnpublishComponents");

        /// <summary>
        /// Removes the selection-state and feature-component mapping information from the system registry.
        /// </summary>
        public static Step UnpublishFeatures = new Step("UnpublishFeatures");

        /// <summary>
        /// Manages the removal of COM classes from the system registry.
        /// </summary>
        public static Step UnregisterClassInfo = new Step("UnregisterClassInfo");

        /// <summary>
        /// The UnregisterComPlus action removes COM+ applications from the registry.
        /// </summary>
        public static Step UnregisterComPlus = new Step("UnregisterComPlus");

        /// <summary>
        /// Manages the removal of extension-related information from the system.
        /// </summary>
        public static Step UnregisterExtensionInfo = new Step("UnregisterExtensionInfo");

        /// <summary>
        /// Removes registration information about installed fonts from the system.
        /// </summary>
        public static Step UnregisterFonts = new Step("UnregisterFonts");

        /// <summary>
        /// Unregisters MIME-related information from the system registry.
        /// </summary>
        public static Step UnregisterMIMEInfo = new Step("UnregisterMIMEInfo");

        /// <summary>
        /// Manages the unregistration of OLE ProgId information with the system.
        /// </summary>
        public static Step UnregisterProgIdInfo = new Step("UnregisterProgIdInfo");

        /// <summary>
        /// Unregisters type libraries with the system.
        /// </summary>
        public static Step UnregisterTypeLibraries = new Step("UnregisterTypeLibraries");

        /// <summary>
        /// Sets ProductID property to the full product identifier.
        /// </summary>
        public static Step ValidateProductID = new Step("ValidateProductID");

        /// <summary>
        /// Modifies the values of environment variables.
        /// </summary>
        public static Step WriteEnvironmentStrings = new Step("WriteEnvironmentStrings");

        /// <summary>
        /// Writes .ini file information.
        /// </summary>
        public static Step WriteIniValues = new Step("WriteIniValues");

        /// <summary>
        /// Sets up registry information.
        /// </summary>
        public static Step WriteRegistryValues = new Step("WriteRegistryValues");

        /// <summary>
        /// The InstallExecuteAgain action runs a script containing all operations in the action sequence since either the start of the installation or the last InstallExecuteAgain action or the last InstallExecute action. The InstallExecute action updates the system without ending the transaction. InstallExecuteAgain performs the same operation as the InstallExecute action but should only be used after InstallExecute.
        /// </summary>
        public static Step InstallExecuteAgain = new Step("InstallExecuteAgain");

        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after the previous action declared in <see cref="Project.Actions"/>.
        /// </summary>
        public static Step PreviousAction = new Step("PreviousAction");

        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after the previous action item in <see cref="Project.Actions"/>.
        /// If <c>Custom Action</c> is the first item in item in <see cref="Project.Actions"/> it will be executed before/after
        /// MSI built-in <c>InstallFinalize</c> action.
        /// </summary>
        public static Step PreviousActionOrInstallFinalize = new Step("PreviousActionOrInstallFinalize"); //if first usage of a CA, same as "InstallFinalize"; otherwise same as "PreviousAction"

        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after the previous action item in <see cref="Project.Actions"/>.
        /// If <c>Custom Action</c> is the first item in item in <see cref="Project.Actions"/> it will be executed before/after
        /// MSI built-in <c>InstallInitialize</c> action.
        /// </summary>
        public static Step PreviousActionOrInstallInitialize = new Step("PreviousActionOrInstallInitialize"); //if first usage of a CA, same as "InstallInitialize"; otherwise same as "PreviousAction"

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Step(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Step(Step value)
        {
            Value = value.ToString();
        }

        /// <summary>
        /// The string value of the Step object
        /// </summary>
        protected string Value;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
