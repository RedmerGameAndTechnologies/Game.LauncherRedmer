using CheckConnectInternet;
using LauncherLes1.View.Pages;
using LauncherLes1.View.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace LauncherLes1.View
{
    public partial class MainWindow : Window
    {
        public static bool isActiveerrorConnectInternetWindow { get; set; } = false;

        Window ConfirmUpdate;

        public MainWindow()
        {
            InitializeComponent();
            CheckConnectInternet();
            AutoUpdateLauncher();
            Cmd($"taskkill");
        }

        private void CheckConnectInternet()
        {
            if (isActiveerrorConnectInternetWindow == false) {
                if (Internet.connect() ==  false)
                {
                    this.Hide();
                    ErrorConnectInternetWindow errorConnectInternetWindow = new ErrorConnectInternetWindow();
                    errorConnectInternetWindow.Show();
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Home(object sender, RoutedEventArgs e)
        {
            SlideMenu.Visibility = Visibility.Hidden;
        }

        private void Discord_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/efEFJfEcXH") { UseShellExecute = true });
        }

        public void Cmd(string line)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c {line}",
                WindowStyle = ProcessWindowStyle.Hidden,
            });
        }

        #region AutoUpdateLauncher
        private void AutoUpdateLauncher()
        {
            if (LauncherLes1.Properties.Settings.Default.isAutoUpdateLauncher == true)
            {
                if (SettingsPage.isActiveUpdateLauncherWindow == false)
                {
                    ConfirmUpdate = new ConfirmUpdateWindow();
                    SettingsPage.isActiveUpdateLauncherWindow = true;
                    ConfirmUpdate.Show();
                }
            }
        }
        #endregion
    }
}
