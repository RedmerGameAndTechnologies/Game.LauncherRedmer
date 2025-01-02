using IWshRuntimeLibrary;
using System.Diagnostics;

namespace LauncherLes1.View.Resources.Script
{
    class CreateIconClass
    {
        public static string execPath = Process.GetCurrentProcess().MainModule.FileName;

        public static void CreateShortcut(string name/*, string serchicon*/)
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + $@"\{name}.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
/*            if (!string.IsNullOrEmpty(serchicon))
            {
                shortcut.IconLocation = serchicon;
            }
            else
            {
                Console.WriteLine("Иконка не найдена.");
            }*/
            shortcut.TargetPath = execPath;
            shortcut.Save();
        }
    }
}