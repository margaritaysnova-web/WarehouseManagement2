using System;
using System.Collections.Generic;
using System.Numerics;
using WarehouseData.Models;

namespace WarehouseLibrary.Services
{
    public interface IProductService
    {
        List<Product> GetByWarehouseId(BigInteger warehouseId);
        Product GetByArticle(string article);
        Product Create(Product product, BigInteger warehouseId);
        Product Update(string article, Product updatedProduct);
        bool Delete(string article);
        void UpdateStock(string article, int quantityChange);
        void ImportFromCsv(string csvContent, BigInteger warehouseId);
        void SeedTestData(List<Warehouse> warehouses);
    }
}