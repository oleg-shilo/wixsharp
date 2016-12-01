using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Controls;

namespace ConsoleApplication1
{
    public partial class ProductActivationForm : WixSharp.Controls.WixForm
    {
        public ProductActivationForm()
        {
            InitializeComponent();


            //Normally defining the start of the next dialog (or going to the previous one) is done outside of 
            //the dialog definition (in the publish elements of the UI element of XML). 
            //
            //WixSharp does the all necessary work for linking dialogs in the sequence and invoking Custom Actions 
            //in CustomUIBuilder.BuildPostLicenseDialogUI. 
            //
            //CustomUIBuilder is intelligent enough to automatically link elements with the well-known IDs (e.g. Button.Next) 
            //to the well-known actions (e.g. navigating to next dialog).
            //
            //If you need to have more control over this process you can always implement your own version of the CustomUIBuilder
            //like the one in CustomUIHelper.cs file in this project.
            
            //-------------------------------------------------------

            //When the built-in CustomUIBuilder.BuildPostLicenseDialogUI is used you can change the actions of the Back, Next and Cancel 
            //buttons by specifying the alternative actions from the WinForms buttons event handlers. If you want to experiment with 
            //this approach then just uncomment the code in this handlers (see event handlers below).  

            //-------------------------------------------------------

            //uncomment if you want to add conditions to the actions
            //NextButton.Conditions.Add(new WixControlCondition { Action = ConditionAction.enable, Value = "USE_ACTIVATION=\"1\"" });
            //NextButton.Conditions.Add(new WixControlCondition { Action = ConditionAction.disable, Value = "NOT (USE_ACTIVATION=\"1\")" });
        }

        void CancelButton_Click()
        {
            //uncomment if you want to control button/action association manually
            //this.EndDialog(EndDialogValue.Exit); 
        }

        void NextButton_Click()
        {
            //uncomment (one of the overrides below) if you want to control button/action association manually
            //this.EndDialog(EndDialogValue.Return);
            //or
            //this.Do(ControlAction.EndDialog, EndDialogValue.Return); 
            //or
            //this.Do("EndDialog", "Return"); 
        }

        void BackButton_Click()
        {
            //uncomment if you want to control button/action association manually
            //this.Do(ControlAction.NewDialog, "LicenseAgreementDlg");
        }

        private void wixButton_Click()
        {
            this.Do(ControlAction.DoAction, "ClaimLicenceKey");
        }
    }
}
