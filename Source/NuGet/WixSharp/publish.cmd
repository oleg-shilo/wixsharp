dotnet nuget push WixSharp.Msi.Core.2.1.2.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp.Core.2.1.2.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json

dotnet nuget push WixSharp.Msi.Core.2.1.2.snupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp.Core.2.1.2.snupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json

dotnet nuget push WixSharp_wix4.bin.2.1.2.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp_wix4.2.1.2.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp-wix4.WPF.2.1.2.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
pause