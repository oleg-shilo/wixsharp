#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2016 Oleg Shilo

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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using WixSharp.Utilities;
using WixToolset.Dtf.WindowsInstaller;
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// This class holds the settings for various WixSharp auto-generation aspects that are not directly related to the 
    /// WXS XML schema. IE enforcement of ComponentId uniqness, ID allocation, etc.
    /// <p>
    /// The detailed information about Wix# auto-generation can be found 
    /// here: http://www.csscript.net/WixSharp/ID_Allocation.html.
    /// </p>
    /// <p> There is also additional configuration settings class that controls pure XML generation aspects: 
    /// <see cref="WixSharp.AutoElements"/>.
    /// </p>
    /// </summary>
    public class AutoGenerationOptions
    {
        /// <summary>
        /// The ID of the root directory, which is automatically assigned to the first directory of the project directories.
        /// <para>
        /// The default value "INSTALLDIR" has special meaning as apart from being a traditional choice of the id for the root
        /// directory it is also used in binding root directory to the textbox of the <c>InstallDirDialog</c>.
        /// </para>
        /// <para>
        /// It is important to have the id as public (upper case). Otherwise WiX doesn't produce correct MSI.
        /// </para>
        /// The auto-assignment can be disabled by setting InstallDirDefaultId to null.
        /// </summary>
        public string InstallDirDefaultId = "INSTALLDIR";

        /// <summary>
        /// Extra parameters that is passed to the `dotnet publish` command when AOT compiling .NET Core custom actions.
        /// <code language="C#">
        /// // ignore the directory build props that are not compatible with AOT compilation
        /// WixSharp.Compiler.AutoGeneration.AotBuildParameters = "-p:ImportDirectoryBuildProps=false";
        /// </code>
        /// </summary>
        public string AotBuildParameters = "";

        /// <summary>
        /// Flag indicating if all system folders (e.g. %ProgramFiles%) should be auto-mapped into their x64 equivalents
        /// when 'project.Platform = Platform.x64'
        /// </summary>
        public bool Map64InstallDirs = true;

        /// <summary>
        /// The prefer batch signing. When signing sign multiple files this option indicates whether to sign them as a batch or 
        /// one by one. While it's usually faster to sign multiple files as a batch sometimes it may be difficult to manage. 
        /// So signing one by one may be preferred at this stage of maturity of this feature.
        /// </summary>
        public bool PreferBatchSigning = false;

        /// <summary>
        /// Forces the WXS to be thread-safe. Default value is <c>false</c>.
        /// <para>
        /// If this field is useful when WixShar accesses some shared resources during WXS generation.
        /// For example: using <see cref="CustomIdAlgorithm"/> that uses project's "target path resolver"
        /// during multiple concurrent builds.
        /// </para>
        /// </summary>
        public bool IsWxsGenerationThreadSafe = false;

        internal object WxsGenerationSynchObject
        {
            get => IsWxsGenerationThreadSafe ? typeof(WixProject) : new object();
        }

        /// <summary>
        /// The custom algorithm for generating WiX elements IDs.
        /// </summary>
        ///<example>The following code demonstrates how to generate File IDs based is on the hash
        ///of the target path of the file being installed.
        ///<code>
        ///WixEntity.CustomIdAlgorithm =
        ///       entity =>
        ///       {
        ///           if (entity is File file)
        ///           {
        ///               var target_path = project.GetTargetPathOf(file);
        ///               var hash = target_path.GetHashCode32();
        ///
        ///               // WiX does not allow '-' char in ID. So need to use `Math.Abs`
        ///               return $"{target_path.PathGetFileName()}_{Math.Abs(hash)}";
        ///           }
        ///
        ///           return null; // pass to default ID generator
        ///       };
        /// </code>
        /// </example>
        public Func<WixEntity, string> CustomIdAlgorithm = null;

        /// <summary>
        /// The legacy default identifier algorithm
        /// </summary>
        public bool LegacyDefaultIdAlgorithm = false;

        /// <summary>
        /// Force Component ID uniqueness.
        /// </summary>
        public bool ForceComponentIdUniqueness = false;

        /// <summary>
        /// The option for adding managed custom action dependencies automatically.
        /// <p> This option is a convenient way to add all the dependencies automatically setting them manually. 
        /// The compiler analyzes the CA assembly dependencies and adds them to the CA binary that is packaged 
        /// with "WixToolset.Dtf.MakeSfxCA.exe". </p>
        /// <p>Note, this method may unnecessarily increase the size of the msi as not all CA dependency assemblies
        /// may be required at runtime (during the installation).</p>
        /// </summary>
        public bool AddManagedCustomActionDependencies = false;

        /// <summary>
        /// Remove media if no files are included
        /// </summary>
        public bool RemoveMediaIfNoFiles = true;

        /// <summary>
        /// The ignore empty directories during wild card resolution
        /// </summary>
        public bool IgnoreWildCardEmptyDirectories = false;

        /// <summary>
        /// Enable validating CA assemblies for all CA methods to be public instance method.
        /// <para>
        /// By default validation is performed in a temporary AppDomain (`CAValidation.InRemoteAppDomain`)
        /// as it is the only way to avoid locking the assembly being validated. Unfortunately
        /// `ReflectionOnlyLoadFrom` is incompatible with this task as it does not allow browsing members
        /// attributes.
        /// </para>
        /// <para>
        /// You may need to disable (`CAValidation.Disabled`) the validation if WixSharp is hosted on the
        /// runtime that does not support AppDomain unloading (e.g. .NET Core)</para>
        /// <para>
        /// If you want to perform validation without the use of temporary AppDomain you can chose the
        /// `CAValidation.InCurrentAppDomain` option. In this case the assembly being validated is loaded in
        /// the current AppDomain but from the memory without locking the assembly file. This option is valid
        /// but not recommended as it may lead to the unpredictable reflection scenarios. </para>
        /// </summary>
        public CAValidation ValidateCAAssemblies = CAValidation.InRemoteAppDomain;

        /// <summary>
        /// The suppress generation of the XML attribute 'id' for <see cref="Bootstrapper.Payload"/> undefined ids
        /// </summary>
        public bool SuppressForBundlePayloadUndefinedIds = true;

        /// <summary>
        /// The template for <see cref="WixSharp.Project.HashedTargetPathIdAlgorithm"/> auto-generated File id.
        /// <para>
        /// The default value is "{file_name}_{dir_hash}".
        /// </para>
        /// </summary>
        public string HashedTargetPathIdAlgorithm_FileIdMask = "{file_name}_{dir_hash}";
    }
}