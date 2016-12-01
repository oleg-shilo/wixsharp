//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Controls;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.CustomUI = new DialogSequence()
                                   .On(NativeDialogs.WelcomeDlg, Buttons.Next, new ShowDialog(NativeDialogs.InstallDirDlg))
                                   .On(NativeDialogs.InstallDirDlg, Buttons.Back, new ShowDialog(NativeDialogs.WelcomeDlg));

        //or
        //Compiler.WixSourceGenerated += Compiler_WixSourceGenerated; 
        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    static void Compiler_WixSourceGenerated(XDocument document)
    {
        document.Root.Select("Product")
                     .Add(XElement.Parse(
                            @"<UI>
                                  <Publish Dialog=""WelcomeDlg"" Control=""Next"" Event=""NewDialog"" Order=""5"" Value=""InstallDirDlg"">1</Publish>
                                  <Publish Dialog=""InstallDirDlg"" Control=""Back"" Event=""NewDialog"" Order=""5""  Value=""WelcomeDlg"">1</Publish>
                              </UI>"));
    }
}