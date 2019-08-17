WiX - WixSharp (C# interface to WiX toolset)
--------------------------------------------------------------
System Requirements:
   * .NET runtime v4.0 or higher must be installed.
   * The current version of WixSharp require WIX (Windows Installer XML).
     The corresponding WIX binaries are already included in WixSharp thus you may but don't have to install WIX.
   * WixSharp requires CS-Script (C# Script).
     The corresponding CS-Script binaries are already included in WixSharp thus you may but don't have to install CS-Script.

 To install:
   1. extract content of the WixSharp.zip
   2. set environment variable WIXSHARP_DIR to the location of the extracted content (folder containing WixSharp.dll)

 To uninstall:
   1. remove environment variable WIXSHARP_DIR manually

 Alternatively you can use install.cmd/uninstall.cmd
 Note: strictly speaking setting environment variables is optional. WIXSHARP_DIR is only needed for building
 WixSharp\Samples\Bootstrapper\NativeBootstrapper sample. And WIXSHARP_WIXDIR only needed if WixSharp compiler
 cannot locate WiX binaries.