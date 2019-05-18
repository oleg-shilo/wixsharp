using System;
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
            switch (path)
            {
                case "AdminToolsFolder":
                    return Environment.SpecialFolder.ApplicationData.ToPath().PathJoin(@"Microsoft\Windows\Start Menu\Programs\Administrative Tools");

                case "AppDataFolder": return Environment.SpecialFolder.ApplicationData.ToPath();
                case "CommonAppDataFolder": return Environment.SpecialFolder.CommonApplicationData.ToPath();

                case "CommonFiles64Folder": return Environment.SpecialFolder.CommonProgramFiles.ToPath().Replace(" (x86)", "");
                case "CommonFilesFolder": return Environment.SpecialFolder.CommonProgramFiles.ToPath();

                case "DesktopFolder": return Environment.SpecialFolder.Desktop.ToPath();
                case "FavoritesFolder": return Environment.SpecialFolder.Favorites.ToPath();

                case "ProgramFiles64Folder": return Environment.SpecialFolder.ProgramFiles.ToPath().Replace(" (x86)", "");
                case "ProgramFilesFolder": return Environment.SpecialFolder.ProgramFiles.ToPath();

                case "MyPicturesFolder": return Environment.SpecialFolder.MyPictures.ToPath();
                case "SendToFolder": return Environment.SpecialFolder.SendTo.ToPath();
                case "LocalAppDataFolder": return Environment.SpecialFolder.LocalApplicationData.ToPath();
                case "PersonalFolder": return Environment.SpecialFolder.Personal.ToPath();

                case "StartMenuFolder": return Environment.SpecialFolder.StartMenu.ToPath();
                case "StartupFolder": return Environment.SpecialFolder.Startup.ToPath();
                case "ProgramMenuFolder": return Environment.SpecialFolder.Programs.ToPath();

                case "System16Folder": return Path.Combine("WindowsFolder".AsWixVarToPath(), "System");
                case "System64Folder": return Environment.SpecialFolder.System.ToPath();
                case "SystemFolder": return Is64OS() ? Path.Combine("WindowsFolder".AsWixVarToPath(), "SysWow64") : Environment.SpecialFolder.System.ToPath();

                case "TemplateFolder": return Environment.SpecialFolder.Templates.ToPath();
                case "WindowsVolume": return Path.GetPathRoot(Environment.SpecialFolder.Programs.ToPath());
                case "WindowsFolder": return Environment.SpecialFolder.System.ToPath().PathGetDirName();
                case "FontsFolder": return Environment.SpecialFolder.System.ToPath().PathGetDirName().PathJoin("Fonts");
                case "TempFolder": return Path.GetTempPath();
                // case "TempFolder": return Path.GetDirectoryName(Environment.SpecialFolder.Desktop.ToPath().Ge, @"Local Settings\Temp");
                default:
                    return path;
            }
        }

        /// <summary>
        /// Equivalent of <see cref="System.IO.Path.Combine"/>.
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