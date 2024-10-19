using System.Diagnostics;

namespace LauncherLes1.View.Resources.Script
{
    class CmdClass
    {
        public static void Cmd(string line)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c {line}",
                WindowStyle = ProcessWindowStyle.Hidden,
            });
        }
    }
}
