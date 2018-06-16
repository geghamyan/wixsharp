//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.Nsis;

public static class Script
{
    public static void Main(string[] args)
    {
        // If `Prerequisite.msi` does not exist yet execute <Wix# Samples>\Bootstrapper\NsisBootstrapper\Build.cmd

        var project = new Project("MainProduct",
                          new ManagedAction(CustomActions.MyAction, Return.ignore, When.After, Step.InstallInitialize, Condition.NOT_Installed));

        var msiFile = Compiler.BuildMsi(project);

        if (System.IO.File.Exists(msiFile))
        {
            new NsisBootstrapper
            {
                DoNotPostVerifyPrerequisite = false,
                PrerequisiteFile = "Prerequisite.msi",
                PrimaryFile = msiFile,
                OutputFile = "MyProduct.exe",
                PrerequisiteRegKeyValue = @"HKLM:Software\My Company\My Product:Installed",

                IconFile = "app_icon.ico",

                VersionInfo = new VersionInformation("1.0.0.0")
                {
                    ProductName = "Test Application",
                    LegalCopyright = "Copyright Test company",
                    FileDescription = "Test Application",
                    FileVersion = "1.0.0",
                    CompanyName = "Test company",
                    InternalName = "setup.exe",
                    OriginalFilename = "setup.exe"
                }
            }
            .Build();
        }
    }
}

public static class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        MessageBox.Show("Hello World!", "Embedded Managed CA");
        session.Log("Begin MyAction Hello World");

        return ActionResult.Success;
    }
}


