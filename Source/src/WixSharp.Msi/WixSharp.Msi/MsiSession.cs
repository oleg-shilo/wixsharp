using System;

using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using WindowsInstaller;

#pragma warning disable CS8981
using sys = System;

#pragma warning disable 1591
#pragma warning disable CA1416 // Validate platform compatibility

namespace WixSharp.UI
{
    /// <summary>
    /// Generic class that represents runtime properties of the MSI setup session as well as some <c>runtime</c>
    /// properties of the product being installed (e.g. CodePage, Caption). It also Simplifies MSI execution
    /// and provides automatic responses on the MSI Messages.
    /// <para>
    /// Normally <c>MsiSession</c> should be extended top meet the needs of the product (MSI) specific setup.</para>
    /// </summary>
    public class MsiSession : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when setup progress changed.
        /// </summary>
        public event EventHandler ProgressChanged;

        /// <summary>
        /// Occurs when setup completes.
        /// </summary>
        public event sys.Action SetupComplete;

        /// <summary>
        /// Occurs when setup starts.
        /// </summary>
        public event sys.Action SetupStarted;

        /// <summary>
        /// Occurs when new setup action started.
        /// </summary>
        public event EventHandler ActionStarted;

        /// <summary>
        /// The UI thread marshalling delegate. It should be set for the environments where cross-thread calls
        /// must be marshalled (e.g. WPF, WinForms). Not needed otherwise (e.g. Console application).
        /// </summary>
        public sys.Action<sys.Action> InUiThread = (action) => action();

        void NotifyHostOnProgress()
        {
            InUiThread(() =>
                {
                    if (ProgressChanged != null)
                        ProgressChanged(this, EventArgs.Empty);
                });
        }

        void NotifyHostOnActionStarted()
        {
            InUiThread(() =>
                {
                    if (ActionStarted != null)
                        ActionStarted(this, EventArgs.Empty);
                });
        }

        int progressTotal = 100;

        /// <summary>
        /// Gets or sets the progress total.
        /// </summary>
        /// <value>The progress total.</value>
        public int ProgressTotal
        {
            get { return progressTotal; }
            protected set
            {
                progressTotal = value;
                OnPropertyChanged("ProgressTotal");
                NotifyHostOnProgress();
            }
        }

        int progressCurrentPosition = 0;

        /// <summary>
        /// Gets or sets the progress current position.
        /// </summary>
        /// <value>The progress current position.</value>
        public int ProgressCurrentPosition
        {
            get { return progressCurrentPosition; }
            protected set
            {
                Thread.Sleep(ProgressStepDelay);

                progressCurrentPosition = value;
                OnPropertyChanged("ProgressCurrentPosition");
                NotifyHostOnProgress();
            }
        }

        /// <summary>
        /// The progress step delay. It is a "for-testing" feature. Set it to positive value (number of milliseconds)
        /// to artificially slow down the installation process. The default value is 0.
        /// </summary>
        public static int ProgressStepDelay = 0;

        string currentActionName = null;

        /// <summary>
        /// Gets or sets the name of the current action.
        /// </summary>
        /// <value>
        /// The name of the current action.
        /// </value>
        public string CurrentActionName
        {
            get { return currentActionName; }
            protected set
            {
                var newValue = (value ?? "").Trim();

                currentActionName = newValue;
                OnPropertyChanged("CurrentActionName");

                //MSI Runtime can periodically fire null-CurrentAction
                if (!string.IsNullOrEmpty(currentActionName))
                    NotifyHostOnActionStarted();
            }
        }

        int language;

        /// <summary>
        /// Gets or sets the product language.
        /// </summary>
        /// <value>The language.</value>
        public int Language
        {
            get { return language; }
            protected set
            {
                language = value;
                OnPropertyChanged("Language");
            }
        }

        int codePage;

        /// <summary>
        /// Gets or sets the product CodePage.
        /// </summary>
        /// <value>The product CodePage.</value>
        public int CodePage
        {
            get { return codePage; }
            protected set
            {
                codePage = value;
                OnPropertyChanged("CodePage");
            }
        }

        bool canCancel;

        /// <summary>
        /// Gets or sets the flag indication the the user can cancel the setup in progress.
        /// </summary>
        /// <value>The can cancel.</value>
        public bool CanCancel
        {
            get { return canCancel; }
            protected set
            {
                canCancel = value;
                OnPropertyChanged("CanCancel");
            }
        }

        string caption;

        /// <summary>
        /// Gets or sets the setup window caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get { return caption; }
            protected set
            {
                caption = value;
                OnPropertyChanged("Caption");
            }
        }

        int ticksPerActionDataMessage;

        protected int TicksPerActionDataMessage
        {
            get { return ticksPerActionDataMessage; }
            set
            {
                ticksPerActionDataMessage = value;
                OnPropertyChanged("TicksPerActionDataMessage");
            }
        }

        bool isProgressForwardDirection;

        /// <summary>
        /// Gets or sets a value indicating whether the progress steps are changing in the forward direction.
        /// </summary>
        /// <value>
        /// <c>true</c> if the progress changes are in forward direction; otherwise, <c>false</c>.
        /// </value>
        public bool IsProgressForwardDirection
        {
            get { return isProgressForwardDirection; }
            protected set
            {
                isProgressForwardDirection = value;
                OnPropertyChanged("IsProgressForwardDirection");
            }
        }

        bool isProgressTimeEstimationAccurate;

        protected bool IsProgressTimeEstimationAccurate
        {
            get { return isProgressTimeEstimationAccurate; }
            set
            {
                isProgressTimeEstimationAccurate = value;
                OnPropertyChanged("IsProgressTimeEstimationAccurate");
            }
        }

        bool cancelRequested;

        /// <summary>
        /// Gets or sets the CancelRequested flag. It should beset to <c>true</c> if user wants to cancel the setup in progress.
        /// </summary>
        /// <value>The CancelRequested value.</value>
        public bool CancelRequested
        {
            get { return cancelRequested; }
            set
            {
                cancelRequested = value;
                OnPropertyChanged("CancelRequested");
            }
        }

        /// <summary>
        /// Called when ActionData MSI message is received.
        /// </summary>
        /// <param name="data">The message data.</param>
        public virtual void OnActionData(string data)
        {
            //if (!string.IsNullOrWhiteSpace(data))
            //    Debug.WriteLine("ActionData>\t\t" + data);
        }

        /// <summary>
        /// Called when Error event occurs (MSI Error message is received or an internal error condition triggered).
        /// </summary>
        /// <param name="data">The message data.</param>
        /// <param name="fatal">if set to <c>true</c> the error is fatal.</param>
        /// <param name="relatedMessageType">Type of the related message. Note the error may be associated with the internal
        /// error condition (e.g. exception is raised). </param>
        public virtual void OnError(string data, bool fatal, MsiInstallMessage? relatedMessageType = null)
        {
            //Debug.WriteLine("Error>\t\t" + data);
        }

        /// <summary>
        /// Called when Warning MSI message is received.
        /// </summary>
        /// <param name="data">The message data.</param>
        public virtual void OnWarning(string data)
        {
            //Debug.WriteLine("Warning>\t\t" + data);
        }

        /// <summary>
        /// Called when User MSI message is received.
        /// </summary>
        /// <param name="data">The message data.</param>
        public virtual void OnUser(string data)
        {
            //Debug.WriteLine("User>\t\t" + data);
        }

        /// <summary>
        /// Called when Info MSI message is received.
        /// </summary>
        /// <param name="data">The message data.</param>
        public virtual void OnInfo(string data)
        {
            //Debug.WriteLine("Info>\t\t" + data);
        }

        /// <summary>
        /// Enables the MSI runtime logging to the specified log file.
        /// </summary>
        /// <param name="logFile">The log file.</param>
        /// <param name="mode">The logging mode.</param>
        public void EnableLog(string logFile, MsiInstallLogMode mode = MsiInstallLogMode.Info | MsiInstallLogMode.Progress | MsiInstallLogMode.PropertyDump |
                                                                       MsiInstallLogMode.Error | MsiInstallLogMode.User | MsiInstallLogMode.ActionData)
        {
            MsiInterop.MsiEnableLog(mode, logFile, MsiLogAttribute.FlushEachLine);
        }

        /// <summary>
        /// Executes the install sequence from the specified MSI file.
        /// </summary>
        /// <param name="msiFile">The MSI file.</param>
        /// <param name="msiParams">The MSI params.</param>
        public void ExecuteInstall(string msiFile, string msiParams = null)
        {
            Execute(msiFile, msiParams ?? "");
        }

        /// <summary>
        /// Executes the uninstall sequence from the specified MSI file.
        /// </summary>
        /// <param name="msiFile">The MSI file.</param>
        /// <param name="msiParams">The MSI params.</param>
        public void ExecuteUninstall(string msiFile, string msiParams = null)
        {
            Execute(msiFile, "REMOVE=ALL " + (msiParams ?? ""));
        }

        /// <summary>
        /// Executes the MSI file with the specified MSI parameters.
        /// </summary>
        /// <param name="msiFile">The MSI file.</param>
        /// <param name="msiParams">The MSI parameters.</param>
        /// <exception cref="System.ApplicationException"></exception>
        public void Execute(string msiFile, string msiParams)
        {
            MsiInstallUIHandler uiHandler = null;

            IntPtr parent = IntPtr.Zero;
            var oldLevel = MsiInterop.MsiSetInternalUI(MsiInstallUILevel.None | MsiInstallUILevel.SourceResOnly, ref parent);
            MsiInstallUIHandler oldHandler = null;
            try
            {
                uiHandler = new MsiInstallUIHandler(OnExternalUI); //must be kept alive until the end of the MsiInstallProduct call

                if (SetupStarted != null)
                    InUiThread(SetupStarted);

                oldHandler = MsiInterop.MsiSetExternalUI(uiHandler, MsiInstallLogMode.ExternalUI, IntPtr.Zero);

                MsiError ret = MsiInterop.MsiInstallProduct(msiFile, msiParams);

                CurrentActionName = "";

                this.MsiErrorCode = (int)ret;

                if (ret != MsiError.NoError)
                {
                    Console.WriteLine(string.Format("Failed to install -- {0}", ret));

                    //(ret==ProductVersion) Another version of this product is already installed

                    throw new ApplicationException(string.Format("Failed to install -- {0}", ret));
                }
            }
            catch (Exception e)
            {
                OnError("Application initialization error: " + e.ToString(), false);
                CurrentActionName = "";
                //   do something meaningful
                throw;
            }
            finally
            {
                if (oldHandler != null)
                {
                    MsiInterop.MsiSetExternalUI(oldHandler, MsiInstallLogMode.None, IntPtr.Zero);
                    oldHandler = null;
                }

                //It is important to reference uiHandler here to keep it alive till the end.
                //The debug build is more forgiving and referencing uiHandler is not essential as the code is not optimized
                if (uiHandler != null)
                {
                    //see https://wixsharp.codeplex.com/discussions/647701 for details
                    Environment.SetEnvironmentVariable("ReasonForThis", "IHadToDoSomethingThatJITWouldNotOptimiseThis");
                    uiHandler = null;
                }

                MsiInterop.MsiSetInternalUI(oldLevel, ref parent);

                if (SetupComplete != null)
                    InUiThread(SetupComplete);
            }
        }

        int OnExternalUI(IntPtr context, uint messageType, string message)
        {
            return OnMessage(message, messageType);
        }

        /// <summary>
        /// Called when MSI message is received. It is actual the MSI <c>Message Loop</c>.
        /// </summary>
        /// <param name="message">The message data.</param>
        /// <param name="messageType">Type of the message. This value get's combined with MessageIcon and a MessageButtons values. Use
        /// <see cref="MsiInterop.MessageTypeMask"/> to extract the <see cref="WindowsInstaller.MsiInstallMessage"/> value.
        /// </param>
        /// <returns>The integer as per MSI documentation.</returns>
        protected virtual int OnMessage(string message, uint messageType)
        {
            MsiInstallMessage msgType = (MsiInstallMessage)(MsiInterop.MessageTypeMask & messageType);
            return OnMessage(message, msgType);
        }

        /// <summary>
        /// Called when MSI message is received. It is actual the MSI <c>Message Loop</c>.
        /// </summary>
        /// <param name="message">The message data.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>The integer as per MSI documentation.</returns>
        protected virtual int OnMessage(string message, MsiInstallMessage messageType)
        {
            try
            {
                switch (messageType)
                {
                    case MsiInstallMessage.ActionData:
                        this.OnActionData(message);
                        return (int)DialogResult.OK;

                    case MsiInstallMessage.ActionStart:
                        this.CurrentActionName = message.Substring(message.LastIndexOf(".") + 1);
                        return (int)DialogResult.OK;

                    case MsiInstallMessage.CommonData:
                        string[] data = MsiParser.ParseCommonData(message);

                        if (data != null && data[0] != null)
                        {
                            switch (data.MSI<int>(1))
                            {
                                case 0:   //   language
                                    {
                                        Language = data.MSI<int>(2);
                                        CodePage = data.MSI<int>(3);
                                    }
                                    break;

                                case 1:   //   caption
                                    {
                                        Caption = data.MSI<string>(2);
                                    }
                                    break;

                                case 2:   //   CancelShow
                                    {
                                        CanCancel = data.MSI<int>(2) == 1;
                                    }
                                    break;

                                default: break;
                            }
                        }

                        return (int)DialogResult.OK;

                    case MsiInstallMessage.Error:
                        OnError(message, false, MsiInstallMessage.Error);
                        return 1;

                    case MsiInstallMessage.FatalExit:
                        OnError(message, true, MsiInstallMessage.FatalExit);
                        return 1;

                    case MsiInstallMessage.FilesInUse:

                        //   display in use files in a dialog, informing the user
                        //   that they should close whatever applications are using
                        //   them.  You must return the DialogResult to the service
                        //   if displayed.
                        {
                            //If locked files need to be reported to the user then MsiSetExternalUIRecord should be used
                            OnError("Files in use", true, MsiInstallMessage.FilesInUse);
                            return 0;   //   we didn't handle it in this case!
                        }

                    case MsiInstallMessage.Info:
                        this.OnInfo(message);
                        return 1;

                    case MsiInstallMessage.Initialize:

                        return 1;

                    case MsiInstallMessage.OutOfDiskSpace:
                        {
                            OnError("Out Of Disk Space", true, MsiInstallMessage.OutOfDiskSpace);
                            break;
                        }

                    case MsiInstallMessage.Progress:
                        {
                            string[] fields = MsiParser.ParseProgressString(message);

                            if (null == fields || null == fields[0])
                            {
                                return (int)DialogResult.OK;
                            }

                            switch (fields.MSI<int>(1))
                            {
                                case 0:   //   reset progress bar
                                    {
                                        ProgressTotal = fields.MSI<int>(2);
                                        IsProgressForwardDirection = fields.MSI<int>(3) == 0;
                                        IsProgressTimeEstimationAccurate = fields.MSI<int>(4) == 0;
                                        ProgressCurrentPosition = IsProgressForwardDirection ? 0 : ProgressTotal;
                                    }
                                    break;

                                case 1:   //   action info
                                    {
                                        if (this.ProgressTotal == 0)
                                            return (int)DialogResult.OK; //The external handler should not act upon any of these messages until the first a Reset progress message is received.

                                        if (fields.MSI<int>(3) == 1)
                                            TicksPerActionDataMessage = fields.MSI<int>(2);
                                    }
                                    break;

                                case 2:   //   progress
                                    {
                                        if (this.ProgressTotal == 0)
                                            return (int)DialogResult.OK; //The external handler should not act upon any of these messages until the first a Reset progress message is received.

                                        if (this.ProgressTotal != 0) //initialized
                                        {
                                            if (IsProgressForwardDirection)
                                                ProgressCurrentPosition = ProgressCurrentPosition + fields.MSI<int>(2);
                                            else
                                                ProgressCurrentPosition = ProgressCurrentPosition - fields.MSI<int>(2);
                                        }
                                    }
                                    break;

                                default: break;
                            }

                            if (this.CancelRequested)
                                return (int)DialogResult.Cancel;
                            else
                                return (int)DialogResult.OK;
                        }

                    case MsiInstallMessage.ResolveSource:
                        return 0;

                    case MsiInstallMessage.ShowDialog:
                        return (int)DialogResult.OK;

                    case MsiInstallMessage.Terminate:
                        return (int)DialogResult.OK;

                    case MsiInstallMessage.User:
                        OnUser(message);
                        return 1;

                    case MsiInstallMessage.Warning:
                        OnWarning(message);
                        return 1;

                    default: break;
                }
            }
            catch (Exception e)
            {
                //   do something meaningful, but don't re-throw here.
                OnError("Application error: " + e.ToString(), false);
            }

            return 0;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                InUiThread(() =>
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        int msiErrorCode;

        /// <summary>
        /// Gets or sets the error code of the last MSI install action (e.g. MsiInterop.MsiInstallProduct call).
        /// </summary>
        /// <value>
        /// The error status.
        /// </value>
        public int MsiErrorCode
        {
            get { return msiErrorCode; }
            set
            {
                msiErrorCode = value;
                OnPropertyChanged("MsiErrorCode");
            }
        }

        /// <summary>
        /// Occurs when some of the current instance property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}