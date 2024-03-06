@echo OFF 
IF EXIST "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat" (
  call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
) ELSE (
  call "C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\Tools\VsDevCmd.bat"
)
echo "Starting Build for all Projects "
MSBuild.exe /nologo /verbosity:minimal /t:Clean,Restore,Build /p:Configuration=Release /p:Platform="Any CPU" ".\Setup.UI\Setup.UI.csproj"
MSBuild.exe /nologo /verbosity:minimal /t:Clean,Restore,Build /p:Configuration=Release /p:Platform="Any CPU" ".\Setup\Setup.csproj"
echo "All builds completed." 
pause