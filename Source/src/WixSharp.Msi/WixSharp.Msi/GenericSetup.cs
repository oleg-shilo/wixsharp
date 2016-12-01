using System;
using System.Threading;
//using System.Threading.Tasks;

namespace WixSharp.UI
{
    /// <summary>
    /// Generic class that represents runtime properties of the typical MSI setup. 
    /// It is a ViewModel class, which has 'value changed' events for all bindable properties 
    /// automatically marshalled for the cross-thread calls.
    /// </summary>
    public class GenericSetup : MsiSession
    {
        /// <summary>
        /// The path to the encapsulated MSI file.
        /// </summary>
        public string MsiFile;

        /// <summary>
        /// The path to the MSI session log file.
        /// </summary>
        public string LogFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSetup" /> class.
        /// </summary>
        /// <param name="msiFile">The MSI file.</param>
        /// <param name="enableLoging">if set to <c>true</c> [enable loging].</param>
        public GenericSetup(string msiFile, bool enableLoging = true)
        {
            this.MsiFile = msiFile;

            if (enableLoging)
                EnableLog(LogFile = msiFile + ".log");

            UpdateStatus();
        }

        void UpdateStatus()
        {
            var msi = new MsiParser(MsiFile);

            IsCurrentlyInstalled = msi.IsInstalled();
            ProductName = msi.GetProductName();
            ProductVersion = msi.GetProductVersion();

            ProductStatus = string.Format("The product is {0}INSTALLED\n\n", IsCurrentlyInstalled ? "" : "NOT ");
        }

        /// <summary>
        /// Starts the fresh installation.
        /// </summary>
        /// <param name="msiParams">The MSI parameters.</param>
        public virtual void StartInstall(string msiParams = null)
        {
            if (!IsCurrentlyInstalled)
            {
                RunAsync(() => ExecuteInstall(MsiFile, msiParams));

                LogFileCreated = true;
                IsRunning = true;
            }
            else
                ErrorStatus = "Product is already installed";
        }

        /// <summary>
        /// Starts the repair installation for the already installed product.
        /// </summary>
        /// <param name="msiParams">The MSI parameters.</param>
        public virtual void StartRepair(string msiParams = null)
        {
            if (IsCurrentlyInstalled)
            {
                RunAsync(() => ExecuteInstall(MsiFile, msiParams));

                LogFileCreated = true;
                IsRunning = true;
            }
            else
                ErrorStatus = "Product is not installed";
        }

        /// <summary>
        /// Starts the uninstallation of the already installed product.
        /// </summary>
        /// <param name="msiParams">The MSI params.</param>
        public virtual void StartUninstall(string msiParams = null)
        {
            if (IsCurrentlyInstalled)
            {
                RunAsync(() => ExecuteUninstall(MsiFile, msiParams));

                LogFileCreated = true;
                IsRunning = true;
            }
            else
                ErrorStatus = "Product is not installed";
        }

        void RunAsync(System.Action action)
        {
            //Task.Factory.StartNew(() =>
            ThreadPool.QueueUserWorkItem(x=>
            {
                try
                {
                    action();
                    ErrorStatus = "Success";
                    UpdateStatus();
                }
                catch
                {
                    ErrorStatus = "Failed. See log file for details.";
                }
                IsRunning = false;
            });
        }

        string errorStatus;

        /// <summary>
        /// Gets or sets the error status.
        /// </summary>
        /// <value>
        /// The error status.
        /// </value>
        public string ErrorStatus
        {
            get { return errorStatus; }
            set
            {
                errorStatus = value;
                OnPropertyChanged("ErrorStatus");
            }
        }

        string productStatus;

        /// <summary>
        /// Gets or sets the product status (installed or not installed).
        /// </summary>
        /// <value>
        /// The product status.
        /// </value>
        public string ProductStatus
        {
            get { return productStatus; }
            set
            {
                productStatus = value;
                OnPropertyChanged("ProductStatus");
            }
        }

        string productVersion;

        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        /// <value>
        /// The product version.
        /// </value>
        public string ProductVersion
        {
            get { return productVersion; }
            set
            {
                productVersion = value;
                OnPropertyChanged("ProductVersion");
            }
        }

        string productName;

        /// <summary>
        /// Gets or sets the MSI ProductName.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                OnPropertyChanged("ProductName");
            }
        }

        bool isRunning;

        /// <summary>
        /// Gets or sets a value indicating whether the setup is in progress.
        /// </summary>
        /// <value>
        /// <c>true</c> if this setup is in progress; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                if (isRunning)
                    NotStarted = false;

                OnPropertyChanged("IsRunning");
                OnPropertyChanged("IsNotRunning");
            }
        }

        bool notStarted = true;

        /// <summary>
        /// Gets or sets a value indicating whether setup was not started yet. This information
        /// can be useful for implementing "not started" UI state in the setup GUI.  
        /// </summary>
        /// <value>
        ///   <c>true</c> if setup was not started; otherwise, <c>false</c>.
        /// </value>
        public bool NotStarted
        {
            get { return notStarted; }
            set
            {
                notStarted = value;
                OnPropertyChanged("NotStarted");
            }
        }

        bool isCurrentlyInstalled;

        /// <summary>
        /// Gets or sets a value indicating whether the product is currently installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product is currently installed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentlyInstalled
        {
            get { return isCurrentlyInstalled; }
            set
            {
                isCurrentlyInstalled = value;
                OnPropertyChanged("IsCurrentlyInstalled");
                OnPropertyChanged("CanInstall");
                OnPropertyChanged("CanUnInstall");
                OnPropertyChanged("CanRepair");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the product can be installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product can install; otherwise, <c>false</c>.
        /// </value>
        public bool CanInstall { get { return !IsCurrentlyInstalled; } }

        /// <summary>
        /// Gets a value indicating whether the product can be uninstalled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product can uninstall; otherwise, <c>false</c>.
        /// </value>
        public bool CanUnInstall { get { return IsCurrentlyInstalled; } }

        /// <summary>
        /// Gets a value indicating whether the product can be repaired.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product can repaired; otherwise, <c>false</c>.
        /// </value>
        public bool CanRepair { get { return IsCurrentlyInstalled; } }

        bool logFileCreated;

        /// <summary>
        /// Gets or sets a value indicating whether the log file created.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the log file created; otherwise, <c>false</c>.
        /// </value>
        public bool LogFileCreated
        {
            get { return logFileCreated; }
            set
            {
                logFileCreated = value;
                OnPropertyChanged("LogFileCreated");
            }
        }
    }
}