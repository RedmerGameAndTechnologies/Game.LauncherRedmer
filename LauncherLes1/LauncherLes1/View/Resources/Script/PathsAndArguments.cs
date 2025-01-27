using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LauncherLes1.View.Resources.Script
{
    class Arguments
    {
        #region Confirm Update
        public static string execPath = Process.GetCurrentProcess().MainModule.FileName;
        public static string workingDir = Path.GetDirectoryName(execPath);
        public static string sourcePath = Path.Combine(workingDir, Path.GetFileName(execPath));
        public static string exenames = Path.GetFileName(sourcePath);
        #endregion

        #region Settings
        public static string curver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        #endregion
    }

    class Paths
    {
        public string Name { get; set; }

        #region Download Game
        public readonly string zipPath;
        public Paths(string name)
        {
            Name = name;
            zipPath = $@".\ChacheDownloadGame_{Name}.zip";
        }
        public static readonly string appTemlPath = "tempDirectoryUnzip";
        #endregion

        #region Confirm Update
        public static readonly string zipPathUpdate = @".\UpdateLaucnher.zip";
        public static readonly string exeLauncherUpdate = @".\NewLauncher.exe";
        public static readonly string exetraPath = @".\";
        #endregion
    }
}
