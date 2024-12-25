using System.Net.Http.Handlers;
using System.Windows.Controls;

namespace LauncherLes1.View.Resources.Script
{
    class ProgressMessage
    {
        public static void ProgressMessageHandler(object sender, HttpProgressEventArgs e, TextBlock text, ProgressBar progress)
        {
            var bytes = BytesConvertToString.BytesToString;
            string message;

            switch (Properties.Settings.Default.outputType)
            {
                case 0:
                    message = "Процесс установки: " + e.ProgressPercentage + "%" + " Осталось: " + bytes(e.BytesTransferred) + "/" + bytes(e.TotalBytes.Value) + " Скорость скачивание: " + Properties.Settings.Default.targetSpeedInKb + "bytes";
                    break;
                case 1:
                    message = "Процесс установки: " + e.ProgressPercentage + "%";
                    break;
                case 2:
                    message = "Процесс установки: " + bytes(e.BytesTransferred) + "/" + bytes(e.TotalBytes.Value);
                    break;
                default:
                    message = "Процесс установки: " + e.ProgressPercentage + "%" + " Осталось: " + bytes(e.BytesTransferred) + "/" + bytes(e.TotalBytes.Value) + "Скорость скачивание: " + Properties.Settings.Default.targetSpeedInKb + "bytes";
                    break;
            }
            text.Dispatcher.Invoke(() => text.Text = message);
            progress.Dispatcher.Invoke(() => progress.Value = e.ProgressPercentage);
        }
    }
}
