using System.Xml.Linq;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// Makes a reservation record for the HTTP Server API configuration store on Windows XP SP2, Windows Server 2003, and later.
    /// </summary>
    public class UrlReservation : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// The UrlPrefix string that defines the portion of the URL namespace to which this reservation pertains.
        /// </summary>
        [Xml]
        public string Url;

        /// <summary>
        /// Security descriptor to apply to the URL reservation. Can't be specified when using children UrlAce elements.
        /// </summary>
        [Xml]
        public string Sddl;

        /// <summary>
        /// Specifies the behavior when trying to install a URL reservation and it already exists. This attribute's value must be one of the following:
        /// <list type="bullet">
        /// <item><description>replace</description></item>
        /// <item><description>ignore</description></item>
        /// <item><description>fail</description></item>
        /// </list>
        /// </summary>
        [Xml]
        public UrlReservationHandleExisting? HandleExisting = UrlReservationHandleExisting.replace;

        /// <summary>
        /// The security principal and which rights to assign to them for the URL reservation.
        /// </summary>
        readonly UrlAce UrlAce;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        public UrlReservation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public UrlReservation(Id id)
        {
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="url"></param>
        /// <param name="securityPrincipal"></param>
        /// <param name="rights"></param>
        public UrlReservation(Id id, string url, string securityPrincipal, UrlReservationRights rights)
        {
            Id = id;
            Url = url;
            UrlAce = new UrlAce(id, rights, securityPrincipal);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The Feature.</param>
        /// <param name="url"></param>
        /// <param name="securityPrincipal"></param>
        /// <param name="rights"></param>
        public UrlReservation(Id id, Feature feature, string url, string securityPrincipal, UrlReservationRights rights)
        {
            Id = id;
            Feature = feature;
            Url = url;
            UrlAce = new UrlAce(new Id(id), rights, securityPrincipal);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="securityPrincipal"></param>
        /// <param name="rights"></param>
        public UrlReservation(string url, string securityPrincipal, UrlReservationRights rights)
        {
            Id = id;
            Url = url;
            UrlAce = new UrlAce(new Id(id), rights, securityPrincipal);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <param name="url"></param>
        /// <param name="securityPrincipal"></param>
        /// <param name="rights"></param>
        public UrlReservation(Feature feature, string url, string securityPrincipal, UrlReservationRights rights)
        {
            Id = id;
            Feature = feature;
            Url = url;
            UrlAce = new UrlAce(new Id(id), rights, securityPrincipal);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="url"></param>
        /// <param name="sddl"></param>
        public UrlReservation(Id id, string url, string sddl)
        {
            Id = id;
            Url = url;
            Sddl = sddl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="feature">The Feature.</param>
        /// <param name="url"></param>
        /// <param name="sddl"></param>
        public UrlReservation(Id id, Feature feature, string url, string sddl)
        {
            Id = id;
            Feature = feature;
            Url = url;
            Sddl = sddl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sddl"></param>
        public UrlReservation(string url, string sddl)
        {
            Id = id;
            Url = url;
            Sddl = sddl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlReservation" /> class.
        /// </summary>
        /// <param name="feature">The Feature.</param>
        /// <param name="url"></param>
        /// <param name="sddl"></param>
        public UrlReservation(Feature feature, string url, string sddl)
        {
            Id = id;
            Feature = feature;
            Url = url;
            Sddl = sddl;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Http);

            XElement UrlReservation = this.ToXElement(WixExtension.Http, GetType().Name);
            if (UrlAce != null)
            {
                var newContext = new ProcessingContext
                {
                    Project = context.Project,
                    Parent = context.Project,
                    XParent = UrlReservation,
                    FeatureComponents = context.FeatureComponents,
                };

                UrlAce.Process(newContext);
            }

            if (context.XParent.Name == "ServiceInstall")
            {
                context.XParent.Add(UrlReservation);
            }
            else
            {
                var component = this.CreateParentComponent();
                component.Add(UrlReservation);
                context.XParent.FindFirst("Component").Parent?.Add(component);
                MapComponentToFeatures(component.Attribute("Id")?.Value, ActualFeatures, context);
            }
        }
    }
}