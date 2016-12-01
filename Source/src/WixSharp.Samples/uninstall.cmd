echo off
set scriptFile=SetEV.cs

echo using System;                                                                                                                                  >%scriptFile%
echo using Microsoft.Win32;                                                                                                                         >>%scriptFile%
echo class Script                                                                                                                                   >>%scriptFile%
echo {                                                                                                                                              >>%scriptFile%
echo    static public void Main(string[] args)                                                                                                      >>%scriptFile%
echo    {                                                                                                                                           >>%scriptFile%
echo        using (var envVars = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment", true))        >>%scriptFile%
echo        {                                                                                                                                       >>%scriptFile%
echo             envVars.DeleteValue("WIXSHARP_WIXDIR");                                                                                            >>%scriptFile%
echo             envVars.DeleteValue("WIXSHARP_DIR");                                                                                               >>%scriptFile%
echo             Console.WriteLine("Environment variables \"WIXSHARP_DIR\" and \"WIXSHARP_WIXDIR\" have been removed.");                            >>%scriptFile%
echo        }                                                                                                                                       >>%scriptFile%
echo    }                                                                                                                                           >>%scriptFile%
echo }                                                                                                                                              >>%scriptFile%

cscs %scriptFile%
del %scriptFile%
pause