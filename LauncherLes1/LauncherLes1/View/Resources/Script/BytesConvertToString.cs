using System;

namespace LauncherLes1.View.Resources.Script
{
    class BytesConvertToString
    {
        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB" };
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(bytes) * num).ToString() + suf[place];
        }
    }
}
