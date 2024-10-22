using Newtonsoft.Json;
namespace LauncherLes1.View.Resources.Script
{
    class ReadJsonFileClass
    {
        [JsonProperty("name")]
        string name { get; }

        [JsonProperty("version")]
        int version { get; }

        [JsonProperty("download")]
        string download { get; }

        [JsonProperty("backround")]
        string backround { get; }
    }
}
