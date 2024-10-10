using CheckConnectInternet;
using LauncherLes1.View.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Windows;

namespace LauncherLes1.View.Windows
{
    public partial class ConfirmUpdateWindow : Window
    {
        private readonly string zipPathUpdate = @".\UpdateLaucnher.zip";
        private readonly string exeLauncherUpdate = @".\NewLauncher.exe";
        private readonly string exetraPath = @".\";

        string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string exename = AppDomain.CurrentDomain.FriendlyName;
        string exepath = Assembly.GetEntryAssembly().Location;

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
                    string readver = wc.DownloadString("https://pastebin.com/raw/dem4T7Xp");
                    if (curver == readver)
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
                        ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                        info.Arguments = $"/c taskkill /f /im LauncherRemer.exe && timeout /t 1 && del LauncherRemer.exe && ren NewLauncher.exe LauncherRemer.exe && LauncherRemer.exe";
                        info.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(info);
                        //Cmd($"taskkill /f /im \"{exename}\" && timeout /t 1 && del \"{exepath}\" && ren NewLaucnher.exe \"{exename}\" && \"{exepath}\"");
                    }
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
