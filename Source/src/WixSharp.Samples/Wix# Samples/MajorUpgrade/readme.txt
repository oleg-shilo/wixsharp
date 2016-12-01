This sample demonstrates how to implement MajorUpgrade deployment scenario.

* Implementing with MajorUpgrade element (recommended)
It is the current WiX technique, which has been introduced in WiX v3.5 to replace more complicated Upgrade element.
Execute build.cmd file to build setup.msi files. 
* Implementing with Upgrade element
It is an older technique, which has been superseded by simplified MajorUpgrade element in WiX v3.5.
Execute build.cmd file to build msi files (setup.1.msi and setup.2.msi) for two versions of the same product. 

Then execute the setup.1.msi to start the installation of the version 1.0.0. Aftere that Then execute the setup.2.msi. Note installation of version 2.0.0 will automatically uninstall version 1.0.0