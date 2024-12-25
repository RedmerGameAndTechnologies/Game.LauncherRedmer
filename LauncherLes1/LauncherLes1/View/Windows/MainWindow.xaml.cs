using CheckConnectInternet;
using LauncherLes1.View.Pages;
using LauncherLes1.View.Resources.Script;
using LauncherLes1.View.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LauncherLes1.View
{
    public partial class MainWindow : Window
    {
        public static bool isActiveerrorConnectInternetWindow { get; set; } = false;
        public static bool test { get; set; }

        Window ConfirmUpdate;

        public MainWindow()
        {
            InitializeComponent();
            AutoUpdateLauncher();
            UpdateUI.Update(BackgroundUIFunction, 0,0,5);

            if (test) StartWarningAnimation();
        }

        public void BackgroundUIFunction(object sender, EventArgs ea)
        {
            if (isActiveerrorConnectInternetWindow == false)
            {
                if (Internet.connect() == false)
                {
                    this.Hide();
                    ErrorConnectInternetWindow errorConnectInternetWindow = new ErrorConnectInternetWindow();
                    errorConnectInternetWindow.Show();
                    StartWarningAnimation();
                }
                else Warning.Opacity = 0;
            }

        }

        private async void StartWarningAnimation()
        {
            Warning.Opacity = 0;
            while (Warning.Opacity < 1)
            {
                Warning.Opacity += 0.1;
                await Task.Delay(100);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        #region AutoUpdateLauncher
        private void AutoUpdateLauncher()
        {
            if (Properties.Settings.Default.isAutoUpdateLauncher == true)
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

        private void Home(object sender, RoutedEventArgs e)
        {
            SlideMenu.Visibility = Visibility.Hidden;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Collapse(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }
    }
}
