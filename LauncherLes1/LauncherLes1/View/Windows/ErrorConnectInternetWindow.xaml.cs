using CheckConnectInternet;
using System.Windows;
using System.Windows.Input;

namespace LauncherLes1.View.Pages
{
    public partial class ErrorConnectInternetWindow : Window
    {
        public ErrorConnectInternetWindow()
        {
            InitializeComponent();

            MainWindow.isActiveerrorConnectInternetWindow = true;
        }

        private void Button_Click_Ignore(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow.isActiveerrorConnectInternetWindow = true;
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void Button_Click_Repeat(object sender, RoutedEventArgs e)
        {
            Internet.connect();
        }
    }
}
