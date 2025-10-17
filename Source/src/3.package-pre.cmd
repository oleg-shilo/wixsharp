echo off

css ..\bin\package.cs
css ..\NuGet\WixSharp\UpdatePackage.cs -pre

cd ..\NuGet\WixSharp
build.cmd
cd ..\..\src
pause
