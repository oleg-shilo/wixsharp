using System;
using System.Diagnostics;
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
        /// <summary>
        /// CheckBox text. <br/>
        /// Default value is <value>"Launch"</value>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Exe ID.
        /// </summary>
        public string ExeId { get; }

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        public void Process(ProcessingContext context)
        {
            if ((context.Project as ManagedProject)?.ManagedUI != null)
                throw new Exception(
                    this.GetType().Name + " can only be used in with native UI (but not ManagedUI). " +
                    "It is designed to overcome the limitations of the native MSI UI. For the setup that " +
                    "uses Managed UI you can launch the application from the `AfterInstall` event or from the " +
                    "exit dialog definition.\n");

            // Debug.Assert(false);
            context.Project.Include(WixExtension.UI);
            context.Project.Include(WixExtension.Util);

            var project = context.Project as Project ??
                          throw new InvalidOperationException("LaunchApplicationFromExitDialog works only with Projects");

            if (project.CustomUI == null)
            {
                context.XParent.FindFirst("UI")
                    .AddElement("Publish",
                                attributesDefinition:
                                    "Dialog=ExitDialog;" +
                                    "Control=Finish;" +
                                    "Event=DoAction;" +
                                    "Value=LaunchApplication",
                                value:
                                    "WIXUI_EXITDIALOGOPTIONALCHECKBOX = 1 and NOT Installed");
            }
            else
            {
                project.CustomUI
                    .On(NativeDialogs.ExitDialog,
                        Buttons.Finish,
                        new ExecuteCustomAction(
                            "LaunchApplication",
                            "WIXUI_EXITDIALOGOPTIONALCHECKBOX = 1 and NOT Installed"));
            }

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
    }
}