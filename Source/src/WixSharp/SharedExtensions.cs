using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#if WIXSHARP_MSI

namespace WixSharp.Msi
#else

namespace WixSharp
#endif
{
    /// <summary>
    /// Collection of generic WixSharp extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Returns <c>true</c> if the OS (this routine is executed on) has an x64 CPU architecture.
        /// </summary>
        /// <returns></returns>
        public static bool Is64OS()
        {
            //cannot use Environment.Is64BitOperatingSystem class as it is v3.5
            string progFiles = Environment.SpecialFolder.ProgramFiles.ToPath();
            string progFiles32 = progFiles;
            if (!progFiles32.EndsWith(" (x86)"))
                progFiles32 += " (x86)";

            return Directory.Exists(progFiles32);
        }

        /// <summary>
        /// Returns full path of the <see cref="Environment.SpecialFolder"/>
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string ToPath(this Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }

        /// <summary>
        /// 'Interprets' a string as a WiX constant and expands into a proper File System path. For example "DesktopFolder"
        /// will be expanded into "[SysDrive]:\Users\[user]\Desktop". This method is a logical equivalent of C# Environment.GetFolderPath.
        /// Though it handles discrepancies between 'special folders' mapping in .NET and WiX.
        ///
        /// <remarks>
        /// The method will always be called from x86 runtime as MSI always loads ManagedUI in x86 host.
        /// From the other hand CustomActions are called in the deployment specific CPU type context.
        /// </remarks>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string AsWixVarToPath(this string path)
        {
            // https://wixtoolset.org/docs/schema/wxs/standarddirectorytype/

            var map = new Dictionary<string, string>
            {
                { "AdminToolsFolder", Environment.SpecialFolder.ApplicationData.ToPath().PathJoin(@"Microsoft\Windows\Start Menu\Programs\Administrative Tools") },
                { "AppDataFolder", Environment.SpecialFolder.ApplicationData.ToPath() },

                { "CommonAppDataFolder", Environment.SpecialFolder.CommonApplicationData.ToPath() },
                { "CommonFilesFolder", Environment.SpecialFolder.CommonProgramFiles.ToPath() },
                { "CommonFiles64Folder", Environment.SpecialFolder.CommonProgramFiles.ToPath().Replace(" (x86)", "") },

                //{ "CommonFiles6432Folder", ????Environment.SpecialFolder.CommonProgramFiles.ToPath().Replace(" (x86)", "") },

                { "DesktopFolder", Environment.SpecialFolder.Desktop.ToPath() },
                { "FavoritesFolder", Environment.SpecialFolder.Favorites.ToPath() },
                { "FontsFolder", Environment.SpecialFolder.System.ToPath().PathGetDirName().PathJoin("Fonts") },
                { "LocalAppDataFolder", Environment.SpecialFolder.LocalApplicationData.ToPath() },
                { "MyPicturesFolder", Environment.SpecialFolder.MyPictures.ToPath() },

                //{ "NetHoodFolder", ???Environment.SpecialFolder.MyPictures.ToPath() },

                { "PersonalFolder", Environment.SpecialFolder.Personal.ToPath() },

                //{ "PrintHoodFolder", ???Environment.SpecialFolder.MyPictures.ToPath() },

                { "ProgramFilesFolder", Environment.SpecialFolder.ProgramFiles.ToPath() },
                { "ProgramFiles64Folder", Environment.SpecialFolder.ProgramFiles.ToPath().Replace(" (x86)", "") },

                //{ "ProgramFiles6432Folder", ???Environment.SpecialFolder.ProgramFiles.ToPath().Replace(" (x86)", "") },

                { "ProgramMenuFolder", Environment.SpecialFolder.Programs.ToPath() },

                //{ "RecentFolder", ???Environment.SpecialFolder.Programs.ToPath() },

                { "SendToFolder", Environment.SpecialFolder.SendTo.ToPath() },
                { "StartMenuFolder", Environment.SpecialFolder.StartMenu.ToPath() },
                { "StartupFolder", Environment.SpecialFolder.Startup.ToPath() },

                // WiX4 introduced new constants `PFiles64` and `PFiles`
                { "PFiles", Environment.SpecialFolder.ProgramFiles.ToPath() },
                { "PFiles64", "ProgramW6432".GetEnvVar(defaultValue: Environment.SpecialFolder.ProgramFiles.ToPath()) }, // ProgramW6432 returns PF64 even if it is called from the 32-bit process

                { "SystemFolder", Is64OS() ? Path.Combine(Environment.SpecialFolder.System.ToPath().PathGetDirName(), "SysWow64") : Environment.SpecialFolder.System.ToPath() },
                { "System16Folder", Path.Combine(Environment.SpecialFolder.System.ToPath().PathGetDirName(), "System") },
                { "System64Folder", Environment.SpecialFolder.System.ToPath() },

                //{ "System6432Folder", ???Environment.SpecialFolder.System.ToPath() },

                { "TempFolder", Path.GetTempPath() },
                { "TemplateFolder", Environment.SpecialFolder.Templates.ToPath() },
                { "WindowsFolder", Environment.SpecialFolder.System.ToPath().PathGetDirName() },
                { "WindowsVolume", Path.GetPathRoot(Environment.SpecialFolder.Programs.ToPath()) },
            };

            var wix3Constant = path; // coming from old WiX3 targeting WixSharp user project
            var wix4Constant = path + "Folder";
            if (map.ContainsKey(wix3Constant))
                return map[wix3Constant];
            else if (map.ContainsKey(wix4Constant))
                return map[wix4Constant];
            else
                return path;
        }

        /// <summary>
        /// Gets the environment variable by the `name`.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string GetEnvVar(this string name, string defaultValue = null)
            => Environment.GetEnvironmentVariable(name) ?? defaultValue;

        /// <summary>
        /// Equivalent of <see cref="Path.Combine(string[])"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string PathJoin(this string path, params string[] items)
        {
            foreach (var item in items)
                path = System.IO.Path.Combine(path, item);
            return path;
        }

        /// <summary>
        /// Identical to <see cref="System.IO.Path.GetDirectoryName(string)"/>. It is useful for Wix# consuming code as it allows avoiding
        /// "using System.IO;" directive, which interferes with Wix# types.
        /// </summary>
        /// <param name="path">The path.</param>
        public static string PathGetDirName(this string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }
    }
}