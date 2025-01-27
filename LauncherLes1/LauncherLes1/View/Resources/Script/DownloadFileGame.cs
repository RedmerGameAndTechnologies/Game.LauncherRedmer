using System;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Net.Http.Handlers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LauncherLes1.View.Resources.Script
{
/*    class DownloadFileGame
    {
        private static Stopwatch stopWatch = new Stopwatch();

        private static HttpClient httpClient = new HttpClient();

        private static CancellationTokenSource cancelTokenSource;

        public static void ServerDownloadChacheGameAsync(string name, string zipPath, string appTemlPath, string appGamePath, string ArgumentsAppString, bool isFileDownloadingNow, bool isStartUnzipUpdateFileApp, ProgressBar ProgressBarExtractFile, Button LaunchGameButton, TextBlock DownloadAppState, Process processApp, int? idProcessApp)
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
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = new Uri(UpdateContent.urlDownload) };
                    ProgressMessageHandler progressMessageHandler = new ProgressMessageHandler(new HttpClientHandler() { AllowAutoRedirect = true });
                    httpClient = new HttpClient(progressMessageHandler) { Timeout = Timeout.InfiniteTimeSpan };
                    stopWatch.Start();
                    //progressMessageHandler.HttpReceiveProgress += ProgressMessageHandler_HttpReceiveProgress;
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
                    if (Properties.Settings.Default.isRunTheGameImmediatelyAfterInstallingIt == true)
                    {
                        DownloadAppState.Dispatcher.Invoke(() => Task.Run(() =>
                        {
                            processApp = new Process();
                            processApp.StartInfo.UseShellExecute = false;
                            processApp.StartInfo.FileName = appGamePath + @".\Defender Rat.exe";
                            processApp.StartInfo.Arguments = ArgumentsAppString;
                            processApp.Start();
                            idProcessApp = processApp.Id;
                        }));
                    }
                    //ComboBoxChooseGameInLauncher.Dispatcher.Invoke(() => ComboBoxChooseGameInLauncher.IsEnabled = true);
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
    }*/
}
