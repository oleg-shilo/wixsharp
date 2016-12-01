This sample demonstrates how to deploy WebSite. .

Even though Wix# supports WixIISExtension (WiX) with IISVirtualDir(Wix#) class this code sample is demonstrates how to usilize WiX extensions.
Limitations: 
- Deployment of certificates is not supported yet.
- The web site content was originally generated on Win7 CLR2.0 and tested on IIS7. It may or may not work on other environments. The current web.config file is compatible with Win10+IIS10+CLR4.0

Execute corresponding .cmd file to build desired msi. Then execute the .msi to start the installation.