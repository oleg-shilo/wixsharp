using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WindowsInstaller;
using WixSharp.Msi;

namespace WixSharp.UI
{
    /// <summary>
    /// Utility class for simplifying MSI interpreting tasks DB querying, message data parsing
    /// </summary>
    public class MsiParser : IDisposable
    {
        bool disposedValue = false;

        /// <summary>
        /// The msi file
        /// </summary>
        protected string msiFile;

        /// <summary>
        /// The msi file database handle.
        /// </summary>
        protected IntPtr db;

        /// <summary>
        /// Opens the specified MSI file and returns the database handle.
        /// </summary>
        /// <param name="msiFile">The msi file.</param>
        /// <returns>Handle to the MSI database.</returns>
        public static IntPtr Open(string msiFile)
        {
            IntPtr db = IntPtr.Zero;
            MsiExtensions.Invoke(() => MsiInterop.MsiOpenDatabase(msiFile, MsiDbPersistMode.ReadOnly, out db));
            return db;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsiParser" /> class.
        /// </summary>
        /// <param name="msiFile">The msi file.</param>
        public MsiParser(string msiFile)
        {
            this.msiFile = msiFile;
            this.db = MsiParser.Open(msiFile);
        }

        /// <summary>
        /// Calls the Dispose method
        /// </summary>
        ~MsiParser()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the Resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Queries the name of the product from the encapsulated MSI database.
        /// <para>
        /// <remarks>The DB view is not closed after the call</remarks>
        /// </para>
        /// </summary>
        /// <returns>Product name.</returns>
        public string GetProductName()
        {
            return GetStringValue("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductName'");
        }

        /// <summary>
        /// Queries the version of the product from the encapsulated MSI database.
        /// <para>
        /// <remarks>The DB view is not closed after the call</remarks>
        /// </para>
        /// </summary>
        /// <returns>Product version.</returns>
        public string GetProductVersion()
        {
            return GetStringValue("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductVersion'");
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetProperty(string name)
        {
            return GetStringValue($"SELECT `Value` FROM `Property` WHERE `Property` = '{name}'");
        }

        /// <summary>
        /// Queries the code of the product from the encapsulated MSI database.
        /// <para>
        /// <remarks>The DB view is not closed after the call</remarks>
        /// </para>
        /// </summary>
        /// <returns>Product code.</returns>
        public string GetProductCode()
        {
            return GetStringValue("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductCode'");
        }

        /// <summary>
        /// Determines whether the specified product code is installed.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <returns>Returns <c>true</c> if the product is installed. Otherwise returns <c>false</c>.</returns>
        public static bool IsInstalled(string productCode)
        {
            StringBuilder sb = new StringBuilder(2048);
            uint size = 2048;
            MsiError err = MsiInterop.MsiGetProductInfo(productCode, "InstallDate", sb, ref size);

            if (err == MsiError.UnknownProduct)
                return false;
            else if (err == MsiError.NoError)
                return true;
            else
                throw new Exception(err.ToString());
        }

        /// <summary>
        /// Determines whether the product from the encapsulated msi file is installed.
        /// </summary>
        /// <returns>Returns <c>true</c> if the product is installed. Otherwise returns <c>false</c>.</returns>
        public bool IsInstalled()
        {
            return IsInstalled(this.GetProductCode());
        }

        ///// <summary>
        ///// Extracts the root components of the top-level install directory from the encapsulated MSI database.
        ///// Typically it is a first child of the 'TARGETDIR' MSI directory.
        ///// <para><remarks>The DB view is not closed after the call</remarks></para>
        ///// </summary>
        ///// <returns>
        ///// Root component of install directory. If the 'TARGETDIR' cannot be located then the return value is the
        ///// expanded value of 'ProgramFilesFolder' WiX constant.
        ///// </returns>
        //public string GetInstallDirectoryRoot()
        //{
        //    while (IntPtr.Zero != (rec = view.NextRecord()))
        //    {
        //        var row = view.GetFieldValues(rec);
        //        data.Add(row);
        //        rec.Close();
        //    }

        //    view.Close();

        //    // Should be 3 if msi has expected content.
        //    //if ((int)qr == 3)
        //    //{
        //    //    string rootDirId = qr.GetString(1);
        //    //    return rootDirId.AsWixVarToPath();
        //    //}
        //    //else
        //    return "ProgramFilesFolder".AsWixVarToPath(); // Always default to Program Files folder.
        //}

        /// <summary>
        /// Returns the full path of the directory entry from the Directory MSI table
        /// </summary>
        /// <param name="name">The name (e.g. INSTALLDIR).</param>
        /// <returns></returns>
        public string GetDirectoryPath(string name)
        {
            string[] subDirs = GetDirectoryPathParts(name).Select(x => x.AsWixVarToPath()).ToArray();
            return string.Join(@"\", subDirs);
        }

        string[] GetDirectoryPathParts(string name)
        {
            var path = new List<string>();
            var names = new Queue<string>(new[] { name });

            while (names.Any())
            {
                var item = names.Dequeue();

                var data = this.db.View("select * from Directory where Directory = '" + item + "'").GetData();
                if (data.Any())
                {
                    var row = data.FirstOrDefault();

                    var subDir = row["DefaultDir"].ToString().Split('|').Last();
                    path.Add(subDir);

                    var parent = (string)row["Directory_Parent"];
                    if (parent != null && parent != "TARGETDIR")
                        names.Enqueue(parent.ToString());
                }
            }
            path.Reverse();
            return path.ToArray();
        }

        /// <summary>
        /// Parses the <c>MsiInstallMessage.CommonData</c> data.
        /// </summary>
        /// <param name="s">Message data.</param>
        /// <returns>Collection of parsed tokens (fields).</returns>
        public static string[] ParseCommonData(string s)
        {
            //Example: 1: 0 2: 1033 3: 1252
            var res = new string[3];
            var regex = new Regex(@"\d:\s?\w+\s");

            int i = 0;

            foreach (Match m in regex.Matches(s))
            {
                if (i > 3) return null;

                res[i++] = m.Value.Substring(m.Value.IndexOf(":") + 1).Trim();
            }

            return res;
        }

        /// <summary>
        /// Parses the <c>MsiInstallMessage.Progress</c> string.
        /// </summary>
        /// <param name="s">Message data.</param>
        /// <returns>Collection of parsed tokens (fields).</returns>
        public static string[] ParseProgressString(string s)
        {
            //1: 0 2: 86 3: 0 4: 1
            var res = new string[4];
            var regex = new Regex(@"\d:\s\d+\s");

            int i = 0;

            foreach (Match m in regex.Matches(s))
            {
                if (i > 4) return null;

                res[i++] = m.Value.Substring(m.Value.IndexOf(":") + 2).Trim();
            }

            return res;
        }

        /// <summary>
        /// Releases the acquired resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (!db.Equals(IntPtr.Zero))
                {
                    var dbr = db;
                    db = IntPtr.Zero;
                    MsiExtensions.Invoke(() => MsiInterop.MsiCloseHandle(dbr));
                }
                disposedValue = true;
            }
        }

        string GetStringValue(string select)
        {
            IntPtr view = IntPtr.Zero;
            try
            {
                view = this.db.View(select);
                return view.NextRecord().GetString(1);
            }
            finally
            {
                if (!view.Equals(IntPtr.Zero))
                {
                    view.Close();
                }
            }
        }
    }
}