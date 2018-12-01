echo off

set CSSCRIPT_DIR=..\..\
set WIXSHARP_DIR=..\..\Wix_bin\\bin
set PATH=%CSSCRIPT_DIR%;%WIXSHARP_DIR%;%PATH%

cscs.exe setup.cs

pause
