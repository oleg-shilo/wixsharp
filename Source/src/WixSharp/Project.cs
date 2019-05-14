#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    // Wix/Msi bug/limitation: every component that is to be placed in the user profile has to have Registry key
    // Wix# places dummy key into every component to handle the problem
    // Wix# auto-generates components contain RemoveFolder elements for all subfolders in the path chain.
    // All auto-generates components are automatically inserted in all features

    /// <summary>
    /// Represents Wix# project. This class defines the WiX/MSI entities and their relationships.
    /// <para>
    /// 		<see cref="Project"/> instance can be compiled into complete MSI or WiX source file with one of the <see cref="Compiler"/> "Build" methods.
    /// </para>
    /// 	<para>
    /// Use <see cref="Project"/> non-default constructor or C# initializers to specify required installation components.
    /// </para>
    /// </summary>
    /// <example>
    /// 	<code>
    /// var project = new Project("MyProduct",
    ///                           new Dir(@"%ProgramFiles%\My Company\My Product",
    ///                                     new File(@"Files\Bin\MyApp.exe"),
    ///                                     new Dir(@"Docs\Manual",
    ///                                     new File(@"Files\Docs\Manual.txt"))));
    ///
    /// project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
    /// project.BuildMsi();
    /// </code>
    /// </example>
    public class Project : WixProject
    {
        internal new string ComponentId(string seed)
        {
            // Component id must be globally unique. Otherwise other products can
            // accidentally trigger MSI ref-counting by installing more than one product
            // with the same component id.
            //
            // The problem is caused by the mind bending MSi concept that a identity does
            // not logically belong to the product but to the target system. Thus if two
            // different products happens too have the component with the same id MSI will
            // treat the components as the same component.
            // Using GUID seems to be a good technique to overcome this limitation but the
            // the vast majority of the samples (e.g. WiX) use human readable ids, which are
            // of course not unique.
            //
            // The excellent reading on the topic can be found here:
            // http://geekswithblogs.net/akraus1/archive/2011/06/17/145898.aspx
            // Ideally we would want to keep readability and uniqueness. Thus the solution
            // is to merge "human readable" id with the project guid.

            if (Compiler.AutoGeneration.ForceComponentIdUniqueness)
                return $"{seed}.{this.UpgradeCode.ToString().Replace("-", "")}";
            else
                return seed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        public Project()
        {
            if (!Compiler.AutoGeneration.LegacyDefaultIdAlgorithm)
                this.CustomIdAlgorithm = this.HashedTargetPathIdAlgorithm;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="name">The name of the project. Typically it is the name of the product to be installed.</param>
        /// <param name="items">The project installable items (e.g. directories, files, registry keys, Custom Actions).</param>
        public Project(string name, params WixObject[] items)
        {
            if (!Compiler.AutoGeneration.LegacyDefaultIdAlgorithm)
                this.CustomIdAlgorithm = this.HashedTargetPathIdAlgorithm;

            Name = name;
            OutFileName = name;

            var dirs = new List<Dir>();
            var actions = new List<Action>();
            var regs = new List<RegValue>();
            var props = new List<Property>();
            var bins = new List<Binary>();
            var genericItems = new List<IGenericEntity>();

            if (items.OfType<Media>().Any())
                this.Media.Clear();

            foreach (WixObject obj in items)
            {
                var rawItems = new List<WixObject>();
                if (obj is WixItems)
                    rawItems.AddRange((obj as WixItems).Items);
                else
                    rawItems.Add(obj);

                foreach (WixObject item in rawItems)
                {
                    if (item is LaunchCondition)
                        LaunchConditions.Add(item as LaunchCondition);
                    else if (item is Dir)
                        dirs.Add(item as Dir);
                    else if (item is Action)
                        actions.Add(item as Action);
                    else if (item is RegValue)
                        regs.Add(item as RegValue);
                    else if (item is RegFile)
                    {
                        var file = item as RegFile;
                        var values = Tasks.ImportRegFile(file.Path);
                        if (file.ActualFeatures.Any())
                            values.ForEach(x =>
                            {
                                x.Feature = file.Feature;
                                x.Features = file.Features;
                            });
                        regs.AddRange(values);
                    }
                    else if (item is Property || item is PropertyRef)
                        props.Add(item as Property);
                    else if (item is Binary)
                        bins.Add(item as Binary);
                    else if (item is WixGuid)
                        GUID = (item as WixGuid).Value;
                    else if (item is Media)
                        Media.Add(item as Media);
                    else if (item is IGenericEntity)
                        genericItems.Add(item as IGenericEntity);
                    else
                        throw new Exception("Unexpected object type is among Project constructor arguments: " + item.GetType().Name);
                }
            }

            Dirs = dirs.ToArray();
            Actions = actions.ToArray();
            RegValues = regs.ToArray();
            Properties = props.ToArray();
            Binaries = bins.ToArray();
            GenericItems = genericItems.ToArray();
        }

        /// <summary>
        /// The product full name or description.
        /// </summary>
        public string Description = "";

        /// <summary>
        /// Parameters of digitaly sign of this project
        /// </summary>
        public DigitalSignature DigitalSignature;

        internal virtual void Preprocess()
        {
            var managedActions = this.Actions.OfType<ManagedAction>()
                                             .Select(x => new { Action = x, Asm = x.ActionAssembly })
                                             .GroupBy(x => x.Asm)
                                             .ToDictionary(x => x.Key);

            foreach (var uniqueAsm in managedActions.Keys)
            {
                var actions = managedActions[uniqueAsm].Select(x => x.Action).ToArray();
                var refAsms = actions.SelectMany(a => a.RefAssemblies).Distinct().ToArray();
                actions.ForEach(a => a.RefAssemblies = refAsms);
            }

            if (WixSharp.Compiler.AutoGeneration.Map64InstallDirs && this.Platform.HasValue && this.Platform.Value == WixSharp.Platform.x64)
            {
                foreach (var dir in this.AllDirs)
                {
                    dir.Name = dir.Name.Map64Dirs();
                    foreach (Shortcut s in dir.Shortcuts)
                        s.Location = s.Location.Map64Dirs();
                }

                foreach (Shortcut s in this.AllFiles.SelectMany(f => f.Shortcuts))
                {
                    s.Location = s.Location.Map64Dirs();
                }

                foreach (var action in this.Actions.OfType<PathFileAction>())
                {
                    action.AppPath = action.AppPath.Map64Dirs();
                }
            }
        }

        /// <summary>
        /// Generic <see cref="T:WixSharp.WixEntity"/> container for defining WiX <c>Package</c> element attributes.
        /// <para>These attributes are the properties about the package to be placed in the Summary Information Stream. These are visible from COM through the IStream interface, and these properties can be seen on the package in Explorer. </para>
        ///<example>The following is an example of defining the <c>Package</c> attributes.
        ///<code>
        /// var project =
        ///     new Project("My Product",
        ///         new Dir(@"%ProgramFiles%\My Company\My Product",
        ///
        ///     ...
        ///
        /// project.Package.AttributesDefinition = @"AdminImage=Yes;
        ///                                          Comments=Release candidate;
        ///                                          Description=Fantastic product...";
        ///
        /// Compiler.BuildMsi(project);
        /// </code>
        /// </example>
        /// </summary>
        public Package Package = new Package();

        /// <summary>
        /// The target platform type.
        /// </summary>
        public Platform? Platform;

        /// <summary>
        /// Collection of Media generic <see cref="T:WixSharp.WixEntity"/> containers for defining WiX <c>Media</c> elements
        /// attributes. Project is always initialized with a sinle Media item. Though if you add multiples media items via
        /// <see cref="T:WixSharp.Project"/> constructor it remeve the initial Media item befeore adding any new items.
        /// <para>These attributes describe a disk that makes up the source media for the installation.</para>
        ///<example>The following is an example of defining the <c>Package</c> attributes.
        ///<code>
        /// var project =
        ///     new Project("My Product",
        ///         new Dir(@"%ProgramFiles%\My Company\My Product",
        ///
        ///     ...
        ///
        /// project.Media.First().Id = 2;
        /// project.Media.First().CompressionLevel = CompressionLevel.mszip;
        ///
        /// Compiler.BuildMsi(project);
        /// </code>
        /// </example>
        /// </summary>
        public List<Media> Media = new List<Media>(new[] { new Media() });

        /// <summary>
        /// The REINSTALLMODE property is a string that contains letters specifying the type of reinstall to perform.
        /// Options are case-insensitive and order-independent. This property should normally always be used in
        /// conjunction with the REINSTALL property.
        /// <para>Note, REINSTALLMODE property will be created only in the automatically produced WiX definition file
        /// only if <see cref="WixSharp.Project.MajorUpgrade"/> is set.</para>
        /// <para>Read more: https://docs.microsoft.com/en-us/windows/desktop/msi/reinstallmode </para>
        /// </summary>
        public string ReinstallMode = "amus";

        /// <summary>
        /// Relative path to RTF file with the custom licence agreement to be displayed in the Licence dialog.
        /// If this value is not specified the default WiX licence agreement will be used.
        /// </summary>
        public string LicenceFile = "";

        /// <summary>
        /// The Encoding to be used for MSI UI dialogs. If not specified the
        /// <c>System.Text.Encoding.UTF8</c> will be used.
        /// </summary>
        public Encoding Encoding = Encoding.UTF8;

        /// <summary>
        /// Type of the MSI User Interface. This value is assigned to the <c>UIRef</c> WiX element during the compilation.
        /// If not specified <see cref="WUI.WixUI_Minimal"/> will used.
        /// </summary>
        public WUI UI = WUI.WixUI_Minimal;

        /// <summary>
        /// The Binary (assembly) implementing WiX embedded UI
        /// </summary>
        public Binary EmbeddedUI = null;

        /// <summary>
        /// The custom UI definition. Use CustomUIBuilder to generate the WiX UI definition or compose
        /// <see cref="WixSharp.Controls.CustomUI"/> manually.
        /// </summary>
        public Controls.CustomUI CustomUI = null;

        /// <summary>
        /// Simplifies authoring for major upgrades, including support for preventing downgrades.
        /// </summary>
        public MajorUpgrade MajorUpgrade = null;

        /// <summary>
        /// This is the value of the <c>UpgradeCode</c> attribute of the Wix <c>Product</c> element.
        /// <para>Both WiX and MSI consider this element as optional even it is the only available identifier
        /// for defining relationship between different versions of the same product. Wix# in contrary enforces
        /// that value to allow any future updates of the product being installed.
        /// </para>
        /// <para>
        /// If user doesn't specify this value Wix# engine will use <see cref="Project.GUID"/> as <c>UpgradeCode</c>.
        /// </para>
        /// </summary>
        public Guid? UpgradeCode;

        private Guid? guid;

        /// <summary>
        /// This value uniquely identifies the software product being installed.
        /// <para>
        /// All setup build scripts for different versions of the same product should have the same <see cref="GUID"/>.
        /// If user doesn't specify this value Wix# engine will generate new random GUID for it.
        /// </para>
        /// <remarks>This value should not be confused with MSI <c>ProductId</c>, which is in fact
        /// not an identifier of the product but rather an identifier of the product particular version.
        /// MSI uses <c>UpgradeCode</c> as a common identification of the product regardless of it's version.
        /// <para>In a way <see cref="GUID"/> is an alias for <see cref="UpgradeCode"/>.</para>
        /// </remarks>
        /// </summary>
        public Guid? GUID
        {
            get { return guid; }
            set
            {
                guid = value;
                ResetWixGuidStartValue();
            }
        }

        internal void ResetWixGuidStartValue()
        {
            WixGuid.ConsistentGenerationStartValue = this.ProductId ?? this.GUID ?? Guid.NewGuid();
        }

        internal void ResetAutoIdGeneration(bool supressWarning)
        {
            ResetWixGuidStartValue();
            WixEntity.ResetIdGenerator(supressWarning);
        }

        /// <summary>
        ///  Set of values in 'Add/Remove Programs' of Control Panel.
        /// </summary>
        public ProductInfo ControlPanelInfo = new ProductInfo();

        /// <summary>
        /// Provides fine control over rebooting at the end of installation.
        /// <para>If set it creates <c>ScheduleReboot</c> element in the <c>InstallExecuteSequence</c> or <c>InstallUISequence</c>.</para>
        /// </summary>
        /// <example>
        /// <code>
        /// var project = new Project("MyProduct",
        ///                   new Dir("%ProgramFiles%",
        ///                   ...
        /// project.ScheduleReboot = new ScheduleReboot { InstallSequence = RebootInstallSequence.Both };
        /// </code>
        /// </example>
        public ScheduleReboot ScheduleReboot;

        /// <summary>
        /// Provideds fine control over rebooting at the end of installation.
        /// <para>If set it creates <c>ForceReboot</c> element in the <c>InstallExecuteSequence</c>.</para>
        /// </summary>
        /// <example>
        /// <code>
        /// var project = new Project("MyProduct",
        ///                   new Dir("%ProgramFiles%",
        ///                   ...
        /// project.ForceReboot = new ForceReboot();
        /// </code>
        /// </example>
        public ForceReboot ForceReboot;

        /// <summary>
        /// Provides fine control over rebooting at the end of installation.
        /// <para>If set it creates MSI <c>REBOOT</c> property with the user specified value <see cref="RebootSupressing"/>.</para>
        /// </summary>
        /// <example>
        /// <code>
        /// var project = new Project("MyProduct",
        ///                   new Dir("%ProgramFiles%",
        ///                   ...
        /// project.RebootSupressing = RebootSupressing.ReallySuppress;
        /// </code>
        /// </example>
        public RebootSupressing? RebootSupressing;

        /// <summary>
        /// Use this attribute if you need to specify the installation scope of this package: per-machine or per-user.
        /// </summary>
        public InstallScope? InstallScope;

        /// <summary>
        /// Use this attribute to specify the privileges required to install the package on Windows Vista and above.
        /// </summary>
        public InstallPrivileges? InstallPrivileges;

        /// <summary>
        /// Version of the product to be installed.
        /// </summary>
        public Version Version = new Version("1.0.0.0");

        /// <summary>
        /// Defines Major Upgrade behavior. By default it is <c>null</c> thus upgrade is not supported.
        /// <para>If you need to implement Major Upgrade define this member to appropriate
        /// <see cref="MajorUpgradeStrategy"/> instance.</para>
        /// <para><c>Note</c>: <see cref="MajorUpgradeStrategy"/> yields WiX UpgradeVersion element, which is arguably the most comprehensive
        /// upgrade definition. However in the later versions of WiX a simplified upgrade definition has been introduced. It relies
        /// on MajorUpgrade WiX element. For most of the upgrade scenarios you will find that MajorUpgrade allows to achieve the same result with
        /// much less effort. Wix# supports MajorUpgrade element via  <see cref="WixSharp.Project.MajorUpgrade"/> member.</para>
        /// </summary>
        ///<example>The following is an example of building product MSI with auto uninstalling any older version of the product
        ///and preventing downgrading.
        ///<code>
        /// var project = new Project("My Product",
        ///                   ...
        ///
        /// project.MajorUpgradeStrategy =  new MajorUpgradeStrategy
        ///                                 {
        ///                                     UpgradeVersions = VersionRange.OlderThanThis,
        ///                                     PreventDowngradingVersions = VersionRange.NewerThanThis,
        ///                                     NewerProductInstalledErrorMessage = "Newer version already installed",
        ///                                 };
        /// Compiler.BuildMsi(project);
        /// </code>
        /// or the same scenario but using predefined <c>MajorUpgradeStrategy.Default</c> strategy.
        ///<code>
        /// project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
        /// </code>
        /// You can also specify custom range of versions:
        ///<code>
        /// project.MajorUpgradeStrategy =  new MajorUpgradeStrategy
        ///                                 {
        ///                                     UpgradeVersions = new VersionRange
        ///                                                           {
        ///                                                              Minimum = "2.0.5.0", IncludeMaximum = true,
        ///                                                              Maximum = "%this%", IncludeMaximum = false
        ///                                                           },
        ///                                     PreventDowngradingVersions = new VersionRange
        ///                                                           {
        ///                                                              Minimum = "%this%", IncludeMinimum = false
        ///                                                           },
        ///                                     NewerProductInstalledErrorMessage = "Newer version already installed",
        ///                                 };
        /// </code>
        /// Note that %this% will be replaced by Wix# compiler with <c>project.Version.ToString()</c> during the MSI building.
        /// However you can use explicit values (e.g. 1.0.0) if you prefer.
        /// </example>
        public MajorUpgradeStrategy MajorUpgradeStrategy;

        /// <summary>
        /// Generates all missing product Guids (e.g. <see cref="UpgradeCode"/> and <see cref="ProductId"/>).
        /// <para>Wix# compiler call this method just before building the MSI. However you can call it any time
        /// if you want to preview auto-generated Guids.</para>
        /// </summary>
        public void GenerateProductGuids()
        {
            if (!GUID.HasValue)
                GUID = Guid.NewGuid();

            if (!UpgradeCode.HasValue)
                UpgradeCode = GUID;

            if (!ProductId.HasValue)
                ProductId = CalculateProductId(guid.Value, Version);
        }

        /// <summary>
        /// Calculates the product id.
        /// </summary>
        /// <param name="productGuid">The product GUID.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public static Guid CalculateProductId(Guid productGuid, Version version)
        {
            return WixGuid.HashGuidByInteger(productGuid, version.GetHashCode() + 1);
        }

        /// <summary>
        /// This is the value of the <c>Id</c> attribute of the Wix <c>Product</c> element.
        /// This value is unique for any given version of a product being installed.
        /// <para></para>
        /// If user doesn't specify this value Wix# engine will derive it from
        /// project <see cref="Project.GUID"/> and the product <see cref="Project.Version"/>.
        /// </summary>
        public Guid? ProductId;

        /// <summary>
        /// Collection of <see cref="Dir"/>s to be installed.
        /// </summary>
        public Dir[] Dirs = new Dir[0];

        /// <summary>
        /// Collection of <see cref="Actions"/>s to be performed during the installation.
        /// </summary>
        public Action[] Actions = new Action[0];

        /// <summary>
        /// Collection of <see cref="RegValue"/>s to be set during the installation.
        /// </summary>
        public RegValue[] RegValues = new RegValue[0];

        /// <summary>
        /// Collection of <see cref="UrlReservation"/> to be installed.
        /// </summary>
        public UrlReservation[] UrlReservations = new UrlReservation[0];

        /// <summary>
        /// Collection of the user defined <see cref="IGenericEntity"/> items.
        /// </summary>
        public IGenericEntity[] GenericItems = new IGenericEntity[0];

        /// <summary>
        /// Collection of WiX/MSI <see cref="Property"/> objects to be created during the installed.
        /// </summary>
        public Property[] Properties = new Property[0];

        /// <summary>
        /// Indicates whether compiler should emit consistent package Id (package code). Set <c>EmitConsistentPackageId</c> to 'false' (default value) if
        /// you want the WiX compilers automatically generate a new package code for each new .msi file built. Or set it to 'true' if you want Wix# to auto generate a
        /// unique consistent package code for a given combination of the product code, product version and product upgrade code.
        /// <para>
        /// WiX package code generation policy discourages the use of this attribute as it is a primary MSI identifier
        /// used to distinguish packages in ARP. Thus WiX tools always auto-generate the code for each build. This in turn makes it impossible to
        /// rebuild a truly identical MSIs from the same WiX code even with the same set of product code, version and upgrade code.
        /// </para><para>
        /// This very artificial limitation has severe practical impact. For example if a specific MSI file is lost it cannot be recreated even if
        /// the original source code that was used to built the lost MSI is available.
        /// </para><para>
        /// From another hand Wix# encourages using a singe GUID (Project.GUID) as a primary identifier of the product. Thus all other MSI identifiers
        /// can be derived by the compiler from the unique combination of this GUID and the product version. This also included generation of the package Id
        /// attribute controlled by the EmitConsistentPackageId.
        /// </para><para>
        /// Wix# does not changes the WiX default package code generation it just gives the opportunity to control it when required.
        /// </para>
        /// </summary>
        public bool EmitConsistentPackageId = false;

        /// <summary>
        /// Collection of WiX/MSI <see cref="Binary"/> objects to be embedded into MSI database.
        /// Normally you doe not need to deal with this property as <see cref="Compiler"/> will populate
        /// it automatically.
        /// </summary>
        public Binary[] Binaries = new Binary[0];

        /// <summary>
        /// Collection of paths to the assemblies referenced by <see cref="ManagedAction"/>s.
        /// </summary>
        public List<string> DefaultRefAssemblies = new List<string>();

        /// <summary>
        /// Collection of the <see cref="T:WixSharp.LaunchCondition"/>s associated with the setup.
        /// </summary>
        public List<LaunchCondition> LaunchConditions = new List<LaunchCondition>();

        /// <summary>
        /// Path to the file containing the image (e.g. bmp) setup dialogs banner. If not specified default image will be used.
        /// </summary>
        public string BannerImage = "";

        /// <summary>
        /// Path to the file containing the image (e.g. bmp) setup dialogs background. If not specified default image will be used.
        /// <remarks>
        /// <para>If the image is to be used in the default ManagedUI dialogs it will be left-docked at runtime and will effectively
        /// play the role of a left-aligned dialog banner. Thus if it is too wide it can push away (to right) the all other UI elements.
        /// The optimal size of the image for ManagedUI is 494x312.</para>
        /// </remarks>
        /// </summary>
        public string BackgroundImage = "";

        /// <summary>
        /// Performs validation of aspect ratio for <see cref="Project.BackgroundImage"/>. Validation assists with avoiding the situations
        /// when ManagedUI dialog has UI elements 'pushed away' from the view by oversizes <see cref="Project.BackgroundImage"/>.
        /// </summary>
        public bool ValidateBackgroundImage = true;

        private Feature _defaultFeature = new Feature("Complete");

        /// <summary>
        /// Gets or Sets the default Feature for the project.
        /// All elements without an explicitly assigned Feature will be placed under the DefaultFeature.
        /// If DefaultFeature is not set, then DefaultFeature will default to a Feature with name 'Complete'.
        /// </summary>
        public Feature DefaultFeature
        {
            get { return _defaultFeature; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value), "DefaultFeature cannot be null");
                _defaultFeature = value;
            }
        }

        /// <summary>
        /// The wild card deduplication algorithm to be used during wild card resolution (<c>ResolveWildCards</c>).
        /// <para>The default implementation does nothing but you can assgn a custom routine that
        /// can be used to do post-resolving deduplication of the <see cref="Dir"/> items.</para>
        /// <para>
        /// The following sample demonstrates how to remove files with the same file name:
        /// </para>
        /// <code>
        /// project.WildCardDedup = dir =>
        /// {
        ///     if (dir.Files.Any())
        ///         dir.Files = dir.Files.DistinctBy(x => x.Name.PathGetFileName()).ToArray();
        /// };
        ///
        /// // or built-in routine
        ///
        /// project.WildCardDedup = Project.UniqueFileNameDedup;
        /// ...
        /// Compiler.BuildMsi(project);
        /// </code>
        /// <para>Note, the need for <c>project.WildCardDedup</c> may araise only for very specific
        /// deployment scenarios. Some of them are discussed in this thread: https://github.com/oleg-shilo/wixsharp/issues/270
        /// </para>
        /// </summary>
        public Action<Dir> WildCardDedup = dir =>
            {
                // Issue #270: Deduplication of files added with wildcards
                // sample dedup
            };

        /// <summary>
        /// The unique file name deduplication algorithm to be used as <see cref="Project.WildCardDedup"/>.
        /// It implements removing the <see cref="Dir.Files"/> items with the same file name.
        /// <para>The actual algorithm implementation is very simple:
        /// <code>
        /// public static Action&lt;Dir&gt; UniqueFileNameDedup = dir =>
        /// {
        ///     if (dir.Files.Any())
        ///         dir.Files = dir.Files.DistinctBy(x => Path.GetFileName(x.Name)).ToArray();
        /// }
        /// </code>
        /// </para>
        /// </summary>
        public static Action<Dir> UniqueFileNameDedup = dir =>
        {
            // Issue #270: Deduplication of files added with wildcards sample dedup
            if (dir.Files.Any())
                dir.Files = dir.Files.DistinctBy(x => x.Name.PathGetFileName()).ToArray();
        };

        /// <summary>
        /// Resolves all wild card specifications if any.
        /// <para>
        /// This method is called by <see cref="Compiler" /> during the compilation. However it might be convenient
        /// to call it before the compilation if any files matching the wild card mask need to be handled in special
        /// way (e.g. shortcuts created). See <c>WildCard Files</c> example.
        /// </para><remarks><see cref="ResolveWildCards" /> should be called only after <see cref="T:WixSharp.WixProject.SourceBaseDir" /> is set.
        /// Otherwise wild card paths may not be resolved correctly.</remarks>
        /// </summary>
        /// <param name="ignoreEmptyDirectories">if set to <c>true</c> empty directories are ignored and not added to the project.</param>
        /// <returns></returns>
        public Project ResolveWildCards(bool ignoreEmptyDirectories = false)
        {
            int iterator = 0;
            var dirList = new List<Dir>();
            var fileList = new List<File>();

            dirList.AddRange(Dirs);

            while (iterator < dirList.Count)
            {
                foreach (Files dirItems in dirList[iterator].FileCollections)
                {
                    foreach (WixEntity item in dirItems.GetAllItems(SourceBaseDir, dirList[iterator]))
                    {
                        if (item is DirFiles)
                        {
                            dirList[iterator].AddDirFileCollection(item as DirFiles);
                        }
                        else if (item is Dir discoveredDir && !dirList[iterator].Dirs.Contains(item))
                        {
                            WildCardDedup?.Invoke(discoveredDir);
                            dirList[iterator].AddDir(discoveredDir);
                        }
                    }
                }

                foreach (Dir dir in dirList[iterator].Dirs)
                    dirList.Add(dir);

                foreach (DirFiles coll in dirList[iterator].DirFileCollections)
                    dirList[iterator].Files = dirList[iterator].Files.Combine(coll.GetFiles(SourceBaseDir));

                //clear resolved collections
                dirList[iterator].FileCollections = new Files[0];
                dirList[iterator].DirFileCollections = new DirFiles[0];

                iterator++;
            }

            if (ignoreEmptyDirectories)
            {
                IEnumerable<Dir> getEmptyDirs() => AllDirs.Where(d => !d.Files.Any() && !d.Dirs.Any());

                IEnumerable<Dir> emptyDirs;

                while ((emptyDirs = getEmptyDirs()).Any())
                {
                    emptyDirs.ForEach(emptyDir => AllDirs.ForEach(d =>
                                                  {
                                                      if (d.Dirs.Contains(emptyDir))
                                                          d.Dirs = d.Dirs.Where(x => x != emptyDir).ToArray();
                                                  }));
                }
            }

            return this;
        }

        /// <summary>
        /// Returns all <see cref="File"/>s of the <see cref="Project"/> matching the <paramref name="match"/> pattern.
        /// </summary>
        /// <param name="match">The match pattern.</param>
        /// <returns>Matching <see cref="File"/>s.</returns>
        public File[] FindFile(Func<File, bool> match)
        {
            return AllFiles.Where(match).ToArray();
        }

        /// <summary>
        /// Finds the first <see cref="File"/> in the <see cref="Project"/> matching the <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Matching <see cref="File"/>.</returns>
        public File FindFirstFile(string fileName)
        {
            return FindFile(f => f.Name.PathGetFileName().SameAs(fileName, ignoreCase: true)).FirstOrDefault();
        }

        /// <summary>
        /// Flattened "view" of all <see cref="File"/>s of the <see cref="Project"/>.
        /// </summary>
        public File[] AllFiles
        {
            get
            {
                int iterator = 0;
                var dirList = new List<Dir>();
                var fileList = new List<File>();

                dirList.AddRange(Dirs);

                while (iterator < dirList.Count)
                {
                    foreach (Dir dir in dirList[iterator].Dirs)
                        dirList.Add(dir);

                    fileList.AddRange(dirList[iterator].Files);

                    iterator++;
                }

                return fileList.ToArray();
            }
        }

        /// <summary>
        /// Flattened "view" of all <see cref="Dir"/>s of the <see cref="Project"/>.
        /// </summary>
        public Dir[] AllDirs
        {
            get
            {
                int iterator = 0;
                var dirList = new List<Dir>();

                dirList.AddRange(Dirs);

                while (iterator < dirList.Count)
                {
                    dirList.AddRange(dirList[iterator].Dirs);
                    iterator++;
                }

                return dirList.ToArray();
            }
        }

        /// <summary>
        /// Calculates the target path of the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public string GetTargetPathOf(File file)
        {
            var dir = this.AllDirs.FirstOrDefault(d => d.Files.Contains(file));
            var path = new List<string>();
            path.Add(file.Name);
            while (dir != null)
            {
                path.Insert(0, dir.Name);
                dir = this.AllDirs.FirstOrDefault(d => d.Dirs.Contains(dir));
            }
            return path.Join("\\");
        }

        /// <summary>
        /// For future use
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private string HashedIdAlgorithm(WixEntity entity)
        {
            if (entity is File file)
            {
                var target_path = this.GetTargetPathOf(file);
                var hash = target_path.GetHashCode32();

                // WiX does not allow '-' char in ID. So need to use `Math.Abs`
                return $"{target_path.PathGetFileName()}_{Math.Abs(hash)}";
            }
            return null; // next two lines produce the same result
                         // return WixEntity.DefaultIdAlgorithm(entity);
                         // return WixEntity.IncrementalIdFor(entity);
        }

        /// <summary>
        /// Finds <see cref="T:WixSharp.Dir"/> corresponding to the specified path.
        /// </summary>
        /// <example>
        /// <code>
        /// new Project("MyProduct",
        ///     new Dir("%ProgramFiles%",
        ///         new Dir("My Company",
        ///             new Dir("My Product",
        ///             ...
        /// </code>
        /// In the sample above the call <c>FindDir(@"%ProgramFiles%\My Company\My Product")</c> returns the last declared <see cref="T:WixSharp.Dir"/>.
        /// </example>
        /// <param name="path">The path string.</param>
        /// <returns><see cref="T:WixSharp.Dir"/> instance if the search was successful, otherwise return <c>null</c></returns>
        public Dir FindDir(string path)
        {
            int iterator = 0;
            var dirList = new List<Dir>();
            int tokenIndex = 0;
            string[] pathTokens = path.Split("\\/".ToCharArray());

            dirList.AddRange(Dirs);

            while (iterator < dirList.Count)
            {
                string dirName = dirList[iterator].Name.Expand().ToLower();
                string currentSubDir = pathTokens[tokenIndex].Expand().ToLower();
                if (dirName == currentSubDir)
                {
                    if (tokenIndex == pathTokens.Length - 1)
                        return dirList[iterator];

                    dirList.AddRange(dirList[iterator].Dirs);
                    tokenIndex++;
                }
                iterator++;
            }

            return null;
        }

        /// <summary>
        /// The installer version
        /// </summary>
        public int InstallerVersion = 200;

        private string codepage = "";

        /// <summary>
        /// Installation UI Code Page. If not specified
        /// ANSICodePage of the <see cref="T:WixSharp.WixProject.Language"/> will be used.
        /// </summary>
        public string Codepage
        {
            get
            {
                if (!codepage.IsEmpty())
                    return codepage;
                else
                    return Encoding.GetEncoding(new CultureInfo(Language.Split(',', ';').FirstOrDefault()).TextInfo.ANSICodePage).WebName;
            }
            set
            {
                codepage = value;
            }
        }

        /// <summary>
        /// List of culture names (see <see cref="CultureInfo"/>) based on the specified <see cref="T:WixSharp.WixProject.Language"/>
        /// </summary>
        public string Culture
        {
            get
            {
                return string.Join(",", Language.Split(',', ';')
                                                .Select(x => new CultureInfo(x.Trim()).Name)
                                                .ToArray());
            }
        }

        internal bool IsLocalized
        {
            get { return (Language.ToLower() != "en-us" && Language.ToLower() != "en") || !LocalizationFile.IsEmpty(); }
        }

        /// <summary>
        /// Path to the Localization file.
        /// </summary>
        public string LocalizationFile = "";

        /// <summary>
        /// Name (path) of the directory which was assigned <see cref="T:WixSharp.Compiler.AutoGeneration.InstallDirDefaultId"/> ID as part of XML auto-generation (see <see cref="T:WixSharp.AutoGenerationOptions"/>).
        /// </summary>
        public string AutoAssignedInstallDirPath = "";

        internal string ActualInstallDirId = "";

        internal Dir GetLogicalInstallDir()
        {
            Dir firstDirWithItems = Dirs.First();

            string logicalPath = firstDirWithItems.Name;
            while (firstDirWithItems.Shortcuts.Count() == 0 &&
                   firstDirWithItems.Dirs.Count() == 1 &&
                   firstDirWithItems.Files.Count() == 0)
            {
                firstDirWithItems = firstDirWithItems.Dirs.First();
            }

            return firstDirWithItems;
        }

        /// <summary>
        /// Builds the MSI file from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="path">The path to the MSI file to be build.</param>
        /// <returns>Path to the built MSI file.</returns>
        public string BuildMsi(string path = null)
        {
            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            if (path == null)
                return Compiler.BuildMsi(this);
            else
                return Compiler.BuildMsi(this, path);
        }

        /// <summary>
        /// Builds the WiX source file and generates batch file capable of building
        /// MSI with WiX toolset.
        /// </summary>
        /// <param name="path">The path to the batch file to be build.</param>
        /// <returns>Path to the batch file.</returns>
        public string BuildMsiCmd(string path = null)
        {
            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            if (path == null)
                return Compiler.BuildMsiCmd(this);
            else
                return Compiler.BuildMsiCmd(this, path);
        }

        /// <summary>
        /// Builds the WiX source file (*.wxs) from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="path">The path to the WXS file to be build.</param>
        /// <param name="type">The type (<see cref="Compiler.OutputType"/>) of the setup file to be defined in the source file (MSI vs. MSM).</param>
        /// <returns>Path to the built WXS file.</returns>
        public string BuildWxs(Compiler.OutputType type = Compiler.OutputType.MSI, string path = null)
        {
            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            if (path == null)
                return Compiler.BuildWxs(this, type);
            else
                return Compiler.BuildWxs(this, path, type);
        }

        /// <summary>
        /// Builds the MSM file from the specified <see cref="Project"/> instance.
        /// </summary>
        /// <param name="path">The path to the MSM file to be build.</param>
        /// <returns>Path to the built MSM file.</returns>
        public string BuildMsm(string path = null)
        {
            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            if (path == null)
                return Compiler.BuildMsm(this);
            else
                return Compiler.BuildMsm(this, path);
        }

        /// <summary>
        /// Builds the WiX source file and generates batch file capable of building
        /// MSM with WiX toolset.
        /// </summary>
        /// <param name="path">The path to the batch file to be build.</param>
        /// <returns>Path to the batch file.</returns>
        public string BuildMsmCmd(string path = null)
        {
            if (Compiler.ClientAssembly.IsEmpty())
                Compiler.ClientAssembly = System.Reflection.Assembly.GetCallingAssembly().GetLocation();

            if (path == null)
                return Compiler.BuildMsmCmd(this);
            else
                return Compiler.BuildMsmCmd(this, path);
        }

        /// <summary>
        /// Algorithm for generating deterministic <see cref="T:WixSharp.File"/>Id(s) based on the hash of the target path.
        /// <para>This algorithm addresses the limitation of the incremental-Id legacy algorithm, which it quite reliable but
        /// non deterministic.</para>
        /// <remarks>
        /// As any hash, the hash of the target path is not 100% reliable thus if you are packaging very high number
        /// files you may occasionally get duplication. Use alternative custom ID-allocation algorithm (e.g. random GUIDs).
        /// </remarks>
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public string HashedTargetPathIdAlgorithm(WixEntity entity)
        {
            if (entity is File file)
            {
                var target_path = this.GetTargetPathOf(file);

                var dir_hash = Math.Abs(target_path.GetHashCode32());
                var file_name = target_path.PathGetFileName().EscapeIllegalCharacters();

                if (Compiler.AutoGeneration.HashedTargetPathIdAlgorithm_FileIdMask != null)
                    return Compiler.AutoGeneration.HashedTargetPathIdAlgorithm_FileIdMask
                                                  .Replace("{file_name}", file_name)
                                                  .Replace("{dir_hash}", dir_hash.ToString())
                                                  .FormatWith(file_name, dir_hash);
                else
                    return $"{file_name}.{dir_hash}";
            }

            return null; // pass to default Id generator
        }
    }
}