using Newtonsoft.Json;
namespace LauncherLes1.View.Resources.Script
{
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