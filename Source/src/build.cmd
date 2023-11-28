echo off

set msbuild=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe

"%msbuild%" /nologo /verbosity:minimal /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU" WixSharp.Suite.sln
"%msbuild%" /nologo /verbosity:minimal /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU" WixSharp.Suite.Lab.sln

pause