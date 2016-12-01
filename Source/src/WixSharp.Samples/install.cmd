echo off
set scriptFile=SetEV.cs

echo using System;                                                                                                                                          >%scriptFile%
echo using System.IO;                                                                                                                                       >>%scriptFile%
echo using Microsoft.Win32;                                                                                                                                 >>%scriptFile%
echo using System.Runtime.InteropServices;                                                                                                                  >>%scriptFile%
echo class Script                                                                                                                                           >>%scriptFile%
echo {                                                                                                                                                      >>%scriptFile%
echo    static public void Main(string[] args)                                                                                                              >>%scriptFile%
echo    {                                                                                                                                                   >>%scriptFile%
echo        using (var envVars = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment", true))                >>%scriptFile%
echo        {                                                                                                                                               >>%scriptFile%
echo             envVars.SetValue("WIXSHARP_WIXDIR", Path.Combine(Environment.CurrentDirectory, "Wix_bin\\bin"));                                           >>%scriptFile%
echo             envVars.SetValue("WIXSHARP_DIR", Environment.CurrentDirectory);                                                                            >>%scriptFile%
echo             int dwResult = 0;                                                                                                                          >>%scriptFile%
echo             SendMessageTimeout((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE, 0, "Environment", SMTO_ABORTIFHUNG, 5000, dwResult);                          >>%scriptFile%
echo             Console.WriteLine("Environment variable \"WIXSHARP_WIXDIR\" set to "+Path.Combine(Environment.CurrentDirectory, "Wix_bin\\bin"));          >>%scriptFile%
echo        }                                                                                                                                               >>%scriptFile%
echo    }                                                                                                                                                   >>%scriptFile%
echo    [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]                                                                                  >>%scriptFile%
echo    [return: MarshalAs(UnmanagedType.Bool)]                                                                                                             >>%scriptFile%
echo    public static extern bool SendMessageTimeout(IntPtr hWnd, int Msg, int wParam, string lParam, int fuFlags, int uTimeout, int lpdwResult);           >>%scriptFile%
echo    public const int HWND_BROADCAST = 0xffff;                                                                                                           >>%scriptFile%
echo    public const int WM_SETTINGCHANGE = 0x001A;                                                                                                         >>%scriptFile%
echo    public const int SMTO_ABORTIFHUNG = 0x0002;                                                                                                         >>%scriptFile%        
echo }                                                                                                                                                      >>%scriptFile%

cscs %scriptFile%
del %scriptFile%

pause