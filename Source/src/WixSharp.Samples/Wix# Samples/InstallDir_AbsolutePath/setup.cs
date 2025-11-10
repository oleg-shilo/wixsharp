//css_dir ..\..\;
//css_ref Wix_bin\WixToolset.Dtf.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref WixSharp.UI.dll;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Controls;
using WixToolset.Dtf.WindowsInstaller;

class Script
{
    static public void Main()
    {
        var project = new ManagedProject("MyProduct",
                          new InstallDir(@"C:\%placeholder%)",
                              new File("setup.cs")));

        // project.UI = WUI.WixUI_ProgressOnly;
        // project.ManagedUI = ManagedUI.Default;
        project.UI = WUI.WixUI_InstallDir;
        // project.AfterInstall += Project_AfterInstall;

        project.Load += (SetupEventArgs e) =>
        {
            MessageBox.Show("Load");
            var installDir = e.Session["INSTALLDIR"];

            if (installDir.IsEmpty() || installDir.Contains("%placeholder%"))
            {
                MessageBox.Show("Load2");
                e.Session["INSTALLDIR"] = Environment.SpecialFolder.CommonDocuments.ToPath();
            }
        };

        project.UIInitialized += (SetupEventArgs e) =>
        {
            // Debug.Assert(false);
            var installDir = e.Session["INSTALLDIR"];

            if (installDir.IsEmpty() || installDir.Contains("%placeholder%"))
                e.Session["INSTALLDIR"] = Environment.SpecialFolder.CommonDocuments.ToPath();
        };

        project.BeforeInstall += (SetupEventArgs e) =>
        {
            MessageBox.Show("e.InstallDir -> " + e.InstallDir, "BeforeInstall");
            e.Result = ActionResult.UserExit;
        };

        project.WixSourceGenerated += (doc) =>
        {
            var sequence = doc.FindFirst("InstallUISequence");
            sequence.AddFirst(
                new XElement("Custom")
                    .AddAttributes("Condition=1;Action=WixSharp_Load_Action;Before=AppSearch"));
        };

        // Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    static void Project_AfterInstall(SetupEventArgs e)
    {
        MessageBox.Show("e.InstallDir -> " + e.InstallDir + "\n" +
                        "EnvVar('INSTALLDIR') -> " + Environment.GetEnvironmentVariable("INSTALLDIR"),
                        "AfterInstall");
    }
}