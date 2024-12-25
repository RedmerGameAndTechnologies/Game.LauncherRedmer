using Newtonsoft.Json;
namespace LauncherLes1.View.Resources.Script
{
    class ReadJsonFileClass
    {
        [JsonProperty("version")]
        int version { get; }

        [JsonProperty("download")]
        string download { get; }

        [JsonProperty("backround")]
        string? backround { get; }

        public ReadJsonFileClass(int version, string download, string? backround) {
            version = version;
            download = download;
            backround = backround;
        }
    }
}
