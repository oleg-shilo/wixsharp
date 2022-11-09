This example shows how to overcome WiX Burn defect that prevents showing MSI UI for the bundled msi files if the UI is not native UI but an embedded UI (WixSharp ManagedUI) provided by user.

You can simply wrap such msi in the self-sufficient executable (self-executable msi) that at runtime launches msi against msiexec.exe with the appropriate runtime context so it can display its own UI regardless if it is native MSI UI or managed embedded UI. 

This sample takes already prepared self-executable msi from the ..\..\..\..\Managed Setup\Self-executable_Msi sample

Note, hello.cs/hello.exe is provided so you can experiment with ExePackages. It is very useful if you want to troubleshoot DetectConditions as it simply displays InstallCommand and UninstallCommand parameters passed into it at runtime.