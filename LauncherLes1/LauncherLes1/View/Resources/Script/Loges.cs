using System;
using System.IO;

namespace LauncherLes1.View.Resources.Script
{
    class Loges
    {
        public static void ExcepctionEventApp(object sender, UnhandledExceptionEventArgs ueea)
        {
            Exception e = (Exception)ueea.ExceptionObject;
            LoggingProcess("EXCEPTION" + e.Message.ToString());
        }

        public static void LoggingProcess(string s)
        {
            if (Directory.Exists(@"Log/") == false)
                Directory.CreateDirectory(@"Log/");
            DateTime now = DateTime.Now;
            string todayTimeLog = now.ToString("mm_HH_dd_MM_yyyy");
            string nameFileLog = @"Log/" + "Log" + todayTimeLog + ".txt";

            using (StreamWriter sw = new StreamWriter(nameFileLog, false))
                sw.WriteLineAsync(s);
        }
    }
}
