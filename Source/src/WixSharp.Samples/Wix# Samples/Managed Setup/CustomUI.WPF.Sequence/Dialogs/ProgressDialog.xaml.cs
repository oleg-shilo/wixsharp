using Caliburn.Micro;
using System.Security.Principal;
using System.Windows.Media.Imaging;
using WixSharp.CommonTasks;
using WixSharp.UI.Forms;
using WixToolset.Dtf.WindowsInstaller;

namespace WixSharp.UI.WPF.Sequence
{
    public partial class ProgressDialog : WpfDialog, IWpfDialog, IProgressDialog
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            UpdateTitles(ManagedFormHost.Runtime.Session);

            model = new ProgressDialogModel { Host = ManagedFormHost };
            ViewModelBinder.Bind(model, this, null);

            model.StartExecute();
        }

        public void UpdateTitles(ISession session)
        {
            if (session.IsUninstalling())
            {
                DialogTitleLabel.Text = "[ProgressDlgTitleRemoving]";
                DialogDescription.Text = "[ProgressDlgTextRemoving]";
            }
            else if (session.IsRepairing())
            {
                DialogTitleLabel.Text = "[ProgressDlgTextRepairing]";
                DialogDescription.Text = "[ProgressDlgTitleRepairing]";
            }
            else if (session.IsInstalling())
            {
                DialogTitleLabel.Text = "[ProgressDlgTitleInstalling]";
                DialogDescription.Text = "[ProgressDlgTextInstalling]";
            }

            // `Localize` resolves [...] titles and descriptions into the localized strings stored in MSI resources tables
            this.Localize();
        }

        ProgressDialogModel model;

        public override MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
            => model?.ProcessMessage(messageType, messageRecord, CurrentStatus.Text) ?? MessageResult.None;

        public override void OnExecuteComplete()
            => model?.OnExecuteComplete();

        public override void OnProgress(int progressPercentage)
        {
            if (model != null)
                model.ProgressValue = progressPercentage;
        }
    }

    public class ProgressDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host;

        ISession session => Host?.Runtime.Session;
        IManagedUIShell shell => Host?.Shell;

        public BitmapImage Banner => session?.GetResourceBitmap("WixSharpUI_Bmp_Banner").ToImageSource();

        public int ProgressValue { get => progressValue; set { progressValue = value; base.NotifyOfPropertyChange(() => ProgressValue); } }

        public bool UacPromptIsVisible => (!WindowsIdentity.GetCurrent().IsAdmin() && Uac.IsEnabled() && !uacPromptActioned);

        public string CurrentAction { get => currentAction; set { currentAction = value; base.NotifyOfPropertyChange(() => CurrentAction); } }

        int progressValue;
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

        public void StartExecute()
            => shell?.StartExecute();

        public void Cancel()
        {
            if (shell.IsDemoMode)
                shell.GoNext();
            else
                shell.Cancel();
        }

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

        public void OnExecuteComplete()
        {
            CurrentAction = null;
            shell?.GoNext();
        }
    }
}