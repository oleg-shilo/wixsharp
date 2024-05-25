using System.Drawing;
using WixSharp.UI.Forms;

namespace WixSharp
{
    /// <summary>
    /// The ISession interface controls the installation process. It is a WixSharp session object that encapsulates state 
    /// and functionality of <see cref="T:WixToolset.Dtf.WindowsInstaller.Session"/> objects but ads some extra features on top.
    /// <para>
    /// You can always access the original <see cref="T:WixToolset.Dtf.WindowsInstaller.Session"/> object via 
    /// <see cref="ISession.SessionContext"/>.
    /// </para>
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Gets or sets the string value of a named installer property.
        /// </summary>
        /// <param name="name"></param>
        string this[string name] { get; set; }

        /// <summary>
        /// The session context object.
        /// <para>
        /// It is a <see cref="T:WixToolset.Dtf.WindowsInstaller.Session"/> object. However because the <see cref="ISession"/>
        /// interface is not dependent on the DTF assembly the actual type of the object is <see cref="T:System.Object"/>.
        /// </para> 
        /// </summary>
        object SessionContext { get; }

        /// <summary>
        /// Returns a collection of FeatureItem
        /// </summary>
        FeatureItem[] Features { get; }

        /// <summary>
        /// Returns the value of the named property of the specified <see cref="T:Microsoft.Deployment.WindowsInstaller.Session"/> object.
        /// <para>It can be uses as a generic way of accessing the properties as it redirects (transparently) access to the
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.Session.CustomActionData"/> if the session is terminated (e.g. in deferred
        /// custom actions).</para>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string Property(string name);

        /// <summary>
        /// Returns the resource bitmap.
        /// </summary>
        /// <param name="name">The name on resource.</param>
        /// <returns></returns>
        Bitmap GetResourceBitmap(string name);

        /// <summary>
        /// Returns the resource data.
        /// </summary>
        /// <param name="name">The name on resource.</param>
        /// <returns></returns>
        byte[] GetResourceData(string name);

        /// <summary>
        /// Returns the resource string.
        /// </summary>
        /// <param name="name">The name on resource.</param>
        /// <returns></returns>
        string GetResourceString(string name);

        /// <summary>
        /// Gets the target system directory path based on specified directory name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string GetDirectoryPath(string name);

        /// <summary>
        /// Gets a value indicating whether the product is being installed.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom action and the session properties
        /// that it depends on are not preserved with 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <value>
        /// <c>true</c> if installing; otherwise, <c>false</c>.
        /// </value>
        bool IsInstalling();

        /// <summary>
        /// Gets a value indicating whether the MSI is running in Repair mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom action and the session properties
        /// that it depends on are not preserved with 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        bool IsRepairing();

        /// <summary>
        /// Determines whether MSI is running in "uninstalling" mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom action and the session properties
        /// that it depends on are not preserved with 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <returns></returns>
        bool IsUninstalling();

        /// <summary>
        /// Writes a message to the log, if logging is enabled.
        /// </summary>
        /// <param name="msg">The line to be written to the log</param>
        void Log(string msg);

        /// <summary>
        /// Gets the log file.
        /// </summary>
        /// <value>
        /// The log file.
        /// </value>
        string LogFile { get; }
    }
}