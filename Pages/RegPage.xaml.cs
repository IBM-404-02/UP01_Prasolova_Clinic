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
using System.Text.RegularExpressions;

namespace UP01_Prasolova_Clinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
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

        private void TextBoxLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[A-Za-z0-9_]+$"))
                e.Handled = true;
        }

        private void TextBoxFIO_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[а-яА-ЯёЁ\-]+$"))
                e.Handled = true;
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text.Trim();
            string fio = TextBoxFIO.Text.Trim();
            string password = PasswordBoxNew.Password;
            string confirm = PasswordBoxConfirm.Password;

            if (login == "" || fio == "" || password == "" || confirm == "")
            {
                Core.ShowError("Заполните все поля");
                return;
            }

            if (!Regex.IsMatch(login, @"^[A-Za-z0-9_]{3,50}$"))
            {
                Core.ShowError("Логин должен содержать от 3 до 50 символов: английские буквы, цифры или _");
                return;
            }

            if (Core.Context.Users.Any(u => u.Login == login))
            {
                Core.ShowError("Пользователь с таким логином уже существует");
                return;
            }

            if (!Regex.IsMatch(fio, @"^[А-ЯЁ][а-яё]+( [А-ЯЁ][а-яё]+){1,2}$"))
            {
                Core.ShowError("Введите ФИО на русском языке: 2–3 слова, каждое с заглавной буквы");
                return;
            }

            string passwordError = Core.CheckPassword(password);
            if (passwordError != null)
            {
                Core.ShowError(passwordError);
                return;
            }

            if (password != confirm)
            {
                Core.ShowError("Пароли не совпадают");
                return;
            }

            string role = ((ComboBoxItem)ComboBoxRole.SelectedItem).Tag.ToString();

            User newUser = new User
            {
                Login = login,
                Password = Core.GetHash(password),
                Role = role,
                FIO = fio
            };

            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();

            Core.ShowInfo("Пользователь успешно зарегистрирован!");
            NavigationService.GoBack();
        }
    }


}
