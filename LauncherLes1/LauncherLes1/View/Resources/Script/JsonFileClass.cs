using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System;
using Newtonsoft.Json;
using System.Windows.Controls;

namespace LauncherLes1.View.Resources.Script
{
    public class UpdateContent {
        public static string urlDownload;
        public static string name;

        public static async Task Main(string urlJSON, Label version, Image urlBackground)
        {
            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(urlJSON);
            ReadJsonFileClass data = JsonConvert.DeserializeObject<ReadJsonFileClass>(json);

            version.Content = "Версия: " + data.version;
            urlDownload = data.download;
            name = data.name;
            urlBackground.Source = new BitmapImage(new Uri(data.backround));
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
}