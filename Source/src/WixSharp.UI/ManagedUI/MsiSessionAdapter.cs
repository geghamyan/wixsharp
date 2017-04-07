using System.Drawing;
using Microsoft.Deployment.WindowsInstaller;

namespace WixSharp
{
    /// <summary>
    /// The MsiSessionAdapter object  controls the installation process.
    /// </summary>
    public class MsiSessionAdapter : ISession
    {
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
        /// <param name="property"></param>
        public string this[string property]
        {
            get { return MsiSession[property]; }
            set { MsiSession[property] = value; }
        }

        /// <summary>
        /// The MSI session object.
        /// </summary>
        public Session MsiSession { get; }

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
        /// <param name="binary">The name on resource.</param>
        /// <returns></returns>
        public Bitmap GetResourceBitmap(string binary)
        {
            return MsiSession.GetEmbeddedBitmap(binary);
        }

        /// <summary>
        /// Returns the resource data.
        /// </summary>
        /// <param name="binary">The name on resource in the Binary table.</param>
        /// <returns></returns>
        public byte[] GetResourceData(string binary)
        {
            return MsiSession.GetEmbeddedData(binary);
        }

        /// <summary>
        /// Returns the resource string.
        /// </summary>
        /// <param name="binary">The name on resource.</param>
        /// <returns></returns>
        public string GetResourceString(string binary)
        {
            return MsiSession.GetEmbeddedString(binary);
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
        /// Gets a value indicating whether the product is being installed.
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
        /// Gets a value indicating whether the product is being repaired.
        /// <para>
        /// This method will fail to retrieve the correct value if called from the deferred custom action and the session properties
        /// that it depends on are not preserved with 'UsesProperties' or 'DefaultUsesProperties'.
        /// </para>
        /// </summary>
        public bool IsRepairing()
        {
            return MsiSession.IsRepairing();
        }

        /// <summary>
        /// Determines whether MSI is running in "uninstalling" mode.
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
        }
    }
}