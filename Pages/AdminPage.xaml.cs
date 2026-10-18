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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>

        public partial class AdminPage : Page
        {
            public AdminPage()
            {
                InitializeComponent();

                var types = Core.Context.ServiceTypes.ToList();
                types.Insert(0, new ServiceType { ID = 0, Name = "Все типы" });
                ComboBoxType.ItemsSource = types;
                ComboBoxType.SelectedIndex = 0;

                UpdateData();
            }

            private void UpdateData()
            {
                if (!IsInitialized)
                    return;

                var list = Core.Context.ServicesRendered
                    .Where(s => s.UserID == Core.CurrentUser.ID)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(TextBoxSearch.Text))
                {
                    string search = TextBoxSearch.Text.ToLower();
                    list = list.Where(s => s.ServiceName.ToLower().Contains(search)).ToList();
                }

                if (ComboBoxType.SelectedItem is ServiceType type && type.ID != 0)
                    list = list.Where(s => s.ServiceTypeID == type.ID).ToList();

                switch (ComboBoxSort.SelectedIndex)
                {
                    case 0: list = list.OrderByDescending(s => s.Date).ToList(); break;
                    case 1: list = list.OrderBy(s => s.Date).ToList(); break;
                    case 2: list = list.OrderBy(s => s.Price).ToList(); break;
                    case 3: list = list.OrderByDescending(s => s.Price).ToList(); break;
                }

                DataGridServices.ItemsSource = list;
            }

            private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(TextBoxSearch.Text))
                    txtHintSearch.Visibility = Visibility.Visible;
                else
                    txtHintSearch.Visibility = Visibility.Hidden;

                UpdateData();
            }

            private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                UpdateData();
            }

            private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                UpdateData();
            }

            private void ButtonReset_Click(object sender, RoutedEventArgs e)
            {
                TextBoxSearch.Text = "";
                ComboBoxType.SelectedIndex = 0;
                ComboBoxSort.SelectedIndex = 0;
            }
        }
}
