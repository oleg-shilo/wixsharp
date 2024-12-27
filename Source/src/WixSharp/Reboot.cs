#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2016 Oleg Shilo

Permission is hereby granted,
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#endregion Licence...

using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Defines parent install sequence elements for <c>ScheduleReboot</c> and <c>ForceReboot</c> elements
    /// </summary>
    public enum RebootInstallSequence
    {
        /// <summary>
        /// InstallExecuteSequence
        /// </summary>
        InstallExecute,

        /// <summary>
        /// InstallUISequence
        /// </summary>
        InstallUI,

        /// <summary>
        /// The both InstallExecuteSequence and InstallUISequence
        /// </summary>
        Both
    }

    /// <summary>
    /// Value of the <c>REBOOT</c> property.
    /// The REBOOT property suppresses certain prompts for a restart of the system. An administrator typically uses this
    /// property with a series of installations to install several products at the same time with only one restart at the
    /// end. For more information, see System Reboots.
    /// <para>
    /// The ForceReboot and ScheduleReboot actions inform the installer to prompt the user to restart the system. The
    /// installer can also determine that a restart is necessary whether there are any ForceReboot or ScheduleReboot actions
    /// in the sequence. For example, the installer automatically prompts for a restart if it needs to replace any files in
    /// use during the installation.
    /// </para>
    /// </summary>
    public enum RebootSupressing
    {
        /// <summary>
        /// Always prompt for a restart at the end of the installation. The UI always prompts the user with an option to
        /// restart at the end. If there is no user interface, and this is not a multiple-package installation, the system
        /// automatically restarts at the end of the installation. If this is a multiple-package installation, there is no
        /// automatic restart of the system and the installer returns ERROR_SUCCESS_REBOOT_REQUIRED.
        /// </summary>
        Force,

        /// <summary>
        /// Suppress prompts for a restart at the end of the installation. The installer still prompts the user with an
        /// option to restart during the installation whenever it encounters the ForceReboot action. If there is no user
        /// interface, the system automatically restarts at each ForceReboot. Restarts at the end of the installation
        /// (for example, caused by an attempt to install a file in use) are suppressed.
        /// </summary>
        Suppress,

        /// <summary>
        /// Suppress all restarts and restart prompts initiated by ForceReboot during the installation. Suppress all restarts
        /// and restart prompts at the end of the installation. Both the restart prompt and the restart itself are suppressed.
        /// For example, restarts at the end of the installation, caused by an attempt to install a file in use, are suppressed.
        /// </summary>
        ReallySuppress
    }

    /// <summary>
    /// Prompts the user for a restart of the system during the installation. Special actions don't have a built-in sequence
    /// number and thus must appear relative to another action. The suggested way to do this is by using the Before or After
    /// attribute.
    /// InstallExecute and InstallExecuteAgain can optionally appear anywhere between InstallInitialize and InstallFinalize.
    /// </summary>
    public partial class ForceReboot : WixObject
    {
        /// <summary>
        /// Defines at what <see cref="Step"/> the action should be executed during the installation.
        /// </summary>
        public Step Step = Step.InstallExecute;

        /// <summary>
        /// Defines order <see cref="When"/> the action should be executed with respect to the action <see cref="Step"/>.
        /// </summary>
        public When When = When.After;

        /// <summary>
        /// If <c>true</c>, this action will not occur.
        /// </summary>
        public bool? Suppress;

        /// <summary>
        /// Text node specifies the condition of the action.
        /// </summary>
        public Condition Condition;

        /// <summary>
        /// The parent install sequence. The only available sequence for this element is <c>InstallExecuteSequence</c>.
        /// </summary>
        public const RebootInstallSequence InstallSequence = RebootInstallSequence.InstallExecute;

        /// <summary>
        /// If <c>true</c>, the sequencing of this action may be overridden by sequencing elsewhere.
        /// </summary>
        public bool? Overridable;

        internal XElement ToXml()
        {
            return new XElement(this.GetType().Name)
                               .SetAttribute("Condition", Condition.ToXValue())
                               .SetAttribute("Overridable", Overridable)
                               .SetAttribute("Suppress", Suppress)
                               .SetAttribute(When.ToString(), Step)
                               .AddAttributes(this.Attributes);
        }

        //not sure support for SequenceNumber is required; disable it out until it's truly needed
        //private XElement ToXml_FutureImplementation(string prevActionName = null)
        //{
        //var element = new XElement(this.GetType().Name, Condition.ToXValue())
        //                      .SetAttribute("Overridable", Overridable)
        //                      .SetAttribute("Suppress", Suppress)
        //                      .SetAttribute("Sequence", SequenceNumber);

        //var step = Step?.ToString();
        //if (Step == Step.PreviousAction)
        //    step = prevActionName;
        //if (Step == Step.PreviousActionOrInstallInitialize)
        //    step = prevActionName ?? Step.InstallInitialize.ToString();
        //if (step != null)
        //    element.SetAttribute(When.ToString(), step);

        //return element;
        //}
    }

    /// <summary>
    /// Prompts the user to restart the system at the end of installation. Special actions don't have a built-in
    /// sequence number and thus must appear relative to another action. The suggested way to do this is by using
    /// the Before or After attribute. InstallExecute and InstallExecuteAgain can optionally appear anywhere between
    /// InstallInitialize and InstallFinalize.
    /// </summary>
    public partial class ScheduleReboot : ForceReboot
    {
        /// <summary>
        /// Indicates what install sequence should ScheduleReboot be placed to.
        /// </summary>
        public new RebootInstallSequence InstallSequence = RebootInstallSequence.InstallExecute;
    }
}