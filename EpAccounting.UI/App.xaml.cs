// ///////////////////////////////////
// File: App.xaml.cs
// Last Change: 02.09.2017  10:30
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI
{
    using System.Windows;
    using EpAccounting.UI.Properties;



    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Settings.Default.WindowState == WindowState.Maximized)
            {
                Settings.Default.Width = 1000;
                Settings.Default.Height = 800;
            }
            else if (Settings.Default.WindowState == WindowState.Minimized)
            {
                Settings.Default.WindowState = WindowState.Normal;
            }

            Settings.Default.Save();
        }
    }
}