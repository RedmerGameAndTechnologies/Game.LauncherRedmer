using LauncherLes1.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private DispatcherTimer dispatcherTimer;

        string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static bool isActiveUpdateLauncherWindow { get; set; } = false;

        Window ConfirmUpdate;

        public SettingsPage()
        {
            InitializeComponent();
            UpdateUI();
            StartCheck_CheckBoxAutoUpdateLauncher();
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

        private void UpdateUI()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(BackgroundUIFunction);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }
        public void BackgroundUIFunction(object sender, EventArgs ea) {
            _textServerVersion.Content = "Версия лаунчера: " + curver;
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
    }
}
