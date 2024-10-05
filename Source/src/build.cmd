echo off

rem cleaning WPF bin folders (MSBuild fails to do it)

rd /Q /S .\WixSharp.UI.WPF\obj
rd /Q /S .\WixSharp.UI.WPF\bin

set EnableNuGetPackageRestore=true
nuget restore WixSharp.Suite.sln

set msbuild=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe

"%msbuild%" /nologo /verbosity:minimal /t:Clean,Restore,Build /p:Configuration=Release /p:Platform="Any CPU" WixSharp.Suite.sln
"%msbuild%" /nologo /verbosity:minimal /t:Clean,Restore,Build /p:Configuration=Release /p:Platform="Any CPU" WixSharp.Suite.Lab.sln
"%msbuild%" /nologo /verbosity:minimal /t:Clean,Restore,Build /p:Configuration=Release /p:Platform="Any CPU" WixSharp.Suite.NET4.5.1.sln


pause