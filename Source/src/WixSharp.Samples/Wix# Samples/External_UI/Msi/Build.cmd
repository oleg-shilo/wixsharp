echo off 
echo.
echo **********************************************************
echo *                                                        *
echo *  PLEASE ENSURE WIXSHARP_WIXDIR EnvVar IS PROPERLY SET  *
echo *                                                        *
echo **********************************************************
echo.
rem set WIXSHARP_WIXDIR=e:\Wix\bin
..\..\..\cscs.exe setup.cs
pause
