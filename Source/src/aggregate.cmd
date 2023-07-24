echo off

echo Aggregating all artifacts for the release package. Not all steps may succeed on all PCs (e.g. building chm)

xcopy /S /Y "WixSharp.Samples\Wix_bin" "..\bin\WixSharp\Wix_bin\"
xcopy /S /Y "WixSharp.Samples\Wix# Samples" "..\bin\WixSharp\Samples\"
copy /Y "WixSharp.Samples\uninstall.cmd" "..\bin\WixSharp\uninstall.cmd"
copy /Y "WixSharp.Samples\install.cmd" "..\bin\WixSharp\install.cmd"
copy /Y "WixSharp.Samples\cscs.exe" "..\bin\WixSharp\cscs.exe"
copy /Y "WixSharp.Samples\nbsbuilder.exe" "..\bin\WixSharp\nbsbuilder.exe"
copy /Y "..\license.txt" "..\bin\WixSharp\license.txt"
copy /Y "..\readme.txt" "..\bin\WixSharp\readme.txt"
copy /Y "WixSharp.Samples\WixSharp.Lab.dll" "..\bin\WixSharp\WixSharp.Lab.dll"
copy /Y "WixSharp.Samples\WixSharp.Lab.xml" "..\bin\WixSharp\WixSharp.Lab.xml"

set msbuild=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe

"%msbuild%" /nologo /verbosity:minimal /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU" "Docs\WixSharp.Docs.sln"

copy /Y "Docs\Build\WixSharp.Reference.chm" "..\bin\WixSharp\WixSharp.Reference.chm"

pause
