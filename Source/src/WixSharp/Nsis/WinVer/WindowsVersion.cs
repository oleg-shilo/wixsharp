namespace WixSharp.Nsis.WinVer
{
    /// <summary>
    /// Represents Windows Version
    /// </summary>
    public struct WindowsVersion
    {
        /// <summary>
        /// Creates an instance of WindowsVersion struct
        /// </summary>
        /// <param name="versionNumber">Represents a windows version</param>
        /// <param name="servicePack">Represents an optional service pack (-1 = no service pack)</param>
        public WindowsVersion(WindowsVersionNumber versionNumber, int servicePack = -1)
        {
            VersionNumber = versionNumber;
            ServicePack = servicePack;
        }

        /// <summary>
        /// Version to check
        /// </summary>
        public WindowsVersionNumber VersionNumber { get; }

        /// <summary>
        /// Service pack
        /// </summary>
        public int ServicePack { get; }
    }
}