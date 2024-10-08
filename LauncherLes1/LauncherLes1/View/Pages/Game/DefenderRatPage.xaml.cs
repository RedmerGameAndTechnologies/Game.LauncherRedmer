using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LauncherLes1.View
{
    public partial class OpenDefenderRatPage : Page
    {
        private readonly string zipPath = @".\ChacheDownloadGame.zip";
        private readonly string appTemlPath = "tempDirectoryUnzip";
        private int? idProcessApp = null;
        private bool appIsStarting = false;
        private bool isStartUnzipUpdateFileApp = true;
        private Process processApp;

        private DispatcherTimer dispatcherTimer;
        private Stopwatch stopWatch = new Stopwatch();
        public static string ArgumentsAppString { get; set; }
        public static int ArgumentsAppSpeedDownload { get; set; } = 81920;

        WebClient clientDownloadApp = new WebClient();
        HttpClient httpClient = new HttpClient();

        string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string exepath = Assembly.GetEntryAssembly().Location;

        public OpenDefenderRatPage()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExcepctionEventApp);

            InitializeComponent();
            UpdateUI();
        }

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

            if (LauncherLes1.Properties.Settings.Default.appIsInstalled == false)
            {
                LaunchGameButton.IsEnabled = true;
                LaunchGameButton.Content = "Установить";
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
            string todayTimeLog = now.ToString("mmHHddMMyyyyy");
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
            ButtonCancelDownloadFile.IsEnabled = true;
            ButtonCancelDownloadFile.Visibility = Visibility.Visible;
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
                DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "Unzip...");
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
                foreach (string dgfse in Directory.GetFileSystemEntries(appTemlPath + "/Game"))
                {
                    FileAttributes attributes = File.GetAttributes(dgfse);
                    DirectoryInfo dirInf = new DirectoryInfo(dgfse);
                    FileInfo fileInf = new FileInfo(dgfse);
                    if (Directory.Exists(@"Game/") == false)
                    {
                        Directory.CreateDirectory("Game");
                    }
                    if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        if (Directory.Exists(@"Game/" + dirInf.Name) == true)
                        {
                            Directory.Delete(@"Game/" + dirInf.Name, true);
                        }
                        dirInf.MoveTo(@"Game/" + dirInf.Name);
                    }
                    else if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        if (File.Exists(@"Game/" + fileInf.Name) == true)
                        {
                            File.Delete(@"Game/" + fileInf.Name);
                        }
                        fileInf.MoveTo(@"Game/" + fileInf.Name);
                    }
                }
                DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "Статус: " + "игра установлена");
                DownloadAppState.Dispatcher.Invoke(() => LauncherLes1.Properties.Settings.Default.appIsInstalled = true);
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
            if (LauncherLes1.Properties.Settings.Default.appIsInstalled == false)
            {
                ServerDownloadChacheGameAsync();
            }
            else {
                try
                {
                    processApp = new Process();
                    processApp.StartInfo.UseShellExecute = false;
                    processApp.StartInfo.FileName = @"Game\The World of Quantrianism.exe";
                    processApp.StartInfo.Arguments = ArgumentsAppString;
                    processApp.Start();
                    idProcessApp = processApp.Id;
                }
                catch (Exception ex)
                {
                    LoggingProcess("EXCEPTION" + ex.Message.ToString());
                }
            }
        }
        #endregion

        #region CancelDownloadGame
        private void ButtonCancelDownloadApp(object sender, RoutedEventArgs e)
        {
            clientDownloadApp.CancelAsync();
            try
            {
                cancelTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "State: " + ex.Message.ToString());
            }
            ButtonCancelDownloadFile.IsEnabled = false;
            ButtonCancelDownloadFile.Visibility = Visibility.Hidden;
        }
        #endregion

        private void ProgressMessageHandler_HttpReceiveProgress(object sender, HttpProgressEventArgs e)
        {
            var calculateBytesSpeedWrite = e.BytesTransferred / 1024d / (stopWatch.ElapsedMilliseconds / 1000d);
            DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "Progress download: " + e.ProgressPercentage + " Average speed download: " + (int)calculateBytesSpeedWrite + " kb/s");
            ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Value = e.ProgressPercentage);
        }

        #region Menu
        private bool ComboBoxChooseGameInLauncherHandle = true;

        private void ComboBoxChooseGameInLauncher_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
                    Process.Start(@".\");
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 1:
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 2:
                    if (Directory.Exists(@"Game/") == true && appIsStarting == false)
                    {
                        try
                        {
                            Directory.Delete(@"Game/", recursive: true);
                            LauncherLes1.Properties.Settings.Default.appIsInstalled = false;
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
