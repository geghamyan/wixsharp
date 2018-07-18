using System.Drawing;

namespace WixSharp
{
    /// <summary>
    /// The ISession interface controls the installation process.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Gets or sets the string value of a named installer property.
        /// </summary>
        /// <param name="name"></param>
        string this[string name] { get; set; }

        /// <summary>
        /// The inner session object.
        /// </summary>
        object InnerSession { get; }

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
        /// <param name="binary">The name on resource.</param>
        /// <returns></returns>
        Bitmap GetResourceBitmap(string binary);

        /// <summary>
        /// Returns the resource data.
        /// </summary>
        /// <param name="binary">The name on resource in the Binary table.</param>
        /// <returns></returns>
        byte[] GetResourceData(string binary);

        /// <summary>
        /// Returns the resource string.
        /// </summary>
        /// <param name="binary">The name on resource.</param>
        /// <returns></returns>
        string GetResourceString(string binary);

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
        /// Gets a value indicating whether the product is being repaired.
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
    }
}