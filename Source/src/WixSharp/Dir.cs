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
using System.Xml.Linq;
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Defines directory to be installed on target system.
    /// <para>
    /// Use this class to define file/directory structure of the deployment solution.
    /// </para>
    ///  You can use predefined Wix# environment constants for well-known installation locations. They are directly mapped
    ///  to the corresponding WiX constants:
    ///  <para>For the full list of the constants consult WiX documentation or use <c>Compiler.GetMappedWixConstants</c>
    ///  to explore them programmatically./</para>
    ///  <para>
    ///  <para><c>Wix#</c> - <c>WiX</c></para>
    ///  <para>%WindowsFolder% - [WindowsFolder]</para>
    ///  <para>%ProgramFiles% - [ProgramFilesFolder]</para>
    ///  <para>%ProgramMenu% - [ProgramMenuFolder]</para>
    ///  <para>%CommonAppDataFolder% - [CommonAppDataFolder]</para>
    ///  <para>%AppDataFolder% - [AppDataFolder]</para>
    ///  <para>%CommonFilesFolder% - [CommonFilesFolder]</para>
    ///  <para>%LocalAppDataFolder% - [LocalAppDataFolder]</para>
    ///  <para>%ProgramFiles64Folder% - [ProgramFiles64Folder]</para>
    ///  <para>%System64Folder% - [System64Folder]</para>
    ///  <para>%SystemFolder% - [SystemFolder]</para>
    ///  <para>%TempFolder% - [TempFolder]</para>
    ///  <para>%Desktop% - [DesktopFolder]</para>
    ///  <para>...</para>
    ///  </para>
    /// </summary>
    /// <example>The following is an example of defining installation directory <c>Progam Files/My Company/My Product</c>
    /// containing a single file <c>MyApp.exe</c> and subdirectory <c>Documentation</c> with <c>UserManual.pdf</c> file.
    /// <code>
    /// var project = new Project("MyProduct",
    ///
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File(@"Release\MyApp.exe"),
    ///             new Dir("Documentation",
    ///                 new File(@"Release\UserManual.pdf")),
    ///             ...
    /// </code>
    /// </example>
    public class Dir : WixEntity
    {
        /// <summary>
        /// The last child <see cref="Dir"/> that was created by <see cref="Dir.ProcessTargetPath(string, Feature)"/>.
        /// </summary>
        protected Dir lastDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dir"/> class.
        /// </summary>
        public Dir()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dir"/> class with properties/fields initialized with specified parameters
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Dir"/> instance.</param>
        /// <param name="targetPath">The name of the directory. Note if the directory is a root installation directory <c>targetPath</c> must
        /// be specified as a full path. However if the directory is a nested installation directory the name must be a directory name only.</param>
        /// <param name="items">Any <see cref="WixEntity"/> which can be contained by directory (e.g. file, subdirectory).</param>
        public Dir(Id id, string targetPath, params WixEntity[] items)
        {
            lastDir = ProcessTargetPath(targetPath, null);
            lastDir.AddItems(items);
            lastDir.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dir"/> class with properties/fields initialized with specified parameters
        /// </summary>
        /// <param name="targetPath">The name of the directory. Note if the directory is a root installation directory <c>targetPath</c> must
        /// be specified as a full path. However if the directory is a nested installation directory the name must be a directory name only.</param>
        /// <param name="items">Any <see cref="WixEntity"/> which can be contained by directory (e.g. file, subdirectory).</param>
        public Dir(string targetPath, params WixEntity[] items)
        {
            lastDir = ProcessTargetPath(targetPath, null);
            lastDir.AddItems(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dir"/> class with properties/fields initialized with specified parameters
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the directory should be included in.</param>
        /// <param name="targetPath">The name of the directory. Note if the directory is a root installation directory <c>targetPath</c> must
        /// be specified as a full path. However if the directory is a nested installation directory the name must be a directory name only.</param>
        /// <param name="items">Any <see cref="WixEntity"/> which can be contained by directory (e.g. file, subdirectory).</param>
        public Dir(Feature feature, string targetPath, params WixEntity[] items)
        {
            this.Feature = feature;

            lastDir = ProcessTargetPath(targetPath, feature);
            lastDir.AddItems(items);
            lastDir.Feature = feature;
        }

        internal Dir(Feature feature, string targetPath, Project project)
        {
            this.Feature = feature;

            //create nested Dirs on-fly but reuse already existing ones in the project
            var nestedDirs = targetPath.Split("\\/".ToCharArray());

            Dir lastFound = null;
            string lastMatching = null;
            string[] flatTree = ToFlatPathTree(targetPath);

            foreach (string path in flatTree)
            {
                var existingDir = project.FindDir(path);
                if (existingDir != null)
                {
                    lastFound = existingDir;
                    lastMatching = path;
                }
                else
                {
                    if (lastFound != null)
                    {
                        Dir currDir = lastFound;

                        string[] newSubDirs = targetPath.Substring(lastMatching.Length + 1).Split("\\/".ToCharArray());
                        for (int i = 0; i < newSubDirs.Length; i++)
                        {
                            Dir nextSubDir = new Dir(feature, newSubDirs[i]);
                            currDir.Dirs = new Dir[] { nextSubDir };
                            currDir = nextSubDir;
                        }

                        currDir.Feature = feature;
                    }
                    else
                    {
                        lastDir = ProcessTargetPath(targetPath, feature);
                        lastDir.Feature = feature;
                    }
                    break;
                }
            }
        }

        internal bool HasItemsToInstall()
        {
            return Files.Any() || FileCollections.Any() || Shortcuts.Any();
        }

        static internal string[] ToFlatPathTree(string path)
        {
            List<string> retval = new List<string>();

            foreach (var dir in path.Split("\\/".ToCharArray()))
            {
                string lastItem = retval.LastOrDefault();
                if (lastItem == null)
                    retval.Add(dir);
                else
                    retval.Add(lastItem + "\\" + dir);
            }

            return retval.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dir"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Dir"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the directory should be included in.</param>
        /// <param name="targetPath">The name of the directory. Note if the directory is a root installation directory <c>targetPath</c> must
        /// be specified as a full path. However if the directory is a nested installation directory the name must be a directory name only.</param>
        /// <param name="items">Any <see cref="WixEntity"/> which can be contained by directory (e.g. file, subdirectory).</param>
        public Dir(Id id, Feature feature, string targetPath, params WixEntity[] items)
        {
            this.Feature = feature;
            lastDir = ProcessTargetPath(targetPath, feature);
            lastDir.AddItems(items);
            lastDir.Id = id;
            lastDir.Feature = feature;
        }

        /// <summary>
        /// Collection of the contained nested <see cref="Dir"/>s (subdirectories).
        /// </summary>
        public Dir[] Dirs = new Dir[0];

        internal Dir AutoParent;

        internal Dir GetRootAutoParent()
        {
            Dir result = this.AutoParent;
            while (result != null)
            {
                if (result.AutoParent == null)
                    break;
                else
                    result = result.AutoParent;
            }
            return result;
        }

        internal bool IsAutoParent()
        {
            return Dirs.Any(x => x.AutoParent == this);
        }

        /// <summary>
        /// Collection of the contained <see cref="File"/>s.
        /// </summary>
        public File[] Files = new File[0];

        /// <summary>
        /// Collection of the <see cref="DirFiles"/> objects. <see cref="DirFiles"/> type is used to specify files
        /// contained by a specific directory with wildcard character pattern.
        /// Files in subdirectories are not included.
        /// <para>
        /// <see cref="DirFiles"/> type is related to but not identical to <see cref="Files"/>, which defines files of
        /// not only a single level directory but all subdirectories as well.
        /// </para>
        /// </summary>
        public DirFiles[] DirFileCollections = new DirFiles[0];

        /// <summary>
        /// Collection of the <see cref="Files"/> objects. <see cref="Files"/> type is used to specify files
        /// contained by a specific directory and all subdirectories with wildcard character pattern.
        /// <para>
        /// <see cref="Files"/> type is related to but not identical to <see cref="DirFiles"/>, which defines only files
        /// of a single level directory.
        /// </para>
        /// </summary>
        public Files[] FileCollections = new Files[0];

        /// <summary>
        /// Collection of WiX/MSI <see cref="ODBCDataSource"/> objects to be created during the installed.
        /// </summary>
        public ODBCDataSource[] ODBCDataSources = new ODBCDataSource[0];

        /// <summary>
        /// Collection of WiX/MSI <see cref="IISVirtualDir"/> objects to be created during the installed.
        /// </summary>
        public IISVirtualDir[] IISVirtualDirs = new IISVirtualDir[0];

        /// <summary>
        /// Collection of the user defined <see cref="IGenericEntity"/> items.
        /// </summary>
        public IGenericEntity[] GenericItems = new IGenericEntity[0];

        /// <summary>
        /// Collection of the contained <see cref="Merge"/> modules.
        /// </summary>
        public Merge[] MergeModules = new Merge[0];

        /// <summary>
        /// Collection of the contained <see cref="ExeFileShortcut"/>s.
        /// </summary>
        public ExeFileShortcut[] Shortcuts = new ExeFileShortcut[0];

        /// <summary>
        /// Collection of directory permissions to be applied to this directory.
        /// </summary>
        public DirPermission[] Permissions = new DirPermission[0];

        /// <summary>
        /// Indicates if the directory is an installation directory.
        /// <para>
        /// Wix# assigns a dedicated WiX UI property WIXUI_INSTALLDIR
        /// to the Id value of the directory, which is marked by user as <see cref="Dir.IsInstallDir"/> or the directory
        /// with the designated Dir Id value defined by Compiler.AutoGeneration.InstallDirDefaultId ('INSTALLDIR' by default).
        /// </para>
        /// </summary>
        public bool IsInstallDir
        {
            internal get
            {
                return this.isInstallDir;
            }

            set
            {
                // It's important to recognize that 'this' can be a composite Dir that has been unfolded into
                // a branch of the child dirs (e.g. new Dir(@"%ProgramFiles%\CustomActionTest"). If it is the case then
                // the intention of the user when he uses initializer to set IsInstallDir is to set it for the directory itself
                // when it is a single directory or to the last leaf of the unfolded tree of the composite dir.
                // Getter should always return the own actual value as it is used by the compiler to decide if the dir
                //should be assigned INSTALLDIR value
                if (lastDir != null)
                    lastDir.isInstallDir = value;
                else
                    this.isInstallDir = value;
            }
        }

        bool isInstallDir;

        /// <summary>
        ///  Returns the WiX <c>Directory</c> as a string.
        /// </summary>
        /// <returns>A string representing the directory.</returns>
        public new string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Processes the target path by splitting path creating nested <see cref="Dir"/>s on-fly.
        /// </summary>
        /// <param name="targetPath">The target path.</param>
        /// <param name="feature">The feature associated with the <see cref="Dir"/>.</param>
        /// <returns></returns>
        protected Dir ProcessTargetPath(string targetPath, Feature feature)
        {
            Dir currDir = this;

            if (System.IO.Path.IsPathRooted(targetPath))
            {
                this.Name = targetPath;
            }
            else
            {
                //create nested Dirs on-fly
                var nestedDirs = targetPath.Split("\\/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                this.Name = nestedDirs.First();
                for (int i = 1; i < nestedDirs.Length; i++)
                {
                    Dir nextSubDir = new Dir(feature, nestedDirs[i]);
                    nextSubDir.AutoParent = currDir;
                    //currDir.MoveAttributesTo(nextSubDir); //attributes may not be set at this stage
                    currDir.Dirs = new Dir[] { nextSubDir };
                    currDir = nextSubDir;
                }
            }

            return currDir;
        }

        internal void AddItems(WixEntity[] items)
        {
            var files = new List<File>();
            var dirs = new List<Dir>();
            var fileCollections = new List<DirFiles>();
            var dirItemsCollections = new List<Files>();
            var shortcuts = new List<ExeFileShortcut>();
            var genericItems = new List<IGenericEntity>();
            var mergeModules = new List<Merge>();
            var dirPermissions = new List<DirPermission>();
            var odbcSources = new List<ODBCDataSource>();
            var iisVirtualDirs = new List<IISVirtualDir>();

            foreach (WixEntity item in items)
                if (item is Dir)
                    dirs.Add(item as Dir);
                else if (item is File)
                    files.Add(item as File);
                else if (item is DirFiles)
                    fileCollections.Add(item as DirFiles);
                else if (item is Files)
                    dirItemsCollections.Add(item as Files);
                else if (item is IGenericEntity)
                    genericItems.Add(item as IGenericEntity);
                else if (item is ExeFileShortcut)
                    shortcuts.Add(item as ExeFileShortcut);
                else if (item is Merge)
                    mergeModules.Add(item as Merge);
                else if (item is DirPermission)
                    dirPermissions.Add(item as DirPermission);
                else if (item is ODBCDataSource)
                    odbcSources.Add(item as ODBCDataSource);
                else if (item is IISVirtualDir)
                    iisVirtualDirs.Add(item as IISVirtualDir);
                else if (item is null)
                    continue;
                else
                    throw new Exception(item.GetType().Name + " is not expected to be a child of WixSharp.Dir");

            Files = files.ToArray();
            Dirs = dirs.ToArray();
            DirFileCollections = fileCollections.ToArray();
            FileCollections = dirItemsCollections.ToArray();
            Shortcuts = shortcuts.ToArray();
            GenericItems = genericItems.ToArray();
            MergeModules = mergeModules.ToArray();
            Permissions = dirPermissions.ToArray();
            ODBCDataSources = odbcSources.ToArray();
            IISVirtualDirs = iisVirtualDirs.ToArray();
        }
    }

    /// <summary>
    /// Defines directory to be installed on target system.
    /// <para>
    /// Use this class to define file/directory structure of the deployment solution.
    ///  You can use predefined Wix# environment constants for well-known installation locations. They are directly mapped
    ///  to the corresponding WiX constants:
    ///  <para>For the full list of the constants consult WiX documentation or use <c>Compiler.GetMappedWixConstants</c>
    ///  to explore them programatically./</para>
    ///  <para>
    ///  <para><c>Wix#</c> - <c>WiX</c></para>
    ///  <para>%WindowsFolder% - [WindowsFolder]</para>
    ///  <para>%ProgramFiles% - [ProgramFilesFolder]</para>
    ///  <para>%ProgramMenu% - [ProgramMenuFolder]</para>
    ///  <para>%CommonAppDataFolder% - [CommonAppDataFolder]</para>
    ///  <para>%AppDataFolder% - [AppDataFolder]</para>
    ///  <para>%CommonFilesFolder% - [CommonFilesFolder]</para>
    ///  <para>%LocalAppDataFolder% - [LocalAppDataFolder]</para>
    ///  <para>%ProgramFiles64Folder% - [ProgramFiles64Folder]</para>
    ///  <para>%System64Folder% - [System64Folder]</para>
    ///  <para>%SystemFolder% - [SystemFolder]</para>
    ///  <para>%TempFolder% - [TempFolder]</para>
    ///  <para>%Desktop% - [DesktopFolder]</para>
    ///  <para>...</para>
    ///  </para>
    /// </para>
    /// <para><see cref="InstallDir"/> class is identical to <see cref="Dir"/> except
    /// it already has <c>IsInstallDir</c> set to <c>true</c> so it is more convenient for
    /// defining your installation directory.</para>
    /// </summary>
    /// <example>The following is an example of defining installation directory <c>Progam Files/My Company/My Product</c>
    /// containing a single file <c>MyApp.exe</c> and subdirectory <c>Documentation</c> with <c>UserManual.pdf</c> file.
    /// <code>
    /// var project = new Project("MyProduct",
    ///         new InstallDir(@"%ProgramFiles%\My Company\My Product",
    ///             new File(@"Release\MyApp.exe"),
    ///             new Dir("Documentation",
    ///                 new File(@"Release\UserManual.pdf")),
    ///             ...
    /// </code>
    /// </example>
    public class InstallDir : Dir
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallDir"/> class.
        /// <para><see cref="InstallDir"/> class is identical to <see cref="Dir"/> except
        /// it already has <c>IsInstallDir</c> set to <c>true</c> so it is more convenient for
        /// defining your installation directory.</para>
        /// </summary>
        public InstallDir()
        {
            IsInstallDir = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallDir"/> class with properties/fields initialized with specified parameters
        /// <para><see cref="InstallDir"/> class is identical to <see cref="Dir"/> except
        /// it already has <c>IsInstallDir</c> set to <c>true</c> so it is more convenient for
        /// defining your installation directory.</para>
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="Dir"/> instance.</param>
        /// <param name="targetPath">The name of the directory. Note if the directory is a root installation directory <c>targetPath</c> must
        /// be specified as a full path. However if the directory is a nested installation directory the name must be a directory name only.</param>
        /// <param name="items">Any <see cref="WixEntity"/> which can be contained by directory (e.g. file, subdirectory).</param>
        public InstallDir(Id id, string targetPath, params WixEntity[] items)
        {
            lastDir = ProcessTargetPath(targetPath, null);
            lastDir.AddItems(items);
            lastDir.Id = id;
            lastDir.IsInstallDir = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallDir"/> class with properties/fields initialized with specified parameters
        /// <para><see cref="InstallDir"/> class is identical to <see cref="Dir"/> except
        /// it already has <c>IsInstallDir</c> set to <c>true</c> so it is more convenient for
        /// defining your installation directory.</para>
        /// </summary>
        /// <param name="targetPath">The name of the directory. Note if the directory is a root installation directory <c>targetPath</c> must
        /// be specified as a full path. However if the directory is a nested installation directory the name must be a directory name only.</param>
        /// <param name="items">Any <see cref="WixEntity"/> which can be contained by directory (e.g. file, subdirectory).</param>
        public InstallDir(string targetPath, params WixEntity[] items)
        {
            lastDir = ProcessTargetPath(targetPath, null);
            lastDir.AddItems(items);
            lastDir.IsInstallDir = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallDir"/> class with properties/fields initialized with specified parameters
        /// <para><see cref="InstallDir"/> class is identical to <see cref="Dir"/> except
        /// it already has <c>IsInstallDir</c> set to <c>true</c> so it is more convenient for
        /// defining your installation directory.</para>
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the directory should be included in.</param>
        /// <param name="targetPath">The name of the directory. Note if the directory is a root installation directory <c>targetPath</c> must
        /// be specified as a full path. However if the directory is a nested installation directory the name must be a directory name only.</param>
        /// <param name="items">Any <see cref="WixEntity"/> which can be contained by directory (e.g. file, subdirectory).</param>
        public InstallDir(Feature feature, string targetPath, params WixEntity[] items)
        {
            lastDir = ProcessTargetPath(targetPath, feature);
            lastDir.AddItems(items);
            lastDir.Feature = feature;
            lastDir.IsInstallDir = true;
        }
    }
}