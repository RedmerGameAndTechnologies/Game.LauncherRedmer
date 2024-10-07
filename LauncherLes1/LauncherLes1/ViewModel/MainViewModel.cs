using CommunityToolkit.Mvvm.Input;
using LauncherLes1.View;
using LauncherLes1.View.Pages;
using MvvmCross.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace LauncherLes1.ViewModel
{
    internal class MainViewModel : ViewModedBase
    {
        private Page HomePage = new HomePage();
        private Page DefenderRat = new OpenDefenderRatPage();
        private Page HS = new HsPage();
        private Page OW = new OverwatchPage();
        private Page D3 = new D3();
        private Page HOTS = new HOTS();
        private Page SC = new SC();
        private Page Settings = new SettingsPage();

        private Page _CurPage;
        public Page CurPage
        {
            get => _CurPage;
            set => Set(ref _CurPage, value);
        }

        public MainViewModel()
        {
            CurPage = HomePage;
        }

        #region Переход страниц
        public ICommand OpenHSPage
        {
            get
            {
                return new RelayCommand(() => CurPage = HS);

            }
        }
        public ICommand OpenDefenderRatPage
        {
            get
            {
                return new RelayCommand(() => CurPage = DefenderRat);
            }
        }
        public ICommand OpenOWPage
        {
            get
            {
                return new RelayCommand(() => CurPage = OW);
            }
        }
        public ICommand OpenD3Page
        {
            get
            {
                return new RelayCommand(() => CurPage = D3);
            }
        }
        public ICommand OpenHOTSPage
        {
            get
            {
                return new RelayCommand(() => CurPage = HOTS);
            }
        }
        public ICommand OpenSCPage
        {
            get
            {
                return new RelayCommand(() => CurPage = SC);
            }
        }

        public ICommand openSettingsPage
        {
            get
            {
                return new RelayCommand(() => CurPage = Settings);
            }
        }

        public ICommand openHomePage
        {
            get
            {
                return new RelayCommand(() => CurPage = HomePage);
            }
        }
        #endregion
    }
}
