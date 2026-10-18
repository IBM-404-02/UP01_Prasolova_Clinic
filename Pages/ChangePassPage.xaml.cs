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
    /// Логика взаимодействия для ChangePassPage.xaml
    /// </summary>
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            Grid grid = (Grid)box.Parent;

            if (string.IsNullOrEmpty(box.Text))
                grid.Children[1].Visibility = Visibility.Visible;
            else
                grid.Children[1].Visibility = Visibility.Hidden;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = (PasswordBox)sender;
            Grid grid = (Grid)box.Parent;

            if (string.IsNullOrEmpty(box.Password))
                grid.Children[1].Visibility = Visibility.Visible;
            else
                grid.Children[1].Visibility = Visibility.Hidden;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text.Trim();
            string current = PasswordBoxCurrent.Password;
            string newPassword = PasswordBoxNew.Password;
            string confirm = PasswordBoxConfirm.Password;

            if (login == "" || current == "" || newPassword == "" || confirm == "")
            {
                Core.ShowError("Заполните все поля");
                return;
            }

            User user = Core.Authenticate(login, current);
            if (user == null)
            {
                Core.ShowError("Неверный логин или текущий пароль");
                return;
            }

            string passwordError = Core.CheckPassword(newPassword);
            if (passwordError != null)
            {
                Core.ShowError(passwordError);
                return;
            }

            if (newPassword != confirm)
            {
                Core.ShowError("Новые пароли не совпадают");
                return;
            }

            if (newPassword == current)
            {
                Core.ShowError("Новый пароль должен отличаться от текущего");
                return;
            }

            user.Password = Core.GetHash(newPassword);
            Core.Context.SaveChanges();

            Core.ShowInfo("Пароль успешно изменён!");
            NavigationService.GoBack();
        }
    }
}
