set ver=2.12.2
echo !!!! Note: symbols are pushed automatically with nupkg 

set nugetkey=%nuget_key%
dotnet nuget push WixSharp.Msi.Core.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp.Core.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json

dotnet nuget push WixSharp_wix4.bin.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp_wix4.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
dotnet nuget push WixSharp-wix4.WPF.%ver%.nupkg -k %nugetkey% -s https://api.nuget.org/v3/index.json
