using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
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

    /// <summary>
    /// Represents the summary information stream of an MSI file.
    /// </summary>
    public class MsiSummary
    {
        /// <summary>
        /// Gets or sets the package code of the MSI file.
        /// </summary>
        public string PackageCode;
        /// <summary>
        /// Gets or sets the creation time of the MSI file in UTC.
        /// </summary>
        public DateTime? CreateTimeUtc;
        /// <summary>
        /// Gets or sets the last save time of the MSI file in UTC.
        /// </summary>
        public DateTime? LastSaveTimeUtc;
    }


    /// <summary>
    /// Provides methods for reading and writing MSI summary information.
    /// </summary>
    public static class MsiSummaryIO
    {
        const uint VT_EMPTY = 0;

        // Summary information property IDs
        const uint PID_REVNUMBER = 9;     // Package Code
        const uint PID_CREATE_DTM = 12;
        const uint PID_LASTSAVE_DTM = 13;

        // Variant types
        const uint VT_LPSTR = 30;
        const uint VT_FILETIME = 64;

        const int MSIDBOPEN_TRANSACT = 1;

        // Overload for string properties
        [DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiSummaryInfoGetPropertyW")]
        static extern MsiError MsiSummaryInfoGetPropertyString(IntPtr hSummary, uint property, out uint dataType, out int intValue, IntPtr fileTimeValue, StringBuilder stringValue, ref uint stringLength);
        [DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiSummaryInfoGetPropertyW")]
        static extern MsiError MsiSummaryInfoGetPropertyTime(IntPtr hSummary, uint property, out uint dataType, out int intValue, out ComTypes.FILETIME fileTimeValue, IntPtr stringValue, IntPtr stringLength);

        /// <summary>
        /// Reads the summary information from the specified MSI file and returns it as an <see cref="MsiSummary"/> object.
        /// </summary>
        /// <param name="msiPath">The path to the MSI file.</param>
        /// <returns>An <see cref="MsiSummary"/> object containing the summary information.</returns>
        public static MsiSummary Read(string msiPath)
        {
            IntPtr si = IntPtr.Zero;
            try
            {
                MsiInterop.MsiGetSummaryInformation(IntPtr.Zero, msiPath, 0, out si).check(nameof(MsiInterop.MsiGetSummaryInformation));

                var result = new MsiSummary();
                result.PackageCode = GetString(si, PID_REVNUMBER);
                result.CreateTimeUtc = GetTime(si, PID_CREATE_DTM);
                result.LastSaveTimeUtc = GetTime(si, PID_LASTSAVE_DTM);
                return result;
            }
            finally
            {
                if (si != IntPtr.Zero) MsiInterop.MsiCloseHandle(si);
            }
        }

        static string GetString(IntPtr si, uint property)
        {
            uint type;
            int dummy;
            uint length = 64;
            StringBuilder sb = new StringBuilder((int)length);

            var rc = MsiSummaryInfoGetPropertyString(
                si, property, out type, out dummy, IntPtr.Zero, sb, ref length);

            if (rc == MsiError.ERROR_MORE_DATA)
            {
                sb = new StringBuilder((int)length + 1);
                length++;
                rc = MsiSummaryInfoGetPropertyString(
                    si, property, out type, out dummy, IntPtr.Zero, sb, ref length);
            }

            rc.check("MsiSummaryInfoGetProperty(" + property + ")");

            return type == VT_LPSTR ? sb.ToString() : null;
        }

        static DateTime? GetTime(IntPtr si, uint property)
        {
            uint type;
            int dummy;
            System.Runtime.InteropServices.ComTypes.FILETIME ft;

            MsiSummaryInfoGetPropertyTime(
                    si, property, out type, out dummy, out ft, IntPtr.Zero, IntPtr.Zero).check("MsiSummaryInfoGetProperty(" + property + ")");

            if (type != VT_FILETIME)
                return null;   // VT_EMPTY: property not present

            long ticks = ((long)(uint)ft.dwHighDateTime << 32) | (uint)ft.dwLowDateTime;
            return DateTime.FromFileTimeUtc(ticks);
        }

        /// <summary>
        /// Writes the specified package code and timestamp to the summary information of the MSI file.
        /// </summary>
        /// <param name="msiPath">The path to the MSI file.</param>
        /// <param name="packageCode">The package code to write.</param>
        /// <param name="utcTimestamp">The timestamp to write. If null, the timestamp will not be written.</param>
        /// <returns>The package code string that was written to the summary information.</returns>
        public static string PatchSummary(this string msiPath, Guid? packageCode, DateTime? utcTimestamp = null)
        {
            var packageCodeStr = packageCode?.ToString("B").ToUpperInvariant() ?? "<UNSPECIFIED>";
            PatchSummary(msiPath, packageCodeStr, utcTimestamp);
            return packageCodeStr;
        }

        /// <summary>
        /// Writes the specified package code and timestamp to the summary information of the MSI file.
        /// </summary>
        /// <param name="msiPath">The path to the MSI file.</param>
        /// <param name="packageCode">The package code to write.</param>
        /// <param name="utcTimestamp">The timestamp to write. If null, the timestamp will not be written.</param>
        public static void PatchSummary(this string msiPath, string packageCode, DateTime? utcTimestamp = null)
        {
            var db = IntPtr.Zero;
            var si = IntPtr.Zero;
            try
            {
                MsiInterop.MsiOpenDatabase(msiPath, MsiDbPersistMode.ReadWrite, out db).check("MsiOpenDatabase");
                MsiInterop.MsiGetSummaryInformation(db, null, 10, out si).check("MsiGetSummaryInformation");

                var none = new ComTypes.FILETIME();
                MsiInterop.MsiSummaryInfoSetProperty(si, PID_REVNUMBER, VT_LPSTR, 0, ref none, packageCode).check("Set PackageCode");

                if (utcTimestamp != null)
                {
                    long ft = utcTimestamp.Value.ToFileTimeUtc();
                    var time = new ComTypes.FILETIME();
                    time.dwLowDateTime = unchecked((int)(ft & 0xFFFFFFFFL));
                    time.dwHighDateTime = (int)(ft >> 32);
                    MsiInterop.MsiSummaryInfoSetProperty(si, PID_CREATE_DTM, VT_FILETIME, 0, ref time, null).check("Set CreateTime");
                    MsiInterop.MsiSummaryInfoSetProperty(si, PID_LASTSAVE_DTM, VT_FILETIME, 0, ref time, null).check("Set LastSaveTime");
                }

                MsiInterop.MsiSummaryInfoPersist(si).check(nameof(MsiInterop.MsiSummaryInfoPersist));
                MsiInterop.MsiDatabaseCommit(db).check(nameof(MsiInterop.MsiDatabaseCommit));
            }
            finally
            {
                if (si != IntPtr.Zero) MsiInterop.MsiCloseHandle(si);
                if (db != IntPtr.Zero) MsiInterop.MsiCloseHandle(db);
            }
        }

        static void check(this MsiError result, string errorContext = "")
        {
            if (result != MsiError.NoError)
                throw new ApplicationException($"Error: {errorContext} failed with error {result}");
        }
    }
}