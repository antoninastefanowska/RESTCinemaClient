using CinemaClient.Client;
using CinemaClient.Client.Model;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CinemaClient
{
    public partial class LoginWindow : Window
    {
        private readonly CinemaServiceClient service;

        public LoginWindow()
        {
            service = new CinemaServiceClient();
            InitializeComponent();
        }

        private Authorization CreateAuthentication()
        {
            string username = UsernameTextbox.Text;
            string password = Encrypt(PasswordTextbox.Password.ToString());
            Authorization authorization = new Authorization(username, password);
            return authorization;
        }

        private string Encrypt(string input)
        {
            byte[] data = Encoding.ASCII.GetBytes(input);
            byte[] hashData = new SHA1Managed().ComputeHash(data);
            string hash = string.Empty;

            foreach (byte b in hashData)
                hash += b.ToString("X2");
            return hash;
        }

        private void StartLoading()
        {
            LoginButton.IsEnabled = false;
            CreateAccountButton.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;
        }

        private void StopLoading()
        {
            LoginButton.IsEnabled = true;
            CreateAccountButton.IsEnabled = true;
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Authorization authorization = CreateAuthentication();
            Login(authorization);
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Authorization authorization = CreateAuthentication();
            CreateAccount(authorization);
        }

        private async void Login(Authorization authorization)
        {
            StartLoading();
            try
            {
                await service.LoginAsync(authorization);
                SwitchWindows(authorization);
            }
            catch (UnsuccessfulResponseException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }

        private async void CreateAccount(Authorization authorization)
        {
            StartLoading();
            try
            {
                await service.CreateUserAsync(authorization);
                Utils.ShowInfo(this, "Konto zostało utworzone.");
                SwitchWindows(authorization);
            }
            catch (UnsuccessfulResponseException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }

        private void SwitchWindows(Authorization authorization)
        {
            MainWindow newWindow = new MainWindow();
            newWindow.Authorization = authorization;
            newWindow.Service = service;
            Application.Current.MainWindow = newWindow;
            Close();
            newWindow.Show();
        }
    }
}
