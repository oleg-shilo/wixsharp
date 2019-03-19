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
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.Controls;
using IO = System.IO;

namespace WixSharp.CommonTasks
{
    /// <summary>
    ///
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// Builds the bootstrapper.
        /// </summary>
        /// <param name="prerequisiteFile">The prerequisite file.</param>
        /// <param name="primaryFile">The primary setup file.</param>
        /// <param name="outputFile">The output (bootsrtapper) file.</param>
        /// <param name="prerequisiteRegKeyValue">The prerequisite registry key value.
        /// <para>This value is used to determine if the <c>PrerequisiteFile</c> should be launched.</para>
        /// <para>This value must comply with the following pattern: &lt;RegistryHive&gt;:&lt;KeyPath&gt;:&lt;ValueName&gt;.</para>
        /// <code>PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed";</code>
        /// Existence of the specified registry value at runtime is interpreted as an indication of the <c>PrerequisiteFile</c> has been alreday installed.
        /// Thus bootstrapper will execute <c>PrimaryFile</c> without launching <c>PrerequisiteFile</c> first.</param>
        /// <param name="doNotPostVerifyPrerequisite">The flag which allows you to disable verification of <c>PrerequisiteRegKeyValue</c> after the prerequisite setup is completed.
        /// <para>Normally if <c>bootstrapper</c> checks if <c>PrerequisiteRegKeyValue</c>/> exists straight after the prerequisite installation and starts
        /// the primary setup only if it does.</para>
        /// <para>It is possible to instruct bootstrapper to continue with the primary setup regardless of the prerequisite installation outcome. This can be done
        /// by setting DoNotPostVerifyPrerequisite to <c>true</c> (default is <c>false</c>)</para>
        ///</param>
        /// <param name="optionalArguments">The optional arguments for the bootstrapper compiler.</param>
        /// <returns>Path to the built bootstrapper file. Returns <c>null</c> if bootstrapper cannot be built.</returns>
        ///
        /// <example>The following is an example of building bootstrapper <c>Setup.msi</c> for deploying .NET along with the <c>MyProduct.msi</c> file.
        /// <code>
        /// WixSharp.CommonTasks.Tasks.BuildBootstrapper(
        ///         @"C:\downloads\dotnetfx.exe",
        ///         "MainProduct.msi",
        ///         "setup.exe",
        ///         @"HKLM:Software\My Company\My Product:Installed"
        ///         false,
        ///         "");
        /// </code>
        /// </example>
        static public string BuildBootstrapper(string prerequisiteFile, string primaryFile, string outputFile, string prerequisiteRegKeyValue, bool doNotPostVerifyPrerequisite, string optionalArguments)
        {
            var nbs = new NativeBootstrapper
            {
                PrerequisiteFile = prerequisiteFile,
                PrimaryFile = primaryFile,
                OutputFile = outputFile,
                PrerequisiteRegKeyValue = prerequisiteRegKeyValue
            };

            nbs.DoNotPostVerifyPrerequisite = doNotPostVerifyPrerequisite;

            if (!optionalArguments.IsEmpty())
                nbs.OptionalArguments = optionalArguments;

            return nbs.Build();
        }

        /// <summary>
        /// Builds the bootstrapper.
        /// </summary>
        /// <param name="prerequisiteFile">The prerequisite file.</param>
        /// <param name="primaryFile">The primary setup file.</param>
        /// <param name="outputFile">The output (bootsrtapper) file.</param>
        /// <param name="prerequisiteRegKeyValue">The prerequisite registry key value.
        /// <para>This value is used to determine if the <c>PrerequisiteFile</c> should be launched.</para>
        /// <para>This value must comply with the following pattern: &lt;RegistryHive&gt;:&lt;KeyPath&gt;:&lt;ValueName&gt;.</para>
        /// <code>PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed";</code>
        /// Existence of the specified registry value at runtime is interpreted as an indication of the <c>PrerequisiteFile</c> has been already installed.
        /// Thus bootstrapper will execute <c>PrimaryFile</c> without launching <c>PrerequisiteFile</c> first.</param>
        /// <param name="doNotPostVerifyPrerequisite">The flag which allows you to disable verification of <c>PrerequisiteRegKeyValue</c> after the prerequisite setup is completed.
        /// <para>Normally if <c>bootstrapper</c> checkers if <c>PrerequisiteRegKeyValue</c>/> exists straight after the prerequisite installation and starts
        /// the primary setup only if it does.</para>
        /// <para>It is possible to instruct bootstrapper to continue with the primary setup regardless of the prerequisite installation outcome. This can be done
        /// by setting DoNotPostVerifyPrerequisite to <c>true</c> (default is <c>false</c>)</para>
        ///</param>
        /// <returns>Path to the built bootstrapper file. Returns <c>null</c> if bootstrapper cannot be built.</returns>
        ///
        /// <example>The following is an example of building bootstrapper <c>Setup.msi</c> for deploying .NET along with the <c>MyProduct.msi</c> file.
        /// <code>
        /// WixSharp.CommonTasks.Tasks.BuildBootstrapper(
        ///         @"C:\downloads\dotnetfx.exe",
        ///         "MainProduct.msi",
        ///         "setup.exe",
        ///         @"HKLM:Software\My Company\My Product:Installed"
        ///         false);
        /// </code>
        /// </example>
        static public string BuildBootstrapper(string prerequisiteFile, string primaryFile, string outputFile, string prerequisiteRegKeyValue, bool doNotPostVerifyPrerequisite)
        {
            return BuildBootstrapper(prerequisiteFile, primaryFile, outputFile, prerequisiteRegKeyValue, doNotPostVerifyPrerequisite, null);
        }

        /// <summary>
        /// Builds the bootstrapper.
        /// </summary>
        /// <param name="prerequisiteFile">The prerequisite file.</param>
        /// <param name="primaryFile">The primary setup file.</param>
        /// <param name="outputFile">The output (bootsrtapper) file.</param>
        /// <param name="prerequisiteRegKeyValue">The prerequisite registry key value.
        /// <para>This value is used to determine if the <c>PrerequisiteFile</c> should be launched.</para>
        /// <para>This value must comply with the following pattern: &lt;RegistryHive&gt;:&lt;KeyPath&gt;:&lt;ValueName&gt;.</para>
        /// <code>PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed";</code>
        /// Existence of the specified registry value at runtime is interpreted as an indication of the <c>PrerequisiteFile</c> has been already installed.
        /// Thus bootstrapper will execute <c>PrimaryFile</c> without launching <c>PrerequisiteFile</c> first.</param>
        /// <returns>Path to the built bootstrapper file. Returns <c>null</c> if bootstrapper cannot be built.</returns>
        static public string BuildBootstrapper(string prerequisiteFile, string primaryFile, string outputFile, string prerequisiteRegKeyValue)
        {
            return BuildBootstrapper(prerequisiteFile, primaryFile, outputFile, prerequisiteRegKeyValue, false, null);
        }

        /// <summary>
        /// Applies digital signature to a file (e.g. msi, exe, dll) with MS <c>SignTool.exe</c> utility.
        /// Please use <see cref="DigitalySignBootstrapper"/> for signing a bootstrapper.
        /// </summary>
        /// <param name="fileToSign">The file to sign.</param>
        /// <param name="pfxFile">Specify the signing certificate in a file. If this file is a PFX with a password, the password may be supplied
        /// with the <c>password</c> parameter.</param>
        /// <param name="timeURL">The timestamp server's URL. If this option is not present (pass to null), the signed file will not be timestamped.
        /// A warning is generated if timestamping fails.</param>
        /// <param name="password">The password to use when opening the PFX file. Should be <c>null</c> if no password required.</param>
        /// <param name="optionalArguments">Extra arguments to pass to the <c>SignTool.exe</c> utility.</param>
        /// <param name="wellKnownLocations">The optional ';' separated list of directories where SignTool.exe can be located.
        /// If this parameter is not specified WixSharp will try to locate the SignTool in the built-in well-known locations (system PATH)</param>
        /// <param name="useCertificateStore">A flag indicating if the value of <c>pfxFile</c> is a name of the subject of the signing certificate
        /// from the certificate store (as opposite to the certificate file). This value can be a substring of the entire subject name.</param>
        /// <param name="dualSign">A flag indicating if the file should be signed with both SHA1 and SHA256.</param>
        /// <param name="outputLevel">A flag indicating the output level</param>
        /// <returns>Exit code of the <c>SignTool.exe</c> process.</returns>
        ///
        /// <example>The following is an example of signing <c>Setup.msi</c> file.
        /// <code>
        /// WixSharp.CommonTasks.Tasks.DigitalySign(
        ///     "Setup.msi",
        ///     "MyCert.pfx",
        ///     "http://timestamp.verisign.com/scripts/timstamp.dll",
        ///     "MyPassword",
        ///     null,
        ///     false);
        /// </code>
        /// </example>
        static public int DigitalySign(string fileToSign, string pfxFile, string timeURL, string password,
            string optionalArguments = null, string wellKnownLocations = null, bool useCertificateStore = false,
            bool dualSign = false, SignOutputLevel outputLevel = SignOutputLevel.Verbose)
        {
            //SHA1: "C:\Program Files\\Microsoft SDKs\Windows\v6.0A\bin\signtool.exe" sign /f "pfxFile" /p password /v "fileToSign" /t timeURL
            //SHA256: "C:\Program Files\\Microsoft SDKs\Windows\v6.0A\bin\signtool.exe" sign /f "pfxFile" /p password /v "fileToSign" /tr timeURL /td sha256 /fd sha256 /as
            //string args = "sign /v /f \"" + pfxFile + "\" \"" + fileToSign + "\"";

            string certPlace = useCertificateStore ? "/n" : "/f";

            string outputLevelArg = string.Empty;
            switch (outputLevel)
            {
                case SignOutputLevel.Minimal:
                    outputLevelArg = "/q ";
                    break;

                case SignOutputLevel.Standard:
                    break;

                case SignOutputLevel.Verbose:
                    outputLevelArg = "/v ";
                    break;

                case SignOutputLevel.Debug:
                    outputLevelArg = "/debug ";
                    break;
            }

            string args = $"sign {outputLevelArg}{certPlace} \"{pfxFile}\"";
            if (password.IsNotEmpty())
                args += $" /p \"{password}\"";

            string sha1 = args;
            if (timeURL != null)
                sha1 += $" /t \"{timeURL}\"";
            if (!optionalArguments.IsEmpty())
                sha1 += " " + optionalArguments;

            sha1 += $" \"{fileToSign}\"";

            var tool = new ExternalTool
            {
                WellKnownLocations = wellKnownLocations ??
                                     @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.15063.0\x86;" +
                                     @"C:\Program Files (x86)\Windows Kits\10\bin\x86;" +
                                     @"C:\Program Files (x86)\Windows Kits\8.1\bin\x86;" +
                                     @"C:\Program Files (x86)\Windows Kits\8.0\bin\x86;" +
                                     @"C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool;" +
                                     @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin;" +
                                     @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin",
                ExePath = "signtool.exe",
                Arguments = sha1
            };

            if (password.IsNotEmpty())
                tool.ConsoleOut = (line) => Compiler.OutputWriteLine(line.Replace(password, "***"));

            var retval = tool.ConsoleRun();
            var sha1Signed = retval == 0 || retval == 2;
            if (!dualSign || !sha1Signed)
                return retval;

            // Append SHA-256 signature
            string sha256 = args + " /as /fd sha256";
            if (timeURL != null)
                sha256 += $" /tr \"{timeURL}\" /td sha256";
            if (!optionalArguments.IsEmpty())
                sha256 += " " + optionalArguments;

            sha256 += $" \"{fileToSign}\"";
            tool.Arguments = sha256;
            return tool.ConsoleRun();
        }

        /// <summary>
        /// Applies digital signature to a bootstrapper and the bootstrapper engine with MS <c>SignTool.exe</c> utility.
        /// <a href="http://wixtoolset.org/documentation/manual/v3/overview/insignia.html">See more about bootstrapper engine signing</a>
        /// </summary>
        /// <param name="bootstrapperFileToSign">The bootstrapper file to sign.</param>
        /// <param name="pfxFile">Specify the signing certificate in a file. If this file is a PFX with a password, the password may be supplied
        /// with the <c>password</c> parameter.</param>
        /// <param name="timeURL">The timestamp server's URL. If this option is not present (pass to null), the signed file will not be timestamped.
        /// A warning is generated if timestamping fails.</param>
        /// <param name="password">The password to use when opening the PFX file. Should be <c>null</c> if no password required.</param>
        /// <param name="optionalArguments">Extra arguments to pass to the <c>SignTool.exe</c> utility.</param>
        /// <param name="wellKnownLocations">The optional ';' separated list of directories where SignTool.exe can be located.
        /// If this parameter is not specified WixSharp will try to locate the SignTool in the built-in well-known locations (system PATH)</param>
        /// <param name="useCertificateStore">A flag indicating if the value of <c>pfxFile</c> is a name of the subject of the signing certificate
        /// from the certificate store (as opposite to the certificate file). This value can be a substring of the entire subject name.</param>
        /// <param name="dualSign">A flag indicating if the file should be signed with both SHA1 and SHA256.</param>
        /// <param name="outputLevel">A flag indicating the output level</param>
        /// <returns>Exit code of the <c>SignTool.exe</c> process.</returns>
        ///
        /// <example>The following is an example of signing <c>SetupBootstrapper.exe</c> file.
        /// <code>
        /// WixSharp.CommonTasks.Tasks.DigitalySignBootstrapper(
        ///     "SetupBootstrapper.exe",
        ///     "MyCert.pfx",
        ///     "http://timestamp.verisign.com/scripts/timstamp.dll",
        ///     "MyPassword",
        ///     null);
        /// </code>
        /// </example>
        static public int DigitalySignBootstrapper(string bootstrapperFileToSign, string pfxFile, string timeURL, string password,
            string optionalArguments = null, string wellKnownLocations = null, bool useCertificateStore = false, bool dualSign = false, SignOutputLevel outputLevel = SignOutputLevel.Verbose)
        {
            var retval = DigitalySignBootstrapperEngine(bootstrapperFileToSign, pfxFile, timeURL, password, optionalArguments, wellKnownLocations, useCertificateStore, dualSign, outputLevel);
            if (retval != 0)
                return retval;

            return DigitalySign(bootstrapperFileToSign, pfxFile, timeURL, password, optionalArguments, wellKnownLocations, useCertificateStore, dualSign, outputLevel);
        }

        /// <summary>
        /// Applies digital signature to a bootstrapper engine with MS <c>SignTool.exe</c> utility.
        /// Note : this method doesn't sign the bootstrapper file but the engine only.
        /// Please use <see cref="DigitalySignBootstrapper"/> for signing both (bootstrapper and bootstrapper engine) instead.
        /// <a href="http://wixtoolset.org/documentation/manual/v3/overview/insignia.html">See more about Bootstrapper engine signing</a>
        /// </summary>
        /// <param name="bootstrapperFileToSign">The Bootstrapper file to sign.</param>
        /// <param name="pfxFile">Specify the signing certificate in a file. If this file is a PFX with a password, the password may be supplied
        /// with the <c>password</c> parameter.</param>
        /// <param name="timeURL">The timestamp server's URL. If this option is not present (pass to null), the signed file will not be timestamped.
        /// A warning is generated if timestamping fails.</param>
        /// <param name="password">The password to use when opening the PFX file. Should be <c>null</c> if no password required.</param>
        /// <param name="optionalArguments">Extra arguments to pass to the <c>SignTool.exe</c> utility.</param>
        /// <param name="wellKnownLocations">The optional ';' separated list of directories where SignTool.exe can be located.
        /// If this parameter is not specified WixSharp will try to locate the SignTool in the built-in well-known locations (system PATH)</param>
        /// <param name="useCertificateStore">A flag indicating if the value of <c>pfxFile</c> is a name of the subject of the signing certificate
        /// from the certificate store (as opposite to the certificate file). This value can be a substring of the entire subject name.</param>
        /// <param name="dualSign">A flag indicating if the file should be signed with both SHA1 and SHA256.</param>
        /// <param name="outputLevel">A flag indicating the output level</param>
        /// <returns>Exit code of the <c>SignTool.exe</c> process.</returns>
        ///
        /// <example>The following is an example of signing <c>SetupBootstrapper.exe</c> file engine.
        /// <code>
        /// WixSharp.CommonTasks.Tasks.DigitalySignBootstrapperEngine(
        ///     "SetupBootstrapper.exe",
        ///     "MyCert.pfx",
        ///     "http://timestamp.verisign.com/scripts/timstamp.dll",
        ///     "MyPassword",
        ///      null);
        /// </code>
        /// </example>
        static public int DigitalySignBootstrapperEngine(string bootstrapperFileToSign, string pfxFile, string timeURL, string password,
            string optionalArguments = null, string wellKnownLocations = null, bool useCertificateStore = false, bool dualSign = false, SignOutputLevel outputLevel = SignOutputLevel.Verbose)
        {
            var insigniaPath = IO.Path.Combine(Compiler.WixLocation, "insignia.exe");
            string enginePath = IO.Path.GetTempFileName();

            try
            {
                var tool = new ExternalTool
                {
                    ExePath = insigniaPath,
                    Arguments = "-ib \"{0}\" -o \"{1}\"".FormatWith(bootstrapperFileToSign, enginePath)
                };

                var retval = tool.ConsoleRun();
                if (retval != 0)
                    return retval;

                retval = DigitalySign(enginePath, pfxFile, timeURL, password, optionalArguments, wellKnownLocations, useCertificateStore, dualSign, outputLevel);
                if (retval != 0)
                    return retval;

                tool = new ExternalTool
                {
                    ExePath = insigniaPath,
                    Arguments = "-ab \"{1}\" \"{0}\" -o \"{0}\"".FormatWith(bootstrapperFileToSign, enginePath)
                };

                tool.ConsoleRun();
                return retval;
            }
            finally
            {
                IO.File.Delete(enginePath);
            }
        }

        //static Task<T> ExecuteInNewContext<T>(Func<T> action)
        //{
        //    var taskResult = new TaskCompletionSource<T>();

        //    var asyncFlow = ExecutionContext.SuppressFlow();

        //    try
        //    {
        //        Task.Run(() =>
        //        {
        //            try
        //            {
        //                var result = action();

        //                taskResult.SetResult(result);
        //            }
        //            catch (Exception exception)
        //            {
        //                taskResult.SetException(exception);
        //            }
        //        })
        //            .Wait();
        //    }
        //    finally
        //    {
        //        asyncFlow.Undo();
        //    }

        //    return taskResult.Task;
        //}
        //static public string EmmitComWxs(string fileIn, string fileOut = null, string extraArgs = null)
        //{
        //    if (fileOut == null)
        //        fileOut = IO.Path.ChangeExtension(fileIn, "wxs");

        //    //heat.exe fileIn -gg -out fileOut

        //    string args = $"\"{fileIn}\" -gg -out \"{fileOut}\" {extraArgs}";

        //    var tool = new ExternalTool
        //    {
        //        WellKnownLocations = Compiler.WixLocation,
        //        ExePath = "heat.exe",
        //        Arguments = args
        //    };
        //    if (tool.ConsoleRun() == 0)
        //        return fileOut;
        //    else
        //        return null;
        //}

        /// <summary>
        /// Applies digital signature to a file (e.g. msi, exe, dll) with MS <c>SignTool.exe</c> utility.
        /// <para>If you need to specify extra SignTool.exe parameters or the location of the tool use the overloaded <c>DigitalySign</c> signature </para>
        /// </summary>
        /// <param name="fileToSign">The file to sign.</param>
        /// <param name="pfxFile">Specify the signing certificate in a file. If this file is a PFX with a password, the password may be supplied
        /// with the <c>password</c> parameter.</param>
        /// <param name="timeURL">The timestamp server's URL. If this option is not present, the signed file will not be timestamped.
        /// A warning is generated if timestamping fails.</param>
        /// <param name="password">The password to use when opening the PFX file.</param>
        /// <returns>Exit code of the <c>SignTool.exe</c> process.</returns>
        ///
        /// <example>The following is an example of signing <c>Setup.msi</c> file.
        /// <code>
        /// WixSharp.CommonTasks.Tasks.DigitalySign(
        ///     "Setup.msi",
        ///     "MyCert.pfx",
        ///     "http://timestamp.verisign.com/scripts/timstamp.dll",
        ///     "MyPassword");
        /// </code>
        /// </example>
        static public int DigitalySign(string fileToSign, string pfxFile, string timeURL, string password)
        {
            return DigitalySign(fileToSign, pfxFile, timeURL, password, null);
        }

        /// <summary>
        /// Imports the reg file.
        /// </summary>
        /// <param name="regFile">The reg file.</param>
        /// <returns></returns>
        /// <example>The following is an example of importing registry entries from the *.reg file.
        /// <code>
        /// var project =
        ///     new Project("MyProduct",
        ///         new Dir(@"%ProgramFiles%\My Company\My Product",
        ///             new File(@"readme.txt")),
        ///         ...
        ///
        /// project.RegValues = CommonTasks.Tasks.ImportRegFile("app_settings.reg");
        ///
        /// Compiler.BuildMsi(project);
        /// </code>
        /// </example>
        static public RegValue[] ImportRegFile(string regFile)
        {
            return RegFileImporter.ImportFrom(regFile);
        }

        /// <summary>
        /// Imports the reg file. It is nothing else but an extension method version of the 'plain' <see cref="T:WixSharp.CommonTasks.Tasks.ImportRegFile"/>.
        /// </summary>
        /// <param name="project">The project object.</param>
        /// <param name="regFile">The reg file.</param>
        /// <returns></returns>
        /// <example>The following is an example of importing registry entries from the *.reg file.
        /// <code>
        /// var project =
        ///     new Project("MyProduct",
        ///         new Dir(@"%ProgramFiles%\My Company\My Product",
        ///             new File(@"readme.txt")),
        ///         ...
        ///
        /// project.ImportRegFile("app_settings.reg");
        ///
        /// Compiler.BuildMsi(project);
        /// </code>
        /// </example>
        static public Project ImportRegFile(this Project project, string regFile)
        {
            project.RegValues = ImportRegFile(regFile);
            return project;
        }

        /// <summary>
        /// Adds the property items to the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Project AddProperties(this Project project, params Property[] items)
        {
            project.Properties = project.Properties.Combine(items).Distinct().ToArray();
            return project;
        }

        /// <summary>
        /// Adds the property to the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Project AddProperty(this Project project, Property item)
        {
            return project.AddProperties(item);
        }

        /// <summary>
        /// Adds the action items to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Project AddActions(this Project project, params Action[] items)
        {
            project.Actions = project.Actions.Combine(items).Distinct().ToArray();
            return project;
        }

        /// <summary>
        /// Adds the action to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Project AddAction(this Project project, Action item)
        {
            return project.AddActions(item);
        }

        /// <summary>
        /// Adds the directory to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Project AddDir(this Project project, Dir item)
        {
            return project.AddDirs(item);
        }

        /// <summary>
        /// Adds the directory items to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Project AddDirs(this Project project, params Dir[] items)
        {
            project.Dirs = project.Dirs.Combine(items).Distinct().ToArray();
            return project;
        }

        /// <summary>
        /// Adds the registry value to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Project AddRegValue(this Project project, RegValue item)
        {
            return project.AddRegValues(item);
        }

        /// <summary>
        /// Adds the registry values to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Project AddRegValues(this Project project, params RegValue[] items)
        {
            project.RegValues = project.RegValues.Combine(items).Distinct().ToArray();
            return project;
        }

        /// <summary>
        /// Adds the UrlReservation to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Project AddUrlReservation(this Project project, UrlReservation item)
        {
            return project.AddUrlReservations(item);
        }

        /// <summary>
        /// Adds the UrlReservations to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Project AddUrlReservations(this Project project, params UrlReservation[] items)
        {
            project.UrlReservations = project.UrlReservations.Combine(items).Distinct().ToArray();
            return project;
        }

        /// <summary>
        /// Adds the binary items to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Project AddBinaries(this Project project, params Binary[] items)
        {
            project.Binaries = project.Binaries.Combine(items).Distinct().ToArray();
            return project;
        }

        /// <summary>
        /// Adds the binary to the Project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Project AddBinary(this Project project, Binary item)
        {
            return project.AddBinaries(item);
        }

        /// <summary>
        /// Localizes the string from the specified localization delegate 'localize'.
        /// <para>This field is initialized by ManagedUI with the localization routine
        /// that is specific for the MSI being executed. You can use this delegate to
        /// do the localization of user content at runtime.</para>
        /// <code>
        /// var localized = Tasks.UILocalize("A later version of [ProductName] is already installed. Setup will now exit.");
        /// </code>
        /// </summary>
        public static Func<string, string> UILocalize = null;

        /// <summary>
        /// Delegate for detection of the "downgrade" condition. Should return <c>true</c> if the downgrading is detected.
        /// </summary>
        /// <param name="thisVersion">The version of the product being installed.</param>
        /// <param name="installedVersion">The detected installed product version.</param>
        /// <returns></returns>
        public delegate bool DowngradeErrorCheck(Version thisVersion, Version installedVersion);

        /// <summary>
        /// Adds the assembly reference to the ManagedAction.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        static public ManagedAction AddRefAssembly(this ManagedAction action, string file)
        {
            return action.AddRefAssemblies(file);
        }

        /// <summary>
        /// Creates a new <see cref="Version"/> object from based on <c>version</c> with the revision part omitted.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        static public Version ClearRevision(this Version version)
        {
            return new Version(version.Major, version.Minor, version.Build);
        }

        /// <summary>
        /// Creates a new <see cref="Version"/> object from based on <c>version</c> with the revision part omitted.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        static public Version ClearRevision(this FileVersionInfo version)
        {
            return new Version(version.FileMajorPart, version.FileMinorPart, version.FileBuildPart);
        }

        /// <summary>
        /// Adds the assembly references to the ManagedAction.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        static public ManagedAction AddRefAssemblies(this ManagedAction action, params string[] files)
        {
            action.RefAssemblies = action.RefAssemblies.Combine(files).Distinct().ToArray();
            return action;
        }

        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds the file association items to the File.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public File AddAssociations(this File file, params FileAssociation[] items)
        {
            file.Associations = file.Associations.Combine(items).Distinct().ToArray();
            return file;
        }

        // public static void AddFileAssociationIcon(this Project project, string exeId, string icon)
        // {
        //     project.WixSourceGenerated += doc =>
        //     {
        //         var iconPath = icon.PathGetFullPath();
        //         var iconId = WixEntity.IncrementalIdFor(new WixEntity { Name = iconPath });

        //         var progId = doc.FindAll("File")
        //                         .First(x => x.HasAttribute("Id", exeId))
        //                         .Parent("Component")
        //                         .FindFirst("ProgId");

        //         if (progId.HasAttribute("Advertise", "no"))
        //             throw new Exception($"You can only add icon to the advertised file association (File.Id: {exeId}).");

        //         progId.SetAttribute("Icon", iconId);

        //         doc.FindSingle("Product")
        //                 .AddElement("Icon", $@"Id={iconId};SourceFile={iconPath}");
        //     };
        // }

        /// <summary>
        /// Adds the file association to the File.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public File AddAssociation(this File file, FileAssociation item)
        {
            return file.AddAssociations(item);
        }

        /// <summary>
        /// Adds the shortcut items to the File.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public File AddShortcuts(this File file, params FileShortcut[] items)
        {
            file.Shortcuts = file.Shortcuts.Combine(items).Distinct().ToArray();
            return file;
        }

        /// <summary>
        /// Adds the shortcut to the File.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public File AddShortcut(this File file, FileShortcut item)
        {
            return file.AddShortcuts(item);
        }

        //////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds the directory items to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Dir AddDirs(this Dir dir, params Dir[] items)
        {
            dir.Dirs = dir.Dirs.Combine(items).Distinct().ToArray();
            return dir;
        }

        /// <summary>
        /// Adds the directory to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Dir AddDir(this Dir dir, Dir item)
        {
            return dir.AddDirs(item);
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Dir AddFiles(this Dir dir, params File[] items)
        {
            dir.Files = dir.Files.Combine(items).Distinct().ToArray();
            return dir;
        }

        /// <summary>
        /// Adds the specified <see cref="WixSharp.IGenericEntity"/> items.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Dir Add(this Dir dir, params IGenericEntity[] items)
        {
            dir.GenericItems = dir.GenericItems.Combine(items).Distinct().ToArray();
            return dir;
        }

        /// <summary>
        /// Adds the specified <see cref="WixSharp.IGenericEntity"/> items.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Project Add(this Project project, params IGenericEntity[] items)
        {
            project.GenericItems = project.GenericItems.Combine(items).Distinct().ToArray();
            return project;
        }

        /// <summary>
        /// Adds the specified <see cref="WixSharp.IGenericEntity"/> items.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public File Add(this File file, params IGenericEntity[] items)
        {
            file.GenericItems = file.GenericItems.Combine(items).Distinct().ToArray();
            return file;
        }

        /// <summary>
        /// Adds the file to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Dir AddFile(this Dir dir, File item)
        {
            return dir.AddFiles(item);
        }

        /// <summary>
        /// Adds the exe shortcut to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Dir AddExeShortcuts(this Dir dir, params ExeFileShortcut[] items)
        {
            dir.Shortcuts = dir.Shortcuts.Combine(items).Distinct().ToArray();
            return dir;
        }

        /// <summary>
        /// Adds the exe shortcut to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Dir AddExeShortcut(this Dir dir, ExeFileShortcut item)
        {
            return dir.AddExeShortcuts(item);
        }

        /// <summary>
        /// Adds the features to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Dir AddFeatures(this Dir dir, params Feature[] items)
        {
            dir.Features = dir.Features.Combine(items).Distinct().ToArray();
            return dir;
        }

        /// <summary>
        /// Adds the feature to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Dir AddFeature(this Dir dir, Feature item)
        {
            return dir.AddFeatures(item);
        }

        /// <summary>
        /// Adds the merge modules to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Dir AddMergeModules(this Dir dir, params Merge[] items)
        {
            dir.MergeModules = dir.MergeModules.Combine(items).Distinct().ToArray();
            return dir;
        }

        /// <summary>
        /// Adds the merge module to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Dir AddMergeModule(this Dir dir, Merge item)
        {
            return dir.AddMergeModules(item);
        }

        /// <summary>
        /// Adds the file collection to teh Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Dir AddFileCollections(this Dir dir, params Files[] items)
        {
            dir.FileCollections = dir.FileCollections.Combine(items).Distinct().ToArray();
            return dir;
        }

        /// <summary>
        /// Adds the file collections to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Dir AddFileCollection(this Dir dir, Files item)
        {
            return dir.AddFileCollections(item);
        }

        /// <summary>
        /// Adds the directory file collection to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        static public Dir AddDirFileCollection(this Dir dir, DirFiles item)
        {
            return dir.AddDirFileCollections(item);
        }

        /// <summary>
        /// Adds the directory file collections to the Dir.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        static public Dir AddDirFileCollections(this Dir dir, params DirFiles[] items)
        {
            dir.DirFileCollections = dir.DirFileCollections.Combine(items).Distinct().ToArray();
            return dir;
        }

        /// <summary>
        /// Removes the dialogs between specified two dialogs. It simply connects 'next' button of the start dialog with the
        /// 'NewDialog' action associated with the end dialog. And vise versa for the 'back' button.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        /// <example>The following is an example of the setup that skips License dialog.
        /// <code>
        /// project.UI = WUI.WixUI_InstallDir;
        /// project.RemoveDialogsBetween(Dialogs.WelcomeDlg, Dialogs.InstallDirDlg);
        /// ...
        /// Compiler.BuildMsi(project);
        /// </code>
        /// </example>
        static public Project RemoveDialogsBetween(this Project project, string start, string end)
        {
            if (project.CustomUI == null)
                project.CustomUI = new Controls.DialogSequence();

            project.CustomUI.On(start, Controls.Buttons.Next, new Controls.ShowDialog(end) { Order = Controls.DialogSequence.DefaultOrder });
            project.CustomUI.On(end, Controls.Buttons.Back, new Controls.ShowDialog(start) { Order = Controls.DialogSequence.DefaultOrder });
            return project;
        }

        /// <summary>
        /// Sets the Project version from the file version of the file specified by it's ID.
        /// <para>This method sets project WixSourceGenerated event handler and injects
        /// "!(bind.FileVersion.&lt;file ID&gt;" into the XML Product's Version attribute.</para>
        ///
        /// <remarks>
        /// If <c>SetVersionFrom</c> is used then Wix# is no longer responsible for setting the final product version.
        /// This task is delegated to WiX so the whole value `project.Version` be replaced with the binding
        /// expression just before passing XML to the WiX compiler.
        /// <para>Note, this methods still sets `Project.Version` as it is needed in order to
        /// maintain auto-generating project/product identities.
        /// </para>
        /// You can also consider using <c>SetVersionFromFile/SetVersionFromFileId</c> as an alternative approach.
        /// </remarks>
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="setProjectVersionAsWell">if set to <c>true</c> set `Project.Version` as well. Needed to
        /// maintain auto-generating project/product identities.</param>
        /// <returns></returns>
        static public Project SetVersionFrom(this Project project, string fileId, bool setProjectVersionAsWell = true)
        {
            project.SetVersionFromIdValue = fileId;
            project.WixSourceGenerated += document =>
                document.FindSingle("Product")
                        .AddAttributes("Version=!(bind.FileVersion." + fileId + ")");

            if (setProjectVersionAsWell)
            {
                var file = project.AllFiles.FirstOrDefault(x => x.Id == fileId);
                if (file != null)
                {
                    var file_path = Utils.PathCombine(project.SourceBaseDir, file.Name);
                    if (IO.File.Exists(file_path))
                        project.Version = new Version(FileVersionInfo.GetVersionInfo(file_path).FileVersion);
                }
                else
                    Console.WriteLine("Warning: ");
            }
            return project;
        }

        /// <summary>
        /// Extracts file version from the file with a specific Id.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fileId">The file id.</param>
        /// <returns></returns>
        static public string ExtractVersionFrom(this Project project, string fileId)
        {
            var file = project.FindFile(x => x.Id == fileId).FirstOrDefault();
            if (file != null)
                try
                {
                    return FileVersionInfo.GetVersionInfo(file.Name).FileVersion;
                }
                catch { }
            return "";
        }

        /// <summary>
        /// Sets the version of the project to the version value retrieved from the file.
        /// <para>If the file is an assembly then the assembly version is returned.</para>
        /// <para>If the file is an MSI then the product version is returned.</para>
        /// <para>If the file is a native binary then file version is returned.</para>
        /// </summary>
        /// <remarks>
        /// Attempt to extract the assembly version may fail because the dll/exe file may not be an assembly
        /// or because it can be in the wrong assembly format (x64 vs x86). In any case the method will fall back to
        /// the file version.</remarks>
        /// <param name="project">The project.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        static public Project SetVersionFromFile(this Project project, string filePath)
        {
            project.Version = GetVersionFromFile(filePath);
            return project;
        }

        /// <summary>
        /// Sets the version of the project to the version value retrieved from the file being a part of the installation.
        /// <para>If the file is an assembly then the assembly version is returned.</para>
        /// <para>If the file is a native binary then file version is returned.</para>
        /// </summary>
        /// <remarks>
        /// Attempt to extract the assembly version may fail because the dll/exe file may not be an assembly
        /// or because it can be in the wrong assembly format (x64 vs x86). In any case the method will fall back to
        /// the file version.</remarks>
        /// <param name="project">The project.</param>
        /// <param name="fileId">The file path.</param>
        /// <returns></returns>
        static public Project SetVersionFromFileId(this Project project, string fileId)
        {
            project.Version = project.ExtractVersionFrom(fileId).ToRawVersion();
            return project;
        }

        /// <summary>
        /// Extracts value retrieved from the file.
        /// <para>If the file is an assembly then the assembly version is returned.</para>
        /// <para>If the file is an MSI then the product version is returned.</para>
        /// <para>If the file is a native binary then file version is returned.</para>
        /// </summary>
        /// <remarks>
        /// Attempt to extract the assembly version may fail because the dll/exe file may not be an assembly
        /// or because it can be in the wrong assembly format (x64 vs x86). In any case the method will fall back to
        /// the file version.</remarks>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        static public Version GetVersionFromFile(string filePath)
        {
            string version = null;

            try
            {
                var file = IO.Path.GetFullPath(filePath);
                if (file.EndsWith(".msi", ignoreCase: true))
                {
                    using (var database = new Database(file, DatabaseOpenMode.ReadOnly))
                    {
                        using (var view = database.OpenView(database.Tables["Property"].SqlSelectString))
                        {
                            view.Execute();
                            version = view.Where(r => r.GetString("Property") == "ProductVersion")
                                          .Select(r => r.GetString("Value"))
                                          .FirstOrDefault();
                        }
                    }
                }
                else
                {
                    version = FileVersionInfo.GetVersionInfo(filePath).FileVersion;
                    if (file.EndsWith(".dll", ignoreCase: true) || file.EndsWith(".exe", ignoreCase: true))
                    {
                        try
                        {
                            version = System.Reflection.Assembly.ReflectionOnlyLoadFrom(file).GetName().Version.ToString();
                        }
                        catch { }
                    }
                }
            }
            catch { }

            if (version == null)
                throw new Exception("Cannot extract version from '" + filePath + "'");

            return new Version(version);
        }

        /// <summary>
        /// Injects CLR dialog between MSI dialogs 'prevDialog' and 'nextDialog'.
        /// Passes custom action CLR method name (showDialogMethod) for instantiating and popping up the CLR dialog.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="showDialogMethod">The show dialog method.</param>
        /// <param name="prevDialog">The previous dialog.</param>
        /// <param name="nextDialog">The next dialog.</param>
        /// <returns></returns>
        /// <example>The following is an example of inserting CustomDialog dialog into the UI sequence between MSI dialogs InsallDirDlg and VerifyReadyDlg.
        /// <code>
        /// public class static Script
        /// {
        ///     public static void Main()
        ///     {
        ///         var project = new Project("CustomDialogTest");
        ///
        ///         project.InjectClrDialog("ShowCustomDialog", Dialogs.InstallDirDlg, Dialogs.VerifyReadyDlg);
        ///         Compiler.BuildMsi(project);
        ///     }
        ///
        ///     [CustomAction]
        ///     public static ActionResult ShowCustomDialog(Session session)
        ///     {
        ///         return WixCLRDialog.ShowAsMsiDialog(new CustomDialog(session));
        ///     }
        ///}
        /// </code>
        /// </example>
        static public Project InjectClrDialog(this Project project, string showDialogMethod, string prevDialog, string nextDialog)
        {
            string wixSharpAsm = typeof(Project).Assembly.Location;
            string wixSharpUIAsm = IO.Path.ChangeExtension(wixSharpAsm, ".UI.dll");

            var showClrDialog = new ManagedAction(showDialogMethod)
            {
                Sequence = Sequence.NotInSequence,
            };

            project.DefaultRefAssemblies.Add(wixSharpAsm);
            project.DefaultRefAssemblies.Add(wixSharpUIAsm);

            //Must use WixUI_Common as other UI type has predefined dialogs already linked between each other and WiX does not allow overriding events
            //http://stackoverflow.com/questions/16961493/override-publish-within-uiref-in-wix
            project.UI = WUI.WixUI_Common;

            if (project.CustomUI != null)
                throw new ApplicationException("Project.CustomUI is already initialized. Ensure InjectClrDialog is invoked before any adjustments made to CustomUI.");

            project.CustomUI = new CommomDialogsUI();
            project.AddAction(showClrDialog);

            //disconnect prev and next dialogs
            project.CustomUI.UISequence.ForEach(x =>
                                        {
                                            if ((x.Dialog == prevDialog && x.Control == Buttons.Next) || (x.Dialog == nextDialog && x.Control == Buttons.Back))
                                                x.Actions.RemoveAll(a => a is ShowDialog);
                                        });
            project.CustomUI.UISequence.RemoveAll(x => x.Actions.Count == 0);

            //create new dialogs connection with showAction in between
            project.CustomUI.On(prevDialog, Buttons.Next, new ExecuteCustomAction(showClrDialog))
                            .On(prevDialog, Buttons.Next, new ShowDialog(nextDialog, Condition.ClrDialog_NextPressed))
                            .On(prevDialog, Buttons.Next, new CloseDialog("Exit", Condition.ClrDialog_CancelPressed) { Order = 2 })

                            .On(nextDialog, Buttons.Back, new ExecuteCustomAction(showClrDialog))
                            .On(nextDialog, Buttons.Back, new ShowDialog(prevDialog, Condition.ClrDialog_BackPressed));

            var installDir = project.AllDirs.FirstOrDefault(d => d.HasItemsToInstall());
            if (installDir != null && project.CustomUI.Properties.ContainsKey("WIXUI_INSTALLDIR"))
                project.CustomUI.Properties["WIXUI_INSTALLDIR"] = installDir.RawId ?? Compiler.AutoGeneration.InstallDirDefaultId;

            return project;
        }

        //not ready yet. Investigation is in progress
        static internal Project InjectClrDialogInFeatureTreeUI(this Project project, string showDialogMethod, string prevDialog, string nextDialog)
        {
            string wixSharpAsm = typeof(Project).Assembly.Location;
            string wixSharpUIAsm = IO.Path.ChangeExtension(wixSharpAsm, ".UI.dll");

            var showClrDialog = new ManagedAction(showDialogMethod)
            {
                Sequence = Sequence.NotInSequence,
                RefAssemblies = new[] { wixSharpAsm, wixSharpUIAsm }
            };

            project.UI = WUI.WixUI_FeatureTree;

            if (project.CustomUI != null)
                throw new ApplicationException("Project.CustomUI is already initialized. Ensure InjectClrDialog is invoked before any adjustments made to CustomUI.");

            project.CustomUI = new DialogSequence();
            project.AddAction(showClrDialog);

            //disconnect prev and next dialogs
            project.CustomUI.UISequence.RemoveAll(x => (x.Dialog == prevDialog && x.Control == Buttons.Next) ||
                                                  (x.Dialog == nextDialog && x.Control == Buttons.Back));

            //create new dialogs connection with showAction in between
            project.CustomUI.On(prevDialog, Buttons.Next, new ExecuteCustomAction(showClrDialog))
                            .On(prevDialog, Buttons.Next, new ShowDialog(nextDialog, Condition.ClrDialog_NextPressed))
                            .On(prevDialog, Buttons.Next, new CloseDialog("Exit", Condition.ClrDialog_CancelPressed) { Order = 2 })

                            .On(nextDialog, Buttons.Back, new ExecuteCustomAction(showClrDialog))
                            .On(nextDialog, Buttons.Back, new ShowDialog(prevDialog, Condition.ClrDialog_BackPressed));

            return project;
        }

        /// <summary>
        /// Gets the file version.
        /// </summary>
        /// <param name="file">The path to the file.</param>
        /// <returns></returns>
        static public Version GetFileVersion(string file)
        {
            var info = FileVersionInfo.GetVersionInfo(file);
            //cannot use info.FileVersion as it can include description string
            return new Version(info.FileMajorPart,
                               info.FileMinorPart,
                               info.FileBuildPart,
                               info.FilePrivatePart);
        }

        /// <summary>
        /// Binds the LaunchCondition to the <c>version</c> condition based on WiXNetFxExtension properties.
        /// <para>The typical conditions are:</para>
        /// <para>   NETFRAMEWORK20='#1'</para>
        /// <para>   NETFRAMEWORK40FULL='#1'</para>
        /// <para>   NETFRAMEWORK35='#1'</para>
        /// <para>   NETFRAMEWORK30_SP_LEVEL and NOT NETFRAMEWORK30_SP_LEVEL='#0'</para>
        /// <para>   ...</para>
        /// The full list of names and values can be found here http://wixtoolset.org/documentation/manual/v3/customactions/wixnetfxextension.html
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="versionCondition">Condition expression.
        /// </param>
        /// <param name="errorMessage">The error message to be displayed if .NET version is not present.</param>
        /// <returns></returns>
        static public Project SetNetFxPrerequisite(this WixSharp.Project project, string versionCondition, string errorMessage = null)
        {
            var condition = Condition.Create(versionCondition);
            string message = errorMessage ?? "Please install the appropriate .NET version first.";

            project.LaunchConditions.Add(new LaunchCondition(condition, message));

            foreach (var prop in condition.GetDistinctProperties())
                project.AddProperty(new PropertyRef(prop));

            project.Include(WixExtension.NetFx);

            return project;
        }

        /// <summary>
        /// Adds the XML fragment to the element specified by <c>placementPath</c>.
        /// <para>Note <c>placementPath</c> can only contain forward slashes.</para>
        /// <example>
        /// The following is an example of adding `Log` element to the `Wix/Bundle` element of the
        /// bootstrapper project.
        /// <code>
        /// bootstrapper.AddXml("Wix/Bundle", "&lt;Log PathVariable=\"LogFileLocation\"/&gt;");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="placementPath">The placement path.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        static public WixProject AddXml(this WixProject project, string placementPath, string xml)
        {
            project.WixSourceGenerated += doc => doc.Select(placementPath)
                                                    .Add(XElement.Parse(xml));

            return project;
        }

        /// <summary>
        /// Adds the XML fragment to the element specified by <c>placementPath</c> and the element name with the
        /// attribute definition.
        /// <para>Note <c>placementPath</c> can only contain forward slashes.</para>
        /// <example>The following is an example of adding `Log` element to the `Wix/Bundle` element of the
        /// bootstrapper project.
        /// <code>
        /// bootstrapper.AddXmlElement("Wix/Bundle", "Log", "PathVariable=LogFileLocation");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="placementPath">The placement path.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="attributesDefinition">The attributes definition.</param>
        /// <returns></returns>
        static public WixProject AddXmlElement(this WixProject project, string placementPath, string elementName, string attributesDefinition)
        {
            project.WixSourceGenerated += doc => doc.Select(placementPath)
                                                    .AddElement(elementName, attributesDefinition);

            return project;
        }

        /// <summary>
        /// Sets the value of the attribute value in the .NET application configuration file according
        /// the specified XPath expression.
        /// <para>
        /// This simple routine is to be used for the customization of the installed config files
        /// (e.g. in the deferred custom actions).
        /// </para>
        /// </summary>
        /// <param name="configFile">The configuration file.</param>
        /// <param name="elementPath">The element XPath value. It should include the attribute name.</param>
        /// <param name="value">The value to be set to the attribute.</param>
        ///
        /// <example>The following is an example demonstrates this simple technique:
        /// <code>
        ///  Tasks.SetConfigAttribute(configFile, "//configuration/appSettings/add[@key='AppName']/@value", "My App");
        /// </code>
        /// </example>
        static public void SetConfigAttribute(string configFile, string elementPath, string value)
        {
            XDocument.Load(configFile)
                     .Root
                     .SetConfigAttribute(elementPath, value)
                     .Document
                     .Save(configFile);
        }

        /// <summary>
        /// Sets the value of the attribute value in the .NET application configuration file according
        /// the specified XPath expression.
        /// <para>
        /// This simple routine is to be used for the customization of the installed config files
        /// (e.g. in the deferred custom actions).
        /// </para>
        /// </summary>
        /// <returns></returns>
        /// <param name="config">The configuration file element.</param>
        /// <param name="elementPath">The element XPath value. It should include the attribute name.</param>
        /// <param name="value">The value to be set to the attribute.</param>
        ///
        /// <example>The following is an example demonstrates this simple technique:
        /// <code>
        ///  XDocument.Load(configFile).Root
        ///           .SetConfigAttribute("//configuration/appSettings/add[@key='AppName']/@value", "My App")
        ///           .SetConfigAttribute(...
        ///           .SetConfigAttribute(...
        ///           .Document.Save(configFile);
        /// </code>
        /// </example>
        static public XElement SetConfigAttribute(this XElement config, string elementPath, string value)
        {
            var valueAttr = ((IEnumerable)config.XPathEvaluate(elementPath)).Cast<XAttribute>().FirstOrDefault();

            if (valueAttr != null)
                valueAttr.Value = value;
            return config;
        }

        /// <summary>
        /// Creates the parent component for a given <see cref="WixSharp.WixEntity"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        static public XElement CreateParentComponent(this WixEntity entity)
        {
            return new XElement("Component").AddAttributes($@"Id={entity.Id}; Guid={WixGuid.NewGuid(entity.Id)}");
        }

        /// <summary>
        /// Maps the component to features. If no features specified then the component is added to the default ("Complete") feature.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="features">The features.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static XElement MapToFeatures(this XElement component, Feature[] features, ProcessingContext context)
        {
            WixObject.MapComponentToFeatures(component.Attr("Id"), features, context);
            return component;
        }

        /// <summary>
        /// Creates the and inserts (into `context`) parent component for a WixEntity object. This method is used
        /// in the default implementation of the <see cref="IGenericEntity.Process(ProcessingContext)"/>.
        /// <para>The best insertion point is located according the following algorithm </para>
        /// <list type="bullet">
        /// <item><description>The parent of the first descendant 'component' element with respect to <c>context.XParent</c>.</description></item>
        /// <item><description>The first descendant 'ProgramFiles64Folder' or 'ProgramFilesFolder' element with respect to <c>context.XParent</c>.</description></item>
        /// <item><description>The <c>context.XParent</c> element itself.</description></item>
        /// </list>
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        static public XElement CreateAndInsertParentComponent(this WixEntity entity, ProcessingContext context)
        {
            XElement component = entity.CreateParentComponent();

            XElement bestParent = context.XParent.FindFirstComponentParent() ??
                                  context.XParent.FirstProgramFilesDir() ??
                                  context.XParent;

            bestParent.Add(component);

            WixEntity.MapComponentToFeatures(component.Attr("Id"), entity.ActualFeatures, context);
            return component;
        }

        /// <summary>
        /// Finds the first descendant 'ProgramFiles' or 'ProgramFiles64Folder' directory element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        static public XElement FirstProgramFilesDir(this XElement element)
        {
            XElement dir = element.FindFirst("Directory");
            while (dir != null)
            {
                if (dir.HasAttribute("Name", x => x.StartsWith("ProgramFiles") && x.EndsWith("Folder")))
                    return dir;

                dir = dir.Elements("Directory").FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Installs the windows service. It uses InstallUtil.exe to complete the actual installation/uninstallation.
        /// During the run for the InstallUtil.exe console window is hidden.
        /// If any error occurred the console output is captured and embedded into the raised Exception object.
        /// </summary>
        /// <remarks>In order for username/password to be accepted, the ServiceProcessInstaller.Account in the target
        /// service must be set to ServiceAccount.User</remarks>
        /// <param name="serviceFile">The service file.</param>
        /// <param name="isInstalling">if set to <c>true</c> [is installing].</param>
        /// <param name="username">The required service username.</param>
        /// <param name="password">The required service password.</param>
        /// <exception cref="T:System.Exception"></exception>
        /// <returns>console output</returns>
        static public string InstallService(string serviceFile, bool isInstalling, string username, string password)
        {
            return InstallService(serviceFile, isInstalling,
                String.Format("/username={0} /password={1} /unattended", username, password));
        }

        /// <summary>
        /// Installs the windows service. It uses InstallUtil.exe to complete the actual installation/uninstallation.
        /// During the run for the InstallUtil.exe console window is hidden.
        /// If any error occurred the console output is captured and embedded into the raised Exception object.
        /// </summary>
        /// <param name="serviceFile">The service file.</param>
        /// <param name="isInstalling">if set to <c>true</c> [is installing].</param>
        /// <param name="args">The additional InstallUtil.exe arguments.</param>
        /// <exception cref="T:System.Exception"></exception>
        /// <returns>console output</returns>
        static public string InstallService(string serviceFile, bool isInstalling, string args = null)
        {
            var util = new ExternalTool
            {
                ExePath = IO.Path.Combine(CurrentFrameworkDirectory, "InstallUtil.exe"),
                Arguments = string.Format("{0} {1} \"{2}\"", isInstalling ? "" : "/u", args ?? "", serviceFile)
            };

            var buf = new StringBuilder();
            int retval = util.ConsoleRun(line => buf.AppendLine(line));
            string output = buf.ToString();

            string logoLastLine = "Microsoft Corporation.  All rights reserved.";
            int pos = output.IndexOf(logoLastLine);
            if (pos != -1)
                output = output.Substring(pos + logoLastLine.Length).Trim();

            if (retval != 0)
                throw new Exception(output);

            return output;
        }

        /// <summary>
        /// Starts the windows service. It uses sc.exe to complete the action.  During the action console window is hidden.
        /// If any error occurred the console output is captured and embedded into the raised Exception object.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <returns></returns>
        static public string StartService(string service, bool throwOnError = true)
        {
            return ServiceDo("start", service, throwOnError);
        }

        /// <summary>
        /// Stops the windows service. It uses sc.exe to complete the action.  During the action console window is hidden.
        /// If any error occurred the console output is captured and embedded into the raised Exception object.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <returns></returns>
        static public string StopService(string service, bool throwOnError = true)
        {
            return ServiceDo("stop", service, throwOnError);
        }

        static string ServiceDo(string action, string service, bool throwOnError)
        {
            var util = new ExternalTool { ExePath = "sc.exe", Arguments = action + " \"" + service + "\"" };

            var buf = new StringBuilder();
            int retval = util.ConsoleRun(line => buf.AppendLine(line));

            if (retval != 0 && throwOnError)
                throw new Exception(buf.ToString());
            return buf.ToString();
        }

        /// <summary>
        /// Gets the directory of .NET framework currently hosting the calling assembly.
        /// </summary>
        /// <value>
        /// The current framework directory.
        /// </value>
        public static string CurrentFrameworkDirectory
        {
            get
            {
                return RuntimeEnvironment.GetRuntimeDirectory();
            }
        }

        /// <summary>
        /// Determines whether the MSI package is implementing EmbeddedUI.
        /// </summary>
        /// <param name="msiPath">The path.</param>
        /// <returns></returns>
        public static bool IsEmbeddedUIPackage(string msiPath)
        {
            var msi = IO.Path.GetFullPath(msiPath);
            using (var database = new Database(msi, DatabaseOpenMode.ReadOnly))
            {
                return database.Tables["MsiEmbeddedUI"] != null;
            }
        }

        /// <summary>
        /// Gets the main window of the process, which satisfies the 'filter' condition.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static IWin32Window GetMainWindow(string processName, Func<Process, bool> filter)
        {
            var processes = Process.GetProcessesByName(processName);
            try
            {
                return new NativeWindow(processes.FirstOrDefault(filter)?.MainWindowHandle);
            }
            finally
            {
                processes.ForEach(x => x.Dispose());
            }
        }

        /// <summary>
        /// Simple implementation of <see cref="System.Windows.Forms.IWin32Window" />.
        /// </summary>
        /// <seealso cref="System.Windows.Forms.IWin32Window" />
        public class NativeWindow : IWin32Window
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NativeWindow"/> class.
            /// </summary>
            /// <param name="hwnd">The HWND.</param>
            public NativeWindow(IntPtr? hwnd) => Handle = (hwnd ?? IntPtr.Zero);

            /// <summary>
            /// Gets the handle to the window represented by the implementer.
            /// </summary>
            public IntPtr Handle { get; }
        }
    }

    /// <summary>
    /// A generic utility class for running console application tools (e.g. compilers, utilities)
    /// </summary>
    public class ExternalTool
    {
        /// <summary>
        /// The default console out handler. It can be used when you want to have fine control over
        /// STD output of the external tool.
        /// </summary>
        /// <example>The following is an example of masking the word 'secret' in the output text.
        /// <code>
        /// ExternalTool.ConsoleOut = (line) => Console.WriteLine(line.Replace("secret", "******"))
        /// var tool = new ExternalTool
        /// {
        ///     ExePath = "tool.exe",
        ///     Arguments = "-a -b",
        /// };
        /// tool.ConsoleRun();
        /// </code>
        /// </example>
        public Action<string> ConsoleOut = Compiler.OutputWriteLine;

        /// <summary>
        /// Controls echoing the executed command in the ConsoleOut.
        /// </summary>
        public bool EchoOn = true;

        /// <summary>
        /// Gets or sets the encoding to be used to process external executable output.
        /// By default it is the value of <see cref="ExternalTool.DefaultEncoding"/>.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public Encoding Encoding;

        /// <summary>
        /// Gets or sets the default encoding to be used to process external executable output.
        /// By default it is the value of <c>System.Text.Encoding.Default</c>.
        /// </summary>
        public static Encoding DefaultEncoding = Encoding.Default;

        /// <summary>
        /// Gets or sets the path to the exe file of the tool to be executed.
        /// </summary>
        /// <value>The exe path.</value>
        public string ExePath { set; get; }

        /// <summary>
        /// Gets or sets the arguments for the exe file of the tool to be executed.
        /// </summary>
        /// <value>The arguments.</value>
        public string Arguments { set; get; }

        /// <summary>
        /// Gets or sets the well known locations for probing the exe file.
        /// <para>
        /// By default probing is conducted in the locations defined in the system environment variable <c>PATH</c>.
        /// By setting <c>WellKnownLocations</c>
        /// you can add some extra probing locations. The directories must be separated by the ';' character.
        /// </para>
        /// </summary>
        /// <value>The well known locations.</value>
        public string WellKnownLocations { set; get; }

        /// <summary>
        /// Runs the exec file with the console output completely hidden and discarded.
        /// </summary>
        /// <returns>The process exit code.</returns>
        public int WinRun()
        {
            string systemPathOriginal = Environment.GetEnvironmentVariable("PATH");
            try
            {
                Environment.SetEnvironmentVariable("PATH", systemPathOriginal + ";" + Environment.ExpandEnvironmentVariables(this.WellKnownLocations ?? ""));

                var process = new Process();
                process.StartInfo.FileName = this.ExePath;
                process.StartInfo.Arguments = this.Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.StandardOutputEncoding = this.Encoding ?? DefaultEncoding;
                process.Start();

                process.WaitForExit();
                return process.ExitCode;
            }
            finally
            {
                Environment.SetEnvironmentVariable("PATH", systemPathOriginal);
            }
        }

        /// <summary>
        /// Runs the exec file with the console and redirects the output into the current process console output.
        /// </summary>
        /// <returns>The process exit code.</returns>
        public int ConsoleRun()
        {
            return ConsoleRun(this.ConsoleOut);
        }

        /// <summary>
        /// Runs the exec file with the console and returns the output text.
        /// </summary>
        /// <returns>The process console output.</returns>
        public string GetConsoleRunOutput()
        {
            var buf = new StringBuilder();
            this.ConsoleRun(line => buf.AppendLine(line));
            return buf.ToString();
        }

        /// <summary>
        /// Runs the exec file with the console and intercepts and redirects the output into the user specified delegate.
        /// </summary>
        /// <param name="onConsoleOut">The on console out.</param>
        /// <returns>The process exit code.</returns>
        public int ConsoleRun(Action<string> onConsoleOut)
        {
            string systemPathOriginal = Environment.GetEnvironmentVariable("PATH");
            try
            {
                Environment.SetEnvironmentVariable("PATH", Environment.ExpandEnvironmentVariables(this.WellKnownLocations ?? "") + ";" + "%WIXSHARP_PATH%;" + systemPathOriginal);

                string exePath = GetFullPath(this.ExePath);

                if (exePath == null)
                {
                    onConsoleOut("Error: Cannot find " + this.ExePath);
                    onConsoleOut("Make sure it is in the System PATH or WIXSHARP_PATH environment variables or WellKnownLocations member/parameter is initialized properly. ");
                    return 1;
                }

                if (EchoOn)
                    onConsoleOut("Execute:\n\"" + this.ExePath + "\" " + this.Arguments);

                var process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = this.Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardOutputEncoding = this.Encoding ?? DefaultEncoding;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                if (onConsoleOut != null)
                {
                    string line = null;
                    while (null != (line = process.StandardOutput.ReadLine()))
                    {
                        onConsoleOut(line);
                    }

                    string error = process.StandardError.ReadToEnd();
                    if (!error.IsEmpty())
                        onConsoleOut(error);
                }
                process.WaitForExit();
                return process.ExitCode;
            }
            finally
            {
                Environment.SetEnvironmentVariable("PATH", systemPathOriginal);
            }
        }

        string GetFullPath(string path)
        {
            if (IO.File.Exists(path))
                return IO.Path.GetFullPath(path);

            foreach (string dir in Environment.GetEnvironmentVariable("PATH").Split(';'))
            {
                if (IO.Directory.Exists(dir))
                {
                    string fullPath = IO.Path.Combine(Environment.ExpandEnvironmentVariables(dir).Trim(), path);
                    if (IO.File.Exists(fullPath))
                        return fullPath;
                }
            }

            return null;
        }
    }
}