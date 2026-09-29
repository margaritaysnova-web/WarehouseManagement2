using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WarehouseData.Models;

namespace WarehouseLibrary.Services
{
    public class ProductService : IProductService
    {
        private List<Product> _products = new List<Product>();
        private static int _articleCounter = 1;

        public void SeedTestData(List<Warehouse> warehouses)
        {
            if (!_products.Any() && warehouses.Any())
            {
                // Добавляем товары для каждого существующего склада
                foreach (var warehouse in warehouses)
                {
                    var categoryTools = new Category { Id = 1, Name = "Инструменты" };
                    var categoryFasteners = new Category { Id = 2, Name = "Крепеж" };

                    var manufacturer1 = new Manufacturer { Id = 1, Name = "ЗАО Инструмент" };
                    var manufacturer2 = new Manufacturer { Id = 2, Name = "ООО Крепеж" };

                    var supplier1 = new Supplier { Id = 1, Name = "ООО Поставка" };
                    var supplier2 = new Supplier { Id = 2, Name = "ИП Снабжение" };

                    _products.Add(new Product
                    {
                        Article = $"ART-{_articleCounter++:D6}",
                        Name = "Молоток строительный",
                        Unit = "шт.",
                        Price = 500,
                        StockQuantity = 100,
                        DiscountPercent = 5,
                        Category = categoryTools,
                        CategoryId = categoryTools.Id,
                        Manufacturer = manufacturer1,
                        ManufacturerId = manufacturer1.Id,
                        Supplier = supplier1,
                        SupplierId = supplier1.Id,
                        WarehouseId = warehouse.WhId,
                        Description = "Молоток для строительных работ"
                    });

                    _products.Add(new Product
                    {
                        Article = $"ART-{_articleCounter++:D6}",
                        Name = "Гвозди 100мм",
                        Unit = "кг",
                        Price = 150,
                        StockQuantity = 1000,
                        DiscountPercent = 0,
                        Category = categoryFasteners,
                        CategoryId = categoryFasteners.Id,
                        Manufacturer = manufacturer2,
                        ManufacturerId = manufacturer2.Id,
                        Supplier = supplier2,
                        SupplierId = supplier2.Id,
                        WarehouseId = warehouse.WhId,
                        Description = "Гвозди строительные 100мм"
                    });
                }
            }
        }

        public List<Product> GetByWarehouseId(BigInteger warehouseId)
        {
            return _products.Where(p => p.WarehouseId == warehouseId).ToList();
        }

        public Product GetByArticle(string article)
        {
            return _products.FirstOrDefault(p => p.Article == article);
        }

        public Product Create(Product product, BigInteger warehouseId)
        {
            if (string.IsNullOrEmpty(product.Article))
            {
                product.Article = $"ART-{_articleCounter++:D6}";
            }
            product.WarehouseId = warehouseId;
            _products.Add(product);
            return product;
        }

        public Product Update(string article, Product updatedProduct)
        {
            var existing = GetByArticle(article);
            if (existing != null)
            {
                existing.Name = updatedProduct.Name;
                existing.Price = updatedProduct.Price;
                existing.StockQuantity = updatedProduct.StockQuantity;
                existing.DiscountPercent = updatedProduct.DiscountPercent;
                existing.Unit = updatedProduct.Unit;
                existing.Description = updatedProduct.Description;
                existing.PhotoPath = updatedProduct.PhotoPath;
                existing.CategoryId = updatedProduct.CategoryId;
                existing.ManufacturerId = updatedProduct.ManufacturerId;
                existing.SupplierId = updatedProduct.SupplierId;
                existing.Category = updatedProduct.Category;
                existing.Manufacturer = updatedProduct.Manufacturer;
                existing.Supplier = updatedProduct.Supplier;
            }
            return existing;
        }

        public bool Delete(string article)
        {
            var product = GetByArticle(article);
            if (product != null)
            {
                _products.Remove(product);
                return true;
            }
            return false;
        }

        public void UpdateStock(string article, int quantityChange)
        {
            var product = GetByArticle(article);
            if (product != null)
            {
                product.StockQuantity += quantityChange;
                if (product.StockQuantity < 0)
                    product.StockQuantity = 0;
            }
        }

        public void ImportFromCsv(string csvContent, BigInteger warehouseId)
        {
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length >= 5)
                {
                    var product = new Product
                    {
                        Article = $"ART-{_articleCounter++:D6}",
                        Name = parts[0].Trim(),
                        Category = new Category { Name = parts[1].Trim() },
                        Manufacturer = new Manufacturer { Name = parts[2].Trim() },
                        Price = decimal.TryParse(parts[3], out decimal price) ? price : 0,
                        StockQuantity = int.TryParse(parts[4], out int qty) ? qty : 0,
                        Unit = "шт.",
                        WarehouseId = warehouseId,
                        SupplierName = "Импортированный поставщик"
                    };
                    _products.Add(product);
                }
            }
        }
    }
}