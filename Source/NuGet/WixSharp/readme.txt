Building MSI:
After building the project the corresponding .msi file can be found in the root project folder.

Tips and Hints:
If you are implementing managed CA you may want to set "Target Framework" to "v3.5" as the lower CLR version will help avoid potential conflicts during the installation (e.g. target system has .NET v3.5 only).

Note: 
Wix# requires WiX Toolset (tools and binaries) to function properly. Wix# is capable of automatically finding WiX tools only if WiX Toolset installed. In all other cases you need to set the environment variable WIXSHARP_WIXDIR or WixSharp.Compiler.WixLocation to the valid path to the WiX binaries.

WiX binaries can be brought to the build environment by either installing WiX Toolset, downloading Wix# suite or by adding WixSharp.wix.bin NuGet package to your project.
                                            
Because of the excessive size of the WiX Toolset the WixSharp.wix.bin NuGet package isn't a direct dependency of the WixSharp package and it needs to be added to the project explicitly:

Compiler.WixLocation = @"..\packages\WixSharp.wix.bin.<version>\tools\bin";

Wix# suite contains WIX v3.10 (v3.10.2103.0) as well as the set of samples for all major deployment scenarios. 
