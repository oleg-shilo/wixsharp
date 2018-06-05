This sample demonstrates how to ensure that setup runs elevated UI.
Normally MSI elevates the actual installation stage (InstallSequence) while leaving the UI interaction (InstallUISequence) unelevated. This sample shows how to elevate the whole round trip.
Note: the ManagedUI gives you the best user experience as native UI does not allow full control over dialogs (the initial setup exit dialog needs to be closed manually).  
Execute corresponding .cmd file to build desired msi. Then execute the .msi to start the installation.