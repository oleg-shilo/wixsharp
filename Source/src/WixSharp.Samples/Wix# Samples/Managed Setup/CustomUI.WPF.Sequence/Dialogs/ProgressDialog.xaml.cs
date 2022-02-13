using System.Security.Principal;
using System.Windows.Media.Imaging;
using Microsoft.Deployment.WindowsInstaller;
using Caliburn.Micro;
using WixSharp.CommonTasks;
using WixSharp.UI.Forms;

namespace WixSharp.UI.WPF
{
    public partial class ProgressDialog : WpfDialog, IWpfDialog
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            var session = ManagedFormHost.Runtime.Session;

            if (session.IsUninstalling())
            {
                DialogTitle.Text = "[ProgressDlgTitleRemoving]";
                DialogDescription.Text = "[ProgressDlgTextRemoving]";
            }
            else if (session.IsRepairing())
            {
                DialogTitle.Text = "[ProgressDlgTextRepairing]";
                DialogDescription.Text = "[ProgressDlgTitleRepairing]";
            }
            else if (session.IsInstalling())
            {
                DialogTitle.Text = "[ProgressDlgTitleInstalling]";
                DialogDescription.Text = "[ProgressDlgTextInstalling]";
            }

            this.Localize(); // this will resolve [...] titles and descriptions into the localized strings stored in MSI resources tables

            // -------

            model = new ProgressDialogModel { Host = ManagedFormHost };
            ViewModelBinder.Bind(model, this, null);

            model.StartExecute();
        }

        ProgressDialogModel model;

        public override MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
            => model?.ProcessMessage(messageType, messageRecord, CurrentStatus.Text) ?? MessageResult.None;
    }

    public class ProgressDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host;

        ISession session => Host?.Runtime.Session;
        public BitmapImage Banner => session?.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();

        public void StartExecute()
        {
            //Host?.Shell.StartExecute();
        }

        public bool UacPromptIsVisible
        {
            get
            {
                if (!WindowsIdentity.GetCurrent().IsAdmin() && Uac.IsEnabled() && !uacPromptActioned)
                    return true;
                else
                    return false;
            }
        }

        public string CurrentAction { get => currentAction; set { currentAction = value; base.NotifyOfPropertyChange(() => CurrentAction); } }

        bool uacPromptActioned = false;
        private string currentAction;

        public string UacPrompt
        {
            get
            {
                if (Uac.IsEnabled())
                {
                    var prompt = session?.Property("UAC_WARNING");
                    if (prompt.IsNotEmpty())
                        return prompt;
                    else
                        return
                            "Please wait for UAC prompt to appear. " +
                            "If it appears minimized then activate it from the taskbar.";
                }
                else
                    return null;
            }
        }

        public void GoPrev()
            => Host?.Shell.GoPrev();

        public void GoNext()
            => Host?.Shell.GoNext();

        public void Cancel()
            => Host?.Shell.Cancel();

        public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, string currentStatus)
        {
            switch (messageType)
            {
                case InstallMessage.InstallStart:
                case InstallMessage.InstallEnd:
                    {
                        uacPromptActioned = true;
                        base.NotifyOfPropertyChange(() => UacPromptIsVisible);
                    }
                    break;

                case InstallMessage.ActionStart:
                    {
                        try
                        {
                            //messageRecord[0] - is reserved for FormatString value

                            /*
                            messageRecord[2] unconditionally contains the string to display

                            Examples:

                               messageRecord[0]    "Action 23:14:50: [1]. [2]"
                               messageRecord[1]    "InstallFiles"
                               messageRecord[2]    "Copying new files"
                               messageRecord[3]    "File: [1],  Directory: [9],  Size: [6]"

                               messageRecord[0]    "Action 23:15:21: [1]. [2]"
                               messageRecord[1]    "RegisterUser"
                               messageRecord[2]    "Registering user"
                               messageRecord[3]    "[1]"

                            */

                            if (messageRecord.FieldCount >= 3)
                                CurrentAction = messageRecord[2].ToString();
                            else
                                CurrentAction = null;
                        }
                        catch
                        {
                            //Catch all, we don't want the installer to crash in an attempt to process message.
                        }
                    }
                    break;
            }
            return MessageResult.OK;
        }
    }
}