using IWshRuntimeLibrary;
using System.Diagnostics;

namespace LauncherLes1.View.Resources.Script
{
    class CreateIconClass
    {
        public static string execPath = Process.GetCurrentProcess().MainModule.FileName;

        public static void CreateShortcut()
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Game.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "New shortcut for a Notepad";
            shortcut.Hotkey = "Ctrl+Shift+N";
            shortcut.TargetPath = execPath;
            shortcut.Save();
        }
    }
}
