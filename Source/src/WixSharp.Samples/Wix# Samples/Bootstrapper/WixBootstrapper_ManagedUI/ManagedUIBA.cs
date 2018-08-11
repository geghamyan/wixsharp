using System;
using System.CodeDom;
using System.ComponentModel;
using System.Diagnostics;
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
    private ISession session;

    public ManagedUIBA()
    {
        session = new BASession(this);
    }

    /// <summary>
    /// Entry point that is called when the bootstrapper application is ready to run.
    /// </summary>
    protected override void Run()
    {
        Debug.Assert(false);

        try
        {
            session["WixSharp_UI_INSTALLDIR"] = "INSTALLDIR";

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
            managedUI.InstallDialogs
                .Add<WelcomeDialog>()
                .Add<LicenceDialog>()
                .Add<SetupTypeDialog>()
                .Add<FeaturesDialog>()
                .Add<InstallDirDialog>()
                .Add<ProgressDialog>()
                .Add<ExitDialog>();

            managedUI.ModifyDialogs
                .Add<MaintenanceTypeDialog>()
                .Add<FeaturesDialog>()
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

    /// <inheritdoc />
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