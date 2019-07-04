namespace WixSharp
{
    /// <summary>
    /// Closes applications or schedules a reboot if application cannot be closed.
    /// <example>The following is an example of closing <c>MyApp.exe</c> application.
    /// The example also illustrates the use of a condition for the <c>CloseApplication</c> entry.
    /// <code>
    /// var project =
    ///     new Project("My Product",
    ///         new CustomActionRef("WixCloseApplications", When.Before, Step.CostFinalize, new Condition("VersionNT > 400"),
    ///         new CloseApplication("MyApp.exe", true, false)),
    ///         ...
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    /// </summary>
    public class CloseApplication : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Identifier for the close application (primary key).
        /// If the Id is not specified, one will be generated.
        /// </summary>
        [Xml]
        public new string Id;

        /// <summary>
        /// asAdsa
        /// </summary>
        /// <value>The Name property gets/sets the value of the string field, _name.</value>
        public int MyProperty { get; set; } = 9;

        /// <summary>
        /// Property to be set if application is still running. Useful for launch conditions or
        /// to conditionalize custom UI to ask user to shutdown apps.
        /// </summary>
        [Xml]
        public string Property;

        /// <summary>
        /// Name of the executable to be closed.
        /// This should only be the file name.
        /// </summary>
        [Xml]
        public string Target;

        /// <summary>
        /// Description to show if application is running and needs to be closed.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// Attempts to terminates process and return the specified exit code if application is still running after sending any requested close and/or end session messages.
        /// If this attribute is specified, the RebootPrompt attribute must be "no". The default is "no".
        /// </summary>
        [Xml]
        public int? TerminateProcess;

        /// <summary>
        /// Optional time in seconds to wait for the application to exit after the close and/or end session messages.
        /// If the application is still running after the timeout then the RebootPrompt or TerminateProcess attributes will be considered.
        /// </summary>
        [Xml]
        public int Timeout = 5;

        /// <summary>
        /// Optionally orders the applications to be closed.
        /// </summary>
        [Xml]
        public int Sequence = 1;

        /// <summary>
        /// Optionally sends a close message to the application.
        /// </summary>
        [Xml]
        public bool CloseMessage = false;

        /// <summary>
        /// Optionally sends a close message to the application from differed action without impersonation.
        /// </summary>
        [Xml]
        public bool ElevatedCloseMessage = false;

        /// <summary>
        /// Sends WM_QUERYENDSESSION then WM_ENDSESSION messages to the application from a differed action without impersonation.
        /// </summary>
        [Xml]
        public bool ElevatedEndSessionMessage = false;

        /// <summary>
        /// Sends WM_QUERYENDSESSION then WM_ENDSESSION messages to the application.
        /// </summary>
        [Xml]
        public bool EndSessionMessage = false;

        /// <summary>
        /// When this attribute is set to "true", the user will be prompted when the application is still running.
        /// The Description attribute must contain the message to display in the prompt.
        /// The prompt occurs before executing any of the other options and gives the options to "Abort", "Retry", or "Ignore".
        /// <para>"Abort" will cancel the install.</para>
        /// <para>"Retry" will attempt the check again and if the application is still running, prompt again.</para>
        /// <para> "Ignore" will continue and execute any other options set on the CloseApplication element.  </para>
        /// </summary>
        [Xml]
        public bool PromptToContinue = false;

        /// <summary>
        /// Optionally prompts for reboot if application is still running.
        /// The TerminateProcess attribute must be "no" or not specified if this attribute is "yes".
        /// </summary>
        [Xml]
        public bool RebootPrompt = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseApplication" /> class.
        /// </summary>
        /// <param name="target">The Target.</param>
        public CloseApplication(string target)
        {
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseApplication" /> class.
        /// </summary>
        /// <param name="target">The Target.</param>
        /// <param name="closeMessage">The CloseMessage.</param>
        public CloseApplication(string target, bool closeMessage)
        {
            Target = target;
            CloseMessage = closeMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseApplication" /> class.
        /// </summary>
        /// <param name="target">The Target.</param>
        /// <param name="closeMessage">The CloseMessage.</param>
        /// <param name="rebootPrompt">The RebootPrompt.</param>
        public CloseApplication(string target, bool closeMessage, bool rebootPrompt)
        {
            Target = target;
            CloseMessage = closeMessage;
            RebootPrompt = rebootPrompt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseApplication" /> class.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="target">The Target.</param>
        public CloseApplication(Id id, string target)
        {
            Id = id;
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseApplication" /> class.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="target">The Target.</param>
        /// <param name="closeMessage">The CloseMessage.</param>
        public CloseApplication(Id id, string target, bool closeMessage)
        {
            Id = id;
            Target = target;
            CloseMessage = closeMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseApplication" /> class.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="target">The Target.</param>
        /// <param name="closeMessage">The CloseMessage.</param>
        /// <param name="rebootPrompt">The RebootPrompt.</param>
        public CloseApplication(Id id, string target, bool closeMessage, bool rebootPrompt)
        {
            Id = id;
            Target = target;
            CloseMessage = closeMessage;
            RebootPrompt = rebootPrompt;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Util);

            context.XParent
                   .Add(this.ToXElement(WixExtension.Util, "CloseApplication"));
        }
    }
}