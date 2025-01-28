using CheckConnectInternet;
using LauncherLes1.View.Pages;
using LauncherLes1.View.Resources.Script;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LauncherLes1.View.Windows
{
    public partial class ConfirmUpdateWindow : Window
    {
        private readonly string urlJSON = "https://raw.githubusercontent.com/RedmerGameAndTechnologies/JsonLauncher/refs/heads/main/VersionLauncher.json";

        private UpdateContentLauncherUpdate updateContentLauncherUpdate = new UpdateContentLauncherUpdate();

        public ConfirmUpdateWindow() {
            InitializeComponent();
            Task task = updateContentLauncherUpdate.Main(urlJSON);
        }

        private void WindowMove(object sender, MouseButtonEventArgs e)
        {   
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonAllowUpdate(object sender, RoutedEventArgs e)
        {
            SettingsPage.isActiveUpdateLauncherWindow = false;
            Close();

            using (WebClient wc = new WebClient())
            {
                if (Internet.connect())
                {
                    if (!string.IsNullOrEmpty(Paths.zipPathUpdate) && File.Exists(Paths.zipPathUpdate))
                        File.Delete(Paths.zipPathUpdate);
                    else if (!string.IsNullOrEmpty(Paths.exeLauncherUpdate) && File.Exists(Paths.exeLauncherUpdate))
                        File.Delete(Paths.exeLauncherUpdate);
                    try
                    {
                        wc.DownloadFile(updateContentLauncherUpdate.fileDownloadLink, "UpdateLaucnher.zip");
                        ZipFile.ExtractToDirectory(Paths.zipPathUpdate, Paths.exetraPath);
                        File.Delete(Paths.zipPathUpdate);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    CmdClass.Cmd($"taskkill /f /im \"{Arguments.exenames}\" && timeout /t 1 && del \"{Arguments.execPath}\" && ren NewLauncher.exe \"{Arguments.exenames}\" && \"{Arguments.execPath}\"");
                }
                else MessageBox.Show("Ошибка", "подключитесь к интернету", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonNoAllowUpdate(object sender, RoutedEventArgs e)
        {
            SettingsPage.isActiveUpdateLauncherWindow = false;
            Close();
        }
    }
}
