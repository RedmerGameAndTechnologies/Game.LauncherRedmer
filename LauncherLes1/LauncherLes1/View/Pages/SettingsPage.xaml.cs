using CheckConnectInternet;
using LauncherLes1.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LauncherLes1.View.Pages
{
    public partial class SettingsPage : Page
    {
        public static bool isActiveUpdateLauncherWindow { get; set; } = false;

        string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private DispatcherTimer dispatcherTimer;
        Window ConfirmUpdate;
        Window ErrorConnectInternetWindow;

        public SettingsPage()
        {
            InitializeComponent();
            UpdateUI();
            StartCheck_CheckBoxAutoUpdateLauncher();
        }

        private void UpdateUI()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(BackgroundUIFunction);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }
        public async void BackgroundUIFunction(object sender, EventArgs ea)
        {
            if (Internet.connect())
            {
                string readver = await HttpResponse("https://pastebin.com/raw/dem4T7Xp");

                readver = readver.Replace(".", ".");
                curver = curver.Replace(".", ".");

                //currentVersion.Content = "Моя версия: " + curver;
                newVersion.Content = "Новая версия: " + readver;

                if (curver == readver)
                {
                    DownloadUpdate.Visibility = Visibility.Visible;
                    currentVersion.Content = "Моя версия: " + curver;
                }
                else
                {
                    currentVersion.Content = "Моя версия: " + curver + " / " + readver;
                    DownloadUpdate.Visibility = Visibility.Hidden;
                    newVersion.Visibility = Visibility.Hidden;
                }
            }
        }

        private void StartCheck_CheckBoxAutoUpdateLauncher()
        {
            if (LauncherLes1.Properties.Settings.Default.isAutoUpdateLauncher == true)
            {
                CheckBox.IsChecked = true;
            }
            else
            {
                CheckBox.IsChecked = false;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            LauncherLes1.Properties.Settings.Default.isAutoUpdateLauncher = false;
            LauncherLes1.Properties.Settings.Default.Save();
        }

        private void CheckBox_Cecked(object sender, RoutedEventArgs e)
        {
            LauncherLes1.Properties.Settings.Default.isAutoUpdateLauncher = true;
            LauncherLes1.Properties.Settings.Default.Save();
        }

        private void CheckUpdateLauncher_Click(object sender, RoutedEventArgs e)
        {
            if (isActiveUpdateLauncherWindow == false)
            {
                ConfirmUpdate = new ConfirmUpdateWindow();
                isActiveUpdateLauncherWindow = true;
                ConfirmUpdate.Show();
            }
        }

        async Task<string> HttpResponse(string line)
        {
            using (var net = new HttpClient())
            {
                var response = await net.GetAsync(line);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
            }
        }
    }
}
