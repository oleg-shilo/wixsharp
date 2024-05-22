using System;
using System.IO;
using System.Runtime.InteropServices;
using IO = System.IO;

namespace WixSharp.Utilities
{
    /// <summary>
    /// Provides functionality to verify the digital signature of files.
    /// </summary>
    public static class VerifyFileSignature
    {
        /// <summary>
        /// Checks if the specified file is signed.
        /// </summary>
        /// <param name="filePath">The path to the file to check.</param>
        /// <returns>True if the file is signed; otherwise, false.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the file path is null or the file does not exist.</exception>
        public static bool IsSigned(string filePath)
        {
            if (filePath == null)
            {
                throw new FileNotFoundException("File path cannot be null.", nameof(filePath));
            }

            if (!IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            var file = new WINTRUST_FILE_INFO
            {
                cbStruct = Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)),
                pcwszFilePath = filePath
            };

            var data = new WINTRUST_DATA
            {
                cbStruct = Marshal.SizeOf(typeof(WINTRUST_DATA)),
                dwUIChoice = WTD_UI_NONE,
                dwUnionChoice = WTD_CHOICE_FILE,
                fdwRevocationChecks = WTD_REVOKE_NONE,
                pFile = Marshal.AllocHGlobal(file.cbStruct)
            };
            Marshal.StructureToPtr(file, data.pFile, false);

            int hr;
            try
            {
                hr = WinVerifyTrust(INVALID_HANDLE_VALUE, WINTRUST_ACTION_GENERIC_VERIFY_V2, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(data.pFile);
            }
            return hr == 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WINTRUST_FILE_INFO
        {
            public int cbStruct;
            public string pcwszFilePath;
            public IntPtr hFile;
            public IntPtr pgKnownSubject;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINTRUST_DATA
        {
            public int cbStruct;
            public IntPtr pPolicyCallbackData;
            public IntPtr pSIPClientData;
            public int dwUIChoice;
            public int fdwRevocationChecks;
            public int dwUnionChoice;
            public IntPtr pFile;
            public int dwStateAction;
            public IntPtr hWVTStateData;
            public IntPtr pwszURLReference;
            public int dwProvFlags;
            public int dwUIContext;
            public IntPtr pSignatureSettings;
        }

        private const int WTD_UI_NONE = 2;
        private const int WTD_REVOKE_NONE = 0;
        private const int WTD_CHOICE_FILE = 1;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private static readonly Guid WINTRUST_ACTION_GENERIC_VERIFY_V2 = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");

        [DllImport("wintrust.dll")]
        private static extern int WinVerifyTrust(IntPtr hwnd, [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID, ref WINTRUST_DATA pWVTData);
    }
}
