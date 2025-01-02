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
    public partial class OpenDefenderRatPage : Page
    {
        private readonly string urlJSON = "https://raw.githubusercontent.com/RedmerGameAndTechnologies/JsonLauncher/refs/heads/main/DefenderRat.json";
        private Process processApp;
        private Stopwatch stopWatch = new Stopwatch();

        WebClient clientDownloadApp = new WebClient();
        HttpClient httpClient = new HttpClient();

        public OpenDefenderRatPage()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Loges.ExcepctionEventApp);

            InitializeComponent();
            UpdateUI.Update(BackgroundUIFunction, 0,0,2);
            _ = UpdateContent.Main(urlJSON, versionGame, background);
        }

        #region BACKGROUNDFUNC
        public void BackgroundUIFunction(object sender, EventArgs ea)
        {
            ProgressBarExtractFile.Minimum = 0;
            Process[] processedUsers = Process.GetProcesses();
            foreach (Process allprocessed in processedUsers)
            {
                if (allprocessed.Id == Arguments.idProcessApp)
                {
                    Arguments.appIsStarting = true;
                    break;
                }
                else if (allprocessed.Id != Arguments.idProcessApp) Arguments.appIsStarting = false;
            }

            if (Arguments.isFileDownloadingNow == true)
            {
                LaunchGameButton.IsEnabled = true;
                LaunchGameButton.Content = "Отмена";
            }
            else
            {
                if (!Directory.Exists(Paths.appGamePath))
                {
                    LaunchGameButton.IsEnabled = true;
                    LaunchGameButton.Content = "Установить";

                    ComboBoxChooseGameInLauncher.IsEnabled = false;
                }
                else{
                    if (Arguments.appIsStarting == false)
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

        #region UPDATEFUNC
        CancellationTokenSource cancelTokenSource;
        public void ServerDownloadChacheGameAsync()
        {
            Arguments.isFileDownloadingNow = true;
            if (!string.IsNullOrEmpty(Paths.zipPath) && File.Exists(Paths.zipPath))
            {
                File.Delete(Paths.zipPath);
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
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = new Uri(UpdateContent.urlDownload) };
                    ProgressMessageHandler progressMessageHandler = new ProgressMessageHandler(new HttpClientHandler() { AllowAutoRedirect = true });
                    httpClient = new HttpClient(progressMessageHandler) { Timeout = Timeout.InfiniteTimeSpan };
                    stopWatch.Start();
                    progressMessageHandler.HttpReceiveProgress += ProgressMessageHandler_HttpReceiveProgress;
                    Stream streamFileServer = await httpClient.GetStreamAsync(httpRequestMessage.RequestUri);
                    Stream fileStreamServer = new FileStream(Paths.zipPath, FileMode.OpenOrCreate, FileAccess.Write);
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
                    using (ZipArchive zipFileServer = ZipFile.OpenRead(Paths.zipPath))
                    {
                        ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Value = 0);
                        int zipFilesCount = zipFileServer.Entries.Count;
                        ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Maximum = zipFilesCount);
                        foreach (var zip in zipFileServer.Entries)
                        {
                            if (Arguments.isStartUnzipUpdateFileApp)
                            {
                                zip.Archive.ExtractToDirectory(Paths.appTemlPath);
                                Arguments.isStartUnzipUpdateFileApp = false;
                                break;
                            }
                        }
                        ProgressBarExtractFile.Dispatcher.Invoke(() => ProgressBarExtractFile.Value = zipFilesCount);
                    }
                    File.Delete(Paths.zipPath);
                    foreach (string dgfse in Directory.GetFileSystemEntries(Paths.appTemlPath + $"/{UpdateContent.name}"))
                    {
                        FileAttributes attributes = File.GetAttributes(dgfse);
                        DirectoryInfo dirInf = new DirectoryInfo(dgfse);
                        FileInfo fileInf = new FileInfo(dgfse);
                        if (!Directory.Exists(Paths.appGamePath))
                        {
                            Directory.CreateDirectory(Paths.appGamePath);
                        }
                        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            if (Directory.Exists(Paths.appGamePath + dirInf.Name))
                            {
                                Directory.Delete(Paths.appGamePath + dirInf.Name, true);
                            }
                            dirInf.MoveTo(Paths.appGamePath + dirInf.Name);
                        }
                        else if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
                        {
                            if (File.Exists(Paths.appGamePath + fileInf.Name))
                            {
                                File.Delete(Paths.appGamePath + fileInf.Name);
                            }
                            fileInf.MoveTo(Paths.appGamePath + fileInf.Name);
                        }
                    }
                    DownloadAppState.Dispatcher.Invoke(() => DownloadAppState.Text = "Статус: " + "игра установлена");
                    DownloadAppState.Dispatcher.Invoke(() => Arguments.isFileDownloadingNow = false);
                    if (Properties.Settings.Default.isRunTheGameImmediatelyAfterInstallingIt == true)
                    {
                        DownloadAppState.Dispatcher.Invoke(() => Task.Run(() =>
                        {
                            processApp = new Process();
                            processApp.StartInfo.UseShellExecute = false;
                            processApp.StartInfo.FileName = Paths.appGamePath + @".\Defender Rat.exe";
                            processApp.StartInfo.Arguments = Arguments.ArgumentsAppString;
                            processApp.Start();
                            Arguments.idProcessApp = processApp.Id;
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
                if (!Directory.Exists(Paths.appGamePath))
                    ServerDownloadChacheGameAsync();
            if (LaunchGameButton.Content.ToString() == "Играть") {
                try
                {
                    processApp = new Process();
                    processApp.StartInfo.UseShellExecute = false;
                    processApp.StartInfo.FileName = Paths.appGamePath + @".\Defender Rat.exe";
                    processApp.StartInfo.Arguments = Arguments.ArgumentsAppString;
                    processApp.Start();
                    Arguments.idProcessApp = processApp.Id;
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
                            Arguments = UpdateContent.name,
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
                    CreateIconClass.CreateShortcut(UpdateContent.name);
                    ComboBoxChooseGameInLauncher.SelectedIndex = -1;
                    break;
                case 2:
                    if (Directory.Exists(Paths.appGamePath) && !Arguments.appIsStarting)
                    {
                        Directory.Delete(Paths.appGamePath, recursive: true);
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