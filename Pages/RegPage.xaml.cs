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
    }


}
