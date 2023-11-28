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
using Path = System.IO.Path;

#pragma warning disable S2223 // Non-constant static fields should not be visible

namespace WixSharp
{
    /// <summary>
    /// Controls activation of the Wix# compiler features.
    /// </summary>
    public enum CompilerSupportState
    {
        /// <summary>
        /// The feature will be enabled automatically when needed
        /// </summary>
        [Obsolete(message: "This value is no longer used by any of the compiler features. " +
                  "Use `CompilerSupportState.Enabled` instead.", error: false)]
        Automatic = 0,

        /// <summary>
        /// The feature will be enabled
        /// </summary>
        Enabled = 0,

        /// <summary>
        /// The feature will be disabled
        /// </summary>
        Disabled = 1
    }

    /// <summary>
    /// Automatically insert elements required for satisfy odd MSI restrictions.
    /// <para>- You must set KeyPath you install in the user profile.</para>
    /// <para>- You must use a registry key under HKCU as component's KeyPath, not a file. </para>
    /// <para>- The Component element cannot have multiple key path set.  </para>
    /// <para>- The project must have at least one directory element.  </para>
    /// <para>- All directories installed in the user profile must have corresponding RemoveDirectory
    /// elements.  </para>
    /// <para>...</para>
    /// <para>
    /// The MSI always wants registry keys as the key paths for per-user components.
    /// It has to do with the way profiles work with advertised content in enterprise deployments.
    /// The fact that you do not want to install any registry doesn't matter. MSI is the boss.
    /// </para>
    /// <para>The following link is a good example of the technique:
    /// http://stackoverflow.com/questions/16119708/component-testcomp-installs-to-user-profile-it-must-use-a-registry-key-under-hk</para>
    /// </summary>
    public static class AutoElements
    {
        /// <summary>
        /// Controls automatic insertion of CreateFolder and RemoveFolder for the directories containing no files.
        /// Required for: NativeBootstrapper, EmbeddedMultipleActions,  EmptyDirectories, InstallDir, Properties,
        /// ReleaseFolder, Shortcuts and WildCardFiles samples.
        /// </summary>
        public static CompilerSupportState SupportEmptyDirectories = CompilerSupportState.Enabled;

        /// <summary>
        /// Disables automatic insertion of <c>KeyPath=yes</c> attribute for the Component element.
        /// Required for: NativeBootstrapper, EmbeddedMultipleActions,  EmptyDirectories, InstallDir, Properties,
        /// ReleaseFolder, Shortcuts and WildCardFiles samples.
        /// <para>Can also be managed by disabling ICE validation via Light.exe command line arguments.</para>
        /// <para>
        /// This flag is a lighter alternative of DisableAutoCreateFolder.
        /// <para>
        /// See: http://stackoverflow.com/questions/10358989/wix-using-keypath-on-components-directories-files-registry-etc-etc
        /// </para>
        /// for some background info.
        /// </para>
        /// </summary>
        public static bool DisableAutoKeyPath = false;

        /// <summary>
        /// <para>
        /// Flag indicating if the legacy algorithm should be used for auto-assigning directory IDs when they are not defined by user.
        /// </para>
        /// The old(historical) algorithm composes the id value as a "path-like" chain of the names of a directory and his parent directories.
        /// This can lead sometimes to excessively long values.
        /// If this flag is <c>false</c> (default) the  default ID assignment algorithm is used as for any other <see cref="WixEntity"/>
        /// ("&lt;dir_name&gt;[.unique_index]").
        /// </summary>
        public static bool LegacyDirIdAlgorithm = false;

        /// <summary>
        /// Enables expanding Wix environment constants in <see cref="WixSharp.RegValue.Value"/>.
        /// <para>This flag was introduced as a fall back mechanism for legacy action of expanding Wix
        /// constants in registry values. This work around was triggered by issue #774.</para>
        /// </summary>
        public static bool ExpandWixConstsInRegValue = false;

        /// <summary>
        /// Enables UAC revealer, which is a work around for the MSI limitation/problem around EmbeddedUI UAC prompt.
        /// <para> The symptom of the problem is the UAC prompt not being displayed during elevation but rather minimized
        /// on the taskbar. This is caused by the fact the all background applications (including MSI runtime) supposed to
        /// register the main window for UAC prompt. And, MSI does not do the registration for EmbeddedUI.
        /// </para>
        /// <para> See "Use the HWND Property to Be Acknowledged as a Foreground Application" section at
        /// https://msdn.microsoft.com/en-us/library/bb756922.aspx
        /// </para>
        /// </summary>
        public static bool EnableUACRevealer = false;

        /// <summary>
        /// The UAC warning message to be displayed at the start of the actual installation (Progress dialog)
        /// of the ManagedUI setup.
        /// <para>The purpose of this message is to draw user attention to the fact that Windows UAC prompt may not
        /// become visible and instead be minimized on the taskbar.
        /// </para>
        /// <remarks>
        /// Windows prevents UIC prompt from stealing the focus if at the time of elevation user performs
        /// interaction with other foreground process (application). It is a controversial aspect of Windows
        /// User Experience that sometimes has undesirable practical implications.
        /// </remarks>
        /// </summary>
        public static string UACWarning = "Please wait for UAC prompt to appear.\r\n\r\nIf it appears minimized then" +
            " activate it from the taskbar.";

        /// <summary>
        /// Forces all <see cref="T:WixSharp.Condition"/> values to be always encoded as CDATA.
        /// </summary>
        public static bool ForceCDataForConditions = false;

        /// <summary>
        /// Flag indicating if the legacy algorithm should be used for handling setups with no directories
        /// to be installed but only non-file system components (e.g. RegKey, User, Firewall exceptions).
        /// <para>The algorithm used in early versions of WixSharp (legacy algorithm) injects a dummy
        /// directory into the setup definition so it satisfies the MSI constraint that every component must
        /// belong to a directory. As many other rules this one has no practical value and rather reflection
        /// of the outdated (~20 years ago) deployment approaches.</para>
        /// <para>The current algorithm also ensures the that there is a XML directory to host the components.
        /// However instead of custom (dummy) directory it inserts 'ProgramFilesFolder'. This way MSI constraint
        /// is satisfied and yet there is no impact on the target file system.</para>
        /// </summary>
        public static bool LegacyDummyDirAlgorithm = false;

        /// <summary>
        /// Enables scheduling deferred actions just after their corresponding
        /// "SetDeferredActionProperties" custom action. Enabled by default.
        /// </summary>
        public static bool ScheduleDeferredActionsAfterTunnelingTheirProperties = true;

        /// <summary>
        /// Disables automatic insertion of user profile registry elements.
        /// Required for: AllInOne, ConditionalInstallation, CustomAttributes, ReleaseFolder, Shortcuts,
        /// Shortcuts (advertised), Shortcuts-2, WildCardFiles samples.
        /// <para>Can also be managed by disabling ICE validation via Light.exe command line arguments.</para>
        /// </summary>
        public static bool DisableAutoUserProfileRegistry = false;

        /// <summary>
        /// Forces automatic insertion of the user profile registry. It is a controversial feature that is only required
        /// for some exotic scenarios. See Issue #1326 (https://github.com/oleg-shilo/wixsharp/issues/1326).
        /// <para>
        /// If inserting user profile for all components is undesirable then you can do it for a specific component manually:
        /// <code>
        /// project.WixSourceGenerated += doc =>
        /// {
        ///     doc.FindAll("Component")
        ///        .Where(x => x.HasAttribute("Id", val => val.Contains("my_component")))
        ///        .ForEach(AutoElements.InsertUserProfileRegValue);
        /// };
        /// </code>
        /// </para>
        /// <para>The problem this feature is trying to solve is closely related to the one handled by
        /// <see cref="WixSharp.AutoElements.DisableAutoUserProfileRegistry"/>. </para>
        /// </summary>
        public static bool ForceUserProfileRegistry = false;

        static void InsertRemoveFolder(XElement xDir, XElement xComponent, string when = "uninstall")
        {
            if (!xDir.IsUserProfileRoot())
            {
                string dirId = xDir.Attribute("Id").Value;
                bool alreadyPresent = xComponent.Elements("RemoveFolder")
                                                .Where(x => x.HasAttribute("Id", dirId))
                                                .Any();
                if (!alreadyPresent)
                    xComponent.Add(new XElement("RemoveFolder",
                                                new XAttribute("Id", xDir.Attribute("Id").Value),
                                                new XAttribute("On", when)));
            }
        }

        internal static XElement InsertUserProfileRemoveFolder(this XElement xComponent)
        {
            var xDir = xComponent.Parent("Directory");
            if (!xDir.Descendants("RemoveFolder").Any() && !xDir.IsUserProfileRoot())
                xComponent.Add(new XElement("RemoveFolder",
                                            new XAttribute("Id", xDir.Attribute("Id").Value),
                                            new XAttribute("On", "uninstall")));

            return xComponent;
        }

        static void InsertCreateFolder(XElement xComponent)
        {
            //prevent adding more than 1 CreateFolder elements - elements that don't specify @Directory
            if (xComponent.Elements("CreateFolder").All(element => element.HasAttribute("Directory")))
                xComponent.Add(new XElement("CreateFolder"));

            EnsureKeyPath(xComponent);
        }

        static void EnsureKeyPath(XElement xComponent)
        {
            if (!DisableAutoKeyPath)
            {
                // A component must have KeyPath set on itself or on a single (just one) nested element.
                // may conflict with the folder removal (https://github.com/oleg-shilo/wixsharp/issues/123).
                if (!xComponent.HasKeyPathElements())
                    xComponent.SetAttribute("KeyPath=yes");
            }
        }

        internal static string FindInstallDirId(this XElement product)
        {
            if (product.FindAll("Directory")
                       .Any(p => p.HasAttribute("Id", Compiler.AutoGeneration.InstallDirDefaultId)))
                return Compiler.AutoGeneration.InstallDirDefaultId;
            return null;
        }

        internal static bool HasKeyPathElements(this XElement xComponent)
        {
            return xComponent.Descendants()
                             .Where(e => e.HasKeyPathSet())
                             .Any();
        }

        internal static XElement ClearKeyPath(this XElement element)
        {
            return element.SetAttribute("KeyPath", null);
        }

        internal static bool HasKeyPathSet(this XElement element)
        {
            var attr = element.Attribute("KeyPath");

            if (attr != null && attr.Value == "yes")
                return true;
            return false;
        }

        /// <summary>
        /// Inserts the user profile reg value. It is a dummy registry value that is required for some
        /// deployment scenarios to work correctly. See <see cref="WixSharp.AutoElements.DisableAutoUserProfileRegistry"/>
        /// for details.
        /// </summary>
        /// <param name="xComponent">The component.</param>
        /// <returns></returns>
        public static XElement InsertUserProfileRegValue(this XElement xComponent)
        {
            // UserProfileRegValue has to be a KeyPath so need to remove any KeyPath on other elements
            var keyPathes = xComponent.Descendants()
                                      .ForEach(e => e.ClearKeyPath());

            xComponent.ClearKeyPath();

            xComponent.Add(
                           new XElement("RegistryKey",
                           new XAttribute("Root", "HKCU"),
                           new XAttribute("Key", @"Software\WixSharp\Used"),
                           new XElement("RegistryValue",
                               new XAttribute("Value", "0"),
                               new XAttribute("Type", "string"),
                               new XAttribute("KeyPath", "yes"))));
            return xComponent;
        }

        static void InsertDummyUserProfileRegistry(XElement xComponent)
        {
            if (!DisableAutoUserProfileRegistry)
            {
                InsertUserProfileRegValue(xComponent);
            }
        }

        static void SetFileKeyPath(XElement element, bool isKeyPath = true)
        {
            if (element.Attribute("KeyPath") == null)
                element.Add(new XAttribute("KeyPath", isKeyPath ? "yes" : "no"));
        }

        static bool ContainsDummyUserProfileRegistry(this XElement xComponent)
        {
            return (from e in xComponent.Elements("RegistryKey")
                    where e.Attribute("Key") != null && e.Attribute("Key").Value == @"Software\WixSharp\Used"
                    select e).Count() != 0;
        }

        static bool ContainsAnyRemoveFolder(this XElement xDir)
        {
            //RemoveFolder is expected to be enclosed in Component and appear only once per Directory element
            return xDir.Elements("Component")
                       .SelectMany(c => c.Elements("RemoveFolder"))
                       .Any();
        }

        static bool ContainsFiles(this XElement xComp)
        {
            return xComp.Elements("File").Any();
        }

        static bool ContainsFilesOrRegistries(this XElement xComp)
        {
            return xComp.Elements("File").Any() || xComp.Elements("RegistryKey").Any();
        }

        static bool ContainsComponents(this XElement xDir)
        {
            return xDir.Elements("Component").Any();
        }

        static bool ContainsAdvertisedShortcuts(this XElement xComp)
        {
            var advertisedShortcuts = from e in xComp.Descendants("Shortcut")
                                      where e.Attribute("Advertise") != null && e.Attribute("Advertise").Value == "yes"
                                      select e;

            return (advertisedShortcuts.Count() != 0);
        }

        static bool ContainsNonAdvertisedShortcuts(this XElement xComp)
        {
            var nonAdvertisedShortcuts = from e in xComp.Descendants("Shortcut")
                                         where e.Attribute("Advertise") == null || e.Attribute("Advertise").Value == "no"
                                         select e;

            return (nonAdvertisedShortcuts.Count() != 0);
        }

        static XElement CreateComponentFor(this XDocument doc, XElement xDir)
        {
            string compId = xDir.Attribute("Id").Value;
            XElement xComponent = xDir.AddElement(
                                  new XElement("Component",
                                  new XAttribute("Id", compId),
                                  new XAttribute("Guid", WixGuid.NewGuid(compId))));

            foreach (XElement xFeature in doc.Root.Descendants("Feature"))
                xFeature.Add(new XElement("ComponentRef",
                    new XAttribute("Id", xComponent.Attribute("Id").Value)));

            return xComponent;
        }

        private static string[] GetUserProfileFolders()
        {
            return new[]
                   {
                       "ProgramMenuFolder",
                       "AppDataFolder",
                       "LocalAppDataFolder",
                       "TempFolder",
                       "PersonalFolder",
                       "DesktopFolder",
                       "StartupFolder"
                   };
        }

        static bool InUserProfile(this XElement xDir)
        {
            string[] userProfileFolders = GetUserProfileFolders();

            XElement xParentDir = xDir;
            do
            {
                if (xParentDir.Name == "Directory")
                {
                    var attrName = xParentDir.Attribute("Name").Value;

                    if (userProfileFolders.Contains(attrName))
                        return true;
                }
                xParentDir = xParentDir.Parent;
            }
            while (xParentDir != null);

            return false;
        }

        static bool IsUserProfileRoot(this XElement xDir)
        {
            string[] userProfileFolders = GetUserProfileFolders();

            return userProfileFolders.Contains(xDir.Attribute("Name").Value);
        }

        internal static void InjectShortcutIcons(XDocument doc)
        {
            var shortcuts = from s in doc.Root.Descendants("Shortcut")
                            where s.HasAttribute("Icon")
                            select s;

            int iconIndex = 1;

            var icons = new Dictionary<string, string>();
            foreach (var iconFile in (from s in shortcuts
                                      select s.Attribute("Icon").Value).Distinct())
            {
                icons.Add(iconFile,
                    "IconFile" + (iconIndex++) + "_" + IO.Path.GetFileName(iconFile).Expand());
            }

            foreach (XElement shortcut in shortcuts)
            {
                string iconFile = shortcut.Attribute("Icon").Value;
                string iconId = icons[iconFile];
                shortcut.Attribute("Icon").Value = iconId;
            }

            XElement product = doc.Root.Select("Product");

            foreach (string file in icons.Keys)
                product.AddElement(
                    new XElement("Icon",
                        new XAttribute("Id", icons[file]),
                        new XAttribute("SourceFile", file)));
        }

        static void InjectPlatformAttributes(XDocument doc)
        {
            var is64BitPlatform = doc.Root.Select("Product/Package").HasAttribute("Platform", val => val == "x64");

            if (is64BitPlatform)
            {
                doc.Descendants("Component")
                   .ForEach(comp =>
                   {
                       if (!comp.HasAttribute("Win64"))
                           comp.SetAttributeValue("Win64", "yes");
                   });
            }
        }

        internal static void ExpandCustomAttributes(XDocument doc, WixProject project)
        {
            foreach (XAttribute instructionAttr in doc.Root.Descendants().Select(x => x.Attribute("WixSharpCustomAttributes")).Where(x => x != null))
            {
                XElement sourceElement = instructionAttr.Parent;

                foreach (string item in instructionAttr.Value.Split(';'))
                    if (item.IsNotEmpty())
                    {
                        if (!ExpandCustomAttribute(sourceElement, item, project))
                        {
                            var message = "Cannot resolve custom attribute definition:" + item;
                            if (item.StartsWith("Component:"))
                                message += "\nNote, some Wix elements may not be contained by 'Component' elements (e.g. 'CloseApplication'). " +
                                    "Thus attempt to set parent component attribute will always fail.\n";

                            throw new ApplicationException(message);
                        }
                    }

                instructionAttr.Remove();
            }
        }

        static Func<XElement, string, WixProject, bool> ExpandCustomAttribute = DefaultExpandCustomAttribute;

        static bool DefaultExpandCustomAttribute(XElement source, string item, WixProject project)
        {
            var attrParts = item.Split(new[] { '=' }, 2, StringSplitOptions.None);

            // {dep}ProductKey=12345 vs
            // Component:{dep}ProductKey=12345 vs
            // {http://schemas.microsoft.com/wix/BalExtension}Overridable=yes"
            // Component:{dep}elementm_Condition=base64:edr34r34r43

            // Note the syntax below is not supported:
            // Component:{http://schemas.microsoft.com/wix/BalExtension}Overridable=yes"
            if (item.Contains(":{http:") || item.Contains(":{https:"))
                throw new Exception("Syntax `" + item + "` is not supported.\n" +
                    "Use `parent:{alias}attribute=value` instead and add XML namespace with the alias.\n" +
                    "For example: `project.IncludeWixExtension(\"WixDependencyExtension.dll\", \"dep\", expectedNamespace);`");

            string[] keyParts;

            if (item.StartsWith("{"))
                item = ":" + item;

            var nameSpec = attrParts.First();

            if (nameSpec.StartsWith("{"))
                keyParts = new[] { nameSpec };  // name specification does not have any `parent prefix`
            else
                keyParts = nameSpec.Split(':'); // here it does

            string element = keyParts.First();
            string key = keyParts.Last();
            string value = attrParts.Last();

            if (element == "Component")
            {
                XElement destElement = source.Parent("Component");
                if (destElement != null)
                {
                    if (key == "element_Condition")
                    {
                        var name = "Condition";
                        var data = value;

                        if (data.StartsWith("base64_"))
                            data = data.Replace("base64_", "").Base64Decode();

                        destElement.AddElement(name, null, data);
                    }
                    else if (key == "Id")
                    {
                        var refs = destElement.Document.Root
                            .FindAll("ComponentRef")
                            .Where(x => x.HasAttribute("Id", destElement.GetAttribute(key)));

                        refs.ForEach(x => x.SetAttribute(key, value)); // reference a new Id

                        destElement.SetAttribute(key, value);
                    }
                    else
                    {
                        destElement.SetAttribute(key, value);
                    }
                    return true;
                }
            }

            if (element == "Custom" && source.Name.LocalName == "CustomAction")
            {
                string id = source.Attribute("Id").Value;
                var elements = source.Document.Descendants("Custom").Where(e => e.Attribute("Action").Value == id);
                if (elements.Any())
                {
                    elements.ForEach(e => e.SetAttribute(key, value));
                    return true;
                }
            }

            if (key.StartsWith("{"))
            {
                source.SetAttribute(key, value);
                return true;
            }

            if (key.StartsWith("xml_include"))
            {
                var parts = value.Split('|');

                string parentName = parts[0];
                string xmlFile = parts[1];

                var placement = source;
                if (!parentName.IsEmpty())
                    placement = source.Parent(parentName);

                if (placement != null)
                {
                    string xmlFilePath;

                    // Strangely enough all relative paths in the wxs are resolved with respect to the
                    // process CurrrentDir but includes it is resolved with respect to the location of the
                    // wxs file containing the include statement. Thus if there is any discrepancy between
                    // source and output dirs then it is safer to use absolute paths instead of relative.
                    if (!xmlFile.IsAbsolutePath() && project.SourceBaseDir != project.OutDir)
                        xmlFilePath = project.SourceBaseDir.ToAbsolutePath().PathCombine(xmlFile);
                    else
                        xmlFilePath = xmlFile;

                    placement.Add(new XProcessingInstruction("include", xmlFilePath));
                    return true;
                }
            }

            return false;
        }

        static void InsertEmptyComponentsInParentDirectories(XDocument doc, XElement item)
        {
            XElement parent = item.Parent("Directory");
            while (parent != null)
            {
                if (parent.Element("Component") == null)
                {
                    var dirId = parent.Attribute("Id")?.Value;
                    if (Compiler.EnvironmentConstantsMapping.ContainsValue(dirId))
                        break; //stop when reached start of user defined subdirs chain: TARGETDIR/ProgramFilesFolder!!!/ProgramFilesFolder.Company/INSTALLDIR

                    //just folder with nothing in it but not the last leaf
                    doc.CreateComponentFor(parent);
                }
                parent = parent.Parent("Directory");
            }
        }

        static void CreateEmptyComponentsInDirectoriesToRemove(XDocument doc)
        {
            XElement product = doc.Root.Select("Product");

            // Create new empty components in parent directories of components with no files or registry
            var dirsWithNoFilesOrRegistryComponents = product.Descendants("Directory")
                .SelectMany(x => x.Elements("Component"))
                .Where(e => !e.ContainsFilesOrRegistries())
                .Select(x => x.Parent("Directory"));

            foreach (var item in dirsWithNoFilesOrRegistryComponents)
            {
                InsertEmptyComponentsInParentDirectories(doc, item);
            }
        }

        internal static void HandleEmptyDirectories(XDocument doc)
        {
            XElement product = doc.Root.Select("Product");

            var dummyDirs = product.Descendants("Directory")
                                   .SelectMany(x => x.Elements("Component"))
                                   .Where(e => e.HasAttribute("Id", v => v.EndsWith(".EmptyDirectory")))
                                   .Select(x => x.Parent("Directory")).ToArray();

            if (SupportEmptyDirectories == CompilerSupportState.Enabled)
            {
                if (dummyDirs.Any())
                {
                    foreach (var item in dummyDirs)
                    {
                        InsertEmptyComponentsInParentDirectories(doc, item);
                    }
                }

                foreach (XElement xDir in product.Descendants("Directory").ToArray())
                {
                    var dirComponents = xDir.Elements("Component");

                    if (dirComponents.Any())
                    {
                        var componentsWithNoFiles = dirComponents.Where(x => !x.ContainsFiles()).ToArray();

                        //'EMPTY DIRECTORY' support processing section
                        foreach (XElement item in componentsWithNoFiles)
                        {
                            // Ridiculous MSI constraints:
                            //  * you cannot install empty folders
                            //    - workaround is to insert empty component with CreateFolder element
                            //  * if Component+CreateFolder element is inserted the folder will not be removed on uninstall
                            //    - workaround is to insert RemoveFolder element in to empty component as well
                            //  * if Component+CreateFolder+RemoveFolder elements are placed in a dummy component to handle an empty folder
                            //    any parent folder with no files/components will not be removed on uninstall.
                            //    - workaround is to insert Component+Create+RemoveFolder elements in any parent folder with no files.
                            //
                            // OMG!!!! If it is not over-engineering I don't know what is.

                            InsertCreateFolder(item);
                            if (!xDir.ContainsAnyRemoveFolder())
                                InsertRemoveFolder(xDir, item, "uninstall");
                        }
                    }
                }
            }
        }

        internal static void InjectAutoElementsHandler(XDocument doc, Project project)
        {
            ExpandCustomAttributes(doc, project);
            InjectShortcutIcons(doc);
            HandleEmptyDirectories(doc);

            XElement product = doc.Root.Select("Product");

            int? absPathCount = null;
            foreach (XElement dir in product.Element("Directory").Elements("Directory"))
            {
                XElement installDir = dir;

                XAttribute installDirName = installDir.Attribute("Name");
                if (IO.Path.IsPathRooted(installDirName.Value))
                {
                    string absolutePath = installDirName.Value;

                    if (dir == product.Element("Directory").Elements("Directory").First()) //only for the first root dir
                    {
                        //ManagedUI will need some hint on the install dir as it cannot rely on the session action (e.g. Set_INSTALLDIR_AbsolutePath)
                        //because it is running outside of the sequence and analyses the tables directly for the INSTALLDIR
                        product.AddElement("Property", "Id=INSTALLDIR_ABSOLUTEPATH; Value=" + absolutePath);
                    }

                    installDirName.Value = $"ABSOLUTEPATH{absPathCount}";

                    //<SetProperty> for INSTALLDIR is an attractive approach but it doesn't allow conditional setting of 'ui' and 'execute' as required depending on UI level
                    // it is ether hard-coded 'both' or hard coded-both 'ui' or 'execute'
                    // <SetProperty Id="INSTALLDIR" Value="C:\My Company\MyProduct" Sequence="both" Before="AppSearch">

                    string actualDirName = installDir.Attribute("Id").Value;
                    string customAction = $"Set_DirAbsolutePath{absPathCount}";

                    product.Add(new XElement("CustomAction",
                                new XAttribute("Id", customAction),
                                new XAttribute("Property", actualDirName),
                                new XAttribute("Value", absolutePath)));

                    product.SelectOrCreate("InstallExecuteSequence").Add(
                            new XElement("Custom", $"(NOT Installed) AND (UILevel < 5) AND ({actualDirName} = ABSOLUTEPATH{absPathCount})",
                                new XAttribute("Action", customAction),
                                new XAttribute("Before", "AppSearch")));

                    product.SelectOrCreate("InstallUISequence").Add(
                            new XElement("Custom", $"(NOT Installed) AND (UILevel = 5) AND ({actualDirName} = ABSOLUTEPATH{absPathCount})",
                                new XAttribute("Action", customAction),
                                new XAttribute("Before", "AppSearch")));

                    if (absPathCount == null)
                        absPathCount = 0;
                    absPathCount++;
                }
            }

            CreateEmptyComponentsInDirectoriesToRemove(doc);

            foreach (XElement xDir in product.Descendants("Directory").ToArray())
            {
                var dirComponents = xDir.Elements("Component").ToArray();

                if (dirComponents.Any())
                {
                    var componentsWithNoFilesOrRegistry = dirComponents.Where(x => !x.ContainsFilesOrRegistries()).ToArray();

                    foreach (XElement item in componentsWithNoFilesOrRegistry)
                    {
                        //if (!item.Attribute("Id").Value.EndsWith(".EmptyDirectory"))
                        EnsureKeyPath(item);

                        if (!xDir.ContainsAnyRemoveFolder())
                            InsertRemoveFolder(xDir, item, "uninstall"); //to keep WiX/compiler happy and allow removal of the dummy directory
                    }
                }

                foreach (XElement xComp in dirComponents)
                {
                    if (xDir.InUserProfile())
                    {
                        if (!xDir.ContainsAnyRemoveFolder())
                            InsertRemoveFolder(xDir, xComp);

                        if (!xComp.ContainsDummyUserProfileRegistry())
                            InsertDummyUserProfileRegistry(xComp);
                    }
                    else
                    {
                        if (xComp.ContainsNonAdvertisedShortcuts())
                            if (!xComp.ContainsDummyUserProfileRegistry())
                                InsertDummyUserProfileRegistry(xComp);
                    }

                    foreach (XElement xFile in xComp.Elements("File"))
                        if (xFile.ContainsAdvertisedShortcuts() && !xComp.ContainsDummyUserProfileRegistry())
                            SetFileKeyPath(xFile);
                }

                if (!xDir.ContainsComponents() && xDir.InUserProfile())
                {
                    if (!xDir.IsUserProfileRoot())
                    {
                        XElement xComp1 = doc.CreateComponentFor(xDir);
                        if (!xDir.ContainsAnyRemoveFolder())
                            InsertRemoveFolder(xDir, xComp1);

                        if (!xComp1.ContainsDummyUserProfileRegistry())
                            InsertDummyUserProfileRegistry(xComp1);
                    }
                }
            }

            //Not a property Id as MSI requires
            Predicate<string> needsProperty =
                value => value.Contains("\\") ||
                         value.Contains("//") ||
                         value.Contains("%") ||
                         value.Contains("[") ||
                         value.Contains("]");

            foreach (XElement xShortcut in product.Descendants("Shortcut"))
            {
                if (xShortcut.HasAttribute("WorkingDirectory", x => needsProperty(x)))
                {
                    string workingDirectory = xShortcut.Attribute("WorkingDirectory").Value;

                    if (workingDirectory.StartsWith("%") && workingDirectory.EndsWith("%")) //%INSTALLDIR%
                    {
                        workingDirectory = workingDirectory.ExpandWixEnvConsts();
                        xShortcut.SetAttributeValue("WorkingDirectory", workingDirectory.Replace("%", ""));
                    }
                    else if (workingDirectory.StartsWith("[") && workingDirectory.EndsWith("]")) //[INSTALLDIR]
                    {
                        xShortcut.SetAttributeValue("WorkingDirectory", workingDirectory.Replace("[", "").Replace("]", ""));
                    }
                    else
                    {
                        string workinDirPath = workingDirectory.ReplaceWixSharpEnvConsts();
                        XElement existingProperty = product.Descendants("Property")
                                                           .FirstOrDefault(p => p.HasAttribute("Value", workingDirectory));

                        if (existingProperty != null)
                        {
                            xShortcut.SetAttributeValue("WorkingDirectory", existingProperty.Attribute("Id").Value);
                        }
                        else
                        {
                            string propId = xShortcut.Attribute("Id").Value + ".WorkDir";
                            product.AddElement("Property", "Id=" + propId + "; Value=" + workinDirPath);
                            xShortcut.SetAttributeValue("WorkingDirectory", propId);
                        }
                    }
                }
            }

            if (ForceUserProfileRegistry)
            {
                product
                    .FindAll("Component")
                    .ForEach(x =>
                             {
                                 if (!x.ContainsDummyUserProfileRegistry())
                                     InsertUserProfileRegValue(x);
                             });
            }
            InjectPlatformAttributes(doc);
        }

        internal static void NormalizeFilePaths(XDocument doc, string sourceBaseDir, bool emitRelativePaths)
        {
            string rootDir = sourceBaseDir;
            if (rootDir.IsEmpty())
                rootDir = Environment.CurrentDirectory;

            rootDir = IO.Path.GetFullPath(rootDir);

            Action<IEnumerable<XElement>, string> normalize = (elements, attributeName) =>
                {
                    elements.Where(e => e.HasAttribute(attributeName))
                            .ForEach(e =>
                                {
                                    var attr = e.Attribute(attributeName);
                                    if (emitRelativePaths)
                                        attr.Value = Utils.MakeRelativeTo(attr.Value, rootDir);
                                    else
                                        attr.Value = Path.GetFullPath(attr.Value);
                                });
                };

            normalize(doc.Root.FindAll("Icon"), "SourceFile");
            normalize(doc.Root.FindAll("File"), "Source");
            normalize(doc.Root.FindAll("Merge"), "SourceFile");
            normalize(doc.Root.FindAll("Binary"), "SourceFile");
            normalize(doc.Root.FindAll("EmbeddedUI"), "SourceFile");
            normalize(doc.Root.FindAll("Payload"), "SourceFile");
            normalize(doc.Root.FindAll("MsiPackage"), "SourceFile");
            normalize(doc.Root.FindAll("ExePackage"), "SourceFile");
        }
    }
}