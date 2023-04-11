using System;
using System.Drawing;
using System.Linq;
using WixSharp.UI.Forms;
using WixToolset.Dtf.WindowsInstaller;

namespace WixSharp
{
    /// <summary>
    /// The MsiSessionAdapter object controls the installation process.
    /// </summary>
    public class MsiSessionAdapter : ISession
    {
        /// <summary>
        /// The MSI session object.
        /// </summary>
        private readonly Session MsiSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsiSessionAdapter"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public MsiSessionAdapter(Session session)
        {
            MsiSession = session;
        }

        /// <summary>
        /// Gets or sets the string value of a named installer property.
        /// </summary>
        /// <param name="name"></param>
        public string this[string name]
        {
            get { return MsiSession[name]; }
            set { MsiSession[name] = value; }
        }

        /// <summary>
        /// The session context object.
        /// </summary>
        public object SessionContext => MsiSession;

        /// <summary>
        /// Returns a collection of FeatureItem
        /// </summary>
        public FeatureItem[] Features
        {
            get
            {
                //Cannot use MsiRuntime.Session.Features (FeatureInfo collection).
                //This WiX feature is just not implemented yet. All members except 'Name' throw InvalidHandeException
                //Thus instead of using FeatureInfo just collect the names and query database for the rest of the properties.
                string[] names = MsiSession.Features.Select(x => x.Name).ToArray();

                return names.Select(name => new FeatureItem(MsiSession, name)).ToArray();
            }
        }

        /// <summary>
        /// Returns the value of the named property of the specified <see cref="T:Microsoft.Deployment.WindowsInstaller.Session"/> object.
        /// <para>It can be uses as a generic way of accessing the properties as it redirects (transparently) access to the
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.Session.CustomActionData"/> if the session is terminated (e.g. in deferred
        /// custom actions).</para>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string Property(string name)
        {
            return MsiSession.Property(name);
        }

        /// <summary>
        /// Returns the resource bitmap.
        /// </summary>
        /// <param name="name">The name on resource.</param>
        /// <returns></returns>
        public Bitmap GetResourceBitmap(string name)
        {
            return MsiSession.GetEmbeddedBitmap(name);
        }

        /// <summary>
        /// Returns the resource data.
        /// </summary>
        /// <param name="name">The name on resource in the Binary table.</param>
        /// <returns></returns>
        public byte[] GetResourceData(string name)
        {
            return MsiSession.GetEmbeddedData(name);
        }

        /// <summary>
        /// Returns the resource string.
        /// </summary>
        /// <param name="name">The name on resource.</param>
        /// <returns></returns>
        public string GetResourceString(string name)
        {
            return MsiSession.GetEmbeddedString(name);
        }

        /// <summary>
        /// Gets the target system directory path based on specified directory name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetDirectoryPath(string name)
        {
            return MsiSession.GetDirectoryPath(name);
        }

        /// <summary>
        /// Gets a value indicating whether the MSI is running in "installing" mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom action and the session properties
        /// that it depends on are not preserved with 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <value>
        /// <c>true</c> if installing; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstalling()
        {
            return MsiSession.IsInstalling();
        }

        /// <summary>
        /// Gets a value indicating whether the MSI is running in "repair" mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom action and the session properties
        /// that it depends on are not preserved with 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public bool IsRepairing()
        {
            return MsiSession.IsRepairing();
        }

        /// <summary>
        /// Determines whether the MSI is running in "uninstalling" mode.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom action and the session properties
        /// that it depends on are not preserved with 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public bool IsUninstalling()
        {
            return MsiSession.IsUninstalling();
        }

        /// <summary>
        /// Writes a message to the log, if logging is enabled.
        /// </summary>
        /// <param name="msg">The line to be written to the log</param>
        public void Log(string msg)
        {
            MsiSession.Log(msg);
            InstallerRuntime.VirtualLog.AppendLine(msg);
        }

        /// <summary>
        /// Gets the log file.
        /// </summary>
        /// <value>
        /// The log file.
        /// </value>
        public string LogFile => MsiSession.GetLogFile().IsNotEmpty() ?
            MsiSession.GetLogFile() : // may fail if the session is closed
            Environment.GetEnvironmentVariable("MsiLogFileLocation");
    }
}