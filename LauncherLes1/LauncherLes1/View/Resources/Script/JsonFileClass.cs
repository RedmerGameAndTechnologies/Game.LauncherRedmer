using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System;
using Newtonsoft.Json;
using System.Windows.Controls;

namespace LauncherLes1.View.Resources.Script
{
    public class UpdateContent {
        public string name;
        public string urlDownload;
        public string appGamePath;

        public async Task Main(string urlJSON, Label version, Image urlBackground)
        {
            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(urlJSON);
            ReadJsonFileClass data = JsonConvert.DeserializeObject<ReadJsonFileClass>(json);

            version.Content = "Версия: " + data.version;
            urlDownload = data.download;
            name = data.name;
            appGamePath = $@"{name}/";
            urlBackground.Source = new BitmapImage(new Uri(data.backround));
        }
    }

    public class UpdateContentLauncherUpdate
    {
        public string version;
        public string fileDownloadLink;

        public async Task Main(string urlJSON)
        {
            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(urlJSON);
            ReadJsonFileClassUpdateLauncher data = JsonConvert.DeserializeObject<ReadJsonFileClassUpdateLauncher>(json);

            fileDownloadLink = data.fileDownloadLink;
            version = data.version;
        }
    }

    public class ReadJsonFileClass
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("version")]
        public string version { get; set; }

        [JsonProperty("download")]
        public string download { get; set; }

        [JsonProperty("backround")]
        public string backround { get; set; }
    }

    public class ReadJsonFileClassUpdateLauncher
    {
        [JsonProperty("version")]
        public string version { get; set; }

        [JsonProperty("fileDownloadLink")]
        public string fileDownloadLink { get; set; }
    }
}