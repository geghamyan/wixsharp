using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.UI.Forms;
using Action = System.Action;
#if WIX4
using WixToolset.Bootstrapper;
#else
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
#endif

[assembly: BootstrapperApplication(typeof(ManagedUIBA))]

public class ManagedUIBA : BootstrapperApplication
{
    private UIShell uiShell;
    private BASession session;

    /// <summary>
    /// Entry point that is called when the bootstrapper application is ready to run.
    /// </summary>
    protected override void Run()
    {
        try
        {

        //Debug.Assert(false);

        session = new BASession(this);

        var detected = new AutoResetEvent(false);

        DetectPackageComplete += (s, e) =>
        {
            //Presence or absence of MyProductPackageId product will be a deciding factor
            //for initializing BA for Install or Uninstall.
            if (e.PackageId == "MyProductPackageId")
            {
                if (e.State == PackageState.Absent)
                {
                }
                else if (e.State == PackageState.Present)
                {
                    session["Installed"] = "00:00:00";
                }
            }
        };

        DetectComplete += (s, e) =>
        {
            detected.Set();
        };

        Engine.Detect();

        detected.WaitOne();

        var managedUI = new ManagedUI();
        managedUI.InstallDialogs.Add<WelcomeDialog>()
            .Add<LicenceDialog>()
            .Add<ProgressDialog>()
            .Add<ExitDialog>();

        managedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
            .Add<ProgressDialog>()
            .Add<ExitDialog>();

        uiShell = new UIShell();
        uiShell.ShowModal(
            new InstallerRuntime(session)
            {
                StartExecute = () =>
                {
                    LaunchAction launchAction =
                    session.IsInstalling() ? LaunchAction.Install :
                        session.IsRepairing() ? LaunchAction.Repair :
                            session.IsUninstalling() ? LaunchAction.Uninstall :
                                LaunchAction.Unknown;

                    this.Engine.Plan(launchAction);
                    this.Engine.Apply(IntPtr.Zero);
                }
            },
            managedUI);

        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
        }

        Engine.Quit(0);
    }

    protected override void OnExecuteMsiMessage(ExecuteMsiMessageEventArgs args)
    {
        uiShell.ProcessMessage((Microsoft.Deployment.WindowsInstaller.InstallMessage)args.MessageType,
            new Record(args.Data.ToArray<object>()),
            MessageButtons.OK, MessageIcon.None, MessageDefaultButton.Button1);
    }

    protected override void OnProgress(ProgressEventArgs args)
    {
        int progressPercentage = args.ProgressPercentage != 0 ? args.ProgressPercentage : args.OverallPercentage;
        uiShell.OnProgress(progressPercentage);
    }

    protected override void OnApplyComplete(ApplyCompleteEventArgs args)
    {
        uiShell.OnExecuteComplete();
    }
}

internal class BASession : ISession
{
    private readonly BootstrapperApplication bootstrapper;
    private readonly Dictionary<string, string> properties = new Dictionary<string, string>();

    public BASession(BootstrapperApplication bootstrapper)
    {
        this.bootstrapper = bootstrapper;
    }

    #region ISession implementation

    public string this[string name]
    {
        get
        {
            string text;
            if (properties.TryGetValue(name, out text))
                return text;

            if (bootstrapper.Engine.StringVariables.Contains(name))
                return bootstrapper.Engine.StringVariables[name];

            return string.Empty;
        }
        set { properties[name] = value; }
    }

    public object SessionContext => null;

    public string Property(string name)
    {
        return this[name];
    }

    public Bitmap GetResourceBitmap(string name)
    {
        switch (name)
        {
            case "WixUI_Bmp_Banner":
                return WixSharp.UI.ManagedUI.Resources.WixUI_Bmp_Banner;

            case "WixUI_Bmp_Dialog":
                return WixSharp.UI.ManagedUI.Resources.WixUI_Bmp_Dialog;
        }

        throw new Exception($"Resource {name} not found.");
    }

    public byte[] GetResourceData(string name)
    {
        switch (name)
        {
            case "WixSharp_UIText":
                return WixSharp.UI.ManagedUI.Resources.WixUI_en_us;
        }

        throw new Exception($"Resource {name} not found.");
    }

    public string GetResourceString(string name)
    {
        switch (name)
        {
            case "WixSharp_LicenceFile":
                return WixSharp.UI.ManagedUI.Resources.WixSharp_LicenceFile;
        }

        throw new Exception($"Resource {name} not found.");
    }

    public string GetDirectoryPath(string name)
    {
        return Property(name);
    }

    public bool IsInstalling()
    {
        return !IsInstalled() && !IsUninstalling();
    }

    public bool IsRepairing()
    {
        return IsInstalled() && !IsUninstalling();
    }

    public bool IsUninstalling()
    {
        return Property("REMOVE").SameAs("All", true);
    }

    public void Log(string msg)
    {
        bootstrapper.Engine.Log(LogLevel.Verbose, msg);
    }

    #endregion

    private bool IsInstalled()
    {
        return Property("Installed").IsNotEmpty();
    }
}
