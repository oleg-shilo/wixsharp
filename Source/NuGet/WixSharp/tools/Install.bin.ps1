param($installPath, $toolsPath, $package, $project)

# $projectFullName = $project.FullName
# $debugString = "install.ps1 executing for " + $projectFullName

# #save the project file first - this commits the changes made by nuget before this     script runs.
# $project.Save()

# #Load the csproj file into an xml object
# $xml = [XML] (gc $project.FullName)

# #grab the namespace from the project element so your xpath works.
# $nsmgr = New-Object System.Xml.XmlNamespaceManager -ArgumentList $xml.NameTable
# $nsmgr.AddNamespace('a',$xml.Project.GetAttribute("xmlns"))

# #link the custom target
# #<UsingTask AssemblyFile="packages\WixSharp.1.0.20.0\build\SetEnvVar.dll" TaskName="SetEnvVar" />
# $node = $xml.Project.SelectSingleNode("//a:UsingTask[@TaskName='SetEnvVar']", $nsmgr)
# if($node -ne $null)
# {
#     $xml.Project.RemoveChild($node)
# }

# $usingTask = $xml.CreateElement("UsingTask", $xml.Project.GetAttribute("xmlns"))

# $attr = $xml.CreateAttribute("AssemblyFile")
# $attr.Value = '$(SolutionDir)'+"packages\"+$package.Id+"."+$package.Version+"\build\SetEnvVar.dll"
# $usingTask.Attributes.Append($attr)

# $attr = $xml.CreateAttribute("TaskName")
# $attr.Value = "SetEnvVar"
# $usingTask.Attributes.Append($attr)

# $xml.Project.AppendChild($usingTask)

# #save the changes.
# $xml.Save($project.FullName)