using ConsoleApplication1;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Controls;

class Script
{
    static public void Main()
    {
        var project = new Project("ChangingFontTest",
                        new Dir(@"%ProgramFiles%\My Company\My Product",
                          new File("readme.txt")));

        project.UI = WUI.WixUI_Common;

        // In this sample we use the CustomUIHelper with the local implementation of the CustomUI sequence
        // this in order to demonstrate how we set the fonts used in the UI.
        project.CustomUI = UIHelper.BuildCustomUI();

        //We want to change the color of the font beeing used in the CustomUI
        project.WixSourceGenerated += UIHelper.Compiler_WixSourceGenerated;

        //We can use a loclization file to change font and color for every string in all dialogs
        //project.LocalizationFile = "wixui.wxl";

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }
}

public class UIHelper
{
    public static CustomUI BuildCustomUI()
    {
        Dialog fontDialog = new CustomFontForm().ToWDialog();

        XElement xml = fontDialog.ToXElement();

        var customUI = new CustomUI();

        SetDefaultTextStyles(customUI);

        customUI.CustomDialogs.Add(fontDialog);

        customUI.On(NativeDialogs.ExitDialog, Buttons.Finish, new CloseDialog() { Order = 9999 });

        customUI.On(NativeDialogs.WelcomeDlg, Buttons.Next, new ShowDialog(NativeDialogs.LicenseAgreementDlg));

        customUI.On(NativeDialogs.LicenseAgreementDlg, Buttons.Back, new ShowDialog(NativeDialogs.WelcomeDlg));
        customUI.On(NativeDialogs.LicenseAgreementDlg, Buttons.Next, new ShowDialog(fontDialog));

        customUI.On(fontDialog, Buttons.Back, new ShowDialog(NativeDialogs.LicenseAgreementDlg));
        customUI.On(fontDialog, Buttons.Next, new ShowDialog(NativeDialogs.InstallDirDlg));
        customUI.On(fontDialog, Buttons.Cancel, new CloseDialog("Exit"));

        customUI.On(NativeDialogs.InstallDirDlg, Buttons.Back, new ShowDialog(fontDialog));
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
    public static void SetDefaultTextStyles(CustomUI customUI)
    {
        //The default styles for the WixUI_* and two ones used in the fontDialog
        //the TextStyles contains a dictionary of string and foSystem.Drawing.Font
        //System.Drawing.Font does not have any color accocietd with it, we will add that later.
        //Here we only create the fonts, set the size and fontstyle.

        //We remove all the default fonts and replace them with new ones.
        customUI.TextStyles.Clear();

        customUI.TextStyles.Add("WixUI_Font_Normal", new Font("Tahoma", 8));
        customUI.TextStyles.Add("WixUI_Font_Normal_Bold", new Font("Tahoma", 8, FontStyle.Bold));
        customUI.TextStyles.Add("WixUI_Font_Bigger", new Font("Tahoma", 12));
        customUI.TextStyles.Add("WixUI_Font_Title", new Font("Tahoma", 9, FontStyle.Bold));
        //Add the two new ones used in the fontDialog
        customUI.TextStyles.Add("MyRedConsolas", new Font("Consolas", 12));
        customUI.TextStyles.Add("MyGreenArial", new Font("Arial", 12, FontStyle.Bold));
    }

    public static void Compiler_WixSourceGenerated(XDocument document)
    {
        //Set the color of the msi dialog text for a given textstyle
        SetColorForTextStyle(document, "WixUI_Font_Title", Color.White);
        SetColorForTextStyle(document, "MyRedConsolas", Color.Red);
        SetColorForTextStyle(document, "MyGreenArial", Color.Green);
    }

    public static void SetColorForTextStyle(XDocument document, string textStyle, Color color)
    {
        document.FindAll("TextStyle")
            .Where(x => x.HasAttribute("Id", textStyle))
            .First()
            .Add(new XAttribute("Red", color.R.ToString()),
                 new XAttribute("Green", color.G.ToString()),
                 new XAttribute("Blue", color.B.ToString()));
    }
}