This example demonstrates how to specify what UI the setup application should have.
Level of UI controlled by the WProject.UI property which can be one of the following values:
	WixUI_ProgressOnly,
    WixUI_Minimal,
    WixUI_InstallDir,
    WixUI_FeatureTree,
    WixUI_Mondo //full UI 
    	
Execute corresponding .cmd file to build desired msi. After msi is built execute it to test setup application User Interface.