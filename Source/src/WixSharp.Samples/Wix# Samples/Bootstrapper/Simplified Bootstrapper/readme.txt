This sample demonstrates how to implement simplified Bootstrapper.
Execute corresponding Build.cmd file to build both MSIs (main product and prerequisite). 
Then execute main msi (MyProduct.msi) to start the installation.
It will install "Fake CRT" at start of UI sequence if it is not already installed. 

Note: this approach can be used only in UI mode as InstallUISequence actions (e.g. installing "Fake CRT") will not be dispatched in "no UI" or "basic UI" mode.