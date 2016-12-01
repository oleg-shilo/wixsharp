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
#endregion
using IO = System.IO;
using System.Collections.Generic;
using System;

namespace WixSharp
{
    /// <summary>
    /// Defines native (un-managed) bootstrapper. The primary usage of <see cref="NativeBootstrapper"/> is to build bootstrappers for automatically installing .NET 
    /// for executing MSIs containing managed Custom Actions (<see cref="ManagedAction"/>).
    /// <para></para>
    /// <remarks>
    /// NativeBootstrapper is subject to the following limitations:
    /// <list type="bullet">
    /// <item><description>Only Win32 native bootstrapper can be built.</description></item>
    /// <item><description>Only one <c>Prerequisite</c> cen be defined.</description></item>
    /// </list>
    /// </remarks>
    /// </summary>
    /// <example>The following is an example of defining and building bootstrapper for installing MyProduct.msi and 
    /// .NET setup (dotnetfx.exe) as prerequisite installation.
    /// <para></para>
    /// <code>
    /// string setup = new NativeBootstrapper
    ///                    {
    ///                        PrerequisiteFile = "C:\Users\Public\Public Downloads\dotnetfx.exe",
    ///                        PrimaryFile = "MyProduct.msi",
    ///                        OutputFile = "setup.exe",
    ///                        PrerequisiteRegKeyValue = @"HKLM:SOFTWARE\Microsoft\.NETFramework:InstallRoot"
    ///                    }
    ///                    .Build();
    /// </code>
    /// </example>
    public partial class NativeBootstrapper
    {
        /// <summary>
        /// Gets or sets the prerequisite file.
        /// </summary>
        /// <value>The prerequisite file.</value>
        public string PrerequisiteFile { set; get; }
        /// <summary>
        /// Gets or sets the primary setup file.
        /// </summary>
        /// <value>The primary setup file.</value>
        public string PrimaryFile { set; get; }
        /// <summary>
        /// Gets or sets the prerequisite registry key value. This value is used to determine if the <see cref="PrerequisiteFile"/> should be launched.
        /// <para>This value must comply with the following pattern: &lt;RegistryHive&gt;:&lt;KeyPath&gt;:&lt;ValueName&gt;.</para>
        /// <code>PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed";</code>
        /// Existence of the specified registry value at runtime is interpreted as an indication that the <see cref="PrerequisiteFile"/> has been alreday installed.
        /// Thus bootstrapper will execute <see cref="PrimaryFile"/> without launching <see cref="PrerequisiteFile"/> first.
        /// </summary>
        /// <value>The prerequisite registry key value.</value>
        public string PrerequisiteRegKeyValue { set; get; }
        /// <summary>
        /// Gets or sets the output file (bootsrtapper) name.
        /// </summary>
        /// <value>The output file name.</value>
        public string OutputFile { set; get; }

        /// <summary>
        /// Gets or sets the flag which allows you to disable verification of <see cref="PrerequisiteRegKeyValue"/> after the prerequisite setup is completed.
        /// <para>Normally if <c>bootstrapper</c> checkes if <see cref="PrerequisiteRegKeyValue"/> exists stright after the prerequisite installation and starts
        /// the primary setup only if it does.</para>
        /// <para>It is possible to instruct bootstrapper to continue with the primary setup regardless of the prerequisite installation outcome. This can be done
        /// by setting DoNotPostVerifyPrerequisite to <c>true</c> (default is <c>false</c>)</para>
        /// </summary>
        /// <value>The do not post verify prerequisite.</value>
        public bool DoNotPostVerifyPrerequisite { set; get; }

        /// <summary>
        /// Gets or sets the optional arguments for the bootstrapper compiler.
        /// </summary>
        /// <value>The optional arguments.</value>
        public string OptionalArguments{ set; get; }

        /// <summary>
        /// Builds bootstrapper file.
        /// </summary>
        /// <returns>Path to the built bootstrapper file. Returns <c>null</c> if bootstrapper cannot be built.</returns>
        public string Build()
        {
            var baseDir = Environment.GetEnvironmentVariable("WIXSHARP_DIR");

            if (!IO.Directory.Exists(baseDir))
                baseDir = IO.Path.GetDirectoryName(this.GetType().Assembly.Location);

            string builderFileName = "nbsbuilder.exe";
            string builderPath = IO.Path.Combine(baseDir, builderFileName);

            if (!IO.File.Exists(builderPath))
                throw new Exception("Bootstrapper builder (" + builderFileName + ") cannot be found in any of the expected locations " +
                                     "(environment variable WIXSHARP_DIR and WixSharp.dll location).");

            Compiler.Run(builderPath,
                         string.Format("\"/out:{0}\" \"/first:{1}\" \"/second:{2}\" \"/reg:{3}\" {4} {5}",
                                        IO.Path.GetFullPath(OutputFile),
                                        IO.Path.GetFullPath(PrerequisiteFile),
                                        IO.Path.GetFullPath(PrimaryFile),
                                        PrerequisiteRegKeyValue,
                                        DoNotPostVerifyPrerequisite ? "/verify:yes" : "/verify:no",
                                        OptionalArguments ?? ""));

            var retval = IO.Path.GetFullPath(OutputFile);
            return IO.File.Exists(retval) ? retval : null;
        }

        ///// <summary>
        ///// Builds the specified prerequisite file.
        ///// </summary>
        ///// <param name="prerequisiteFile">The prerequisite file.</param>
        ///// <param name="primaryFile">The primary file.</param>
        ///// <param name="prerequisiteRegKeyValue">The prerequisite reg key value.</param>
        ///// <param name="outputFile">The output file.</param>
        ///// <returns></returns>
        //static public string Build(string prerequisiteFile, string primaryFile, string prerequisiteRegKeyValue, string outputFile)
        //{
        //    return new NativeBootstrapper
        //                {
        //                    OutputFile = outputFile,
        //                    PrerequisiteFile = prerequisiteFile,
        //                    PrerequisiteRegKeyValue = prerequisiteRegKeyValue,
        //                    PrimaryFile = primaryFile
        //                }
        //                .Build();
        //}
    }
}
