using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
#pragma warning disable 1591

    /// <summary>
    ///  Threading model for the CLSID.
    /// </summary>
    public enum ThreadingModel
    {
        apartment,
        free,
        both,
        neutral,
        single,
        rental
    }

#pragma warning restore 1591

    /// <summary>
    /// Application ID containing Distributed COM (DCOM) information for the associated application GUID.
    /// </summary>
    public class AppId : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Creates a new <see cref="AppId"/> element.
        /// </summary>
        public AppId()
        {
            isAutoId = false;
        }

        private Guid? appId;

        /// <summary>
        /// The GUID that corresponds to the named executable.
        /// </summary>
        [Xml]
        public new Guid? Id
        {
            get => appId;
            set
            {
                appId = value;
                base.Id = value?.ToString();
            }
        }

        /// <summary>
        /// Set this value to true to configure to activate on the same system as persistent storage.
        /// </summary>
        [Xml]
        public bool? ActivateAtStorage;

        /// <summary>
        /// Set this value to true in order to create a normal AppId table row. Set this value to 'no' in order to
        /// generate Registry rows that perform similar registration
        /// (without the often problematic Windows Installer advertising behavior).
        /// </summary>
        [Xml]
        public bool Advertise;

        /// <summary>
        /// Describes the application associated with this AppId.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// If the class is a DLL that needs to be run in an external executable process, this field specifies the path
        /// to the executable.
        /// </summary>
        [Xml]
        public string DllSurrogate;

        /// <summary>
        /// If this field is set, the class will be installed as a service with the name in the field.
        /// </summary>
        [Xml]
        public string LocalService;

        /// <summary>
        /// If an activation function in the class is called and its COSERVERINFO structure is unspecified, it
        /// will be run on the remote server specified in this field.
        /// </summary>
        [Xml]
        public string RemoteServerName;

        /// <summary>
        /// Set this value to true to configure the class to run as the currently logged in and active desktop user.
        /// </summary>
        [Xml]
        public bool RunAsInteractiveUser;

        /// <summary>
        /// Parameters to pass when the class is being run as a service.
        /// </summary>
        [Xml]
        public string ServiceParameters;

        /// <summary>
        /// COM Classes associated with this AppId.
        /// </summary>
        public ComRegistration[] ComClasses;

        /// <summary>
        /// Serializes class into WiX document.
        /// </summary>
        /// <param name="context">Processing context for WiX Document.</param>
        /// <exception cref="ValidationException"></exception>
        public void Process(ProcessingContext context)
        {
            string[] AdvertiseParents = { "Fragment", "Module", Compiler.ProductElementName };

            // If the parent element is a Fragment, Module, or Product, ensure Advertised is true.
            if (!Advertise && AdvertiseParents.Any(s => s == context.XParent.Name.LocalName))
            {
                throw new ValidationException($"If {nameof(AppId)} is a child of the elements "
                    + $"{AdvertiseParents.JoinBy(", ")}, then {nameof(Advertise)} must be set to true.");
            }

            if (Advertise && Description != null)
            {
                throw new ValidationException($"If {nameof(AppId)} is Advertised, {nameof(Description)} cannot be set.");
            }

            if (appId == null)
                throw new ValidationException($"AppId {nameof(Id)} field cannot be null.");

            XElement appIdElement = this.ToXElement("AppId");

            if (ComClasses?.Length > 0)
            {
                var appIdContext = new ProcessingContext
                {
                    Project = context.Project,
                    Parent = this,
                    FeatureComponents = context.FeatureComponents,
                    XParent = appIdElement
                };

                _ = ComClasses.ForEach(comClass => comClass.Process(appIdContext));
            }

            context.XParent.Add(appIdElement);
        }
    }

    /// <summary>
    /// COM Class registration.
    /// </summary>
    /// <example>The following is an example of deploying and registering  a COM server.
    /// <code>
    /// var project =  new Project("MyProduct",
    ///     new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         new File(@"Files\Bin\MyApp.exe",
    ///                 new ComRegistration
    ///                 {
    ///                     Id = new Guid("6f330b47-2577-43ad-9095-1861ba25889b"),
    ///                     Description = "MY DESCRIPTION",
    ///                     ThreadingModel = ThreadingModel.apartment,
    ///                     Context = "InprocServer32",
    ///                     ProgIds = new[]
    ///                     {
    ///                         new ProgId
    ///                         {
    ///                             Id = "PROG.ID.1",
    ///                             Description ="Version independent ProgID ",
    ///                             ProgIds = new[]
    ///                             {
    ///                                 new ProgId
    ///                                 {
    ///                                     Id = "prog.id",
    ///                                     Description="some description"
    ///                                 }
    ///                             }
    ///                         }
    ///                     }
    ///                 })));
    /// </code>
    /// </example>
    /// <seealso cref="WixSharp.WixEntity" />
    /// <seealso cref="WixSharp.IGenericEntity" />
    public class ComRegistration : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Creates a new COM class registration entity.
        /// </summary>
        public ComRegistration()
        {
            isAutoId = false;
        }

        private Guid? clsId;

        /// <summary>
        /// The Class identifier (CLSID) of a COM server.
        /// </summary>
        [Xml]
        public new Guid? Id
        {
            get => clsId;
            set
            {
                clsId = value;
                base.Id = value?.ToString();
            }
        }

        /// <summary>
        /// Set this value to "true" in order to create a normal Class table row. Set this value to "false" in order to
        /// generate Registry rows that perform similar registration (without the often problematic Windows Installer
        /// advertising behavior).
        /// </summary>
        [Xml]
        public bool? Advertise;

        /// <summary>
        /// This attribute is only allowed when a Class is advertised. Using this attribute will reference an
        /// Application ID containing DCOM information for the associated application GUID. The value must correspond
        /// to an AppId/@Id of an AppId element nested under a Fragment, Module, or Product element. To associate an
        /// AppId with a non-advertised class, nest the class within a parent AppId element.
        /// </summary>
        [Xml]
        public Guid? AppId;

        /// <summary>
        /// This column is optional only when the Context column is set to "LocalServer" or "LocalServer32" server
        /// context. The text is registered as the argument against the OLE server and is used by OLE for invoking
        /// the server. Note that the resolution of properties in the Argument field is limited. A property formatted
        /// as [Property] in this field can only be resolved if the property already has the intended value when the
        /// component owning the class is installed. For example, for the argument "[#MyDoc.doc]" to resolve to the
        /// correct value, the same process must be installing the file MyDoc.doc and the component that owns the
        /// class.
        /// </summary>
        [Xml]
        public string Argument;

        /// <summary>
        /// The server context(s) for this COM server. This attribute is optional for VB6 libraries that are marked
        /// "PublicNotCreateable". Class elements marked Advertised must specify at least one server context. It is
        /// most common for there to be a single value for the Context attribute. This attribute's value should be a
        /// space-delimited list congaing one or more of the following:
        /// <list type="bullet">
        /// <item> <term>LocalServer</term>  <description>A 16-bit local server application.</description></item>
        /// <item><term>LocalServer32</term><description>A 32-bit local server application.</description></item>
        /// <item><term>InprocServer</term><description>A 16-bit in-process server DLL.</description></item>
        /// <item><term>InprocServer32</term><description>A 32-bit in-process server DLL.</description></item>
        /// </list>
        /// </summary>
        [Xml]
        public string Context;

        /// <summary>
        /// Set this attribute's value to 'true' to identify an object as an ActiveX Control. The default value is 'false'.
        /// </summary>
        [Xml]
        public bool? Control;

        /// <summary>
        /// The threading model
        /// <list type="bullet">
        /// <item>apartment</item>
        /// <item>free     </item>
        /// <item>both     </item>
        /// <item>neutral  </item>
        /// <item>single   </item>
        /// <item>rental   </item>
        /// </list>
        /// </summary>
        [Xml]
        public ThreadingModel ThreadingModel = ThreadingModel.apartment;

        /// <summary>
        /// Localized description associated with the Class ID and Program ID.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// The default inproc handler. May be optionally provided only for Context = LocalServer or LocalServer32.
        /// Value of "1" creates a 16-bit InprocHandler (appearing as the InprocHandler value). Value of "2" creates a
        /// 32-bit InprocHandler (appearing as the InprocHandler32 value). Value of "3" creates 16-bit as well as
        /// 32-bit InprocHandlers. A non-numeric value is treated as a system file that serves as the 32-bit
        /// InprocHandler (appearing as the InprocHandler32 value).
        /// </summary>
        [Xml]
        public string ForeignServer;

        /// <summary>
        /// The file providing the icon associated with this CLSID. Reference to an Icon element (should match the Id
        /// attribute of an Icon element). This is currently not supported if the value of the Advertise attribute
        /// is "false".
        /// </summary>
        [Xml]
        public string Icon;

        /// <summary>
        /// Icon index into the icon file.
        /// </summary>
        [Xml]
        public int IconIndex;

        /// <summary>
        /// Specifies the CLSID may be insertable.
        /// </summary>
        [Xml]
        public bool? Insertable;

        /// <summary>
        /// Specifies the CLSID may be programmable.
        /// </summary>
        [Xml]
        public bool? Programmable;

        /// <summary>
        /// When the value is "yes", the bare file name can be used for COM servers. The installer registers the file name only instead of the complete path. This enables the server in the current directory to take precedence and allows multiple copies of the same component.
        /// </summary>
        [Xml]
        public bool? RelativePath;

        /// <summary>
        /// The safe for initializing. May only be specified if the value of the Advertise attribute is "false".
        /// </summary>
        [Xml]
        public bool? SafeForInitializing;

        /// <summary>
        /// May only be specified if the value of the Advertise attribute is "no".
        /// </summary>
        [Xml]
        public bool? SafeForScripting;

        /// <summary>
        /// Specifies whether or not to use the short path for the COM server. This can only apply when Advertise is set to 'no'. The default is 'no' meaning that it will use the long file name for the COM server.
        /// </summary>
        [Xml]
        public bool? ShortPath;

        /// <summary>
        /// May only be specified if the value of the Advertise attribute is "no" and the ForeignServer attribute is not specified. File Id of the COM server file. If this element is nested under a File element, this value defaults to the value of the parent File/@Id.
        /// </summary>
        [Xml]
        public string Server;

        /// <summary>
        /// Version for the CLSID.
        /// </summary>
        [Xml]
        public string Version;

        /// <summary>
        /// ProgId(s) associated with Class must be a child element of the ClassRegistration instance.
        /// </summary>
        public ProgId[] ProgIds;

        /// <summary>
        /// Interface(s) associated with Class must be a child element of the ClassRegistration instance.
        /// </summary>
        public Interface[] Interfaces;

        /// <summary>
        /// Serializes class into WiX document.
        /// </summary>
        /// <param name="context">Processing context for WiX Document.</param>
        /// <exception cref="ValidationException"></exception>
        public void Process(ProcessingContext context)
        {
            XElement element = this.ToXElement("Class");

            if (id == null)
                throw new ValidationException("Class Identifier (CLSID) cannot be null.");

            if (Advertise == true && AppId.HasValue)
                throw new ValidationException($"{nameof(AppId)} may not be set if the class is Advertised.");

            if (Advertise == true && Context.IsNullOrEmpty())
                throw new ValidationException($"{nameof(Context)} must be set if class is Advertised.");

            if ((Advertise == true || !Server.IsNullOrEmpty()) && !ForeignServer.IsNullOrEmpty())
                throw new ValidationException($"{nameof(ForeignServer)} cannot be set if class is Advertised or {nameof(Server)} is set.");

            if (Advertise == true && SafeForInitializing.HasValue)
                throw new ValidationException($"{nameof(SafeForInitializing)} cannot be set if class is Advertised.");

            if (Advertise == true && SafeForScripting.HasValue)
                throw new ValidationException($"{nameof(SafeForScripting)} cannot be set if class is Advertised.");

            if (ProgIds?.Length > 0 || Interfaces?.Length > 0)
            {
                var comRegContext = new ProcessingContext
                {
                    Project = context.Project,
                    Parent = this,
                    FeatureComponents = context.FeatureComponents,
                    XParent = element
                };

                if (ProgIds?.Length > 0)
                    _ = ProgIds.ForEach(progId => progId.Process(comRegContext));

                if (Interfaces?.Length > 0)
                    _ = Interfaces.ForEach(interfaceChild => interfaceChild.Process(comRegContext));
            }

            context.XParent.Add(element);
        }
    }

    /// <summary>
    /// COM ProgId registration. If ProgId has an associated Class, it must be a child of that element.
    /// </summary>
    public class ProgId : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Creates a new COM ProgId registration.
        /// </summary>
        public ProgId()
        {
            isAutoId = false;
        }

        /// <summary>
        /// COM Class ProgId.
        /// </summary>
        [Xml]
        public new string Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        /// <summary>
        /// Not available from WiX documentation.
        /// </summary>
        [Xml]
        public bool? Advertise;

        /// <summary>
        /// Not available in WiX documentation.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// For an advertised ProgId, the Id of an Icon element. For a non-advertised ProgId, this is the Id of a file
        /// containing an icon resource.
        /// </summary>
        [Xml]
        public string Icon;

        /// <summary>
        /// Icon index into the icon file.
        /// </summary>
        [Xml]
        public string IconIndex;

        /// <summary>
        /// Specifies that the associated ProgId should not be opened by users. The value is presented as a warning to
        /// users. An empty string is also valid for this attribute.
        /// </summary>
        [Xml]
        public string NoOpen;

        /// <summary>
        /// Child ProgId(s) associated with the instance of <c>ProgId</c>.
        /// </summary>
        public ProgId[] ProgIds;

        /// <summary>
        /// File extensions that refer to this ProgId, including MIME type and Verb data.
        /// </summary>
        public Extension[] Extensions;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            XElement element = this.ToXElement("ProgId");

            if (Id.IsNullOrEmpty())
            {
                throw new ValidationException($"{nameof(ProgId)} must contain the name of a COM ProgId in the {nameof(Id)} property.");
            }

            if (ProgIds?.Length > 0 || Extensions?.Length > 0)
            {
                var progIdContext = new ProcessingContext
                {
                    Project = context.Project,
                    Parent = this,
                    FeatureComponents = context.FeatureComponents,
                    XParent = element
                };

                if (Extensions?.Length > 0)
                    _ = Extensions.ForEach(ext => ext.Process(progIdContext));

                if (ProgIds?.Length > 0)
                    _ = ProgIds.ForEach(progId => progId.Process(progIdContext));
            }

            if (context.XParent.LocalName() == "Class" || context.XParent.LocalName() == "ProgId")
            {
                context.XParent.Add(element);
            }
            else
            {
                //Debug.Assert(false);

                var findComponent = context.XParent.Parent("Component") ?? // lookup upwards
                                    context.XParent.FindFirst("Component"); // lookup downwards

                if (findComponent != null)
                {
                    findComponent.Add(element);
                }
                else
                {
                    context.XParent.Add(element);
                    //XElement newComponent = this.WrapInNewParentComponent(element);
                    //context.XParent.Add(newComponent);

                    //MapComponentToFeatures(newComponent.Attr("Id"), ActualFeatures, context);
                }
            }

        }
    }

    /// <summary>
    /// Associates a Component or COM ProgID with a file extension or system action.
    /// </summary>
    public class Extension : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Creates a new file extension association.
        /// </summary>
        public Extension()
        {
            isAutoId = false;
        }

        /// <summary>
        /// This is simply the file extension, like "doc" or "xml". Do not include the preceding period.
        /// </summary>
        [Xml]
        public new string Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        /// <summary>
        /// Whether this extension is to be advertised. The default is no.
        /// </summary>
        [Xml]
        public bool? Advertise;

        /// <summary>
        /// The MIME type that is to be written.
        /// </summary>
        [Xml]
        public string ContentType;

        /// <summary>
        /// Extensibility point in the WiX XML Schema.
        /// Schema extensions can register additional attributes at this point in the schema.
        /// </summary>
        public IGenericEntity[] GenericEntities = new IGenericEntity[0];

        /// <summary>
        /// MIME content-types for an <see cref="Extension"/>.
        /// </summary>
        public MimeType[] MIMETypes = new MimeType[0];

        /// <summary>
        /// Verb definitions for an <see cref="Extension"/>.
        /// </summary>
        public Verb[] Verbs = new Verb[0];

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            var element = this.ToXElement("Extension");

            if (Id.IsNullOrEmpty() || Id[0] == '.')
            {
                throw new ValidationException(
                    $"Extension must have an {nameof(Id)} that is the file extension without the preceding period.");
            }

            if (!Advertise.HasValue)
                Advertise = false;

            var extensionContext = new ProcessingContext
            {
                Project = context.Project,
                Parent = this,
                FeatureComponents = context.FeatureComponents,
                XParent = element
            };

            GenericEntities.ForEach(e => e.Process(extensionContext));
            MIMETypes.ForEach(t => t.Process(extensionContext));
            Verbs.ForEach(v => v.Process(extensionContext));

            context.XParent.Add(element);
        }
    }

    /// <summary>
    /// Verb definition for an Extension. When advertised, this element creates a row in the Verb table. When not advertised, this element creates the appropriate rows in Registry table.
    /// </summary>
    public class Verb : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Creates a new Verb definition for an <see cref="Extension"/>.
        /// </summary>
        public Verb()
        {
            isAutoId = false;
        }

        /// <summary>
        /// The verb for the command.
        /// </summary>
        [Xml]
        public new string Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        /// <summary>
        /// The value for command arguments.
        /// </summary>
        /// <remarks>
        /// The resolution of properties in the Argument field is limited.
        /// A property formatted as [Property] in this field can only be resolved if the property already has the intended value when the component owning the verb is installed.
        /// For example, for the argument "[#MyDoc.doc]" to resolve to the correct value, the same process must be installing the file MyDoc.doc and the component that owns the verb.
        /// </remarks>
        [Xml]
        public string Argument;

        /// <summary>
        /// The localized text displayed on the context menu.
        /// </summary>
        [Xml]
        public string Command;

        /// <summary>
        /// The sequence of the commands.
        /// </summary>
        /// <remarks>
        /// Only verbs for which the Sequence is specified are used to prepare an ordered list for the default value of the shell key.
        /// The Verb with the lowest value in this column becomes the default verb. Used only for Advertised verbs.
        /// </remarks>
        [Xml]
        public int? Sequence;

        /// <summary>
        /// Either this attribute or the <see cref="TargetProperty"/> attribute must be specified for a non-advertised verb. The value should be the identifier of the target file to be executed for the verb.
        /// </summary>
        [Xml]
        public string TargetFile;

        /// <summary>
        /// Either this attribute or the <see cref="TargetFile"/> attribute must be specified for a non-advertised verb.
        /// The value should be the identifier of the property which will resolve to the path to the target file to be executed for the verb.
        /// </summary>
        [Xml]
        public string TargetProperty;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="ValidationException"></exception>
        public void Process(ProcessingContext context)
        {
            var element = this.ToXElement("Verb");
            bool isParentAdvertised;

            if (Id.IsNullOrEmpty())
                throw new ValidationException($"{nameof(Verb)} must contain the name of an executable or command in the {nameof(Id)} property.");

            if (!(context.Parent is Extension))
            {
                throw new ValidationException($"{nameof(Verb)} element must be a child of {nameof(Extension)}.");
            }
            else
            {
                isParentAdvertised = (context.Parent as Extension)?.Advertise == true;
            }

            if (!isParentAdvertised)
            {
                if (Sequence.HasValue)
                    throw new ValidationException($"If parent {nameof(Extension)} is not advertised, {nameof(Sequence)} should not be set.");

                if (!(TargetFile.IsNullOrEmpty() ^ TargetProperty.IsNullOrEmpty()))
                    throw new ValidationException($"If parent {nameof(Extension)} is not advertised, either {nameof(TargetFile)} or {nameof(TargetProperty)} must be set.");
            }

            if (isParentAdvertised && !(TargetFile.IsNullOrEmpty() || TargetProperty.IsNullOrEmpty()))
                throw new ValidationException($"If parent {nameof(Extension)} is advertised, it may not specify a {nameof(TargetFile)} or {nameof(TargetProperty)}.");

            context.XParent.Add(element);
        }
    }

    /// <summary>
    /// MIME content-type for an Extension
    /// </summary>
    public class MimeType : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Whether this MIME is to be advertised. The default is to match whatever the parent extension element uses.
        /// If the parent element is not advertised, then this element cannot be advertised either.
        /// </summary>
        [Xml]
        public bool? Advertise;

        /// <summary>
        /// Class ID for the COM server that is to be associated with the MIME content.
        /// </summary>
        [Xml(Name = "Class")]
        public Guid COMClassId;

        /// <summary>
        /// This is the identifier for the MIME content. It is commonly written in the form of: type/format.
        /// </summary>
        [Xml]
        public string ContentType;

        /// <summary>
        /// If 'yes', become the content type for the parent Extension. The default value is 'no'.
        /// </summary>
        [Xml]
        public bool Default;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            var element = this.ToXElement("MIME");
            bool? isParentAdvertised;

            if (!(context.Parent is Extension p))
                throw new ValidationException($"{nameof(MimeType)} may only be a child of an {nameof(Extension)}.");
            else
                isParentAdvertised = p.Advertise;

            if (ContentType.IsNullOrEmpty())
                throw new ValidationException($"{nameof(ContentType)} must have a value.");

            if (!Advertise.HasValue)
                Advertise = isParentAdvertised;

            if (isParentAdvertised == false && Advertise == true)
                throw new ValidationException($"{nameof(MimeType)} cannot be advertised if parent {nameof(Extension)} is not also advertised.");

            context.XParent.Add(element);
        }
    }

    /// <summary>
    /// COM Interface registration for parent Class.
    /// </summary>
    public class Interface : WixEntity, IGenericEntity
    {
        private Guid? ifaceId;

        /// <summary>
        /// Creates a new COM Interface registration.
        /// </summary>
        public Interface()
        {
            isAutoId = false;
        }

        /// <summary>
        /// GUID identifier for COM Interface.
        /// </summary>
        [Xml]
        public new Guid Id
        {
            get => ifaceId.Value;
            set
            {
                ifaceId = value;
                base.Id = value.ToString();
            }
        }

        /// <summary>
        /// Identifies the interface from which the current interface is derived.
        /// </summary>
        [Xml]
        public string BaseInterface;

        /// <summary>
        /// Name for COM Interface.
        /// </summary>
        [Xml]
        public new string Name
        {
            get => base.Name;
            set => base.Name = value;
        }

        /// <summary>
        /// Number of methods implemented on COM Interface.
        /// </summary>
        [Xml]
        public int NumMethods;

        /// <summary>
        /// GUID CLSID for proxy stub to COM Interface.
        /// </summary>
        [Xml]
        public string ProxyStubClassId;

        /// <summary>
        /// GUID CLSID for 32-bit proxy stub to COM Interface.
        /// </summary>
        [Xml]
        public string ProxyStubClassId32;

        /// <summary>
        /// Determines whether a Typelib version entry should be created with the other COM Interface registry keys. Default is 'yes'.
        /// </summary>
        public bool? Versioned;

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="Project" />.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            var element = this.ToXElement("Interface");

            context.XParent.Add(element);
        }
    }

    /// <summary>
    /// Register a type library (TypeLib). Please note that in order to properly use this non-advertised, you will need use this element with Advertise='no' and also author the appropriate child Interface elements by extracting them from the type library itself.
    /// </summary>
    /// <example>The following is an example of deploying and registering  a COM server.
    /// <code>
    /// var project =  new Project("MyProduct",
    ///     new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         new File(@"Files\Bin\MyApp.exe",
    ///                 new TypeLib
    ///                 {
    ///                     Id = new Guid("6f330b47-2577-43ad-9095-1861ba25889b"),
    ///                     Language = 33,
    ///                     MajorVersion = 23
    ///                 },
    ///                 new ComRegistration
    ///                 {
    ///                     Id = new Guid("6f330b47-2577-43ad-9095-1861ba25889b"),
    ///                     Description = "MY DESCRIPTION",
    ///                     ThreadingModel = ThreadingModel.apartment,
    ///                     Context = "InprocServer32",
    ///                     ProgIds = new[]
    ///                     {
    ///                         new ProgId
    ///                         {
    ///                             Id = "PROG.ID.1",
    ///                             Description ="Version independent ProgID ",
    ///                             ProgIds = new[]
    ///                             {
    ///                                 new ProgId
    ///                                 {
    ///                                     Id = "prog.id",
    ///                                     Description="some description"
    ///                                 }
    ///                             }
    ///                         }
    ///                     }
    ///                 })));
    /// </code>
    /// </example>
    /// <seealso cref="WixSharp.WixEntity" />
    /// <seealso cref="WixSharp.IGenericEntity" />
    public class TypeLib : WixEntity, IGenericEntity
    {
        private Guid? libId;

        /// <summary>
        /// Creates a new TypeLib registration.
        /// </summary>
        public TypeLib()
        {
            isAutoId = false;
        }

        /// <summary>
        /// The GUID that identifies the type library.
        /// </summary>
        [Xml]
        public new Guid? Id
        {
            get => libId;
            set
            {
                libId = value;
                base.Id = value?.ToString();
            }
        }

        /// <summary>
        /// Value of 'true' will create a row in the TypeLib table. Value of 'false' will create rows in the Registry table. The default value is 'false'.
        /// </summary>
        [Xml]
        public bool? Advertise;

        /// <summary>
        /// Value of 'true' means the type library describes controls, and should not be displayed in type browsers intended for nonvisual objects. This attribute can only be set if Advertise='false'.
        /// </summary>
        [Xml]
        public bool? Control;

        /// <summary>
        /// The cost associated with the registration of the type library in bytes. This attribute cannot be set if Advertise='no'.
        /// </summary>
        [Xml]
        public int? Cost;

        /// <summary>
        /// The localizable description of the type library.
        /// </summary>
        [Xml]
        public string Description;

        /// <summary>
        /// Value of 'true' means the type library exists in a persisted form on disk. This attribute can only be set if Advertise='false'.
        /// </summary>
        [Xml]
        public bool? HasDiskImage;

        /// <summary>
        /// The identifier of the Directory element for the help directory.
        /// </summary>
        [Xml]
        public string HelpDirectory;

        /// <summary>
        /// Value of 'true' means the type library should not be displayed to users, although its use is not restricted. Should be used by controls. Hosts should create a new type library that wraps the control with extended properties. This attribute can only be set if Advertise='false'.
        /// </summary>
        [Xml]
        public bool? Hidden;

        /// <summary>
        /// The language of the type library. This must be a non-negative integer.
        /// </summary>
        [Xml]
        public int Language;

        /// <summary>
        /// 	The major version of the type library. The value should be an integer from 0 - 255.
        /// </summary>
        [Xml]
        public int? MajorVersion;

        /// <summary>
        /// 	The minor version of the type library. The value should be an integer from 0 - 255.
        /// </summary>
        [Xml]
        public int? MinorVersion;

        /// <summary>
        /// The resource id of a typelib. The value is appended to the end of the typelib path in the registry.
        /// </summary>
        [Xml]
        public int? ResourceId;

        /// <summary>
        /// Value of 'true' means the type library is restricted, and should not be displayed to users. This attribute can only be set if Advertise='false'.
        /// </summary>
        [Xml]
        public bool? Restricted;

        /// <summary>
        /// AppIds associated with this Type Library.
        /// </summary>
        public AppId[] AppIds;

        /// <summary>
        /// COM Classes associated with this Type Library.
        /// </summary>
        public ComRegistration[] COMClasses;

        /// <summary>
        /// Interfaces associated with this Type Library.
        /// </summary>
        public Interface[] Interfaces;

        /// <summary>
        /// The method demonstrates the correct way of integrating RemoveFolderEx.
        /// <para>
        /// The sample also shows various XML manipulation techniques available with Fluent XElement extensions:
        /// <para>- Auto XML serialization of CLR object with serializable members marked with XMLAttribute.</para>
        /// <para>- XML namespace-transparent lookup method FindSingle.</para>
        /// </para>
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ValidationException"></exception>
        public void Process(ProcessingContext context)
        {
            XElement typeLibElement = this.ToXElement("TypeLib");

            if (!libId.HasValue)
                throw new ValidationException("Type Libraries must have a GUID.");

            // If advertised, the TypeLib cannot be a control.
            if (Control.HasValue && Advertise == true)
                throw new ValidationException($"If {nameof(TypeLib)} is advertised, {nameof(Control)} cannot be set.");

            if (Advertise == false && Cost.HasValue)
                throw new ValidationException($"If {nameof(TypeLib)} is not advertised, it cannot have a {nameof(Cost)}.");

            if (Advertise == true && HasDiskImage == true)
                throw new ValidationException($"If {nameof(TypeLib)} is advertised, {nameof(HasDiskImage)} must be false.");

            if (Advertise == true && Hidden.HasValue)
                throw new ValidationException($"If {nameof(TypeLib)} is advertised, {nameof(Hidden)} must be null.");

            if (Advertise == true && Restricted.HasValue)
                throw new ValidationException($"If {nameof(TypeLib)} is advertised, {nameof(Restricted)} must be null.");

            if (AppIds?.Length > 0 || COMClasses?.Length > 0 || Interfaces?.Length > 0)
            {
                var typeLibContext = new ProcessingContext
                {
                    Project = context.Project,
                    Parent = this,
                    FeatureComponents = context.FeatureComponents,
                    XParent = typeLibElement
                };

                if (AppIds?.Length > 0)
                    _ = AppIds.ForEach(id => id.Process(typeLibContext));

                if (COMClasses?.Length > 0)
                    _ = COMClasses.ForEach(cls => cls.Process(typeLibContext));

                if (Interfaces?.Length > 0)
                    _ = Interfaces.ForEach(iface => iface.Process(typeLibContext));
            }

            context.XParent.Add(typeLibElement);
        }
    }
}