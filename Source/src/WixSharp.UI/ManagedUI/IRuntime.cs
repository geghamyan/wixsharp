using System.Collections.Generic;
using Microsoft.Deployment.WindowsInstaller;

namespace WixSharp
{
    /// <summary>
    /// Represents runtime context.
    /// </summary>
    public interface IRuntime
    {
        /// <summary>
        /// Starts the execution of the MSI installation.
        /// </summary>
        System.Action StartExecute { get; }

        /// <summary>
        /// Cancels the execution of the MSI installation, which is already started (progress is displayed).
        /// </summary>
        System.Action CancelExecute { get; }

        /// <summary>
        /// The session object.
        /// </summary>
        ISession Session { get; }

        /// <summary>
        /// Repository of the session properties to be captured and transfered to the deferred CAs.
        /// </summary>
        Dictionary<string, string> Data { get; }

        /// <summary>
        /// Localizes the specified text.
        /// <para>The localization is performed according two possible scenarios. The method will return the match form the MSI embedded localization file.
        /// However if it cannot find the match the method will try to find the and return the match from the MSI session properties.</para>
        /// <para>This method is mainly used by 'LocalizeWith' extension for a single step localization of WinForm controls.</para>
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        string Localize(string text);

        /// <summary>
        /// Invokes Client Handlers
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="UIShell"></param>
        /// <returns></returns>
        ActionResult InvokeClientHandlers(string eventName, IShellView UIShell = null);

        /// <summary>
        /// Fetches Install Directory
        /// </summary>
        void FetchInstallDir();
    }
}