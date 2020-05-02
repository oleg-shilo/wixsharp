using System.Collections.Generic;

namespace WixSharp.Nsis
{
    /// <summary>
    /// Container class for common members of the <see cref="NsisBootstrapper"/> packages.
    /// </summary>
    public abstract class Package
    {
        /// <summary>
        /// Gets or sets the setup package file name.
        /// Executables and .ps1, .bat, .cmd, .vbs, .js scripts are supported.
        /// </summary>
        /// <value>The setup package file name.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the command line option name.
        /// </summary>
        /// <value>The option name.</value>
        public string OptionName { get; set; }

        /// <summary>
        /// Gets or sets preset command line arguments.
        /// </summary>
        /// <value>The preset command line arguments.</value>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to start the process in a new window.
        /// </summary>
        public bool CreateNoWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the operating system shell to start the process.
        /// </summary>
        public bool UseShellExecute { get; set; }

        /// <summary>
        /// Collection of the package dependencies.
        /// </summary>
        public IList<Payload> Payloads { get; } = new List<Payload>();
    }

    /// <summary>
    /// Describes a primary package of the <see cref="NsisBootstrapper"/>.
    /// </summary>
    public class PrimaryPackage : Package
    {
    }

    /// <summary>
    /// Describes a prerequisite package of the <see cref="NsisBootstrapper"/>.
    /// </summary>
    public class PrerequisitePackage : Package
    {
        /// <summary>
        /// Gets or sets the prerequisite registry key value. This value is used to determine if the prerequisite file should be launched.
        /// <para>This value must comply with the following pattern: &lt;RegistryHive&gt;:&lt;KeyPath&gt;:&lt;ValueName&gt;.</para>
        /// <code>Prerequisite.RegKeyValue = @"HKLM:Software\My Company\My Product:Installed";</code>
        /// Existence of the specified registry value at runtime is interpreted as an indication that the prerequisite file has been already installed.
        /// Thus bootstrapper will execute the primary file without launching the prerequisite file first.
        /// </summary>
        /// <value>The prerequisite registry key value.</value>
        public string RegKeyValue { get; set; }

        /// <summary>
        /// Gets or sets the flag which allows you to disable verification of <see cref="RegKeyValue"/> after the prerequisite setup is completed.
        /// <para>Normally if <c>bootstrapper</c> checks if <see cref="RegKeyValue"/> exists straight after the prerequisite installation and starts
        /// the primary setup only if it does.</para>
        /// <para>It is possible to instruct bootstrapper to continue with the primary setup regardless of the prerequisite installation outcome. This can be done
        /// by setting PostVerify to <c>false</c> (default is <c>true</c>)</para>
        /// </summary>
        /// <value>Post verify prerequisite.</value>
        public bool PostVerify { get; set; } = true;
    }
}
