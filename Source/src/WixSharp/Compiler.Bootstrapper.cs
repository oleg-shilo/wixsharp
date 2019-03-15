#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2015 Oleg Shilo

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

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using IO = System.IO;

namespace WixSharp
{
    //This code requires heavy optimization and refactoring. Toady it serves the purpose of refining the API.
    public partial class Compiler
    {
        /// <summary>
        /// Builds WiX Bootstrapper application from the specified <see cref="Bundle"/> project instance.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="path">The path.</param>
        /// <exception cref="System.ApplicationException">Wix compiler/linker cannot be found</exception>
        public static string Build(Bundle project, string path)
        {
            path = path.ExpandEnvVars();

            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            string oldCurrDir = Environment.CurrentDirectory;

            try
            {
                //System.Diagnostics.Debug.Assert(false);
                Compiler.TempFiles.Clear();
                string compiler = Utils.PathCombine(WixLocation, "candle.exe");
                string linker = Utils.PathCombine(WixLocation, "light.exe");

                if (!IO.File.Exists(compiler) || !IO.File.Exists(linker))
                {
                    Compiler.OutputWriteLine("Wix binaries cannot be found. Expected location is " + compiler.PathGetDirName());
                    throw new ApplicationException("Wix compiler/linker cannot be found");
                }
                else
                {
                    if (!project.SourceBaseDir.IsEmpty())
                        Environment.CurrentDirectory = project.SourceBaseDir;

                    string wxsFile = BuildWxs(project);

                    string objFile = IO.Path.ChangeExtension(wxsFile, ".wixobj");
                    string pdbFile = IO.Path.ChangeExtension(wxsFile, ".wixpdb");

                    string extensionDlls = "";
                    foreach (string dll in project.WixExtensions.Distinct())
                        extensionDlls += " -ext \"" + dll + "\"";

                    string wxsFiles = "";
                    foreach (string file in project.WxsFiles.Distinct())
                        wxsFiles += " \"" + file + "\"";

                    var candleOptions = CandleOptions + " " + project.CandleOptions;
                    string command = candleOptions + " " + extensionDlls + " \"" + wxsFile + "\" ";

                    string outDir = null;
                    if (wxsFiles.IsNotEmpty())
                    {
                        command += wxsFiles;
                        outDir = IO.Path.GetDirectoryName(wxsFile);
                        // if multiple files are specified candle expect a path for the -out switch
                        // or no path at all (use current directory)
                        // note the '\' character must be escaped twice: as a C# string and as a CMD char
                        if (outDir.IsNotEmpty())
                            command += $" -out \"{outDir}\\\\\"";
                    }
                    else
                        command += $" -out \"{objFile}\"";

                    command = command.ExpandEnvVars();

                    Run(compiler, command);

                    if (IO.File.Exists(objFile))
                    {
                        string outFile = wxsFile.PathChangeExtension(".exe");

                        if (path.IsNotEmpty())
                            outFile = IO.Path.GetFullPath(path);

                        if (IO.File.Exists(outFile))
                            IO.File.Delete(outFile);

                        string fragmentObjectFiles = project.WxsFiles
                                            .Distinct()
                                            .Join(" ", file => "\"" + outDir.PathCombine(IO.Path.GetFileNameWithoutExtension(file)) + ".wixobj\"");

                        string lightOptions = LightOptions + " " + project.LightOptions;

                        if (fragmentObjectFiles.IsNotEmpty())
                            lightOptions += " " + fragmentObjectFiles;

                        Run(linker, lightOptions + " \"" + objFile + "\" -out \"" + outFile + "\"" + extensionDlls + " -cultures:" + project.Language);

                        if (IO.File.Exists(outFile))
                        {
                            Compiler.TempFiles.Add(wxsFile);

                            Compiler.OutputWriteLine("\n----------------------------------------------------------\n");
                            Compiler.OutputWriteLine("Bootstrapper file has been built: " + path + "\n");
                            Compiler.OutputWriteLine(" Name       : " + project.Name);
                            Compiler.OutputWriteLine(" Version    : " + project.Version);
                            Compiler.OutputWriteLine(" UpgradeCode: {" + project.UpgradeCode + "}\n");

                            if (!PreserveDbgFiles && !project.PreserveDbgFiles)
                            {
                                objFile.DeleteIfExists();
                                pdbFile.DeleteIfExists();
                            }

                            project.DigitalSignature?.Apply(outFile);
                        }
                    }
                    else
                    {
                        Compiler.OutputWriteLine("Cannot build " + wxsFile);
                        Trace.WriteLine("Cannot build " + wxsFile);
                    }
                }

                if (!PreserveTempFiles && !project.PreserveTempFiles)
                    foreach (var file in Compiler.TempFiles)
                        try
                        {
                            if (IO.File.Exists(file))
                                IO.File.Delete(file);
                        }
                        catch { }
            }
            finally
            {
                Environment.CurrentDirectory = oldCurrDir;
            }

            return path;
        }

        /// <summary>
        /// Builds the WiX source file and generates batch file capable of building
        /// WiX/MSI bootstrapper with WiX toolset.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="path">The path to the batch file to be created.</param>
        /// <exception cref="System.ApplicationException">Wix compiler/linker cannot be found</exception>
        public static string BuildCmd(Bundle project, string path = null)
        {
            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            if (path == null)
                path = IO.Path.GetFullPath(IO.Path.Combine(project.OutDir, "Build_" + project.OutFileName) + ".cmd");

            path = path.ExpandEnvVars();

            //System.Diagnostics.Debug.Assert(false);
            string wixLocationEnvVar = $"set WixLocation={WixLocation}" + Environment.NewLine;
            string compiler = Utils.PathCombine(WixLocation, "candle.exe");
            string linker = Utils.PathCombine(WixLocation, "light.exe");
            string batchFile = path;

            if (!IO.File.Exists(compiler) || !IO.File.Exists(linker))
            {
                Compiler.OutputWriteLine("Wix binaries cannot be found. Expected location is " + IO.Path.GetDirectoryName(compiler));
                throw new ApplicationException("Wix compiler/linker cannot be found");
            }
            else
            {
                string wxsFile = BuildWxs(project);

                if (!project.SourceBaseDir.IsEmpty())
                    Environment.CurrentDirectory = project.SourceBaseDir;

                string objFile = IO.Path.ChangeExtension(wxsFile, ".wixobj");
                string pdbFile = IO.Path.ChangeExtension(wxsFile, ".wixpdb");

                string extensionDlls = "";
                foreach (string dll in project.WixExtensions.Distinct())
                    extensionDlls += " -ext \"" + dll + "\"";

                string wxsFiles = "";
                foreach (string file in project.WxsFiles.Distinct())
                    wxsFiles += " \"" + file + "\"";

                var candleOptions = CandleOptions + " " + project.CandleOptions;

                string batchFileContent = wixLocationEnvVar + "\"" + compiler + "\" " + candleOptions + " " + extensionDlls +
                                          " \"" + IO.Path.GetFileName(wxsFile) + "\" ";

                string outDir = null;
                if (wxsFiles.IsNotEmpty())
                {
                    batchFileContent += wxsFiles;
                    outDir = IO.Path.GetDirectoryName(wxsFile);
                    // if multiple files are specified candle expect a path for the -out switch
                    // or no path at all (use current directory)
                    // note the '\' character must be escaped twice: as a C# string and as a CMD char
                    if (outDir.IsNotEmpty())
                        batchFileContent += $" -out \"{outDir}\\\\\"";
                }
                else
                    batchFileContent += $" -out \"{objFile}\"";

                batchFileContent += "\r\n";

                string fragmentObjectFiles = project.WxsFiles
                                            .Distinct()
                                            .Join(" ", file => "\"" + outDir.PathCombine(IO.Path.GetFileNameWithoutExtension(file)) + ".wixobj\"");

                string lightOptions = LightOptions + " " + project.LightOptions;

                if (fragmentObjectFiles.IsNotEmpty())
                    lightOptions += " " + fragmentObjectFiles;

                if (path.IsNotEmpty())
                    lightOptions += " -out \"" + IO.Path.ChangeExtension(objFile, ".exe") + "\"";

                batchFileContent += "\"" + linker + "\" " + lightOptions + " \"" + objFile + "\" " + extensionDlls + " -cultures:" + project.Language + "\r\npause";

                batchFileContent = batchFileContent.ExpandEnvVars();

                using (var sw = new IO.StreamWriter(batchFile))
                    sw.Write(batchFileContent);
            }

            return path;
        }

        /// <summary>
        /// Builds the WiX source file (*.wxs) from the specified <see cref="Bundle"/> instance.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public static string BuildWxs(Bundle project)
        {
            lock (typeof(Compiler))
            {
                //very important to keep "ClientAssembly = " in all "public Build*" methods to ensure GetCallingAssembly
                //returns the build script assembly but not just another method of Compiler.
                if (ClientAssembly.IsEmpty())
                    ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

                project.Validate();

                lock (Compiler.AutoGeneration.WxsGenerationSynchObject)
                {
                    var oldAlgorithm = AutoGeneration.CustomIdAlgorithm;
                    try
                    {
                        project.ResetAutoIdGeneration(supressWarning: false);

                        AutoGeneration.CustomIdAlgorithm = project.CustomIdAlgorithm ?? AutoGeneration.CustomIdAlgorithm;

                        string file = IO.Path.GetFullPath(IO.Path.Combine(project.OutDir, project.OutFileName) + ".wxs");

                        if (IO.File.Exists(file))
                            IO.File.Delete(file);

                        string extraNamespaces = project.WixNamespaces.Distinct()
                                                                      .Select(x => x.StartsWith("xmlns:") ? x : "xmlns:" + x)
                                                                      .ConcatItems(" ");

                        var wix3Namespace = "http://schemas.microsoft.com/wix/2006/wi";
                        var wix4Namespace = "http://wixtoolset.org/schemas/v4/wxs";

                        var wixNamespace = Compiler.IsWix4 ? wix4Namespace : wix3Namespace;

                        var doc = XDocument.Parse(
                               @"<?xml version=""1.0"" encoding=""utf-8""?>
                             " + $"<Wix xmlns=\"{wixNamespace}\" {extraNamespaces} " + @" >
                        </Wix>");

                        doc.Root.Add(project.ToXml());

                        AutoElements.NormalizeFilePaths(doc, project.SourceBaseDir, EmitRelativePaths);

                        project.InvokeWixSourceGenerated(doc);

                        AutoElements.ExpandCustomAttributes(doc, project);

                        if (WixSourceGenerated != null)
                            WixSourceGenerated(doc);

                        string xml = "";
                        using (IO.StringWriter sw = new StringWriterWithEncoding(Encoding.Default))
                        {
                            doc.Save(sw, SaveOptions.None);
                            xml = sw.ToString();
                        }

                        //of course you can use XmlTextWriter.WriteRaw but this is just a temporary quick'n'dirty solution
                        //http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=2657663&SiteID=1
                        xml = xml.Replace("xmlns=\"\"", "");

                        DefaultWixSourceFormatedHandler(ref xml);

                        project.InvokeWixSourceFormated(ref xml);
                        if (WixSourceFormated != null)
                            WixSourceFormated(ref xml);

                        using (var sw = new IO.StreamWriter(file, false, Encoding.Default))
                            sw.WriteLine(xml);

                        Compiler.OutputWriteLine("\n----------------------------------------------------------\n");
                        Compiler.OutputWriteLine("Wix project file has been built: " + file + "\n");

                        project.InvokeWixSourceSaved(file);
                        if (WixSourceSaved != null)
                            WixSourceSaved(file);

                        return file;
                    }
                    finally
                    {
                        AutoGeneration.CustomIdAlgorithm = oldAlgorithm;
                        project.ResetAutoIdGeneration(supressWarning: true);
                    }
                }
            }
        }

        /// <summary>
        /// Builds WiX Bootstrapper application from the specified <see cref="Bundle"/> project instance.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public static string Build(Bundle project)
        {
            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            string outFile = IO.Path.GetFullPath(IO.Path.Combine(project.OutDir, project.OutFileName) + ".exe");

            Utils.EnsureFileDir(outFile);

            if (IO.File.Exists(outFile))
                IO.File.Delete(outFile);

            Build(project, outFile);

            return IO.File.Exists(outFile) ? outFile : null;
        }
    }
}