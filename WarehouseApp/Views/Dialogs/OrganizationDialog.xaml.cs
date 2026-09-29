using System.Windows;
using WarehouseData.Models;
using WarehouseLibrary.Services;
using WarehouseApp.ViewModels;
using WarehouseApp.Views.Dialogs;
using WarehouseApp.Commands;
using WarehouseApp.Converters;

namespace WarehouseApp.Views.Dialogs
{
    public partial class OrganizationDialog : Window
    {
        public string OrganizationName { get; private set; }

        public OrganizationDialog(string title, string currentName = "")
        {
            InitializeComponent();
            Title = title;
            NameTextBox.Text = currentName;
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(OrganizationName);

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем текст напрямую из TextBox
            OrganizationName = NameTextBox.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(OrganizationName))
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Наименование не может быть пустым!",
                               "Ошибка валидации",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}