using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WindowsInstaller;

namespace WixSharp.UI
{
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

namespace WixSharp
{
    public static class EmbedTransform
    {
        static void check(this MsiError result, string errorContext = "")
        {
            if (result != MsiError.NoError) throw new ApplicationException("Error: EmbedTransform.Embed->" + errorContext);
        }

        public static void Do(string msi, string mst)
        {
            MsiInterop.MsiOpenDatabase(msi, MsiDbPersistMode.ReadWrite, out IntPtr db).check(nameof(MsiInterop.MsiOpenDatabase));
            MsiInterop.MsiDatabaseOpenView(db, "SELECT `Name`,`Data` FROM _Storages", out IntPtr view).check(nameof(MsiInterop.MsiDatabaseOpenView));

            var record = MsiInterop.MsiCreateRecord(2);

            MsiInterop.MsiRecordSetString(record, 1, Path.GetFileName(mst)).check(nameof(MsiInterop.MsiRecordSetString));
            MsiInterop.MsiRecordSetStream(record, 2, mst).check(nameof(MsiInterop.MsiRecordSetStream));

            MsiInterop.MsiViewExecute(view, record).check(nameof(MsiInterop.MsiViewExecute));
            MsiInterop.MsiViewModify(view, MsiModifyMode.ModifyAssign, record).check(nameof(MsiInterop.MsiViewModify));
            MsiInterop.MsiDatabaseCommit(db).check(nameof(MsiInterop.MsiDatabaseCommit));

            MsiInterop.MsiCloseHandle(view).check(nameof(MsiInterop.MsiCloseHandle));
            MsiInterop.MsiCloseHandle(db).check(nameof(MsiInterop.MsiCloseHandle));
        }
    }
}