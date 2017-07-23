using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using io = System.IO;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using sys = System.Windows.Forms;
using System.Drawing.Imaging;

using System.Windows.Forms;

using WixSharp.UI.Forms;
using System.Threading;

namespace WixSharp
{
    //public class ClrDialogs
    //{
    //    static Type WelcomeDialog = typeof(WelcomeDialog);
    //    static Type LicenceDialog = typeof(LicenceDialog);
    //    static Type FeaturesDialog = typeof(FeaturesDialog);
    //    static Type InstallDirDialog = typeof(InstallDirDialog);
    //    static Type ExitDialog = typeof(ExitDialog);

    //    static Type RepairStartDialog = typeof(RepairStartDialog);
    //    static Type RepairExitDialog = typeof(RepairExitDialog);

    //    static Type ProgressDialog = typeof(ProgressDialog);
    //}
#pragma warning disable 1591

    public static class UIExtensions
    {
        public static System.Drawing.Icon GetAssiciatedIcon(this string extension)
        {
            var dummy = Path.GetTempPath() + extension;
            System.IO.File.WriteAllText(dummy, "");
            var result = System.Drawing.Icon.ExtractAssociatedIcon(dummy);
            System.IO.File.Delete(dummy);
            return result;
        }

        internal static sys.Control ForceAutoScale(this sys.Control control)
        {
            var graphics = control.CreateGraphics();
            float scalingFactor = graphics.DpiY / 96; //96 DPI corresponds to 100% scaling
            control.Scale(new SizeF(scalingFactor, scalingFactor));
            return control;
        }

        internal static float GetCurrentScaling(this sys.Control control)
        {
            var graphics = control.CreateGraphics();
            float scalingFactor = graphics.DpiY / 96; //96 DPI corresponds to 100% scaling
            return scalingFactor;
        }

        public static sys.Control CustomScale(this sys.Control control, float scalingFactor)
        {
            control.Scale(new SizeF(scalingFactor, scalingFactor));
            return control;
        }

        public static bool IsReadOnly(this TreeNode node)
        {
            return node is ReadOnlyTreeNode r_node && r_node.IsReadOnly;
        }

        public static bool IsModified(this TreeNode node)
        {
            return node is ReadOnlyTreeNode r_node && r_node.Checked != r_node.DefaultChecked;
        }

        public static void ResetCheckedToDefault(this TreeNode node, int delay = -1)
        {
            if (node is ReadOnlyTreeNode _node && _node.IsReadOnly)
            {
                if (delay == -1)
                    node.Checked = _node.DefaultChecked;
                else
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        Thread.Sleep(delay);

                        node.TreeView.BeginInvoke((MethodInvoker)delegate
                        {
                            node.Checked = _node.DefaultChecked;
                        });
                    });
            }
        }

        public static sys.Control ClearChildren(this sys.Control control)
        {
            foreach (sys.Control item in control.Controls)
                item.Dispose();

            control.Controls.Clear();
            return control;
        }

#pragma warning restore 1591

        //NOT RELIABLE
        //The detection of the installdir is not deterministic. For example 'Shortcuts' sample has
        //three logical installdirs INSTALLDIR, DesktopFolder and ProgramMenuFolder. The INSTALLDIR
        //is the real one that we need to discover but there is no way to understand its role by analyzing
        //the MSI tables. And the other problem is that we cannot rely on its name as user can overwrite it.
        //WIX solves this problem by requiring the user explicitly link the installdir ID to the WIXUI_INSTALLDIR
        //property: <Property Id="WIXUI_INSTALLDIR" Value="INSTALLDIR"  />.
        //public static string GetInstallDirectoryName(this Session session)
        //{
        //    List<Dictionary<string, object>> result = session.OpenView("select * from Component");
        //
        //    var dirs = result.Select(x => x["Directory_"]).Cast<string>().Distinct().ToArray();
        //
        //    string shortestDir = dirs.Select(x => new { Name = x, Parts = session.GetDirectoryPathParts(x) })
        //                             .OrderBy(x => x.Parts.Length)
        //                             .Select(x => x.Name)
        //                             .FirstOrDefault();
        //    if (shortestDir == null)
        //        throw new Exception("GetInstallDirectoryPath Error: cannot find InstallDirectory");
        //    else
        //        return shortestDir;
        //}

        /// <summary>
        /// Gets the target system directory path based on specified directory name (MSI Directory table).
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetDirectoryPath(this Session session, string name)
        {
            string[] subDirs = session.GetDirectoryPathParts(name)
                                        .Select(x => x.AsWixVarToPath())
                                        .ToArray();
            return string.Join(@"\", subDirs);
        }

        static string[] GetDirectoryPathParts(this Session session, string name)
        {
            var path = new List<string>();
            var names = new Queue<string>(new[] { name });

            while (names.Any())
            {
                var item = names.Dequeue();

                using (var sql = session.Database.OpenView("select * from Directory where Directory = '" + item + "'"))
                {
                    sql.Execute();
                    using (var record = sql.Fetch())
                    {
                        if (record != null)
                        {
                            var subDir = record.GetString("DefaultDir").Split('|').Last();
                            path.Add(subDir);

                            if (!record.IsNull("Directory_Parent"))
                            {
                                var parent = record.GetString("Directory_Parent");
                                if (parent != "TARGETDIR")
                                    names.Enqueue(parent);
                            }
                        }
                    }
                }
            }
            path.Reverse();
            return path.ToArray();
        }

        internal static string UserOrDefaultContentOf(string extenalFilePath, string srcDir, string outDir, string fileName, object defaultContent)
        {
            string extenalFile = Utils.PathCombine(srcDir, extenalFilePath);

            if (extenalFilePath.IsNotEmpty()) //important to test before PathComibed
                return extenalFile;

            var file = Path.Combine(outDir, fileName);

            if (defaultContent is byte[])
                io.File.WriteAllBytes(file, (byte[])defaultContent);
            else if (defaultContent is Bitmap)
                ((Bitmap)defaultContent).Save(file, ImageFormat.Png);
            else if (defaultContent is string)
                io.File.WriteAllBytes(file, ((string)defaultContent).GetBytes());
            else if (defaultContent == null)
                return "<null>";
            else
                throw new Exception("Unsupported ManagedUI resource type.");

            Compiler.TempFiles.Add(file);
            return file;
        }

        /// <summary>
        /// Localizes the control its contained <see cref="T:System.Windows.Forms.Control.Text"/> from the specified localization
        /// delegate 'localize'.
        /// <para>The method substitutes both localization file (*.wxl) entries and MSI properties contained by the input string
        /// with their translated/converted values.</para>
        /// <remarks>
        /// Note that both localization entries and MSI properties must be enclosed in the square brackets
        /// (e.g. "[ProductName] Setup", "[InstallDirDlg_Title]").
        /// </remarks>
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="localize">The localize.</param>
        /// <returns></returns>
        public static sys.Control LocalizeWith(this sys.Control control, Func<string, string> localize)
        {
            var controls = new Queue<sys.Control>(new[] { control });

            while (controls.Any())
            {
                var item = controls.Dequeue();

                item.Text = item.Text.LocalizeWith(localize);

                item.Controls
                .OfType<sys.Control>()
                .ForEach(x => controls.Enqueue(x));
            }
            return control;
        }

        static Regex locRegex = new Regex(@"\[.+?\]");
        static Regex cleanRegex = new Regex(@"{\\(.*?)}"); //removes font info "{\WixUI_Font_Bigger}Welcome to the [ProductName] Setup Wizard"

        /// <summary>
        /// Localizes the string from the specified localization delegate 'localize'.
        /// <para>The method substitutes both localization file (*.wxl) entries and MSI properties contained by the input string
        /// with their translated/converted values.</para>
        /// <remarks>
        /// Note that both localization entries and MSI properties must be enclosed in the square brackets
        /// (e.g. "[ProductName] Setup", "[InstallDirDlg_Title]").
        /// </remarks>
        /// </summary>
        /// <param name="textToLocalize">The text to localize.</param>
        /// <param name="localize">The localize.</param>
        /// <returns></returns>
        public static string LocalizeWith(this string textToLocalize, Func<string, string> localize)
        {
            if (textToLocalize.IsEmpty()) return textToLocalize;

            var result = new StringBuilder(textToLocalize);

            //first rum will replace all UI constants, which in turn may contain MSI properties to resolve.
            //second run will resolve properties if any found.
            for (int i = 0; i < 2; i++)
            {
                string text = result.ToString();
                result.Length = 0; //clear

                int lastEnd = 0;
                foreach (Match match in locRegex.Matches(text))
                {
                    result.Append(text.Substring(lastEnd, match.Index - lastEnd));
                    lastEnd = match.Index + match.Length;

                    string key = match.Value.Trim('[', ']');

                    result.Append(localize(key));
                }

                if (lastEnd != text.Length)
                    result.Append(text.Substring(lastEnd, text.Length - lastEnd));
            }
            return cleanRegex.Replace(result.ToString(), "");
        }
    }
}