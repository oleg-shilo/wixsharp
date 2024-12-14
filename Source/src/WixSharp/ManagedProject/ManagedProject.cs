using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using WixSharp.CommonTasks;
using WixToolset.Dtf.WindowsInstaller;

using IO = System.IO;
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file

namespace WixSharp
{
    /// <summary>
    /// Extends <see cref="T:WixSharp.Project"/> with runtime event-driven behavior ans managed UI
    /// (see <see cref="T:WixSharp.ManagedUI"/> and <see cref="T:WixSharp.UI.Forms.ManagedForm"/>).
    /// <para>
    /// The managed project has three very important events that are raised during deployment at
    /// run: Load/BeforeInstall/AfterInstall.
    /// </para>
    /// <remark> ManagedProject still maintains backward compatibility for the all older features.
    /// That is why it is important to distinguish the use cases associated with the project class
    /// members dedicated to the same problem domain but resolving the problems in different ways:
    /// <para><c>UI support</c></para>
    /// <para>project.UI - to be used to define native MSI/WiX UI.</para>
    /// <para>
    /// project.CustomUI - to be used for minor to customization of native MSI/WiX UI and injection
    /// of CLR dialogs.
    /// </para>
    /// <para>
    /// project.ManagedUI - to be used to define managed Embedded UI. It allows full customization
    /// of the UI
    /// </para>
    /// <para></para>
    /// <para><c>Events</c></para>
    /// <para>project.WixSourceGenerated</para>
    /// <para>project.WixSourceFormated</para>
    /// <para>
    /// project.WixSourceSaved - to be used at compile time to customize WiX source code (XML) generation.
    /// </para>
    /// <para></para>
    /// <para>project.Load</para>
    /// <para>project.BeforeInstall</para>
    /// <para>
    /// project.AfterInstall - to be used at runtime (msi execution) to customize deployment behaver.
    /// </para>
    /// </remark>
    /// </summary>
    /// <example>
    /// The following is an example of a simple setup handling the three setup events at runtime.
    /// <code>
    ///var project = new ManagedProject("ManagedSetup",
    ///new Dir(@"%ProgramFiles%\My Company\My Product",
    ///new File(@"..\Files\bin\MyApp.exe")));
    ///
    ///project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
    ///
    ///project.ManagedUI = ManagedUI.Empty;
    ///
    ///project.Load += project_Load;
    ///project.BeforeInstall += project_BeforeInstall;
    ///project.AfterInstall += project_AfterInstall;
    ///
    ///project.BuildMsi();
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
        /// <param name="name">
        /// The name of the project. Typically it is the name of the product to be installed.
        /// </param>
        /// <param name="items">
        /// The project installable items (e.g. directories, files, registry keys, Custom Actions).
        /// </param>
        public ManagedProject(string name, params WixObject[] items)
            : base(name, items)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the full or reduced custom drawing algorithm
        /// should be used for rendering the Features dialog tree view control. The reduced
        /// algorithm involves no manual positioning of the visual elements so it handles better
        /// custom screen resolutions. However it also leads to the slightly less intuitive tree
        /// view item appearance.
        /// <para>
        /// Reduced custom drawing will render disabled tree view items with the text grayed out.
        /// </para>
        /// <para>
        /// Full custom drawing will render disabled tree view items with the checkbox grayed out.
        /// </para>
        /// </summary>
        /// <value><c>true</c> (default) if custom drawing should be reduced otherwise, <c>false</c>.</value>
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
        /// Occurs when an exception thrown in one of the project runtime events or Managed UI dialog is not caught.
        /// </summary>
        /// <param name="e">The <see cref="ExceptionEventArgs"/> instance containing the event data.</param>
        public delegate void UnhandledExceptionEventHandler(ExceptionEventArgs e);

        /// <summary>
        /// Indicates if the installations should be aborted if managed event handler throws an
        /// unhanded exception.
        /// <para>Aborting is the default behavior if this field is not set.</para>
        /// </summary>
        public bool? AbortSetupOnUnhandledExceptions;

        /// <summary>
        /// Occurs on EmbeddedUI initialized but before the first dialog is displayed. It is only
        /// invoked if ManagedUI is set.
        /// </summary>
        public event SetupEventHandler UIInitialized;

        /// <summary>
        /// Occurs on EmbeddedUI loaded and ShellView (main window) is displayed but before first
        /// dialog is positioned within ShellView. It is only invoked if ManagedUI is set.
        /// <para>
        /// Note that this event is fired on the loading the UI main window thus it's not a good
        /// stage for any decision regarding aborting/continuing the whole setup process. That is
        /// UILoaded event will ignore any value set to SetupEventArgs.Result by the user.
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
        /// Occurs after InstallFiles standard action. The event is fired from the elevated
        /// execution context.
        /// <para>If it is required that the event handler is invoked without elevation then you can
        /// call <see cref="WixSharp.CommonTasks.Tasks.UnelevateAfterInstallEvent(ManagedProject)"/> so the `Project.AfterInstall` event is
        /// scheduled for unelevated execution.</para>
        /// </summary>
        public event SetupEventHandler AfterInstall;

        /// <summary>
        /// The execution model for <see cref="BeforeInstall"/> event.
        /// </summary>
        public EventExecution BeforeInstallEventExecution = EventExecution.MsiSessionScopeImmediate;

        /// <summary>
        /// The execution model for <see cref="Load"/> event.
        /// </summary>
        public EventExecution LoadEventExecution = EventExecution.MsiSessionScopeImmediate;

        /// <summary>
        /// The execution model for <see cref="AfterInstall"/> event.
        /// </summary>
        public EventExecution AfterInstallEventExecution = EventExecution.MsiSessionScopeDeferred;

        /// <summary>
        /// Occurs when an unhandled exception is thrown either from Managed UI or from ManagedProject event (e.g. <see cref="WixSharp.ManagedProject.BeforeInstall"/>).
        /// </summary>
        public event UnhandledExceptionEventHandler UnhandledException;

        /// <summary>
        /// An instance of ManagedUI defining MSI UI dialogs sequence. User should set it if he/she
        /// wants native MSI dialogs to be replaced by managed ones.
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

        internal static Dictionary<string, string> HandlerAotDeclaringTypes = new Dictionary<string, string>();

        void Bind<T>(Expression<Func<T>> expression, When when = When.Before, Step step = null, EventExecution eventExecution = default)
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

                foreach (var type in handler.GetInvocationList().Select(x => x.Method.DeclaringType))
                {
                    string location = type.Assembly.Location;

                    var rootType = type.RootDeclaringType();

                    if (HandlerAotDeclaringTypes.ContainsKey(location))
                        HandlerAotDeclaringTypes[location] += "," + rootType.FullName;
                    else
                        HandlerAotDeclaringTypes[location] = rootType.FullName;

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
                    if (eventExecution == EventExecution.MsiSessionScopeDeferred)
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
                        this.AddAction(new ManagedAction(dllEntry)
                        {
                            Id = new Id(dllEntry),
                            ActionAssembly = thisAsm,
                            Return = Return.check,
                            When = when,
                            Step = step,

                            Condition = Condition.Create("1")
                        });
                }
            }
        }

        void BindUnhandledExceptionHadler<T>(Expression<Func<T>> expression)
        {
            var name = Reflect.NameOf(expression);
            var handler = expression.Compile()() as Delegate;

            if (handler != null)
            {
                foreach (var type in handler.GetInvocationList().Select(x => x.Method.DeclaringType))
                {
                    string location = type.Assembly.Location;

                    // Resolving scriptAsmLocation is not properly tested yet
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

                this.AddProperty(new Property("WixSharp_UnhandledException_Handlers", GetHandlersInfo(handler as MulticastDelegate)));
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
                Condition = Condition.Always,
                UsesProperties = "UpgradeCode"
            });
        }

        /// <summary>
        /// The default properties mapped for use with the setup events (deferred custom actions). See <see
        /// cref="ManagedAction.UsesProperties"/> for the details.
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
                                            .Concat(new[] { "MsiLogFileLocation" })
                                            .Where(x => x.IsNotEmpty())
                                            .Select(x => x.Trim())
                                            .Distinct()
                                            .ToArray());
            }
        }

        string defaultDeferredProperties = "INSTALLDIR,UILevel,ProductName,FOUNDPREVIOUSVERSION,UpgradeCode,ManagedProjectElevatedEvents";

        /// <summary>
        /// Flags that indicates if <c>WixSharp_InitRuntime_Action</c> custom action should be
        /// always scheduled. The default value is <c>true</c>.
        /// <para>
        /// <c>WixSharp_InitRuntime_Action</c> is the action, which ManagedSetup performs at startup
        /// (before AppSearch). In this action the most important MSI properties are pushed into
        /// Session.CustomActionData. These properties are typically consumed from other custom
        /// actions (e.g. Project.AfterInstall event) and they are:
        /// <list type="bullet">
        /// <item>
        /// <description>Installed</description>
        /// </item>
        /// <item>
        /// <description>REMOVE</description>
        /// </item>
        /// <item>
        /// <description>ProductName</description>
        /// </item>
        /// <item>
        /// <description>REINSTALL</description>
        /// </item>
        /// <item>
        /// <description>UPGRADINGPRODUCTCODE</description>
        /// </item>
        /// <item>
        /// <description>UILevel</description>
        /// </item>
        /// <item>
        /// <description>WIXSHARP_MANAGED_UI</description>
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// However in same cases (e.g. oversizes msi file) it is desirable to minimize amount of
        /// time the msi loaded into memory (e.g. by custom actions). Thus setting
        /// AlwaysScheduleInitRuntime to <c>false</c> will prevent scheduling
        /// <c>WixSharp_InitRuntime_Action</c> unless any of the ManagedProject events (e.g.
        /// Project.AfterInstall) has user handler assigned.
        /// </para>
        /// </summary>
        public bool AlwaysScheduleInitRuntime = true;

        internal bool IsNetCore = Environment.Version.Major > 5;

        override internal void Preprocess()
        {
            // Debug.Assert(false);
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

                // With .NET-Core we do not schedule any CA to initialize reflection based algorithm for invoking
                // event handlers at runtime. All event handlers will be exported as entry points of the AOT compiled
                // client assembly instead.
                if (this.IsNetCore)
                {
                    needInvokeInitRuntime = false;
                    ValidateAotReadiness();
                }

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

                var elevatedEvents = new List<string>();
                if (this.BeforeInstallEventExecution == EventExecution.ExternalElevatedProcess)
                    elevatedEvents.Add("BeforeInstall");
                if (this.LoadEventExecution == EventExecution.ExternalElevatedProcess)
                    elevatedEvents.Add("Load");
                if (this.AfterInstallEventExecution == EventExecution.ExternalElevatedProcess)
                    elevatedEvents.Add("AfterInstall");

                if (elevatedEvents.Any())
                {
                    if (this.IsNetCore)
                    {
                        Compiler.OutputWriteLine(
                            $"Error: Event execution model `{EventExecution.ExternalElevatedProcess}` " +
                            $"is not supported on .NET (Core family). Either use different model (e.g. " +
                            $"{nameof(EventExecution.MsiSessionScopeDeferred)}). Or use WixSharp Visual Studio " +
                            $"project template targetting .NET Framework.");
                    }

                    this.AddProperty(new Property("ManagedProjectElevatedEvents", elevatedEvents.JoinBy("|")));
                    this.DefaultDeferredProperties = ",ManagedProjectElevatedEvents";

                    // to be picked by the session serialization
                    this.AddProperty(new Property("DefaultDeferredProperties", DefaultDeferredProperties));

                    var eventHostAsm = this.GetType().Assembly.Location.PathChangeFileName("WixSharp.MsiEventHost.exe");
                    if (eventHostAsm.FileExists())
                        this.DefaultRefAssemblies.Add(eventHostAsm);
                    else
                        Compiler.OutputWriteLine($"Error: {eventHostAsm} is not found. Elevated events will not be executed.");
                }

                if (ManagedUI != null)
                {
                    this.AddProperty(new Property("WixSharp_UI_INSTALLDIR", ManagedUI.InstallDirId ?? "INSTALLDIR"));

                    if (AutoElements.EnableUACRevealer)
                        this.AddProperty(new Property("UAC_REVEALER_ENABLED", "true"));

                    if (AutoElements.UACWarning.IsNotEmpty())
                        this.AddProperty(new Property("UAC_WARNING", AutoElements.UACWarning));

                    InjectDialogs("WixSharp_InstallDialogs", ManagedUI.InstallDialogs);
                    InjectDialogs("WixSharp_ModifyDialogs", ManagedUI.ModifyDialogs);

                    this.EmbeddedUI = new EmbeddedAssembly(new Id("WixSharp_EmbeddedUI_Asm"), ManagedUI.GetType().Assembly.Location);

                    this.DefaultRefAssemblies.AddRange(ManagedUI.InstallDialogs.Assemblies); // WixSharp.UI and WixSharp.UI.WPF
                    this.DefaultRefAssemblies.AddRange(ManagedUI.ModifyDialogs.Assemblies);
                    this.DefaultRefAssemblies.AddRange(GetUiDialogsDependencies(ManagedUI)); // Caliburn, etc.
                    this.DefaultRefAssemblies.Add(ManagedUI.GetType().Assembly.Location);

                    this.DefaultRefAssemblies.FilterDuplicates();

                    Bind(() => UIInitialized);
                    Bind(() => UILoaded);

                    AddCancelFromUIIHandler();
                }

                Bind(() => Load, When.Before, Step.AppSearch, this.LoadEventExecution);
                Bind(() => BeforeInstall, When.Before, Step.InstallFiles, this.BeforeInstallEventExecution);
                Bind(() => AfterInstall, When.After, Step.InstallFiles, this.AfterInstallEventExecution);
                BindUnhandledExceptionHadler(() => UnhandledException);
            }

            if (ManagedUI != null)
                ManagedUI.BeforeBuild(this);
        }

        /// <summary>
        /// Validates the aot readiness.
        /// </summary>
        void ValidateAotReadiness()
        {
            ValidateAotHandler(() => this.Load);
            ValidateAotHandler(() => this.BeforeInstall);
            ValidateAotHandler(() => this.AfterInstall);
            ValidateAotHandler(() => this.UnhandledException);
        }

        static void ValidateAotHandler<T>(Expression<Func<T>> expression)
        {
            // check if the types declaring event handlers are public or handlers assemblies are
            // listed in this assembly `InternalsVisibleTo` attribute (usually in AssemblyInfo.cs file)
            // Debug.Assert(false);

            var name = Reflect.NameOf(expression);
            var handler = expression.Compile()() as Delegate;

            if (handler != null)
                foreach (var type in handler.GetInvocationList().Select(x => x.Method.DeclaringType))
                {
                    if (type.RootDeclaringType().IsNotPublic)
                    {
                        var friendAsm = type.Assembly.GetName().Name + ".aot";
                        var markedAsFriendly = type.Assembly.GetCustomAttributes(false)
                            .OfType<InternalsVisibleToAttribute>()
                            .Any(x => x.AssemblyName == friendAsm);

                        if (!markedAsFriendly)
                        {
                            var error =
                                $"Event handler of `{type.FullName}` is invisible at runtime. " +
                                $"Either make it public or mark `{type.Assembly.GetName().Name}` assembly as visible with " +
                                $"`[assembly: InternalsVisibleTo(assemblyName: \"{type.Assembly.GetName().Name}.aot\")]` " +
                                $"attribute (e.g. in the AssemblyInfo.cs file).";

                            Compiler.OutputWriteLine($"Error: " + error);
                            throw new Exception(error);
                        }
                    }
                }
        }

        /// <summary>
        /// Gets the UI dialogs dependencies.
        /// </summary>
        /// <param name="ui">The UI.</param>
        /// <returns></returns>
        string[] GetUiDialogsDependencies(IManagedUI ui)
        {
            var dependsOnWpfDialogsBase = ui.InstallDialogs
                                            .Combine(ui.ModifyDialogs)
                                            .SelectMany(x => x.Assembly.GetReferencedAssemblies())
                                            .Any(a => a.Name.StartsWith("WixSharp.UI.WPF"));

            var usngWpfStockDialogs = ui.InstallDialogs
                                        .Combine(ui.ModifyDialogs)
                                        .Any(a => a.Assembly.GetName().Name == "WixSharp.UI.WPF");

            if (usngWpfStockDialogs || dependsOnWpfDialogsBase)
            {
                var result = new List<string>();

                // Caliburn.Micro renamed Caliburn.Micro.dll into Caliburn.Micro.Core.dll in the 4.0 version.
                // Or any other MVVM framework that is used by the WPF dialogs.

                // bool TryToLoad(string asmName)
                // {
                //     try
                //     {
                //         result.Add(System.Reflection.Assembly.Load(asmName).Location);
                //         return true;
                //     }
                //     catch { }
                //     return false;
                // }

                return result.ToArray();
            }
            else
                return new string[0];
        }

        void InjectDialogs(string name, ManagedDialogs dialogs)
        {
            if (dialogs.Any())
            {
                var dialogsInfo = new StringBuilder();

                foreach (var item in dialogs)
                {
                    if (!this.DefaultRefAssemblies.Contains(item.Assembly.GetLocation()))
                    {
                        this.DefaultRefAssemblies.Add(item.Assembly.GetLocation());
                        try
                        {
                            var asm = Type.GetType("WixToolset.Mba.Core.Engine")?.Assembly?.Location;
                            if (asm != null)
                                this.DefaultRefAssemblies.Add(asm);
                        }
                        catch { }
                    }

                    var info = GetDialogInfo(item);

                    ValidateDialogInfo(info);
                    dialogsInfo.Append(info + "\n");
                }
                this.AddProperty(new Property(name, dialogsInfo.ToString().Trim()));
            }
        }

        /// <summary>
        /// Reads and returns the dialog types from the string definition. This method is to be used
        /// by WixSharp assembly.
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
                GetDialog(info);
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

        /// <summary>
        /// Gets the dialog type specified by a vertical pipe-concatenated assembly name and class.
        /// </summary>
        /// <param name="info">Vertical pipe-concatenated assembly name and class.</param>
        /// <returns><Dialog type./returns>
        internal static Type GetDialog(string info)
        {
            string[] parts = info.Split('|');

            string assemblyName = parts[0];
            string dialogTypeName = parts[1];

            //
            // Try to load the assembly first from execution context.
            //
            // When not yet available, load dynamically.
            //
            System.Reflection.Assembly assembly;

            assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.StartsWith(assemblyName));

            if (assembly == null)
            {
                try
                {
                    assembly = System.Reflection.Assembly.Load(assemblyName);
                }
                catch (FileLoadException flex)
                {
                    Exception newEx = new Exception
                    ($"The assembly with name '{assemblyName}' could not be loaded dynamically for retrieval of the type for '{dialogTypeName}'. "
                      + $" The fusion log was: '{flex.FusionLog}'. "
                      + $"Make sure the loaded file '{flex.FileName}' is contained in the installation in a loadable fashion."
                    , innerException: flex
                    );

                    newEx.Data.Add($"{nameof(GetDialog)}-info", info);

                    throw newEx;
                }
                catch (Exception ex)
                {
                    Exception newEx = new Exception
                    ( $"The assembly with name '{assemblyName}' could not be loaded dynamically for retrieval of the type for '{dialogTypeName}'. "
                      + "Make sure it is contained in the installation in a loadable fashion."
                    , innerException: ex
                    );

                    newEx.Data.Add($"{nameof(GetDialog)}-info", info);

                    throw newEx;
                }
            }

            //
            // Retrieve the type of the dialog.
            //
            var dialogType = assembly.GetType(dialogTypeName);
            if (dialogType == null)
            {
                throw new Exception
                ( $"Cannot instantiate '{dialogTypeName}'. "
                  + $"Make sure you added this type assembly to your setup with 'project.{nameof(DefaultRefAssemblies)}'."
                );
            }

            return dialogType;
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

            // Try to see if it's already loaded. `Assembly.Load(name)` still loads from the file if
            // found. Even though the file-less (or renamed) assembly is loaded. Yep, it's not what
            // one would expect.
            //
            // If not done this way it might locks the asm file, which in turn leads to the problems
            // if host is a cs-script app.

            var assembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.StartsWith(parts[0]))
                           ?? System.Reflection.Assembly.Load(parts[0]);

            Type type = null;

            // Ideally need to iterate through the all types in order to find even private ones
            // Though loading some internal (irrelevant) types can fail because of the dependencies.
            try
            {
                type = assembly.GetTypes().Single(t => t.FullName == parts[1]);
            }
            catch { }

            // If we failed to iterate through the types then try to load the type explicitly.
            // Though in this case it has to be public.
            if (type == null)
                type = assembly.GetType(parts[1]);

            var method = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Static)
                             .Single(m => m.Name.Split('|').First() == parts[2]);

            return method;
        }

        internal static void InvokeClientHandlers(string eventName, Session session, Exception e)
        {
            var args = new ExceptionEventArgs { Session = session, Exception = e };
            var handlerName = $"WixSharp_{eventName}_Handlers";
            try
            {
                string handlersInfo = args.Session.Property(handlerName);

                if (handlersInfo.IsNotEmpty())
                {
                    foreach (string item in handlersInfo.Trim().Split('\n').Select(x => x.Trim()))
                    {
                        MethodInfo method = GetHandler(item);
                        method.Call(args);
                    }
                }
            }
            catch (Exception ex)
            {
                args.Session.Log($"WixSharp failed to invoke {handlerName} :" + Environment.NewLine + ex.ToPublicString());
            }
        }

        /// <summary>
        /// Invokes the MSI event handlers.
        /// <p>
        /// This method is not intended to be used by the WixSharp users but by WixSharp internally.
        /// Though it is public because it needs to be invoked by other assemblies.
        /// </p>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns></returns>
        public static ActionResult InvokeClientHandlersExternally(Session session, string eventName)
        {
            // Debug.Assert(false);
            var sessionFile = Path.GetTempFileName();
            var logFile = sessionFile + ".log";
            var eventHost = System.Reflection.Assembly.GetExecutingAssembly().Location.PathChangeFileName("WixSharp.MsiEventHost.exe");

            var evenHandler = "WixSharp_{0}_Handlers".FormatWith(eventName);

            ActionResult result = default;
            try
            {
                var sessionData = session.Serialize(evenHandler);
                IO.File.WriteAllText(sessionFile, sessionData);

                // Note, we are using eventHost executed with "runas". The proper Windows elevation scenario requires
                // the executable to be compiled with the elevation request embedded in the exe manifest.
                // however in this case Windows Defender may block the execution of the exe. Even though the file is signed,
                // executed from the elevated context.
                // And yet it is happy to allow the execution of the exe if it is executed with "runas".
                // Yes that crazy!!!!

                using (var process = eventHost.StartElevated($"-event:{eventName} \"-session:{sessionFile}\" \"-log:{logFile}\""))
                {
                    process.WaitForExit();

                    var output = "";
                    if (logFile.FileExists())
                        output = IO.File.ReadAllText(logFile);

                    // ExitCode == 0 is success regardless of output
                    // ExitCode != 0 && output.HasText is failure as the exception was thrown in the external process
                    // ExitCode != 0 && output.IsEmpty is a normal flow where the ExitCode may simply carry user the user cancellation value
                    if (process.ExitCode != 0 && output.IsNotEmpty())
                        throw new Exception(output);

                    bool shoudReadPropertiesBack = false; // not sure if this is needed

                    if (shoudReadPropertiesBack)
                    {
                        var updatedSessionData = IO.File.ReadAllText(sessionFile);
                        if (updatedSessionData != sessionData)
                            session.DeserializeAndUpdateFrom(updatedSessionData);
                    }

                    return (ActionResult)process.ExitCode;
                }
            }
            catch (Exception e)
            {
                session.Log("WixSharp aborted the session because of the error:" + Environment.NewLine + e.ToPublicString());
                if (session.AbortOnError())
                    result = ActionResult.Failure;

                ManagedProject.InvokeClientHandlers("UnhandledException", session, e);
            }
            finally
            {
                sessionFile.DeleteIfExists();
                logFile.DeleteIfExists();
            }

            return result;
        }

        /// <summary>
        /// Invokes the msi event handlers.
        /// <p>
        /// This method is not intended to be used by the WixSharp users but by WixSharp internally.
        /// Though it is public because it needs to be invoked by other assemblies.
        /// </p>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="UIShell">The UI shell.</param>
        /// <returns></returns>
        public static ActionResult InvokeClientHandlers(Session session, string eventName, IShellView UIShell = null)
        {
            // Debug.Assert(false);
            var runAsElevated = session.Property("ManagedProjectElevatedEvents").Split('|').Contains(eventName);

            ActionResult result = runAsElevated ?
                InvokeClientHandlersExternally(session, eventName) :
                InvokeClientHandlersInternally(session, eventName, UIShell);

            return result;
        }

        /// <summary>
        /// Invokes the MSI event handlers.
        /// <p>
        /// This method is not intended to be used by the WixSharp users but by WixSharp internally.
        /// Though it is public because it needs to be invoked by other assemblies.
        /// </p>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="UIShell">The UI shell.</param>
        /// <returns></returns>
        public static ActionResult InvokeClientHandlersInternally(Session session, string eventName, IShellView UIShell)
        {
            var eventArgs = Convert(session);
            eventArgs.ManagedUI = UIShell;

            try
            {
                string handlersInfo = session.Property("WixSharp_{0}_Handlers".FormatWith(eventName));

                if (!string.IsNullOrEmpty(handlersInfo))
                {
                    foreach (string item in handlersInfo.Trim().Split('\n').Select(x => x.Trim()))
                    {
                        MethodInfo method = GetHandler(item);
                        method.Call(eventArgs);

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

                ManagedProject.InvokeClientHandlers("UnhandledException", session, e);
            }
            return eventArgs.Result;
        }

        internal static string[] SessionSerializableProperties = new[]
        {
            "Installed",
            "REMOVE",
            "ProductName",
            "ProductCode",
            "UpgradeCode",
            "REINSTALL",
            "MsiFile",
            "UPGRADINGPRODUCTCODE",
            "FOUNDPREVIOUSVERSION",
            "UILevel",
            "WIXSHARP_MANAGED_UI",
            "WIXSHARP_MANAGED_UI_HANDLE",
        };

        internal static ActionResult Init(Session session)
        {
            //System.Diagnostics.Debugger.Launch();

            // need to push these properties into runtime data so it is available in the deferred actions
            var data = new SetupEventArgs.AppData();
            try
            {
                foreach (var name in SessionSerializableProperties)
                    data[name] = session.Property(name);
            }
            catch (Exception e)
            {
                session.Log(e.Message);
            }

            data.MergeReplace(session.Property("WIXSHARP_RUNTIME_DATA"));

            session.SetProperty("WIXSHARP_RUNTIME_DATA", data.ToString());

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
                if (!session.IsDisconnected())
                    session.CustomActionData.SetEnvironmentVariables();
            }
            catch (Exception e)
            {
                if (session.IsActive())
                    session.Log(e.Message);
            }
            return result;
        }
    }
}
