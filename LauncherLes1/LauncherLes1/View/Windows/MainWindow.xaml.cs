using CheckConnectInternet;
using LauncherLes1.View.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LauncherLes1.View
{
    public partial class MainWindow : Window
    {
        public static bool isActiveErrorConnectInternetPage { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            CheckConnectInternet();
        }

        private void CheckConnectInternet()
        {
            if (isActiveErrorConnectInternetPage == false) {
                if (Internet.connect() ==  false)
                {
                    this.Hide();
                    ErrorConnectInternetPage errorConnectInternetPage = new ErrorConnectInternetPage();
                    errorConnectInternetPage.Show();
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Home(object sender, RoutedEventArgs e)
        {
            SlideMenu.Visibility = Visibility.Hidden;
        }

        private void Discord_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/efEFJfEcXH") { UseShellExecute = true });
        }
    }
}
