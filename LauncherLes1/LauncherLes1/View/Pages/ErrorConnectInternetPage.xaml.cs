using System;
using CheckConnectInternet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LauncherLes1.View.Pages
{
    public partial class ErrorConnectInternetPage : Window
    {
        public ErrorConnectInternetPage()
        {
            InitializeComponent();
        }

        private void WindowMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); // this
            }
        }

        private void Button_Click_Ignore(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow.isActiveErrorConnectInternetPage = true;
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void Button_Click_Repeat(object sender, RoutedEventArgs e)
        {
            Internet.connect();
        }
    }
}
