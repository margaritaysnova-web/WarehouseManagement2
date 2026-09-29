using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using WarehouseData.Models;
using WarehouseLibrary.Services;
using WarehouseApp.Commands;
using WarehouseApp.Views.Dialogs;
using Microsoft.Win32;
using WarehouseApp.ViewModels;
using WarehouseApp.Converters;
using WarehouseLibrary.Services;

namespace WarehouseApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IOrganizationService _organizationService;
        private readonly IWarehouseService _warehouseService;
        private readonly IProductService _productService;

        private Organization _selectedOrganization;
        private Warehouse _selectedWarehouse;
        private Product _selectedProduct;
        private readonly IInvoiceService _invoiceService;

        public ObservableCollection<Organization> Organizations { get; set; }
        public ObservableCollection<Warehouse> Warehouses { get; set; }
        public ObservableCollection<Product> Products { get; set; }


        public Organization SelectedOrganization
        {
            get => _selectedOrganization;
            set
            {
                _selectedOrganization = value;
                OnPropertyChanged();
                LoadWarehouses();
                UpdateContextInfo();
            }
        }

        public Warehouse SelectedWarehouse
        {
            get => _selectedWarehouse;
            set
            {
                _selectedWarehouse = value;
                OnPropertyChanged();
                LoadProducts();
                UpdateContextInfo();
            }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
                // Уведомляем команды, что возможность выполнения изменилась
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Свойства контекста для отображения
        public string ContextOrganization => SelectedOrganization?.OrgName ?? "Не выбрана";
        public string ContextWarehouse => SelectedWarehouse?.WhName ?? "Не выбран";

        public bool IsOrganizationSelected => SelectedOrganization != null;
        public bool IsWarehouseSelected => SelectedWarehouse != null;

        // Команды для организаций
        public ICommand AddOrganizationCommand { get; }
        public ICommand EditOrganizationCommand { get; }
        public ICommand DeleteOrganizationCommand { get; }

        // Команды для складов
        public ICommand AddWarehouseCommand { get; }
        public ICommand EditWarehouseCommand { get; }
        public ICommand DeleteWarehouseCommand { get; }

        // Команды для товаров
        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ImportCsvCommand { get; }

        public ICommand AddIncomeInvoiceCommand { get; }
        public ICommand AddOutcomeInvoiceCommand { get; }

        public MainViewModel(
            IOrganizationService organizationService,
            IWarehouseService warehouseService,
            IProductService productService,
            IInvoiceService invoiceService)
        {
            _organizationService = organizationService;
            _warehouseService = warehouseService;
            _productService = productService;
            _invoiceService = invoiceService;

            Organizations = new ObservableCollection<Organization>();
            Warehouses = new ObservableCollection<Warehouse>();
            Products = new ObservableCollection<Product>();

            // Инициализация команд
            AddOrganizationCommand = new RelayCommand(AddOrganization);
            EditOrganizationCommand = new RelayCommand(EditOrganization, () => SelectedOrganization != null);
            DeleteOrganizationCommand = new RelayCommand(DeleteOrganization, () => SelectedOrganization != null);

            AddWarehouseCommand = new RelayCommand(AddWarehouse, () => SelectedOrganization != null);
            EditWarehouseCommand = new RelayCommand(EditWarehouse, () => SelectedWarehouse != null);
            DeleteWarehouseCommand = new RelayCommand(DeleteWarehouse, () => SelectedWarehouse != null);

            AddProductCommand = new RelayCommand(AddProduct, () => SelectedWarehouse != null);
            EditProductCommand = new RelayCommand(EditProduct, () => SelectedProduct != null);
            DeleteProductCommand = new RelayCommand(DeleteProduct, () => SelectedProduct != null);
            ImportCsvCommand = new RelayCommand(ImportCsv, () => SelectedWarehouse != null);

            AddIncomeInvoiceCommand = new RelayCommand(AddIncomeInvoice, () => SelectedWarehouse != null);
            AddOutcomeInvoiceCommand = new RelayCommand(AddOutcomeInvoice, () => SelectedWarehouse != null);

            LoadData();
        }

        private void LoadData()
        {
            _organizationService.SeedTestData();
            var organizations = _organizationService.GetAll();

            _warehouseService.SeedTestData(organizations);

            var allWarehouses = _warehouseService.GetAll();
            _productService.SeedTestData(allWarehouses);

            Organizations.Clear();
            foreach (var org in organizations)
            {
                Organizations.Add(org);
            }
        }

        private void LoadWarehouses()
        {
            Warehouses.Clear();
            Products.Clear();

            if (SelectedOrganization != null)
            {
                var warehouses = _warehouseService.GetByOrganizationId(SelectedOrganization.OrgId);
                foreach (var wh in warehouses)
                {
                    Warehouses.Add(wh);
                }
            }

            OnPropertyChanged(nameof(IsOrganizationSelected));
        }

        private void LoadProducts()
        {
            Products.Clear();

            if (SelectedWarehouse != null)
            {
                var products = _productService.GetByWarehouseId(SelectedWarehouse.WhId);
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }

            OnPropertyChanged(nameof(IsWarehouseSelected));
        }

        private void UpdateContextInfo()
        {
            OnPropertyChanged(nameof(ContextOrganization));
            OnPropertyChanged(nameof(ContextWarehouse));
        }

        // Методы CRUD для организаций
        private void AddOrganization()
        {
            var dialog = new OrganizationDialog("Добавление организации");
            if (dialog.ShowDialog() == true)
            {
                var org = _organizationService.Create(dialog.OrganizationName);
                Organizations.Add(org);
            }
        }

        private void EditOrganization()
        {
            if (SelectedOrganization == null) return;

            var dialog = new OrganizationDialog(
                "Редактирование организации",
                SelectedOrganization.OrgName);

            if (dialog.ShowDialog() == true)
            {
                _organizationService.Update(
                    SelectedOrganization.OrgId,
                    dialog.OrganizationName);

                // Полностью обновляем список
                RefreshOrganizationsList();
                UpdateContextInfo();
            }
        }

        private void DeleteOrganization()
        {
            if (SelectedOrganization == null) return;

            var dialog = new DeleteConfirmationDialog(SelectedOrganization.OrgName);
            if (dialog.ShowDialog() == true)
            {
                _organizationService.Delete(SelectedOrganization.OrgId);
                Organizations.Remove(SelectedOrganization);

                // Очищаем связанные данные
                Warehouses.Clear();
                Products.Clear();
            }
        }

        // Методы CRUD для складов
        private void AddWarehouse()
        {
            var dialog = new WarehouseDialog("Добавление склада");
            if (dialog.ShowDialog() == true)
            {
                var warehouse = _warehouseService.Create(
                    dialog.WarehouseName,
                    dialog.WarehouseAddress,
                    SelectedOrganization.OrgId);
                Warehouses.Add(warehouse);
            }
        }

        private void EditWarehouse()
        {
            if (SelectedWarehouse == null) return;

            var dialog = new WarehouseDialog(
                "Редактирование склада",
                SelectedWarehouse.WhName,
                SelectedWarehouse.WhAddress);

            if (dialog.ShowDialog() == true)
            {
                _warehouseService.Update(
                    SelectedWarehouse.WhId,
                    dialog.WarehouseName,
                    dialog.WarehouseAddress);

                // Полностью обновляем список складов
                RefreshWarehousesList();
                UpdateContextInfo();
            }
        }

        private void DeleteWarehouse()
        {
            if (SelectedWarehouse == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить склад '{SelectedWarehouse.WhName}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _warehouseService.Delete(SelectedWarehouse.WhId);
                Warehouses.Remove(SelectedWarehouse);
                Products.Clear();
            }
        }

        // Методы CRUD для товаров
        private void AddProduct()
        {
            var dialog = new ProductDialog("Добавление товара");
            if (dialog.ShowDialog() == true)
            {
                var product = new Product
                {
                    Name = dialog.ProductName,
                    Price = dialog.Price,
                    StockQuantity = dialog.Quantity,
                    DiscountPercent = dialog.Discount,
                    Unit = dialog.Unit,
                    Description = dialog.Description,
                    PhotoPath = dialog.PhotoPath,
                    Category = new Category { Id = 1, Name = dialog.Category },
                    Manufacturer = new Manufacturer { Id = 1, Name = dialog.Manufacturer },
                    Supplier = new Supplier { Id = 1, Name = dialog.Supplier },
                    SupplierName = dialog.Supplier
                };

                var created = _productService.Create(product, SelectedWarehouse.WhId);
                Products.Add(created);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void EditProduct()
        {
            if (SelectedProduct == null) return;

            var dialog = new ProductDialog(
                "Редактирование товара",
                SelectedProduct.Name,
                SelectedProduct.Category?.Name ?? "",
                SelectedProduct.Manufacturer?.Name ?? "",
                SelectedProduct.Supplier?.Name ?? SelectedProduct.SupplierName,
                SelectedProduct.Price,
                SelectedProduct.StockQuantity,
                SelectedProduct.DiscountPercent,
                SelectedProduct.Unit,
                SelectedProduct.Description ?? "",
                SelectedProduct.PhotoPath ?? "");

            if (dialog.ShowDialog() == true)
            {
                var updatedProduct = new Product
                {
                    Name = dialog.ProductName,
                    Price = dialog.Price,
                    StockQuantity = dialog.Quantity,
                    DiscountPercent = dialog.Discount,
                    Unit = dialog.Unit,
                    Description = dialog.Description,
                    PhotoPath = dialog.PhotoPath,
                    Category = new Category { Id = 1, Name = dialog.Category },
                    Manufacturer = new Manufacturer { Id = 1, Name = dialog.Manufacturer },
                    Supplier = new Supplier { Id = 1, Name = dialog.Supplier },
                    SupplierName = dialog.Supplier
                };

                _productService.Update(SelectedProduct.Article, updatedProduct);
                RefreshProductsList();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить товар '{SelectedProduct.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _productService.Delete(SelectedProduct.Article);
                Products.Remove(SelectedProduct);
            }
        }

        private void ImportCsv()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Выберите файл для импорта"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string csvContent = System.IO.File.ReadAllText(openFileDialog.FileName);
                    _productService.ImportFromCsv(csvContent, SelectedWarehouse.WhId);
                    LoadProducts(); // Обновляем список

                    MessageBox.Show(
                        "Импорт успешно завершен!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при импорте: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
        private void RefreshOrganizationsList()
        {
            var selectedId = SelectedOrganization?.OrgId;

            Organizations.Clear();
            foreach (var org in _organizationService.GetAll())
            {
                Organizations.Add(org);
            }

            if (selectedId.HasValue)
            {
                SelectedOrganization = Organizations.FirstOrDefault(o => o.OrgId == selectedId.Value);
            }

            OnPropertyChanged(nameof(Organizations));
        }
        private void RefreshWarehousesList()
        {
            if (SelectedOrganization == null) return;

            var selectedId = SelectedWarehouse?.WhId;

            Warehouses.Clear();
            var warehouses = _warehouseService.GetByOrganizationId(SelectedOrganization.OrgId);
            foreach (var wh in warehouses)
            {
                Warehouses.Add(wh);
            }

            if (selectedId.HasValue)
            {
                SelectedWarehouse = Warehouses.FirstOrDefault(w => w.WhId == selectedId.Value);
            }

            OnPropertyChanged(nameof(Warehouses));
        }

        private void RefreshProductsList()
        {
            if (SelectedWarehouse == null) return;

            var selectedArticle = SelectedProduct?.Article;

            Products.Clear();
            var products = _productService.GetByWarehouseId(SelectedWarehouse.WhId);
            foreach (var product in products)
            {
                Products.Add(product);
            }

            if (!string.IsNullOrEmpty(selectedArticle))
            {
                SelectedProduct = Products.FirstOrDefault(p => p.Article == selectedArticle);
            }

            OnPropertyChanged(nameof(Products));
        }

        private void AddIncomeInvoice()
        {
            if (SelectedWarehouse == null) return;

            var dialog = new InvoiceDialog(
                _invoiceService,
                _productService,
                "In",
                SelectedWarehouse.WhId,
                SelectedWarehouse.WhName,
                Products);

            if (dialog.ShowDialog() == true)
            {
                // Обновляем список товаров после проведения накладной
                RefreshProductsList();
            }
        }

        private void AddOutcomeInvoice()
        {
            if (SelectedWarehouse == null) return;

            var dialog = new InvoiceDialog(
                _invoiceService,
                _productService,
                "Out",
                SelectedWarehouse.WhId,
                SelectedWarehouse.WhName,
                Products);

            if (dialog.ShowDialog() == true)
            {
                // Обновляем список товаров после проведения накладной
                RefreshProductsList();
            }
        }
    }
}