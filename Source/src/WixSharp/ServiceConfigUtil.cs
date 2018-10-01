using System;

namespace WixSharp
{
    /// <summary>
    /// Service configuration information for failure actions.
    /// </summary>
    public class ServiceConfigUtil : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Action to take on the first failure of the service
        /// </summary>
        [Xml]
        public FailureActionType FirstFailureActionType = FailureActionType.none;

        /// <summary>
        /// Action to take on the second failure of the service.
        /// </summary>
        [Xml]
        public FailureActionType SecondFailureActionType = FailureActionType.none;

        /// <summary>
        /// Action to take on the third failure of the service.
        /// </summary>
        [Xml]
        public FailureActionType ThirdFailureActionType = FailureActionType.none;

        /// <summary>
        /// If any of the three *ActionType attributes is "runCommand",
        /// this specifies the command to run when doing so.
        /// </summary>
        [Xml]
        public string ProgramCommandLine;

        /// <summary>
        /// If any of the three *ActionType attributes is "reboot",
        /// this specifies the message to broadcast to server users before doing so.
        /// </summary>
        [Xml]
        public string RebootMessage;

        /// <summary>
        /// Number of days after which to reset the failure count to zero if there are no failures.
        /// </summary>
        [Xml]
        public int? ResetPeriodInDays;

        /// <summary>
        /// If any of the three *ActionType attributes is "restart",
        /// this specifies the number of seconds to wait before doing so.
        /// </summary>
        [Xml]
        public int? RestartServiceDelayInSeconds;
        
        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Util);
            context.XParent.Add(this.ToXElement(WixExtension.Util, "ServiceConfig"));

        }
    }
}