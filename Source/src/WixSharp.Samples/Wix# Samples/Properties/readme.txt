This example demonstrates how work with MSI properties.  
You can analyse properties values during setup with Session instance or by executing SQL 
commands against installation database. 
    	
Execute corresponding .cmd file to build desired msi. After msi is built execute it:
 - With InstallNormal.cmd. To test reading properties default values.
 - With InstallPropertyModified.cmd. To test stetting property value from command line and reading it back
   during the installation. 
   Note: You need to modify InstallPropertyModified.cmd before execution to point to existing text file 
   instead of C:\test.txt. Also note that that public properties must have capitalized names (MSI naming convention).