using CheckConnectInternet;
using LauncherLes1.View.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace LauncherLes1.View.Windows
{
    public partial class ConfirmUpdateWindow : Window
    {
        private readonly string zipPathUpdate = @".\UpdateLaucnher.zip";
        private readonly string exetraPath = @".\";

        string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string exename = AppDomain.CurrentDomain.FriendlyName;
        string exepath = Assembly.GetEntryAssembly().Location;

        public ConfirmUpdateWindow()
        {
            InitializeComponent();
            isUpdateText();
        }

        private async void isUpdateText() {
            string readver = await HttpResponse("https://pastebin.com/raw/dem4T7Xp");

            readver = readver.Replace(".", ".");
            curver = curver.Replace(".", ".");

            if (curver == readver)
            {
                _textUpdateLauncher.Text = "У вас последняя версия лаунчера";
                MyVersion.Text = "Моя версия: " + curver;

                Button1.Visibility = Visibility.Hidden;
            }
            else
            {
                _textUpdateLauncher.Text = "Вышла новая версия лаунчера";

                MyVersion.Text = "Моя версия: " + curver;
                NewVersion.Text = "Новая версия: " + readver;
            }
        }

        private void ButtonAllowUpdate(object sender, RoutedEventArgs e)
        {
            SettingsPage.isActiveUpdateLauncherWindow = true;
            Close();

            using (WebClient wc = new WebClient())
            {
                if (Internet.connect())
                {
                    string readver = wc.DownloadString("https://pastebin.com/raw/dem4T7Xp");
                    if (curver == readver)
                    {
                        Button1.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(zipPathUpdate) && File.Exists(zipPathUpdate))
                        {
                            File.Delete(zipPathUpdate);
                        }
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
                        Cmd($"taskkill /f /im \"{exename}\" && timeout /t 1 && del \"{exepath}\" && ren NewLaucnher.exe \"{exename}\" && \"{exepath}\"");
                    }
                }
                else MessageBox.Show("Ошибка", "подключитесь к интернету", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            //string readver = await HttpResponse("https://pastebin.com/raw/dem4T7Xp");
            //using (HttpClient clientDownloadApp = new HttpClient())
            //{
            //    if (Internet.connect())
            //    {
            //        if (curver == readver)
            //        {
            //            Button1.Visibility = Visibility.Hidden;
            //        }
            //        else
            //        {
            //            try
            //            {
            //                using (var stream = await clientDownloadApp.GetStreamAsync("https://getfile.dokpub.com/yandex/get/https://disk.yandex.ru/d/nG-fxSX_twOw5A"))
            //                    if (stream == null)
            //                    {
            //                        throw new Exception("Поток не получен. Возможно, ошибка в URL.");
            //                    }
            //                ZipFile.ExtractToDirectory(zipPathUpdate, exetraPath);
            //                File.Delete(zipPathUpdate);
            //                Cmd($"taskkill /f /im \"{exename}\" && timeout /t 1 && del \"{exepath}\" && ren NewLaucnher.exe \"{exename}\" && \"{exepath}\"");
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //            }
            //            //if (zipPathUpdate == null)
            //            //{
            //            //    File.Delete(zipPathUpdate);
            //            //}
            //            //else
            //            //{
            //            //    using (var stream = await clientDownloadApp.GetStreamAsync("https://getfile.dokpub.com/yandex/get/https://disk.yandex.ru/d/wcuilEg_8UcZXw"))
            //            //    using (var file = new FileStream("New.exe", FileMode.CreateNew))
            //            //        await stream.CopyToAsync(file);
            //            //    //ZipFile.ExtractToDirectory(zipPathUpdate, exetraPath);
            //            //}


            //            //using (var file = new FileStream("New.exe", FileMode.CreateNew))
            //            //    await stream.CopyToAsync(file);

            //        }
            //    }
            //    else MessageBox.Show("Ошибка", "подключитесь к интернету", MessageBoxButton.OK, MessageBoxImage.Information);
            //}

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

        async Task<string> HttpResponse(string line) {
            using (var net = new HttpClient()) {
                var response = await net.GetAsync(line);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
            }
        }
    }
}
