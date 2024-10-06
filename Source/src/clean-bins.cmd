echo off

rem cleaning WPF bin folders (MSBuild fails to do it)

rd /Q /S .\WixSharp.UI.WPF\obj
rd /Q /S .\WixSharp.UI.WPF\bin

pause
