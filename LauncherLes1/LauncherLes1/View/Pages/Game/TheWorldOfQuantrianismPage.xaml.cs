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
using LauncherLes1.View.Resources.Script;

namespace LauncherLes1.View
{
    public partial class TheWorldOfQuantrianismPage : Page
    {
        private readonly string urlJSON = "https://raw.githubusercontent.com/RedmerGameAndTechnologies/JsonLauncher/refs/heads/main/TheWorldOfQuantrianism.json";
        private readonly string zipPath = @".\ChacheDownloadGame.zip";
        private readonly string appTemlPath = "tempDirectoryUnzip";

        private int? idProcessApp = null;
        private bool appIsStarting = false;
        private bool isStartUnzipUpdateFileApp = true;
        private bool isFileDownloadingNow = false;
        public static string ArgumentsAppString { get; set; }

        private Process processApp;
        private Stopwatch stopWatch = new Stopwatch();
        private UpdateContent updateContent = new UpdateContent();

        WebClient clientDownloadApp = new WebClient();
        HttpClient httpClient = new HttpClient();

        public TheWorldOfQuantrianismPage()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Loges.ExcepctionEventApp);

            InitializeComponent();
            UpdateUI.Update(BackgroundUIFunction, 0,0,2);

            Task task = updateContent.Main(urlJSON, versionGame, background);
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
                if (!Directory.Exists(updateContent.appGamePath))
                {
                    LaunchGameButton.IsEnabled = true;
                    LaunchGameButton.Content = "Установить";

                    ComboBoxChooseGameInLauncher.IsEnabled = false;
                }
                else{
                    LaunchGameButton.Content = appIsStarting ? "Игра запущена" : "Играть";
                    LaunchGameButton.IsEnabled = !appIsStarting;
                }
            }
        }
        #endregion

        #region UPDATEFUNC
        CancellationTokenSource cancelTokenSource;
        public void ServerDownloadChacheGameAsync()
        {
            isFileDownloadingNow = true;
            if (!string.IsNullOrEmpty(zipPath) && File.Exists(zipPath))
            {
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
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = new Uri(updateContent.urlDownload) };
                    ProgressMessageHandler progressMessageHandler = new ProgressMessageHandler(new HttpClientHandler() { AllowAutoRedirect = true });
                    httpClient = new HttpClient(progressMessageHandler) { Timeout = Timeout.InfiniteTimeSpan };
                    stopWatch.Start();
                    progressMessageHandler.HttpReceiveProgress += ProgressMessageHandler_HttpReceiveProgress;
                    Stream streamFileServer = await httpClient.GetStreamAsync(httpRequestMessage.RequestUri);
                    Stream fileStreamServer = new FileStream(zipPath, FileMode.OpenOrCreate, FileAccess.Write);
                    try
                    {
                        await streamFileServer.CopyToAsync(fileStreamServer, Properties.Settings.Default.targetSpeedInKb, cancellationToken);
                        cancelTokenSource.Dispose();
                        streamFileServer.Dispose();
                        fileStreamServer.Dispose();
                        return;
                    }
                    catch (Exception e)
                    {
                        DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "State: " + e.Message.ToString());
                        LaunchGameButton.Dispatcher.Invoke(() => LaunchGameButton.IsEnabled);
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
                            if (isStartUnzipUpdateFileApp)
                            {
                                zip.Archive.ExtractToDirectory(appTemlPath);
                                isStartUnzipUpdateFileApp = false;
                                break;
                            }
                        }
                        ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Value = zipFilesCount);
                    }
                    File.Delete(zipPath);
                    foreach (string dgfse in Directory.GetFileSystemEntries(appTemlPath + $"/{updateContent.name}"))
                    {
                        FileAttributes attributes = File.GetAttributes(dgfse);
                        DirectoryInfo dirInf = new DirectoryInfo(dgfse);
                        FileInfo fileInf = new FileInfo(dgfse);
                        if (!Directory.Exists(updateContent.appGamePath))
                        {
                            Directory.CreateDirectory(updateContent.appGamePath);
                        }
                        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            if (Directory.Exists(updateContent.appGamePath + dirInf.Name))
                            {
                                Directory.Delete(updateContent.appGamePath + dirInf.Name, true);
                            }
                            dirInf.MoveTo(updateContent.appGamePath + dirInf.Name);
                        }
                        else if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
                        {
                            if (File.Exists(updateContent.appGamePath + fileInf.Name))
                            {
                                File.Delete(updateContent.appGamePath + fileInf.Name);
                            }
                            fileInf.MoveTo(updateContent.appGamePath + fileInf.Name);
                        }
                    }
                    DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "Статус: " + "игра установлена");
                    DownloadAppState.Dispatcher.Invoke(() => isFileDownloadingNow = false);
                    if (Properties.Settings.Default.isRunTheGameImmediatelyAfterInstallingIt == true)
                    {
                        DownloadAppState.Dispatcher.Invoke(() => Task.Run(() =>
                        {
                            processApp = new Process();
                            processApp.StartInfo.UseShellExecute = false;
                            processApp.StartInfo.FileName = updateContent.appGamePath + @".\Defender Rat.exe";
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
                Loges.LoggingProcess("EXCEPTION E: " + e.Message.ToString());
                DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "State task: " + e.Message.ToString());
            }
        }
        #endregion

        #region ButtonLaunchGame
        private void ButtonLaunchGame(object sender, RoutedEventArgs e)
        {
            if (LaunchGameButton.Content.ToString() == "Установить")
                if (!Directory.Exists(updateContent.appGamePath))
                    ServerDownloadChacheGameAsync();
            if (LaunchGameButton.Content.ToString() == "Играть") {
                try
                {
                    processApp = new Process();
                    processApp.StartInfo.UseShellExecute = false;
                    processApp.StartInfo.FileName = updateContent.appGamePath + @".\Defender Rat.exe";
                    processApp.StartInfo.Arguments = ArgumentsAppString;
                    processApp.Start();
                    idProcessApp = processApp.Id;
                }
                catch (Exception ex)
                {
                    Loges.LoggingProcess("EXCEPTION" + ex.Message.ToString());
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
            => ProgressMessage.ProgressMessageHandler(sender, e, DownloadAppState, ProgressBarExtractFile);
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
                            Arguments = updateContent.name,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex) {
                        Loges.LoggingProcess("EXCEPTION: " + ex.Message.ToString());
                        MessageBox.Show($"Не известная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 1:
                    CreateIconClass.CreateShortcut(updateContent.name);
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 2:
                    if (Directory.Exists(updateContent.appGamePath) && !appIsStarting)
                    {
                        Directory.Delete(updateContent.appGamePath, recursive: true);
                        DownloadAppState.Text = "Статус: Игра удалена";
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