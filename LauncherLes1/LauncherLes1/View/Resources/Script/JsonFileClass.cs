using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System;
using Newtonsoft.Json;
using System.Windows.Controls;

namespace LauncherLes1.View.Resources.Script
{
    public class UpdateContent {
        public static async Task Main(string urlJSON, Label version, string urlDownload, Image urlBackground)
        {
            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(urlJSON);
            ReadJsonFileClass data = JsonConvert.DeserializeObject<ReadJsonFileClass>(json);

            version.Content = data.version;
            urlDownload = data.download;
            urlBackground.Source = new BitmapImage(new Uri(data.backround));
        }
    }

    public class ReadJsonFileClass
    {
        [JsonProperty("version")]
        public string version { get; set; }

        [JsonProperty("download")]
        public string download { get; set; }

        [JsonProperty("backround")]
        public string backround { get; set; }
    }
}