using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Linq;
using ConsoleApplication1;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.Controls;
using System;

class CustomUIHelper
{
    /// <summary>
    /// Builds the custom UI.
    /// This is the equivalent of the CustomUIBuilder.BuildPostLicenseDialogUI implementation
    /// </summary>
    /// <returns></returns>
    public static CustomUI BuildCustomUI()
    {
        Dialog activationDialog = new ProductActivationForm().ToWDialog();

        XElement xml = activationDialog.ToXElement();

        var customUI = new CustomUI();

        customUI.CustomDialogs.Add(activationDialog);

        customUI.On(NativeDialogs.ExitDialog, Buttons.Finish, new CloseDialog() { Order = 9999 });

        customUI.On(NativeDialogs.WelcomeDlg, Buttons.Next, new ShowDialog(NativeDialogs.LicenseAgreementDlg));

        customUI.On(NativeDialogs.LicenseAgreementDlg, Buttons.Back, new ShowDialog(NativeDialogs.WelcomeDlg));
        customUI.On(NativeDialogs.LicenseAgreementDlg, Buttons.Next, new ShowDialog(activationDialog, "LicenseAccepted = \"1\""));

        customUI.On(activationDialog, Buttons.Back, new ShowDialog(NativeDialogs.LicenseAgreementDlg));

        customUI.On(activationDialog, Buttons.Next, new DialogAction { Name = "DoAction", Value = "ValidateLicenceKey" },
                                                    new ShowDialog(NativeDialogs.InstallDirDlg, "SERIALNUMBER_VALIDATED = \"TRUE\""));

        customUI.On(activationDialog, Buttons.Cancel, new CloseDialog("Exit"));

        customUI.On(NativeDialogs.InstallDirDlg, Buttons.Back, new ShowDialog(activationDialog));
        customUI.On(NativeDialogs.InstallDirDlg, Buttons.Next, new SetTargetPath(),
                                                         new ShowDialog(NativeDialogs.VerifyReadyDlg));

        customUI.On(NativeDialogs.InstallDirDlg, Buttons.ChangeFolder,
                                                         new SetProperty("_BrowseProperty", "[WIXUI_INSTALLDIR]"),
                                                         new ShowDialog(CommonDialogs.BrowseDlg));

        customUI.On(NativeDialogs.VerifyReadyDlg, Buttons.Back, new ShowDialog(NativeDialogs.InstallDirDlg, Condition.NOT_Installed),
                                                          new ShowDialog(NativeDialogs.MaintenanceTypeDlg, Condition.Installed));

        customUI.On(NativeDialogs.MaintenanceWelcomeDlg, Buttons.Next, new ShowDialog(NativeDialogs.MaintenanceTypeDlg));

        customUI.On(NativeDialogs.MaintenanceTypeDlg, Buttons.Back, new ShowDialog(NativeDialogs.MaintenanceWelcomeDlg));
        customUI.On(NativeDialogs.MaintenanceTypeDlg, Buttons.Repair, new ShowDialog(NativeDialogs.VerifyReadyDlg));
        customUI.On(NativeDialogs.MaintenanceTypeDlg, Buttons.Remove, new ShowDialog(NativeDialogs.VerifyReadyDlg));

        return customUI;
    }
}
