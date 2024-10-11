using CheckConnectInternet;
using LauncherLes1.View.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;

namespace LauncherLes1.View.Windows
{
    public partial class ConfirmUpdateWindow : Window
    {
        private readonly string zipPathUpdate = @".\UpdateLaucnher.zip";
        private readonly string exeLauncherUpdate = @".\NewLauncher.exe";
        private readonly string exetraPath = @".\";

        public static string execPath = Process.GetCurrentProcess().MainModule.FileName;
        public static string workingDir = Path.GetDirectoryName(execPath);
        public static string sourcePath = Path.Combine(workingDir, Path.GetFileName(execPath));
        public static string exenames = Path.GetFileName(sourcePath);

        public ConfirmUpdateWindow()
        {
            InitializeComponent();
        }

        private void ButtonAllowUpdate(object sender, RoutedEventArgs e)
        {
            SettingsPage.isActiveUpdateLauncherWindow = false;
            Close();

            using (WebClient wc = new WebClient())
            {
                if (Internet.connect())
                {
                    if (!string.IsNullOrEmpty(zipPathUpdate) && File.Exists(zipPathUpdate))
                        File.Delete(zipPathUpdate);
                    else if (!string.IsNullOrEmpty(exeLauncherUpdate) && File.Exists(exeLauncherUpdate))
                        File.Delete(exeLauncherUpdate);
                    try
                    {
                        wc.DownloadFile("https://getfile.dokpub.com/yandex/get/https://disk.yandex.ru/d/nG-fxSX_twOw5A", "UpdateLaucnher.zip");
                        ZipFile.ExtractToDirectory(zipPathUpdate, exetraPath);
                        File.Delete(zipPathUpdate);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    Cmd($"taskkill /f /im \"{exenames}\" && timeout /t 1 && del \"{execPath}\" && ren NewLauncher.exe \"{exenames}\" && \"{execPath}\"");
                }
                else MessageBox.Show("Ошибка", "подключитесь к интернету", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonNoAllowUpdate(object sender, RoutedEventArgs e)
        {
            SettingsPage.isActiveUpdateLauncherWindow = false;
            Close();
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
    }
}
