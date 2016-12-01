using ConsoleApplication1;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Controls;

class Script
{
    static public void Main()
    {
        //Support for building native MSI UI is an experimental feature and no longer supported.
        //It has been superseded by the "Managed UI" feature available in v1.0.22.0 and higher. 

        Dialog productActivationDialog = new ProductActivationForm().ToWDialog();

        var project = new Project("CustomDialogTest",

                          new Dir(@"%ProgramFiles%\My Company\My Product",
                              new File(@"..\..\AppFiles\CommunityLicence.txt") { Condition = "LICENCING_MODEL = \"COMMUNITY\"".ToCondition() },
                              new File(@"..\..\AppFiles\ProLicence.txt") { Condition = "LICENCING_MODEL = \"PRO\"".ToCondition() },
                              new File(@"..\..\AppFiles\DemoLicence.txt") { Condition = "LICENCING_MODEL = \"DEMO\"".ToCondition() },
                              new File(@"..\..\AppFiles\TrialLicence.txt") { Condition = "LICENCING_MODEL = \"TRIAL\"".ToCondition() }),

                          new Property("SERIALNUMBER", "123-456-DEMO"),
                          new Property("UseActivation", "1"),
                          new Property("LICENCING_MODEL", "DEMO"),
                          new Property("SERIALNUMBER_VALIDATED", "FALSE"),

                          new ManagedAction("ValidateLicenceKey") { Id = "ValidateLicenceKey", Sequence = Sequence.NotInSequence },
                          new ManagedAction("ClaimLicenceKey") { Id = "ClaimLicenceKey", Sequence = Sequence.NotInSequence });

        project.UI = WUI.WixUI_Common;
        project.CustomUI = CustomUIBuilder.BuildPostLicenseDialogUI(customDialog: productActivationDialog,
                                                                    onNextActions: new DialogAction[]{
                                                                                       new ExecuteCustomAction ("ValidateLicenceKey"),
                                                                                       new ShowDialog(NativeDialogs.InstallDirDlg, "SERIALNUMBER_VALIDATED = \"TRUE\"")});
        
        //In this sample we are using built-in BuildPostLicenseDialogUI but if it is not suitable for
        //you can define you own routine for building UI definition.
        //CustomUIHelper.cs is an example of such an alternative. In this case it does the same job 
        //as built-in BuildPostLicenseDialogUI but you can modify it to suite your needs better.
        //
        //alternatively you can call CustoUIHelper with the local implementation of the CustomUI sequence
        //project.CustomUI = CustoUIHelper.BuildCustomUI();

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult ValidateLicenceKey(Session session)
    {
        string keyword = session["LICENCING_MODEL"];

        if (session["SERIALNUMBER"].Contains(keyword))
        {
            session["SERIALNUMBER_VALIDATED"] = "TRUE";
        }
        else
        {
            MessageBox.Show("Provided activation key is invalid", "Product Activation");
            session["SERIALNUMBER_VALIDATED"] = "FALSE";
        }

        return ActionResult.NotExecuted;
    }

    [CustomAction]
    public static ActionResult ClaimLicenceKey(Session session)
    {
        //session["SERIALNUMBER"] = "123-456-" + session["LICENCING_MODEL"]; //this will not update the textbox as MSI binding is "write-only ("one way" binding mode)

        try
        {
            Process.Start("http://www.csscript.net/WixSharp/" + session["LICENCING_MODEL"] + "_licence.html");
        }
        catch { }

        return ActionResult.Success;
    }
}