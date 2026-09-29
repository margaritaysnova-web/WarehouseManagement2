using System.Windows;
using WarehouseData.Models;
using WarehouseLibrary.Services;
using WarehouseApp.ViewModels;
using WarehouseApp.Views.Dialogs;
using WarehouseApp.Commands;
using WarehouseApp.Converters;

namespace WarehouseApp.Views.Dialogs
{
    public partial class DeleteConfirmationDialog : Window
    {
        private readonly string _expectedName;

        public DeleteConfirmationDialog(string organizationName)
        {
            InitializeComponent();
            _expectedName = organizationName;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmationTextBox.Text.Trim() == _expectedName)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(
                    "Название организации не совпадает!",
                    "Ошибка подтверждения",
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