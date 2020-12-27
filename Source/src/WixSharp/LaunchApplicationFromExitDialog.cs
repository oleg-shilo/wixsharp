using System;
using System.Xml.Linq;
using WixSharp.Controls;

namespace WixSharp
{
    /// <summary>
    /// Launches the application after the finish, if the corresponding checkbox is selected.
    /// </summary>
    ///
    /// <example>
    /// <code>
    /// new LaunchApplicationFromExitDialog("EXE_ID", "Launch app")
    /// </code>
    /// </example>
    public class LaunchApplicationFromExitDialog : WixEntity, IGenericEntity
    {
        #region Properties

        /// <summary>
        /// CheckBox text. <br/>
        /// Default value is <value>"Launch"</value>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Exe ID.
        /// </summary>
        public string ExeId { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Launches the application after the finish, if the corresponding checkbox is selected.
        /// </summary>
        /// <param name="exeId"></param>
        /// <param name="description"></param>
        public LaunchApplicationFromExitDialog(string exeId, string description = "Launch")
        {
            ExeId = exeId ?? throw new ArgumentNullException(nameof(exeId));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.UI);
            context.Project.Include(WixExtension.Util);

            var project = context.Project as Project ??
                          throw new InvalidOperationException("LaunchApplicationFromExitDialog works only with Projects");

            if (project.CustomUI == null)
                throw new InvalidOperationException("LaunchApplicationFromExitDialog can only work with Projects with native custom UI (project.CustomUI)");

            project.CustomUI
                .On(NativeDialogs.ExitDialog,
                    Buttons.Finish,
                    new ExecuteCustomAction(
                        "LaunchApplication",
                        "WIXUI_EXITDIALOGOPTIONALCHECKBOX = 1 and NOT Installed"));

            context.XParent
                .Add(new XElement("Property")
                    .SetAttribute("Id", "WIXUI_EXITDIALOGOPTIONALCHECKBOXTEXT")
                    .SetAttribute("Value", Description));
            context.XParent
                .Add(new XElement("Property")
                    .SetAttribute("Id", "WIXUI_EXITDIALOGOPTIONALCHECKBOX")
                    .SetAttribute("Value", "1"));

            context.XParent
                .Add(new XElement("Property")
                    .SetAttribute("Id", "WixShellExecTarget")
                    .SetAttribute("Value", $"[#{ExeId}]"));
            context.XParent
                .Add(new XElement("CustomAction")
                    .SetAttribute("Id", "LaunchApplication")
                    .SetAttribute("BinaryKey", "WixCA")
                    .SetAttribute("DllEntry", "WixShellExec")
                    .SetAttribute("Impersonate", "yes"));
        }

        #endregion Methods
    }
}