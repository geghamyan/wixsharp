﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using WixSharp;
using WixSharp.UI.Forms;

using IO = System.IO;

namespace WixSharp.UI.WPF
{
    public partial class FeaturesDialog : WpfDialog, IWpfDialog
    {
        public FeaturesDialog()
        {
            InitializeComponent();
        }

        public void Init()
        {
            ViewModelBinder.Bind(new FeaturesDialogModel { Host = ManagedFormHost, }, this, null);
        }
    }

    public class FeaturesDialogModel : Caliburn.Micro.Screen
    {
        public ManagedForm Host;

        ISession session => Host?.Runtime.Session;
        public BitmapImage Banner => session?.GetResourceBitmap("WixUI_Bmp_Banner").ToImageSource();

        public string Description { get; set; } = "fasdfasdfsd";

        string installDirProperty => session?.Property("WixSharp_UI_INSTALLDIR");

        public string InstallDirPath
        {
            get
            {
                if (Host == null) return null;

                string installDirPropertyValue = session.Property(installDirProperty);

                if (installDirPropertyValue.IsEmpty())
                {
                    // We are executed before any of the MSI actions are invoked so the INSTALLDIR (if set to absolute path)
                    // is not resolved yet. So we need to do it manually
                    var installDir = session.GetDirectoryPath(installDirProperty);

                    if (installDir == "ABSOLUTEPATH")
                        installDir = session.Property("INSTALLDIR_ABSOLUTEPATH");

                    return installDir;
                }
                else
                {
                    //INSTALLDIR set either from the command line or by one of the early setup events (e.g. UILoaded)
                    return installDirPropertyValue;
                }
            }

            set => session[installDirProperty] = value;
        }

        public void ChangeInstallDir()
        {
            using (var dialog = new FolderBrowserDialog { SelectedPath = InstallDirPath })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    InstallDirPath = dialog.SelectedPath;
            }
        }

        public void GoPrev()
            => Host?.Shell.GoPrev();

        public void GoNext()
            => Host?.Shell.GoNext();

        public void Cancel()
            => Host?.Shell.Cancel();

        public void Reset()
        {
            System.Windows.MessageBox.Show("!!!");
        }
    }
}