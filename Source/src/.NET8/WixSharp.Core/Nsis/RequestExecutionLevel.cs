namespace WixSharp.Nsis
{
    /// <summary>
    /// Specifies the requested execution level for Windows Vista+.
    /// </summary>
    public enum RequestExecutionLevel
    {
        /// <summary>
        /// Keep the manifest empty and let Windows decide which execution level is required.
        /// </summary>
        None,

        /// <summary>
        /// The application runs with the same access token as the parent process.
        /// </summary>
        RunAsInvoker,

        /// <summary>
        /// The application runs with the highest privileges the current user can obtain.
        /// </summary>
        HighestAvailable,

        /// <summary>
        /// The application runs only for administrators and requires that the application be launched with the full access token of an administrator.
        /// </summary>
        RequireAdministrator
    }
}