using LauncherLes1.View.Resources.Script;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LauncherLes1.View
{
    public partial class OpenDefenderRatPage : Page
    {
        private readonly static string name = "DefenderRat";
        private readonly string zipPath = @".\ChacheDownloadGame.zip";
        private readonly string appTemlPath = "tempDirectoryUnzip";
        private readonly string appGamePath = $@"{name}/";
        private int? idProcessApp = null;
        private bool appIsStarting = false;
        private bool isStartUnzipUpdateFileApp = true;
        private static bool isFileDownloadingNow = false;
        private Process processApp;

        private DispatcherTimer dispatcherTimer;
        private Stopwatch stopWatch = new Stopwatch();
        public static string ArgumentsAppString { get; set; }
        public static int ArgumentsAppSpeedDownload { get; set; } = 81920;

        WebClient clientDownloadApp = new WebClient();
        HttpClient httpClient = new HttpClient();

        public OpenDefenderRatPage()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExcepctionEventApp);

            InitializeComponent();
            UpdateUI();
            //ReadJsonFile();
        }

/*        #region ReadJsonFile
        private void ReadJsonFile()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.GetStringAsync("https://pastebin.com/raw/rv38asrb");
                }
                catch (HttpRequestException e) when (e.Message.Contains("404"))
                {
                    MessageBox.Show($"Файла нет на сервере: {e.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

            }
        }
        #endregion*/

        private void UpdateUI()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(BackgroundUIFunction);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        #region BACKGROUNDFUNC
        public void BackgroundUIFunction(object sender, EventArgs ea)
        {
            ProgressBarExtractFile.Minimum = 0;
            Process[] processedUsers = Process.GetProcesses();
            foreach (Process allprocessed in processedUsers)
            {
                if (allprocessed.Id == idProcessApp)
                {
                    appIsStarting = true;
                    break;
                }
                else if (allprocessed.Id != idProcessApp) appIsStarting = false;
            }

            if (isFileDownloadingNow == true)
            {
                LaunchGameButton.IsEnabled = true;
                LaunchGameButton.Content = "Отмена";
            }
            else
            {
                if (!Directory.Exists(appGamePath))
                {
                    LaunchGameButton.IsEnabled = true;
                    LaunchGameButton.Content = "Установить";

                    ComboBoxChooseGameInLauncher.IsEnabled = false;
                }
                else{
                    if (appIsStarting == false)
                    {
                        LaunchGameButton.IsEnabled = true;
                        LaunchGameButton.Content = "Играть";
                    }
                    else
                    {
                        LaunchGameButton.IsEnabled = false;
                        LaunchGameButton.Content = "Игра запущена";
                    }
                }
            }
        }
        #endregion

        #region Loges
        private static void ExcepctionEventApp(object sender, UnhandledExceptionEventArgs ueea)
        {
            Exception e = (Exception)ueea.ExceptionObject;
            LoggingProcess("EXCEPTION" + e.Message.ToString());
        }

        private static void LoggingProcess(string s)
        {
            if (Directory.Exists(@"Log/") == false)
            {
                Directory.CreateDirectory(@"Log/");
            }
            DateTime now = DateTime.Now;
            string todayTimeLog = now.ToString("mm_HH_dd_MM_yyyy");
            string nameFileLog = @"Log/" + "Log" + todayTimeLog + ".txt";

            using (StreamWriter sw = new StreamWriter(nameFileLog, false))
            {
                sw.WriteLineAsync(s);
            }
        }
        #endregion

        #region UPDATEFUNC
        CancellationTokenSource cancelTokenSource;
        public void ServerDownloadChacheGameAsync()
        {
            isFileDownloadingNow = true;
            if (!string.IsNullOrEmpty(zipPath) && File.Exists(zipPath)) {
                File.Delete(zipPath);
            }
            try
            {
                LaunchGameButton.IsEnabled = false;
                if (cancelTokenSource == null || cancelTokenSource.IsCancellationRequested)
                {
                    cancelTokenSource = new CancellationTokenSource();
                }
                CancellationToken cancellationToken = cancelTokenSource.Token;
                Task downloadFileHTTP = Task.Run(async () =>
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = new Uri("https://getfile.dokpub.com/yandex/get/https://disk.yandex.ru/d/EWSdZyEUQgtjVA") };
                    ProgressMessageHandler progressMessageHandler = new ProgressMessageHandler(new HttpClientHandler() { AllowAutoRedirect = true });
                    httpClient = new HttpClient(progressMessageHandler) { Timeout = Timeout.InfiniteTimeSpan };
                    stopWatch.Start();
                    progressMessageHandler.HttpReceiveProgress += ProgressMessageHandler_HttpReceiveProgress;
                    Stream streamFileServer = await httpClient.GetStreamAsync(httpRequestMessage.RequestUri);
                    Stream fileStreamServer = new FileStream(zipPath, FileMode.OpenOrCreate, FileAccess.Write);
                    try
                    {
                        await streamFileServer.CopyToAsync(fileStreamServer, ArgumentsAppSpeedDownload, cancellationToken);
                        cancelTokenSource.Dispose();
                        streamFileServer.Dispose();
                        fileStreamServer.Dispose();
                        return;
                    }
                    catch (Exception e)
                    {
                        DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "State: " + e.Message.ToString());
                        LaunchGameButton.Dispatcher.Invoke(() => LaunchGameButton.IsEnabled = true);
                        cancelTokenSource.Dispose();
                        streamFileServer.Dispose();
                        fileStreamServer.Dispose();
                        return;
                    }
                }, cancellationToken);
                downloadFileHTTP.ContinueWith(obj =>
                {
                DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "Распаковка файлов...");
                using (ZipArchive zipFileServer = ZipFile.OpenRead(zipPath))
                {
                        
                    ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Value = 0);
                    int zipFilesCount = zipFileServer.Entries.Count;
                    ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Maximum = zipFilesCount);
                    foreach (var zip in zipFileServer.Entries)
                    {
                        if (isStartUnzipUpdateFileApp == true)
                        {
                            zip.Archive.ExtractToDirectory(appTemlPath);
                            isStartUnzipUpdateFileApp = false;
                            break;
                        }
                    }
                    ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Value = zipFilesCount);
                }
                File.Delete(zipPath);
                foreach (string dgfse in Directory.GetFileSystemEntries(appTemlPath + $"/{name}"))
                {
                    FileAttributes attributes = File.GetAttributes(dgfse);
                    DirectoryInfo dirInf = new DirectoryInfo(dgfse);
                    FileInfo fileInf = new FileInfo(dgfse);
                    if (!Directory.Exists(appGamePath))
                    {
                        Directory.CreateDirectory(appGamePath);
                    }
                    if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        if (Directory.Exists(appGamePath + dirInf.Name))
                        {
                            Directory.Delete(appGamePath + dirInf.Name, true);
                        }
                        dirInf.MoveTo(appGamePath + dirInf.Name);
                    }
                    else if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        if (File.Exists(appGamePath + fileInf.Name))
                        {
                            File.Delete(appGamePath + fileInf.Name);
                        }
                        fileInf.MoveTo(appGamePath + fileInf.Name);
                    }
                }
                DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "Статус: " + "игра установлена");
                DownloadAppState.Dispatcher.Invoke(() => isFileDownloadingNow = false);
                if (Properties.Settings.Default.isRunTheGameImmediatelyAfterInstallingIt == true) {
                   DownloadAppState.Dispatcher.Invoke(() => Task.Run(() => {
                        processApp = new Process();
                        processApp.StartInfo.UseShellExecute = false;
                        processApp.StartInfo.FileName = appGamePath + @".\Defender Rat.exe";
                        processApp.StartInfo.Arguments = ArgumentsAppString;
                        processApp.Start();
                        idProcessApp = processApp.Id;
                   }));
                }
                ComboBoxChooseGameInLauncher.Dispatcher.Invoke(() => ComboBoxChooseGameInLauncher.IsEnabled = true);
                LaunchGameButton.Dispatcher.Invoke(() => LaunchGameButton.IsEnabled = true);
                ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Value = 0);
                return;
                }, cancellationToken);
            }
            catch (Exception e)
            {
                LoggingProcess("EXCEPTION E: " + e.Message.ToString());
                DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "State task: " + e.Message.ToString());
            }
        }
        #endregion

        #region ButtonLaunchGame
        private void ButtonLaunchGame(object sender, RoutedEventArgs e)
        {
            if (LaunchGameButton.Content.ToString() == "Установить") {
                if (!Directory.Exists(appGamePath))
                {
                    ServerDownloadChacheGameAsync();
                }
            }
            if (LaunchGameButton.Content.ToString() == "Играть") {
                try
                {
                    processApp = new Process();
                    processApp.StartInfo.UseShellExecute = false;
                    processApp.StartInfo.FileName = appGamePath + @".\Defender Rat.exe";
                    processApp.StartInfo.Arguments = ArgumentsAppString;
                    processApp.Start();
                    idProcessApp = processApp.Id;
                }
                catch (Exception ex)
                {
                    LoggingProcess("EXCEPTION" + ex.Message.ToString());
                }
            }
            if (LaunchGameButton.Content.ToString() == "Отмена") {
                clientDownloadApp.CancelAsync();
                try
                {
                    cancelTokenSource.Cancel();
                }
                catch (Exception ex)
                {
                    DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "State: " + ex.Message.ToString());
                }
            }
        }
        #endregion

        #region ProgresBarDownloadGame
        private void ProgressMessageHandler_HttpReceiveProgress(object sender, HttpProgressEventArgs e)
        {
            string message;

            switch (Properties.Settings.Default.outputType) {
                case 0:
                    message = "Процесс установки: " + e.ProgressPercentage + "%" + " Скорость скачивание: " + BytesToString(e.BytesTransferred) + "/" + BytesToString(e.TotalBytes.Value);
                    break;
                case 1:
                    message = "Процесс установки: " + e.ProgressPercentage + "%";
                    break;
                case 2:
                    message = "Процесс установки: " + BytesToString(e.BytesTransferred) + "/" + BytesToString(e.TotalBytes.Value);
                    break;
                default:
                    message = "Процесс установки: " + e.ProgressPercentage + "%" + " Скорость скачивание: " + BytesToString(e.BytesTransferred) + "/" + BytesToString(e.TotalBytes.Value);
                    break;
            }
            DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = message);
            ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Value = e.ProgressPercentage);
        }
        #endregion

        #region BytesConvertToString
        private static string BytesToString(long byteCount) {
            string[] suf = { "B", "KB", "MB", "GB"};
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(bytes) * num).ToString() + suf[place];
        }
        #endregion

        #region Menu
        private bool ComboBoxChooseGameInLauncherHandle = true;

        private void ComboBoxChooseGameInLauncher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxChooseGameInLauncherHandle) ComboBoxChooseGameInLauncher_Handle();
            ComboBoxChooseGameInLauncherHandle = true;
        }

        private void ComboBoxChooseGameInLauncher_DropDownClosed(object sender, EventArgs e)
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
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = $@"/{name}",
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex) {
                        MessageBox.Show($"Не известная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 1:
                    CreateIconClass.CreateShortcut(name);
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 2:
                    if (Directory.Exists(appGamePath) == true && appIsStarting == false)
                    {
                        try
                        {
                            Directory.Delete(appGamePath, recursive: true);
                            DownloadAppState.Text = "Статус: " + "Игра удалена";
                        }
                        catch (Exception ex) {
                            MessageBox.Show($"Ошибка у вас игра запущена: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 3:
                    ServerDownloadChacheGameAsync();
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 4:
                    ServerDownloadChacheGameAsync();
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
            }
        }
        #endregion
    }
}
