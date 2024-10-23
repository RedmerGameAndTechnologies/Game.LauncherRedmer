using CheckConnectInternet;
using LauncherLes1.View.Resources.Script;
using LauncherLes1.View.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;

namespace LauncherLes1.View.Pages
{
    public partial class SettingsPage : Page
    {
        public static bool isActiveUpdateLauncherWindow { get; set; } = false;

        string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private DispatcherTimer dispatcherTimer;
        Window ConfirmUpdate;

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

                if (curver.CompareTo(readver) < 0)
                {
                    DownloadUpdate.Visibility = Visibility.Visible;
                    currentVersion.Content = "Моя версия: " + curver + " \nНовая версия: " + readver;
                }
                else
                {
                    currentVersion.Content = "Моя версия: " + curver;
                    DownloadUpdate.Visibility = Visibility.Hidden;
                }
            }
        }

        private void StartCheck_CheckBoxAutoUpdateLauncher()
        {
            if (Properties.Settings.Default.isAutoUpdateLauncher == true) {
                CheckBox.IsChecked = true;
            }
            else {
                CheckBox.IsChecked = false;
            }

            if (Properties.Settings.Default.isRunTheGameImmediatelyAfterInstallingIt == true) {
                CheckBoxInstallNow.IsChecked = true;
            }
            else {
                CheckBoxInstallNow.IsChecked = false;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoUpdateLauncher = false;
            Properties.Settings.Default.Save();
        }

        private void CheckBox_Cecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isAutoUpdateLauncher = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxInstallNow_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isRunTheGameImmediatelyAfterInstallingIt = false;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxInstallNow_Cecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.isRunTheGameImmediatelyAfterInstallingIt = true;
            Properties.Settings.Default.Save();
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

        #region Menu
        private bool ComboBoxChooseGameInLauncherHandle = true;

        private void ComboBoxChooseSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxChooseGameInLauncherHandle) ComboBoxChooseGameInLauncher_Handle();
            ComboBoxChooseGameInLauncherHandle = true;
        }

        private void ComboBoxChooseSettings_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox ComboBoxChooseGameInLauncher = sender as ComboBox;
            ComboBoxChooseGameInLauncherHandle = !ComboBoxChooseGameInLauncher.IsDropDownOpen;
            ComboBoxChooseGameInLauncher_Handle();
        }

        private void ComboBoxChooseGameInLauncher_Handle()
        {
            switch (ComboBoxChooseGameInLauncher.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.outputType = 0;
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 1:
                    Properties.Settings.Default.outputType = 1;
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 2:
                    Properties.Settings.Default.outputType = 2;
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
            }
        }
        #endregion
    }
}
