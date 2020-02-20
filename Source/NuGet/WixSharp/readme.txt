Do not use in production!
Only tests!

Building MSI:
After building the project the corresponding .msi file can be found in the root project folder.

Note: 
Wix# requires WiX Toolset (tools and binaries) to function properly. Wix# is capable of automatically finding WiX tools only if WiX Toolset installed. In all other cases you need to set the environment variable WIXSHARP_WIXDIR or WixSharp.Compiler.WixLocation to the valid path to the WiX binaries.

WiX binaries can be brought to the build environment by either installing WiX Toolset, downloading Wix# suite or by adding WixSharp.wix.bin NuGet package to your project.
                                            
Because of the excessive size of the WiX Toolset the WixSharp.wix.bin NuGet package isn't a direct dependency of the WixSharp package and it needs to be added to the project explicitly:

Compiler.WixLocation = @"..\packages\WixSharp.wix.bin.<version>\bin";

Wix# suite contains WIX v4.0 (v4.0.0.5918) as well as the set of samples for all major deployment scenarios. 