using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

// This file is an equivalent of the <1% functionality of the original MsiInterop.cs
// (https://github.com/jkuemerle/MsiInterop). MsiInterop.cs was removed (after commit#30c275e5fd0a)
// because of the licensing clash (MIT vs LGPL).

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace WindowsInstaller
{
    public enum MsiError : UInt32
    {
        NoError = 0,
        NoMoreItems = 259,
        UnknownProduct = 1605
    }

    internal enum SummaryInformationStreamProperty : int
    {
        /// <summary>Codepage</summary>
        Codepage = 1,

        /// <summary>Title</summary>
        Title = 2,

        /// <summary>Subject</summary>
        Subject = 3,

        /// <summary>Author</summary>
        Author = 4,

        /// <summary>Keywords</summary>
        Keywords = 5,

        /// <summary>Comments</summary>
        Comments = 6,

        /// <summary>Template</summary>
        Template = 7,

        /// <summary>LastSavedBy</summary>
        LastSavedBy = 8,

        /// <summary>RevisionNumber</summary>
        RevisionNumber = 9,

        /// <summary>LastPrinted</summary>
        LastPrinted = 11,

        /// <summary>CreateTime</summary>
        CreateTime = 12,

        /// <summary>LastSaveTime</summary>
        LastSaveTime = 13,

        /// <summary>PageCount</summary>
        PageCount = 14,

        /// <summary>WordCount</summary>
        WordCount = 15,

        /// <summary>CharacterCount</summary>
        CharacterCount = 16,

        /// <summary>CreatingApplication</summary>
        CreatingApplication = 18,

        /// <summary>Security</summary>
        Security = 19,
    }

    public enum MsiColInfoType
    {
        Names = 0,
    }

    public enum MsiDbPersistMode
    {
        ReadOnly = 0,
        ReadWrite = 1,
    }

    public enum MsiModifyMode
    {
        ModifyAssign = 3
    }

    [Flags]
    public enum MsiInstallLogMode : UInt32
    {
        None = 0,
        FatalExit = (1 << (Int32)(MsiInstallMessage.FatalExit >> 24)),
        Error = (1 << (Int32)(MsiInstallMessage.Error >> 24)),
        Warning = (1 << (Int32)(MsiInstallMessage.Warning >> 24)),
        User = (1 << (Int32)(MsiInstallMessage.User >> 24)),
        Info = (1 << (Int32)(MsiInstallMessage.Info >> 24)),
        ActionStart = (1 << (Int32)(MsiInstallMessage.ActionStart >> 24)),
        ActionData = (1 << (Int32)(MsiInstallMessage.ActionData >> 24)),
        CommonData = (1 << (Int32)(MsiInstallMessage.CommonData >> 24)),
        PropertyDump = (1 << (Int32)(MsiInstallMessage.Progress >> 24)),
        Progress = (1 << (Int32)(MsiInstallMessage.Progress >> 24)),
        ShowDialog = (1 << (Int32)(MsiInstallMessage.ShowDialog >> 24)),
        ExternalUI = FatalExit | Error | Warning | User | ActionStart | ActionData | CommonData | Progress | ShowDialog
    }

    public enum MsiInstallMessage : Int64
    {
        FatalExit = 0x00000000,
        Error = 0x01000000,
        Warning = 0x02000000,
        User = 0x03000000,
        Info = 0x04000000,
        FilesInUse = 0x05000000,
        ResolveSource = 0x06000000,
        OutOfDiskSpace = 0x07000000,
        ActionStart = 0x08000000,
        ActionData = 0x09000000,
        Progress = 0x0a000000,
        CommonData = 0x0b000000,
        Initialize = 0x0c000000,
        Terminate = 0x0d000000,
        ShowDialog = 0x0e000000,
    }

    [Flags]
    public enum MsiInstallUILevel : UInt32
    {
        None = 2,
        SourceResOnly = 0x100,
    }

    public enum MsiLogAttribute
    {
        FlushEachLine = (1 << 1),
    }

    public delegate Int32 MsiInstallUIHandler(IntPtr context, UInt32 messageType, [MarshalAs(UnmanagedType.LPTStr)] string message);

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    sealed public class MsiInterop
    {
        public const UInt32 MessageTypeMask = 0xff000000;
        public const Int32 MsiNullInteger = Int32.MinValue;

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiEnableLog(MsiInstallLogMode mode, string file, MsiLogAttribute attributes);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetProductInfo(string product, string property, StringBuilder value, ref UInt32 valueSize);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiInstallProduct(string product, string commandLine);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiOpenProduct(string product, out IntPtr handle);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiInstallUIHandler MsiSetExternalUI([MarshalAs(UnmanagedType.FunctionPtr)] MsiInstallUIHandler handler, MsiInstallLogMode filter, IntPtr context);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseOpenView(IntPtr database, string query, out IntPtr view);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public IntPtr MsiCreateRecord(uint numOfFields);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiOpenDatabase(string path, MsiDbPersistMode mode, out IntPtr handle);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiDatabaseCommit(IntPtr database);

        [DllImport("msi")]
        extern static public MsiError MsiCloseHandle(IntPtr handle);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiInstallUILevel MsiSetInternalUI(MsiInstallUILevel level, ref IntPtr parentWnd);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public UInt32 MsiRecordGetFieldCount(IntPtr record);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public Int32 MsiRecordGetInteger(IntPtr record, UInt32 field);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordGetString(IntPtr record, UInt32 field, StringBuilder value, ref UInt32 valueSize);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordSetString(IntPtr record, uint field, string value);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewModify(IntPtr view, MsiModifyMode modifyMode, IntPtr record);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiRecordSetStream(IntPtr record, uint field, string filePath);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewClose(IntPtr view);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewExecute(IntPtr view, IntPtr record);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public bool MsiRecordIsNull(IntPtr record, UInt32 field);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewFetch(IntPtr view, ref IntPtr record);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewGetColumnInfo(IntPtr view, MsiColInfoType type, out IntPtr record);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiGetProperty(IntPtr install, string name, StringBuilder value, ref uint valueSize);
    }
}