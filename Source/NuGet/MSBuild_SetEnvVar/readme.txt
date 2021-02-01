<Project>
 
   <UsingTask AssemblyFile="C:\Users\osh\Documents\Visual Studio 2013\Projects\ClassLibrary1\SetEnvVar.dll" TaskName="SetEnvVar" />

   <Target Name="AfterBuild">
       <SetEnvVar Values="PROJECTNAME=$(ProjectName)"/>
       <Exec Command="&quot;C:\Users\osh\Documents\Visual Studio 2013\Projects\ClassLibrary1\bin\Debug\script.exe&quot;" />
   </Target>
    
</Project>