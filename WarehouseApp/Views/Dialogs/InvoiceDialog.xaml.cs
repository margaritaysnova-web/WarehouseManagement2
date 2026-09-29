using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using WarehouseData.Models;
using WarehouseLibrary.Services;

namespace WarehouseApp.Views.Dialogs
{
    public partial class InvoiceDialog : Window, INotifyPropertyChanged
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IProductService _productService;
        private Invoice _invoice;
        private ObservableCollection<InvoiceItem> _items;

        public event PropertyChangedEventHandler PropertyChanged;

        public InvoiceDialog(
            IInvoiceService invoiceService,
            IProductService productService,
            string type,
            BigInteger warehouseId,
            string warehouseName,
            ObservableCollection<Product> availableProducts)
        {
            InitializeComponent();

            _invoiceService = invoiceService;
            _productService = productService;

            // Создаем новую накладную
            _invoice = _invoiceService.Create(type, warehouseId, warehouseName);

            // Заполняем информацию
            InvoiceNumberText.Text = _invoice.InvoiceNumber;
            InvoiceTypeText.Text = type == "In" ? "ПРИХОД" : "РАСХОД";
            StatusText.Text = _invoice.Status;
            TitleTextBlock.Text = type == "In" ? "Приходная накладная" : "Расходная накладная";

            // Загружаем доступные товары в выпадающий список
            ProductComboBox.ItemsSource = availableProducts;
            ProductComboBox.DisplayMemberPath = "Name";
            ProductComboBox.SelectedValuePath = "Article";

            // Привязываем список позиций
            _items = new ObservableCollection<InvoiceItem>(_invoice.Items);
            InvoiceItemsGrid.ItemsSource = _items;

            DataContext = this;
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductComboBox.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка для расходной накладной - хватает ли товара
            if (_invoice.InvoiceType == "Out" && quantity > selectedProduct.StockQuantity)
            {
                MessageBox.Show($"Недостаточно товара на складе! Доступно: {selectedProduct.StockQuantity}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _invoiceService.AddItem(
                _invoice.InvoiceId,
                selectedProduct.Article,
                selectedProduct.Name,
                quantity,
                selectedProduct.Price);

            // Обновляем список
            _items.Clear();
            foreach (var item in _invoice.Items)
            {
                _items.Add(item);
            }

            UpdateTotal();
        }

        private void UpdateTotal()
        {
            TotalText.Text = $"Итого: {_invoice.TotalAmount:N2} руб.";
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_invoice.Items.Any())
            {
                MessageBox.Show("Добавьте хотя бы один товар в накладную!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Вы уверены, что хотите провести накладную?\nПосле проведения изменения остатков будут применены.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool success = _invoiceService.ProcessInvoice(_invoice.InvoiceId, _productService);

                if (success)
                {
                    MessageBox.Show("Накладная успешно проведена! Остатки обновлены.",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при проведении накладной!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}