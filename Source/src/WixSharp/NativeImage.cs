using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
#if WIX3
using WixToolset.Dtf.WindowsInstaller;
#else
using WixToolset.Dtf.WindowsInstaller;
#endif
using static WixSharp.SetupEventArgs;

namespace WixSharp
{
    /// <summary>
    /// Improves the performance of managed applications by creating native images. Requires the .NET Framework 2.0 or newer to be installed on the target machine since it runs NGen.
    /// </summary>
    /// <example>The following is an example of defining an assembly file that needs to have native image generated for x86 platform.
    /// <code>
    /// new Project("MyProduct",
    ///     new Dir(@"%ProgramFiles%\MyCompany\MyProduct",
    ///         new Files(@"Release\Bin\logger.dll",
    ///             new NativeImage { Platform = NativeImagePlatform.x86}),
    ///         ...
    /// </code>
    /// </example>
    /// <seealso cref="WixSharp.WixEntity" />
    /// <seealso cref="WixSharp.IGenericEntity" />
    public class NativeImage : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixEntity"/>.
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para>
        /// <para>If the <see cref="Id"/> value is not specified explicitly by the user the Wix# compiler
        /// generates it automatically insuring its uniqueness.</para>
        /// <remarks>
        ///  Note: The ID auto-generation is triggered on the first access (evaluation) and in order to make the id
        ///  allocation deterministic the compiler resets ID generator just before the build starts. However if you
        ///  accessing any auto-id before the Build*() is called you can it interferes with the ID auto generation and eventually
        ///  lead to the WiX ID duplications. To prevent this from happening either:"
        ///  <para> - Avoid evaluating the auto-generated IDs values before the call to Build*()</para>
        ///  <para> - Set the IDs (to be evaluated) explicitly</para>
        ///  <para> - Prevent resetting auto-ID generator by setting WixEntity.DoNotResetIdGenerator to true";</para>
        /// </remarks>
        /// </summary>
        /// <value>The id.</value>
        [WixSharp.Xml]
        new public string Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }

        /// <summary>
        /// Sets the platform(s) for which native images will be generated.
        /// </summary>
        [WixSharp.Xml]
        public NativeImagePlatform Platform;

        /// <summary>
        /// Sets the priority of generating the native images for this assembly.This attribute's value must be one of the following:
        /// 0 - This is the highest priority, it means that image generation occurs syncronously during the setup process.This option will slow down setup performance.
        /// 1 - This will queue image generation to the NGen service to occur immediately.This option will slow down setup performance.
        /// 2 - This will queue image generation to the NGen service to occur after all priority 1 assemblies have completed.This option will slow down setup performance.
        /// 3 - This is the lowest priority, it will queue image generation to occur when the machine is idle.This option should not slow down setup performance. This is the default value.
        /// </summary>
        [WixSharp.Xml]
        public int? Priority;

        /// <summary>
        /// The directory to use for locating dependent assemblies. For DLL assemblies and assemblies installed to the Global Assembly Cache (GAC), this attribute should be set to the directory of the application which loads this assembly. For EXE assemblies, this attribute does not need to be set because NGen will use the directory of the assembly file by default.
        /// <para></para>
        /// <para>
        /// The value can be in the form of a directory identifier, or a formatted string that resolves to either a directory identifier or a full path to a directory.
        /// </para>
        /// </summary>
        [WixSharp.Xml]
        public string AppBaseDirectory;

        /// <summary>
        /// The application which will load this assembly. For DLL assemblies which are loaded via reflection, this attribute should be set to indicate the application which will load this assembly. The configuration of the application (usually specified via an exe.config file) will be used to determine how to resolve dependencies for this assembly.
        /// <para>
        /// The value can be in the form of a file identifier, or a formatted string that resolves to either a file identifier or a full path to a file.
        /// </para>
        /// <para>
        /// When a shared component is loaded at run time, using the Load method, the application's configuration file determines the dependencies that are loaded for the shared component — for example, the version of a dependency that is loaded. This attribute gives guidance on which dependencies would be loaded at run time in order to figure out which dependency assemblies will also need to have native images generated (assuming the Dependency attribute is not set to "no").
        /// </para>
        /// <para>
        /// This attribute cannot be set if the AssemblyApplication attribute is set on the parent File element (please note that these attributes both refer to the same application assembly but do very different things: specifiying File/@AssemblyApplication will force an assembly to install to a private location next to the indicated application, whereas this AssemblyApplication attribute will be used to help resolve dependent assemblies while generating native images for this assembly).
        /// </para>
        /// </summary>
        [WixSharp.Xml]
        public string AssemblyApplication;

        /// <summary>
        /// Set to "true" to generate native images that can be used under a debugger. The default value is "false".
        /// </summary>
        [WixSharp.Xml]
        public bool? Debug;

        /// <summary>
        /// Set to "false" to generate native images that can be used under a profiler. The default value is "false".
        /// </summary>
        [WixSharp.Xml]
        public bool? Profile;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.NetFx);

            //serialize itself and add to the parent component
            context.XParent
                   .Add(this.ToXElement(WixExtension.NetFx, "NativeImage"));
        }
    }
}