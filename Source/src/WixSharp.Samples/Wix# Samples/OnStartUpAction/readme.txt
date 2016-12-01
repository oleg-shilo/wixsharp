This sample demonstrates how to implement custom action which is executed before any other MSI action.
Execute corresponding Build.cmd file to build both MSIs (main product and prerequisite). 
Then execute main msi (MyProduct.msi) to start the installation.

Note: this approach can be used only in UI mode as InstallUISequence actions will not be dispatched in "no UI" or "basic UI" mode.