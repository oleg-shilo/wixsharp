// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.WixBA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using WixToolset.Mba.Core;
    using IO = System.IO;

    /// <summary>
    /// The states of detection.
    /// </summary>
    public enum DetectionState
    {
        Absent,
        Present,
    }

    /// <summary>
    /// The states of upgrade detection.
    /// </summary>
    public enum UpgradeDetectionState
    {
        // There are no Upgrade related bundles installed.
        None,

        // All Upgrade related bundles that are installed are older than or the same version as this bundle.
        Older,

        // At least one Upgrade related bundle is installed that is newer than this bundle.
        Newer,
    }

    /// <summary>
    /// The states of installation.
    /// </summary>
    public enum InstallationState
    {
        Initializing,
        Detecting,
        Waiting,
        Planning,
        Applying,
        Applied,
        Failed,
    }

    /// <summary>
    /// The model of the installation view in WixBA.
    /// </summary>
    public class InstallationViewModel : PropertyNotifyBase
    {
        private readonly RootViewModel root;

        private readonly Dictionary<string, int> downloadRetries;
        private bool downgrade;
        private string downgradeMessage;

        private ICommand licenseCommand;
        private ICommand launchHomePageCommand;
        private ICommand launchNewsCommand;
        private ICommand launchVSExtensionPageCommand;
        private ICommand installCommand;
        private ICommand repairCommand;
        private ICommand uninstallCommand;
        private ICommand openLogCommand;
        private ICommand openLogFolderCommand;
        private ICommand tryAgainCommand;

        private string message;
        private DateTime cachePackageStart;
        private DateTime executePackageStart;

        /// <summary>
        /// Creates a new model of the installation view.
        /// </summary>
        public InstallationViewModel(RootViewModel root)
        {
            this.root = root;
            this.downloadRetries = new Dictionary<string, int>();

            this.root.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.RootPropertyChanged);

            WixBA.Model.Bootstrapper.DetectBegin += this.DetectBegin;
            WixBA.Model.Bootstrapper.DetectRelatedBundle += this.DetectedRelatedBundle;
            WixBA.Model.Bootstrapper.DetectComplete += this.DetectComplete;
            WixBA.Model.Bootstrapper.PlanPackageBegin += this.PlanPackageBegin;
            WixBA.Model.Bootstrapper.PlanComplete += this.PlanComplete;
            WixBA.Model.Bootstrapper.ApplyBegin += this.ApplyBegin;
            WixBA.Model.Bootstrapper.CacheAcquireBegin += this.CacheAcquireBegin;
            WixBA.Model.Bootstrapper.CacheAcquireResolving += this.CacheAcquireResolving;
            WixBA.Model.Bootstrapper.CacheAcquireComplete += this.CacheAcquireComplete;
            WixBA.Model.Bootstrapper.ExecutePackageBegin += this.ExecutePackageBegin;
            WixBA.Model.Bootstrapper.ExecutePackageComplete += this.ExecutePackageComplete;
            WixBA.Model.Bootstrapper.Error += this.ExecuteError;
            WixBA.Model.Bootstrapper.ApplyComplete += this.ApplyComplete;
            WixBA.Model.Bootstrapper.SetUpdateComplete += this.SetUpdateComplete;
        }

        void RootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (("DetectState" == e.PropertyName) || ("UpgradeDetectState" == e.PropertyName) || ("InstallState" == e.PropertyName))
            {
                base.OnPropertyChanged("RepairEnabled");
                base.OnPropertyChanged("InstallEnabled");
                base.OnPropertyChanged("IsComplete");
                base.OnPropertyChanged("IsSuccessfulCompletion");
                base.OnPropertyChanged("IsFailedCompletion");
                base.OnPropertyChanged("StatusText");
                base.OnPropertyChanged("UninstallEnabled");
            }
        }

        /// <summary>
        /// Gets the version for the application.
        /// </summary>
        public string Version
        {
            get { return String.Concat("v", WixBA.Model.Version.ToString()); }
        }

        /// <summary>
        /// The Publisher of this bundle.
        /// </summary>
        public string Publisher
        {
            get
            {
                string company = "[AssemblyCompany]";
                return WixDistribution.ReplacePlaceholders(company, typeof(WixBA).Assembly);
            }
        }

        /// <summary>
        /// The Publisher of this bundle.
        /// </summary>
        public string SupportUrl
        {
            get
            {
                return WixDistribution.SupportUrl;
            }
        }

        public string VSExtensionUrl
        {
            get
            {
                return WixDistribution.VSExtensionsLandingUrl;
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    base.OnPropertyChanged("Message");
                }
            }
        }

        /// <summary>
        /// Gets and sets whether the view model considers this install to be a downgrade.
        /// </summary>
        public bool Downgrade
        {
            get
            {
                return this.downgrade;
            }

            set
            {
                if (this.downgrade != value)
                {
                    this.downgrade = value;
                    base.OnPropertyChanged("Downgrade");
                }
            }
        }

        public string DowngradeMessage
        {
            get
            {
                return this.downgradeMessage;
            }
            set
            {
                if (this.downgradeMessage != value)
                {
                    this.downgradeMessage = value;
                    base.OnPropertyChanged("DowngradeMessage");
                }
            }
        }

        public ICommand LaunchHomePageCommand
        {
            get
            {
                if (this.launchHomePageCommand == null)
                {
                    this.launchHomePageCommand = new RelayCommand(param => WixBA.LaunchUrl(this.SupportUrl), param => true);
                }

                return this.launchHomePageCommand;
            }
        }

        public ICommand LaunchNewsCommand
        {
            get
            {
                if (this.launchNewsCommand == null)
                {
                    this.launchNewsCommand = new RelayCommand(param => WixBA.LaunchUrl(WixDistribution.NewsUrl), param => true);
                }

                return this.launchNewsCommand;
            }
        }

        public ICommand LaunchVSExtensionPageCommand
        {
            get
            {
                if (this.launchVSExtensionPageCommand == null)
                {
                    this.launchVSExtensionPageCommand = new RelayCommand(param => WixBA.LaunchUrl(WixDistribution.VSExtensionsLandingUrl), param => true);
                }

                return this.launchVSExtensionPageCommand;
            }
        }

        public ICommand LicenseCommand
        {
            get
            {
                if (this.licenseCommand == null)
                {
                    this.licenseCommand = new RelayCommand(param => this.LaunchLicense(), param => true);
                }

                return this.licenseCommand;
            }
        }

        public bool LicenseEnabled
        {
            get { return this.LicenseCommand.CanExecute(this); }
        }

        public ICommand CloseCommand
        {
            get { return this.root.CloseCommand; }
        }

        public bool IsComplete
        {
            get { return this.IsSuccessfulCompletion || this.IsFailedCompletion; }
        }

        public bool IsSuccessfulCompletion
        {
            get { return InstallationState.Applied == this.root.InstallState; }
        }

        public bool IsFailedCompletion
        {
            get { return InstallationState.Failed == this.root.InstallState; }
        }

        public ICommand InstallCommand
        {
            get
            {
                if (this.installCommand == null)
                {
                    this.installCommand = new RelayCommand(
                        param => WixBA.Plan(LaunchAction.Install),
                        param => this.root.DetectState == DetectionState.Absent && this.root.UpgradeDetectState != UpgradeDetectionState.Newer && this.root.InstallState == InstallationState.Waiting);
                }

                return this.installCommand;
            }
        }

        public bool InstallEnabled
        {
            get { return this.InstallCommand.CanExecute(this); }
        }

        public ICommand RepairCommand
        {
            get
            {
                if (this.repairCommand == null)
                {
                    this.repairCommand = new RelayCommand(param => WixBA.Plan(LaunchAction.Repair), param => this.root.DetectState == DetectionState.Present && this.root.InstallState == InstallationState.Waiting);
                }

                return this.repairCommand;
            }
        }

        public bool RepairEnabled
        {
            get { return this.RepairCommand.CanExecute(this); }
        }

        public ICommand UninstallCommand
        {
            get
            {
                if (this.uninstallCommand == null)
                {
                    this.uninstallCommand = new RelayCommand(param => WixBA.Plan(LaunchAction.Uninstall), param => this.root.DetectState == DetectionState.Present && this.root.InstallState == InstallationState.Waiting);
                }

                return this.uninstallCommand;
            }
        }

        public bool UninstallEnabled
        {
            get { return true; }
            // get { return this.UninstallCommand.CanExecute(this); }
        }

        public ICommand OpenLogCommand
        {
            get
            {
                if (this.openLogCommand == null)
                {
                    this.openLogCommand = new RelayCommand(param => WixBA.OpenLog(new Uri(WixBA.Model.Engine.GetVariableString("WixBundleLog"))));
                }
                return this.openLogCommand;
            }
        }

        public ICommand OpenLogFolderCommand
        {
            get
            {
                if (this.openLogFolderCommand == null)
                {
                    string logFolder = IO.Path.GetDirectoryName(WixBA.Model.Engine.GetVariableString("WixBundleLog"));
                    this.openLogFolderCommand = new RelayCommand(param => WixBA.OpenLogFolder(logFolder));
                }
                return this.openLogFolderCommand;
            }
        }

        public ICommand TryAgainCommand
        {
            get
            {
                if (this.tryAgainCommand == null)
                {
                    this.tryAgainCommand = new RelayCommand(param =>
                        {
                            this.root.Canceled = false;
                            WixBA.Plan(WixBA.Model.PlannedAction);
                        }, param => this.IsFailedCompletion);
                }

                return this.tryAgainCommand;
            }
        }

        public string StatusText
        {
            get
            {
                switch (this.root.InstallState)
                {
                    case InstallationState.Applied:
                        return "Complete";

                    case InstallationState.Failed:
                        return this.root.Canceled ? "Cancelled" : "Failed";

                    default:
                        return "Unknown"; // this shouldn't be shown in the UI.
                }
            }
        }

        /// <summary>
        /// Launches the license in the default viewer.
        /// </summary>
        private void LaunchLicense()
        {
            string folder = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            WixBA.LaunchUrl(IO.Path.Combine(folder, "License.txt"));
        }

        private void DetectBegin(object sender, DetectBeginEventArgs e)
        {
            this.root.DetectState = RegistrationType.Full == e.RegistrationType ? DetectionState.Present : DetectionState.Absent;
            WixBA.Model.PlannedAction = LaunchAction.Unknown;
        }

        private void DetectedRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
        {
            if (e.RelationType == RelationType.Upgrade)
            {
                if (WixBA.Model.Engine.CompareVersions(this.Version, e.Version) > 0)
                {
                    if (this.root.UpgradeDetectState == UpgradeDetectionState.None)
                    {
                        this.root.UpgradeDetectState = UpgradeDetectionState.Older;
                    }
                }
                else
                {
                    this.root.UpgradeDetectState = UpgradeDetectionState.Newer;
                }
            }

            if (!WixBA.Model.BAManifest.Bundle.Packages.ContainsKey(e.ProductCode))
            {
                WixBA.Model.BAManifest.Bundle.AddRelatedBundleAsPackage(e);
            }
        }

        private void SetUpdateComplete(object sender, SetUpdateCompleteEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.NewPackageId) && !WixBA.Model.BAManifest.Bundle.Packages.ContainsKey(e.NewPackageId))
            {
                WixBA.Model.BAManifest.Bundle.AddUpdateBundleAsPackage(e);
            }
        }

        private void DetectComplete(object sender, DetectCompleteEventArgs e)
        {
            // Parse the command line string before any planning.
            this.ParseCommandLine();
            this.root.InstallState = InstallationState.Waiting;

            if (LaunchAction.Uninstall == WixBA.Model.Command.Action &&
                ResumeType.Arp != WixBA.Model.Command.Resume) // MSI and WixStdBA require some kind of confirmation before proceeding so WixBA should, too.
            {
                WixBA.Model.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for uninstall");
                WixBA.Plan(LaunchAction.Uninstall);
            }
            else if (Hresult.Succeeded(e.Status))
            {
                if (this.root.UpgradeDetectState == UpgradeDetectionState.Newer)
                {
                    this.Downgrade = true;
                    this.DowngradeMessage = "There is already a newer version of WiX installed on this machine.";
                }

                if (LaunchAction.Layout == WixBA.Model.Command.Action)
                {
                    WixBA.PlanLayout();
                }
                else if (WixBA.Model.Command.Display != Display.Full)
                {
                    // If we're not waiting for the user to click install, dispatch plan with the default action.
                    WixBA.Model.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for non-interactive mode.");
                    WixBA.Plan(WixBA.Model.Command.Action);
                }
            }
            else
            {
                this.root.InstallState = InstallationState.Failed;
            }

            // Force all commands to reevaluate CanExecute.
            // InvalidateRequerySuggested must be run on the UI thread.
            this.root.Dispatcher.Invoke(new Action(CommandManager.InvalidateRequerySuggested));
        }

        private void PlanPackageBegin(object sender, PlanPackageBeginEventArgs e)
        {
            // If we're able to run our BA, we don't want to install .NET since the one on the machine is already good enough.
            if (e.PackageId.StartsWith("NetFx4", StringComparison.OrdinalIgnoreCase) || e.PackageId.StartsWith("DesktopNetCoreRuntime", StringComparison.OrdinalIgnoreCase))
            {
                e.State = RequestState.None;
            }
        }

        private void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (Hresult.Succeeded(e.Status))
            {
                this.root.PreApplyState = this.root.InstallState;
                this.root.InstallState = InstallationState.Applying;
                WixBA.Model.Engine.Apply(this.root.ViewWindowHandle);
            }
            else
            {
                this.root.InstallState = InstallationState.Failed;
            }
        }

        private void ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            this.downloadRetries.Clear();
        }

        private void CacheAcquireBegin(object sender, CacheAcquireBeginEventArgs e)
        {
            this.cachePackageStart = DateTime.Now;
        }

        private void CacheAcquireResolving(object sender, CacheAcquireResolvingEventArgs e)
        {
            if (e.Action == CacheResolveOperation.Download && !this.downloadRetries.ContainsKey(e.PackageOrContainerId))
            {
                this.downloadRetries.Add(e.PackageOrContainerId, 0);
            }
        }

        private void CacheAcquireComplete(object sender, CacheAcquireCompleteEventArgs e)
        {
            this.AddPackageTelemetry("Cache", e.PackageOrContainerId ?? String.Empty, DateTime.Now.Subtract(this.cachePackageStart).TotalMilliseconds, e.Status);

            if (e.Status < 0 && this.downloadRetries.TryGetValue(e.PackageOrContainerId, out var retries) && retries < 3)
            {
                this.downloadRetries[e.PackageOrContainerId] = retries + 1;
                switch (e.Status)
                {
                    case -2147023294: //HRESULT_FROM_WIN32(ERROR_INSTALL_USEREXIT)
                    case -2147024894: //HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND)
                    case -2147012889: //HRESULT_FROM_WIN32(ERROR_INTERNET_NAME_NOT_RESOLVED)
                        break;

                    default:
                        e.Action = BOOTSTRAPPER_CACHEACQUIRECOMPLETE_ACTION.Retry;
                        break;
                }
            }
        }

        private void ExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
        {
            lock (this)
            {
                this.executePackageStart = e.ShouldExecute ? DateTime.Now : DateTime.MinValue;
            }
        }

        private void ExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
        {
            lock (this)
            {
                if (DateTime.MinValue < this.executePackageStart)
                {
                    this.AddPackageTelemetry("Execute", e.PackageId ?? String.Empty, DateTime.Now.Subtract(this.executePackageStart).TotalMilliseconds, e.Status);
                    this.executePackageStart = DateTime.MinValue;
                }
            }
        }

        private void ExecuteError(object sender, ErrorEventArgs e)
        {
            lock (this)
            {
                if (!this.root.Canceled)
                {
                    // If the error is a cancel coming from the engine during apply we want to go back to the preapply state.
                    if (InstallationState.Applying == this.root.InstallState && (int)Error.UserCancelled == e.ErrorCode)
                    {
                        this.root.InstallState = this.root.PreApplyState;
                    }
                    else
                    {
                        this.Message = e.ErrorMessage;

                        if (Display.Full == WixBA.Model.Command.Display)
                        {
                            // On HTTP authentication errors, have the engine try to do authentication for us.
                            if (ErrorType.HttpServerAuthentication == e.ErrorType || ErrorType.HttpProxyAuthentication == e.ErrorType)
                            {
                                e.Result = Result.TryAgain;
                            }
                            else // show an error dialog.
                            {
                                MessageBoxButton msgbox = MessageBoxButton.OK;
                                switch (e.UIHint & 0xF)
                                {
                                    case 0:
                                        msgbox = MessageBoxButton.OK;
                                        break;

                                    case 1:
                                        msgbox = MessageBoxButton.OKCancel;
                                        break;
                                    // There is no 2! That would have been MB_ABORTRETRYIGNORE.
                                    case 3:
                                        msgbox = MessageBoxButton.YesNoCancel;
                                        break;

                                    case 4:
                                        msgbox = MessageBoxButton.YesNo;
                                        break;
                                        // default: stay with MBOK since an exact match is not available.
                                }

                                MessageBoxResult result = MessageBoxResult.None;
                                WixBA.View.Dispatcher.Invoke((Action)delegate ()
                                    {
                                        result = MessageBox.Show(WixBA.View, e.ErrorMessage, "WiX Toolset", msgbox, MessageBoxImage.Error);
                                    }
                                    );

                                // If there was a match from the UI hint to the msgbox value, use the result from the
                                // message box. Otherwise, we'll ignore it and return the default to Burn.
                                if ((e.UIHint & 0xF) == (int)msgbox)
                                {
                                    e.Result = (Result)result;
                                }
                            }
                        }
                    }
                }
                else // canceled, so always return cancel.
                {
                    e.Result = Result.Cancel;
                }
            }
        }

        private void ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            WixBA.Model.Result = e.Status; // remember the final result of the apply.

            // Set the state to applied or failed unless the state has already been set back to the preapply state
            // which means we need to show the UI as it was before the apply started.
            if (this.root.InstallState != this.root.PreApplyState)
            {
                this.root.InstallState = Hresult.Succeeded(e.Status) ? InstallationState.Applied : InstallationState.Failed;
            }

            // If we're not in Full UI mode, we need to alert the dispatcher to stop and close the window for passive.
            if (Display.Full != WixBA.Model.Command.Display)
            {
                // If its passive, send a message to the window to close.
                if (Display.Passive == WixBA.Model.Command.Display)
                {
                    WixBA.Model.Engine.Log(LogLevel.Verbose, "Automatically closing the window for non-interactive install");
                    WixBA.Dispatcher.BeginInvoke(new Action(WixBA.View.Close));
                }
                else
                {
                    WixBA.Dispatcher.InvokeShutdown();
                }
                return;
            }
            else if (Hresult.Succeeded(e.Status) && LaunchAction.UpdateReplace == WixBA.Model.PlannedAction) // if we successfully applied an update close the window since the new Bundle should be running now.
            {
                WixBA.Model.Engine.Log(LogLevel.Verbose, "Automatically closing the window since update successful.");
                WixBA.Dispatcher.BeginInvoke(new Action(WixBA.View.Close));
                return;
            }
            else if (this.root.AutoClose)
            {
                // Automatically closing since the user clicked the X button.
                WixBA.Dispatcher.BeginInvoke(new Action(WixBA.View.Close));
                return;
            }

            // Force all commands to reevaluate CanExecute.
            // InvalidateRequerySuggested must be run on the UI thread.
            this.root.Dispatcher.Invoke(new Action(CommandManager.InvalidateRequerySuggested));
        }

        private void ParseCommandLine()
        {
            // Get array of arguments based on the system parsing algorithm.
            string[] args = BootstrapperCommand.ParseCommandLineToArgs(WixBA.Model.Command.CommandLine);
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].StartsWith("InstallFolder=", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Allow relative directory paths. Also validates.
                    string[] param = args[i].Split(new char[] { '=' }, 2);
                    this.root.InstallDirectory = IO.Path.Combine(Environment.CurrentDirectory, param[1]);
                }
            }
        }

        private void AddPackageTelemetry(string prefix, string id, double time, int result)
        {
            lock (this)
            {
                string key = String.Format("{0}Time_{1}", prefix, id);
                string value = time.ToString();
                WixBA.Model.Telemetry.Add(new KeyValuePair<string, string>(key, value));

                key = String.Format("{0}Result_{1}", prefix, id);
                value = String.Concat("0x", result.ToString("x"));
                WixBA.Model.Telemetry.Add(new KeyValuePair<string, string>(key, value));
            }
        }
    }
}