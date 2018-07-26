using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using WixSharp;
using WixSharp.UI.Forms;

public class BASession : ISession
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

    public FeatureItem[] Features { get; } = new FeatureItem[0];

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

        return null;
    }

    public byte[] GetResourceData(string name)
    {
        switch (name)
        {
            case "WixSharp_UIText":
                return WixSharp.UI.ManagedUI.Resources.WixUI_en_us;
        }

        return null;
    }

    public string GetResourceString(string name)
    {
        switch (name)
        {
            case "WixSharp_LicenceFile":
                return WixSharp.UI.ManagedUI.Resources.WixSharp_LicenceFile;
        }

        return null;
    }

    public string GetDirectoryPath(string name)
    {
        if (name.Equals("INSTALLDIR", StringComparison.Ordinal))
        {
            var programFiles = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            return Path.Combine(programFiles, Property("WixBundleName"));
        }
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