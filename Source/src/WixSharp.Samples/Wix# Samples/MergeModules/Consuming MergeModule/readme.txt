This sample demonstrates how to build MSI using on MergeModule (MyMergeModule.msm).
MyMergeModule.msm is a built result of the Wix# Samples\MergeModules\Building MergeModule sample.
Execute corresponding .cmd file to build desired msi.
-----------------------------------------------------
Note that WiX syntax allows Merge element to belong to the Product element. However the documentation 
(WiX v3.0.4917.0) explicitly requires it to be nested inside of the Directory element (even if there is no 
neither technical nor logical reason for this). Thus Wix# simply follows the WiX convention.
Also note that WiX does not allow child Condition element in the Merge element.