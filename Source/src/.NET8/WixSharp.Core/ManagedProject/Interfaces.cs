using System;
using System.Collections.Generic;
using System.Linq;
using WixToolset.Dtf.WindowsInstaller;
using Reflection = System.Reflection;

#pragma warning disable 1591

namespace WixSharp
{
    /// <summary>
    /// Interface of a typical UI dialog for reflecting the installation progress. It is functionally a typical
    /// <see cref="T:WixSharp.IManagedDialog"/> except that it initiates the MSI execution by calling
    /// <see cref="T:WixSharp.IManagedUIShell.StartExecute"/> on loading the dialog. ManagedUI dialogs sequence
    /// should have only a single dialog of this type.
    /// </summary>
    public interface IProgressDialog : IManagedDialog
    {
    }

    public interface IDialog
    {
    }

    public interface IWpfDialogHost
    {
        void SetDialogContent(IWpfDialog content);
    }

    public class CustomDialogWith<T2> : IDialog { }

    /// <summary>
    /// A custom WPF UI dialog interface.
    /// </summary>
    /// <seealso cref="WixSharp.IDialog" />
    public interface IWpfDialog : IDialog
    {
        /// <summary>
        /// Gets or sets the reference to the WinForm host (parent) of the CustomDialog content. This member is set by WixSHarp runtime.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        IManagedDialog Host { get; set; }

        /// <summary>
        /// This method is invoked by WixSHarp runtime when the custom dialog content is internally fully initialized.
        /// This is a convenient place to do further initialization activities (e.g. localization).
        /// </summary>
        void Init();
    }

    /// <summary>
    /// Interface of a typical UI dialog managed by shell (the main window) of the MSI external/embedded UI.
    /// </summary>
    public interface IManagedDialog : IDialog
    {
        /// <summary>
        /// Gets or sets the UI shell (main UI window). This property is set the ManagedUI runtime (IManagedUI).
        /// On the other hand it is consumed (accessed) by the UI dialog (IManagedDialog).
        /// </summary>
        /// <value>
        /// The shell.
        /// </value>
        IManagedUIShell Shell { get; set; }

        /// <summary>
        /// Processes information and progress messages sent to the user interface.
        /// <para> This method directly mapped to the
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI.ProcessMessage"/>.</para>
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
        MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);

        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
        void OnExecuteComplete();

        /// <summary>
        /// Called when MSI execute started.
        /// </summary>
        void OnExecuteStarted();

        /// <summary>
        /// Called when MSI execution progress is changed.
        /// </summary>
        /// <param name="progressPercentage">The progress percentage.</param>
        void OnProgress(int progressPercentage);
    }

    /// <summary>
    /// The interface representing main window of the embedded UI.
    /// </summary>
    public interface IShellView
    {
        /// <summary>
        /// Sets the size of the main window of the embedded UI.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        void SetSize(int width, int height);

        /// <summary>
        /// Occurs when on current dialog changed (e.g. because of 'Next' navigation).
        /// </summary>
        event Action<IManagedDialog> OnCurrentDialogChanged;

        /// <summary>
        /// Gets the current dialog of the UI sequence.
        /// </summary>
        /// <value>The current dialog.</value>
        IManagedDialog CurrentDialog { get; }

        IManagedUIShell Shell { get; }
    }

    /// <summary>
    /// Interface of the main window implementation of the MSI external/embedded UI. This interface is designed to be
    /// used by the Wix#/MSI UI dialogs. It is the interface that is directly available for all UI dialogs and it
    /// allows the dialogs accessing the MSI runtime context.
    /// </summary>
    public interface IManagedUIShell
    {
        /// <summary>
        /// Gets the runtime context object. Typically this object is of the <see cref="T:WixSharp.MsiRuntime"/> type.
        /// </summary>
        /// <value>
        /// The runtime context.
        /// </value>
        object RuntimeContext { get; }

        /// <summary>
        /// Gets the MSI log text.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        string Log { get; }

        /// <summary>
        /// Gets a value indicating whether the MSI session was interrupted (canceled) by user.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it was user interrupted; otherwise, <c>false</c>.
        /// </value>
        bool UserInterrupted { get; set; }

        /// <summary>
        /// Gets the MSI installation errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        List<string> Errors { get; }

        /// <summary>
        /// Gets a value indicating whether MSI session ended with error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if error was detected; otherwise, <c>false</c>.
        /// </value>
        bool ErrorDetected { get; set; }

        /// <summary>
        /// Gets or sets the custom error description to be displayed in the ExitDialog
        /// in case of <see cref="IManagedUIShell.ErrorDetected"/> being set to <c>true</c>.
        /// </summary>
        /// <value>
        /// The custom error description.
        /// </value>
        string CustomErrorDescription { get; set; }

        /// <summary>
        /// Gets the sequence of the UI dialogs specific for the current setup type (e.g. install vs. modify).
        /// </summary>
        /// <value>
        /// The dialogs.
        /// </value>
        ManagedDialogs Dialogs { get; }

        IManagedDialog MessageDialog { get; set; }

        /// <summary>
        /// Proceeds to the next UI dialog.
        /// </summary>
        void GoNext();

        /// <summary>
        /// Moves to the previous UI Dialog.
        /// </summary>
        void GoPrev();

        /// <summary>
        /// Moves to the UI Dialog by the specified index in the <see cref="T:WixSharp.IManagedUIShell.Dialogs"/> sequence.
        /// </summary>
        /// <param name="index">The index.</param>
        void GoTo(int index);

        /// <summary>
        /// Moves to the UI Dialog by the specified index in the <see cref="T:WixSharp.IManagedUIShell.Dialogs"/> sequence.
        /// </summary>
        void GoTo<T>();

        /// <summary>
        /// Cancels the MSI installation.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Exits this MSI UI application.
        /// </summary>
        void Exit();

        /// <summary>
        /// Starts the execution of the MSI installation.
        /// </summary>
        void StartExecute();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is demo mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is demo mode; otherwise, <c>false</c>.
        /// </value>
        bool IsDemoMode { get; set; }
    }

    /// <summary>
    /// Interface for an embedded external user interface implementing Wix# ManagedUI architecture (ManagedUI runtime).
    /// <para>The interface itself is reasonable simple and it basically implements the
    /// collection/sequence of the runtime UI dialogs with a couple of methods for integrating the ManagedUI
    /// with the MSI.</para>
    /// </summary>
    public interface IManagedUI
    {
        /// <summary>
        /// Gets or sets the id of the 'installdir' (destination folder) directory. It is the directory,
        /// which is bound to the input UI elements of the Browse dialog (e.g. WiX BrowseDlg, Wix# InstallDirDialog).
        /// </summary>
        /// <value>
        /// The install dir identifier.
        /// </value>
        string InstallDirId { get; set; }

        /// <summary>
        /// A window icon that appears in the left top corner of the UI shell window.
        /// </summary>
        string Icon { get; set; }

        /// <summary>
        /// Sequence of the dialogs to be displayed during the installation of the product.
        /// </summary>
        ManagedDialogs InstallDialogs { get; }

        /// <summary>
        /// Sequence of the dialogs to be displayed during the customization of the installed product.
        /// </summary>
        ManagedDialogs ModifyDialogs { get; }

        /// <summary>
        /// This method is called (indirectly) by Wix# compiler just before building the MSI. It allows embedding UI specific resources (e.g. license file, properties)
        /// into the MSI.
        /// </summary>
        /// <param name="project">The project.</param>
        void BeforeBuild(ManagedProject project);
    }

    /// <summary>
    /// Customized version of 'List&lt;Type&gt;', containing Fluent extension methods
    /// </summary>
    public class ManagedDialogs : List<Type>
    {
        /// <summary>
        /// Adds an typeof(T) object to the end of the collections.
        /// </summary>
        /// <typeparam name="T">Type implementing ManagedUI dialog</typeparam>
        /// <returns></returns>
        public ManagedDialogs Add<T>() where T : IDialog
        {
            base.Add(Validate(typeof(T)));
            return this;
        }

        /// <summary>
        /// Indexes the of dialog implementing specified interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int IndexOfDialogImplementing<T>()
            => this.FindIndex(x => typeof(T).IsAssignableFrom(x));

        Type Validate(Type type)
        {
            if (type.GetGenericTypeBaseName() == "WixSharp.CustomDialogWith")
            {
                var userContentInterfaceName = "WixSharp.UI.WPF.IWpfDialogContent";

                if (type.GenericTypeArguments.Any(t => t.Implements(userContentInterfaceName)))
                {
                    var userContentHostDependencies = (Reflection.AssemblyName[])
                        Reflection.Assembly.Load("WixSharp.UI.WPF")
                        .GetType("WixSharp.UI.WPF.DependencyDescriptor")
                        .GetMethod("GetRefAssemblies")
                        .Invoke(null, new object[0]);

                    var userContentDependencies = type.GenericTypeArguments
                                                      .Where(t => t.Implements(userContentInterfaceName))
                                                      .SelectMany(t =>
                                                      {
                                                          var asms = new List<System.Reflection.AssemblyName>();
                                                          asms.Add(t.Assembly.GetName());
                                                          asms.AddRange(t.Assembly.GetWixSharpDependencies());
                                                          return asms;
                                                      });

                    indirectRefAssemblies.AddRange(userContentHostDependencies);
                    indirectRefAssemblies.AddRange(userContentDependencies);
                }
                else
                {
                    if (type.GenericTypeArguments.Count() == 1)
                        throw new ValidationException($"Error: The generic type argument {type.GenericTypeArguments.First()} must implement {userContentInterfaceName} interface");
                    else
                        throw new ValidationException($"Error: The generic type argument of {type} must implement {userContentInterfaceName} interface");
                }
            }
            return type;
        }

        /// <summary>
        /// Adds an Type object to the end of the collections.
        /// </summary>
        /// <param name="type">Type implementing ManagedUI dialog.</param>
        /// <returns></returns>
        public new ManagedDialogs Add(Type type)
        {
            base.Add(Validate(type));
            return this;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        /// <returns></returns>
        public new ManagedDialogs Clear()
        {
            base.Clear();
            return this;
        }

        List<System.Reflection.AssemblyName> indirectRefAssemblies = new List<System.Reflection.AssemblyName>();

        public string[] Assemblies
            => this.SelectMany(x => x.Assembly.GetReferencedAssemblies())
                   .Where(a => a.Name.StartsWith("WixSharp.") || a.Name.StartsWith("Cliburn."))
                   .Concat(indirectRefAssemblies)
                   .Select(a => System.Reflection.Assembly.Load(a.FullName))
                   .Select(a => a.Location)
                   .Distinct()
                   .ToArray();
    }
}