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
    /// new LaunchApplicationAction("EXE_ID")
    /// {
    ///     Description = "Launch app",
    /// }
    /// </code>
    /// </example>
    public class LaunchApplicationAction : WixEntity, IGenericEntity
    {
        #region Properties

        /// <summary>
        /// CheckBox text.
        /// </summary>
        public string Description { get; set; } = "Launch";

        /// <summary>
        /// Exe ID.
        /// </summary>
        public string ExeId { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Launches the application after the finish, if the corresponding checkbox is selected.
        /// </summary>
        /// <param name="exeId"></param>
        public LaunchApplicationAction(string exeId)
        {
            ExeId = exeId ?? throw new ArgumentNullException(nameof(exeId));
        }

        #endregion

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
                          throw new InvalidOperationException("LaunchApplicationAction works only with Projects");
            
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

        #endregion
    }
}