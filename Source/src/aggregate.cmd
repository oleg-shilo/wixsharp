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

REM "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /p:Configuration=Release "Docs\WixSharp.Docs.shfbproj"

copy /Y "Docs\Build\WixSharp.Reference.chm" "..\bin\WixSharp\WixSharp.Reference.chm"
REM del "..\bin\WixSharp\WixSharp.Reference.chw"

pause
