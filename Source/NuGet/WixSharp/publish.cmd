set ver=2.1.6
dotnet nuget push WixSharp.Msi.Core.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp.Core.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json

dotnet nuget push WixSharp.Msi.Core.%ver%.snupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp.Core.%ver%.snupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json

dotnet nuget push WixSharp_wix4.bin.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp_wix4.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp-wix4.WPF.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
pause