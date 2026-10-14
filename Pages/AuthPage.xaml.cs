using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UP01_Prasolova_Clinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;
        public AuthPage()
        {
            InitializeComponent();
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                Core.ShowError("Введите логин и пароль");
                return;
            }

            User user = Core.Authenticate(TextBoxLogin.Text.Trim(), PasswordBox.Password);
            if (user == null)
            {
                failedAttempts++;
                Core.ShowError("Неверный логин или пароль");
                return;
            }

            failedAttempts = 0;
            Core.CurrentUser = user;
            Core.ShowInfo("Добро пожаловать, " + user.FIO + "!");

            if (user.Role == "Admin")
                NavigationService.Navigate(new AdminPage());
            else
                NavigationService.Navigate(new UserPage());
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangePassPage());
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text))
                txtHintLogin.Visibility = Visibility.Visible;
            else
                txtHintLogin.Visibility = Visibility.Hidden;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
                txtHintPass.Visibility = Visibility.Visible;
            else
                txtHintPass.Visibility = Visibility.Hidden;
        }
    }

}
