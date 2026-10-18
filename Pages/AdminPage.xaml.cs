using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Prasolova_Clinic.Pages
{
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

            var list = Core.Context.ServicesRendered.ToList();

            if (!string.IsNullOrWhiteSpace(TextBoxSearch.Text))
            {
                string search = TextBoxSearch.Text.ToLower();
                list = list.Where(s => s.ServiceName.ToLower().Contains(search)
                                    || s.User.FIO.ToLower().Contains(search)).ToList();
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

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGridServices.SelectedItems.Cast<ServiceRendered>().ToList();

            if (selected.Count == 0)
            {
                Core.ShowError("Выберите записи для удаления");
                return;
            }

            if (MessageBox.Show("Удалить выбранные записи (" + selected.Count + " шт.)?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            Core.Context.ServicesRendered.RemoveRange(selected);
            Core.Context.SaveChanges();

            Core.ShowInfo("Записи удалены");
            UpdateData();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditServicePage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            ServiceRendered selected = DataGridServices.SelectedItem as ServiceRendered;

            if (selected == null)
            {
                Core.ShowError("Выберите запись для редактирования");
                return;
            }

            NavigationService.Navigate(new AddEditServicePage(selected));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                UpdateData();
        }
    }
}