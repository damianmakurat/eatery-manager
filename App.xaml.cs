﻿using System.Configuration;
using System.Data;
using System.Windows;

namespace eatery_manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppSettings.CreateDefaultSettingsFileIfNotExists();
        }
    }

}
