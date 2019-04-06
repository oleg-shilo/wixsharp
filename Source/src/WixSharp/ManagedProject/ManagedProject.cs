using System;
using System.Collections.Generic;

using System.Diagnostics;

using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp.CommonTasks;

using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Extends <see cref="T:WixSharp.Project"/> with runtime event-driven behavior ans managed UI (see <see cref="T:WixSharp.ManagedUI"/> and
    /// <see cref="T:WixSharp.UI.Forms.ManagedForm"/>).
    /// <para>
    /// The managed project has three very important events that are raised during deployment at run: Load/BeforeInstall/AfterInstall.
    /// </para>
    /// <remark>
    /// ManagedProject still maintains backward compatibility for the all older features. That is why it is important to distinguish the use cases
    /// associated with the project class members dedicated to the same problem domain but resolving the problems in different ways:
    /// <para><c>UI support</c></para>
    /// <para>   project.UI - to be used to define native MSI/WiX UI.</para>
    /// <para>   project.CustomUI - to be used for minor to customization of native MSI/WiX UI and injection of CLR dialogs. </para>
    /// <para>   project.ManagedUI - to be used to define managed Embedded UI. It allows full customization of the UI</para>
    /// <para> </para>
    /// <para><c>Events</c></para>
    /// <para>   project.WixSourceGenerated</para>
    /// <para>   project.WixSourceFormated</para>
    /// <para>   project.WixSourceSaved - to be used at compile time to customize WiX source code (XML) generation.</para>
    /// <para> </para>
    /// <para>   project.Load</para>
    /// <para>   project.BeforeInstall</para>
    /// <para>   project.AfterInstall - to be used at runtime (msi execution) to customize deployment behaver.</para>
    /// </remark>
    /// </summary>
    /// <example>The following is an example of a simple setup handling the three setup events at runtime.
    /// <code>
    /// var project = new ManagedProject("ManagedSetup",
    ///                   new Dir(@"%ProgramFiles%\My Company\My Product",
    ///                       new File(@"..\Files\bin\MyApp.exe")));
    ///
    /// project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
    ///
    /// project.ManagedUI = ManagedUI.Empty;
    ///
    /// project.Load += project_Load;
    /// project.BeforeInstall += project_BeforeInstall;
    /// project.AfterInstall += project_AfterInstall;
    ///
    /// project.BuildMsi();
    /// </code>
    /// </example>
    public class ManagedProject : Project
    {
        //some materials to consider: http://cpiekarski.com/2012/05/18/wix-custom-action-sequencing/

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedProject"/> class.
        /// </summary>
        public ManagedProject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedProject"/> class.
        /// </summary>
        /// <param name="name">The name of the project. Typically it is the name of the product to be installed.</param>
        /// <param name="items">The project installable items (e.g. directories, files, registry keys, Custom Actions).</param>
        public ManagedProject(string name, params WixObject[] items)
            : base(name, items)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the full or reduced custom drawing algorithm should be used for rendering the Features dialog
        /// tree view control. The reduced algorithm involves no manual positioning of the visual elements so it handles better custom screen resolutions.
        /// However it also leads to the slightly less intuitive tree view item appearance.
        /// <para>Reduced custom drawing will render disabled tree view items with the text grayed out.</para>
        /// <para>Full custom drawing will render disabled tree view items with the checkbox grayed out.</para>
        /// </summary>
        /// <value>
        /// <c>true</c> (default) if custom drawing should be reduced otherwise, <c>false</c>.
        /// </value>
        public bool MinimalCustomDrawing
        {
            get
            {
                return Properties.Where(x => x.Name == "WixSharpUI_TreeNode_TexOnlyDrawing").FirstOrDefault() != null;
            }

            set
            {
                if (value)
                {
                    var prop = Properties.Where(x => x.Name == "WixSharpUI_TreeNode_TexOnlyDrawing").FirstOrDefault();
                    if (prop != null)
                        prop.Value = "true";
                    else
                        this.AddProperty(new Property("WixSharpUI_TreeNode_TexOnlyDrawing", "true"));
                }
                else
                {
                    Properties = Properties.Where(x => x.Name != "WixSharpUI_TreeNode_TexOnlyDrawing").ToArray();
                }
            }
        }

        /// <summary>
        /// Event handler of the ManagedSetup for the MSI runtime events.
        /// </summary>
        /// <param name="e">The <see cref="SetupEventArgs"/> instance containing the event data.</param>
        public delegate void SetupEventHandler(SetupEventArgs e);

        /// <summary>
        /// Indicates if the installations should be aborted if managed event handler throws an unhanded exception.
        /// <para>
        /// Aborting is the default behavior if this field is not set.
        /// </para>
        /// </summary>
        public bool? AbortSetupOnUnhandledExceptions;

        /// <summary>
        /// Occurs on EmbeddedUI initialized but before the first dialog is displayed. It is only invoked if ManagedUI is set.
        /// </summary>
        public event SetupEventHandler UIInitialized;

        /// <summary>
        /// Occurs on EmbeddedUI loaded and ShellView (main window) is displayed but before first dialog is positioned within ShellView.
        /// It is only invoked if ManagedUI is set.
        /// <para>Note that this event is fired on the loading the UI main window thus it's not a good stage for any decision regarding
        /// aborting/continuing the whole setup process. That is UILoaded event will ignore any value set to SetupEventArgs.Result by the user.
        /// </para>
        /// </summary>
        public event SetupEventHandler UILoaded;

        /// <summary>
        /// Occurs before AppSearch standard action.
        /// </summary>
        public event SetupEventHandler Load;

        /// <summary>
        /// Occurs before InstallFiles standard action.
        /// </summary>
        public event SetupEventHandler BeforeInstall;

        /// <summary>
        /// Occurs after InstallFiles standard action. The event is fired from the elevated execution context.
        /// </summary>
        public event SetupEventHandler AfterInstall;

        /// <summary>
        /// An instance of ManagedUI defining MSI UI dialogs sequence. User should set it if he/she wants native MSI dialogs to be
        /// replaced by managed ones.
        /// </summary>
        public IManagedUI ManagedUI;

        bool preprocessed = false;

        string thisAsm = typeof(ManagedProject).Assembly.Location;

        bool IsHandlerSet<T>(Expression<Func<T>> expression)
        {
            var name = Reflect.NameOf(expression);
            var handler = expression.Compile()() as Delegate;
            return (handler != null);
        }

        void Bind<T>(Expression<Func<T>> expression, When when = When.Before, Step step = null, bool elevated = false)
        {
            var name = Reflect.NameOf(expression);
            var handler = expression.Compile()() as Delegate;

            const string wixSharpProperties = "WIXSHARP_RUNTIME_DATA";

            if (handler != null)
            {
                if (this.AbortSetupOnUnhandledExceptions.HasValue)
                {
                    var abortOnErrorName = "WIXSHARP_ABORT_ON_ERROR";

                    if (!Properties.Any(p => p.Name == abortOnErrorName))
                        this.AddProperty(new Property(abortOnErrorName, this.AbortSetupOnUnhandledExceptions.Value.ToString()));
                }

                //foreach (string handlerAsm in handler.GetInvocationList().Select(x => x.Method.DeclaringType.Assembly.Location))
                foreach (var type in handler.GetInvocationList().Select(x => x.Method.DeclaringType))
                {
                    string location = type.Assembly.Location;

                    //Resolving scriptAsmLocation is not properly tested yet
                    bool resolveInMemAsms = true;

                    if (resolveInMemAsms)
                    {
                        if (location.IsEmpty())
                            location = type.Assembly.GetLocation();
                    }

                    if (location.IsEmpty())
                        throw new ApplicationException($"The location of the assembly for ManagedProject event handler ({type}) cannot be obtained.\n" +
                                                        "The assembly must be a file based one but it looks like it was loaded from memory.\n" +
                                                        "If you are using CS-Script to build MSI ensure it has 'InMemoryAssembly' set to false.");

                    if (!this.DefaultRefAssemblies.Contains(location))
                        this.DefaultRefAssemblies.Add(location);
                }

                this.AddProperty(new Property("WixSharp_{0}_Handlers".FormatWith(name), GetHandlersInfo(handler as MulticastDelegate)));

                string dllEntry = "WixSharp_{0}_Action".FormatWith(name);
                if (step != null)
                {
                    if (elevated)
                        this.AddAction(new ElevatedManagedAction(dllEntry)
                        {
                            Id = new Id(dllEntry),
                            ActionAssembly = thisAsm,
                            Return = Return.check,
                            When = when,
                            Step = step,
                            Condition = Condition.Create("1"),
                            UsesProperties = "WixSharp_{0}_Handlers,{1},{2}".FormatWith(name, wixSharpProperties, DefaultDeferredProperties),
                        });
                    else
                        this.AddAction(new ManagedAction(dllEntry) { Id = new Id(dllEntry), ActionAssembly = thisAsm, Return = Return.check, When = when, Step = step, Condition = Condition.Create("1") });
                }
            }
        }

        void AddCancelFromUIIHandler()
        {
            string dllEntry = "CancelRequestHandler";
            this.AddAction(new ElevatedManagedAction(dllEntry)
            {
                Id = new Id(dllEntry),
                ActionAssembly = thisAsm,
                Return = Return.check,
                When = When.Before,
                Step = Step.InstallFinalize,
                Condition = Condition.NOT_BeingRemoved,
                UsesProperties = "UpgradeCode"
            });
        }

        /// <summary>
        /// The default properties mapped for use with the deferred custom actions. See <see cref="ManagedAction.UsesProperties"/> for the details.
        /// <para>The default value is "INSTALLDIR,UILevel"</para>
        /// </summary>
        public string DefaultDeferredProperties
        {
            set { defaultDeferredProperties = value; }
            get
            {
                return string.Join(",", this.Properties
                                            .Where(x => x.IsDeferred)
                                            .Select(x => x.Name)
                                            .Concat(defaultDeferredProperties.Split(','))
                                            .Where(x => x.IsNotEmpty())
                                            .Select(x => x.Trim())
                                            .ToArray());
            }
        }

        string defaultDeferredProperties = "INSTALLDIR,UILevel,ProductName";

        /// <summary>
        /// Flags that indicates if <c>WixSharp_InitRuntime_Action</c> custom action should be always scheduled. The default value is <c>true</c>.
        /// <para><c>WixSharp_InitRuntime_Action</c> is the action, which ManagedSetup performs at startup (before AppSearch).
        /// In this action the most important MSI properties are pushed into Session.CustomActionData. These properties are typically consumed
        /// from other custom actions (e.g. Project.AfterInstall event) and they are:
        /// <list type="bullet">
        /// <item><description>Installed</description></item>
        /// <item><description>REMOVE</description></item>
        /// <item><description>ProductName</description></item>
        /// <item><description>REINSTALL</description></item>
        /// <item><description>UPGRADINGPRODUCTCODE</description></item>
        /// <item><description>UILevel</description></item>
        /// <item><description>WIXSHARP_MANAGED_UI</description></item>
        /// </list>
        /// </para>
        /// <para>However in same cases (e.g. oversizes msi file) it is desirable to minimize amount of time the msi loaded into memory
        /// (e.g. by custom actions). Thus setting AlwaysScheduleInitRuntime to <c>false</c> will prevent scheduling <c>WixSharp_InitRuntime_Action</c>
        /// unless any of the ManagedProject events (e.g. Project.AfterInstall) has user handler assigned.
        /// </para>
        /// </summary>
        public bool AlwaysScheduleInitRuntime = true;

        override internal void Preprocess()
        {
            //Debug.Assert(false);
            base.Preprocess();

            if (!preprocessed)
            {
                preprocessed = true;

                //It is too late to set prerequisites. Launch conditions are evaluated after UI is popped up.
                //this.SetNetFxPrerequisite(Condition.Net35_Installed, "Please install .NET v3.5 first.");

                if (ManagedUI?.Icon != null)
                {
                    this.AddBinary(new Binary(new Id("ui_shell_icon"), ManagedUI.Icon));
                }

                string dllEntry = "WixSharp_InitRuntime_Action";

                bool needInvokeInitRuntime = (IsHandlerSet(() => UIInitialized)
                                              || IsHandlerSet(() => Load)
                                              || IsHandlerSet(() => UILoaded)
                                              || IsHandlerSet(() => BeforeInstall)
                                              || IsHandlerSet(() => AfterInstall)
                                              || AlwaysScheduleInitRuntime);
                if (needInvokeInitRuntime)
                    this.AddAction(new ManagedAction(dllEntry)
                    {
                        Id = new Id(dllEntry),
                        ActionAssembly = thisAsm,
                        Return = Return.check,
                        When = When.Before,
                        Step = Step.AppSearch,
                        Condition = Condition.Always
                    });

                if (ManagedUI != null)
                {
                    this.AddProperty(new Property("WixSharp_UI_INSTALLDIR", ManagedUI.InstallDirId ?? "INSTALLDIR"));

                    if (AutoElements.EnableUACRevealer)
                        this.AddProperty(new Property("UAC_REVEALER_ENABLED", "true"));

                    if (AutoElements.UACWarning.IsNotEmpty())
                        this.AddProperty(new Property("UAC_WARNING", AutoElements.UACWarning));

                    ManagedUI.BeforeBuild(this);

                    InjectDialogs("WixSharp_InstallDialogs", ManagedUI.InstallDialogs);
                    InjectDialogs("WixSharp_ModifyDialogs", ManagedUI.ModifyDialogs);

                    this.EmbeddedUI = new EmbeddedAssembly(new Id("WixSharp_EmbeddedUI_Asm"), ManagedUI.GetType().Assembly.Location);

                    this.DefaultRefAssemblies.Add(ManagedUI.GetType().Assembly.Location);

                    Bind(() => UIInitialized);
                    Bind(() => UILoaded);

                    AddCancelFromUIIHandler();
                }

                Bind(() => Load, When.Before, Step.AppSearch);
                Bind(() => BeforeInstall, When.Before, Step.InstallFiles);
                Bind(() => AfterInstall, When.After, Step.InstallFiles, true);
            }
        }

        void InjectDialogs(string name, ManagedDialogs dialogs)
        {
            if (dialogs.Any())
            {
                var dialogsInfo = new StringBuilder();

                foreach (var item in dialogs)
                {
                    if (!this.DefaultRefAssemblies.Contains(item.Assembly.Location))
                        this.DefaultRefAssemblies.Add(item.Assembly.Location);

                    var info = GetDialogInfo(item);

                    ValidateDialogInfo(info);
                    dialogsInfo.Append(info + "\n");
                }
                this.AddProperty(new Property(name, dialogsInfo.ToString().Trim()));
            }
        }

        /// <summary>
        /// Reads and returns the dialog types from the string definition. This method is to be used by WixSharp assembly.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static List<Type> ReadDialogs(string data)
        {
            return data.Split('\n')
                       .Select(x => x.Trim())
                       .Where(x => x.IsNotEmpty())
                       .Select(x => ManagedProject.GetDialog(x))
                       .ToList();
        }

        static void ValidateDialogInfo(string info)
        {
            try
            {
                GetDialog(info, true);
            }
            catch (Exception)
            {
                //may need to do extra logging; not important for now
                throw;
            }
        }

        static string GetDialogInfo(Type dialog)
        {
            var info = "{0}|{1}".FormatWith(
                                 dialog.Assembly.FullName,
                                 dialog.FullName);
            return info;
        }

        internal static Type GetDialog(string info)
        {
            return GetDialog(info, false);
        }

        internal static Type GetDialog(string info, bool validate)
        {
            string[] parts = info.Split('|');

            var assembly = System.Reflection.Assembly.Load(parts[0]);

            if (validate)
                ProjectValidator.ValidateAssemblyCompatibility(assembly);

            return assembly.GetType(parts[1]);
        }

        static void ValidateHandlerInfo(string info)
        {
            try
            {
                GetHandler(info);
            }
            catch (Exception)
            {
                //may need to do extra logging; not important for now
                throw;
            }
        }

        internal static string GetHandlersInfo(MulticastDelegate handlers)
        {
            var result = new StringBuilder();

            foreach (Delegate action in handlers.GetInvocationList())
            {
                var handlerInfo = "{0}|{1}|{2}".FormatWith(
                                                action.Method.DeclaringType.Assembly.FullName,
                                                action.Method.DeclaringType.FullName,
                                                action.Method.Name);

                ValidateHandlerInfo(handlerInfo);

                result.AppendLine(handlerInfo);
            }
            return result.ToString().Trim();
        }

        static MethodInfo GetHandler(string info)
        {
            string[] parts = info.Split('|');

            var assembly = System.Reflection.Assembly.Load(parts[0]);

            Type type = null;

            //Ideally need to iterate through the all types in order to find even private ones
            //Though loading some internal (irrelevant) types can fail because of the dependencies.
            try
            {
                assembly.GetTypes().Single(t => t.FullName == parts[1]);
            }
            catch { }

            //If we failed to iterate through the types then try to load the type explicitly. Though in this case it has to be public.
            if (type == null)
                type = assembly.GetType(parts[1]);

            var method = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Static)
                             .Single(m => m.Name == parts[2]);

            return method;
        }

        static void InvokeClientHandler(string info, SetupEventArgs eventArgs)
        {
            MethodInfo method = GetHandler(info);

            if (method.IsStatic)
                method.Invoke(null, new object[] { eventArgs });
            else
                method.Invoke(Activator.CreateInstance(method.DeclaringType), new object[] { eventArgs });
        }

        internal static ActionResult InvokeClientHandlers(Session session, string eventName, IShellView UIShell = null)
        {
            var eventArgs = Convert(session);
            eventArgs.ManagedUI = UIShell;

            try
            {
                string handlersInfo = session.Property("WixSharp_{0}_Handlers".FormatWith(eventName));

                if (!string.IsNullOrEmpty(handlersInfo))
                {
                    foreach (string item in handlersInfo.Trim().Split('\n'))
                    {
                        InvokeClientHandler(item.Trim(), eventArgs);
                        if (eventArgs.Result == ActionResult.Failure || eventArgs.Result == ActionResult.UserExit)
                            break;
                    }

                    eventArgs.SaveData();
                }
            }
            catch (Exception e)
            {
                session.Log("WixSharp aborted the session because of the error:" + Environment.NewLine + e.ToPublicString());
                if (session.AbortOnError())
                    eventArgs.Result = ActionResult.Failure;
            }
            return eventArgs.Result;
        }

        internal static ActionResult Init(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            var data = new SetupEventArgs.AppData();
            try
            {
                data["Installed"] = session["Installed"];
                data["REMOVE"] = session["REMOVE"];
                data["ProductName"] = session["ProductName"];
                data["ProductCode"] = session["ProductCode"];
                data["UpgradeCode"] = session["UpgradeCode"];
                data["REINSTALL"] = session["REINSTALL"];
                data["MsiFile"] = session["OriginalDatabase"];
                data["UPGRADINGPRODUCTCODE"] = session["UPGRADINGPRODUCTCODE"];
                data["UILevel"] = session["UILevel"];
                data["WIXSHARP_MANAGED_UI"] = session["WIXSHARP_MANAGED_UI"];
                data["WIXSHARP_MANAGED_UI_HANDLE"] = session["WIXSHARP_MANAGED_UI_HANDLE"];
            }
            catch (Exception e)
            {
                session.Log(e.Message);
            }

            data.MergeReplace(session["WIXSHARP_RUNTIME_DATA"]);

            session["WIXSHARP_RUNTIME_DATA"] = data.ToString();

            return ActionResult.Success;
        }

        internal static SetupEventArgs Convert(Session session)
        {
            //Debugger.Launch();
            var result = new SetupEventArgs { Session = session };
            try
            {
                string data = session.Property("WIXSHARP_RUNTIME_DATA");
                result.Data.InitFrom(data);
                result.Data.SetEnvironmentVariables();
                session.CustomActionData.SetEnvironmentVariables();
            }
            catch (Exception e)
            {
                session.Log(e.Message);
            }
            return result;
        }
    }
}