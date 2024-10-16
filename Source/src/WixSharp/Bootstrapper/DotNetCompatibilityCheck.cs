using System;
using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Represents a class for checking .NET compatibility.
    /// <example>The following is an example adding DotNetCompatibilityCheck as an XML element.>
    /// <code>
    /// bundle.AddXml(new DotNetCompatibilityCheck(
    ///     "DOTNET_RUNTIME_CHECK",
    ///     RollForward.latestMinor,
    ///     RuntimeType.desktop,
    ///     Platform.x64,
    ///     new Version(8, 0, 0, 0)));
    /// </code>
    /// </example>
    ///
    /// <example>The following is an example adding DotNetCompatibilityCheck as an <see cref="IXmlAware"/> entity.>
    /// <code>
    /// bundle.GenericItems.Add(new DotNetCompatibilityCheck(
    ///     "DOTNET_RUNTIME_CHECK",
    ///     RollForward.latestMinor,
    ///     RuntimeType.desktop,
    ///     Platform.x64,
    ///     new Version(8, 0, 0, 0)));
    /// </code>
    /// </example>
    /// </summary>
    public class DotNetCompatibilityCheck : WixEntity, IXmlAware, IGenericEntity
    {
        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixEntity" />.
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para><para>If the <see cref="Id" /> value is not specified explicitly by the user the Wix# compiler
        /// generates it automatically insuring its uniqueness.</para><remarks>
        /// Note: The ID auto-generation is triggered on the first access (evaluation) and in order to make the id
        /// allocation deterministic the compiler resets ID generator just before the build starts. However if you
        /// accessing any auto-id before the Build*() is called you can it interferes with the ID auto generation and eventually
        /// lead to the WiX ID duplications. To prevent this from happening either:
        /// <para> - Avoid evaluating the auto-generated IDs values before the call to Build*()</para><para> - Set the IDs (to be evaluated) explicitly</para><para> - Prevent resetting auto-ID generator by setting WixEntity.DoNotResetIdGenerator to true";</para></remarks>
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [Xml]
        public new string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Gets the name of the property to set.
        /// </summary>
        [Xml]
        public string Property;

        /// <summary>
        /// Gets the roll forward policy for the compatibility check.
        /// </summary>
        [Xml]
        public RollForward RollForward;

        /// <summary>
        /// Gets the runtime type for the compatibility check.
        /// </summary>
        [Xml]
        public RuntimeType RuntimeType;

        /// <summary>
        /// Gets the platform for the compatibility check.
        /// </summary>
        [Xml]
        public Platform Platform;

        /// <summary>
        /// Gets the version for the compatibility check.
        /// </summary>
        [Xml]
        public Version Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetCompatibilityCheck"/> class.
        /// </summary>
        public DotNetCompatibilityCheck()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetCompatibilityCheck"/> class.
        /// </summary>
        /// <param name="id">The identifier of the compatibility check.</param>
        /// <param name="property">The name of the property to set.</param>
        /// <param name="rollForward">The roll forward policy.</param>
        /// <param name="runtime">The runtime type.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="version">The version.</param>
        public DotNetCompatibilityCheck(string id, string property, RollForward rollForward, RuntimeType runtime, Platform platform, Version version)
        {
            Id = id;
            Property = property;
            RollForward = rollForward;
            RuntimeType = runtime;
            Platform = platform;
            Version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetCompatibilityCheck"/> class.
        /// </summary>
        /// <param name="property">The name of the property to set.</param>
        /// <param name="rollForward">The roll forward policy.</param>
        /// <param name="runtime">The runtime type.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="version">The version.</param>
        public DotNetCompatibilityCheck(string property, RollForward rollForward, RuntimeType runtime, Platform platform, Version version)
        {
            Property = property;
            RollForward = rollForward;
            RuntimeType = runtime;
            Platform = platform;
            Version = version;
        }

        /// <summary>
        ///  Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
            => new XElement("Fragment", this.ToXElement(WixExtension.NetFx.ToXName("DotNetCompatibilityCheck")));

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project" />. See 'Wix#/samples/Extensions' sample for the details on how
        /// to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            // context.XParent is not inserted in the XML doc yet so its parent (XML root) is not available.
            // Thus the line below will not work:
            // context.XParent.Parent.Add(new XElement("Fragment", this.ToXml()));

            // Thus instead of injecting the element in the XParent directly schedule the injection event when the doc
            // is generated

            context.Project.Include(WixExtension.NetFx);
            context.Project.WixSourceGenerated += (doc) =>
                    doc.Root.Add(this.ToXml());
        }
    }
}