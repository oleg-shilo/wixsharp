This example demonstrates how to distinguish between normale and "silent" installation mode.
The example uses the CustomAction which is to be executed only in "silent" installation mode.

Build msi by executing build.cmd. Then you can test conditional CA execution by launching installing the
sample application in normal (InstallNormal.cmd) and "silent" (InstallSilent.cmd) mode.
	
Note: The third parameter in the WAction constructor is a working directory. It has to be a directory ID.
That is why @"%ProgramFiles%\My Company\My Product" is translated into ID with ToDirID() call.