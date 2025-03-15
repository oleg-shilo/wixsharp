using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// Class representing Visual Studio project, which build output is to be imported into the WiX
    /// setup definition (WXS file) with WiX Heat utility.
    /// </summary>
    public class VsProject
    {
        /// <summary>
        /// Gets the XML element name to be used in WXS code.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the project path.
        /// </summary>
        /// <value>
        /// The project path.
        /// </value>
        public string ProjectPath { get; }

        /// <summary>
        /// Gets the path to the build output of the project.
        /// </summary>
        /// <value>
        /// The build path.
        /// </value>
        public string BuildPath => ProjectPath.PathGetDirName().PathJoin(BuildDir);

        /// <summary>
        /// The build output sub-directory of the VS project. By default it is  <c>"bin\Release"</c>
        /// </summary>
        public string BuildDir;

        /// <summary>
        /// The target directory id from the WXS definition.
        /// </summary>
        public string TargetDir;

        /// <summary>
        /// Include primary output of the project, e.g. the assembly exe or dll.
        /// </summary>
        public bool Binaries = true;

        /// <summary>
        /// Include debug symbol files, e.g. pdb.
        /// </summary>
        public bool Symbols = false;

        /// <summary>
        /// Include documentation files.
        /// </summary>
        public bool Documents = false;

        /// <summary>
        /// Include content files.
        /// </summary>
        public bool Content = true;

        /// <summary>
        /// Include the localized resource assemblies.
        /// </summary>
        public bool Satellites = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="VsProject"/> class.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="buildDir">The build directory.</param>
        public VsProject(string projectPath, string buildDir = @"bin\Release")
        {
            ProjectPath = projectPath;
            BuildDir = buildDir;
            Name = Path.GetFileNameWithoutExtension(projectPath).Normalize();
        }
    }

    /// <summary>
    /// Extension methods for more convenient use of <see cref="WixSharp.Harvester"/> class.
    /// </summary>
    /// <example>The following is an example of adding TestApp1 and TestApp2 VS projects build output to the
    /// installation directory.
    /// <code>
    /// var project =
    ///     new ManagedProject("HeatAggregatedMsi",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File("readme.txt")));
    ///
    /// project.AddVsProjectOutput(
    ///     @"TestApps\TestApp1\TestApp1.csproj",
    ///     @"TestApps\TestApp2\TestApp2.csproj");
    /// </code>
    /// </example>
    public static class HarvesterExtensions
    {
        /// <summary>
        /// Adds the Visual Studio project output.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="vsProjects">The vs projects.</param>
        /// <returns></returns>
        public static Project AddVsProjectOutput(this Project project, params string[] vsProjects)
        {
            var harvester = new Harvester(project);
            harvester.AddProjects(vsProjects);
            return project;
        }

        /// <summary>
        /// Adds the Visual Studio project output.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="vsProjects">The vs projects.</param>
        /// <returns></returns>
        public static Project AddVsProjectOutput(this Project project, params VsProject[] vsProjects)
        {
            var harvester = new Harvester(project);
            harvester.AddProjects(vsProjects);
            return project;
        }
    }

    /// <summary>
    /// This class uses WiX Heat utility to aggregate the Visual Studio project build output and inject it in the
    /// WixSharp project. This class serves the same objective as <see cref="WixSharp.Files"/> except it passes the
    /// full control over what and how to include the aggregated deployment assets to the WiX Heat utility.
    /// <para>You may find using <see cref="WixSharp.Files"/> being more beneficial if you need very precise control
    /// over the aggregation. But otherwise this very simple and easy to use class can be an excellent alternative.
    /// </para>
    /// <para>If you want to minimize dependencies (e.g. heat.exe) you can use <see cref="WixSharp.Files.FromBuildDir(string, string)"/>
    /// to achieve the same effect:</para>
    /// <code>
    /// new Dir(@"%ProgramFiles%\My Company\My Product",
    ///     Files.FromBuildDir(@"TestApps\TestApp2\bin\Release")
    ///     </code>
    /// </summary>
    /// <example>The following is an example of adding TestApp1 and TestApp2 VS projects build output to the
    /// installation directory.
    /// <code>
    /// var project =
    ///     new ManagedProject("HeatAggregatedMsi",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File("readme.txt")));
    ///
    /// var harvester = new Harvester(project);
    /// harvester.AddProjects(
    ///     @"TestApps\TestApp1\TestApp1.csproj",
    ///     @"TestApps\TestApp2\TestApp2.csproj");
    /// </code>
    /// </example>
    public class Harvester
    {
        static string heat = WixTools.Heat;
        Project project;
        string targetDir;
        IList<VsProject> components = new List<VsProject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Harvester"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="targetDir">The target directory.</param>
        public Harvester(Project project, string targetDir = null)
        {
            this.project = project;
            this.targetDir = targetDir ?? Compiler.AutoGeneration.InstallDirDefaultId;
            this.project.WixSourceGenerated += WixSourceGenerated;
        }

        void WixSourceGenerated(XDocument document)
        {
            var defaultFeature = document.FindAll("Feature")
                                         .FirstOrDefault(x => x.HasAttribute("Id", project.DefaultFeature.Id));

            foreach (var project in components)
            {
                if (project.Binaries)
                    defaultFeature.AddElement("ComponentGroupRef", $"Id={project.Name}.Binaries");

                if (project.Content)
                    defaultFeature.AddElement("ComponentGroupRef", $"Id={project.Name}.Content");

                if (project.Satellites)
                    defaultFeature.AddElement("ComponentGroupRef", $"Id={project.Name}.Satellites");
            }
        }

        /// <summary>
        /// Adds the references to the Visual Studio projects.
        /// </summary>
        /// <param name="projects">Multiple project paths.</param>
        /// <returns></returns>
        public void AddProjects(params VsProject[] projects)
            => projects.ForEach(x => AddProject(x));

        /// <summary>
        /// Adds the references to the Visual Studio projects.
        /// </summary>
        /// <param name="projects">Multiple project paths.</param>
        /// <returns></returns>
        public void AddProjects(params string[] projects)
            => projects.ForEach(x => AddProject(x));

        /// <summary>
        /// Adds the reference to the Visual Studio project.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <returns></returns>
        public Harvester AddProject(string projectPath)
        {
            AddProject(new VsProject(projectPath));
            return this;
        }

        /// <summary>
        /// Adds the reference to the Visual Studio project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// Project '{project.ProjectPath}' not found.
        /// or
        /// 'Heat' failure...
        /// </exception>
        public Harvester AddProject(VsProject project)
        {
            if (!System.IO.File.Exists(project.ProjectPath))
                throw new Exception($"Project '{project.ProjectPath}' not found.");

            var sourceDir = project.BuildPath;
            var projectDir = project.ProjectPath.PathGetDirName();
            var targetDir = project.TargetDir ?? this.targetDir;

            var output = this.project.OutDir.PathJoin($"{project.Name}.wxs");

            var tool = new ExternalTool
            {
                ExePath = heat,
                Arguments = new[]
                            {
                                $"project \"{project.ProjectPath}\"",
                                project.Binaries ? "-pog Binaries" : "",
                                project.Symbols ? "-pog Symbols" : "",
                                project.Documents  ? "-pog Documents" : "",
                                project.Satellites ? "-pog Satellites" : "",
                                project.Content ? "-pog Content" : "",
                                "-ag","-sfrag",
                                $"-directoryid \"{targetDir}\"",
                                "-template fragment",
                                $"-platform AnyCPU",
                                $"-projectname \"{project.Name}\"",
                                $"-out \"{output}\""
                            }.JoinBy(" ")
            };

            // Note, all StdOut and StdErr will be printed by the `tool` anyway
            if (tool.ConsoleRun() != 0)
                throw new Exception("'Heat' failure...");

            var xml = XDocument.Load(output);
            foreach (var fragment in xml.Root.Elements())
                this.project.AddXml("Wix", fragment.ToString());

            // As intended the TargetDir for candle is the sourceDir
            this.project.WixOptions +=
                $" -d\"{project.Name}.TargetDir\"=\"{sourceDir}\" " +
                $" -d\"{project.Name}.ProjectDir\"=\"{projectDir}\" ";
            components.Add(project);

            if (!Compiler.PreserveTempFiles)
                System.IO.File.Delete(output);

            return this;
        }
    }
}