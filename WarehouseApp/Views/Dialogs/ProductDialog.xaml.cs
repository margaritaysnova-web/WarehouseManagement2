using System.Windows;
using Microsoft.Win32;

namespace WarehouseApp.Views.Dialogs
{
    public partial class ProductDialog : Window
    {
        public string ProductName { get; private set; }
        public string Category { get; private set; }
        public string Manufacturer { get; private set; }
        public string Supplier { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public decimal Discount { get; private set; }
        public string Unit { get; private set; }
        public string Description { get; private set; }
        public string PhotoPath { get; private set; }

        public ProductDialog(string title, string name = "", string category = "",
                            string manufacturer = "", string supplier = "",
                            decimal price = 0, int quantity = 0,
                            decimal discount = 0, string unit = "шт.",
                            string description = "", string photoPath = "")
        {
            InitializeComponent();
            Title = title;

            // Заполняем поля
            NameTextBox.Text = name;
            CategoryTextBox.Text = category;
            ManufacturerTextBox.Text = manufacturer;
            SupplierTextBox.Text = supplier;
            PriceTextBox.Text = price > 0 ? price.ToString() : "";
            QuantityTextBox.Text = quantity > 0 ? quantity.ToString() : "";
            DiscountTextBox.Text = discount > 0 ? discount.ToString() : "0";
            UnitTextBox.Text = unit;
            DescriptionTextBox.Text = description;
            PhotoPathTextBox.Text = photoPath;
            PhotoPath = photoPath;
        }

        private void BrowsePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображение товара",
                Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PhotoPathTextBox.Text = openFileDialog.FileName;
                PhotoPath = openFileDialog.FileName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем и проверяем название
            ProductName = NameTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                MessageBox.Show("Наименование товара не может быть пустым!",
                               "Ошибка валидации",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                return;
            }

            // Получаем категорию
            Category = CategoryTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(Category))
            {
                Category = "Без категории";
            }

            // Получаем производителя
            Manufacturer = ManufacturerTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(Manufacturer))
            {
                Manufacturer = "Не указан";
            }

            // Получаем поставщика
            Supplier = SupplierTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(Supplier))
            {
                Supplier = "Не указан";
            }

            // Проверяем цену
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Цена должна быть числом!",
                               "Ошибка валидации",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                return;
            }
            Price = price;

            // Проверяем количество
            if (!int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                MessageBox.Show("Количество должно быть целым числом!",
                               "Ошибка валидации",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                return;
            }
            Quantity = quantity;

            // Получаем скидку
            Discount = 0;
            if (!string.IsNullOrWhiteSpace(DiscountTextBox.Text))
            {
                if (!decimal.TryParse(DiscountTextBox.Text, out decimal discount))
                {
                    MessageBox.Show("Скидка должна быть числом!",
                                   "Ошибка валидации",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                    return;
                }
                Discount = discount;
            }

            Unit = UnitTextBox.Text?.Trim() ?? "шт.";
            Description = DescriptionTextBox.Text?.Trim();
            PhotoPath = PhotoPathTextBox.Text?.Trim();

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