using CommunityToolkit.Mvvm.Input;
using LauncherLes1.View;
using LauncherLes1.View.Pages;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace LauncherLes1.ViewModel
{
    internal class MainViewModel : ViewModedBase
    {
        private Page HomePage = new HomePage();
        private Page DefenderRat = new OpenDefenderRatPage();
        private Page TheWorldOfQuantrianism = new TheWorldOfQuantrianismPage();
        private Page Settings = new SettingsPage();

        private Page _CurPage;
        public Page CurPage
        {
            get => _CurPage;
            set => Set(ref _CurPage, value);
        }

        public MainViewModel()
            => CurPage = HomePage;

        #region Переход страниц
        public ICommand OpenDefenderRatPage
        {
            get
            {
                return new RelayCommand(() => CurPage = DefenderRat);
            }
        }

        public ICommand TheWorldOfQuantrianismPage
        {
            get
            {
                return new RelayCommand(() => CurPage = TheWorldOfQuantrianism);
            }
        }

        public ICommand openHomePage
        {
            get
            {
                return new RelayCommand(() => CurPage = HomePage);
            }
        }

        public ICommand openSettingsPage
        {
            get
            {
                return new RelayCommand(() => CurPage = Settings);
            }
        }

        public ICommand OpenDiscord
        {
            get
            {
                return new RelayCommand(() => Process.Start(new ProcessStartInfo("https://discord.gg/efEFJfEcXH") { UseShellExecute = true }));
            }
        }
        #endregion
    }
}
