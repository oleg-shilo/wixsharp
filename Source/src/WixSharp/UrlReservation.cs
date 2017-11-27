using System.Xml.Linq;

namespace WixSharp
{
	/// <summary>
	/// Specifies the behavior when trying to install a URL reservation and it already exists.
	/// </summary>
	public enum UrlReservationHandleExisting
    {
        /// <summary>
        /// Replaces the existing URL reservation (the default).
        /// </summary>
        replace,

        /// <summary>
        /// Keeps the existing URL reservation.
        /// </summary>
        ignore,

        /// <summary>
        /// The installation fails.
        /// </summary>
        fail,
    }

	/// <summary>
	/// Rights for this ACE.
	/// </summary>
	public enum UrlReservationRights
    {
        register,
        @delegate,
        all,
    }

	/// <summary>
	/// Makes a reservation record for the HTTP Server API configuration store on Windows XP SP2, Windows Server 2003, and later.
	/// </summary>
	public class UrlReservation : WixEntity
    {
        /// <summary>
        /// Specifies the behavior when trying to install a URL reservation and it already exists. This attribute's value must be one of the following:
        /// <list type="bullet">
        /// <item><description>replace</description></item>
        /// <item><description>ignore</description></item>
        /// <item><description>fail</description></item>
        /// </list>
        /// </summary>
        public UrlReservationHandleExisting HandleExisting = UrlReservationHandleExisting.replace;

        /// <summary>
        /// Security descriptor to apply to the URL reservation. Can't be specified when using children UrlAce elements.
        /// </summary>
        public string Sddl;

        /// <summary>
        /// The UrlPrefix string that defines the portion of the URL namespace to which this reservation pertains.
        /// </summary>
        public string Url;

        /// <summary>
        /// Rights for this ACE. Default is "all". This attribute's value must be one of the following:
        /// <list type="bullet">
        /// <item><description>register</description></item>
        /// <item><description>delegate</description></item>
        /// <item><description>all</description></item>
        /// </list>
        /// </summary>
        public UrlReservationRights Rights;

        /// <summary>
        /// The security principal for this ACE.
        ///  When the UrlReservation is under a ServiceInstall element, this defaults to "NT SERVICE\ServiceInstallName".
        ///  This may be either a SID or an account name in a format that LookupAccountName supports. When using a SID, an asterisk must be prepended.
        /// <example>
        /// "*S-1-5-18"
        /// </example> 
        /// </summary>
        public string SecurityPrincipal;

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
            SecurityPrincipal = securityPrincipal;
            Rights = rights;
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
		/// Emits WiX XML for UrlReservation.
		/// </summary>
		/// <returns></returns>
		public XElement ToXml()
        {
            var retval = this.ToXElement(WixExtension.Http.ToXName("UrlReservation"))
                .SetAttribute("Id", this.Id)
                .SetAttribute("Url", this.Url)
                .SetAttribute("Sddl", this.Sddl)
                .SetAttribute("HandleExisting", this.HandleExisting);

	        if (Sddl.IsEmpty())
	        {
				retval.AddElement(WixExtension.Http.ToXName("UrlAce"))
					.SetAttribute("Id", this.Id)
					.SetAttribute("Rights", this.Rights)
					.SetAttribute("SecurityPrincipal", this.SecurityPrincipal);
			}

            return retval;
        }
    }
}
