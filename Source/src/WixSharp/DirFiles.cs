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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Defines files of a given source directory to be installed on target system.
    /// Note that files in subdirectories are not included.
    /// <para>
    /// Use this class to define files to be automatically included into the deployment solution
    /// if their name matches specified wildcard character pattern (<see cref="DirFiles.IncludeMask"/>).
    /// </para>
    /// <para>
    /// This class is a logical equivalent of <see cref="Files"/> except that it analyses files in a single directory.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Note that all files matching the wildcard are resolved into absolute paths, so it may not always be suitable
    /// if the Wix# script is to be compiled into WiX XML source only (Compiler.<see cref="WixSharp.Compiler.BuildWxs(WixSharp.Project)"/>).
    /// This is not a problem if the Wix# script
    /// is compiled into MSI file (Compiler.<see cref="Compiler.BuildMsi(WixSharp.Project)"/>).
    /// </remarks>
    /// <example>The following is an example of defining installation files with a wildcard character pattern.
    /// <code>
    /// new Project("MyProduct",
    ///     new Dir(@"%ProgramFiles%\MyCompany\MyProduct",
    ///         new DirFiles(@"Release\Bin\*.*"),
    ///             new Dir("GlobalResources", new DirFiles(@"Release\Bin\GlobalResources\*.*")),
    ///             new Dir("Images", new DirFiles(@"Release\Bin\Images\*.*")),
    ///             ...
    /// </code>
    /// </example>
    public partial class DirFiles : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">The relative path to directory source directory. It must include wildcard pattern for files to be included
        /// into MSI (<c>new DirFiles(@"Release\Bin\*.*")</c>).</param>
        public DirFiles(string sourcePath)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">The relative path to directory source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new DirFiles(@"Release\Bin\*.*")</c>).</param>
        /// <param name="filter">Filter to be applied for every file to be evaluated for the inclusion into MSI.
        /// (e.g. <c>new Files(typical, @"Release\Bin\*.dll", f => !f.EndsWith(".Test.dll"))</c>).</param>
        public DirFiles(string sourcePath, Predicate<string> filter)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            Filter = filter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the directory files should be included in.</param>
        /// <param name="sourcePath">The relative path to directory source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new DirFiles(@"Release\Bin\*.*")</c>).</param>
        public DirFiles(Feature feature, string sourcePath)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            Feature = feature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the directory files should be included in.</param>
        /// <param name="sourcePath">The relative path to directory source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new DirFiles(@"Release\Bin\*.*")</c>).</param>
        /// <param name="filter">Filter to be applied for every file to be evaluated for the inclusion into MSI.
        /// (e.g. <c>new Files(typical, @"Release\Bin\*.dll", f => !f.EndsWith(".Test.dll"))</c>).</param>
        public DirFiles(Feature feature, string sourcePath, Predicate<string> filter)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            Filter = filter;
            Feature = feature;
        }

        /// <summary>
        /// The relative path from source directory to directory to lookup for files matching the <see cref="DirFiles.IncludeMask"/>.
        /// </summary>
        public string Directory = "";

        /// <summary>
        /// Wildcard pattern for files to be included into MSI.
        /// <para>Default value is <c>*.*</c>.</para>
        /// </summary>
        public string IncludeMask = "*.*";

        /// <summary>
        /// The filter delegate. It is applied for every file to be evaluated for the inclusion into MSI.
        /// </summary>
        public Predicate<string> Filter = (file => true);

        /// <summary>
        /// The delegate that is called when a file matching the wildcard of the sourcePath is rocessed
        /// and a <see cref="WixSharp.File"/> item is added to the project. It is the most convelient way of
        /// adjusting the <see cref="WixSharp.File"/> item properties.
        /// </summary>
        public Action<File> OnProcess = null;

        /// <summary>
        /// Analyses <paramref name="baseDirectory"/> and returns all files matching <see cref="DirFiles.IncludeMask"/>.
        /// </summary>
        /// <param name="baseDirectory">The base directory for file analysis. It is used in conjunction with
        /// relative <see cref="DirFiles.Directory"/>.Though <see cref="DirFiles.Directory"/> takes precedence if it is an absolute path.</param>
        /// <returns>Array of <see cref="File"/>s.</returns>
        public File[] GetFiles(string baseDirectory)
        {
            if (IO.Path.IsPathRooted(Directory))
                baseDirectory = Directory;
            if (baseDirectory.IsEmpty())
                baseDirectory = Environment.CurrentDirectory;

            baseDirectory = IO.Path.GetFullPath(baseDirectory);

            IO.Path.GetFullPath(baseDirectory);
            string rootDirPath;
            if (IO.Path.IsPathRooted(Directory))
                rootDirPath = Directory;
            else
                rootDirPath = Utils.PathCombine(baseDirectory, Directory);

            var files = new List<File>();
            var excludeWildcards = new List<Compiler.Wildcard>();

            foreach (string file in IO.Directory.GetFiles(rootDirPath, IncludeMask))
            {
                bool ignore = false;

                if (!ignore && Filter(file))
                {
                    var filePath = IO.Path.GetFullPath(file);

                    var item = new File(filePath)
                    {
                        Feature = this.Feature,
                        Features = this.Features,
                        AttributesDefinition = this.AttributesDefinition,
                        Attributes = this.Attributes.Clone()
                    };

                    OnProcess?.Invoke(item);

                    files.Add(item);
                }
            }
            return files.ToArray();
        }
    }
}