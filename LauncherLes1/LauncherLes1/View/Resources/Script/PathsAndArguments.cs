namespace LauncherLes1.View.Resources.Script
{
    class Arguments
    {
        public static int? idProcessApp = null;
        public static bool appIsStarting = false;
        public static bool isStartUnzipUpdateFileApp = true;
        public static bool isFileDownloadingNow = false;
        public static string ArgumentsAppString { get; set; }
    }

    class Paths
    {
        public static readonly string zipPath = @".\ChacheDownloadGame.zip";
        public static readonly string appTemlPath = "tempDirectoryUnzip";
        public static readonly string appGamePath = $@"{UpdateContent.name}/";
    }
}
