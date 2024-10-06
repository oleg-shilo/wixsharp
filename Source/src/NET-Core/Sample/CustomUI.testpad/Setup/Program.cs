using System;
using System.Linq;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Msi;

namespace WixSharp.CustomUI
{
    internal static class Program
    {
        static void Main()
        {
            var project = new ManagedProject("MyProduct",
                              new Dir(@"%ProgramFiles%\My Company\My Product",
                                  new File("Program.cs")));

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");

            // project.AddXmlInclude(@"..\Setup.UI\wix\Setup.UI.wxi");
            project.AddUIProject("Setup.UI");

            project.Load += Project_Load;

            project.BuildMsi();
        }

        static void Project_Load(SetupEventArgs e)
        {
            MessageBox.Show("Project_Load", "WixSharp");
            e.Result = WixToolset.Dtf.WindowsInstaller.ActionResult.Failure;
        }
    }
}