This sample demonstrates how to modify UI elements (e.g. buttons) captions.

Almost any UI element caption can be changed/overwritten by using Wix Localization mechanism. In this code sample en-US is used as a UI language and wixui.wxl as a "text" overwriting instruction file. 

wixui.wxl content indicates for Wix compiled that the captions on the following buttons should be changed:
- "Next" should be replaced with "Next Step" 
- "Back" should be replaced with "Step Back" 
- "Cancel" should be replaced with "Cancel Setup" 
Execute corresponding .cmd file to build desired msi. After msi is built execute it and see the effect of UI customization.