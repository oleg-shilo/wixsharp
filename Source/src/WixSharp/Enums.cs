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

namespace WixSharp
{
    /// <summary>
    /// Specifies predefined values for <see cref="Project.UI"/>,
    /// which control type of User Interface used to interact with user during the installation.
    /// </summary>
    public enum WUI
    {
        /// <summary>
        /// WixUI_ProgressOnly is "no-UI" dialog set which includes only progress bar.
        /// </summary>
        WixUI_ProgressOnly,

        /// <summary>
        /// WixUI_Minimal is the simplest of the built-in WixUI dialog sets.
        /// </summary>
        WixUI_Minimal,

        /// <summary>
        /// WixUI_InstallDir does not allow the user to choose what features to install, but it adds a dialog to
        /// let the user choose a directory where the product will be installed.
        /// </summary>
        WixUI_InstallDir,

        /// <summary>
        /// WixUI_Common is defines "common" built-in dialog set. It is used to define additional
        /// custom dialogs.
        /// </summary>
        WixUI_Common,

        /// <summary>
        /// WixUI_FeatureTree built-in dialog set.
        /// <para>WixUI_FeatureTree is a simpler version of WixUI_Mondo that omits the setup type dialog.</para>
        /// </summary>
        WixUI_FeatureTree,

        /// <summary>
        /// WixUI_Mondo includes the full set of dialogs (hence "Mondo").
        /// </summary>
        WixUI_Mondo,

        /// <summary>
        /// WixUI_Advanced provides the option of a one-click install like WixUI_Minimal, but it also allows directory and feature
        /// selection like other dialog sets if the user chooses to configure advanced options.
        /// </summary>
        WixUI_Advanced
    }

    /// <summary>
    /// Specifies predefined values for <see cref="Action.Return"/>,
    /// which controls invoking type of <c>Custom Actions</c>.
    /// </summary>
    public enum Return
    {
        /// <summary>
        /// Indicates that the custom action will run asynchronously but the installer will wait for the return code at sequence end.
        /// </summary>
        asyncWait,

        /// <summary>
        /// Indicates that the custom action will run asynchronously and execution may continue after the installer terminates.
        /// </summary>
        asyncNoWait,

        /// <summary>
        /// Indicates that the custom action will run synchronously and the return code will be checked for success.
        /// </summary>
        check,

        /// <summary>
        /// Indicates that the custom action will run synchronously and the return code will not be checked.
        /// </summary>
        ignore
    }

    //good read: http://stackoverflow.com/questions/5564619/what-is-the-purpose-of-administrative-installation-initiated-using-msiexec-a

    /// <summary>
    /// Specifies predefined values for <see cref="Action.Sequence" />,
    /// which controls which MSI sequence contains corresponding <c>Custom Action</c>.
    /// </summary>
    public class Sequence
    {
        /// <summary>
        /// <c>Custom Action</c> belongs to <c>InstallExecuteSequence</c>.
        /// </summary>
        public static Sequence InstallExecuteSequence = new Sequence("InstallExecuteSequence");

        /// <summary>
        /// <c>Custom Action</c> belongs to <c>InstallUISequence</c>.
        /// </summary>
        public static Sequence InstallUISequence = new Sequence("InstallUISequence");

        /// <summary>
        /// The AdminExecuteSequence table lists actions that the installer calls in sequence when the top-level ADMIN action is executed.
        /// </summary>
        public static Sequence AdminExecuteSequence = new Sequence("AdminExecuteSequence");

        /// <summary>
        /// The AdminUISequence table lists actions that the installer calls in sequence when the top-level ADMIN action is executed and the internal user interface level is set to full UI or reduced UI. The installer skips the actions in this table if the user interface level is set to basic UI or no UI.
        /// </summary>
        public static Sequence AdminUISequence = new Sequence("AdminUISequence");

        /// <summary>
        /// <c>Custom Action</c> does not belong to any sequence. Use this value when you need <c>Custom Action</c>
        /// to be invoked not from the installation sequence but from another <c>Custom Action</c>.
        /// </summary>
        public static Sequence NotInSequence = new Sequence("NotInSequence");

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Sequence(string value)
        {
            Value = value;
        }

        /// <summary>
        /// The string value of the Sequence object
        /// </summary>
        protected string Value;

        /// <summary>
        /// Gets the string values of the Sequence object. Note there can be more that a single value. For example
        /// Sequence.InstallExecuteSequence | Sequence.InstallUISequence will yield "InstallExecuteSequence" and
        /// "InstallExecuteSequence"
        /// </summary>
        /// <returns></returns>
        public string[] GetValues()
        {
            return (Value ?? "").Split('|');
        }

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

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Sequence operator +(Sequence first, Sequence second)
        {
            return new Sequence(first.Value + "|" + second.Value);
        }

        /// <summary>
        /// Implements the operator |.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Sequence operator |(Sequence first, Sequence second)
        {
            return new Sequence(first.Value + "|" + second.Value);
        }
    }

    /// <summary>
    /// Specifies predefined values for <see cref="Action.Execute"/> attribute,
    /// which controls at what stage of installation script <c>Custom Action</c> will be executed.
    /// </summary>
    public enum Execute
    {
        /// <summary>
        /// Indicates that the custom action will run after successful completion of the installation script (at the end of the installation).
        /// </summary>
        commit,

        /// <summary>
        /// Indicates that the custom action runs in-script (possibly with elevated privileges).
        /// </summary>
        deferred,

        /// <summary>
        /// Indicates that the custom action will only run in the first sequence that runs it.
        /// </summary>
        firstSequence,

        /// <summary>
        /// Indicates that the custom action will run during normal processing time with user privileges. This is the default.
        /// </summary>
        immediate,

        /// <summary>
        /// Indicates that the custom action will only run in the first sequence that runs it in the same process.
        /// </summary>
        oncePerProcess,

        /// <summary>
        /// Indicates that a custom action will run in the rollback sequence when a failure occurs during installation, usually to undo changes made by a deferred custom action.
        /// </summary>
        rollback,

        /// <summary>
        /// Indicates that a custom action should be run a second time if it was previously run in an earlier sequence.
        /// </summary>
        secondSequence
    }

    /// <summary>
    /// Specifies predefined values for <see cref="Action.When"/>,
    /// which controls when the <c>Custom Action</c> occurs relative to
    /// the order controlling <c>Action</c>.
    /// <para>The order-controlling <c>Action</c> is defined by <see cref="Step"/></para>
    /// </summary>
    public enum When
    {
        /// <summary>
        /// Execute after order controlling action.
        /// </summary>
        After,

        /// <summary>
        /// Execute before order controlling action.
        /// </summary>
        Before
    }

    /// <summary>
    /// Use this attribute to specify the privileges required to install the package on Windows Vista and above.
    /// </summary>
    public enum InstallPrivileges
    {
        /// <summary>
        /// Set this value to declare that the package does not require elevated privileges to install.
        /// </summary>
        limited,

        /// <summary>
        /// Set this value to declare that the package requires elevated privileges to install. This is the default value.
        /// </summary>
        elevated
    }

    /// <summary>
    /// Use this attribute to specify the priviliges required to install the package on Windows Vista and above.
    /// </summary>
    public enum InstallScope
    {
        /// <summary>
        /// Set this value to declare that the package is a per-machine installation and requires elevated privileges to install. Sets the ALLUSERS property to 1.
        /// </summary>
        perMachine,

        /// <summary>
        /// Set this value to declare that the package is a per-user installation and does not require elevated privileges to install. Sets the package's InstallPrivileges attribute to "limited."
        /// </summary>
        perUser
    }

    /// <summary>
    /// Sets the default script language (<see cref="IISVirtualDir.DefaultScript"/>) for the Web site.
    /// </summary>
    public enum DefaultScript
    {
        /// <summary>
        /// VBScript
        /// </summary>
        VBScript,

        /// <summary>
        /// JScript
        /// </summary>
        JScript
    }

    /// <summary>
    /// Sets the platform(s) for which native images will be generated.
    /// </summary>
    public class NativeImagePlatform : StringEnum<NativeImagePlatform>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeImagePlatform"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public NativeImagePlatform(string value) : base(value) { }

        /// <summary>
        /// Attempt to generate native images only for the 32-bit version of the .NET Framework on the target machine. If the 32-bit version of the .NET Framework 2.0 or newer is not present on the target machine, native image custom actions will not be scheduled. This is the default value.
        /// </summary>
        public static NativeImagePlatform x86 = new NativeImagePlatform("32bit"); //it's illegal to start member name from digit (e.g. NativeImagePlatform.32bit)

        /// <summary>
        /// Attempt to generate native images only for the 64-bit version of the .NET Framework on the target machine. If a 64-bit version of the .NET Framework 2.0 or newer is not present on the target machine, native image custom actions will not be scheduled.
        /// </summary>
        public static NativeImagePlatform x64 = new NativeImagePlatform("64bit");

        /// <summary>
        /// Attempt to generate native images for the 32-bit and 64-bit versions of the .NET Framework on the target machine. If a version of the .NET Framework 2.0 or newer is not present on the target machine for a processor architecture, native image custom actions will not be scheduled for that processor architecture.
        /// </summary>
        public static NativeImagePlatform all = new NativeImagePlatform("all");
    }

    /// <summary>
    /// Sets the <see cref="T:WixSharp.Project.Package.Platform"/>) for the target platform type.
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Set this value to declare that the package is an x86 package.
        /// </summary>
        x86,

        /// <summary>
        /// Set this value to declare that the package is an ia64 package. This value requires that the InstallerVersion property be set to 200 or greater.
        /// </summary>
        ia64,

        /// <summary>
        ///  Set this value to declare that the package is an x64 package. This value requires that the InstallerVersion property be set to 200 or greater.
        /// </summary>
        x64,

        /// <summary>
        ///  Set this value to declare that the package is an arm package. This value requires that the  InstallerVersion property be set to 500 or greater.
        /// </summary>
        arm
    }

    /// <summary>
    /// Indicates the compression level for a cabinet.
    /// </summary>
    public enum CompressionLevel
    {
#pragma warning disable 1591
        high,
        medium,
        low,
        mszip,
        none
    }

    /// <summary>
    /// Sets the <see cref="T:IISVirtualDir.Certificate.StoreLocation"/> for the Web site certificate.
    /// </summary>
    public enum StoreLocation
    {
        currentUser,
        localMachine
    }

#pragma warning restore 1591

    /// <summary>
    /// Sets the (<see cref="T:IISVirtualDir.Certificate.StoreName"/>) for the Web site certificate.
    /// </summary>
    public enum StoreName
    {
        /// <summary>
        /// Contains the certificates of certificate authorities that the user trusts to issue certificates to others. Certificates
        /// in these stores are normally supplied with the operating system or by the user's network administrator.
        /// </summary>
        ca,

        /// <summary>
        /// Use the "personal" value instead.
        /// </summary>
        my,

        /// <summary>
        /// Contains personal certificates. These certificates will usually have an associated private key. This store is often referred to as the "MY" certificate store.
        /// </summary>
        personal,

        /// <summary>
        ///
        /// </summary>
        request,

        /// <summary>
        /// Contains the certificates of certificate authorities that the user trusts to issue certificates to others. Certificates in these stores are normally supplied with the operating system or by the user's network administrator. Certificates in this store are typically self-signed.
        /// </summary>
        root,

        /// <summary>
        /// Contains the certificates of those that the user normally sends enveloped messages to or receives signed messages from. See MSDN documentation for more information.
        /// </summary>
        otherPeople,

        /// <summary>
        /// Contains the certificates of those directly trusted people and resources.
        /// </summary>
        trustedPeople,

        /// <summary>
        /// Contains the certificates of those publishers who are trusted.
        /// </summary>
        trustedPublisher,
    }

    /// <summary>
    /// Values of the application isolation level of <see cref="IISVirtualDir.Isolation"/> for pre-IIS 6 applications
    /// </summary>
    public enum Isolation
    {
        /// <summary>
        /// Means the application executes within the IIS process.
        /// </summary>
        low,

        /// <summary>
        /// Executes pooled in a separate process.
        /// </summary>
        medium,

        /// <summary>
        /// Means execution alone in a separate process.
        /// </summary>
        high
    }

    /// <summary>
    /// Determines what service action should be taken on an error.
    /// </summary>
    public enum SvcErrorControl
    {
        /// <summary>
        ///Logs the error and continues with the startup operation.
        /// </summary>
        ignore,

        /// <summary>
        ///Logs the error, displays a message box and continues the startup operation.
        /// </summary>
        normal,

        /// <summary>
        /// Logs the error if possible and the system is restarted with the last configuration known to be good. If the last-known-good configuration is being started, the startup operation fails.
        /// </summary>
        critical
    }

    /// <summary>
    /// Determines when the service should be started. The Windows Installer does not support boot or system.
    /// </summary>
    public enum SvcStartType
    {
        /// <summary>
        /// The service will start during startup of the system.
        /// </summary>
        auto,

        /// <summary>
        ///The service will start when the service control manager calls the StartService function.
        /// </summary>
        demand,

        /// <summary>
        /// The service can no longer be started.
        /// </summary>
        disabled,

        /// <summary>
        /// The service is a device driver that will be started by the operating system boot loader. This value is not currently supported by the Windows Installer.
        /// </summary>
        boot,

        /// <summary>
        /// The service is a device driver that will be started by the IoInitSystem function. This value is not currently supported by the Windows Installer.
        /// </summary>
        system
    }

    /// <summary>
    /// The Windows Installer does not currently support kernelDriver or systemDriver. This attribute's value must be one of the following:
    /// </summary>
    public enum SvcType
    {
        /// <summary>
        /// A Win32 service that runs its own process.
        /// </summary>
        ownProcess,

        /// <summary>
        /// A Win32 service that shares a process.
        /// </summary>
        shareProcess,

        /// <summary>
        /// A kernel driver service. This value is not currently supported by the Windows Installer.
        /// </summary>
        kernelDriver,

        /// <summary>
        /// A file system driver service. This value is not currently supported by the Windows Installer.
        /// </summary>
        systemDriver
    }

    /// <summary>
    /// Specifies whether an action occur on install, uninstall or both.
    /// </summary>
    public enum SvcEventType
    {
        /// <summary>
        /// Specifies that occur on install.
        /// </summary>
        install,

        /// <summary>
        /// Specifies that occur on uninstall.
        /// </summary>
        uninstall,

        /// <summary>
        /// Specifies that occur on install and uninstall.
        /// </summary>
        both
    }

    /// <summary>
    /// Indicates how value of the environment variable should be set.
    /// </summary>
    public enum EnvVarPart
    {
        /// <summary>
        /// This value is the entire environmental variable. This is the default.
        /// </summary>
        all,

        /// <summary>
        ///This value is prefixed.
        /// </summary>
        first,

        /// <summary>
        ///This value is appended.
        /// </summary>
        last
    }

    /// <summary>
    /// Specifies whether the environmental variable should be created, set or removed when the parent component is installed.
    /// </summary>
    public enum EnvVarAction
    {
        /// <summary>
        /// Creates the environment variable if it does not exist, then set it during installation. This has no effect on the value of
        /// the environment variable if it already exists.
        /// </summary>
        create,

        /// <summary>
        /// Creates the environment variable if it does not exist, and then set it during installation. If the environment variable exists,
        /// set it during the installation.
        /// </summary>
        set,

        /// <summary>
        /// Removes the environment variable during an installation. The installer only removes an environment variable
        /// during an installation if the name and value of the variable match the entries in the Name and Value attributes.
        /// If you want to remove an environment variable, regardless of its value, do not set the Value attribute.
        /// </summary>
        remove
    }

    /// <summary>
    /// Specifies the architecture for this assembly.
    /// </summary>
    public enum ProcessorArchitecture
    {
        /// <summary>
        /// The file is a .NET Framework assembly that is processor-neutral.
        /// </summary>
        msil,

        /// <summary>
        /// The file is a .NET Framework assembly for the x86 processor.
        /// </summary>
        x86,

        /// <summary>
        /// The file is a .NET Framework assembly for the x64 processor.
        /// </summary>
        x64,

        /// <summary>
        /// The file is a .NET Framework assembly for the ia64 processor.
        /// </summary>
        ia64
    }

    /// <summary>
    /// Specifies the architecture for the driver to be installed.
    /// </summary>
    public enum DriverArchitecture
    {
        /// <summary>
        /// The driver is for the x86 processor.
        /// </summary>
        x86,

        /// <summary>
        /// The driver is for the x64 processor.
        /// </summary>
        x64,

        /// <summary>
        /// The driver is for the ia64 processor.
        /// </summary>
        ia64
    }

    /// <summary>
    /// Specifies what Action should be executed on the RegistryKey when un-/installing
    /// </summary>
    public enum RegistryKeyAction
    {
        /// <summary>
        /// Creates the key, if absent, when the parent component is installed.
        /// </summary>
        create,

        /// <summary>
        /// Creates the key, if absent, when the parent component is installed then remove the key with all its values and subkeys when the parent component is uninstalled.
        /// </summary>
        createAndRemoveOnUninstall,

        /// <summary>
        /// Does nothing; this element is used merely in WiX authoring for organization and does nothing to the final output.
        /// </summary>
        none
    }

    /// <summary>
    /// Determines the initial display of this feature in the feature tree.
    /// </summary>
    public enum FeatureDisplay
    {
        /// <summary>
        /// Initially shows the feature collapsed. This is the default value.
        /// </summary>
        collapse,

        /// <summary>
        ///  Initially shows the feature expanded.
        /// </summary>
        expand,

        /// <summary>
        ///  Prevents the feature from displaying in the user interface.
        /// </summary>
        hidden
    }

    /// <summary>
    /// Bootstrapper variable (<see cref="WixSharp.Bootstrapper.Variable"/>) type.
    /// </summary>
    public enum VariableType
    {
        /// <summary>
        /// The string type
        /// </summary>
        @string,

        /// <summary>
        /// The numeric type
        /// </summary>
        numeric,

        /// <summary>
        /// The version type
        /// </summary>
        version
    }

    /// <summary>
    /// Specify whether the DOM object should use XPath language or the old XSLPattern language (default) as the query language.
    /// </summary>
    public enum XmlFileSelectionLanguage
    {
        /// <summary>
        /// XPath language
        /// </summary>
        XPath,

        /// <summary>
        /// XSLPattern language
        /// </summary>
        XSLPattern,
    }

    /// <summary>
    /// The type of modification to be made to the XML file when the component is installed.
    /// </summary>
    public enum XmlFileAction
    {
        /// <summary>
        /// Creates a new element under the element specified in ElementPath.
        /// The Name attribute is required in this case and specifies the name of the new element.
        /// The Value attribute is not necessary when createElement is specified as the action.
        /// If the Value attribute is set, it will cause the new element's text value to be set.
        /// </summary>
        createElement,

        /// <summary>
        /// Deletes a value from the element specified in the ElementPath.
        /// If Name is specified, the attribute with that name is deleted.
        /// If Name is not specified, the text value of the element specified in the ElementPath is deleted.
        /// The Value attribute is ignored if deleteValue is the action specified.
        /// </summary>
        deleteValue,

        /// <summary>
        /// Sets a value in the element specified in the ElementPath.
        /// If Name is specified, and attribute with that name is set to the value specified in Value.
        /// If Name is not specified, the text value of the element is set.
        /// Value is a required attribute if setValue is the action specified.
        /// </summary>
        setValue,

        /// <summary>
        /// Sets all the values in the elements that match the ElementPath.
        /// If Name is specified, attributes with that name are set to the same value specified in Value.
        /// If Name is not specified, the text values of the elements are set.
        /// Value is a required attribute if setBulkValue is the action specified.
        /// </summary>
        bulkSetValue,
    }

    /// <summary>
    /// Rights for this ACE.
    /// </summary>
    public enum UrlReservationRights
    {
        /// <summary>
        /// The 'register' rights value of the child UrlAce element
        /// </summary>
        register,

        /// <summary>
        /// The 'delete' rights value of the child UrlAce element
        /// </summary>
        @delegate,

        /// <summary>
        /// The 'all' rights value of the child UrlAce element
        /// </summary>
        all,
    }

    /// <summary>
    /// Specifies the behavior when trying to install a URL reservation and it already exists.
    /// </summary>
    public enum UrlReservationHandleExisting
    {
        /// <summary>
        /// Replaces the existing URL reservation (the default).
        /// </summary>
        replace,

        /// <summary>
        /// Keeps the existing URL reservation.
        /// </summary>
        ignore,

        /// <summary>
        /// The installation fails.
        /// </summary>
        fail,
    }

    /// <summary>
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

    /// <summary>
    /// CA assembly validation mode
    /// </summary>
    public enum CAValidation
    {
        /// <summary>
        /// The CA assembly is loaded in the temporary remote AppDomain for validation.
        /// Assembly file is unlocked and at the end of the validation the assembly is unloaded.
        /// </summary>
        InRemoteAppDomain,

        /// <summary>
        /// The CA assembly is loaded in the current AppDomain for validation.
        /// Assembly file is unlocked but the assembly will not be unloaded at the end of the validation.
        /// This mode may lead to unpredictable behaviour.
        /// </summary>
        InCurrentAppDomain,

        /// <summary>
        /// CA assembly validation is disabled.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Sign Tool output level
    /// </summary>
    public enum SignOutputLevel
    {
        /// <summary>
        /// Displays verbose output regardless of whether the command runs successfully or fails,
        /// and displays warning messages.
        /// </summary>
        Verbose,

        /// <summary>
        /// Displays no output if the command runs successfully,
        /// and displays minimal output if the command fails.
        /// </summary>
        Minimal,

        /// <summary>
        /// Displays standard output
        /// </summary>
        Standard,

        /// <summary>
        /// Displays debugging information.
        /// </summary>
        Debug
    }
}