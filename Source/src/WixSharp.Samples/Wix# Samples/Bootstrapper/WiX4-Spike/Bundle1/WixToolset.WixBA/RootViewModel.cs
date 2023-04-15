// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.WixBA
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using WixToolset.Mba.Core;

    /// <summary>
    /// The errors returned from the engine
    /// </summary>
    public enum Error
    {
        UserCancelled = 1223,
    }

    /// <summary>
    /// The model of the root view in WixBA.
    /// </summary>
    public class RootViewModel : PropertyNotifyBase
    {
        private ICommand cancelCommand;
        private ICommand closeCommand;

        private bool canceled;
        private InstallationState installState;
        private DetectionState detectState;
        private UpgradeDetectionState upgradeDetectState;

        /// <summary>
        /// Creates a new model of the root view.
        /// </summary>
        public RootViewModel()
        {
            this.InstallationViewModel = new InstallationViewModel(this);
            this.ProgressViewModel = new ProgressViewModel(this);
            this.UpdateViewModel = new UpdateViewModel(this);
        }

        public InstallationViewModel InstallationViewModel { get; private set; }
        public ProgressViewModel ProgressViewModel { get; private set; }
        public UpdateViewModel UpdateViewModel { get; private set; }
        public Dispatcher Dispatcher { get; set; }
        public IntPtr ViewWindowHandle { get; set; }
        public bool AutoClose { get; set; }

        public ICommand CloseCommand
        {
            get
            {
                if (this.closeCommand == null)
                {
                    this.closeCommand = new RelayCommand(param => WixBA.View.Close());
                }

                return this.closeCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                if (this.cancelCommand == null)
                {
                    this.cancelCommand = new RelayCommand(param =>
                    {
                        this.CancelButton_Click();
                    },
                    param => !this.Canceled);
                }

                return this.cancelCommand;
            }
        }

        public bool CancelAvailable
        {
            get { return InstallationState.Applying == this.InstallState; }
        }

        public bool Canceled
        {
            get
            {
                return this.canceled;
            }

            set
            {
                if (this.canceled != value)
                {
                    this.canceled = value;
                    base.OnPropertyChanged("Canceled");
                }
            }
        }

        /// <summary>
        /// Gets and sets the detect state of the view's model.
        /// </summary>
        public DetectionState DetectState
        {
            get
            {
                return this.detectState;
            }

            set
            {
                if (this.detectState != value)
                {
                    this.detectState = value;

                    // Notify all the properties derived from the state that the state changed.
                    base.OnPropertyChanged("DetectState");
                }
            }
        }

        /// <summary>
        /// Gets and sets the upgrade detect state of the view's model.
        /// </summary>
        public UpgradeDetectionState UpgradeDetectState
        {
            get
            {
                return this.upgradeDetectState;
            }

            set
            {
                if (this.upgradeDetectState != value)
                {
                    this.upgradeDetectState = value;

                    // Notify all the properties derived from the state that the state changed.
                    base.OnPropertyChanged("UpgradeDetectState");
                }
            }
        }

        /// <summary>
        /// Gets and sets the installation state of the view's model.
        /// </summary>
        public InstallationState InstallState
        {
            get
            {
                return this.installState;
            }

            set
            {
                if (this.installState != value)
                {
                    this.installState = value;

                    // Notify all the properties derived from the state that the state changed.
                    base.OnPropertyChanged("InstallState");
                    base.OnPropertyChanged("CancelAvailable");
                }
            }
        }

        /// <summary>
        /// Gets and sets the state of the view's model before apply begins in order to return to that state if cancel or rollback occurs.
        /// </summary>
        public InstallationState PreApplyState { get; set; }

        /// <summary>
        /// Gets and sets the path where the bundle is currently installed or will be installed.
        /// </summary>
        public string InstallDirectory
        {
            get
            {
                return WixBA.Model.InstallDirectory;
            }

            set
            {
                if (WixBA.Model.InstallDirectory != value)
                {
                    WixBA.Model.InstallDirectory = value;
                    base.OnPropertyChanged("InstallDirectory");
                }
            }
        }

        /// <summary>
        /// The Title of this bundle.
        /// </summary>
        public string Title
        {
            get
            {
                return WixDistribution.ShortProduct;
            }
        }

        /// <summary>
        /// Prompts the user to make sure they want to cancel.
        /// This needs to run on the UI thread, use Dispatcher.Invoke to call this from a background thread.
        /// </summary>
        public void CancelButton_Click()
        {
            if (this.Canceled)
            {
                return;
            }

            if (Display.Full == WixBA.Model.Command.Display)
            {
                this.Canceled = (MessageBoxResult.Yes == MessageBox.Show(WixBA.View, "Are you sure you want to cancel?", "WiX Toolset", MessageBoxButton.YesNo, MessageBoxImage.Error));
            }
            else
            {
                this.Canceled = true;
            }
        }
    }
}
