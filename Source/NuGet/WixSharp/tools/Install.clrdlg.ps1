param($installPath, $toolsPath, $package, $project)

$projectFullName = $project.FullName
$debugString = "install.ps1 executing for " + $projectFullName

#save the project file first - this commits the changes made by nuget before this     script runs.
$project.Save()

#Load the csproj file into an xml object
$xml = [XML] (Get-Content $project.FullName)

#grab the namespace from the project element so your xpath works.
$nsmgr = New-Object System.Xml.XmlNamespaceManager -ArgumentList $xml.NameTable
$nsmgr.AddNamespace('a', $xml.Project.GetAttribute("xmlns"))

#link the form designer to the EmptyDialog.cs
$node = $xml.Project.SelectSingleNode("//a:Compile[@Include='EmptyDialog.Designer.cs']", $nsmgr)
$depUpon = $xml.CreateElement("DependentUpon", $xml.Project.GetAttribute("xmlns"))
$depUpon.InnerXml = "EmptyDialog.cs"
$node.AppendChild($depUpon)

#link the form resource to the EmptyDialog.cs
$node = $xml.Project.SelectSingleNode("//a:EmbeddedResource[@Include='EmptyDialog.resx']", $nsmgr)
$depUpon = $xml.CreateElement("DependentUpon", $xml.Project.GetAttribute("xmlns"))
$depUpon.InnerXml = "EmptyDialog.cs"
$node.AppendChild($depUpon)

#delete the bundled program.cs file.
$prog = $xml.Project.SelectSingleNode("//a:Compile[@Include='Program.cs']", $nsmgr)
if ($prog -ne $null) {
    $prog.SelectSingleNode("..").RemoveChild($prog)
}
#save the changes.
$xml.Save($project.FullName)