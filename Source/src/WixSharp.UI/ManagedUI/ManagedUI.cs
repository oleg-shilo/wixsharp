using System;
using System.Linq;
using System.Threading;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp.CommonTasks;
using WixSharp.UI.ManagedUI;
using System.Diagnostics;
using WixSharp.UI.Forms;
using System.Xml.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WixSharp
{
    /// <summary>
    /// Implements as standard dialog-based MSI embedded UI. 
    /// <para>
    /// This class allows defining separate sequences of UI dialogs for 'install' 
    /// and 'modify' MSI executions. The dialog sequence can contain any mixture 
    /// of built-in standard dialogs and/or custom dialogs (Form inherited from <see cref="T:WixSharp.UI.Forms.ManagedForm"/>).
    /// </para>
    /// </summary>
    /// <example>The following is an example of installing <c>MyLibrary.dll</c> assembly and registering it in GAC.
    /// <code>
    /// ...
    /// project.ManagedUI = new ManagedUI();
    /// project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
    ///                                 .Add(Dialogs.Licence)
    ///                                 .Add(Dialogs.SetupType)
    ///                                 .Add(Dialogs.Features)
    ///                                 .Add(Dialogs.InstallDir)
    ///                                 .Add(Dialogs.Progress)
    ///                                 .Add(Dialogs.Exit);
    ///
    /// project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
    ///                                .Add(Dialogs.Features)
    ///                                .Add(Dialogs.Progress)
    ///                                .Add(Dialogs.Exit);
    ///
    /// </code>
    /// </example>
    public class ManagedUI : IManagedUI, IEmbeddedUI
    {
        /// <summary>
        /// The default implementation of ManagedUI. It implements all major dialogs of a typical MSI UI.
        /// </summary>
        static public ManagedUI Default = new ManagedUI
        {
            InstallDialogs = new ManagedDialogs()
                                        .Add<WelcomeDialog>()
                                        .Add<LicenceDialog>()
                                        .Add<SetupTypeDialog>()
                                        .Add<FeaturesDialog>()
                                        .Add<InstallDirDialog>()
                                        .Add<ProgressDialog>()
                                        .Add<ExitDialog>(),

            ModifyDialogs = new ManagedDialogs()
                                       .Add<MaintenanceTypeDialog>()
                                       .Add<FeaturesDialog>()
                                       .Add<ProgressDialog>()
                                       .Add<ExitDialog>()
        };

        /// <summary>
        /// The default implementation of ManagedUI with no UI dialogs.
        /// </summary>
        static public ManagedUI Empty = new ManagedUI();

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedUI"/> class.
        /// </summary>
        public ManagedUI()
        {
            InstallDialogs = new ManagedDialogs();
            ModifyDialogs = new ManagedDialogs();
        }

        /// <summary>
        /// This method is called (indirectly) by Wix# compiler just before building the MSI. It allows embedding UI specific resources (e.g. license file, properties)
        /// into the MSI.
        /// </summary>
        /// <param name="project">The project.</param>
        public void BeforeBuild(ManagedProject project)
        {
            var file = LocalizationFileFor(project);
            ValidateUITextFile(file);
            project.AddBinary(new Binary(new Id("WixSharp_UIText"), file));
            project.AddBinary(new Binary(new Id("WixSharp_LicenceFile"), LicenceFileFor(project)));
            project.AddBinary(new Binary(new Id("WixUI_Bmp_Dialog"), DialogBitmapFileFor(project)));
            project.AddBinary(new Binary(new Id("WixUI_Bmp_Banner"), DialogBannerFileFor(project)));
        }

        /// <summary>
        /// Validates the UI text file (localization file) for being compatible with ManagedUI.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="throwOnError">if set to <c>true</c> [throw on error].</param>
        /// <returns></returns>
        public static bool ValidateUITextFile(string file, bool throwOnError = true)
        {
            try
            {
                var data = new ResourcesData();
                data.InitFromWxl(System.IO.File.ReadAllBytes(file));
            }
            catch (Exception e)
            {
                //may need to do extra logging; not important for now
                if (throwOnError)
                    throw new Exception("Localization file is incompatible with ManagedUI.", e);
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Gets or sets the id of the 'installdir' (destination folder) directory. It is the directory,
        /// which is bound to the input UI elements of the Browse dialog (e.g. WiX BrowseDlg, Wix# InstallDirDialog).
        /// </summary>
        /// <value>
        /// The install dir identifier.
        /// </value>
        public string InstallDirId { get; set; }

        internal string LocalizationFileFor(Project project)
        {
            return UIExtensions.UserOrDefaultContentOf(project.LocalizationFile, project.SourceBaseDir, project.OutDir, project.Name + ".wxl", Resources.WixUI_en_us);
        }

        internal string LicenceFileFor(Project project)
        {
            return UIExtensions.UserOrDefaultContentOf(project.LicenceFile, project.SourceBaseDir, project.OutDir, project.Name + ".licence.rtf", Resources.WixSharp_LicenceFile);
        }

        internal string DialogBitmapFileFor(Project project)
        {
            return UIExtensions.UserOrDefaultContentOf(project.BackgroundImage, project.SourceBaseDir, project.OutDir, project.Name + ".dialog_bmp.png", Resources.WixUI_Bmp_Dialog);
        }

        internal string DialogBannerFileFor(Project project)
        {
            return UIExtensions.UserOrDefaultContentOf(project.BannerImage, project.SourceBaseDir, project.OutDir, project.Name + ".dialog_banner.png", Resources.WixUI_Bmp_Banner);
        }

        /// <summary>
        /// Sequence of the dialogs to be displayed during the installation of the product.
        /// </summary>
        public ManagedDialogs InstallDialogs { get; set; }
        /// <summary>
        /// Sequence of the dialogs to be displayed during the customization of the installed product.
        /// </summary>
        public ManagedDialogs ModifyDialogs { get; set; }

        /// <summary>
        /// A window icon that appears in the left top corner of the UI shell window.
        /// </summary>
        public string Icon { get; set; }

        ManualResetEvent uiExitEvent = new ManualResetEvent(false);
        IUIContainer shell;

        void ReadDialogs(Session session)
        {
            InstallDialogs.Clear()
                          .AddRange(ManagedProject.ReadDialogs(session.Property("WixSharp_InstallDialogs")));

            ModifyDialogs.Clear()
                          .AddRange(ManagedProject.ReadDialogs(session.Property("WixSharp_ModifyDialogs")));

        }

        Mutex cancelRequest = null;

        /// <summary>
        /// Initializes the specified session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <param name="uiLevel">The UI level.</param>
        /// <returns></returns>
        /// <exception cref="Microsoft.Deployment.WindowsInstaller.InstallCanceledException"></exception>
        public bool Initialize(Session session, string resourcePath, ref InstallUIOptions uiLevel)
        {
            //System.Diagnostics.Debugger.Launch();
            if (session != null && (session.IsUninstalling() || uiLevel.IsBasic()))
                return false; //use built-in MSI basic UI

            string upgradeCode = session["UpgradeCode"];

            using (cancelRequest)
            {
                try
                {
                    ReadDialogs(session);

                    var startEvent = new ManualResetEvent(false);

                    var uiThread = new Thread(() =>
                    {
                        session["WIXSHARP_MANAGED_UI"] = System.Reflection.Assembly.GetExecutingAssembly().ToString();
                        shell = new UIShell(); //important to create the instance in the same thread that call ShowModal
                        shell.ShowModal(new MsiRuntime(session)
                                        {
                                            StartExecute = () => startEvent.Set(),
                                            CancelExecute = () =>
                                            {
                                                // NOTE: IEmbeddedUI interface has no way to cancel the installation, which has been started 
                                                // (e.g. ProgressDialog is displayed). What is even worse is that UI can pass back to here  
                                                // a signal the user pressed 'Cancel' but nothing we can do with it. Install is already started
                                                // and session object is now invalid. 
                                                // To solve this we use this work around - set a unique "cancel request mutex" form here 
                                                // and ManagedProjectActions.CancelRequestHandler built-in CA will pick the request and yield 
                                                // return code UserExit. 
                                                cancelRequest = new Mutex(true, "WIXSHARP_UI_CANCEL_REQUEST." + upgradeCode);
                                            }
                                        },
                                        this);
                        uiExitEvent.Set();
                    });

                    uiThread.SetApartmentState(ApartmentState.STA);
                    uiThread.Start();

                    int waitResult = WaitHandle.WaitAny(new[] { startEvent, uiExitEvent });
                    if (waitResult == 1)
                    {
                        //UI exited without starting the install. Cancel the installation.
                        throw new InstallCanceledException();
                    }
                    else
                    {
                        // Start the installation with a silenced internal UI.
                        // This "embedded external UI" will handle message types except for source resolution.
                        uiLevel = InstallUIOptions.NoChange | InstallUIOptions.SourceResolutionOnly;
                        shell.OnExecuteStarted();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    session.Log("Cannot attach ManagedUI: " + e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Processes information and progress messages sent to the user interface.
        /// </summary>
        /// <param name="messageType">Message type.</param>
        /// <param name="messageRecord">Record that contains message data.</param>
        /// <param name="buttons">Message buttons.</param>
        /// <param name="icon">Message box icon.</param>
        /// <param name="defaultButton">Message box default button.</param>
        /// <returns>
        /// Result of processing the message.
        /// </returns>
        /// <remarks>
        /// <p>
        /// Win32 MSI API:
        /// <a href="http://msdn.microsoft.com/library/en-us/msi/setup/embeddeduihandler.asp">EmbeddedUIHandler</a></p>
        /// </remarks>
        public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            return shell.ProcessMessage(messageType, messageRecord, buttons, icon, defaultButton);
        }

        /// <summary>
        /// Shuts down the embedded UI at the end of the installation.
        /// </summary>
        /// <remarks>
        /// If the installation was canceled during initialization, this method will not be called.
        /// If the installation was canceled or failed at any later point, this method will be called at the end.
        /// <p>
        /// Win32 MSI API:
        /// <a href="http://msdn.microsoft.com/library/en-us/msi/setup/shutdownembeddedui.asp">ShutdownEmbeddedUI</a></p>
        /// </remarks>
        public void Shutdown()
        {
            shell.OnExecuteComplete();
            uiExitEvent.WaitOne();
        }
    }
}
