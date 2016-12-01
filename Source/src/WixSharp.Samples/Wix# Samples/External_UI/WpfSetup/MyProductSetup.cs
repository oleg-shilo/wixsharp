using WixSharp.UI;

namespace WpfSetup
{
    public class MyProductSetup : GenericSetup
    {
        bool installDocumentation;

        public bool InstallDocumentation
        {
            get { return installDocumentation; }
            set
            {
                installDocumentation = value;
                OnPropertyChanged("InstallDocumentation");
            }
        }

        public bool InitialCanInstall { get; set; }

        public bool InitialCanUnInstall { get; set; }

        public bool InitialCanRepair { get; set; }

        public void StartRepair()
        {
            //The MSI will abort any attempt to start unless CUSTOM_UI is set. This  a feature for preventing starting the MSI without this custom GUI.
            base.StartRepair("CUSTOM_UI=true");
        }

        public void StartChange()
        {
            //Adjust the MSI properties to indicate which feature you want to install
            if (InstallDocumentation)
                base.StartRepair("CUSTOM_UI=true ADDLOCAL=Binaries,Documentation");
            else
                base.StartRepair("CUSTOM_UI=true REMOVE=Documentation");
        }

        public void StartInstall()
        {
            if (InstallDocumentation)
                base.StartInstall("CUSTOM_UI=true ADDLOCAL=Binaries,Documentation");
            else
                base.StartInstall("CUSTOM_UI=true ADDLOCAL=Binaries");
        }
       
        public MyProductSetup(string msiFile, bool enableLoging = true)
            : base(msiFile, enableLoging)
        {
            InitialCanInstall = CanInstall;
            InitialCanUnInstall = CanUnInstall;
            InitialCanRepair = CanRepair;

            SetupStarted += MyProductSetup_SetupStarted;

            //Uncomment if you want to see current action name changes. Otherwise it is too quick.
            //ProgressStepDelay = 50;
        }


        private void MyProductSetup_SetupStarted()
        {
            //this.LogFileCreated
        }
    }
}