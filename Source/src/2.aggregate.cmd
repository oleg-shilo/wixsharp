echo off

echo Aggregating all artifacts for the release package. Not all steps may succeed on all PCs (e.g. building chm)

rd /S /Q .\..\bin\WixSharp
md "..\bin\WixSharp"
md "..\bin\WixSharp\Wix_bin"
xcopy /S /Y "WixSharp.Samples\Wix# Samples" "..\bin\WixSharp\Samples\"
copy /Y "WixSharp.Samples\Wix_bin\*" "..\bin\WixSharp\Wix_bin\"
copy /Y "..\license.txt" "..\bin\WixSharp\license.txt"
copy /Y "..\readme.txt" "..\bin\WixSharp\readme.txt"
copy /Y "WixSharp.Samples\*.exe" "..\bin\WixSharp\"
copy /Y "WixSharp.Samples\WixSharp*.dll" "..\bin\WixSharp\"
copy /Y "WixSharp.Samples\WixSharp*.xml" "..\bin\WixSharp\"

.\WixSharp.Samples\cscs.exe -l .\..\bin\clean_bins.cs

set msbuild=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe

"%msbuild%" /nologo /verbosity:minimal /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU" "Docs\WixSharp.Docs.sln"

copy /Y "Docs\Build\WixSharp.Reference.chm" "..\bin\WixSharp\WixSharp.Reference.chm"

pause
