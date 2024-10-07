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
