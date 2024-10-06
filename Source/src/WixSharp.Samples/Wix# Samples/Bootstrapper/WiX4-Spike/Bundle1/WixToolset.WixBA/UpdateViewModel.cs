// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.WixBA
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using WixToolset.Mba.Core;

    /// <summary>
    /// The states of the update view model.
    /// </summary>
    public enum UpdateState
    {
        Unknown,
        Initializing,
        Checking,
        Current,
        Available,
        Failed,
    }

    /// <summary>
    /// The model of the update view.
    /// </summary>
    public class UpdateViewModel : PropertyNotifyBase
    {
        private RootViewModel root;
        private UpdateState state;
        private ICommand updateCommand;
        private string updateVersion;
        private string updateChanges;


        public UpdateViewModel(RootViewModel root)
        {
            this.root = root;
            WixBA.Model.Bootstrapper.DetectUpdateBegin += this.DetectUpdateBegin;
            WixBA.Model.Bootstrapper.DetectUpdate += this.DetectUpdate;
            WixBA.Model.Bootstrapper.DetectUpdateComplete += this.DetectUpdateComplete;
            WixBA.Model.Bootstrapper.DetectComplete += this.DetectComplete;

            this.root.PropertyChanged += new PropertyChangedEventHandler(this.RootPropertyChanged);

            this.State = UpdateState.Initializing;
        }

        void RootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("InstallState" == e.PropertyName)
            {
                base.OnPropertyChanged("CanUpdate");
            }
        }

        public bool CheckingEnabled
        {
            get { return this.State == UpdateState.Initializing || this.State == UpdateState.Checking; }
        }

        public bool CanUpdate
        {
            get
            {
                switch(this.root.InstallState)
                {
                    case InstallationState.Waiting:
                    case InstallationState.Applied:
                    case InstallationState.Failed:
                        return this.IsUpdateAvailable;
                    default:
                        return false;
                }
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                if (this.updateCommand == null)
                {
                    this.updateCommand = new RelayCommand(param => WixBA.Plan(LaunchAction.UpdateReplace), param => this.CanUpdate);
                }

                return this.updateCommand;
            }
        }

        public bool IsUpdateAvailable
        {
            get { return this.State == UpdateState.Available; }
        }

        /// <summary>
        /// Gets and sets the state of the update view model.
        /// </summary>
        public UpdateState State
        {
            get
            {
                return this.state;
            }

            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    base.OnPropertyChanged("State");
                    base.OnPropertyChanged("CanUpdate");
                    base.OnPropertyChanged("CheckingEnabled");
                    base.OnPropertyChanged("IsUpdateAvailable");
                }
            }
        }
        /// <summary>
        /// The version of an available update.
        /// </summary>
        public string UpdateVersion
        {
            get
            {
                return updateVersion;
            }
            set
            {
                if (this.updateVersion != value)
                {
                    this.updateVersion = value;
                    base.OnPropertyChanged("UpdateVersion");
                }
            }
        }

        /// <summary>
        /// The changes in the available update.
        /// </summary>
        public string UpdateChanges
        {
            get
            {
                return updateChanges;
            }
            set
            {
                if (this.updateChanges != value)
                {
                    this.updateChanges = value;
                    base.OnPropertyChanged("UpdateChanges");
                }
            }
        }

        private void DetectUpdateBegin(object sender, DetectUpdateBeginEventArgs e)
        {
            // Don't check for updates if:
            //   the first check failed (no retry)
            //   if we are being run as an uninstall
            //   if we are not under a full UI.
            if ((UpdateState.Failed != this.State) && (LaunchAction.Uninstall != WixBA.Model.Command.Action) && (Display.Full == WixBA.Model.Command.Display))
            {
                this.State = UpdateState.Checking;
                e.Skip = false;
            }
        }
        
        private void DetectUpdate(object sender, DetectUpdateEventArgs e)
        {
            // The list of updates is sorted in descending version, so the first callback should be the largest update available.
            // This update should be either larger than ours (so we are out of date), the same as ours (so we are current)
            // or smaller than ours (we have a private build). If we really wanted to, we could leave the e.StopProcessingUpdates alone and
            // enumerate all of the updates.
            WixBA.Model.Engine.Log(LogLevel.Verbose, String.Format("Potential update v{0} from '{1}'; current version: v{2}", e.Version, e.UpdateLocation, WixBA.Model.Version));
            if (WixBA.Model.Engine.CompareVersions(e.Version, WixBA.Model.Version) > 0)
            {
                WixBA.Model.Engine.SetUpdate(null, e.UpdateLocation, e.Size, UpdateHashType.None, null);
                this.UpdateVersion = String.Concat("v", e.Version.ToString());
                string changesFormat = @"<body style='overflow: auto;'>{0}</body>";
                this.UpdateChanges = String.Format(changesFormat, e.Content);
                this.State = UpdateState.Available;
            }
            else
            {
                this.State = UpdateState.Current;
            }
            e.StopProcessingUpdates = true;
        }

        private void DetectUpdateComplete(object sender, DetectUpdateCompleteEventArgs e)
        {
            // Failed to process an update, allow the existing bundle to still install.
            if ((UpdateState.Failed != this.State) && !Hresult.Succeeded(e.Status))
            {
                this.State = UpdateState.Failed;
                WixBA.Model.Engine.Log(LogLevel.Verbose, String.Format("Failed to locate an update, status of 0x{0:X8}, updates disabled.", e.Status));
                e.IgnoreError = true;
            }
            // If we are uninstalling, we don't want to check or show an update
            // If we are checking, then the feed didn't find any valid enclosures
            // If we are initializing, we're either uninstalling or not a full UI
            else if ((LaunchAction.Uninstall == WixBA.Model.Command.Action) || (UpdateState.Initializing == this.State) || (UpdateState.Checking == this.State))
            {
                this.State = UpdateState.Unknown;
            }
        }

        private void DetectComplete(object sender, DetectCompleteEventArgs e)
        {
            if (this.State == UpdateState.Initializing || this.State == UpdateState.Checking)
            {
                this.State = UpdateState.Unknown;
            }
        }
    }
}
