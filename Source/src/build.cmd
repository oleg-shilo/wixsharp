echo off

set devenv=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\devenv.com
set msbuild=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe
set maniglia=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe

"%maniglia%" /nologo /verbosity:minimal /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU" WixSharp.Suite.sln
REM "%msbuild%" /nologo /verbosity:minimal /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU" WixSharp.Suite.Lab.sln

pause