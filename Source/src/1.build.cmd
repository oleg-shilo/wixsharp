echo off

rem cleaning WPF bin folders (MSBuild fails to do it)

rd /Q /S .\WixSharp.UI.WPF\obj
rd /Q /S .\WixSharp.UI.WPF\bin

set msbuild=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe

"%msbuild%" /nologo /verbosity:minimal /t:Clean,Restore,Build /p:Configuration=Release /p:Platform="Any CPU" /p:BuildInParallel=true WixSharp.Suite.sln
rem "%msbuild%" /nologo /verbosity:minimal /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU" WixSharp.Suite.Lab.sln

pause