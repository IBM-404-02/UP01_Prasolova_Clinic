using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_Prasolova_Clinic.Pages
{
    public partial class AddEditServicePage : Page
    {
        private ServiceRendered current;

        public AddEditServicePage(ServiceRendered service)
        {
            InitializeComponent();
            current = service;

            ComboBoxPatient.ItemsSource = Core.Context.Users.Where(u => u.Role == "User").ToList();
            ComboBoxType.ItemsSource = Core.Context.ServiceTypes.ToList();
            DatePickerDate.DisplayDateEnd = DateTime.Today;

            if (current != null)
            {
                Title = "Редактирование услуги";
                ComboBoxPatient.SelectedItem = current.User;
                ComboBoxType.SelectedItem = current.ServiceType;
                DatePickerDate.SelectedDate = current.Date;
                TextBoxName.Text = current.ServiceName;
                TextBoxQuantity.Text = current.Quantity.ToString();
                TextBoxPrice.Text = current.Price.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                Title = "Добавление услуги";
                DatePickerDate.SelectedDate = DateTime.Today;
            }

            TextTitle.Text = Title;
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

        private void TextBoxQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[0-9]+$"))
                e.Handled = true;
        }

        private void TextBoxPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[0-9.,]+$"))
                e.Handled = true;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxPatient.SelectedItem == null || ComboBoxType.SelectedItem == null
                || DatePickerDate.SelectedDate == null || string.IsNullOrWhiteSpace(TextBoxName.Text)
                || TextBoxQuantity.Text == "" || TextBoxPrice.Text == "")
            {
                Core.ShowError("Заполните все поля");
                return;
            }

            if (!int.TryParse(TextBoxQuantity.Text, out int quantity) || quantity < 1 || quantity > 100)
            {
                Core.ShowError("Количество должно быть целым числом от 1 до 100");
                return;
            }

            string priceText = TextBoxPrice.Text.Replace(',', '.');
            if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price)
                || price < 1 || price > 1000000)
            {
                Core.ShowError("Цена должна быть числом от 1 до 1 000 000");
                return;
            }

            if (DatePickerDate.SelectedDate.Value > DateTime.Today)
            {
                Core.ShowError("Дата оказания услуги не может быть в будущем");
                return;
            }

            if (current == null)
            {
                current = new ServiceRendered();
                Core.Context.ServicesRendered.Add(current);
            }

            current.User = (User)ComboBoxPatient.SelectedItem;
            current.ServiceType = (ServiceType)ComboBoxType.SelectedItem;
            current.Date = DatePickerDate.SelectedDate.Value;
            current.ServiceName = TextBoxName.Text.Trim();
            current.Quantity = quantity;
            current.Price = price;

            Core.Context.SaveChanges();

            Core.ShowInfo("Данные сохранены");
            NavigationService.GoBack();
        }
    }
}