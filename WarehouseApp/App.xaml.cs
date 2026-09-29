using System.Windows;
using WarehouseData.Models;
using WarehouseLibrary.Services;
using WarehouseApp.ViewModels;
using WarehouseApp.Views.Dialogs;
using WarehouseApp.Commands;
using WarehouseApp.Converters;

namespace WarehouseApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                var orgService = new OrganizationService();
                var warehouseService = new WarehouseService();
                var productService = new ProductService();
                var invoiceService = new InvoiceService();  // Добавьте эту строку

                var mainViewModel = new MainViewModel(
                    orgService,
                    warehouseService,
                    productService,
                    invoiceService);  // Добавьте параметр

                var mainWindow = new MainWindow { DataContext = mainViewModel };
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}