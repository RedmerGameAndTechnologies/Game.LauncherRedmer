using CheckConnectInternet;
using LauncherLes1.View.Pages;
using LauncherLes1.View.Windows;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace LauncherLes1.View
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer;

        public static bool isActiveerrorConnectInternetWindow { get; set; } = false;

        string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        Window ConfirmUpdate;

        public MainWindow()
        {
            InitializeComponent();
            CheckConnectInternet();
            AutoUpdateLauncher();
            UpdateUI();
        }

        private void UpdateUI()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(BackgroundUIFunction);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }
        public void BackgroundUIFunction(object sender, EventArgs ea)
        {
            _textServerVersion.Content = "Версия лаунчера: " + curver;
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
