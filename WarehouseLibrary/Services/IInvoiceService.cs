using System.Collections.Generic;
using System.Numerics;
using WarehouseData.Models;

namespace WarehouseLibrary.Services
{
    public interface IInvoiceService
    {
        List<Invoice> GetAll();
        List<Invoice> GetByWarehouseId(BigInteger warehouseId);
        Invoice GetById(BigInteger id);
        Invoice Create(string type, BigInteger warehouseId, string warehouseName);
        void AddItem(BigInteger invoiceId, string productArticle, string productName, int quantity, decimal price);
        void RemoveItem(BigInteger invoiceId, int itemId);
        bool ProcessInvoice(BigInteger invoiceId, IProductService productService);
        bool Delete(BigInteger id);
    }
}