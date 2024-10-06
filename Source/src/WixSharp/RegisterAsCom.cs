using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// Registers a file in the registry (e.g. *.dll, *.ocx).<br/>
    /// <br/>
    /// It utilizes heat.exe to extract the registry entries from the file and adds them
    /// to the build (*.wxs) file.<br/>
    /// Files that do not have any registration entries stay as they are.<br/>
    /// </summary>
    ///
    /// <example>
    /// The following are examples on how to register files.<br/>
    /// <br/>
    /// Creates the corresponding registry entries for a given file:<br/>
    /// <code>
    /// new File(@"Some\File\Location",
    ///     new RegisterAsCom()
    /// );
    /// </code>
    /// <hr/>
    /// Creates the corresponding registry entries for a given file as COM objects that are easier to read.<br/>
    /// Can lead to Candle errors because heat.exe sometimes leaves stuff empty.<br/>
    /// <code>
    /// new File(@"Some\File\Location",
    ///     new RegisterAsCom()
    ///     {
    ///         CreateComObjects = true
    ///     }
    /// );
    /// </code>
    /// <hr/>
    /// Runs heat.exe with the additional argument '-gg' and omits the default arguments '-ag' and '-svb6'.<br/>
    /// <code>
    /// new File(@"Some\File\Location",
    ///     new RegisterAsCom()
    ///     {
    ///         HeatArguments = new[] { "-gg" },
    ///         OverrideDefaults = true
    ///     }
    /// );
    /// </code>
    /// </example>
    public class RegisterAsCom : WixEntity, IGenericEntity
    {
        // Get the path to heat.exe
        // private static readonly string _heat = Path.Combine(Compiler.WixLocation, "heat.exe");
        static string heat = WixTools.Heat;

        /// <summary>
        /// Whether to create the output as COM objects or plain registry entries (adds argument '-scom' if false).<br/>
        /// COM objects are easier to read but can lead to Candle errors because heat.exe sometimes leaves stuff empty.
        /// </summary>
        public bool CreateComObjects { get; set; }

        /// <summary>
        /// Whether to ignore the warnings returned by heat.exe and to stop forwarding them to the console.
        /// </summary>
        public bool HideWarnings { get; set; }

        /// <summary>
        /// Omits the default arguments '-ag' and '-svb6' from the call to heat.exe if further customization is wanted.
        /// </summary>
        public bool OverrideDefaults { get; set; }

        /// <summary>
        /// Additional arguments that should be passed to heat.exe.
        /// </summary>
        public string[] HeatArguments { get; set; }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project" />. See 'Wix#/samples/Extensions'
        /// sample for the details on how
        /// to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.Exception"></exception>
        public void Process(ProcessingContext context)
        {
            // Get the project from context
            var project = context.Project;

            // Get the XElements for File, Component and Directory
            var thisFile = context.XParent;
            var thisComponent = context.XParentComponent;
            var thisDirectory = thisComponent.Parent;

            // Get the corresponding IDs
            var thisFileId = thisFile.GetAttribute("Id");
            var thisComponentId = thisComponent.GetAttribute("Id");
            var thisDirectoryId = thisDirectory.GetAttribute("Id");

            // Get the source directory to store the temp files
            var sourceDir = Path.Combine(Environment.CurrentDirectory, project.SourceBaseDir);

            // Get the source file and define the temp output file
            var sourceFile = Path.Combine(sourceDir, thisFile.GetAttribute("Source"));
            var outputFile = Path.Combine(project.OutDir, (Path.GetFileName(sourceFile) + ".wxs"));

            // Add the default arguments to HeatArguments if OverrideDefaults is not set
            if (!OverrideDefaults)
            {
                HeatArguments = HeatArguments.Combine(
                    "-ag",
                    "-svb6"
                                                     );
            }

            // Do not create COM objects if CreateComObjects is not set
            if (!CreateComObjects)
            {
                HeatArguments = HeatArguments.Combine("-scom");
            }

            // Combine the arguments for heat.exe
            var heatArgs = new[] { $"file \"{sourceFile}\"" };

            heatArgs = heatArgs.Combine(HeatArguments);
            heatArgs = heatArgs.Combine($"-out \"{outputFile}\"");

            // Define the start info for heat.exe
            var pi = new ProcessStartInfo
            {
                FileName = heat,
                Arguments = string.Join(" ", heatArgs),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Extract the registry entries from sourceFile and save it as outputFile
            using (var p = System.Diagnostics.Process.Start(pi))
            {
                p.WaitForExit();

                // Throw an exception if heat.exe encountered an error
                if (p.ExitCode != 0)
                    throw new Exception(p.StandardOutput.ReadToEnd());

                // Get the output from heat.exe
                var output = p.StandardOutput.ReadToEnd().Split('\n');

                // If heat.exe sent more than the header and HideWarnings is not set,
                // write it to the console to log any warning
                if (output.Length > 4 && !HideWarnings)
                {
                    foreach (var line in output)
                    {
                        Console.WriteLine(line);
                    }
                }
            }

            // Load outputFile as XDocument
            var xml = XDocument.Load(outputFile);

            // Get the XElements for File, Component and Directory from the output
            var component = xml.FindFirst("Component");
            var directory = component.Parent;
            var file = component.Select("File");

            // Get the corresponding IDs
            var componentId = component.GetAttribute("Id");
            var directoryId = directory.GetAttribute("Id");
            var fileId = file.GetAttribute("Id");

            // Get the whole content
            var content = directory.ToString();

            // Replace the IDs generated by heat.exe with the correct ones from this File
            content = content.Replace(fileId, thisFileId);
            content = content.Replace(componentId, thisComponentId);
            content = content.Replace(directoryId, thisDirectoryId);

            // Transform the content back to an XElement
            directory = XElement.Parse(content);

            // Get the new File and Component
            component = directory.Select("Component");
            file = component.Select("File");

            // Remove File from the Component
            file.Remove();

            // Add the registry entries from File to this File
            if (file.Nodes().Count() > 0)
            {
                thisFile.Add(file.Nodes());
            }

            // Add the registry entries from Component to this Component
            if (component.Nodes().Count() > 0)
            {
                thisComponent.Add(component.Nodes());
            }

            // Remove the temp file if not specified otherwise
            if (!Compiler.PreserveTempFiles && !project.PreserveTempFiles)
                System.IO.File.Delete(outputFile);
        }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Registers a file in the registry (e.g. *.dll, *.ocx).<br/>
        /// <br/>
        /// It utilizes heat.exe to extract the registry entries from the file and adds them
        /// to the build (*.wxs) file.<br/>
        /// Files that do not have any registration entries stay as they are.<br/>
        /// </summary>
        /// <param name="file">The file to register.</param>
        /// <param name="createComObjects">
        /// Whether to create the output as COM objects or plain registry entries (adds argument '-scom' if false).<br/>
        /// COM objects are easier to read but can lead to Candle errors because heat.exe sometimes leaves stuff empty.
        /// </param>
        /// <param name="heatArguments">Additional arguments that should be passed to heat.exe.</param>
        /// <param name="overrideDefaults">Omits the default arguments '-ag' and '-svb6' from the call to heat.exe if further customization is wanted.</param>
        /// <param name="hideWarnings">Whether to ignore the warnings returned by heat.exe and to stop forwarding them to the console.</param>
        /// <example>
        ///
        /// The following are examples on how to register files:<br/>
        /// <br/>
        ///Creates the corresponding registry entries for a given file.<br/>
        /// <code>
        /// new File(@"Some\File\Location")
        ///         .RegisterAsCom();
        /// </code>
        /// <hr/>
        /// Creates the corresponding registry entries for a given file as COM objects that are easier to read.<br/>
        /// Can lead to Candle errors because heat.exe sometimes leaves stuff empty.<br/>
        /// <code>
        /// new File(@"Some\File\Location")
        ///         .RegisterAsCom(true);
        /// </code>
        /// <hr/>
        /// Runs heat.exe with the additional argument '-gg' and omits the default arguments '-ag' and '-svb6'.<br/>
        /// <code>
        /// new File(@"Some\File\Location")
        ///         .RegisterAsCom(heatArguments: new[] { "-gg" }, overrideDefaults: true);
        /// </code>
        /// This comes down to the same as:<br/>
        /// <code>
        /// new File(@"Some\File\Location")
        ///         .RegisterAsCom(false, new[] { "-gg" }, true);
        /// </code>
        /// Or:<br/>
        /// <code>
        /// new File(@"Some\File\Location",
        ///     new RegisterAsCom()
        ///     {
        ///         HeatArguments = new[] { "-gg" },
        ///         OverrideDefaults = true
        ///     }
        /// );
        /// </code>
        /// </example>
        public static File RegisterAsCom(this File file, bool createComObjects = false, string[] heatArguments = null, bool overrideDefaults = false, bool hideWarnings = false)
        {
            file.Add(new RegisterAsCom()
            {
                CreateComObjects = createComObjects,
                HeatArguments = heatArguments,
                OverrideDefaults = overrideDefaults,
                HideWarnings = hideWarnings
            });

            return file;
        }

        /// <summary>
        /// Registers multiple files in the registry (e.g. *.dll, *.ocx).<br/>
        /// <br/>
        /// It utilizes heat.exe to extract the registry entries from the files and adds them
        /// to the build (*.wxs) file.<br/>
        /// Files that do not have any registration entries stay as they are.<br/>
        /// </summary>
        ///
        /// <param name="files">The files to register.</param>
        /// <param name="createComObjects">
        /// Whether to create the output as COM objects or plain registry entries (adds argument '-scom' if false).<br/>
        /// COM objects are easier to read but can lead to Candle errors because heat.exe sometimes leaves stuff empty.
        /// </param>
        /// <param name="heatArguments">Additional arguments that should be passed to heat.exe.</param>
        /// <param name="overrideDefaults">Omits the default arguments '-ag' and '-svb6' from the call to heat.exe if further customization is wanted.</param>
        /// <param name="hideWarnings">Whether to ignore the warnings returned by heat.exe and to stop forwarding them to the console.</param>
        /// <example>
        /// The following are examples on how to register files:<br/>
        /// <br/>
        /// Creates the corresponding registry entries for given files.<br/>
        /// <code>
        /// new Files(@"Some\File\Location\*.*")
        ///         .RegisterAsCom();
        /// </code>
        /// <hr/>
        /// Creates the corresponding registry entries for given files as COM objects that are easier to read.<br/>
        /// Can lead to Candle errors because heat.exe sometimes leaves stuff empty.<br/>
        /// <code>
        /// new Files(@"Some\File\Location\*.*")
        ///         .RegisterAsCom(true);
        /// </code>
        /// <hr/>
        /// Runs heat.exe with the additional argument '-gg' and omits the default arguments '-ag' and '-svb6'.<br/>
        /// <code>
        /// new Files(@"Some\File\Location\*.*")
        ///         .RegisterAsCom(heatArguments: new[] { "-gg" }, overrideDefaults: true);
        /// </code>
        /// This comes down to the same as:<br/>
        /// <code>
        /// new Files(@"Some\File\Location\*.*")
        ///         .RegisterAsCom(false, new[] { "-gg" }, true);
        /// </code>
        /// Or:<br/>
        /// <code>
        /// new Files(@"Some\File\Location\*.*")
        /// {
        ///     OnProcess = file =>
        ///     {
        ///         file.RegisterAsCom(false, new[] { "-gg" }, true);
        ///     }
        /// };
        /// </code>
        /// </example>
        ///
        public static Files RegisterAsCom(this Files files, bool createComObjects = false, string[] heatArguments = null, bool overrideDefaults = false, bool hideWarnings = false)
        {
            files.OnProcess = file =>
            {
                file.RegisterAsCom(createComObjects, heatArguments, overrideDefaults, hideWarnings);
            };

            return files;
        }

        /// <summary>
        /// Registers multiple files in the registry (e.g. *.dll, *.ocx).<br/>
        /// <br/>
        /// It utilizes heat.exe to extract the registry entries from the files and adds them
        /// to the build (*.wxs) file.<br/>
        /// Files that do not have any registration entries stay as they are.<br/>
        /// </summary>
        ///
        /// <param name="dirFiles">The files to register.</param>
        /// <param name="createComObjects">
        /// Whether to create the output as COM objects or plain registry entries (adds argument '-scom' if false).<br/>
        /// COM objects are easier to read but can lead to Candle errors because heat.exe sometimes leaves stuff empty.
        /// </param>
        /// <param name="heatArguments">Additional arguments that should be passed to heat.exe.</param>
        /// <param name="overrideDefaults">Omits the default arguments '-ag' and '-svb6' from the call to heat.exe if further customization is wanted.</param>
        /// <param name="hideWarnings">Whether to ignore the warnings returned by heat.exe and to stop forwarding them to the console.</param>
        ///
        /// The following are examples on how to register files:<br/>
        /// <br/>
        /// <example>
        /// Creates the corresponding registry entries for given files.<br/>
        /// <code>
        /// new DirFiles(@"Some\File\Location\*.*")
        ///         .RegisterAsCom();
        /// </code>
        /// <hr/>
        /// Creates the corresponding registry entries for given files as COM objects that are easier to read.<br/>
        /// Can lead to Candle errors because heat.exe sometimes leaves stuff empty.<br/>
        /// <code>
        /// new DirFiles(@"Some\File\Location\*.*")
        ///         .RegisterAsCom(true);
        /// </code>
        /// <hr/>
        /// Runs heat.exe with the additional argument '-gg' and omits the default arguments '-ag' and '-svb6'.<br/>
        /// <code>
        /// new DirFiles(@"Some\File\Location\*.*")
        ///         .RegisterAsCom(heatArguments: new[] { "-gg" }, overrideDefaults: true);
        /// </code>
        /// This comes down to the same as:<br/>
        /// <code>
        /// new DirFiles(@"Some\File\Location\*.*")
        ///         .RegisterAsCom(false, new[] { "-gg" }, true);
        /// </code>
        /// Or:<br/>
        /// <code>
        /// new DirFiles(@"Some\File\Location\*.*")
        /// {
        ///     OnProcess = file =>
        ///     {
        ///         file.RegisterAsCom(false, new[] { "-gg" }, true);
        ///     }
        /// };
        /// </code>
        /// </example>
        public static DirFiles RegisterAsCom(this DirFiles dirFiles, bool createComObjects = false, string[] heatArguments = null, bool overrideDefaults = false, bool hideWarnings = false)
        {
            dirFiles.OnProcess = file =>
            {
                file.RegisterAsCom(createComObjects, heatArguments, overrideDefaults, hideWarnings);
            };

            return dirFiles;
        }
    }
}