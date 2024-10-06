using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WindowsInstaller;

namespace WixSharp.UI
{
    static class LocalExtensions
    {
        /// <summary>
        /// Gets the environment variable by the `name`.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string GetEnvVar(this string name, string defaultValue = null)
            => Environment.GetEnvironmentVariable(name) ?? defaultValue;

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

        public static string PathJoin(this string path, params string[] items)
        {
            foreach (var item in items)
                path = System.IO.Path.Combine(path, item);
            return path;
        }

        public static string ToPath(this Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
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
                // WiX4 introduced new constants `PFiles64` and `PFiles`
                case "PFiles": return Environment.SpecialFolder.ProgramFiles.ToPath();
                case "PFiles64":
                    return "ProgramW6432".GetEnvVar(
                                          defaultValue: Environment.SpecialFolder.ProgramFiles.ToPath()); // ProgramW6432 returns PF64 even if it is called from the 32-bit process

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
    }

    static class MsiExtensions
    {
        public static void Invoke(Func<MsiError> action)
        {
            MsiError res = action();
            if (res != MsiError.NoError)
                throw new Exception(res.ToString());
        }

        public static IntPtr View(this IntPtr db, string sql)
        {
            IntPtr view = IntPtr.Zero;
            Invoke(() => MsiInterop.MsiDatabaseOpenView(db, sql, out view));
            Invoke(() => MsiInterop.MsiViewExecute(view, IntPtr.Zero));
            return view;
        }

        public static IntPtr NextRecord(this IntPtr view)
        {
            IntPtr record = IntPtr.Zero;
            var res = MsiInterop.MsiViewFetch(view, ref record);
            if (res == MsiError.NoMoreItems)
                return IntPtr.Zero;
            else if (res != MsiError.NoError)
                throw new Exception(res.ToString());
            return record;
        }

        public static string GetString(this IntPtr record, uint fieldIndex)
        {
            uint valueSize = 2048;
            var builder = new StringBuilder((int)valueSize);
            Invoke(() => MsiInterop.MsiRecordGetString(record, fieldIndex, builder, ref valueSize));

            return builder.ToString();
        }

        public static List<Dictionary<string, object>> GetData(this IntPtr view, bool close = true)
        {
            var data = new List<Dictionary<string, object>>();

            IntPtr rec;
            while (IntPtr.Zero != (rec = view.NextRecord()))
            {
                var row = view.GetFieldValues(rec);
                data.Add(row);
                rec.Close();
            }

            if (close)
                view.CloseView();

            return data;
        }

        public static Dictionary<string, object> GetFieldValues(this IntPtr view, IntPtr record)
        {
            IntPtr names;
            var info = (IntPtr)MsiInterop.MsiViewGetColumnInfo(view, MsiColInfoType.Names, out names);

            var result = new Dictionary<string, object>();

            for (uint i = 0; i <= MsiInterop.MsiRecordGetFieldCount(names); i++)
            {
                string name = names.GetString(i);
                result[name] = record.GetObject(i);
            }

            info.Close();
            names.Close();

            return result;
        }

        public static object GetObject(this IntPtr record, uint fieldIndex)
        {
            if (MsiInterop.MsiRecordIsNull(record, fieldIndex))
                return null;

            int result = record.GetInt(fieldIndex);
            if (result == MsiInterop.MsiNullInteger) //the field is s string
                return record.GetString(fieldIndex);
            else
                return result;
        }

        public static int GetInt(this IntPtr record, uint fieldIndex)
        {
            return MsiInterop.MsiRecordGetInteger(record, fieldIndex);
        }

        public static void Close(this IntPtr handle)
        {
            Invoke(() => MsiInterop.MsiCloseHandle(handle));
        }

        public static void CloseView(this IntPtr view)
        {
            Invoke(() => MsiInterop.MsiViewClose(view));
            Close(view);
        }

        public static int ToInt(this string obj)
        {
            return int.Parse(obj);
        }

        //Needed to treated as "1 based" arrays as it is very hared to follow the MSDN documentation.
        //http://msdn.microsoft.com/en-us/library/windows/desktop/aa370573(v=vs.85).aspx
        //Yes it is inefficient but it saves dev time and effort for translating C# data structures into MSI string fields
        public static T MSI<T>(this string[] obj, int fieldIndex)
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)(obj[fieldIndex - 1] == "0");
            else if (typeof(T) == typeof(int))
                return (T)(object)(obj[fieldIndex - 1].ToInt());
            else if (typeof(T) == typeof(string))
                return (T)(object)(obj[fieldIndex - 1]);

            //else if (typeof(T) == typeof(char))
            //{
            //    if (obj[fieldIndex + 1].Length == 1)
            //        return (T)(object)(obj[fieldIndex + 1])[0];
            //    else
            //        throw new Exception("Value contains more than a single character");
            //}
            else
                throw new Exception("Only Int32, String and Boolean conversion is supported");
        }
    }
}

namespace WixSharp.Msi
{
    /// <summary>
    ///
    /// </summary>
    public static class EmbedTransform
    {
        static void check(this MsiError result, string errorContext = "")
        {
            if (result != MsiError.NoError) throw new ApplicationException("Error: EmbedTransform.Embed->" + errorContext);
        }

        /// <summary>
        /// Embeds a language transformation (mst file) in the specified msi file.
        /// </summary>
        /// <param name="msi">The MSI file.</param>
        /// <param name="mst">The MST file.</param>
        public static void Do(string msi, string mst)
        {
            var lngId = new CultureInfo(Path.GetFileNameWithoutExtension(mst)).LCID.ToString();

            MsiInterop.MsiOpenDatabase(msi, MsiDbPersistMode.ReadWrite, out IntPtr db).check(nameof(MsiInterop.MsiOpenDatabase));
            MsiInterop.MsiDatabaseOpenView(db, "SELECT `Name`,`Data` FROM _Storages", out IntPtr view).check(nameof(MsiInterop.MsiDatabaseOpenView));

            var record = MsiInterop.MsiCreateRecord(2);
            MsiInterop.MsiRecordSetString(record, 1, lngId).check(nameof(MsiInterop.MsiRecordSetString));
            MsiInterop.MsiRecordSetStream(record, 2, mst).check(nameof(MsiInterop.MsiRecordSetStream));

            MsiInterop.MsiViewExecute(view, record).check(nameof(MsiInterop.MsiViewExecute));
            MsiInterop.MsiViewModify(view, MsiModifyMode.ModifyAssign, record).check(nameof(MsiInterop.MsiViewModify));
            MsiInterop.MsiDatabaseCommit(db).check(nameof(MsiInterop.MsiDatabaseCommit));

            MsiInterop.MsiCloseHandle(view).check(nameof(MsiInterop.MsiCloseHandle));
            MsiInterop.MsiCloseHandle(db).check(nameof(MsiInterop.MsiCloseHandle));
        }
    }
}