namespace WixSharp
{
    /// <summary>
    /// The security principal and which rights to assign to them for the URL reservation.
    /// </summary>
    class UrlAce : WixEntity, IGenericEntity
    {
        /// <summary>
        /// Primary key used to identify this particular entry.
        /// </summary>
        [Xml]
        public new string Id { get { return base.Id; } set { base.Id = value; } }

        /// <summary>
        /// Rights for this ACE. Default is "all". This attribute's value must be one of the following:
        /// <list type="bullet">
        /// <item><description>register</description></item>
        /// <item><description>delegate</description></item>
        /// <item><description>all</description></item>
        /// </list>
        /// </summary>
        [Xml]
        public UrlReservationRights Rights;

        /// <summary>
        /// The security principal for this ACE.
        ///  When the UrlReservation is under a ServiceInstall element, this defaults to "NT SERVICE\ServiceInstallName".
        ///  This may be either a SID or an account name in a format that LookupAccountName supports. When using a SID, an asterisk must be prepended.
        /// <example>
        /// "*S-1-5-18"
        /// </example>
        /// </summary>
        [Xml]
        public string SecurityPrincipal;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlAce" /> class.
        /// </summary>
        /// <param name="rights">The Rights.</param>
        /// <param name="securityPrincipal">The SecurityPrincipal.</param>
        public UrlAce(UrlReservationRights rights, string securityPrincipal)
        {
            Rights = rights;
            SecurityPrincipal = securityPrincipal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlAce" /> class.
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="rights">The Rights.</param>
        /// <param name="securityPrincipal">The SecurityPrincipal.</param>
        public UrlAce(Id id, UrlReservationRights rights, string securityPrincipal)
        {
            Id = id;
            Rights = rights;
            SecurityPrincipal = securityPrincipal;
        }

        /// <summary>
        /// Adds itself as an XML content into the WiX source being generated from the <see cref="WixSharp.Project"/>.
        /// See 'Wix#/samples/Extensions' sample for the details on how to implement this interface correctly.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Http);
            context.XParent.Add(this.ToXElement(WixExtension.Http, GetType().Name));
        }
    }
}