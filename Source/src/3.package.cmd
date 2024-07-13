echo off

css ..\bin\package.cs
css ..\NuGet\WixSharp\UpdatePackage.cs

cd ..\NuGet\WixSharp
build.WiX4.cmd
cd ..\..\src
pause
