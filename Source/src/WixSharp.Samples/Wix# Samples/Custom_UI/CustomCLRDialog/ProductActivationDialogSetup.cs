using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.Controls;

public static class ProductActivationDialogSetup
{
    static public void Build()
    {
        var project = new Project("CustomDialogTest");

        InjectProductActivationDialog(project);

        Compiler.PreserveTempFiles = true; 
        Compiler.BuildMsi(project);
    }

    [CustomAction]
    public static ActionResult ShowProductActivationDialog(Session session)
    {
        return WixCLRDialog.ShowAsMsiDialog(new CustomDialog(session));
    }

    static void InjectProductActivationDialog(Project project)
    {
        //Injects CLR dialog CustomDialog between MSI dialogs LicenseAgreementDlg and InstallDirDlg.
        //Passes custom action ShowProductActivationDialog for instantiating and popping up the CLR dialog.

        //This is practically a full equivalent of the WixSharp.CommonTasks.Tasks.InjectClrDialog(this Project project,...) extension method,
        //except it places and additional LicenseAccepted condition

        ManagedAction customDialog = new ShowClrDialogAction("ShowProductActivationDialog");

        project.Actions = project.Actions.Add(customDialog);

        project.UI = WUI.WixUI_Common;

        var customUI = new CommomDialogsUI();

        var prevDialog = NativeDialogs.LicenseAgreementDlg;
        var nextDialog = NativeDialogs.InstallDirDlg;

        //disconnect prev and next dialogs
        customUI.UISequence.RemoveAll(x => (x.Dialog == prevDialog && x.Control == Buttons.Next) ||
                                           (x.Dialog == nextDialog && x.Control == Buttons.Back));

        //create new dialogs connection with showAction in between
        customUI.On(prevDialog, Buttons.Next, new ExecuteCustomAction(customDialog.Id));
        customUI.On(prevDialog, Buttons.Next, new ShowDialog(nextDialog, Condition.ClrDialog_NextPressed + " AND LicenseAccepted = \"1\""));
        customUI.On(prevDialog, Buttons.Next, new CloseDialog("Exit", Condition.ClrDialog_CancelPressed) { Order = 2 });

        customUI.On(nextDialog, Buttons.Back, new ExecuteCustomAction(customDialog.Id));
        customUI.On(nextDialog, Buttons.Back, new ShowDialog(prevDialog, Condition.ClrDialog_BackPressed));

        project.CustomUI = customUI;
    }
}
