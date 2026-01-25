using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WixSharp
{
    //TODO: Should be replaced with the forked fix: https://github.com/Profy/ShellFileDialogs

    /// <summary>
    /// A custom UI class for showing the new style folder selection dialog.
    /// </summary>
    public class OpenFolderDialog
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string path,
                                                              IntPtr pbc,
                                                              ref Guid riid,
                                                              out IShellItem ppv);

        private static Guid IID_IShellItem = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        /// <summary>
        /// Shows the folder selection dialog. This method is a wrapper around Windows Shell Folder Picker dialog, which uses modern style UI.
        /// </summary>
        /// <param name="initialFolderPath">The initial folder path.</param>
        /// <returns></returns>
        public static (bool? isSelected, string selectedPath) Select(string initialFolderPath = null)
        {
            var selection = ShellFileDialogs.FolderBrowserDialog.ShowDialog(GetActiveWindow(), "Select Folder", initialFolderPath);

            return (selection != null, selection);
        }

        /// <summary>
        /// Shows the folder selection dialog. This method is a wrapper around Windows Shell Folder Picker dialog, which uses modern style UI.
        /// </summary>
        /// <param name="initialFolderPath">The initial folder path.</param>
        /// <returns></returns>
        public static (bool isSelected, string selectedPath) Select_min(string initialFolderPath = null)
        {
            FileOpenDialog d;

            var dialog = (IFileOpenDialog)(d = new FileOpenDialog());
            dialog.SetOptions(FOS.FOS_PICKFOLDERS | FOS.FOS_FORCEFILESYSTEM); // Folder picker with modern styling

            // it will throw com exception if user cancels
            try
            {
                if (initialFolderPath.IsNotEmpty())
                {
                    IShellItem folderItem;
                    int hr = SHCreateItemFromParsingName(Environment.CurrentDirectory, IntPtr.Zero, ref IID_IShellItem, out folderItem);

                    dialog.SetFolder(folderItem);
                }

                IntPtr parent = GetActiveWindow();
                dialog.Show(parent);

                IntPtr hwndDialog = IntPtr.Zero;

                dialog.GetResult(out IShellItem selectedItem);

                // throw invalid memory access exception if user cancels

                selectedItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out IntPtr pszString);
                string folderPath = Marshal.PtrToStringAuto(pszString);
                Marshal.FreeCoTaskMem(pszString);
                return (true, folderPath);
            }
            catch { }

            return (false, null);
        }
    }

    [ComImport]
    [Guid("d57c7288-d4ad-4768-be02-9d969532d960")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IFileOpenDialog
    {
        void Show([In] IntPtr hwndParent);

        void SetFileTypes(); // Not used, can be empty

        void SetFileTypeIndex(int iFileType);

        void GetFileTypeIndex(out int piFileType);

        void Advise(); // Not used

        void Unadvise();

        void SetOptions([In] FOS fos);

        void GetOptions(out FOS fos);

        void SetDefaultFolder(IShellItem psi);

        void SetFolder(IShellItem psi);

        void GetFolder(out IShellItem ppsi);

        void GetCurrentSelection(out IShellItem ppsi);

        void SetFileName(string pszName);

        void GetFileName(out string pszName);

        void SetTitle(string pszTitle);

        void SetOkButtonLabel(string pszText);

        void SetFileNameLabel(string pszLabel);

        void GetResult(out IShellItem ppsi);

        void AddPlace(IShellItem psi, int alignment);

        void SetDefaultExtension(string pszDefaultExtension);

        void Close(int hr);

        void SetClientGuid();

        void ClearClientData();

        void SetFilter([MarshalAs(UnmanagedType.IUnknown)] object pFilter);

        void GetResults(out object ppenum);

        void GetSelectedItems(out object ppsai);
    }

    [ComImport]
    [Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
    class FileOpenDialog
    {
        // public int GetWindow(out IntPtr phwnd);
    }

    [ComImport]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IShellItem
    {
        void GetDisplayName(SIGDN sigdnName, out IntPtr ppszName);

        void BindToHandler();

        void GetParent();

        void GetAttributes();

        void Compare();
    }

    enum SIGDN : uint
    {
        SIGDN_FILESYSPATH = 0x80058000,
        SIGDN_URL = 0x80068000
    }

    [Flags]
    enum FOS : uint
    {
        FOS_OVERWRITEPROMPT = 0x2,
        FOS_STRICTFILETYPES = 0x4,
        FOS_NOCHANGEDIR = 0x8,
        FOS_PICKFOLDERS = 0x20,
        FOS_FORCEFILESYSTEM = 0x40,
        FOS_ALLNONSTORAGEITEMS = 0x80,
        FOS_NOVALIDATE = 0x100,
        FOS_ALLOWMULTISELECT = 0x200,
        FOS_PATHMUSTEXIST = 0x800,
        FOS_FILEMUSTEXIST = 0x1000,
        FOS_CREATEPROMPT = 0x2000,
        FOS_SHAREAWARE = 0x4000,
        FOS_NOREADONLYRETURN = 0x8000,
        FOS_NOTESTFILECREATE = 0x10000,
        FOS_HIDEMRUPLACES = 0x20000,
        FOS_HIDEPINNEDPLACES = 0x40000,
        FOS_NODEREFERENCELINKS = 0x100000,
        FOS_OKBUTTONNEEDSINTERACTION = 0x2000000,
        FOS_DONTADDTORECENT = 0x20000000,
        FOS_FORCESHOWHIDDEN = 0x10000000,
        FOS_DEFAULTNOMINIMODE = 0x20000000,
        FOS_FORCEPREVIEWPANEON = 0x40000000
    }
}