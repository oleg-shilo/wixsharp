This example demonstrates how launch the application at the end of installation.
Instead of using Custom Action (CA) for this you can use Managed CA. In this case you have a bit more 
control on how your application should be started. For example System.Diagnostics.Process.Start()allows starting 
"well-known" applications (e.g.) without specifying full path.
    	
Execute corresponding .cmd file to build desired msi. After msi is built execute it. Notepad should start at the end 
of the installation and it should remain open after the installation is completed.