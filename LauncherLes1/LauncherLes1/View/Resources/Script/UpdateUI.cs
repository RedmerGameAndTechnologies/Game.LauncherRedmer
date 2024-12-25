using System;
using System.Windows.Threading;

namespace LauncherLes1.View.Resources.Script
{
    class UpdateUI
    {
        private static DispatcherTimer dispatcherTimer;

        public static void Update(EventHandler BackgroundUIFunction, int hours, int minutes, int seconds)
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(BackgroundUIFunction);
            dispatcherTimer.Interval = new TimeSpan(hours, minutes, seconds);
            dispatcherTimer.Start();
        }
    }
}
