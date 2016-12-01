using System;
using System.Threading.Tasks;
using WixSharp.UI;

namespace WpfSetup
{
    class SetupSession : MsiSession
    {
        string msiFile;
        public string LogFile;

        public SetupSession( string msiFile)
        {
            this.msiFile = msiFile;
            EnableLog(LogFile = msiFile + ".log");

            UpdateStatus();

            //Uncomment if you want to see current action name changes. Otherwise it is too quick.
            //ProgressStepDelay = 100;
        }

        void UpdateStatus()
        {
            var msi = new MsiParser(msiFile);

            IsCurrentlyInstalled = msi.IsInstalled();
            ProductName = msi.GetProductName();
            ProductVersion = msi.GetProductVersion();

            ProductStatus = string.Format("The product is {0}INSTALLED\n\n", IsCurrentlyInstalled ? "" : "NOT ");
        }

        public void Install()
        {
            if (!IsCurrentlyInstalled)
            {
                RunAsync(() => ExecuteInstall(msiFile));

                LogFileCreated = true;
                IsRunning = true;
            }
            else
                ErrorStatus = "Product is already installed";
        }

        public void Repair()
        {
            if (IsCurrentlyInstalled)
            {
                RunAsync(() => ExecuteRepair(msiFile));

                LogFileCreated = true;
                IsRunning = true;
            }
            else
                ErrorStatus = "Product is not installed";
        }

        public void Uninstall()
        {
            if (IsCurrentlyInstalled)
            {
                RunAsync(() => ExecuteUninstall(msiFile));

                LogFileCreated = true;
                IsRunning = true;
            }
            else
                ErrorStatus = "Product is not installed";
        }

        void RunAsync(Action action)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    action();
                    ErrorStatus = "Success";
                    UpdateStatus();
                }
                catch (Exception e)
                {
                    ErrorStatus = "Failed. See log file for details.";
                }
                IsRunning = false;
            });
        }

        string errorStatus;

        public string ErrorStatus
        {
            get { return errorStatus; }
            set
            {
                errorStatus = value;
                InUiThread(() => OnPropertyChanged("ErrorStatus"));
            }
        }

        string productStatus;

        public string ProductStatus
        {
            get { return productStatus; }
            set
            {
                productStatus = value;
                InUiThread(() => OnPropertyChanged("ProductStatus"));
            }
        }

        string productVersion;
        
        public string ProductVersion
        {
            get { return productVersion; }
            set
            {
                productVersion = value;
                InUiThread(() => OnPropertyChanged("ProductVersion"));
            }
        }

        string productName;
        
        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                InUiThread(() => OnPropertyChanged("ProductName"));
            }
        }

        bool isRunning;

        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                if (isRunning)
                    NotStarted = false;
                
                InUiThread(() =>
                    {
                        OnPropertyChanged("IsRunning");
                        OnPropertyChanged("IsNotRunning");
                    });
            }
        }

        bool notStarted = true;
        
        public bool NotStarted
        {
            get { return notStarted; }
            set
            {
                notStarted = value;
                InUiThread(() => OnPropertyChanged("NotStarted"));
            }
        }

        bool isCurrentlyInstalled;

        public bool IsCurrentlyInstalled
        {
            get { return isCurrentlyInstalled; }
            set
            {
                isCurrentlyInstalled = value;
                InUiThread(() =>
                    {
                        OnPropertyChanged("IsCurrentlyInstalled");
                        OnPropertyChanged("CanInstall");
                        OnPropertyChanged("CanUnInstall");
                        OnPropertyChanged("CanRepair");
                    });
            }
        }

        public bool CanInstall { get { return !IsCurrentlyInstalled; } }

        public bool CanUnInstall { get { return IsCurrentlyInstalled; } }

        public bool CanRepair { get { return IsCurrentlyInstalled; } }

        bool logFileCreated;

        public bool LogFileCreated
        {
            get { return logFileCreated; }
            set
            {
                logFileCreated = value;
                InUiThread(() => OnPropertyChanged("LogFileCreated"));
            }
        }
    }
}