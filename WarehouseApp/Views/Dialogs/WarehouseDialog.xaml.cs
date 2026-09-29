using System.Windows;
using WarehouseData.Models;
using WarehouseLibrary.Services;
using WarehouseApp.ViewModels;
using WarehouseApp.Views.Dialogs;
using WarehouseApp.Commands;
using WarehouseApp.Converters;

namespace WarehouseApp.Views.Dialogs
{
    public partial class WarehouseDialog : Window
    {
        public string WarehouseName { get; private set; }
        public string WarehouseAddress { get; private set; }

        public WarehouseDialog(string title, string currentName = "", string currentAddress = "")
        {
            InitializeComponent();
            Title = title;
            NameTextBox.Text = currentName;
            AddressTextBox.Text = currentAddress;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Наименование склада не может быть пустым!",
                               "Ошибка валидации",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                return;
            }

            WarehouseName = NameTextBox.Text.Trim();
            WarehouseAddress = AddressTextBox.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}