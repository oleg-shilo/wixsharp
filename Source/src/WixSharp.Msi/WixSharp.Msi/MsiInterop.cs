using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

//This file is an equivalent of the <1% functionality of the original MsiInterop.cs that was removed (after commit#30c275e5fd0a) 
//because of the licensing clash (MIT vs LGPL).
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace WindowsInstaller
{
    public enum MsiError : UInt32
    {
        NoError         = 0,
        NoMoreItems     = 259,
        UnknownProduct  = 1605
    }

    public enum MsiColInfoType : Int32
    {
        Names = 0,
    }

    public enum MsiDbPersistMode
    {
        ReadOnly = 0,
    }

    [Flags]
    public enum MsiInstallLogMode : UInt32
    {
        None            = 0,
        FatalExit       = (1 << (Int32) (MsiInstallMessage.FatalExit >> 24)),
        Error           = (1 << (Int32) (MsiInstallMessage.Error >> 24)),
        Warning         = (1 << (Int32) (MsiInstallMessage.Warning >> 24)),
        User            = (1 << (Int32) (MsiInstallMessage.User >> 24)),
        Info            = (1 << (Int32) (MsiInstallMessage.Info >> 24)),
        ActionStart     = (1 << (Int32) (MsiInstallMessage.ActionStart >> 24)),
        ActionData      = (1 << (Int32) (MsiInstallMessage.ActionData >> 24)),
        CommonData      = (1 << (Int32) (MsiInstallMessage.CommonData >> 24)),
        PropertyDump    = (1 << (Int32) (MsiInstallMessage.Progress >> 24)),
        Progress        = (1 << (Int32) (MsiInstallMessage.Progress >> 24)),
        ShowDialog      = (1 << (Int32) (MsiInstallMessage.ShowDialog >> 24)),
        ExternalUI      = FatalExit | Error | Warning | User | ActionStart | ActionData | CommonData | Progress | ShowDialog
    }

    public enum MsiInstallMessage : Int64
    {
        FatalExit       = 0x00000000,
        Error           = 0x01000000,
        Warning         = 0x02000000,
        User            = 0x03000000,
        Info            = 0x04000000,
        FilesInUse      = 0x05000000,
        ResolveSource   = 0x06000000,
        OutOfDiskSpace  = 0x07000000,
        ActionStart     = 0x08000000,
        ActionData      = 0x09000000,
        Progress        = 0x0a000000,
        CommonData      = 0x0b000000,
        Initialize      = 0x0c000000,
        Terminate       = 0x0d000000,
        ShowDialog      = 0x0e000000,
    }

    public enum MsiInstallUILevel : UInt32
    {
        None            = 2,
        SourceResOnly   = 0x100,
    }

    public enum MsiLogAttribute : Int32
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
        extern static public MsiError MsiOpenDatabase(string path, MsiDbPersistMode mode, out IntPtr handle);

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
        extern static public MsiError MsiViewClose(IntPtr view);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewExecute(IntPtr view, IntPtr record);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public bool MsiRecordIsNull(IntPtr record, UInt32 field);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewFetch(IntPtr view, ref IntPtr record);

        [DllImport("msi", CharSet = CharSet.Auto)]
        extern static public MsiError MsiViewGetColumnInfo(IntPtr view, MsiColInfoType type, out IntPtr record);
    }
}
