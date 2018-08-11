using System;
using WixSharp;
using WixSharp.Bootstrapper;
using io = System.IO;

public class Script
{
    //The UI implementation is based on the work of BRYANPJOHNSTON
    //http://bryanpjohnston.com/2012/09/28/custom-wix-managed-bootstrapper-application/

    public static void Main(string[] args)
    {
        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")))
            {
                InstallScope = InstallScope.perMachine,
                GUID = new Guid("83B74C92-1681-4700-B955-07A394C0BC93")
            };

        productProj.Actions = productProj.Actions.Combine(new WixQuietExecAction("notepad.exe", ""));
        //productProj.Actions = productProj.Actions.Combine(new WixQuietExecAction("timeout", "/t 5"));

        string productMsi = productProj.BuildMsi();

        //------------------------------------

        var bootstrapper =
            new Bundle("My Product",
                new PackageGroupRef("NetFx40Web"),
                new MsiPackage(productMsi) {Id = "MyProductPackageId", DisplayInternalUI = false, Visible = true})
            {
                Version = new Version("1.0.0.0"),
                UpgradeCode = new Guid("31D03788-CC7E-47C2-9797-43CEFC9AF57E"),
                Application = new ManagedBootstrapperApplication("%this%", "BootstrapperCore.config")
            };

        // you can also use System.Reflection.Assembly.GetExecutingAssembly().Location

        var directory = io.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(directory))
            throw new Exception("Failed to get directory path.");

        bootstrapper.Application.Payloads = new[]
        {
            new Payload(io.Path.Combine(directory, "BootstrapperCore.config")),
            new Payload(io.Path.Combine(directory, "WixSharp.UI.dll")),
            new Payload(io.Path.Combine(directory, "WixSharp.dll"))
        };

        bootstrapper.PreserveTempFiles = true;
        bootstrapper.SuppressWixMbaPrereqVars = true;

        bootstrapper.Build("MyProduct.exe");
        io.File.Delete(productMsi);
    }
}
