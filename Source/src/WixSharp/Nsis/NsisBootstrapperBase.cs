using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WixSharp.Nsis
{
    public abstract class Package
    {
        public string FileName { get; set; }

        public string OptionName { get; set; }

        public string Arguments { get; set; }
    }

    public class PrimaryPackage : Package
    {
    }

    public class PrerequisitePackage : Package
    {
        public string RegKeyValue { get; set; }

        public bool PostVerify { get; set; } = true;
    }

    public class NsisBootstrapperBase
    {
        public PrerequisitePackage Prerequisite { get; } = new PrerequisitePackage();

        public PrimaryPackage Primary { get; } = new PrimaryPackage();

        /// <summary>
        /// Gets or sets the prerequisite file.
        /// Executables and .ps1, .bat, .cmd, .vbs, .js scripts are supported.
        /// </summary>
        /// <value>The prerequisite file.</value>
        public string PrerequisiteFile {
            get => Prerequisite.FileName;
            set => Prerequisite.FileName = value;
        }

        /// <summary>
        /// Gets or sets the primary setup file.
        /// Executables and .ps1, .bat, .cmd, .vbs, .js scripts are supported.
        /// </summary>
        /// <value>The primary setup file.</value>
        public string PrimaryFile
        {
            get => Primary.FileName;
            set => Primary.FileName = value;
        }

        /// <summary>
        /// Gets or sets the prerequisite registry key value. This value is used to determine if the <see cref="Prerequisite"/> file should be launched.
        /// <para>This value must comply with the following pattern: &lt;RegistryHive&gt;:&lt;KeyPath&gt;:&lt;ValueName&gt;.</para>
        /// <code>PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed";</code>
        /// Existence of the specified registry value at runtime is interpreted as an indication that the <see cref="Prerequisite"/> file has been already installed.
        /// Thus bootstrapper will execute <see cref="Primary"/> file without launching <see cref="Prerequisite"/> file first.
        /// </summary>
        /// <value>The prerequisite registry key value.</value>
        public string PrerequisiteRegKeyValue
        {
            get => Prerequisite.RegKeyValue;
            set => Prerequisite.RegKeyValue = value;
        }

        /// <summary>
        /// Gets or sets the flag which allows you to disable verification of <see cref="PrerequisitePackage.RegKeyValue"/> after the prerequisite setup is completed.
        /// <para>Normally if <c>bootstrapper</c> checkes if <see cref="PrerequisitePackage.RegKeyValue"/> exists stright after the prerequisite installation and starts
        /// the primary setup only if it does.</para>
        /// <para>It is possible to instruct bootstrapper to continue with the primary setup regardless of the prerequisite installation outcome. This can be done
        /// by setting DoNotPostVerifyPrerequisite to <c>true</c> (default is <c>false</c>)</para>
        /// </summary>
        /// <value>The do not post verify prerequisite.</value>
        public bool DoNotPostVerifyPrerequisite
        {
            get => !Prerequisite.PostVerify;
            set => Prerequisite.PostVerify = !value;
        }

        /// <summary>
        /// Gets or sets command line option name for the prerequisite file.
        /// </summary>
        /// <value>The option name of the prerequisite file.</value>
        public string PrerequisiteFileOptionName
        {
            get => Prerequisite.OptionName;
            set => Prerequisite.OptionName = value;
        }

        /// <summary>
        /// Gets or sets command line option name for the primary file.
        /// </summary>
        /// <value>The option name of the primary file.</value>
        public string PrimaryFileOptionName
        {
            get => Primary.OptionName;
            set => Primary.OptionName = value;
        }

        /// <summary>
        /// Gets or sets preset command line arguments for the prerequisite file.
        /// </summary>
        /// <value>The preset command line arguments of the prerequisite file.</value>
        public string PrerequisiteFileArguments
        {
            get => Prerequisite.Arguments;
            set => Prerequisite.Arguments = value;
        }

        /// <summary>
        /// Gets or sets preset command line arguments for the primary file.
        /// </summary>
        /// <value>The preset command line arguments of the primary file.</value>
        public string PrimaryFileArguments
        {
            get => Primary.Arguments;
            set => Primary.Arguments = value;
        }
    }
}
